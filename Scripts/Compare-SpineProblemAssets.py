from __future__ import annotations

import csv
import json
import math
from collections import Counter
from pathlib import Path
from typing import Any

from PIL import Image


ROOT = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST")
OUT_DIR = Path(r"C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\diagnostics\spine_compare")


ASSETS = [
    ("bad", "乔菲立绘", "正常使徒", "Chopi"),
    ("bad", "市长罗涅立绘", "正常使徒", "RohneMayor"),
    ("bad", "欧若拉立绘", "正常使徒", "Aurora"),
    ("bad", "欧若拉动画", "战斗模型", "Aurora"),
    ("bad", "修萝立绘", "正常使徒", "Suro"),
    ("bad", "蕾特立绘", "正常使徒", "Lethe"),
    ("good", "阿莱特立绘", "正常使徒", "Allet"),
    ("good", "埃尔芬王道立绘", "正常使徒", "ErpinRoyale"),
    ("good", "达雅立绘", "正常使徒", "Daya"),
    ("good", "大木头立绘", "正常使徒", "BigWood"),
    ("good", "阿伊拉立绘", "正常使徒", "Ayla"),
    ("good", "贝拉立绘", "正常使徒", "Vela"),
    ("good", "阿梅利亚立绘", "正常使徒", "Amelia"),
]


def find_asset(category: str, code: str) -> Path | None:
    category_dir = ROOT / category
    direct = category_dir / code.lower() / f"{code}.spine-json"
    if direct.exists():
        return direct
    hits = list(category_dir.rglob(f"{code}.spine-json"))
    return hits[0] if hits else None


def parse_atlas(atlas: Path):
    lines = atlas.read_text(encoding="utf-8-sig").splitlines()
    page_name = lines[0].strip() if lines else ""
    props: dict[str, str] = {}
    regions: list[dict[str, Any]] = []
    name: str | None = None
    info: dict[str, str] = {}
    for line in lines[1:]:
        s = line.strip()
        if not s:
            continue
        if ":" not in s:
            if name and "bounds" in info:
                regions.append({"name": name, **info})
            name = s
            info = {}
            continue
        k, v = s.split(":", 1)
        if name is None:
            props[k.strip()] = v.strip()
        else:
            info[k.strip()] = v.strip()
    if name and "bounds" in info:
        regions.append({"name": name, **info})
    return page_name, props, regions


def bounds_nums(region: dict[str, Any]):
    return [int(v.strip()) for v in region["bounds"].split(",")]


def count_skin_attachments(data: dict[str, Any]):
    result = {}
    for skin in data.get("skins", []):
        count = 0
        slots = 0
        types = Counter()
        for slot_attachments in skin.get("attachments", {}).values():
            slots += 1
            for attachment in slot_attachments.values():
                count += 1
                types[attachment.get("type", "region")] += 1
        result[skin.get("name", "<unnamed>")] = {
            "slots": slots,
            "attachments": count,
            "types": dict(types),
        }
    return result


def iter_attachments(data: dict[str, Any]):
    for skin in data.get("skins", []):
        skin_name = skin.get("name", "")
        for slot_name, slot_attachments in skin.get("attachments", {}).items():
            for attachment_name, attachment in slot_attachments.items():
                yield skin_name, slot_name, attachment_name, attachment


def vertices_kind(attachment: dict[str, Any]):
    if attachment.get("type") != "mesh":
        return ""
    vertices = attachment.get("vertices", [])
    uvs = attachment.get("uvs", [])
    return "weighted" if len(vertices) > len(uvs) else "plain"


def analyze_asset(group: str, label: str, category: str, code: str):
    json_path = find_asset(category, code)
    if not json_path:
        return {"group": group, "label": label, "category": category, "code": code, "missing": True}

    atlas_path = json_path.with_suffix(".atlas")
    if not atlas_path.exists():
        atlas_path = json_path.with_name(f"{code}.atlas")
    data = json.loads(json_path.read_text(encoding="utf-8-sig"))
    page_name, atlas_props, regions = parse_atlas(atlas_path)
    png_path = atlas_path.with_name(page_name)
    png_size = Image.open(png_path).size if png_path.exists() else None

    declared_size = None
    if "size" in atlas_props:
        declared_size = tuple(int(v.strip()) for v in atlas_props["size"].split(","))
    max_extent = None
    if regions:
        max_extent = (
            max(bounds_nums(r)[0] + bounds_nums(r)[2] for r in regions),
            max(bounds_nums(r)[1] + bounds_nums(r)[3] for r in regions),
        )
    rotate_counts = Counter(str(r.get("rotate", "false")).lower() for r in regions)
    offset_count = sum(1 for r in regions if "offsets" in r)

    attachment_types = Counter()
    mesh_kinds = Counter()
    mesh_vertices_counts = []
    mesh_uv_counts = []
    missing_region_names = []
    atlas_names = {r["name"] for r in regions}
    mesh_count = 0
    for _, _, attachment_name, attachment in iter_attachments(data):
        typ = attachment.get("type", "region")
        attachment_types[typ] += 1
        if typ == "mesh":
            mesh_count += 1
            mesh_kinds[vertices_kind(attachment)] += 1
            mesh_vertices_counts.append(len(attachment.get("vertices", [])))
            mesh_uv_counts.append(len(attachment.get("uvs", [])))
        path_name = attachment.get("path") or attachment_name
        if typ in {"region", "mesh", "linkedmesh"} and path_name not in atlas_names:
            missing_region_names.append(path_name)

    transforms = data.get("transform", []) or data.get("transformConstraints", [])
    path_constraints = data.get("path", []) or data.get("pathConstraints", [])
    physics = data.get("physics", []) or data.get("physicsConstraints", [])
    ik = data.get("ik", []) or data.get("ikConstraints", [])
    deform_anims = []
    draworder_anims = []
    for anim_name, anim in data.get("animations", {}).items():
        if anim.get("deform") or anim.get("ffd"):
            deform_anims.append(anim_name)
        if anim.get("drawOrder") or anim.get("draworder"):
            draworder_anims.append(anim_name)

    return {
        "group": group,
        "label": label,
        "category": category,
        "code": code,
        "json": str(json_path),
        "atlas": str(atlas_path),
        "png": str(png_path),
        "skeleton_version": data.get("skeleton", {}).get("spine"),
        "declared_size": declared_size,
        "png_size": png_size,
        "max_extent": max_extent,
        "size_matches_png": declared_size == png_size,
        "max_extent_over_declared": (
            max_extent[0] > declared_size[0] or max_extent[1] > declared_size[1]
            if declared_size and max_extent
            else None
        ),
        "max_extent_over_png": (
            max_extent[0] > png_size[0] or max_extent[1] > png_size[1]
            if png_size and max_extent
            else None
        ),
        "region_count": len(regions),
        "rotate_counts": dict(rotate_counts),
        "offset_count": offset_count,
        "skin_summary": count_skin_attachments(data),
        "attachment_types": dict(attachment_types),
        "mesh_kinds": dict(mesh_kinds),
        "mesh_vertices_minmax": (
            min(mesh_vertices_counts) if mesh_vertices_counts else 0,
            max(mesh_vertices_counts) if mesh_vertices_counts else 0,
        ),
        "mesh_uv_minmax": (
            min(mesh_uv_counts) if mesh_uv_counts else 0,
            max(mesh_uv_counts) if mesh_uv_counts else 0,
        ),
        "missing_region_ref_count": len(set(missing_region_names)),
        "missing_region_refs_sample": sorted(set(missing_region_names))[:20],
        "ik_count": len(ik),
        "transform_count": len(transforms),
        "path_constraint_count": len(path_constraints),
        "physics_count": len(physics),
        "deform_anim_count": len(deform_anims),
        "deform_anims_sample": deform_anims[:20],
        "draworder_anim_count": len(draworder_anims),
        "draworder_anims_sample": draworder_anims[:20],
    }


def flatten_for_csv(item: dict[str, Any]):
    return {
        "group": item.get("group"),
        "label": item.get("label"),
        "category": item.get("category"),
        "code": item.get("code"),
        "missing": item.get("missing", False),
        "version": item.get("skeleton_version"),
        "declared_size": item.get("declared_size"),
        "png_size": item.get("png_size"),
        "max_extent": item.get("max_extent"),
        "size_matches_png": item.get("size_matches_png"),
        "max_extent_over_declared": item.get("max_extent_over_declared"),
        "max_extent_over_png": item.get("max_extent_over_png"),
        "region_count": item.get("region_count"),
        "rotate_counts": item.get("rotate_counts"),
        "offset_count": item.get("offset_count"),
        "attachment_types": item.get("attachment_types"),
        "mesh_kinds": item.get("mesh_kinds"),
        "mesh_vertices_minmax": item.get("mesh_vertices_minmax"),
        "mesh_uv_minmax": item.get("mesh_uv_minmax"),
        "missing_region_ref_count": item.get("missing_region_ref_count"),
        "ik_count": item.get("ik_count"),
        "transform_count": item.get("transform_count"),
        "path_constraint_count": item.get("path_constraint_count"),
        "physics_count": item.get("physics_count"),
        "deform_anim_count": item.get("deform_anim_count"),
        "draworder_anim_count": item.get("draworder_anim_count"),
        "json": item.get("json"),
    }


def main():
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    results = [analyze_asset(*asset) for asset in ASSETS]

    json_path = OUT_DIR / "spine_asset_compare.json"
    csv_path = OUT_DIR / "spine_asset_compare.csv"
    md_path = OUT_DIR / "spine_asset_compare.md"
    json_path.write_text(json.dumps(results, ensure_ascii=False, indent=2), encoding="utf-8")
    rows = [flatten_for_csv(r) for r in results]
    with csv_path.open("w", encoding="utf-8-sig", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0].keys()))
        writer.writeheader()
        writer.writerows(rows)

    lines = ["# Spine problem asset comparison", ""]
    lines.append("| group | label | code | category | atlas size | png size | max extent | size ok | max>declared | types | mesh kind | skins | constraints |")
    lines.append("|---|---|---|---|---:|---:|---:|---|---|---|---|---|---|")
    for r in results:
        if r.get("missing"):
            lines.append(f"| {r['group']} | {r['label']} | {r['code']} | {r['category']} | missing | | | | | | | | |")
            continue
        skins = "; ".join(
            f"{k}:{v['attachments']}" for k, v in r["skin_summary"].items()
        )
        constraints = f"ik{r['ik_count']}/tr{r['transform_count']}/path{r['path_constraint_count']}/phys{r['physics_count']}"
        lines.append(
            f"| {r['group']} | {r['label']} | {r['code']} | {r['category']} | "
            f"{r['declared_size']} | {r['png_size']} | {r['max_extent']} | "
            f"{r['size_matches_png']} | {r['max_extent_over_declared']} | "
            f"{r['attachment_types']} | {r['mesh_kinds']} | {skins} | {constraints} |"
        )
    lines.append("")
    lines.append("## Notable findings")
    bad = [r for r in results if r.get("group") == "bad" and not r.get("missing")]
    good = [r for r in results if r.get("group") == "good" and not r.get("missing")]
    for r in bad:
        flags = []
        if not r["size_matches_png"]:
            flags.append("atlas declared size differs from PNG")
        if r["max_extent_over_declared"]:
            flags.append("region bounds exceed declared atlas size")
        if r["max_extent_over_png"]:
            flags.append("region bounds exceed actual PNG")
        if r["attachment_types"].get("mesh", 0) > 0 and r["attachment_types"].get("region", 0) == 0:
            flags.append("almost/all mesh attachments")
        if r["skin_summary"].get("Normal", {}).get("attachments", 1) == 0:
            flags.append("Normal skin has zero attachments")
        lines.append(f"- {r['label']} / {r['code']}: " + ("; ".join(flags) if flags else "no obvious structural flag"))
    lines.append("")
    lines.append("Good samples included: " + ", ".join(f"{r['label']}({r['code']})" for r in good))
    md_path.write_text("\n".join(lines), encoding="utf-8")

    print(json_path)
    print(csv_path)
    print(md_path)
    for r in results:
        print(r["group"], r["label"], r["code"], "sizeOk", r.get("size_matches_png"), "types", r.get("attachment_types"), "skins", r.get("skin_summary"))


if __name__ == "__main__":
    main()
