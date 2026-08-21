# 本地化机翻初稿说明

本批文件最初由脚本从旧版 `CultLeaderModCode/Patches/LocInjectPatch.cs` 中文注入表抽取 key 后机翻生成。之后本地化架构已调整为：`LocInjectPatch.cs` 不再保存大段中文文本，而是按当前语言读取 `CultLeaderMod/localization/{language}/{table}.json`。

## 输出目录

- `CultLeaderMod/localization/eng`：英文机翻初稿
- `CultLeaderMod/localization/jpn`：日文机翻初稿
- `CultLeaderMod/localization/kor`：韩文机翻初稿
- `CultLeaderMod/localization/zhs`：完整中文文本，已从旧 C# 注入表导出

## 当前加载规则

- `zhs` / `zh` / `zh_CN` 等：读取 `zhs`
- `eng` / `en` 等：读取 `eng`
- `jpn` / `ja` / `jp` 等：读取 `jpn`
- `kor` / `ko` / `kr` 等：读取 `kor`
- 其他未知语言：先尝试同名目录，失败后回退 `eng`，再失败回退 `zhs`

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
- `LocInjectPatch.cs` 现在已经改为 JSON 加载器。以后改本地化文本时优先改 JSON，不要再把大段文本写回 C#。
- 英文、日文、韩文仍为机翻初稿；英文已作为第一套实际可加载的非中文本地化文件，但需要后续人工润色。

## 2026-08-21 补充

- 已补修英/日/韩三套文本中 `{IfUpgraded:show:...|...}` 动态条件内残留的中文分支。
- 已补修英/日/韩 `咻咻咻咻手套` 标题的机翻占位。
- 当前 `eng/jpn/kor` 三套文件均通过覆盖校验：每套 615 条 key，缺失 key 0，额外 key 0，动态变量 token mismatch 0。
- 日文、韩文仍按“可加载机翻草稿”处理，未做最终术语表统一。
