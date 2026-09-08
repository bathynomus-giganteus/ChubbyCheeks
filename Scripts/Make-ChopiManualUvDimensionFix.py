from __future__ import annotations

import json
import shutil
from pathlib import Path
from typing import Any


SOURCE_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopimanualuvfixa")
OUT_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopimanualuvdima")
OUT_CODE = "ChopiManualUvDimA"

TARGET_INDICES = {
    85, 22, 64, 74, 35, 34, 63, 40, 90, 5,
    27, 83, 84, 7, 6, 28, 25, 32, 20, 23,
    3, 10, 58, 91, 45, 81, 57, 53, 55, 73,
    8, 89, 4, 42, 98, 76, 1, 17,
}


def parse_atlas_names(atlas_path: Path) -> list[str]:
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
        elif name is not None:
            props.append(stripped)
    if name and any(prop.startswith("bounds:") for prop in props):
        regions.append(name)
    return regions


def iter_attachments(data: dict[str, Any]):
    for skin in data.get("skins", []):
        for slot_name, slot_attachments in skin.get("attachments", {}).items():
            for attachment_name, attachment in slot_attachments.items():
                yield slot_name, attachment_name, attachment


def main() -> None:
    atlas_names = parse_atlas_names(SOURCE_DIR / "ChopiManualUvFixA.atlas")
    target_names = {atlas_names[index - 1] for index in TARGET_INDICES}

    data = json.loads((SOURCE_DIR / "ChopiManualUvFixA.spine-json").read_text(encoding="utf-8-sig"))
    changed = []
    for slot_name, attachment_name, attachment in iter_attachments(data):
        if attachment.get("type") != "mesh":
            continue
        path_name = attachment.get("path") or attachment_name
        if path_name not in target_names:
            continue
        if isinstance(attachment.get("width"), (int, float)) and isinstance(attachment.get("height"), (int, float)):
            attachment["width"], attachment["height"] = attachment["height"], attachment["width"]
            changed.append((slot_name, attachment_name, path_name, attachment["width"], attachment["height"]))

    data.setdefault("skeleton", {})["hash"] = f"{data.get('skeleton', {}).get('hash', '')}-cult-manual-uv-dim-a"

    OUT_DIR.mkdir(parents=True, exist_ok=True)
    (OUT_DIR / f"{OUT_CODE}.spine-json").write_text(
        json.dumps(data, ensure_ascii=False, separators=(",", ":")),
        encoding="utf-8",
    )
    atlas = (SOURCE_DIR / "ChopiManualUvFixA.atlas").read_text(encoding="utf-8-sig")
    atlas = atlas.replace("ChopiManualUvFixA.png", f"{OUT_CODE}.png", 1)
    (OUT_DIR / f"{OUT_CODE}.atlas").write_text(atlas, encoding="utf-8")
    shutil.copy2(SOURCE_DIR / "ChopiManualUvFixA.png", OUT_DIR / f"{OUT_CODE}.png")

    print(OUT_DIR)
    print(f"changed_mesh_dimensions={len(changed)}")
    print(changed[:16])


if __name__ == "__main__":
    main()
