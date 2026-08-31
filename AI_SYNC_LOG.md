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


## 2026-08-28 - 建筑师通关对话 key 修复

- 修复教主与建筑师通关对话未生效：日志显示真实角色 entry 为 `CULT_LEADER_MOD_CHARACTER_CULT_LEADER_MOD_CHARACTER`，旧本地化误写为 `CULT_LEADER_MOD`。
- 在 zhs/eng/jpn/kor `ancients.json` 中追加正确 `THE_ARCHITECT.talk.CULT_LEADER_MOD_CHARACTER_CULT_LEADER_MOD_CHARACTER.*` 键，保留旧键兼容。
- 将 Architect attacker 标记统一为可解析枚举值 `Both`，避免日/韩本地化把枚举写成自然语言后解析失败。
- 四个 `ancients.json` 已通过 JSON 语法校验，并已手动同步到 Steam loose localization；zhs override 也已同步。
- 代码编译阶段通过；正常 build 仍因 `SlayTheSpire2.exe` 锁定 Steam 目录 DLL 而无法覆盖 DLL，但本次对话修复本身主要依赖 loose localization。

## 2026-08-28 - 本地同步完成

- 用户关闭游戏后重新执行 dotnet build，构建与复制均成功，Steam mod 目录 CultLeaderMod.dll 已更新。
- 当前已同步改动包括：埃尔德形态下低级性格 buff 漏转换修复、【团体跳级】升级后消耗、建筑师通关对话正确 key。
- 静态复查：未再发现直接 PowerCmd.Apply<HealingPower/PlatingPower/VigorPower/RetainPower/BitterPainPower> 的漏点。
- zhs/eng/jpn/kor `ancients.json` 均通过 JSON 语法校验。

## 2026-08-28 - 固若坚冰触发不再消耗层数

- 用户确认【固若坚冰】无论主动触发还是被动触发，都不应消耗层数。
- 修改 `SolidIcePower.TriggerActive()`：主动触发仍根据当前层数获得格挡，但不再 `ModifyAmount(-1)` 扣除固若坚冰。
- 被动回合结束获得格挡原本就不消耗层数，本次保留该逻辑。
- 同步 zhs/eng/jpn/kor `powers.json` 描述，移除“主动触发移除1层”的说法。
- 已同步 zhs `localization_override` 的 `powers.json`。
- 验证：`dotnet build` 通过并完成 `Copying mod...`；仍只有 4 个既有 warning，无新增错误。

## 2026-08-29 - 魔力乱打单卡攻击动画原型

- 用户提供 `C:\Users\888\Downloads\ErpinRoyale_composition.gif` 和 Trickcal Studio 项目 JSON，要求先给【魔力乱打】加入“角色主立绘不动、角色旁边原位播放使徒攻击动画”的测试效果。
- 当前未接 Spine；先采用更稳的 GIF 拆帧方案验证打牌动画管线：
  - 将 GIF 拆成 85 张 420x420 透明 PNG 帧，输出到 `CultLeaderMod/images/vfx/magic_strike/frame_000.png` 至 `frame_084.png`。
  - 新增 `CultLeaderModCode/Vfx/ApostleVfxPlayer.cs`，运行时创建临时 `CanvasLayer + TextureRect`，按 24fps 播放帧图，播放完自动 `QueueFree()`。
  - 在 `Apostle_Pure_01`【魔力乱打】实际有效触发时调用 `ApostleVfxPlayer.PlayMagicStrikeBesidePlayer()`；若没有可触发的治愈/生命本源层数导致卡牌无效果，则不播放动画。
- 验证：
  - `dotnet build` 通过并完成 `Copying mod...`，仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck` 成功，Godot 扫描、导入并打包 85 帧动画资源。
- 后续若测试位置/大小/时长不合适，优先调整 `ApostleVfxPlayer` 中的 `Position`、`Scale`、`MagicStrikeFrameSeconds`，不必改卡牌逻辑。

## 2026-08-29 - 魔力乱打卡牌查看页 idle 立绘原型

- 用户提供 `C:\Users\888\Downloads\ErpinRoyale_Idle_2.gif`，要求尝试在卡牌查看页面显示该角色立绘。
- 当前仍先采用 GIF 拆帧方案验证 UI 挂载入口：
  - 将 idle GIF 拆成 41 张 360x360 透明 PNG 帧，输出到 `CultLeaderMod/images/vfx/magic_strike_preview/frame_000.png` 至 `frame_040.png`。
  - 新增 `CultLeaderModCode/Patches/CardInspectApostlePreviewPatch.cs`。
  - Patch `NInspectCardScreen.UpdateCardDisplay()`：当当前大卡为 `Apostle_Pure_01`【魔力乱打】时，在查看屏左侧添加循环播放的 `TextureRect`；切换到其他卡或关闭查看屏时移除。
- 验证：
  - `dotnet build` 通过并完成 `Copying mod...`；仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck` 成功，Godot 扫描、导入并打包 41 帧 idle 预览资源。
- 注意：
  - 该功能目前只针对【魔力乱打】。
  - 编译能验证类型和资源打包，但 `NInspectCardScreen` 是否覆盖百科/卡组查看的所有入口仍需进游戏实测。
  - 后续位置/大小可优先调 `CardInspectApostlePreviewPatch.PositionPreview()` 和 `PreviewSize`。

## 2026-08-29 - 魔力乱打攻击动画镜像修正

- 用户测试指出【魔力乱打】打出时，使徒攻击动画方向反了。
- 已水平镜像 `CultLeaderMod/images/vfx/magic_strike/frame_000.png` 至 `frame_084.png`。
- 本次只调整打出卡牌时播放的攻击动画；卡牌查看页的 `magic_strike_preview` idle 立绘未修改。
- 验证：`dotnet build /t:ExportPck` 成功，Godot 打包 0 warning / 0 error；Steam mod 目录 `CultLeaderMod.pck` 已更新。

## 2026-08-29 - 魔力乱打卡牌查看页立绘位置调整

- 用户测试指出【魔力乱打】卡牌查看页左侧 idle 立绘位置会遮挡左方向键。
- 调整 `CardInspectApostlePreviewPatch.PositionPreview()`：
  - 横向锚点从屏幕宽度 `0.22` 改为 `0.16`。
  - 纵向锚点从屏幕高度 `0.50` 改为 `0.38`。
  - 预览尺寸仍保持 360x360。
- 当前方案仍为 GIF 拆帧 `TextureRect` 循环播放，未接入 Spine。
- 验证：游戏关闭后重新执行 `dotnet build` 与 `dotnet build /t:ExportPck`，均 0 warning / 0 error；Steam mod 目录 `CultLeaderMod.dll` 与 `CultLeaderMod.pck` 均已更新。
- 后续方向：
  - 若继续使用 GIF/PNG 帧，可以快速批量扩展到更多卡牌。
  - 若改为 Spine，需要先确认 STS2/Godot 工程可用的 Spine runtime 或可导入格式，再封装为可复用的 `ApostlePreview`/`ApostleVfx` 节点。
  - 互动功能建议等 Spine/节点挂载稳定后再做，可通过 `Control.GuiInput`、透明点击区域、idle/touch/attack 动画状态机实现。

## 2026-08-29 - 贝拉立绘与打牌动画原型

- 用户要求实装“贝拉”的立绘和动画，并提醒攻击动画需要镜像。
- 资源定位：
  - 项目内 `CardHoverTipsPatch` 显示“贝拉”对应 `Apostle_Lively_12`。
  - 网页 `character-names.json` 确认中文“贝拉”对应资源名 `Vela`，不是 `Belita`（贝丽塔）。
  - 已从 Journey Studio 公开资源下载 `Vela` 原始 Spine 素材到 `C:\Users\888\Desktop\New_folder\坨坨\活泼\贝拉`：`Vela.skel.bytes`、`Vela.atlas.txt`、`Vela.png`，供后续正式 Spine 化使用。
- 当前仍采用已验证的 GIF/PNG 帧方案作为可测原型：
  - 新增 `CultLeaderMod/images/vfx/vela_preview/frame_000.png` 至 `frame_040.png`，用于卡牌查看页左侧循环立绘。
  - 新增 `CultLeaderMod/images/vfx/vela_attack/frame_000.png` 至 `frame_023.png`，用于打出卡牌时的短促攻击动画；生成后已水平镜像。
  - `ApostleVfxPlayer` 重构为通用 `PlayFrameVfxAsync()`，保留【魔力乱打】调用，并新增 `PlayVelaGhostBesidePlayer()`。
  - `Apostle_Lively_12`【警戒线上的幽灵】打出时调用贝拉攻击动画。
  - `CardInspectApostlePreviewPatch` 重构为小型 profile 注册表，当前支持【魔力乱打】和【警戒线上的幽灵】两张卡的查看页立绘。
- 验证：
  - `dotnet build` 通过；仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck` 成功，Godot 导入并打包新增 65 帧资源，0 warning / 0 error。
  - Steam mod 目录 `CultLeaderMod.dll` 与 `CultLeaderMod.pck` 已更新。
- 注意：
  - 这次不是正式 Spine 运行时接入，而是用网页公开素材生成的临时帧动画。
  - 贝拉预览图目前来自网页角色卡片/头像风格，带少量 UI 元素；后续接原始 Spine 后可替换为更干净的动态立绘。

## 2026-08-29 - 使徒攻击动画随机位置与图层叠放规则

- 用户要求使徒攻击动画随机出现在教主身边的小范围内，中心大致为教主立绘中心偏下；动画图层在教主图层上方；若多个动画同时播放，新动画应叠在旧动画上方。
- 修改 `ApostleVfxPlayer`：
  - 新增统一锚点 `PlayerLowerCenterAnchor = (0.30, 0.42)`，作为当前屏幕比例近似的教主立绘偏下位置。
  - 新增随机偏移范围 `PlayerBesideJitterRange = (90, 55)`，每次播放都会在该范围内随机落点。
  - 新增全局 `_vfxSequence`，每个动画创建独立 `CanvasLayer`，层级为 `BaseVfxLayer + sequence % VfxLayerCycle`。
  - 动画节点名称追加 sequence，避免同时播放时节点重名。
- 当前该规则覆盖所有通过 `ApostleVfxPlayer.PlayFrameVfxAsync()` 播放的使徒动画，包括【魔力乱打】与【警戒线上的幽灵】。
- 验证：`dotnet build` 通过；仍只有 4 个既有 warning。`dotnet build /t:ExportPck` 成功，Godot 打包 0 warning / 0 error；Steam mod 目录 DLL/PCK 已更新。
- 后续如能稳定获取玩家/教主立绘节点坐标，可将当前屏幕比例锚点替换为真实角色节点世界坐标。

## 2026-08-29 - 使徒动画素材命名规则与贝拉素材替换

- 用户纠正贝拉素材用反：`动画.gif` 才是打牌/战斗动画，`立绘.gif` 才是卡牌查看页立绘。
- 已将贝拉旧的网页临时帧替换为用户整理目录中的正式 GIF：
  - 源目录：`C:\Users\888\Desktop\New_folder\坨坨\活泼\贝拉`
  - `动画.gif` -> `CultLeaderMod/images/vfx/vela_attack/frame_000.png` 至 `frame_078.png`，共 79 帧，420x420，已水平镜像，用于【警戒线上的幽灵】打出动画。
  - `立绘.gif` -> `CultLeaderMod/images/vfx/vela_preview/frame_000.png` 至 `frame_192.png`，共 193 帧，360x360，不镜像，用于卡牌查看页立绘。
- 代码同步：
  - `ApostleVfxPlayer.PlayVelaGhostBesidePlayer()` 的帧数改为 79，播放速度改为约 30fps。
  - `CardInspectApostlePreviewPatch` 中【警戒线上的幽灵】预览 profile 改为 193 帧，播放速度约 30fps。
- 以后所有使徒卡牌动画默认遵守该规则：
  - `C:\Users\888\Desktop\New_folder\坨坨\<性格>\<使徒>\动画.gif` = 打出卡牌/战斗中出现的攻击动画，默认镜像。
  - `C:\Users\888\Desktop\New_folder\坨坨\<性格>\<使徒>\立绘.gif` = 卡牌查看/百科/卡组详情页面左侧立绘，默认不镜像。
  - 如果某张卡没有这两个固定文件，再考虑使用同目录其他 GIF/Spine/PNG 资源临时代替。
- 验证：`dotnet build` 通过；仍只有 4 个既有 warning。`dotnet build /t:ExportPck` 成功，Steam mod 目录 DLL/PCK 已更新。

## 2026-08-29 - 冷静使徒动画第一批接入

- 用户确认按“小重构 + 先接几张卡测试”的方案实装冷静使徒动画。
- 新增 `CultLeaderModCode/Vfx/ApostleAnimationProfiles.cs`：
  - 集中维护卡牌类型到战斗动画/查看页立绘 profile 的映射。
  - `ApostleVfxPlayer` 改为通过 `ApostleAnimationProfiles.TryGetBattleProfile()` 播放；旧的 `PlayMagicStrikeBesidePlayer()`、`PlayVelaGhostBesidePlayer()` 保留为兼容包装。
  - `CardInspectApostlePreviewPatch` 改为通过 `ApostleAnimationProfiles.TryGetPreviewProfile()` 查找查看页立绘。
- 第一批接入 5 张冷静使徒卡：
  - `Apostle_Calm_01`：`calm_01_attack` 46 帧；`calm_01_preview` 41 帧。
  - `Apostle_Calm_03`：`calm_03_attack` 65 帧；`calm_03_preview` 9 帧。
  - `Apostle_Calm_05`：`calm_05_attack` 68 帧；`calm_05_preview` 41 帧。
  - `Apostle_Calm_13`：`calm_13_attack` 101 帧；`calm_13_preview` 37 帧。
  - `Apostle_Calm_25`：`calm_25_attack` 45 帧；`calm_25_preview` 81 帧。
- 源素材遵守固定规则：
  - `C:\Users\888\Desktop\New_folder\坨坨\冷静\<使徒>\动画.gif` -> 战斗动画，420x420，水平镜像。
  - `C:\Users\888\Desktop\New_folder\坨坨\冷静\<使徒>\立绘.gif` -> 卡牌查看页立绘，360x360，不镜像。
- 本批只给这 5 张卡的 `OnPlay` 开头增加动画播放调用，不修改卡牌实际效果数值与机制。
- 验证：
  - `dotnet build` 通过；仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck` 通过，Godot 0 warning / 0 error。
  - Steam mod 目录已同步：`CultLeaderMod.dll` 时间为 2026-08-29 16:52:40，`CultLeaderMod.pck` 时间为 2026-08-29 16:52:43。
- 注意：
  - 本次只接 5 张卡，PCK 已增至约 117MB；后续若全量接入 26 张冷静使徒，建议先考虑降帧/降分辨率/改 Spine 或运行时读取策略，否则包体会明显膨胀。

## 2026-08-29 - 外部运行时读取动画实验：Apostle_Calm_01

- 用户要求先试验“运行时读取”动画帧，避免大量使徒动画全部进入 PCK 导致包体膨胀。
- 新增 `CultLeaderModCode/Vfx/ExternalVfxTextureLoader.cs`：
  - 支持从普通磁盘路径读取 `frame_000.png` 等帧图。
  - 读取方式为 Godot `Image.Load(...)` + `ImageTexture.CreateFromImage(...)`。
  - 帧贴图会按外部文件绝对路径缓存，避免同一帧反复从磁盘解码。
  - 候选目录包括 DLL 所在目录、`AppContext.BaseDirectory/mods/CultLeaderMod`、当前目录下的 `mods/CultLeaderMod` 等。
- 修改 `BattleVfxProfile` 与 `PreviewProfile`：
  - 新增 `ExternalFrameDirectory` 可选字段。
  - `ApostleVfxPlayer` 与 `CardInspectApostlePreviewPatch` 均改为外部目录优先，找不到才回退到 `res://`。
- 本次只对 `Apostle_Calm_01` 做运行时读取实验：
  - 外部战斗动画目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\external_vfx\calm_01_attack`，46 帧。
  - 外部查看页立绘目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\external_vfx\calm_01_preview`，41 帧。
  - 为验证确实走外部读取，`Apostle_Calm_01` 的 PCK 兜底路径临时指向 `res://CultLeaderMod/images/vfx/_external_runtime_probe_missing/...`。如果游戏里阿雅动画正常播放，即证明外部读取成功。
- 验证：
  - `dotnet build` 通过，仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck` 通过，Godot 0 warning / 0 error。
  - Steam mod 目录已同步：`CultLeaderMod.dll` 时间为 2026-08-29 17:02:59，`CultLeaderMod.pck` 时间为 2026-08-29 17:03:03。
- 注意：
  - 当前只是本地实验；若后续确认可行，需要把 `external_vfx` 纳入 Workshop 打包/上传流程，否则工坊版本可能缺少外部帧文件。
  - 工程内旧 `calm_01_*` 帧目录暂未删除；本次通过临时不存在的兜底路径验证外部优先读取，避免被 PCK 残留资源误判。

## 2026-08-29 - 使徒动画全量外部资源接入

- 用户确认外部运行时读取路线可行，并要求把目前素材目录中可找到的其他动画都接入。
- 本次将使徒动画从 PCK 内部帧资源改为 `external_vfx` 外部目录优先读取：
  - 工程目录：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\external_vfx`
  - 本地测试目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\external_vfx`
  - 工坊工作目录：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\release\workshop\CultLeaderModWorkspace\content\external_vfx`
- 生成结果：
  - 当前共 30 组唯一使徒素材 profile。
  - 共 4440 张 PNG 帧，约 370.25 MB。
  - `external_vfx/.gdignore` 已加入，避免 Godot 把这些外部运行时资源导入并塞进 PCK。
  - 已确认项目目录、本地 mod 目录、工坊 content 目录均没有 `.import` 文件。
- 代码结构：
  - `CultLeaderModCode/Vfx/ApostleAnimationProfiles.cs` 统一维护卡牌到动画 profile 的映射。
  - `CultLeaderModCode/Patches/ApostleAnimationPlayPatch.cs` 在 `CardModel.OnPlayWrapper` 前缀中自动触发卡牌对应的使徒战斗动画。
  - 旧的单卡 `OnPlay` 手动动画调用已移除，避免同一张卡播放两次动画。
  - `CultLeaderMod.csproj` 已把 `external_vfx/**/*` 纳入 build 后复制流程，`dotnet build` 会同步到本地 Steam mod 目录。
- 当前接入范围：
  - 纯粹：仅 `Apostle_Pure_01` 使用 `埃尔芬（王道）` 素材；普通 `Apostle_Pure_15` 暂不自动复用王道素材。
  - 冷静：`Apostle_Calm_01` 至 `Apostle_Calm_26`，其中 `艾米莉娅` 对应用户素材目录 `阿梅利亚`。
  - 活泼：`Apostle_Lively_08`、`Apostle_Lively_08_1`、`Apostle_Lively_08_2`、`Apostle_Lively_08_3`、`Apostle_Lively_11`、`Apostle_Lively_12`。
  - 狂热/忧郁：当前源素材目录未发现可用 GIF，暂未接入。
- 重要范式：
  - `C:\Users\888\Desktop\New_folder\坨坨\<性格>\<使徒>\动画.gif` = 战斗/打牌动画，默认水平镜像。
  - `C:\Users\888\Desktop\New_folder\坨坨\<性格>\<使徒>\立绘.gif` = 卡牌查看/百科/卡组详情页立绘，默认不镜像。
  - 不要试图用统一自动裁剪/统一缩放解决所有角色大小问题；用户会在测试后指出具体哪张卡需要单独调 scale/offset/canvas。
  - Steam 创意工坊可以上传 DLL/PCK 之外的额外资源目录，因此 `external_vfx` 应随 workshop content 一起发布。
- 验证：
  - `dotnet build` 成功，0 warning / 0 error。
  - `dotnet build /t:ExportPck` 成功，0 warning / 0 error。
  - 本地测试目录与工坊工作目录均已同步最新 `CultLeaderMod.dll`、`CultLeaderMod.pck` 和 `external_vfx`。

## 2026-08-29 - 修复外部动画帧文件名不兼容

- 用户测试反馈：冷静大部分卡牌看不到卡牌查看页立绘，卡牌动画也不显示；纯粹/活泼部分卡也有类似问题。
- 原因确认：
  - `external_vfx` 中实际生成的帧文件名为 `frame_0.png`、`frame_1.png` 等不补零格式。
  - `ExternalVfxTextureLoader` 原本只寻找 `frame_000.png`、`frame_001.png` 等三位补零格式。
  - 因此外部目录存在、帧文件也存在，但运行时大部分查找失败。
- 修复：
  - `ExternalVfxTextureLoader.ResolveExternalFramePath()` 现在同时尝试 `frame_{0:000}.png` 与 `frame_{0}.png` 两种命名格式。
  - 这是全局加载器修复；虽然用户要求先看冷静，但也会顺带修复纯粹/活泼已有外部 profile 的同类问题。
- 验证：
  - `dotnet build` 成功，仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck` 成功，0 warning / 0 error。
  - 本地 Steam mod 目录已更新：`CultLeaderMod.dll` 时间 17:39:46，`CultLeaderMod.pck` 时间 17:39:48。
  - `external_vfx` 仍为 4441 个文件 / 4440 张 PNG / 0 个 `.import`。

## 2026-08-29 - 修复卡牌查看页使徒立绘残留

- 用户测试反馈：大部分冷静立绘已能显示，但很多立绘第一次显示后不会消失，再查看其他卡牌时会残留在屏幕上。
- 修复：
  - `CardInspectApostlePreviewPatch` 中的预览节点新增统一 group：`CultLeaderInspectApostlePreviews`。
  - 创建预览时写入 alive meta；移除时先标记 dead、隐藏、清空 Texture、移出 group，再 `QueueFree()`。
  - 动画循环会检查 alive meta，避免节点已经等待释放时继续换帧“诈尸”。
  - 新增全局清理：切换/更新卡牌显示时，不只清理当前 `NInspectCardScreen` 下的节点，也会递归扫描场景树中所有同名预览节点，移除不属于当前查看界面的旧节点；这可以清理旧版本未加入 group 的残留节点。
- 同时保留上一次修复：
  - 外部帧文件名兼容 `frame_000.png` 和 `frame_0.png`。
  - profile 查找兼容运行时派生/包装类型。
- 验证：
  - `dotnet build` 成功，仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck` 成功，0 warning / 0 error。
  - 本地 Steam mod 目录已更新：`CultLeaderMod.dll` 时间 2026-08-29 21:58:11，`CultLeaderMod.pck` 时间 2026-08-29 21:58:14。

## 2026-08-29 - 全卡使徒动画改为 manifest 驱动接入

- 用户确认卡牌查看页立绘不会再残留，并要求把其他卡牌相关动画都按同样方法实装。
- 本次将动画映射从手写 profile 列表改为 `external_vfx_manifest.json` 驱动：
  - `CultLeaderModCode/Vfx/ApostleAnimationProfiles.cs` 启动时读取 manifest。
  - manifest 中的 `classes` 会解析为 `CultLeaderMod.CultLeaderModCode.Cards.<ClassName>`，并统一注册战斗动画与卡牌查看页立绘。
  - profile 查找仍保留派生/包装类型兼容逻辑。
- 素材来源：
  - `C:\Users\888\Desktop\New_folder\坨坨\<性格>\<使徒>\动画.gif`：战斗/打牌动画，生成时默认水平镜像。
  - `C:\Users\888\Desktop\New_folder\坨坨\<性格>\<使徒>\立绘.gif`：卡牌查看/百科/卡组详情页立绘，不镜像。
  - 若没有 `动画.gif`，本次临时使用 `动画1.gif` 作为战斗动画来源，例如 `黄油`、`x锡安x`、`双雄相争`、`为了艾鲁皮恩`；后续用户可指定是否改用 `动画2.gif` / `动画3.gif`。
- 生成结果：
  - manifest 共 134 个唯一动画 profile。
  - 134 个 profile 均有战斗动画。
  - 132 个 profile 有卡牌查看页立绘。
  - `DualRivalsCard` / `ForElruienCard` 目前只有战斗动画，因为素材目录中没有 `立绘.gif`。
  - `external_vfx` 当前约 266 个目录、18881 张 PNG、1644.32 MB。
- 重要范式：
  - 不做全局透明裁切/自动缩放；用户已经确认各攻击动画的角色大小、特效范围差异很大，后续按具体卡牌反馈单独调 scale/offset/canvas。
  - PCK 只打入 `external_vfx_manifest.json`，不打入 `external_vfx` 大帧目录；`external_vfx/.gdignore` 必须保留。
  - Workshop content 需要带上 `external_vfx` loose files，否则工坊版本没有动画帧。
- 验证：
  - `dotnet build /v:minimal` 成功，仍只有 4 个既有 warning。
  - `dotnet build /t:ExportPck /v:minimal` 成功，Godot 0 warning / 0 error。
  - 本地测试目录已更新：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\CultLeaderMod.pck` 约 40.75 MB。
  - 工坊 staging 已同步：`release\workshop\CultLeaderModWorkspace\content` 下包含最新 DLL/PDB/PCK、manifest 与 `external_vfx`。

## 2026-08-29 - 卡牌动画三档设置开关

- 用户担心全量 PNG 帧资源过大，并要求在设置里加入卡牌动画开关，同时先继续测试 PNG 版本。
- 本次新增三档动画模式：
  - `完全关闭`：不播放使徒牌战斗动画，也不显示卡牌查看页/百科使徒立绘动画。
  - `仅保留稀有卡`：只对 `Rare` 与 `Ancient` 卡保留战斗动画和查看页立绘。
  - `完全保留`：全量启用。默认值暂定为 `完全保留`，方便当前测试 PNG 全量可用性。
- 新增文件：
  - `CultLeaderModCode/Vfx/CultLeaderAnimationSettings.cs`
    - 保存/读取用户设置到 `%APPDATA%\CultLeaderMod\settings.json`。
    - 提供 `Allows(CardModel card)` 统一判断动画是否允许。
  - `CultLeaderModCode/Vfx/CultLeaderSettingsPage.cs`
    - 使用 RitsuLib `RegisterModSettings` 注册“教主 Mod 设置”页面。
    - 在“视觉效果”section 中加入“卡牌动画”下拉选项。
- 已接入：
  - `ApostleAnimationPlayPatch`：打牌时先检查设置，再播放战斗动画。
  - `CardInspectApostlePreviewPatch`：查看卡牌时先检查设置，再显示立绘动画；不允许时会清理旧预览节点。
- Spine 迁移调查：
  - 游戏根目录存在 `E:\SteamLibrary\steamapps\common\Slay the Spire 2\libspine_godot.windows.template_release.x86_64.dll`，说明 STS2/Godot 环境有 Spine 运行时相关支持。
  - 后续应先做单卡 Spine 原型，确认 `.skel.bytes` / `.atlas.txt` / `.png` 的加载方式、节点类型、动画状态机和图层控制，再替换当前 PNG 帧管线。
- 验证：
  - 正常 `dotnet build` 的 C# 编译阶段通过，但正式复制到 Steam mod 目录失败，因为用户正在运行游戏，`SlayTheSpire2.exe` 锁定 `CultLeaderMod.dll`。
  - 使用临时 `ModsPath` 的完整 build 成功，0 warning / 0 error。
  - 因游戏未关闭，本次设置开关尚未同步到正式本地测试目录；等用户关闭游戏后需要重新 `dotnet build` + `ExportPck` 并同步 workshop staging。

## 2026-08-29 - 精简【强制起床装置】百科悬浮框

- 用户反馈【强制起床装置】相关词条过多，百科页面额外窗口过多，要求参考【循环】处理。
- 修改 `CardHoverTipsPatch`：
  - 从 `PowerTipsByCard` 中移除 `Apostle_Melancholy_11` 对易伤、虚弱、脆弱、中毒、灾厄的逐个 PowerTip 展示。
  - 在 `CompactStatusTipsByCard` 中新增 `Apostle_Melancholy_11` 合并提示：`易伤  虚弱  脆弱  中毒  灾厄  苦痛施予`。
  - 效果与【循环】一致：只显示一个“相关状态”框，避免百科/查看页生成过多悬浮窗口。
- 验证：
  - 使用临时 `ModsPath` 执行 `dotnet build /v:minimal` 成功。
  - 仍只有 4 个既有 warning，0 error。
  - 因用户正在测试/游戏可能仍在运行，本次未覆盖正式 Steam mod 目录；待用户关闭游戏后同步。

## 2026-08-30 - Journey Spine 资源下载脚本
- 新增脚本 `Scripts/Download-JourneySpineAssets.ps1`，用于从 `https://journey.927927927.xyz/spine-manifest.json` 读取资源清单，并从 `https://assets.927927927.xyz/spine/` 下载 Spine runtime 三件套到 `E:\work\Cult_leader_mod\SPINE`。
- 脚本支持扫描 `E:\work\Cult_leader_mod\坨坨` 下的 `动画.json` / `立绘.json`，也支持 `-ResourceCode ErpinRoyale` 手动指定资源；支持 `-DryRun` 只列计划不下载，避免误耗流量。
- 默认只下载 `.skel.bytes` / `.atlas.txt` / 贴图文件；如需 Unity `.asset/.meta` 等附加文件，可使用 `-IncludeDataFiles`。

## 2026-08-30 - Spine 第一批资源下载
- 已通过 `Scripts/Download-JourneySpineAssets.ps1` 下载第一批 Journey Spine runtime 资源到 `E:\work\Cult_leader_mod\SPINE`。
- 第一批 ResourceCode：`Allet`, `Erpin`, `ErpinRoyale`, `Daya`, `BigWood`, `Delia`, `Arco`, `Epica`, `Vela`, `Amelia`, `Ashur`, `AshurMagi`。
- 下载结果：命中 25 套完整资源，SPINE 目录当前约 57.55 MB；只下载 `.skel.bytes` / `.atlas.txt` / `.png`，未下载 Unity `.asset/.meta` 附加文件。
- 已生成 `E:\work\Cult_leader_mod\SPINE\name-match-report.csv`，用于记录本地使徒中文文件夹名与 Journey ResourceCode 的自动匹配情况；仍有若干中文名需要人工确认。

## 2026-08-30 - 埃尔芬（王道）Spine 原型接入
- 新增 `CultLeaderModCode/Vfx/ApostleSpinePrototype.cs`，仅针对 `Apostle_Pure_01` / 埃尔芬（王道）做 Spine runtime 原型。
- 原型使用 `E:\work\Cult_leader_mod\SPINE\战斗模型\erpinroyale` 的 `ErpinRoyale.skel.bytes` / `ErpinRoyale.atlas.txt` / `ErpinRoyale.png` 播放战斗动画 `Skill1_1_End`，并使用 `E:\work\Cult_leader_mod\SPINE\正常使徒\erpinroyale` 播放卡牌查看立绘 `Idle_1`。
- 为避免误判，`Apostle_Pure_01` 命中 Spine 原型后不再 fallback 到 PNG 帧动画；失败时只写 `[SPINE_PROTO]` 日志，不播放旧 PNG。
- `ApostleVfxPlayer.PlayForCard` 已在普通 PNG 帧播放前优先调用 Spine 原型；`CardInspectApostlePreviewPatch` 对该卡优先挂载 Spine preview 节点。
- `dotnet build /v:minimal` 通过，保留项目既有 4 个 warning。

## 2026-08-30 - 修复 Spine 原型未触发
- 首次测试中日志没有出现 `[SPINE_PROTO]`，说明不是 Spine 资源加载失败，而是播放入口未触发。
- 原因：`Apostle_Pure_01` 等大量使徒牌继承 `ModCardTemplate`，不是 `CultLeaderModCard`；旧入口 `if (__instance is CultLeaderModCard)` 会跳过这些卡。
- 已将 `ApostleAnimationPlayPatch` 改为对 `CardModel` 使用 `CultLeaderAnimationSettings.Allows(__instance)` 后直接按 `__instance.GetType()` 播放。
- 已将 `CardInspectApostlePreviewPatch` 改为按命名空间 `CultLeaderMod.CultLeaderModCode.Cards` 识别本 mod 卡牌，不再要求 `CultLeaderModCard` 基类。
- `dotnet build /v:minimal` 通过，保留既有 4 个 warning。

## 2026-08-30 - ErpinRoyale Spine 不显示原因确认
- 测试后日志出现 `[SPINE_PROTO]`，说明动画入口已触发。
- 失败根因：Journey 下载的 `ErpinRoyale.skel.bytes` 为 Spine `4.1.08`，但 STS2 当前 `libspine_godot.windows.template_release.x86_64.dll` runtime 为 Spine `4.2`，日志明确报错 `Skeleton version 4.1.08 does not match runtime version 4.2`。
- 已给 `ApostleSpinePrototype` 增加 skeleton 版本预检；版本不兼容时跳过 Spine 原型并回退当前 PNG-frame VFX，避免【魔力乱打】动画被实验代码挡掉。
- 后续若要正式 Spine 化，需要获取/导出 Spine 4.2 兼容的 `.skel.bytes` / `.atlas.txt` / `.png`，或确认可用的 4.1 runtime 接入方式；不建议替换 STS2 自带 native Spine runtime。

## 2026-08-31 - 个别异常 Spine 资源回退到 GIF 帧

- 用户确认 `ChopiManualUvFixA/B`、`ChopiManualUvDimA`、`ChopiAllRotateUvA` 等 atlas/UV 修复尝试仍不能把乔菲立绘修到可接受状态，因此停止继续硬修这批异常 Spine 资源。
- 当前决策：只有个别问题角色回退到 GIF/PNG 帧，其余角色继续使用 Spine。
- 已强制从 Spine prototype 排除并改走 `external_vfx_manifest.json` 帧动画的卡牌：
  - `Apostle_Calm_23` / 蕾特
  - `Apostle_Lively_13` / 修罗
  - `Apostle_Melancholy_10` / 洛涅（市长）
  - `Apostle_Melancholy_25` / 乔菲
  - `Apostle_Melancholy_26` / 欧若拉
- 新增脚本 `Scripts/Build-SelectedGifFallbackVfx.py`：
  - 从 `E:\work\Cult_leader_mod\坨坨` 读取上述角色的 `动画.gif` 与 `立绘.gif`。
  - 战斗动画默认镜像，导出到 `external_vfx/<key>_attack/frame_000.png...`。
  - 百科/查看页立绘不镜像，导出到 `external_vfx/<key>_preview/frame_000.png...`。
  - 同步 `external_vfx_manifest.json` 和选中帧目录到本地 mod、Steam Workshop 订阅目录、Workshop staging。
- 本次生成帧数量：
  - `calm_23_attack` 81 张，`calm_23_preview` 96 张
  - `lively_13_attack` 96 张，`lively_13_preview` 41 张
  - `melancholy_10_attack` 56 张，`melancholy_10_preview` 41 张
  - `melancholy_25_attack` 30 张，`melancholy_25_preview` 21 张
  - `melancholy_26_attack` 85 张，`melancholy_26_preview` 49 张
- 验证：
  - `dotnet build /v:minimal` 成功，0 error，仍只有 4 个既有 warning。
  - 本地 mod、Workshop 订阅目录、Workshop staging 三处 DLL 哈希一致：`AC5FFAE9FFD5F554146C7CF02796F145BF648624F6D8E709D5353E6A2FCC3074`。
  - 三处均存在上述 10 个 `external_vfx` 帧目录。

## 2026-08-31 - GIF 帧动画显示微调

- 针对用户反馈的 GIF 立绘偏大、Spine 卡切换到 GIF 卡时旧 Spine 立绘残留、欧若拉战斗 GIF 偏大/帧率低/无淡出，做了以下处理：
  - `CardInspectApostlePreviewPatch`：GIF/PNG 帧立绘尺寸从 `360x360` 缩小到 `300x300`。
  - `CardInspectApostlePreviewPatch`：在显示 GIF/PNG 帧 preview 前先调用 `ApostleSpinePrototype.RemoveAllPreviews()`，修复从 Spine preview 切到 GIF preview 时旧 Spine 节点不消失的问题。
  - `ApostleVfxPlayer`：战斗帧动画改为播放前加载到 `BattleFrameCache`，避免首次逐帧从 loose PNG 读取造成卡顿感。
  - `ApostleVfxPlayer`：战斗帧动画最后 `0.5s` 开始边播放边淡出，播放结束后直接移除节点。
  - `ApostleAnimationProfiles`：欧若拉 `melancholy_26` 战斗动画单独缩放为 `0.95`，帧间隔调整为 `1/40s`。
- 验证：
  - `dotnet build /v:minimal` 成功，0 error，仍只有 4 个既有 warning。
  - 本地 mod、Workshop 订阅目录、Workshop staging 三处 DLL 哈希一致：`1C3F0C495056E55EA26EF44CB98750CC98F2508766B0241EB20DAA012B1B190C`。

## 2026-08-31 - GIF 立绘逐角色位置与尺寸微调

- 用户反馈修罗 GIF 立绘位置合适但可略微放大，其他四个 GIF 回退角色需要往左上移动并略微缩小。
- 修改 `ApostleAnimationProfiles.PreviewProfile`：新增 `FrameSize` 与 `PositionOffset`，让 GIF/PNG 帧立绘支持逐角色调参。
- 当前参数：
  - 修罗 / `lively_13`：`315x315`，位置不偏移。
  - 蕾特 / `calm_23`：`280x280`，位置偏移 `(-28, -24)`。
  - 洛涅（市长） / `melancholy_10`：`280x280`，位置偏移 `(-28, -24)`。
  - 乔菲 / `melancholy_25`：`280x280`，位置偏移 `(-28, -24)`。
  - 欧若拉 / `melancholy_26`：`280x280`，位置偏移 `(-28, -24)`。
- 验证：
  - `dotnet build /v:minimal` 成功，0 error，仍只有 4 个既有 warning。
  - 本地 mod、Workshop 订阅目录、Workshop staging 三处 DLL 哈希一致：`231BBABF1D5FD65AB04CFB27F6A80759FE02D837028592FAEFF053F12793D954`。
