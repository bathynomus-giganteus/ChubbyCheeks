from __future__ import annotations

import csv
import shutil
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


SOURCE_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopi")
OUT_RESOURCE_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopimanualfix03")
OUT_DIAG_DIR = Path(r"C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\diagnostics\spine_atlas\ChopiManualFix03")
OUT_CODE = "ChopiManualFix03"

# Indices supplied by user from the numbered diagnostic image.
SWAP_INDICES = {
    85, 22, 64, 74, 35, 34, 63, 40, 90, 5,
    27, 83, 84, 7, 6, 28, 25, 32, 20, 23,
    3, 10, 58, 91, 45, 81, 57, 53, 55, 73,
    8, 89, 4, 42, 98, 76, 1, 17,
}


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


def scaled_rect(index: int, props: list[str], sx: float, sy: float):
    x, y, w, h = [int(v.strip()) for v in get_prop(props, "bounds").split(",")]
    nx = round(x * sx)
    ny = round(y * sy)
    nw = round(w * sx)
    nh = round(h * sy)
    if index in SWAP_INDICES:
        # Rotate the rectangle around its top-left anchor: x/y stay fixed, the
        # already-scaled width/height swap.
        sw = nh
        sh = nw
        nw = sw
        nh = sh
    return x, y, w, h, nx, ny, nw, nh


def transparent_rect(image: Image.Image, x: int, y: int, width: int, height: int):
    x0 = max(0, x)
    y0 = max(0, y)
    x1 = min(image.width, x + width)
    y1 = min(image.height, y + height)
    if x1 <= x0 or y1 <= y0:
        return
    clear = Image.new("RGBA", (x1 - x0, y1 - y0), (0, 0, 0, 0))
    image.paste(clear, (x0, y0))


def paste_clipped(base: Image.Image, tile: Image.Image, x: int, y: int):
    x0 = max(0, x)
    y0 = max(0, y)
    x1 = min(base.width, x + tile.width)
    y1 = min(base.height, y + tile.height)
    if x1 <= x0 or y1 <= y0:
        return
    crop = tile.crop((x0 - x, y0 - y, x1 - x, y1 - y))
    base.alpha_composite(crop, (x0, y0))


def main():
    source_atlas = SOURCE_DIR / "Chopi.atlas"
    page_name, page_props, regions = parse_atlas(source_atlas)
    page = Image.open(SOURCE_DIR / page_name).convert("RGBA")

    # User-selected inverse scale anchors:
    # 56 Hair back_1 bottom -> texture bottom; 59 Hair_babo right -> texture right.
    _, ref56_props = regions[55]
    _, ref59_props = regions[58]
    x56, y56, w56, h56 = [int(v.strip()) for v in get_prop(ref56_props, "bounds").split(",")]
    x59, y59, w59, h59 = [int(v.strip()) for v in get_prop(ref59_props, "bounds").split(",")]
    sx = page.width / (x59 + w59)
    sy = page.height / (y56 + h56)

    OUT_RESOURCE_DIR.mkdir(parents=True, exist_ok=True)
    OUT_DIAG_DIR.mkdir(parents=True, exist_ok=True)
    shutil.copy2(SOURCE_DIR / "Chopi.spine-json", OUT_RESOURCE_DIR / f"{OUT_CODE}.spine-json")

    atlas_lines = [f"{OUT_CODE}.png", f"size:{page.width},{page.height}"]
    for prop in page_props:
        if not prop.startswith("size:"):
            atlas_lines.append(prop)

    fixed_page = page.copy()
    diag = page.copy()
    draw = ImageDraw.Draw(diag)
    font = load_font(14)
    colors = ["#ff3b30", "#00c7ff", "#ffd60a", "#32d74b", "#bf5af2"]
    csv_rows = []

    for index, (name, props) in enumerate(regions, start=1):
        ox, oy, ow, oh, nx, ny, nw, nh = scaled_rect(index, props, sx, sy)
        is_swapped = index in SWAP_INDICES
        if is_swapped:
            source_x = round(ox * sx)
            source_y = round(oy * sy)
            source_w = round(ow * sx)
            source_h = round(oh * sy)
            source_tile = page.crop((
                max(0, source_x),
                max(0, source_y),
                min(page.width, source_x + source_w),
                min(page.height, source_y + source_h),
            ))
            # The atlas no longer marks this region as rotated, so rotate the
            # pixels back into the new bounds. `-90` matches the previously
            # better ChopiNoRotateCW candidate.
            rotated_tile = source_tile.rotate(-90, expand=True)
            if rotated_tile.size != (nw, nh):
                rotated_tile = rotated_tile.resize((max(1, nw), max(1, nh)), Image.Resampling.LANCZOS)
            transparent_rect(fixed_page, nx, ny, nw, nh)
            paste_clipped(fixed_page, rotated_tile, nx, ny)

        atlas_lines.append(name)
        atlas_lines.append(f"bounds:{nx},{ny},{nw},{nh}")
        if not is_swapped:
            rotate = get_prop(props, "rotate")
            if rotate:
                atlas_lines.append(f"rotate:{rotate}")
        atlas_lines.extend(without_props(props, {"bounds", "rotate"}))

        color = "#ff3b30" if is_swapped else colors[(index - 1) % len(colors)]
        draw.rectangle((nx, ny, nx + nw, ny + nh), outline=color, width=3 if is_swapped else 1)
        label = str(index)
        bbox = draw.textbbox((0, 0), label, font=font)
        tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
        lx = max(0, min(page.width - tw - 4, nx + 2))
        ly = max(0, min(page.height - th - 4, ny + 2))
        draw.rectangle((lx, ly, lx + tw + 4, ly + th + 3), fill=(0, 0, 0, 185))
        draw.text((lx + 2, ly), label, fill=color, font=font)

        csv_rows.append([
            index,
            name,
            get_prop(props, "rotate") or "false",
            "swap" if is_swapped else "keep",
            ox,
            oy,
            ow,
            oh,
            nx,
            ny,
            nw,
            nh,
        ])

    fixed_page.save(OUT_RESOURCE_DIR / f"{OUT_CODE}.png")
    (OUT_RESOURCE_DIR / f"{OUT_CODE}.atlas").write_text("\n".join(atlas_lines) + "\n", encoding="utf-8")
    diag.save(OUT_DIAG_DIR / f"{OUT_CODE}_numbers_only_overlay.png")
    with (OUT_DIAG_DIR / f"{OUT_CODE}_rects.csv").open("w", encoding="utf-8-sig", newline="") as handle:
        writer = csv.writer(handle)
        writer.writerow([
            "index", "name", "original_rotate", "manual_action",
            "original_x", "original_y", "original_w", "original_h",
            "new_x", "new_y", "new_w", "new_h",
        ])
        writer.writerows(csv_rows)

    print(OUT_RESOURCE_DIR / f"{OUT_CODE}.atlas")
    print(OUT_DIAG_DIR / f"{OUT_CODE}_numbers_only_overlay.png")
    print(OUT_DIAG_DIR / f"{OUT_CODE}_rects.csv")
    print(f"sx={sx} sy={sy} swapped={len(SWAP_INDICES)}")


if __name__ == "__main__":
    main()
