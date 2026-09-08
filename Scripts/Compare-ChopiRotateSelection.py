from __future__ import annotations

from pathlib import Path


ATLAS = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopi\Chopi.atlas")
SELECTED = {
    85, 22, 64, 74, 35, 34, 63, 40, 90, 5,
    27, 83, 84, 7, 6, 28, 25, 32, 20, 23,
    3, 10, 58, 91, 45, 81, 57, 53, 55, 73,
    8, 89, 4, 42, 98, 76, 1, 17,
}


def parse_atlas(path: Path):
    lines = path.read_text(encoding="utf-8-sig").splitlines()
    regions = []
    name = None
    props = []
    for line in lines[1:]:
        stripped = line.strip()
        if not stripped:
            continue
        if ":" not in stripped:
            if name and any(prop.startswith("bounds:") for prop in props):
                regions.append((name, props))
            name = stripped
            props = []
        elif name is not None:
            props.append(stripped)
    if name and any(prop.startswith("bounds:") for prop in props):
        regions.append((name, props))
    return regions


def main() -> None:
    regions = parse_atlas(ATLAS)
    rotate90 = {
        index
        for index, (_name, props) in enumerate(regions, start=1)
        if any(prop.startswith("rotate:90") for prop in props)
    }

    print("rotate90_count", len(rotate90), sorted(rotate90))
    print("selected_count", len(SELECTED), sorted(SELECTED))
    print("rotate90_not_selected")
    for index in sorted(rotate90 - SELECTED):
        print(index, regions[index - 1][0])
    print("selected_not_rotate90")
    for index in sorted(SELECTED - rotate90):
        print(index, regions[index - 1][0])


if __name__ == "__main__":
    main()
