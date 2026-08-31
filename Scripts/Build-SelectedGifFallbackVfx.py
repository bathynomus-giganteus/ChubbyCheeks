from __future__ import annotations

import json
import shutil
from pathlib import Path
from typing import Any

from PIL import Image, ImageOps, ImageSequence


PROJECT_ROOT = Path(r"C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod")
SOURCE_ROOT = Path(r"E:\work\Cult_leader_mod\坨坨")

MANIFEST_PATH = PROJECT_ROOT / "external_vfx_manifest.json"
LOCAL_MOD_DIR = Path(r"E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod")
WORKSHOP_SUB_DIR = Path(r"E:\SteamLibrary\steamapps\workshop\content\2868840\3784977251")
WORKSHOP_STAGE_DIR = PROJECT_ROOT / "release" / "workshop" / "CultLeaderModWorkspace" / "content"

TARGETS = {
    # key: (personality folder, apostle folder)
    "calm_23": ("冷静", "蕾特"),
    "lively_13": ("活泼", "修罗"),
    "melancholy_10": ("忧郁", "洛涅（市长）"),
    "melancholy_25": ("忧郁", "乔菲"),
    "melancholy_26": ("忧郁", "欧若拉"),
}

ATTACK_SIZE = 420
PREVIEW_SIZE = 360
MAX_ATTACK_FRAMES = 96
MAX_PREVIEW_FRAMES = 96


def load_manifest() -> list[dict[str, Any]]:
    return json.loads(MANIFEST_PATH.read_text(encoding="utf-8-sig"))


def save_manifest(entries: list[dict[str, Any]]) -> None:
    MANIFEST_PATH.write_text(json.dumps(entries, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def frame_indices(total: int, max_frames: int) -> list[int]:
    if total <= max_frames:
        return list(range(total))
    return [round(i * (total - 1) / (max_frames - 1)) for i in range(max_frames)]


def normalize_frame(frame: Image.Image, size: int, mirror: bool) -> Image.Image:
    image = frame.convert("RGBA")
    if mirror:
        image = ImageOps.mirror(image)

    # Keep the source visual scale; only fit canvases larger than the target.
    ratio = min(size / image.width, size / image.height, 1.0)
    new_size = (max(1, round(image.width * ratio)), max(1, round(image.height * ratio)))
    if new_size != image.size:
        image = image.resize(new_size, Image.Resampling.LANCZOS)

    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    canvas.alpha_composite(image, ((size - image.width) // 2, (size - image.height) // 2))
    return canvas


def export_gif_frames(source: Path, output_dir: Path, size: int, max_frames: int, mirror: bool) -> int:
    if not source.exists():
        print(f"missing source gif: {source}")
        return 0

    if output_dir.exists():
        shutil.rmtree(output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    with Image.open(source) as gif:
        frames = [frame.copy() for frame in ImageSequence.Iterator(gif)]

    selected = frame_indices(len(frames), max_frames)
    for out_index, source_index in enumerate(selected):
        image = normalize_frame(frames[source_index], size, mirror)
        image.save(output_dir / f"frame_{out_index:03}.png")

    return len(selected)


def update_entry(entry: dict[str, Any], key: str, source_dir: Path, attack_count: int, preview_count: int) -> None:
    entry["source"] = str(source_dir)
    entry["attack_source"] = str(source_dir / "动画.gif")
    entry["preview_source"] = str(source_dir / "立绘.gif")
    entry["attack_count"] = attack_count
    entry["preview_count"] = preview_count


def mirror_to_mod_dirs() -> None:
    for mod_dir in [LOCAL_MOD_DIR, WORKSHOP_SUB_DIR, WORKSHOP_STAGE_DIR]:
        mod_dir.mkdir(parents=True, exist_ok=True)
        shutil.copy2(MANIFEST_PATH, mod_dir / "external_vfx_manifest.json")

        source_external = PROJECT_ROOT / "external_vfx"
        target_external = mod_dir / "external_vfx"
        if target_external.exists():
            for key in TARGETS:
                for suffix in ["attack", "preview"]:
                    path = target_external / f"{key}_{suffix}"
                    if path.exists():
                        shutil.rmtree(path)
        target_external.mkdir(parents=True, exist_ok=True)

        for key in TARGETS:
            for suffix in ["attack", "preview"]:
                source_dir = source_external / f"{key}_{suffix}"
                if source_dir.exists():
                    shutil.copytree(source_dir, target_external / source_dir.name, dirs_exist_ok=True)


def main() -> None:
    manifest = load_manifest()
    by_key = {entry.get("key"): entry for entry in manifest}

    external_root = PROJECT_ROOT / "external_vfx"
    external_root.mkdir(parents=True, exist_ok=True)

    for key, (personality, apostle) in TARGETS.items():
        source_dir = SOURCE_ROOT / personality / apostle
        attack_count = export_gif_frames(
            source_dir / "动画.gif",
            external_root / f"{key}_attack",
            ATTACK_SIZE,
            MAX_ATTACK_FRAMES,
            mirror=True,
        )
        preview_count = export_gif_frames(
            source_dir / "立绘.gif",
            external_root / f"{key}_preview",
            PREVIEW_SIZE,
            MAX_PREVIEW_FRAMES,
            mirror=False,
        )

        if key not in by_key:
            by_key[key] = {"key": key, "classes": [], "cards": []}
            manifest.append(by_key[key])

        update_entry(by_key[key], key, source_dir, attack_count, preview_count)
        print(f"{key}: attack={attack_count}, preview={preview_count}, source={source_dir}")

    save_manifest(manifest)
    mirror_to_mod_dirs()
    print("updated", MANIFEST_PATH)
    print("synced selected external_vfx folders to local/workshop/staging")


if __name__ == "__main__":
    main()
