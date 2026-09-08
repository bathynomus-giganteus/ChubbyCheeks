from __future__ import annotations

import json
from pathlib import Path


SKELETON = Path(r"E:\work\Cult_leader_mod\SPINE_4_2_TEST\正常使徒\chopimanualfix03\ChopiManualFix03.spine-json")
TARGETS = {
    "Axe", "Skirt", "Skirt_backside", "Body top", "Calf_L", "Calf_R",
    "Hair back_L", "Hair back_R", "Tail", "Leg L", "Leg R", "Belly",
    "Thigh_L", "Thigh_R", "Face", "Forearm_L", "Forearm_R",
}


def main() -> None:
    data = json.loads(SKELETON.read_text(encoding="utf-8-sig"))
    for skin in data.get("skins", []):
        skin_name = skin.get("name", "")
        for slot, attachments in skin.get("attachments", {}).items():
            for attachment_name, attachment in attachments.items():
                path_name = attachment.get("path") or attachment_name
                if path_name not in TARGETS and attachment_name not in TARGETS:
                    continue
                attachment_type = attachment.get("type", "region")
                uvs_count = len(attachment.get("uvs", []))
                print(f"{skin_name}\t{slot}\t{attachment_name}\tpath={path_name}\ttype={attachment_type}\tuvs={uvs_count}")


if __name__ == "__main__":
    main()
