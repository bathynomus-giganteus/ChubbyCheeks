# 本地化机翻初稿说明

本批文件由脚本从 `CultLeaderModCode/Patches/LocInjectPatch.cs` 当前中文注入表抽取 key 后机翻生成。

## 输出目录

- `CultLeaderMod/localization/eng`：英文机翻初稿
- `CultLeaderMod/localization/jpn`：日文机翻初稿
- `CultLeaderMod/localization/kor`：韩文机翻初稿

## 覆盖范围

- `cards`：357 条
- `powers`：88 条
- `relics`：60 条
- `characters`：21 条
- `card_keywords`：14 条
- `events`：61 条
- `ancients`：8 条
- `gameplay_ui`：6 条

## 注意

- 这是机翻草稿，只用于先攒文本和测试 key 覆盖，不代表最终译文质量。
- 脚本保护了 `{...}` 动态变量、`[...]` 富文本标签和换行，后续仍需游戏内检查。
- 当前 `LocInjectPatch.cs` 仍会无视语言参数注入中文；若要让这些文件真正按语言生效，需要后续把该 patch 改成按 `language` 分支注入或停止覆盖非中文语言。
