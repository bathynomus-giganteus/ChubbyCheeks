from __future__ import annotations

import json
import shutil
from pathlib import Path
from typing import Any


SOURCE_DIR = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopi")
OUT_BASE = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒")


VARIANTS = {
    # Derived from actual PNG size / atlas declared page size.
    "ChopiSkelScaleXyUp": (1024 / 913, 1024 / 898),
    "ChopiSkelScaleXyDown": (913 / 1024, 898 / 1024),
    # Approximate PR-style single atlas scale candidates.
    "ChopiSkelScaleUniformUp": ((1024 / 913 + 1024 / 898) / 2, (1024 / 913 + 1024 / 898) / 2),
    "ChopiSkelScaleUniformDown": ((913 / 1024 + 898 / 1024) / 2, (913 / 1024 + 898 / 1024) / 2),
}


def scale_vertices(values: list[Any], sx: float, sy: float, uvs_len: int | None = None):
    if not values:
        return
    weighted = uvs_len is not None and len(values) > uvs_len
    if weighted:
        i = 0
        while i < len(values):
            bone_count = int(values[i])
            i += 1
            for _ in range(bone_count):
                i += 1  # bone index
                i += 1  # weight
                if i < len(values):
                    values[i] *= sx
                    i += 1
                if i < len(values):
                    values[i] *= sy
                    i += 1
    else:
        for i in range(0, len(values), 2):
            values[i] *= sx
            if i + 1 < len(values):
                values[i + 1] *= sy


def scale_attachment(attachment: dict[str, Any], sx: float, sy: float):
    typ = attachment.get("type", "region")
    if typ == "region":
        for key, scale in [("x", sx), ("y", sy), ("width", sx), ("height", sy)]:
            if key in attachment:
                attachment[key] *= scale
    elif typ in {"mesh", "linkedmesh"}:
        if "width" in attachment:
            attachment["width"] *= sx
        if "height" in attachment:
            attachment["height"] *= sy
        if typ == "mesh" and "vertices" in attachment:
            scale_vertices(attachment["vertices"], sx, sy, len(attachment.get("uvs", [])))
    elif typ in {"boundingbox", "path", "clipping"}:
        if "vertices" in attachment:
            scale_vertices(attachment["vertices"], sx, sy)
        if typ == "path" and "lengths" in attachment:
            uniform = (sx + sy) / 2
            attachment["lengths"] = [v * uniform for v in attachment["lengths"]]
    elif typ == "point":
        if "x" in attachment:
            attachment["x"] *= sx
        if "y" in attachment:
            attachment["y"] *= sy


def scale_json(data: dict[str, Any], sx: float, sy: float):
    # PR #29 scales bone x/y/length, root scale inverse. Keep that behavior for
    # uniform variants; for anisotropic variants length uses average scale.
    uniform = (sx + sy) / 2
    inv_uniform = 1 / uniform if uniform else 1

    for bone in data.get("bones", []):
        if "x" in bone:
            bone["x"] *= sx
        if "y" in bone:
            bone["y"] *= sy
        if "length" in bone:
            bone["length"] *= uniform

    for bone in data.get("bones", []):
        if "parent" not in bone:
            bone["scaleX"] = bone.get("scaleX", 1) * inv_uniform
            bone["scaleY"] = bone.get("scaleY", 1) * inv_uniform

    for skin in data.get("skins", []):
        for slot_attachments in skin.get("attachments", {}).values():
            for attachment in slot_attachments.values():
                if isinstance(attachment, dict):
                    scale_attachment(attachment, sx, sy)

    skeleton = data.get("skeleton", {})
    if "width" in skeleton:
        skeleton["width"] *= inv_uniform
    if "height" in skeleton:
        skeleton["height"] *= inv_uniform


def write_variant(code: str, sx: float, sy: float):
    out_dir = OUT_BASE / code.lower()
    out_dir.mkdir(parents=True, exist_ok=True)

    data = json.loads((SOURCE_DIR / "Chopi.spine-json").read_text(encoding="utf-8-sig"))
    scale_json(data, sx, sy)
    data.setdefault("skeleton", {})["hash"] = f"{data.get('skeleton', {}).get('hash', '')}-cult-scale-{code}"

    (out_dir / f"{code}.spine-json").write_text(
        json.dumps(data, ensure_ascii=False, separators=(",", ":")),
        encoding="utf-8",
    )

    atlas = (SOURCE_DIR / "Chopi.atlas").read_text(encoding="utf-8-sig")
    atlas = atlas.replace("Chopi.png", f"{code}.png", 1)
    (out_dir / f"{code}.atlas").write_text(atlas, encoding="utf-8")
    shutil.copy2(SOURCE_DIR / "Chopi.png", out_dir / f"{code}.png")
    print(code, out_dir, "sx", sx, "sy", sy)


def main():
    for code, (sx, sy) in VARIANTS.items():
        write_variant(code, sx, sy)


if __name__ == "__main__":
    main()
