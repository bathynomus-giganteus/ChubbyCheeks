from __future__ import annotations

import json
import shutil
from pathlib import Path
from typing import Any, Callable


SOURCE_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopi")
OUT_BASE = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒")

TARGET_INDICES = {
    85, 22, 64, 74, 35, 34, 63, 40, 90, 5,
    27, 83, 84, 7, 6, 28, 25, 32, 20, 23,
    3, 10, 58, 91, 45, 81, 57, 53, 55, 73,
    8, 89, 4, 42, 98, 76, 1, 17,
}


def parse_atlas_names(atlas_path: Path):
    lines = atlas_path.read_text(encoding="utf-8-sig").splitlines()
    regions: list[str] = []
    name: str | None = None
    props: list[str] = []
    for line in lines[1:]:
        stripped = line.strip()
        if not stripped:
            continue
        if ":" not in stripped:
            if name and any(prop.startswith("bounds:") for prop in props):
                regions.append(name)
            name = stripped
            props = []
        else:
            if name is not None:
                props.append(stripped)
    if name and any(prop.startswith("bounds:") for prop in props):
        regions.append(name)
    return regions


def transform_uvs(uvs: list[Any], transform: Callable[[float, float], tuple[float, float]]):
    for i in range(0, len(uvs), 2):
        u = float(uvs[i])
        v = float(uvs[i + 1])
        nu, nv = transform(u, v)
        uvs[i] = max(0.0, min(1.0, nu))
        uvs[i + 1] = max(0.0, min(1.0, nv))


def iter_attachments(data: dict[str, Any]):
    for skin in data.get("skins", []):
        for slot_name, slot_attachments in skin.get("attachments", {}).items():
            for attachment_name, attachment in slot_attachments.items():
                yield slot_name, attachment_name, attachment


def make_variant(code: str, transform: Callable[[float, float], tuple[float, float]]):
    atlas_names = parse_atlas_names(SOURCE_DIR / "Chopi.atlas")
    target_names = {atlas_names[index - 1] for index in TARGET_INDICES}

    data = json.loads((SOURCE_DIR / "Chopi.spine-json").read_text(encoding="utf-8-sig"))
    changed = []
    for slot_name, attachment_name, attachment in iter_attachments(data):
        if attachment.get("type") != "mesh":
            continue
        path_name = attachment.get("path") or attachment_name
        if path_name not in target_names:
            continue
        if "uvs" not in attachment:
            continue
        transform_uvs(attachment["uvs"], transform)
        changed.append((slot_name, attachment_name, path_name))

    data.setdefault("skeleton", {})["hash"] = f"{data.get('skeleton', {}).get('hash', '')}-cult-uv-{code}"
    out_dir = OUT_BASE / code.lower()
    out_dir.mkdir(parents=True, exist_ok=True)
    (out_dir / f"{code}.spine-json").write_text(
        json.dumps(data, ensure_ascii=False, separators=(",", ":")),
        encoding="utf-8",
    )
    atlas = (SOURCE_DIR / "Chopi.atlas").read_text(encoding="utf-8-sig")
    atlas = atlas.replace("Chopi.png", f"{code}.png", 1)
    (out_dir / f"{code}.atlas").write_text(atlas, encoding="utf-8")
    shutil.copy2(SOURCE_DIR / "Chopi.png", out_dir / f"{code}.png")
    print(code, out_dir, "target_regions", len(target_names), "changed_mesh_attachments", len(changed))
    print("changed sample", changed[:12])


def main():
    make_variant("ChopiUvFixA", lambda u, v: (v, 1.0 - u))
    make_variant("ChopiUvFixB", lambda u, v: (1.0 - v, u))


if __name__ == "__main__":
    main()
