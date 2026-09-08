from __future__ import annotations

import argparse
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


def parse_atlas(atlas_path: Path):
    lines = atlas_path.read_text(encoding="utf-8").splitlines()
    if not lines:
        raise ValueError(f"Empty atlas: {atlas_path}")

    page_name = lines[0].strip()
    regions = []
    i = 1
    while i < len(lines):
        line = lines[i].strip()
        i += 1
        if not line or ":" in line:
            continue

        name = line
        info = {"name": name, "rotate": "false"}
        while i < len(lines):
            sub = lines[i].strip()
            if not sub:
                i += 1
                break
            if ":" not in sub:
                break
            key, value = sub.split(":", 1)
            info[key.strip()] = value.strip()
            i += 1

        if "bounds" in info:
            x, y, w, h = [int(v.strip()) for v in info["bounds"].split(",")]
            info.update({"x": x, "y": y, "w": w, "h": h})
            regions.append(info)

    return page_name, regions


def load_font(size: int):
    candidates = [
        r"C:\Windows\Fonts\msyh.ttc",
        r"C:\Windows\Fonts\simhei.ttf",
        r"C:\Windows\Fonts\arial.ttf",
    ]
    for candidate in candidates:
        p = Path(candidate)
        if p.exists():
            return ImageFont.truetype(str(p), size)
    return ImageFont.load_default()


def crop_region(page: Image.Image, region: dict) -> Image.Image:
    x, y, w, h = region["x"], region["y"], region["w"], region["h"]
    tile = page.crop((x, y, x + w, y + h))
    rotate = str(region.get("rotate", "false")).lower()
    if rotate in {"90", "true"}:
        tile = tile.rotate(90, expand=True)
    elif rotate == "180":
        tile = tile.rotate(180, expand=True)
    elif rotate == "270":
        tile = tile.rotate(270, expand=True)
    return tile


def draw_overlay(page: Image.Image, regions: list[dict], out_path: Path):
    image = page.convert("RGBA")
    draw = ImageDraw.Draw(image)
    font = load_font(13)
    colors = ["#ff4d4d", "#40c4ff", "#ffd740", "#69f0ae", "#ea80fc"]
    for idx, region in enumerate(regions):
        x, y, w, h = region["x"], region["y"], region["w"], region["h"]
        color = colors[idx % len(colors)]
        draw.rectangle((x, y, x + w, y + h), outline=color, width=2)
        label = f"{idx + 1}.{region['name']}"
        bbox = draw.textbbox((0, 0), label, font=font)
        tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
        draw.rectangle((x, max(0, y - th - 3), x + tw + 4, max(th + 3, y)), fill=(0, 0, 0, 170))
        draw.text((x + 2, max(0, y - th - 2)), label, fill=color, font=font)
    image.save(out_path)


def make_contact_sheet(page: Image.Image, regions: list[dict], out_path: Path):
    font = load_font(18)
    small_font = load_font(14)
    cell_w, cell_h = 180, 170
    padding = 12
    cols = 5
    rows = math.ceil(len(regions) / cols)
    sheet = Image.new("RGBA", (cols * cell_w, rows * cell_h), (245, 245, 245, 255))
    draw = ImageDraw.Draw(sheet)

    for idx, region in enumerate(regions):
        col = idx % cols
        row = idx // cols
        x0 = col * cell_w
        y0 = row * cell_h
        draw.rectangle((x0, y0, x0 + cell_w - 1, y0 + cell_h - 1), outline=(210, 210, 210, 255))

        tile = crop_region(page, region)
        max_w = cell_w - padding * 2
        max_h = cell_h - 52
        scale = min(max_w / max(1, tile.width), max_h / max(1, tile.height), 1.0)
        show = tile.resize((max(1, int(tile.width * scale)), max(1, int(tile.height * scale))), Image.Resampling.LANCZOS)
        px = x0 + (cell_w - show.width) // 2
        py = y0 + 8 + (max_h - show.height) // 2
        checker = Image.new("RGBA", show.size, (255, 255, 255, 255))
        cd = ImageDraw.Draw(checker)
        step = 10
        for yy in range(0, show.height, step):
            for xx in range(0, show.width, step):
                if (xx // step + yy // step) % 2:
                    cd.rectangle((xx, yy, xx + step - 1, yy + step - 1), fill=(225, 225, 225, 255))
        checker.alpha_composite(show)
        sheet.alpha_composite(checker, (px, py))

        name = region["name"]
        if len(name) > 18:
            name = name[:17] + "…"
        draw.text((x0 + 6, y0 + cell_h - 40), f"{idx + 1}. {name}", fill=(20, 20, 20, 255), font=small_font)
        draw.text(
            (x0 + 6, y0 + cell_h - 20),
            f"{region['w']}x{region['h']} rot={region.get('rotate', 'false')}",
            fill=(90, 90, 90, 255),
            font=small_font,
        )

    sheet.save(out_path)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("atlas", type=Path)
    parser.add_argument("--out", type=Path, default=None)
    args = parser.parse_args()

    atlas_path = args.atlas
    page_name, regions = parse_atlas(atlas_path)
    page_path = atlas_path.with_name(page_name)
    out_dir = args.out or (atlas_path.parent / "_atlas_debug")
    out_dir.mkdir(parents=True, exist_ok=True)

    page = Image.open(page_path).convert("RGBA")
    full_path = out_dir / f"{atlas_path.stem}_full.png"
    overlay_path = out_dir / f"{atlas_path.stem}_overlay.png"
    sheet_path = out_dir / f"{atlas_path.stem}_slices.png"
    page.save(full_path)
    draw_overlay(page, regions, overlay_path)
    make_contact_sheet(page, regions, sheet_path)
    print(full_path)
    print(overlay_path)
    print(sheet_path)
    print(f"regions={len(regions)}")


if __name__ == "__main__":
    main()
