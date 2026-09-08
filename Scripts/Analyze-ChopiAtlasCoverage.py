from __future__ import annotations

import csv
from pathlib import Path

from PIL import Image


ATLAS = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopimanualuvfixa\ChopiManualUvFixA.atlas")
PNG = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopimanualuvfixa\ChopiManualUvFixA.png")
OUT = Path(r"C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\diagnostics\spine_atlas\ChopiManualUvFixA_coverage.csv")


def parse_atlas(path: Path):
    lines = path.read_text(encoding="utf-8-sig").splitlines()
    regions = []
    name = None
    props = {}
    for line in lines[1:]:
        stripped = line.strip()
        if not stripped:
            continue
        if ":" not in stripped:
            if name and "bounds" in props:
                regions.append((name, props))
            name = stripped
            props = {}
        else:
            key, value = stripped.split(":", 1)
            if name:
                props[key.strip()] = value.strip()
    if name and "bounds" in props:
        regions.append((name, props))
    return regions


def alpha_bbox(image: Image.Image, x: int, y: int, w: int, h: int):
    x0 = max(0, x)
    y0 = max(0, y)
    x1 = min(image.width, x + w)
    y1 = min(image.height, y + h)
    if x1 <= x0 or y1 <= y0:
        return None
    crop = image.crop((x0, y0, x1, y1))
    alpha = crop.getchannel("A")
    bbox = alpha.getbbox()
    if bbox is None:
        return None
    return (x0 + bbox[0], y0 + bbox[1], x0 + bbox[2], y0 + bbox[3])


def main() -> None:
    image = Image.open(PNG).convert("RGBA")
    rows = []
    for index, (name, props) in enumerate(parse_atlas(ATLAS), start=1):
        x, y, w, h = [int(v.strip()) for v in props["bounds"].split(",")]
        bbox = alpha_bbox(image, x, y, w, h)
        region_area = max(0, min(image.width, x + w) - max(0, x)) * max(0, min(image.height, y + h) - max(0, y))
        if bbox is None:
            alpha_area = 0
            bbox_text = ""
            edge_touch = ""
        else:
            bx0, by0, bx1, by1 = bbox
            alpha_area = (bx1 - bx0) * (by1 - by0)
            bbox_text = f"{bx0},{by0},{bx1 - bx0},{by1 - by0}"
            edge_touch = " ".join([
                flag for flag, hit in [
                    ("left", bx0 <= x),
                    ("top", by0 <= y),
                    ("right", bx1 >= x + w),
                    ("bottom", by1 >= y + h),
                ] if hit
            ])
        rows.append({
            "index": index,
            "name": name,
            "bounds": props["bounds"],
            "rotate": props.get("rotate", ""),
            "alpha_bbox": bbox_text,
            "region_area": region_area,
            "alpha_bbox_area": alpha_area,
            "fill_ratio": round(alpha_area / region_area, 4) if region_area else 0,
            "edge_touch": edge_touch,
            "out_of_bounds": x < 0 or y < 0 or x + w > image.width or y + h > image.height,
        })

    OUT.parent.mkdir(parents=True, exist_ok=True)
    with OUT.open("w", encoding="utf-8-sig", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0].keys()))
        writer.writeheader()
        writer.writerows(rows)
    print(OUT)
    for row in rows:
        if row["out_of_bounds"] or row["fill_ratio"] < 0.08 or row["edge_touch"]:
            print(row)


if __name__ == "__main__":
    main()
