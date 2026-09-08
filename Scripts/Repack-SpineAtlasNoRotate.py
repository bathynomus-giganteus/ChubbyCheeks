from __future__ import annotations

import argparse
import shutil
from pathlib import Path

from PIL import Image


def parse_atlas(atlas_path: Path):
    lines = atlas_path.read_text(encoding="utf-8-sig").splitlines()
    if not lines:
        raise ValueError(f"Empty atlas: {atlas_path}")

    page_name = lines[0].strip()
    page_props: list[str] = []
    regions: list[tuple[str, list[str]]] = []
    i = 1
    while i < len(lines):
        line = lines[i]
        stripped = line.strip()
        if not stripped:
            i += 1
            continue
        if ":" in stripped:
            page_props.append(stripped)
            i += 1
            continue

        name = stripped
        props: list[str] = []
        i += 1
        while i < len(lines):
            prop = lines[i].strip()
            if not prop:
                i += 1
                break
            if ":" not in prop:
                break
            props.append(prop)
            i += 1
        regions.append((name, props))

    return page_name, page_props, regions


def get_prop(props: list[str], key: str) -> str | None:
    prefix = f"{key}:"
    for prop in props:
        if prop.startswith(prefix):
            return prop[len(prefix) :].strip()
    return None


def without_props(props: list[str], keys: set[str]) -> list[str]:
    prefixes = tuple(f"{key}:" for key in keys)
    return [prop for prop in props if not prop.startswith(prefixes)]


def rotate_region(tile: Image.Image, rotate_value: str | None, clockwise: bool) -> Image.Image:
    if not rotate_value:
        return tile
    value = rotate_value.strip().lower()
    if value in {"true", "90"}:
        return tile.rotate(-90 if clockwise else 90, expand=True)
    if value == "270":
        return tile.rotate(90 if clockwise else -90, expand=True)
    if value == "180":
        return tile.rotate(180, expand=True)
    return tile


def repack(
    atlas_path: Path,
    out_dir: Path,
    out_code: str,
    *,
    width: int,
    margin: int,
    clockwise: bool,
):
    page_name, page_props, regions = parse_atlas(atlas_path)
    page = Image.open(atlas_path.with_name(page_name)).convert("RGBA")

    entries = []
    x = margin
    y = margin
    row_height = 0
    canvas_height = margin

    for name, props in regions:
        bounds = get_prop(props, "bounds")
        if not bounds:
            continue
        bx, by, bw, bh = [int(v.strip()) for v in bounds.split(",")]
        tile = page.crop((bx, by, bx + bw, by + bh))
        tile = rotate_region(tile, get_prop(props, "rotate"), clockwise)
        tw, th = tile.size

        if x + tw + margin > width:
            x = margin
            y += row_height + margin
            row_height = 0

        entries.append((name, props, tile, x, y, tw, th))
        x += tw + margin
        row_height = max(row_height, th)
        canvas_height = max(canvas_height, y + th + margin)

    out_dir.mkdir(parents=True, exist_ok=True)
    canvas = Image.new("RGBA", (width, canvas_height), (0, 0, 0, 0))
    for _, _, tile, x, y, _, _ in entries:
        canvas.alpha_composite(tile, (x, y))

    out_png = out_dir / f"{out_code}.png"
    out_atlas = out_dir / f"{out_code}.atlas"
    out_json = out_dir / f"{out_code}.spine-json"
    canvas.save(out_png)

    out_lines = [f"{out_code}.png", f"size:{width},{canvas_height}"]
    for prop in page_props:
        if not prop.startswith("size:"):
            out_lines.append(prop)

    for name, props, _, x, y, tw, th in entries:
        out_lines.append(name)
        out_lines.append(f"bounds:{x},{y},{tw},{th}")
        out_lines.extend(without_props(props, {"bounds", "rotate"}))

    out_atlas.write_text("\n".join(out_lines) + "\n", encoding="utf-8")

    source_json = atlas_path.with_suffix(".spine-json")
    if source_json.exists():
        shutil.copy2(source_json, out_json)

    print(out_atlas)
    print(out_png)
    print(out_json if out_json.exists() else "no spine-json copied")
    print(f"regions={len(entries)} size={width}x{canvas_height} clockwise={clockwise}")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("atlas", type=Path)
    parser.add_argument("--out-dir", type=Path, required=True)
    parser.add_argument("--out-code", required=True)
    parser.add_argument("--width", type=int, default=1024)
    parser.add_argument("--margin", type=int, default=2)
    parser.add_argument("--ccw", action="store_true", help="Use the opposite 90-degree rotation direction.")
    args = parser.parse_args()

    repack(
        args.atlas,
        args.out_dir,
        args.out_code,
        width=args.width,
        margin=args.margin,
        clockwise=not args.ccw,
    )


if __name__ == "__main__":
    main()
