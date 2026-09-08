from __future__ import annotations

import csv
import json
import shutil
from pathlib import Path
from typing import Any

from PIL import Image, ImageDraw, ImageFont


SOURCE_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopi")
OUT_RESOURCE_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopiallrotateuva")
OUT_DIAG_DIR = Path(r"C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\diagnostics\spine_atlas\ChopiAllRotateUvA")
OUT_CODE = "ChopiAllRotateUvA"


def load_font(size: int):
    for candidate in [
        r"C:\Windows\Fonts\arialbd.ttf",
        r"C:\Windows\Fonts\msyhbd.ttc",
        r"C:\Windows\Fonts\msyh.ttc",
        r"C:\Windows\Fonts\arial.ttf",
    ]:
        path = Path(candidate)
        if path.exists():
            return ImageFont.truetype(str(path), size)
    return ImageFont.load_default()


def parse_atlas(atlas_path: Path):
    lines = atlas_path.read_text(encoding="utf-8-sig").splitlines()
    page_name = lines[0].strip()
    page_props: list[str] = []
    regions: list[tuple[str, list[str]]] = []
    name: str | None = None
    props: list[str] = []

    for line in lines[1:]:
        stripped = line.strip()
        if not stripped:
            continue
        if ":" not in stripped:
            if name and any(p.startswith("bounds:") for p in props):
                regions.append((name, props.copy()))
            name = stripped
            props = []
            continue
        if name is None:
            page_props.append(stripped)
        else:
            props.append(stripped)

    if name and any(p.startswith("bounds:") for p in props):
        regions.append((name, props.copy()))

    return page_name, page_props, regions


def get_prop(props: list[str], key: str):
    prefix = f"{key}:"
    for prop in props:
        if prop.startswith(prefix):
            return prop[len(prefix):].strip()
    return None


def without_props(props: list[str], remove: set[str]):
    prefixes = tuple(f"{key}:" for key in remove)
    return [prop for prop in props if not prop.startswith(prefixes)]


def transform_uvs(uvs: list[Any]) -> None:
    for i in range(0, len(uvs), 2):
        u = float(uvs[i])
        v = float(uvs[i + 1])
        uvs[i] = max(0.0, min(1.0, v))
        uvs[i + 1] = max(0.0, min(1.0, 1.0 - u))


def iter_attachments(data: dict[str, Any]):
    for skin in data.get("skins", []):
        for slot_name, slot_attachments in skin.get("attachments", {}).items():
            for attachment_name, attachment in slot_attachments.items():
                yield slot_name, attachment_name, attachment


def transparent_rect(image: Image.Image, x: int, y: int, width: int, height: int) -> None:
    x0 = max(0, x)
    y0 = max(0, y)
    x1 = min(image.width, x + width)
    y1 = min(image.height, y + height)
    if x1 <= x0 or y1 <= y0:
        return
    image.paste(Image.new("RGBA", (x1 - x0, y1 - y0), (0, 0, 0, 0)), (x0, y0))


def paste_clipped(base: Image.Image, tile: Image.Image, x: int, y: int) -> None:
    x0 = max(0, x)
    y0 = max(0, y)
    x1 = min(base.width, x + tile.width)
    y1 = min(base.height, y + tile.height)
    if x1 <= x0 or y1 <= y0:
        return
    crop = tile.crop((x0 - x, y0 - y, x1 - x, y1 - y))
    base.alpha_composite(crop, (x0, y0))


def main() -> None:
    page_name, page_props, regions = parse_atlas(SOURCE_DIR / "Chopi.atlas")
    page = Image.open(SOURCE_DIR / page_name).convert("RGBA")
    rotate_indices = {
        index
        for index, (_name, props) in enumerate(regions, start=1)
        if get_prop(props, "rotate") == "90"
    }
    rotate_names = {regions[index - 1][0] for index in rotate_indices}

    # Same user-guided inverse-scale anchors as ManualFix03.
    _, ref56_props = regions[55]
    _, ref59_props = regions[58]
    x56, y56, w56, h56 = [int(v.strip()) for v in get_prop(ref56_props, "bounds").split(",")]
    x59, y59, w59, h59 = [int(v.strip()) for v in get_prop(ref59_props, "bounds").split(",")]
    sx = page.width / (x59 + w59)
    sy = page.height / (y56 + h56)

    fixed_page = page.copy()
    atlas_lines = [f"{OUT_CODE}.png", f"size:{page.width},{page.height}"]
    atlas_lines.extend(prop for prop in page_props if not prop.startswith("size:"))

    OUT_RESOURCE_DIR.mkdir(parents=True, exist_ok=True)
    OUT_DIAG_DIR.mkdir(parents=True, exist_ok=True)

    diag = page.copy()
    draw = ImageDraw.Draw(diag)
    font = load_font(14)
    rows = []
    for index, (name, props) in enumerate(regions, start=1):
        ox, oy, ow, oh = [int(v.strip()) for v in get_prop(props, "bounds").split(",")]
        nx = round(ox * sx)
        ny = round(oy * sy)
        nw = round(ow * sx)
        nh = round(oh * sy)
        is_rotated = index in rotate_indices
        if is_rotated:
            source_tile = page.crop((
                max(0, nx),
                max(0, ny),
                min(page.width, nx + nw),
                min(page.height, ny + nh),
            ))
            nw, nh = nh, nw
            rotated_tile = source_tile.rotate(-90, expand=True)
            if rotated_tile.size != (nw, nh):
                rotated_tile = rotated_tile.resize((max(1, nw), max(1, nh)), Image.Resampling.LANCZOS)
            transparent_rect(fixed_page, nx, ny, nw, nh)
            paste_clipped(fixed_page, rotated_tile, nx, ny)

        atlas_lines.append(name)
        atlas_lines.append(f"bounds:{nx},{ny},{nw},{nh}")
        atlas_lines.extend(without_props(props, {"bounds", "rotate"}))

        color = "#ff3b30" if is_rotated else "#00c7ff"
        draw.rectangle((nx, ny, nx + nw, ny + nh), outline=color, width=2 if is_rotated else 1)
        label = str(index)
        bbox = draw.textbbox((0, 0), label, font=font)
        tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
        lx = max(0, min(page.width - tw - 4, nx + 2))
        ly = max(0, min(page.height - th - 4, ny + 2))
        draw.rectangle((lx, ly, lx + tw + 4, ly + th + 3), fill=(0, 0, 0, 185))
        draw.text((lx + 2, ly), label, fill=color, font=font)
        rows.append([index, name, "rotate90" if is_rotated else "keep", nx, ny, nw, nh])

    data = json.loads((SOURCE_DIR / "Chopi.spine-json").read_text(encoding="utf-8-sig"))
    changed = []
    for slot_name, attachment_name, attachment in iter_attachments(data):
        if attachment.get("type") != "mesh" or "uvs" not in attachment:
            continue
        path_name = attachment.get("path") or attachment_name
        if path_name not in rotate_names:
            continue
        transform_uvs(attachment["uvs"])
        changed.append((slot_name, attachment_name, path_name))
    data.setdefault("skeleton", {})["hash"] = f"{data.get('skeleton', {}).get('hash', '')}-cult-all-rotate-uv-a"

    fixed_page.save(OUT_RESOURCE_DIR / f"{OUT_CODE}.png")
    (OUT_RESOURCE_DIR / f"{OUT_CODE}.atlas").write_text("\n".join(atlas_lines) + "\n", encoding="utf-8")
    (OUT_RESOURCE_DIR / f"{OUT_CODE}.spine-json").write_text(
        json.dumps(data, ensure_ascii=False, separators=(",", ":")),
        encoding="utf-8",
    )
    diag.save(OUT_DIAG_DIR / f"{OUT_CODE}_numbers_only_overlay.png")
    with (OUT_DIAG_DIR / f"{OUT_CODE}_rects.csv").open("w", encoding="utf-8-sig", newline="") as handle:
        writer = csv.writer(handle)
        writer.writerow(["index", "name", "action", "x", "y", "w", "h"])
        writer.writerows(rows)

    print(OUT_RESOURCE_DIR)
    print(f"rotate_regions={len(rotate_indices)} changed_mesh_attachments={len(changed)}")
    print(OUT_DIAG_DIR / f"{OUT_CODE}_numbers_only_overlay.png")


if __name__ == "__main__":
    main()
