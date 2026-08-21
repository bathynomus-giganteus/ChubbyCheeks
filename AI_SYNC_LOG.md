# AI Sync Log

> 用途：Codex 与 DeepSeek harness 共同维护 CultLeaderMod 时的轻量同步日志。  
> 规则：只追加，不删除；每次任务结束后记录修改范围、构建结果、测试状态、下一步。

## 2026-08-20 22:50 - Codex

- 任务：整理 DeepSeek harness 继续项目所需的工程交接 prompt，并建立 Codex × DeepSeek 联动机制。
- 读取依据：`PROJECT_KNOWLEDGE.md`、`PROJECT_RULES_FOR_AI.md`、`CONTINUE_PROMPT_STS2_MOD.md`、`HANDOFF_DEEPSEEK_SESSION.md`、`README.md`、`CultLeaderMod.json`、工坊 `workshop.json`、`git status` / `git log`。
- 修改文件：
  - 新增 `DEEPSEEK_HARNESS_HANDOFF_PROMPT.md`
  - 新增 `AI_SYNC_LOG.md`
- 当前工程状态：
  - 最近发布分支：`cultleader-card-tuning-20260820`
  - 最近提交：`59e716a Balance and refine apostle card mechanics`
  - 该分支已推送到 GitHub。
  - Steam Workshop 物品 ID：`3784977251`
  - 工坊 changeNote：`v0.2.05`
- 构建结果：本次只整理交接文件，未重新构建；上一轮代码改动已 `dotnet build` 通过，0 errors / 4 known warnings。
- 待用户验证：上一轮卡牌改动与工坊版本是否在游戏内符合预期，尤其富文本颜色/`[sine]` 是否实际渲染。
- 下一步建议：
  - 如果 DeepSeek harness 接手，先读 `DEEPSEEK_HARNESS_HANDOFF_PROMPT.md`。
  - 后续双方每轮结束都追加本文件。
  - 长期范式或已确认坑点继续追加到 `PROJECT_KNOWLEDGE.md`。

## 2026-08-20 22:58 - Codex

- 任务：修正 DeepSeek harness 交接前提。
- 读取依据：用户说明“旧会话 DeepSeek harness 不能读，是在当前 Codex 软件里的”。
- 修改文件：
  - `DEEPSEEK_HARNESS_HANDOFF_PROMPT.md`
  - `AI_SYNC_LOG.md`
- 结论：DeepSeek harness 不应尝试读取 Codex app 内旧会话；旧会话中的重要信息必须通过项目内文件同步。后续共同维护以 `AI_SYNC_LOG.md`、`PROJECT_KNOWLEDGE.md`、`PROJECT_RULES_FOR_AI.md`、当前源码和 git 状态为准。
- 构建结果：仅修改文档，未构建。

## 2026-08-20 23:16 - Codex

- 任务：为当前版本建立可回退备份点，并实装教主现有卡牌的鼠标悬浮提示。
- 备份点：
  - 分支：`backup/pre-hover-tooltips-20260820-225833`
  - 标签：`backup-pre-hover-tooltips-20260820-225833`
  - 指向提交：`59e716a Balance and refine apostle card mechanics`
  - 状态：已推送到 GitHub，可随时 checkout/reset 回退。
- 修改文件：
  - 新增 `CultLeaderModCode/Patches/CardHoverTipsPatch.cs`
- 实现范式：
  - 采用集中式 Harmony patch 挂到 `CardModel.HoverTips` getter。
  - 固定词条/标签：对 `使徒` 与五种性格 tag 追加对应自定义 keyword hover tip 的兜底显示。
  - 新增/引用能力：通过 `ModelDb.GetById<PowerModel>(ModelDb.GetId(powerType))` + `HoverTipFactory.FromPower(power)` 生成 Power 说明框。
  - 衍生卡牌：通过 `ModelDb.GetById<CardModel>(ModelDb.GetId(cardType))` + `HoverTipFactory.FromCard(card, false)` 生成卡牌预览框。
  - 后续新增卡牌时，优先在 `PowerTipsByCard` / `CardTipsByCard` 映射表中追加类型映射，不要逐张复制 UI 逻辑。
- 当前覆盖：
  - 大部分现有卡牌中直接 `PowerCmd.Apply<TPower>` 的自定义/原版 Power 悬浮。
  - 衍生卡预览：黄油飞射 → 黄油融化；戏剧性演出 → 三张分支卡；魔弹装填/魔.弹.の.射.手 → 魔弹衍生卡。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。warning 仍为既有项：3 个 `oldOwner` nullable，1 个 `Apostle_Melancholy_19` async no await。
- 测试状态：未进游戏实测 hover UI；构建已复制 mod。建议游戏内检查：使徒牌性格词条、Power 说明框、衍生卡预览是否都显示，尤其确认是否存在重复 keyword tip。
- GitHub 状态：本地提交已创建；推送当前分支时 GitHub 连接被重置（`Recv failure: Connection was reset`），因此该提交暂时只在本地，稍后网络恢复后需要重试 `git push origin cultleader-card-tuning-20260820`。

## 2026-08-20 23:40 - Codex

- 任务：根据游戏内测试反馈修正卡牌悬浮提示。
- 修改文件：
  - `CultLeaderModCode/Patches/CardHoverTipsPatch.cs`
  - `CultLeaderModCode/Patches/LocInjectPatch.cs`
- 修复内容：
  - 使徒牌悬浮提示新增“使徒名称”框，名称来自 `C:\Users\888\Desktop\New_folder\卡牌信息.xlsx` 的“后台名称/使徒名称”列映射。
  - 五种性格关键词 hover 改为：
    - 标题：`使徒性格`
    - 描述：`纯粹/冷静/狂热/活泼/忧郁`
  - 对卡牌描述中的固定机制词进行自动悬浮补充：`再生`→原版 `RegenPower`，`覆甲`→原版 `PlatingPower`，`活力`→原版 `VigorPower`，`保留`→自定义 `RetainPower`，`苦痛施予`→自定义 `BitterPainPower`，`计划妥当`→原版 `RetainHandPower`。
  - 对卡牌本地化描述里的上述固定词条尝试自动包裹 Godot RichText 标准颜色标签 `[color=#FFD84A]...[/color]`。
  - 补充缺失的 `RookieCardPower` 与 `PatCardPower` 本地化，避免悬浮框显示内部代码名。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。
- 测试状态：未进游戏实测；建议重点验证 `[color=#FFD84A]` 是否被 STS2 卡牌描述渲染器接受。如果仍显示原色或露 BBCode，需要改走引擎关键词高亮/描述节点补丁。

## 2026-08-20 23:55 - Codex

- 任务：根据第二轮 hover UI 反馈调整显示合并规则。
- 修改文件：
  - `CultLeaderModCode/Patches/CardHoverTipsPatch.cs`
- 修复内容：
  - 不再把“使徒牌”和“使徒名称”拆成两个框；使徒名改为显示在“使徒牌”框内。
  - 乌洛斯/循环这类多性格使徒牌不再显示多个性格框；统一为一个“使徒性格”框，内容用空格分隔：`纯粹 冷静 狂热 活泼 忧郁`。
  - 衍生牌使徒名称手动去掉括号附注：黄油融化显示“黄油”，埃皮康衍生牌显示“埃皮卡”，魔弹衍生牌显示“x锡安x”。
  - 移除 `PatCard` 对 `PatCardPower` 的 Power hover 映射，避免摸摸头效果显示无意义/缺失图标。
  - 对 RitsuLib 自动生成的原始自定义 tag hover 做过滤，再追加本 mod 的合并版 hover，避免重复框。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。

## 2026-08-21 - Codex

- 任务：继续修正卡牌 hover 与百科筛选 UI。
- 修改文件：
  - `CultLeaderModCode/Patches/CardHoverTipsPatch.cs`
  - `CultLeaderModCode/Patches/CardLibraryApostleFilterPatch.cs`
  - `CultLeaderModCode/Patches/LocInjectPatch.cs`
- 修复内容：
  - “使徒牌”悬浮框内移除固定说明“使徒之力凝聚的卡牌”，现在只显示 `使徒名称：xxx`，缺少映射时显示 `使徒名称：未知`。
  - 对悬浮框过多的重点卡牌做紧凑化处理：
    - `循环/TestRainbowCard`：将再生、覆甲、活力、保留、苦痛施予合并为一个“相关状态”提示框。
    - `助手埃皮康/Apostle_Lively_08_1`：将再生、覆甲、活力、苦痛施予合并为一个“相关状态”提示框。
    - 这不是全局双列 hover 容器补丁，而是针对当前溢出最严重卡牌减少提示框数量，避免 hover UI 从右侧挤到左侧。
  - 百科/卡牌库的教主筛选按钮改为短标签，按钮宽度缩小；弹出菜单仍使用完整筛选文本，并放大字体、行距与内边距。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。warning 仍为既有项：3 个 `oldOwner` nullable，1 个 `Apostle_Melancholy_19` async no await。
- 测试状态：未进游戏实测；建议重点检查百科筛选按钮位置、弹出菜单大小，以及循环/助手埃皮康 hover 是否仍会左右跳位。

## 2026-08-21 - Codex

- 任务：为英文、日文、韩文本地化先生成机翻文本初稿。
- 修改/新增文件：
  - 更新 `CultLeaderMod/localization/eng/cards.json`
  - 更新 `CultLeaderMod/localization/eng/characters.json`
  - 更新 `CultLeaderMod/localization/eng/powers.json`
  - 更新 `CultLeaderMod/localization/eng/relics.json`
  - 新增 `CultLeaderMod/localization/eng/ancients.json`
  - 新增 `CultLeaderMod/localization/eng/card_keywords.json`
  - 新增 `CultLeaderMod/localization/eng/events.json`
  - 新增 `CultLeaderMod/localization/eng/gameplay_ui.json`
  - 新增 `CultLeaderMod/localization/jpn/*.json`
  - 新增 `CultLeaderMod/localization/kor/*.json`
  - 新增 `LOCALIZATION_MACHINE_TRANSLATION_NOTES.md`
- 生成方式：
  - 从 `CultLeaderModCode/Patches/LocInjectPatch.cs` 当前中文注入表抽取 key。
  - 同时合并旧 `zhs/eng` JSON 中仅存在于文件的 key。
  - 使用机翻生成 `eng/jpn/kor` 三套文本，每套 615 条。
  - 生成时保护 `{...}` 动态变量、`[sine]` 等富文本标签和换行；生成后校验三种语言均为 0 个受保护 token mismatch。
- 重要注意：
- 当前 `LocInjectPatch.cs` 仍然不按 `language` 参数分支，会对所有语言注入中文。
- 因此这批 JSON 是“文本准备/覆盖率测试”初稿；若要真正游戏内切换语言，需要下一步重构 `LocInjectPatch`，避免非中文语言被中文运行时注入覆盖。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。warning 仍为既有项。

## 2026-08-21 - Codex

- 任务：重构本地化架构，使英文等 JSON 文件能按语言实际加载。
- 修改/新增文件：
  - 重写 `CultLeaderModCode/Patches/LocInjectPatch.cs`
  - 更新并补全 `CultLeaderMod/localization/zhs/*.json`
  - 更新 `LOCALIZATION_MACHINE_TRANSLATION_NOTES.md`
- 实现内容：
  - 旧版 `LocInjectPatch.cs` 中的大型中文 dictionary 已移除。
  - 新版 `LocInjectPatch.cs` 只负责按 `language` 读取 `CultLeaderMod/localization/{lang}/{table}.json` 并 `MergeWith` 到游戏本地化表。
  - 支持语言别名：
    - `zhs` / `zh` / `zh_cn` 等 → `zhs`
    - `eng` / `en` 等 → `eng`
    - `jpn` / `ja` / `jp` 等 → `jpn`
    - `kor` / `ko` / `kr` 等 → `kor`
  - 未知语言先尝试同名目录，再回退英文，最后回退中文。
  - 中文完整文本已从旧 C# 注入表导出为 `zhs` JSON；每种语言均为 8 个表、615 条 key。
  - 已将 `CultLeaderMod/localization` 同步到 Steam mod 目录 `E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\CultLeaderMod\localization`。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。warning 仍为既有项。
- 测试建议：
  - 游戏语言为中文时检查是否仍显示完整中文。
  - 游戏语言切到英文时检查卡牌、能力、遗物、事件等是否显示英文机翻文本而非中文。

## 2026-08-21 - Codex

- 任务：继续整理日文、韩文本地化机翻版。
- 修改文件：
  - `CultLeaderMod/localization/jpn/cards.json`
  - `CultLeaderMod/localization/jpn/relics.json`
  - `CultLeaderMod/localization/kor/cards.json`
  - `CultLeaderMod/localization/kor/relics.json`
  - 顺手修正 `eng/cards.json`、`eng/relics.json` 中同源的动态分支中文残留。
- 实现内容：
  - 修复 `{IfUpgraded:show:...|...}` 动态条件文本中漏出的中文，使英文、日文、韩文不再残留这类简中分支文本。
  - 修复 `咻咻咻咻手套` 在英/日/韩中的标题机翻占位。
  - 校验 `eng/jpn/kor` 均为 8 个表、615 条 key，缺失 key 为 0，额外 key 为 0，动态变量 token mismatch 为 0，已知中文残留模式为 0。
  - 已将 `CultLeaderMod/localization` 同步到 Steam mod 目录。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。warning 仍为既有项。
- 注意：日文、韩文仍为机翻草稿，术语和语气尚未人工统一；若后续要做可发布质量，需要按卡牌/能力逐批润色。

## 2026-08-21 - Codex

- 任务：补充当前工坊简介的英 / 日 / 韩版本。
- 修改文件：
  - 新增 `WORKSHOP_LOCALIZED_DESCRIPTIONS.md`
  - 更新未跟踪工坊配置 `release/workshop/CultLeaderModWorkspace/workshop.json`
- 实现内容：
  - 保留中文简介原文不动。
  - 根据当前工坊简介补充 English / 日本語 / 한국어 三套机翻草稿。
  - 工坊 JSON 中新增 `description_localizations.eng/jpn/kor`，供后续上传脚本或人工复制使用。
  - 未向 `CultLeaderMod.json` 游戏 manifest 添加未知多语言字段，避免潜在加载兼容风险。
- 注意：`release/` 目录当前整体仍为未跟踪目录；若要把工坊配置纳入 Git，需要另行决定提交策略。

## 2026-08-21 - Codex

- 任务：修复英 / 日 / 韩游戏内显示 localization key / 代码名，而不是实际文本的问题。
- 现象：
  - 用户测试发现三种语言都显示代码，没有实际文字。
  - 最新游戏日志 `C:\Users\888\AppData\Roaming\SlayTheSpire2\logs\godot.log` 中大量出现 `GetRawText: Key 'CULT_LEADER_MOD_CARD_...' not found in table 'cards'`。
  - 日志中没有出现 `LocInjectPatch` 的加载日志，说明 JSON 文件本身存在，但本地化注入没有执行。
- 原因判断：
  - `LocManager.SetLanguageInternal` 当前实际签名为 `(string language, Dictionary<string, LocTable> tables, bool overridesActive, List<LocValidationError> validationErrors)`。
  - 旧 patch 没有明确匹配 4 参数签名，且只依赖语言切换时机，导致注入不稳定 / 未触发。
- 修复：
  - `LocInjectPatch` 改为精确 patch `SetLanguageInternal` 的 4 参数签名。
  - 新增 `LocInjectPatch.Install()`：在 `Entry.Init()` 中主动注入当前语言，并注册 `SubscribeToLocaleChange` 回调，语言切换时再次注入。
  - `dotnet build` 现在会自动复制 `CultLeaderMod/localization/**/*.json` 到游戏 mod 目录，避免只复制 DLL 而遗漏 loose JSON。
- 构建结果：`dotnet build` 通过，0 errors / 4 known warnings。
- 测试建议：
  - 重启游戏后检查日志是否出现 `Localization injected from Entry.Init` 或 `Localization injected from SetLanguageInternal/LocaleChange`。
  - 若仍显示代码，优先检查日志中 `Failed to read localization resource/file` 或 `Missing localization table`。

## 2026-08-21 - Codex

- 任务：继续修复本地化仍显示代码的问题。
- 新发现：
  - 最新日志显示 `LocInjectPatch` 已经执行，但 `cards` 只注入 6 条、`characters` 只注入 7 条，其他表缺失。
  - 这说明注入器优先从 `res://CultLeaderMod/localization/...` 读到了 PCK 内旧版残留 JSON，而不是 loose 文件中的完整 615-key JSON。
- 修复：
  - `LocInjectPatch.ReadText` 改为优先读取文件系统 loose JSON（`mods/CultLeaderMod/CultLeaderMod/localization/...`），只有找不到 loose 文件时才回退 `res://`。
  - 修正 `CultLeaderMod.csproj` 中 build 复制本地化文件的目标路径，确保复制到 `$(ModsPath)CultLeaderMod/CultLeaderMod/localization/%(RecursiveDir)...`。
- 验证：
  - `dotnet build` 通过，0 errors / 4 known warnings。
  - 游戏 mod 目录正确位置下 `eng/jpn/kor/zhs` 均为 8 个 JSON、615 条 key。
- 注意：
  - 之前错误复制出的 `mods/CultLeaderMod/eng|jpn|kor|zhs` 和旧的 `localization/localization` 重复目录只会制造 manifest 扫描噪音，不是当前显示代码的根因；如需清理可后续手动删。
