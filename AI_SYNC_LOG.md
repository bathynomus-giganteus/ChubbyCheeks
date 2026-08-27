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

## 2026-08-21 - Codex

- 任务：记录后续卡牌口径调整。
- 用户记录：
  - 【芬多精波动】和【调皮的笑容】的“层数”统计口径后续改为“种类数”。
- 后续实现注意：
  - 不要按状态/减益总层数累计。
  - 应统计不同状态/减益的类型数量。
  - 需要同步修改中文及 eng/jpn/kor 本地化描述，确保动态数字含义从“层数”改为“种类数”。

## 2026-08-21 - Codex

- 任务：继续记录后续卡牌口径/费用调整。
- 用户记录：
  - 【敲爆栗】的费用 +1 效果持续到本场战斗结束，而不是下一次；该持续时间需要同步加入卡牌描述。
  - 【芬多精波动】在改成按“种类数”统计后，升级前后费用均 -1。
- 后续实现注意：
  - 【敲爆栗】需要检查当前实现是否是一次性/下一次费用变化；若是，应改为本场战斗范围的持久费用修改或对应 tracker。
  - 【芬多精波动】需要同时改代码费用、本地化描述和 eng/jpn/kor 机翻文本。

## 2026-08-21 - Codex

- 任务：记录卡牌类型修正。
- 用户记录：
  - 【执行教理】应该是攻击牌，而不是技能牌。
- 后续实现注意：
  - 同步修改卡牌代码中的 `CardType`、游戏内卡牌类型显示、筛选/奖励表现。
  - 若本地化描述或衍生说明中写到了“技能牌”，需要同步修改中英日韩文本。

## 2026-08-21 - Codex

- 任务：记录未实装剧情/对话。
- 用户记录：
  - 战斗结束后【教主】和【建筑师】的对话没有实装上。
- 后续实现注意：
  - 检查战斗结束/胜利后对话触发点是否存在，是否需要 patch 战斗结算或胜利界面流程。
  - 检查该对话是否需要限定教主角色、指定建筑师对象/事件，以及中英日韩本地化文本。

## 2026-08-21 - Codex

- 任务：记录百科界面 UI 待调整项。
- 用户记录：
  - 百科界面的筛选按钮，按钮本体再向右移动大约一个按钮高度的距离。
  - 筛选按钮弹出菜单的字体可以减小一些。
- 后续实现注意：
  - 优先检查当前百科筛选按钮布局补丁/场景节点定位逻辑。
  - 调整弹出菜单字体时注意不要影响游戏其他通用菜单字体，尽量限定在百科筛选菜单范围。

## 2026-08-22 - Codex

- 任务：修正五张性格卡牌的“检索使徒牌”行为。
- 用户确认语义：
  - 五张【性格卡牌】应改为“从抽牌堆随机将一张……移入手牌”。
  - 这不是复制卡牌，也不是从整套卡组/牌库生成新牌。
- 修复：
  - `PersonalityCardFetchPower` 的检索来源从 `PileType.Deck` 改为 `PileType.Draw`。
  - 移除 `combatState.CloneCard(selected)`，直接把抽牌堆中选中的现有卡牌通过 `CardPileCmd.Add(selected, PileType.Hand, ...)` 移入手牌。
  - 同步更新 zhs/eng/jpn/kor 四套 `cards.json` 中五张性格卡牌与 `PersonalityCardFetchPower` 的描述。
- 验证：
  - 四语目标 key 均已检查，文本已显示 draw pile / 抽牌堆 + move / 移入语义。
  - `dotnet build` 的 C# 编译阶段通过，但最终复制到 Steam mod 目录失败，因为 `SlayTheSpire2.exe` 正在运行并锁定 `E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\CultLeaderMod.dll`。
- 测试注意：
  - 需要关闭游戏后重新执行 `dotnet build`，让 DLL 和 loose localization JSON 同步到游戏目录。

## 2026-08-24 - Codex

- 任务：修复【黄油的黄牌】与【天气晴朗卡】两个遗物显示 localization key / 代码的问题。
- 原因：
  - 两个遗物的 6 个本地化 key（title / description / flavor）误写在四语 `cards.json` 中。
  - 遗物界面实际查询 `relics` 表，因此 `CULT_LEADER_MOD_RELIC_BUTTER_YELLOW_CARD_RELIC.*` 与 `CULT_LEADER_MOD_RELIC_CLEAR_WEATHER_CARD_RELIC.*` 会显示为代码。
  - `C:\Users\888\AppData\Roaming\SlayTheSpire2\localization_override\zhs\relics.json` 也是旧版 60-key 文件，缺少这两个遗物。
- 修复：
  - 将 eng/jpn/kor/zhs 四套 `cards.json` 中的这 6 个 key 迁移到对应 `relics.json`。
  - 四语项目文件验证：`cards_relic_keys=0`，`relic_keys=66`。
  - 备份旧 zhs override 到 `localization_override\zhs\relics_before_butter_sun_fix_20260824-203908.json`，并用当前完整 zhs `relics.json` 覆盖 override。
  - 手动同步四语 `cards.json` / `relics.json` 到 Steam mod 目录的 loose localization 路径。
- 验证/注意：
  - `dotnet build` C# 编译阶段通过，但因游戏正在运行，DLL 被 `SlayTheSpire2.exe` 锁定，完整 build 复制目标失败。
  - 本次只改 JSON，本地化 loose 文件已经手动同步；若游戏已启动，需要重启游戏让本地化表重新加载。

## 2026-08-24 - Codex

- 任务：修复【黄油飞射】计数刚好在战斗结束时达到 100 不触发变换的问题。
- 原因判断：
  - 原实现只在 `AfterDamageReceived` 内计数并立即 `CardCmd.Transform`。
  - 若最后一次受怪物攻击后战斗随即进入胜利/收尾流程，立即 Transform 可能被战斗清理流程跳过或未能可靠落到永久卡组。
- 修复：
  - `Apostle_Lively_05` 新增 `TransformToMeltedButterIfReady()`：当 `DamageTaken >= 100` 且该牌位于永久 `PileType.Deck` 时，将其变为【黄油融化】。
  - 在 `AfterCombatVictory` 中调用兜底检查，让胜利结算时尽量立刻转换。
  - 在 `BeforeRoomEntered` 中再次调用兜底检查，避免胜利结算时机仍被游戏流程吞掉。
  - 原受击达到 100 时仍保留立即转换逻辑。
- 验证/注意：
  - `dotnet build` C# 编译阶段通过，说明 `AfterCombatVictory`/`BeforeRoomEntered` override 均可用。
  - 完整 build 复制到 Steam mod 目录失败，因为 `SlayTheSpire2.exe` 仍在运行并锁定 DLL；需要关闭游戏后重新 build 同步。

## 2026-08-26 - Codex

- 任务：根据 `C:\Users\888\Desktop\New_folder\korean loc` 中的韩语校对文件更新项目韩文本地化。
- 来源文件：
  - `ancients.txt`
  - `card_keywords.txt`
  - `cards.txt`
  - `characters.txt`
  - `events.txt`
  - `gameplay_ui.txt`
  - `powers.txt`
  - `relics.txt`
- 修复/更新：
  - 已备份旧韩语表到 `.codex_backups\kor_loc_before_20260826-115322`。
  - 用源文件中与当前项目 key 完全匹配的文本覆盖 `CultLeaderMod/localization/kor/*.json`。
  - 源 `cards.txt` 里仍包含【黄油的黄牌】和【天气晴朗卡】两个遗物的 6 个 key；本次没有把它们塞回 `cards.json`，而是按项目范式放入 `kor/relics.json`。
  - `kor/cards.json` 保持 353 keys 且 `cards_relic_keys=0`；`kor/relics.json` 保持 66 keys。
  - 已手动同步韩语 loose JSON 到 Steam mod 目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\CultLeaderMod\localization\kor`。
- 验证：
  - 8 个韩语 JSON 表均可解析。
  - 所有表括号/富文本标签基本配对检查通过。
  - 与中文表对照后未发现普通动态变量 key 丢失；`IfUpgraded` 分支内文字不同属于翻译差异。
  - 当前不存在 `C:\Users\888\AppData\Roaming\SlayTheSpire2\localization_override\kor`，韩语环境不会被旧 override 覆盖。

## 2026-08-26 - Codex

- 任务：实装用户新一批卡牌与机制调整，并检查已记录待办状态。
- 已确认此前记录已实装：
  - 【芬多精波动】和【调皮的笑容】已经按“减益/状态种类数”统计，而不是总层数。
  - 【敲爆栗】的费用 +1 已使用 `EnergyCost.AddThisCombat(1)`，持续本场战斗。
  - 【执行教理】已经是 `CardType.Attack`。
- 本轮已实装：
  - 【帮帮我朋友们】：基础伤害 8/11；触发最多 3/5 层治愈；不再给临时力量；获得 8 格挡；每触发 1 层治愈，当前牌伤害 +1；描述同步四语。
  - 【今天的目标就是那家伙】：`PirateMarkPower` 现在要求 `cardSource.Type == CardType.Attack` 且为 powered attack，技能/能力伤害不触发。
  - 【要来见少女吗】：费用降为 1；效果改为每恢复 1 HP 或获得 1 层治愈，获得 1 层活力（埃尔德形态下转为狂热）。
  - 【里科塔全套餐】：升级后每层回复从 4 降为 3。
  - 【雪雾】：从“实际受到生命伤害后减免”改为在攻击伤害进入格挡前的 additive modifier，只在玩家有格挡时减少怪物将要造成的攻击伤害。
  - 【蜜瓜吖】：覆甲满足条件后的额外回复从 10/13 降为 5/8。
  - 【保留】Power：回合末选择至多等于当前层数的手牌保留；只消耗实际选择张数的层数，选 0 不消耗；描述改为“每消耗一层可以在回合结束时保留一张卡牌”。
  - 【胡萝卜治愈】：改为获得 4/6 保留并回复 4/6 生命，保留消耗关键词。
  - 【松鼠雷电】：获得保留从 1 增至 3。
  - 【蜂蜜鱼】：改为获得 3/5 保留；若保留层数不少于 15/12，回复 5 生命；保留消耗关键词。
  - 【有罪宣言】：增加保留关键词。
  - 【魔.弹.の.射.手】：魔弹耗尽时消耗所有手牌/抽牌堆/弃牌堆中的【魔.弹.の.射.手】，再加入【终.末.の.爆.炸】。
  - 【终.末.の.爆.炸】：伤害从 40/55 改为 55/60。
  - 【黄油飞射】：在 `AfterCombatEnd` 也执行一次变身兜底，配合既有 `AfterCombatVictory` / `BeforeRoomEntered`，用于处理刚好战斗结束时计数到 100 的情况。
- 本地化与部署：
  - zhs/eng/jpn/kor 的相关卡牌与 Power 描述已同步更新。
  - 因 `localization_override/zhs` 仍有旧中文文本，本轮已备份到 `.codex_backups\zhs_override_before_20260826_batch_*` 并用当前完整 zhs JSON 覆盖。
  - 已确认 zhs 项目表、Steam loose 表、AppData override 中【天气晴朗卡】和【黄油的黄牌】遗物 key 均在 `relics.json`，`cards.json` 不含这两个 `_RELIC_` key。
- 验证：
  - `dotnet build CultLeaderMod.sln -p:ModsPath="...\tmp\buildmods\"` 通过，0 errors，仅 4 个历史 warning。
  - 游戏进程未运行时执行普通 `dotnet build CultLeaderMod.sln` 成功并同步到 Steam mod 目录。
- 待实测注意：
  - 新【保留】流程需要实测回合末选择 UI 与层数扣减是否符合手感。
  - 【黄油飞射】战斗结束变身已增加多重兜底，但仍建议用“怪物攻击导致战斗结束且计数刚到 100”的场景复测。

## 2026-08-26 - Codex

- 任务：调整【警戒线上的幽灵】、【休假中潜逃】，并确认两者联动。
- 已实装：
  - 【警戒线上的幽灵】：
    - 费用从 2 降至 1。
    - 伤害从 14/18 降至 8/10。
    - 仍对所有敌人造成伤害并给予 1 层【存续】。
  - 【存续】/ `ExtantPower`：
    - 旧效果：敌人受到攻击伤害时，按玩家当前保留/幸福层数追加伤害。
    - 新效果：回合结束保留结算后，玩家本次每保留 1 张手牌，拥有【存续】的敌人每层受到 3 点无来源攻击伤害。
    - 多层存续会叠加，例如 2 层存续且本次保留 4 张牌时，该敌人受到 24 点伤害。
  - 【休假中潜逃】：
    - 获得治愈从 6/9 改为 4/6。
    - 获得格挡从 6/9 改为 4/6。
    - 新增“本回合保留你的手牌”，实现为给玩家施加 1 层原版 `RetainHandPower`。
- 联动结论：
  - 【休假中潜逃】使用 `RetainHandPower`，应进入游戏原生回合末 `AfterFlush(... retainedCards)` 的 retainedCards 统计。
  - 【存续】现在正是读取 `retainedCards.Count` 触发，因此【休假中潜逃】保留的手牌应能触发【警戒线上的幽灵】的存续伤害。
- 本地化：
  - zhs/eng/jpn/kor 的【警戒线上的幽灵】、【休假中潜逃】、【存续】描述已同步。
  - 已同步 zhs 到 Steam loose 文件和 `C:\Users\888\AppData\Roaming\SlayTheSpire2\localization_override\zhs`，避免旧 override 覆盖。
- 验证：
  - 临时 ModsPath 编译通过：0 errors，仅 4 个历史 warning。
  - 游戏未运行，普通 `dotnet build CultLeaderMod.sln` 成功并同步到 Steam mod 目录。

## 2026-08-22 - Codex

- 任务：记录【谢绝non grata】效果问题。
- 用户记录：
  - 【谢绝non grata】的效果应该是：如果层数不低于3层，则消耗1层。
- 用户记录：
  - 【要来少女身边吗】描述有一个多余的消耗。
- 用户记录：
  - 埃尔芬的法杖效果从每20次改为每10次。
- 用户记录：
  - 生命宝石的效果从每3个房间改为每2个房间。
- 用户记录：
  - 奈亚的海豚水枪改为：你每获得8次治愈，回复1生命并抽一张牌。
- 用户记录：
  - 【雪花蝶舞】的伤害次数和【百帕斯卡 挥棒! 】的伤害值没有做动态显示。

## 2026-08-22 - Codex

- 任务：批量实装记录项。
- 已完成：谢绝non grata、要来少女身边吗、埃尔芬法杖、生命宝石、奈亚水枪、芬多精波动/调皮的笑容种类数、芬多精波动费用、敲爆栗持续费用、执行教理类型、雪花蝶舞/百帕斯卡动态显示、百科筛选按钮位置与字体。
- 未完成：战斗结束后教主与建筑师的对话；需要接入 TheArchitect 事件/战斗流程后再实装。

## 2026-08-22 - Codex

- 任务：记录未来可选功能。
- 用户记录：
  - 使用教主角色时，替换某些通用卡牌（诅咒牌、状态牌）的卡图。

## 2026-08-23 - Codex

- 任务：记录待实装卡牌调整。
- 用户记录：
  - 【要来少女身边吗】取消移除消耗（保留原消耗关键词）。
  - 【黄瓜油】降为3治愈。
  - 【帮帮我朋友们】触发至多3/5次治愈，之后对全体敌人造成13/16点伤害；本场战斗中每触发过一层治愈，伤害+3。13/16需使用动态数字。
  - 【魔力乱打】移除治愈改为触发治愈。
  - 【清晰的界限】改为：触发最多3次治愈，每触发一次抽一张牌。
  - 【今天的目标就是那家伙】只有攻击牌造成的伤害能触发；描述不用改。
  - 【团体跳级】变为稀有卡；升级后1费，并增加消耗。
  - 【战术无人机】被多段攻击时没有按照次数叠加覆甲。
  - 【要来少女身边】的数字4的颜色动态特效没了。
  - 【elenA-超频+】多段攻击好像还是只触发一次。
  - 【帮帮我朋友们】卡牌描述文字顺序有问题：“之后对全体敌人造成13点伤害”应该在“本场战斗中每触发过一层治愈，此牌伤害+3”的前面。
  - 【帮帮我朋友们】每层治愈加成从+3改为+1。
- 实现方向：
  - 运行时 Patch `CardModel.PortraitPath` 或 `NCard.UpdatePortrait`。
  - 仅当角色为 `CultLeaderModCharacter` 且卡牌类型为 Curse/Status 时替换卡图。
  - 替换图需要进入 Godot 导入并导出 PCK。

## 2026-08-26 - Codex

- 任务：实装【要来少女的身边吗？】数字 4 动态效果、【有罪宣言】中文文本格式、【鹿派斩击】/【围猎】/【次元定位】/【清晰的界限】机制改动。
- 已完成：
  - 【要来少女的身边吗？】中文描述中的固定数字 4 改为 `[sine][green]4[/green][/sine]`。
  - 【有罪宣言】中文描述三条效果改为同一行，用 `；` 分隔。
  - 【鹿派斩击】重做为：3 费稀有攻击牌，造成 3 点伤害 4 次，根据 `DamageResult.TotalDamage` 汇总实际造成总伤害并获得等量活力/狂热，然后将自身置入抽牌堆随机位置。当前升级无额外效果。
  - 【清晰的界限】改为触发最多 2/3 次治愈，每触发一次抽 1 张牌。
  - 【围猎】重做为：2 费稀有攻击牌，造成 7/9 点伤害 3 次，并施加 50/75 层【围猎标记】。
  - 新增 `AbilityDamageTakenBonusPower`：玩家通过能力牌对拥有此 debuff 的敌人造成伤害时，伤害增加 `Amount%`；计数可叠加。
  - 【猩红之雨】改名并重做为【次元定位】：1 费罕见技能牌，获得 6 活力/狂热，获得 1 层【次元定位】。
  - 新增 `DimensionPositionPower`：下一张攻击牌进入伤害修正时快照核心增益；攻击结算后恢复被消耗、触发或移除导致低于快照值的层数，然后移除 1 层自身。
- 次元定位当前恢复白名单：
  - 治愈、生命本源、覆甲、固若坚冰、活力、狂热、保留、幸福、苦痛施予、苦痛爆发、力量、敏捷、人工制品、缓冲。
- 注意：
  - 次元定位选择“前后快照”方案，不全局拦截 `PowerCmd.ModifyAmount`，避免重新污染 TEST/洗牌/卡牌移动流程。
  - 如果某张攻击牌同时消耗并新增同一种 buff，当前实现只保证结算后不低于攻击前层数，不会额外累计中间新增量。
- 本地化：
  - zhs/eng/jpn/kor 的相关卡牌描述和两个新 Power 描述已补。
  - 已同步 `C:\Users\888\AppData\Roaming\SlayTheSpire2\localization_override\zhs`。
- 验证：
  - `dotnet build CultLeaderMod.sln --nologo -p:ModsPath="C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\tmp\buildmods\"` 通过。
  - 游戏未运行，`dotnet build CultLeaderMod.sln --nologo` 已同步 Steam mod 目录。
  - 所有 localization JSON 通过 `ConvertFrom-Json` 校验。

## 2026-08-26 - 用户记录：下一轮待实装卡牌/状态调整

- 【围猎】：【围猎标记】图标改为对应使徒头像。
- 【总有一天会发生的事】：新增获得 5/8 保留；伤害倍率降为 2/3。
- 【黄油飞射】：描述去掉“加入卡组后开始计数，”和“（被格挡时也计入）”。
- 【呱呱雨】：每回合获得保留提升至 3/4。
- 【充满魄力的新秀】：获得保留从 2 提升至 3。
- 【圣裁宣告】：效果修改为基础伤害 20/30；此牌被保留时依据手牌数增加伤害。
- 【开核桃大师】：获得 6 点格挡；回合结束时根据格挡数量的一半获得保留。判定要放在覆甲结算之后。
- 【DX-炮弹】：改为获得 3/4 保留，并新增抽 1 张牌。
- 【炸弹来啦】：重做为 3 回合后对全体敌人造成 15 伤害；触发之前每回合开始获得 2 保留。
- 【幸福的bee】：重做为若当前保留不少于 9，则移除 9 层，向随机敌人施加“朱bee”减益，重复 3 次。
  - “朱bee”减益效果：回合开始时获得 1 虚弱，受到 5 点伤害。
- 【噶哦哦】：重做为技能牌，1 费，获得 3 保留，从卡组将一张攻击牌添加到手牌。
- 【面包流星】：移除苦痛施予层数从 6/4 降为 3/2。
- 【魔力喷发】：每层获得格挡从 5/7 改为 6/8。
- 【Rock and Peace】：伤害从 9/12 提升至 10/13。
- 【苦痛施予】和【苦痛爆发】：每层可给予的中毒提升至 3 层，灾厄提升至 6 层。
- 【急速切割】：重做为目标每有一种负面效果，对其造成 5 点伤害并抽 1 张牌；消耗。
- 【土豆番薯】：重做：
  - 新增获得 6/8 苦痛施予。
  - 眩晕需求层数改为 20/16。
  - 消耗层数同样为 20/16。
  - 目标改为所有敌人，不再选择单体施放。
  - 触发眩晕效果之后才消耗苦痛施予。
- 【脑机连接开始】：效果改为造成 5 点伤害；每有 1 张手牌获得 1 覆甲。
- 【魔弹装填】：升级后获得魔弹层数不再增加，仍为 5。
- 【踏光寻月】/【月之领域】：月之领域效果改为每有 1 层月之领域，敌人每有 1 类负面状态，对敌人造成的攻击伤害 +1。
- 【次元定位】：升级后获得活力改为 9。
- 【淬火击】：活力效果倍数从 3/5 降为 2/3。
- 【向前迈进的决心】：进化后每次触发获得活力层数从 2 提升为 3。
- 【要来见少女吗】：升级后变为 0 费。

备注：以上为记录项，尚未实装。实装时需同步 zhs/eng/jpn/kor 本地化、中文 localization_override、Steam mod 目录，并记录详细 changelog。

## 2026-08-27 - Codex

- 任务：继续实装 2026-08-26 记录的大批卡牌/状态调整。
- 已完成：
  - 【围猎标记】图标改为对应使徒头像 `res://CultLeaderMod/images/badges/portraits/纯粹_岚.png`。
  - 【总有一天会发生的事】：新增获得 5/8 层保留；伤害倍率从 5/8 降为 2/3。
  - 【黄油飞射】：描述去掉“加入卡组后开始计数”和“被格挡时也计入”相关文字；未改动已修好的计数/战斗结束兜底变换逻辑。
  - 【呱呱雨】：每回合获得保留从 2/3 提升至 3/4。
  - 【充满魄力的新秀】：给手牌附加的“打出获得保留”从 2 提升至 3。
  - 【圣裁宣告】：基础伤害改为 20/30；被保留时，伤害按本次实际保留的手牌数量提升。
  - 【开核桃大师】：改为获得 6 格挡，并施加新的 `WalnutMasterPower`；该 Power 在 `AfterSideTurnEnd` 读取当前格挡的一半获得保留，设计上晚于覆甲等 `BeforeSideTurnEnd` 结算。
  - 【DX-炮弹】：获得保留改为 3/4，并新增抽 1 张牌。
  - 【炸弹来啦】：改为 1 费技能牌，施加新的 `BombComingPower`；3 回合后对所有敌人造成 15 点伤害，触发前每回合开始获得 2 保留。
  - 【幸福的bee】：改为若当前保留不少于 9，则移除 9 保留，向随机敌人施加 1 层【朱bee】，重复 3 次。
  - 【朱bee】/ `BeePower`：改为敌方 debuff；敌方回合开始时每层获得 1 虚弱并受到 5 伤害，然后减少 1 层。
  - 【噶哦哦】：改为 1 费技能牌，获得 3 保留，并从抽牌堆随机将一张攻击牌移入手牌（移动，不复制）。
  - 【面包流星】：移除苦痛施予从 6/4 降为 3/2。
  - 【魔力喷发】：每层苦痛施予获得格挡从 5/7 提升至 6/8。
  - 【Rock and Peace】：伤害从 9/12 提升至 10/13。
  - 【苦痛施予】和【苦痛爆发】：每层给予的中毒从 2 提升至 3，灾厄从 4 提升至 6。
  - 【急速切割】：改为目标每有一种负面效果，对其造成 5 点伤害并抽 1 张牌；增加消耗关键词；不再移除负面层数。
  - 【土豆番薯】：新增获得 6/8 苦痛施予；若苦痛施予不少于 20/16，则眩晕所有敌人，然后移除 20/16 苦痛施予；不再选择单体目标。
  - 【脑机连接开始】：改为造成 5 点伤害；每有一张其他手牌获得 1 覆甲；不再丢弃手牌。
  - 【魔弹装填】：升级后不再提升魔弹层数，仍为 5；仍保留费用降低。
  - 【踏光寻月】/【月之领域】：月之领域改为攻击敌人时，敌人每有一种负面效果，每层月之领域使攻击伤害 +1；仅攻击牌触发。
  - 【次元定位】：升级后获得活力从 6 提升至 9。
  - 【淬火击】：活力效果倍率从 3/5 降为 2/3。
  - 【向前迈进的决心】：卡牌新增动态变量 `VigorGain`，升级后每次触发获得活力从 2 提升至 3；`ForwardResolvePower` 改为按卡牌配置的触发收益执行。
  - 【要来见少女吗】：升级后变为 0 费。
- 本地化：
  - zhs/eng/jpn/kor 的相关卡牌描述与新 Power 文本已更新。
  - 新增 Power 文本 key：`CULT_LEADER_MOD_POWER_WALNUT_MASTER_POWER.*`、`CULT_LEADER_MOD_POWER_BOMB_COMING_POWER.*`。
- 验证：
  - `dotnet build` 通过。
  - 当前仍只有 4 个既有 warning：`TempMaxHpPower`、`TempMaxHpLossPower`、`LifeEssencePower` 的 nullable oldOwner warning，以及 `Apostle_Melancholy_19` async-without-await warning。
  - 所有 localization JSON 已通过 `ConvertFrom-Json` 语法校验。

## 2026-08-27 - Codex follow-up

- 任务：根据用户测试反馈修正上一批卡牌的升级数值、触发顺序、文本和性格卡/次元定位问题。
- 已完成：
  - 【炸弹来啦】：升级后伤害从 15 提升到 20，触发前每回合获得保留从 2 提升到 3。
  - 【开核桃大师】：升级后格挡从 6 提升到 11。
  - 【噶哦哦】：升级后获得保留从 3 提升到 6；仍然从抽牌堆移动攻击牌到手牌，不复制。
  - 【DX-炮弹】：升级后伤害从 8 提升到 11；保留仍为 3/4。
  - 【幸福的bee】：升级后保留需求和移除值从 9 降为 6。
  - 【总有一天会发生的事】：打出顺序改为先获得 5/8 保留，再读取当前保留层数并设置延迟伤害，确保自身给予的保留计入伤害。
  - 【土豆番薯】：改为只有“打出此牌前已有的苦痛施予”达到 20/16 时才尝试眩晕并消耗苦痛；本卡新获得的 6/8 苦痛不再用于当场凑门槛。
  - 【急速切割】：中文/英日韩描述移除正文中的额外“消耗/Exhaust”，只保留卡牌关键词显示。
  - 【次元定位】：图标改为对应使徒头像 `res://CultLeaderMod/images/badges/portraits/狂热_06.png`。
  - 【次元定位】：新增 `AfterPowerAmountChanged` 损失记录兜底；攻击牌在伤害前消耗/触发的核心 buff（例如【魔力乱打】先触发治愈再造成伤害）也会在攻击结算后尝试恢复。
  - 五张开局性格 Choice 卡的卡图改为明确命名的 `personality_pure/calm/frenzy/lively/melancholy.png`，不再使用 a/b/c/d/e 旧命名路径。
  - 五张战斗用性格卡改为配置 `PowerCmd.Apply<PersonalityCardFetchPower>` 返回的实际实例；`PersonalityCardFetchPower` 改为 `PowerInstanceType.Instanced`，避免多张性格卡共用/覆盖同一个 Power 配置。
- 暂未大改：
  - 【围猎】目前代码只判定 `cardSource?.Type == CardType.Power`，与用户“角色立绘下方 Power 直接造成的伤害也应受加成”的意图不完全一致。已确认这是机制边界问题，等用户明天进一步测试后再决定是否扩展为“非攻击牌/非普通卡牌直伤”识别。
- 验证：
  - `dotnet build` 通过。
  - 当前仍只有 4 个既有 warning，无新增 warning。
  - zhs/eng/jpn/kor localization JSON 通过语法校验，并已同步 Steam mod 目录与 zhs localization_override。

## 2026-08-27 - Codex follow-up 2

- 用户补充：
  - 后续同步/交接时也要包含 `卡牌信息.xlsx`（当前已知路径 `C:\Users\888\Desktop\New_folder\卡牌信息.xlsx`）。
  - 【土豆番薯】应为先获得 6/8 苦痛施予；若苦痛施予达到 20/16，则消耗 20/16 层、眩晕所有敌人并消耗此牌。
  - 五张战斗性格卡的图标仍显示为同一个。
- 已完成：
  - 【土豆番薯】移除固定 `Exhaust` 关键词，改为达标后手动 `CardCmd.Exhaust(choiceContext, this)`；未达标时不会消耗苦痛施予，也不会消耗此牌。
  - 【土豆番薯】zhs/eng/jpn/kor 描述同步为条件消耗文本。
  - `PersonalityCardFetchPower.AfterApplied` 现在会根据 `cardSource` 自动配置性格 tag、升级状态和图标路径，尽量避免 UI 在后置 `Configure()` 前缓存默认纯粹图标。
  - 仍保留五张性格卡中对返回 Power 实例的显式 `Configure(...)`，作为逻辑兜底。
  - `PROJECT_KNOWLEDGE.md` 和长期 memory 记录了 `卡牌信息.xlsx` 同步规则。
- 验证：
  - `dotnet build` 通过，仍只有 4 个既有 warning。
  - localization JSON 通过语法校验。
  - 已同步 Steam mod 目录与 zhs localization_override。

## 2026-08-27 - Codex follow-up 3

- 用户反馈：
  - 控制台添加的五张战斗性格卡 `PERSONALITY_SELECT_*` 打出后，buff 图标仍显示为同一个。
- 已完成：
  - 不再让五张战斗性格卡共用 `PersonalityCardFetchPower` 这一种注册 Power。
  - `PersonalityCardFetchPower` 改为未注册的共享基类，保留通用“回合开始从抽牌堆移动对应性格使徒牌到手牌”的逻辑。
  - 新增并注册五个固定子类：
    - `PersonalityCardFetchPurePower`：纯粹，图标 `personality_pure.png`。
    - `PersonalityCardFetchCalmPower`：冷静，图标 `personality_calm.png`。
    - `PersonalityCardFetchFrenzyPower`：狂热，图标 `personality_frenzy.png`。
    - `PersonalityCardFetchLivelyPower`：活泼，图标 `personality_lively.png`。
    - `PersonalityCardFetchMelancholyPower`：忧郁，图标 `personality_melancholy.png`。
  - 五张 `PersonalitySelect*Card` 分别改为 `PowerCmd.Apply<对应子类Power>`，从类型层面固定图标与抓牌性格。
  - `CardHoverTipsPatch` 中五张性格卡的 hover power 也分别改为对应子类。
 - zhs/eng/jpn/kor 新增五个子类 Power 的 title/description。

## 2026-08-28 - v0.4.01 发布准备

### 发布版本

- 将 `CultLeaderMod.json` 版本号更新为 `v0.4.01`。
- 更新工坊 workspace 的 `workshop.json`：
  - 主简介与四语简介中的卡牌数量同步为 154 张。
  - `changeNote` / `changeNote_localizations` / `changeNote_localizations_short` 更新为 v0.4.01 详细更新日志。
  - 本次更新日志重点说明韩语本地化校正：根据玩家提供的韩语文本文件校正卡牌、能力、遗物、事件和 UI 文本，并与中/英/日文本结构保持同步。

### v0.4.01 更新重点

- 大规模调整五种性格体系下的多张使徒牌，覆盖纯粹、冷静、狂热、活泼、忧郁多条路线。
- 重做/调整多个关键机制：
  - 保留：回合结束时按实际保留张数消耗层数。
  - 存续：根据玩家保留的手牌数对拥有存续的敌人造成伤害。
  - 苦痛施予 / 苦痛爆发：中毒与灾厄数值更新。
  - 月之领域：按敌人负面状态种类提高攻击伤害。
  - 次元定位：尝试恢复下一张攻击牌触发、消耗或移除的增益效果。
- 调整多张卡牌效果与数值，包括：
  - 【帮帮我朋友们】
  - 【今天的目标就是那家伙】
  - 【休假中潜逃】
  - 【警戒线上的幽灵】
  - 【救世主降临】
  - 【随机招募】
  - 【鹿派斩击】
  - 【围猎】
  - 【次元定位】
  - 【终末的爆炸】
  - 【土豆番薯】
  - 【急速切割】
  - 【魔弹装填】
  - 【幸福的bee】
  - 【炸弹来啦】
  - 【噶哦哦】
  - 【DX-炮弹】
  - 【开核桃大师】
- 修复五张战斗用性格卡共用同一个状态图标/状态类型的问题；它们现在分别使用独立 Power，并从抽牌堆移动对应性格的使徒牌到手牌。
- 继续修复本地化 raw key / 代码显示问题，并补充卡牌、能力、遗物、事件、关键词和百科悬浮提示文本。
- 验证：
  - 使用临时 `ModsPath` 的 `dotnet build -p:ModsPath="...\tmp\buildmods\"` 通过，仍只有 4 个既有 warning。
  - 正常 `dotnet build` 的代码编译阶段通过，但 Steam 目录 `CultLeaderMod.dll` 被正在运行的 `SlayTheSpire2.exe (16884)` 锁定，因此未能覆盖 Steam DLL；需要游戏关闭后再执行正常 build/同步。
  - loose localization 已同步到 Steam mod 目录，zhs override 已备份并同步。

## 2026-08-27 - Codex correction

- 用户指出不应修改开局五选二 Choice 卡。
- 已立即回滚此前对 `PersonalityChoicePure/Calm/Frenzy/Lively/MelancholyCard` 卡图路径的修改：
  - 纯粹恢复为 `personality/b.png`
  - 冷静恢复为 `personality/a.png`
  - 狂热恢复为 `personality/d.png`
  - 活泼恢复为 `personality/e.png`
  - 忧郁恢复为 `personality/c.png`
- 验证：
  - 上述五个开局 Choice 卡文件相对 Git 已无 diff。
  - 临时 `ModsPath` 构建通过，仍只有 4 个既有 warning。
- 注意：
  - 后续修五张战斗性格卡 `PersonalitySelect*Card` 时，不要再触碰开局 `PersonalityChoice*Card`。

