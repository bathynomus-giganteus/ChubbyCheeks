# CultLeaderMod 项目知识库

> 来源：从旧会话 `slay the spire 2 mod （deepseek）` 的 61 次压缩摘要、用户消息和项目状态中提炼。
> 用途：新会话/后续维护时先读本文件，避免重复踩坑。
> 与本文件配套的即时交接信息见 `HANDOFF.md`。

## 1. 项目与关键路径

- 项目目录：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod`
- Steam 游戏：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\`
- 部署目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\`
- 游戏 DLL：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll`
- Godot：
  - `C:\Users\888\Tools\MegaDot\4.5.1-m.14\MegaDot_v4.5.1-stable_mono_win64.exe`
  - `E:\game\Godot\Godot_v4.5.1-stable_mono_win64.exe`
  - 另有 4.7.1 console 版本用于 headless export
- 卡牌设计 Excel：`C:\Users\888\Desktop\新建文件夹\卡牌信息.xlsx`（设计规格以此为准）
- 素材目录：`C:\Users\888\Desktop\新建文件夹\坨坨\{纯粹|冷静|狂热|活泼|忧郁}\{使徒名}\`
- 卡牌来源 JSON：`tmp\all_cards.json`、`tmp\card_mapping.json`
- GitHub：`https://github.com/bathynomus-giganteus/ChubbyCheeks.git`，分支 `main`
- ILSpy：`C:\Users\888\.dotnet\tools\ilspycmd.exe`
- Python（Pillow 等）：`C:\Users\888\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe`
- 游戏日志：`C:\Users\888\AppData\Roaming\SlayTheSpire2\logs\godot.log`

## 2. 技术栈与总体架构

- 框架：**RitsuLib only，不使用 BaseLib**。
- 入口：`CultLeaderModCode\Entry.cs`
  - `[ModInitializer]`，注册程序集和 Godot 脚本。
- 角色：`CultLeaderModCode\Character\CultLeaderModCharacter.cs`
  - 继承 `ModCharacterTemplate<CultLeaderModCardPool, CultLeaderModRelicPool, CultLeaderModPotionPool>`。
- 卡池/遗物池/药水池：
  - `CultLeaderModCode\Character\CultLeaderModCardPool.cs`
  - `CultLeaderModCode\Character\CultLeaderModRelicPool.cs`
  - `CultLeaderModCode\Character\CultLeaderModPotionPool.cs`
- 卡牌基类：`CultLeaderModCode\Cards\CultLeaderModCard.cs`
- 使徒卡接口：`CultLeaderModCode\Cards\ApostlePersonality.cs`
  - `IApostleCard` 含 `Personality`、`ApostleName`、`StarIconPath` 等信息。
- 核心辅助类：
  - `ApostleCardHelper.cs`：`ApplyWithAuthority()` 等。
  - `ApostlePowerRules.cs`：`ApplyApostlePower<TBase,TUpgraded>()`。
  - `ApostleCardPlayHelpers.cs`：`ApplyLivelyPower`、`ApplyCalmPower` 等。
  - `ApostleCardEffectHelpers.cs`：`RandomEnemy`、`AliveEnemies`、`TriggerPureStacks`、`RemovePureStacks`、`ApplyTemporaryStrengthLoss` 等。
- 重要 Patch：
  - `Patches\LocInjectPatch.cs`：本地化注入。
  - `Patches\CombatCardFilterPatch.cs`、`Patches\CardFrameColorPatch.cs`。
  - `Cards\ApostleBadge.cs`：头像/徽章映射。
  - `Cards\ApostleUpgradePatch.cs`：升级相关自动处理。

## 3. 五性格与 Buff 体系

| 性格 | 基础 Buff | 埃尔德形态 Buff | 卡框颜色 |
|---|---|---|---|
| 纯粹 Pure | RegenPower 再生 | LifeEssencePower 生命本源 | 绿色 |
| 冷静 Calm | PlatingPower 覆甲 | SolidIcePower 固若坚冰 | 蓝色 |
| 狂热 Frenzy/Fanatic | VigorPower 活力 | FervorPower 狂热 | 红色 |
| 活泼 Lively | RetainHandPower 计划妥当 | HappinessPower 幸福 | 黄色 |
| 忧郁 Melancholy | BitterPainPower 苦痛 | BitterPainBurstPower 苦痛爆发 | 紫色 |

- `苦痛` 是 **Buff（PowerType.Buff）**，即使效果是负面的；不会每回合自然衰减。
- 教主 Authority：`CultLeaderAuthorityPower`，最多 5 层，叠满后转换为 `ElderFormPower`。
- 埃尔德形态：把基础 Buff 转成埃尔德 Buff。
- 初始牌：`CultLeaderManifestation`（教主显灵）提供 Authority。

## 4. 卡牌设计范式

- 每性格 30 张，总计 150 张；先用测试卡铺满，后续实装卡替换同稀有度测试卡槽。
- 卡牌文件命名：`Apostle_{Personality}_{Number}.cs`。
- 类继承 `ModCardTemplate`，打标签：
  - `CultLeaderCardTags.Apostle`
  - 性格标签 `Pure / Calm / Frenzy / Lively / Melancholy`
- 测试卡通用形态：
  - `[CardKeyword.Exhaust]`
  - `EnergyVar(0)`
  - `OnUpgrade()` 通常 +1 能量
- 卡牌数值用 `CanonicalVars`：
  - `DamageVar(...)`、`BlockVar(...)`、`PowerVar<T>(...)`、`MiscVar("Draw", ...)` 等。
- `ApostleName` 与 `PortraitPath` 使用英文文件名映射：`{apostle_en}_card.png`。
- `CanBeGeneratedInCombat`：
  - 只影响战斗内生成/变换，**不影响普通奖励池**。
  - 衍生牌要设 `CanBeGeneratedInCombat => false`。
  - 奖励/变换/随机招募过滤放在 `CultLeaderModCardPool`，但保留在 `ModelDb.AllCards` 以支持控制台测试。
- 5 选 2 性格选择卡不允许出现在百科/奖励中。
- 循环卡 `TestRainbowCard`：五性格 + Apostle 标签，`CardRarity.Basic`，卡框用 shader。

## 5. 卡牌效果约定

- 性格只是方向标签，卡牌效果以卡牌描述为准，不局限于该性格 Buff。
- 斩杀/处决类效果参考原版 **狂宴 Feast**。
- 多段攻击用原生 `WithHitCount`，不要手动重复攻击。
- `小小塞巴斯蒂安` 召唤机制暂缓，需要召唤基础设施。
- 隐藏计数 Power 不要在 UI 中暴露，也不参与可见 Buff 计算；例如 `FullBlockCounterPower`。
- 只做外科手术式修改，不要全局重写无关卡牌或本地化。
- 不要改动 `魔力乱打` 超出已批准修复的范围。

## 6. 本地化规则

- 主机制：`LocInjectPatch.cs` Harmony Postfix 挂在 `LocManager.SetLanguageInternal`，直接注入这些表：
  - `cards`、`characters`、`powers`、`relics`、`gameplay_ui`、`card_keywords`
- Key 格式：
  - 卡牌：`CULT_LEADER_MOD_CARD_{UPPERCASE_CLASS_NAME}.title/description`
  - Power：`CULT_LEADER_MOD_POWER_{TYPE_POWER}.title/description`
  - 关键词：`CULT_LEADER_MOD_KEYWORD_{STEM}.{field}`
- 换行在 C# 字符串里用 `\n`，**不要** `{NL}`，也不要真实换行。
- 动态变量：
  - `{Damage:diff()}`
  - `{Block:diff()}`
  - `{Draw:diff()}`
  - `{Energy:energyIcons()}`
- 升级额外文本：
  - `"text{IfUpgraded:show:\nupgraded text|}"`
  - 整串保持在一行源码里。
- 不要写 `升级后：`。
- 如果 `CanonicalKeywords` 已含 `CardKeyword.Exhaust`，描述里不要重复写“消耗”。
- 游戏内文本必须中文，不能出现原始 key。
- 使徒名/性格信息放 hover tooltip，不放卡牌正文。

## 7. 构建与部署

- 游戏必须关闭后再部署，否则 DLL 被占用。
- 正常部署：
  `dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj"`
- 游戏未关闭时，可以先构建到临时目录：
  `dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj" -p:ModsPath="C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\tmp\buildmods\"`
- 新增/修改图片或场景时，需要 Godot 重新导入并导出 PCK，`dotnet build` 只管 DLL。
- 本地化 JSON 在 PCK 中不可靠，主要依赖 `LocInjectPatch.cs`。
- 写中文文件用 `[System.IO.File]::ReadAllText / WriteAllText`，UTF-8 无 BOM；不要用 `Set-Content`。
- PowerShell 删除文件可能被策略拦截，必要时用 Node.js 脚本。
- 不要 `git commit`，除非用户明确要求。

## 8. 卡图与素材规范

- 卡图模板已冻结，不要改：
  - 4:3 比例
  - 原图居中
  - 左右侧边栏用性格色
- 输出到性格目录和 `big/` 目录。
- 文件名必须英文：`{apostle_en}_card.png`。
- 生成脚本：`Scripts\process_card_art.py`。
- 所有图片必须是 PNG/JPG，**禁止 webp**；转换后可删除 webp 源文件。
- 用户不喜欢拉伸/缩放变形，偏好居中 + padding。
- 使徒头像/徽章：卡牌右上角，约 56×56，由 `ApostleBadge.cs` 处理。
- 肖像路径示例：`res://CultLeaderMod/images/card_portraits/{personality}/{name}_card.png`。

## 9. 关键 API 与命令模式

- 格挡：
  `CreatureCmd.GainBlock(Owner.Creature, (decimal)amount, ValueProp.Move, cardPlay, false)`
- 单体伤害：
  `DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext)`
- AOE：
  循环敌人并逐个 `Targeting(enemy)`；不要用 `TargetingAllOpponents()`。
- 抽牌：
  `CardPileCmd.Draw(choiceContext, count, Owner, false)`
- 消耗：
  `CardCmd.Exhaust(choiceContext, card, false)`
- 眩晕：
  `CreatureCmd.Stun(enemy, "")`
- 玩家手牌：
  `Owner.PlayerCombatState.Hand.Cards`
- 弃牌堆：
  `Owner.PlayerCombatState.DiscardPile`
- 金币：
  `PlayerCmd.GainGold(amount, Owner)`
- 能量：
  `PlayerCmd.GainEnergy(amount, player)`
- 能量费用读取：
  `card.EnergyCost.Canonical`，不要用 `RealCost`。
- Power 应用：
  `PowerCmd.Apply<T>(choiceContext, target, amount, applier, cardSource, silent=false)`
- Power 增减：
  `PowerCmd.ModifyAmount(...)`、`PowerCmd.Decrement(power)`、`PowerCmd.Remove(power)`
- 常见 PowerModel 钩子：
  - `AfterCurrentHpChanged`
  - `AfterDamageReceived`
  - `AfterPlayerTurnStart`
  - `AfterSideTurnStart`
  - `BeforeSideTurnEnd`
  - `AfterSideTurnEnd`
  - `ModifyHpLostBeforeOsty`
  - `ModifyHpLostAfterOsty`

## 10. 用户偏好与协作方式

- 使用中文沟通；卡牌名习惯用 `【】`。
- 偏好一次改一个小点，构建验证后再继续，非常在意回归。
- 要求改动精准，不要顺手格式化或重写无关文件。
- 每轮测试后要列出控制台指令。
- 视觉资源用户会自己在 Godot 里导入/导出，不要擅自接管 Godot UI 工作。
- 需要新增图片时，说明 Godot 导入和 PCK 导出步骤。
- 用户会频繁在游戏内实测；尽量做到每个改动可测试、可验证。
- 没有明确要求时不要提交 Git、不要建分支。

## 11. 当前进度与待办

- 已实装/推进较多：纯粹系、狂热系；冷静系开始测试修复；活泼系正在实装。
- 最近一次关键工作：
  - 冷静 `雪花蝶舞`、`百帕斯卡 挥棒!` 修复。
  - 活泼 `黄油融化`、`戏剧性演出` 及三张衍生牌。
  - `CanBeGeneratedInCombat` 过滤逻辑调整。
- 最近一次部署卡点：游戏运行中，DLL 无法覆盖。
- 最后一次用户要求：
  1. 重新对照 Excel 检查 `循环` 卡逻辑。
  2. 找 `摸摸头`、`捏捏脸`、`爆栗子`、`黄油融化` 的卡图素材。
  3. 尝试在查看 `黄油飞射` 时悬浮显示 `黄油融化` 衍生牌；不好做就保留在百科。
  4. 修正 `黄油融化` 错误使用“提格”头像。
- 已知未完成/需确认：
  - 活泼系许多卡仍是 `ApplyLivelyPower(1) + Draw(1) + GainEnergy(0)` 占位逻辑。
  - `看看我` 的临时最大生命值语义还不是真临时，当前用永久 `GainMaxHp` 代替，需用户确认。
  - 若干 Power 仍缺：`呱呱雨`、`再来一次`、`幸福的bee`、`我想听你讲个故事`、`黄油飞射/黄油融化` 的整局计数逻辑。
  - `魔力乱打` 只能做已批准修复。
  - `Apostle_Melancholy_19` 当前有 `CS1998` warning：外层 `OnPlay` 标记为 `async` 但自身没有 `await`；效果不阻断运行，后续统一清理 warning 时可改为普通 `Task` 并返回 `Task.CompletedTask`。

## 12. 后续平衡/文本改动清单（2026-08-20）

说明：以下卡牌中 `X/Y` 表示升级前/升级后。写卡牌本地化描述时，仿照已有卡牌使用动态数字，不要把可变数值硬编码进描述。

### 纯粹

- 【我来保护你】：获得再生由 5 变为 3，并且不再获得格挡。
- 【铁锹击】：效果描述由“造成最大生命值”改为“造成自身最大生命值”，效果不变。
- 【玛戈玛回复】：若再生层数不足 5，获得的再生由 3 变为 2。
- 【谢绝】：满足条件后减少再生的层数由 2 变为 1。
- 【欧珀粉】：每点再生提供格挡由 1/2 变为 2/3。
- 【快躲开啊】：伤害由 50/65 变为 35/45。
- 【魔女档案】：费用由 3/2 变为 2/1。
- 【要来少女身边吗】：获得格挡由 25 变为 15；文本中数字 4 加粗。
- 【循环】：每一层造成的伤害由 6/9 变为 3/4。

### 狂热

- 【小小塞巴斯蒂安】：效果修改为生命值相当于 `5 + 当前活力的 2/4 倍`；文本中的数字 5 加粗。

### 冷静

- 【蜜瓜吖】：回复额外生命从 14/18 降低为 10/13。
- 【XG-激光】：重做为“对随机敌人造成等同于当前覆甲的伤害 X/X+1 次”。
- 【雪花蝶舞】：重做为基础伤害 8/11；每完全格挡一次敌方攻击，伤害次数 +1；文本中“+1”的数字 1 加粗。
- 【界限模糊】：获得 6 覆甲（升级不再变化）；触发护甲次数变为 2/3。
- 【心中的珍珠】：费用由 1/0 变为 2/1。
- 【限定贴纸】：获得覆甲变为 1/2；最大变化卡牌数量变为 1/2。

### 活泼

- 【次元裂缝】：增添消耗。
- 【蜂蜜鱼~哈姆】：获得保留由 3/5 降为 2/4。
- 【圣裁宣告】：移除保留。
- 【呱呱雨】：描述中的数字 7 加粗。
- 【戏剧性演出】：描述中的数字 3 加粗。

### 忧郁

- 【急速切割】：重做为“移除目标敌人随机 5 层减益状态，对其造成 5/8 点伤害并抽一张牌，最多重复 3/5 次”。
- 【芬多精波动】：根据目标敌人减益层数获得等量再生，不再移除这些减益；费用变为 3/2。
- 【有罪宣言】：伤害从 88/100 变为 50/65。
- 【魔弹装填】：获得 5/8 层魔弹；不再消耗苦痛施予；增加 2 张【魔弹の射手】进入抽牌堆；显示文字中数字 2 加粗/改色。
- 【魔弹の射手】：描述中“否则将本牌送入消耗堆”改为“否则消耗”。
- 【月之领域】：重做为“攻击具有减益效果的敌人时，伤害 +X”。这里的 X 假设为当前【月之领域】层数，最好做成动态数字；描述使用引号内文字。

### 文本表现待调研

- 用户希望上述 7 个“加粗数字”后续如果可行，改成类似动画的动态彩色字体；若实现复杂或风险高，可以保持加粗。
- 当前项目未发现卡牌本地化层面已有动态彩色字体范式；实现前需先确认 STS2/RitsuLib 文本渲染是否支持富文本颜色、动画或自定义 inline token。

### 第一批低风险修改进度

- 2026-08-20 已实装并通过 `dotnet build`：纯数值、费用、关键词、简单描述和动态变量格式修正。
- 已改内容包括：【我来保护你】、【铁锹击】、【玛戈玛恢复】、【谢绝】、【欧珀粉】、【快躲开啊】、【魔女档案】、【要来少女的身边吗？】、【循环】、【小小塞巴斯蒂安】、【蜜瓜吖】、【界限模糊】、【心中的珍珠】、【限定贴纸!】、【次元裂缝】、【圣裁宣告】、【蜂蜜鱼~哈姆!】、【魔弹の射手】描述、【有罪宣言】。
- 2026-08-20 回归修正：【要来少女的身边吗？】再生固定 4 且升级后仍为 4；【圣裁宣告】移除自带保留但保留“被保留时依当前保留层数增加伤害”的特效与描述；【呱呱雨~】持续 7 回合保持普通数字，是否加粗/彩色另走文本渲染专项。
- 2026-08-20 已继续实装并通过 `dotnet build`：【XG-激光】、【雪花蝶舞】、【急速切割】、【芬多精波动】、【魔弹装填】、【月之领域】。
- 实现解释：
  - 【XG-激光】：按 X 费牌处理，造成当前覆甲数值的随机敌人伤害，命中次数为 X / X+1。
  - 【雪花蝶舞】：基础伤害 8/11，命中次数为 1 + 本场战斗完全格挡敌方攻击次数。
  - 【急速切割】：每次先尝试移除目标随机 5 层减益；若至少移除 1 层，则造成 5/8 伤害并抽 1 张，最多重复 3/5 次；若目标没有可移除减益则停止。
  - 【芬多精波动】：统计目标敌人全部减益层数，获得等量再生，不再移除目标减益，费用 3/2。
  - 【魔弹装填】：获得 5/8 层魔弹；不再消耗苦痛施予；固定将 2 张【魔.弹.の.射.手】加入抽牌堆，费用 3/2。
  - 【月之领域】：攻击具有减益效果的敌人时，伤害 + 月之领域层数。
- 2026-08-20 已撤回 BBCode 加粗 `[b]...[/b]`。经只读检查，原版 PCK 中大量使用 `[gold]...[/gold]`、`[blue]...[/blue]`、`[red]...[/red]`、`[green]...[/green]`、`[purple]...[/purple]`、`[orange]...[/orange]`、`[aqua]...[/aqua]`、`[pink]...[/pink]` 等游戏自定义颜色标签；当前已在部分卡牌描述中试用这些色名标签。若游戏内仍不显示颜色，说明当前卡牌描述渲染路径未启用富文本解析，需要改渲染/解析层，而不是继续换标签。

### 2026-08-20 本轮细节修正

- 【要来少女的身边吗？】已添加消耗关键词，描述追加“消耗”。
- 【快躲开啊啊!!!噫…?】效果原本已结束回合，本轮仅在描述中补充“结束你的回合”。
- 【魔弹装填】添加【魔.弹.の.射.手】、【魔.弹.の.射.手】回到抽牌堆、以及添加【终.末.の.爆.炸】均从抽牌堆顶改为普通加入抽牌堆（`CardPilePosition.Random`）。
- 【有罪宣言】费用降低计数从战斗全局 `DebuffAppliedTrackerPower.GetTotal` 改为卡牌实例自己的 `DebuffApplied` 动态变量；战斗中途生成的新卡从 0 开始，打出后清零。隐藏 tracker 仍保留给其他卡使用，并负责通知各牌堆里的【有罪宣言】实例自增。
- 【月之领域】Power 描述改为“攻击具有减益效果的敌人时，伤害+{Amount}。”以显示当前层数。

### 2026-08-21 待调整记录

- 用户记录：【芬多精波动】和【调皮的笑容】的“层数”统计口径后续改为“种类数”。实现时不要继续按减益/状态的总层数累计；应统计不同状态/减益的类型数量，并同步修改本地化描述与动态数字含义。
- 用户记录：【敲爆栗】的费用 +1 效果应持续到本场战斗结束，而不是只影响下一次；实现时需要同步写入卡牌描述。
- 用户记录：【芬多精波动】在改为“种类数”统计后，升级前后费用均 -1（即基础与升级版各自降低 1 费），并同步更新本地化描述/费用显示。
- 用户记录：【执行教理】应该是攻击牌，而不是技能牌；实现时同步修改卡牌类型、筛选/奖励表现，以及中英日韩本地化中的类型相关文本。
- 用户记录：战斗结束后【教主】和【建筑师】的对话目前没有实装上；后续需要检查战斗结束/胜利后对话触发点、角色限定条件与本地化文本。
- 用户记录：百科界面的筛选按钮，按钮本体再向右移动大约一个按钮高度的距离；弹出菜单的字体可以减小一些。

### 2026-08-22 已确认范式：性格卡牌检索

- 五张【性格卡牌】的效果语义固定为：回合开始时，从抽牌堆随机将一张对应性格使徒牌移入手牌。
- 实现位置：`CultLeaderModCode/Powers/PersonalityCardFetchPower.cs`。
- 实现要点：
  - 检索 `PileType.Draw.GetPile(player).Cards`，不要检索 `PileType.Deck`。
  - 对抽牌堆中的现有 `CardModel` 调用 `CardPileCmd.Add(selected, PileType.Hand, ...)`，让游戏移动该卡。
  - 不要使用 `CombatState.CloneCard`，否则会变成复制卡牌，和设计不符。
  - 升级版仍允许在移入手牌前对选中卡执行 `CardCmd.Upgrade(..., CardPreviewStyle.None)`。
- 文本同步：
  - zhs/eng/jpn/kor 的五张性格卡牌描述与 `PersonalityCardFetchPower` 描述都要写成“抽牌堆 / draw pile”与“移入 / move”。

### 2026-08-24 本地化表归属范式：遗物文本必须在 relics.json

- 遗物的 `CULT_LEADER_MOD_RELIC_*.title / description / flavor` 必须放在各语言 `relics.json`，不能放在 `cards.json`。
- 已修复案例：
  - `CULT_LEADER_MOD_RELIC_CLEAR_WEATHER_CARD_RELIC.*`（天气晴朗卡 / 太阳卡）
  - `CULT_LEADER_MOD_RELIC_BUTTER_YELLOW_CARD_RELIC.*`（黄油的黄牌 / 黄油卡片）
- 症状：如果遗物文本误放入 `cards.json`，游戏遗物界面会显示 raw key / 代码。
- 检查命令范式：
  - `cards.json` 中不应出现 `_RELIC_` key。
  - `relics.json` 中应包含所有 `CULT_LEADER_MOD_RELIC_*` key。
- 特别注意：
  - 中文环境还可能被 `C:\Users\888\AppData\Roaming\SlayTheSpire2\localization_override\zhs\relics.json` 的旧表覆盖；若修复后仍显示 key，检查 override 是否缺 key。

### 2026-08-24 黄油飞射变换范式

- 【黄油飞射】计数到 100 后应变为【黄油融化】。
- 受击时即时变换之外，还需要战斗结束兜底：
  - `AfterCombatVictory`：胜利结算时若永久卡组中的【黄油飞射】已经 `DamageTaken >= 100`，执行变换。
  - `BeforeRoomEntered`：再次检查，防止胜利结算时机被游戏流程跳过。
- 兜底检查必须限制 `base.Pile?.Type == PileType.Deck`，只转换永久卡组里的真实卡牌，避免误处理战斗中临时对象或预览对象。

### 2026-08-26 韩文本地化校正规则

- 用户提供的韩语校对稿目录：`C:\Users\888\Desktop\New_folder\korean loc`。
- 该目录中的 `.txt` 文件实际为 JSON 表，可用于校正项目 `CultLeaderMod/localization/kor/*.json`。
- 应用规则：
  - 只覆盖当前项目中已存在且 key 完全匹配的条目。
  - 不要把源文件中的旧式/额外 key 直接加入项目。
  - 即使源 `cards.txt` 含有 `CULT_LEADER_MOD_RELIC_*`，遗物 key 仍必须放入 `kor/relics.json`，不能回流到 `kor/cards.json`。
  - 修改后检查：`kor/cards.json` 中 `_RELIC_` key 数应为 0，`kor/relics.json` 应包含 66 个遗物字段。
- 当前备份点：`.codex_backups\kor_loc_before_20260826-115322`。

### 2026-08-26 卡牌调整与保留机制范式

- 【帮帮我朋友们】当前效果：触发最多 3/5 层治愈，获得 8 格挡，对所有敌人造成 8/11 点伤害；本场战斗中每触发 1 层治愈，当前牌伤害 +1；不再获得临时力量。
- 【今天的目标就是那家伙】/ `PirateMarkPower`：只允许攻击牌造成的 powered attack 伤害触发，必须检查 `cardSource?.Type == CardType.Attack`。
- 【要来见少女吗】/ `FrenzyOnHealPower`：费用 1；每恢复 1 HP 或获得 1 层治愈，获得 1 活力；埃尔德形态下通过 `ApostlePowerRules.ApplyApostlePower<VigorPower, FervorPower>` 转为狂热。
- 【雪雾】/ `FlatDamageReductionPower`：必须在格挡结算前减少“怪物将要造成的攻击伤害”，不要用实际 HP 损失做减免。当前实现使用 `ModifyDamageAdditive`，并要求玩家当前有格挡。
- 【保留】/ `RetainPower` 当前范式：回合结束时选择至多等于当前保留层数的手牌保留；只消耗实际选择张数的保留层数，选 0 不消耗。描述固定为“每消耗一层可以在回合结束时保留一张卡牌”。
- 本轮其他数值：
  - 【里科塔全套餐】升级后每层回复 3。
  - 【蜜瓜吖】覆甲满足条件后额外回复 5/8。
  - 【胡萝卜治愈】获得 4/6 保留，回复 4/6 生命，消耗。
  - 【松鼠雷电】获得 3 保留。
  - 【蜂蜜鱼】获得 3/5 保留；若保留不少于 15/12，回复 5 生命，消耗。
  - 【有罪宣言】带保留关键词。
  - 【魔.弹.の.射.手】魔弹耗尽时消耗所有手牌/抽牌堆/弃牌堆里的同名衍生牌，再加入【终.末.の.爆.炸】。
  - 【终.末.の.爆.炸】伤害 55/60。
- 中文本地化 override 注意：
  - `C:\Users\888\AppData\Roaming\SlayTheSpire2\localization_override\zhs` 会覆盖项目和 Steam loose JSON；每次更新 zhs 文本后若游戏进程中仍显示旧文本/raw key，应优先检查并同步该目录。
  - 2026-08-26 本轮已备份旧 override 到 `.codex_backups\zhs_override_before_20260826_batch_*` 并覆盖为当前 zhs 表。

### 2026-08-26 存续机制范式

- 【警戒线上的幽灵】当前数值：1 费攻击牌，对所有敌人造成 8/10 点伤害，并给予 1 层【存续】。
- 【存续】/ `ExtantPower` 当前效果：
  - 回合结束保留结算后读取 `AfterFlush(... retainedCards)`。
  - 玩家本次每保留 1 张手牌，拥有【存续】的敌人每层受到 3 点伤害。
  - 公式：`3 × retainedCards.Count × ExtantPower.Amount`。
  - 不再在敌人受到攻击伤害时追加伤害；不要恢复旧的 `ModifyHpLostBeforeOsty` 逻辑。
- 【休假中潜逃】当前效果：
  - 获得 4/6 治愈和 4/6 格挡。
  - 本回合保留你的手牌，实现方式为 `PowerCmd.Apply<RetainHandPower>(..., amount: 1)`。
  - 该效果应被【存续】读取到，因为【存续】统计的是原生回合末 `retainedCards`。

### 2026-08-26 次元定位 / 恢复攻击牌消耗 buff 的实现范式

- 不要为了“恢复攻击牌消耗的 buff”去全局拦截所有 `PowerCmd.ModifyAmount` 或卡牌移动流程；之前 TEST/洗牌问题说明这类全局改动很容易污染原生流程。
- 当前固定方案：`DimensionPositionPower` 在下一张攻击牌进入 `ModifyDamageAdditive` 时快照玩家核心正面状态，在 `AfterAttack` 后把低于快照值的层数补回，并消耗 1 层自身。
- 当前恢复白名单：
  - 教主五系基础/升级状态：治愈、生命本源、覆甲、固若坚冰、活力、狂热、保留、幸福、苦痛施予、苦痛爆发。
  - 常见基础正面状态：力量、敏捷、人工制品、缓冲。
- 这个方案恢复的是“攻击结算后相比攻击前减少的层数”，而不是精确累计所有中间负向 delta；如果未来出现“同一张攻击牌同时消耗并新增同种 buff”的设计，再考虑更细粒度的 delta tracker。

### 2026-08-26 围猎标记范式

- 【围猎】使用 `AbilityDamageTakenBonusPower` 实现“能力对目标造成的伤害增加 X%”。
- `Amount` 直接表示百分比（50/75），允许叠加。
- 当前触发条件：目标是该 Power 的拥有者、伤害来源为玩家、`cardSource?.Type == CardType.Power`。
- 这意味着只有通过能力牌本身造成的伤害会吃加成；如果以后要让“PowerModel 后续自动伤害”也吃加成，需要额外定义来源识别方式。

### 2026-08-27 活泼/忧郁/狂热批量调整范式

- 【噶哦哦】这类“从抽牌堆找牌到手牌”的效果按用户偏好应优先移动现有战斗牌对象，不复制；实现方式参考 `CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Top, source, false)`。
- 【圣裁宣告】的“被保留加伤害”由隐藏 `RetainCardCounterPower.AfterFlush` 读取本次 `retainedCards.Count`；不要改回读取保留层数。
- 【开核桃大师】使用 `WalnutMasterPower.AfterSideTurnEnd` 在覆甲等 `BeforeSideTurnEnd` 之后读取最终格挡，按 `floor(Block / 2)` 获得保留。
- 【炸弹来啦】使用 `BombComingPower.AfterPlayerTurnStart` 做倒计时；`Amount > 1` 时获得保留并递减，`Amount == 1` 时造成全体伤害并移除自身。
- 【朱bee】/ `BeePower` 现在是敌方 debuff，不是玩家召唤物；敌方回合开始时按层数施加虚弱和伤害，然后递减。
- 【月之领域】现在只增强玩家攻击牌对敌人的攻击伤害：加成为 `月之领域层数 × 目标负面状态种类数`。不要改回“只要有负面就 +Amount”的旧逻辑。
- 【向前迈进的决心】的触发收益从 `ForwardResolvePower` 常量改为由卡牌动态变量 `VigorGain` 配置，升级后应为 3。

### 2026-08-27 性格卡与次元定位修正范式

- 五张开局性格 Choice 卡应使用明确命名图片：
  - 纯粹 `personality_pure.png`
  - 冷静 `personality_calm.png`
  - 狂热 `personality_frenzy.png`
  - 活泼 `personality_lively.png`
  - 忧郁 `personality_melancholy.png`
- 五张战斗用性格卡应用 `PowerCmd.Apply<PersonalityCardFetchPower>` 的返回值并立即 `Configure(...)`，不要用 `owner.GetPower<PersonalityCardFetchPower>()` 配置，否则多实例时可能配置到旧 Power。
- `PersonalityCardFetchPower` 必须允许 `PowerInstanceType.Instanced`，否则多个性格抓牌效果会互相覆盖，表现为图标/抓取性格像同一个。
- 【次元定位】不能只依赖 `ModifyDamageAdditive` 时的快照；部分攻击牌（如【魔力乱打】）会在真正造成伤害前先触发/消耗 buff。当前范式是：
  - 继续在伤害修正阶段拍快照；
  - 同时在 `AfterPowerAmountChanged` 中记录由攻击牌导致的核心 buff 负向变化；
  - `AfterAttack` 时按快照缺口与记录损失的较大值恢复，避免重复恢复。

### 2026-08-27 卡牌信息表同步规则

- 用户要求后续也同步/交接 `卡牌信息.xlsx`。
- 当前已知路径：`C:\Users\888\Desktop\New_folder\卡牌信息.xlsx`。
- 在整理 DeepSeek/Codex 交接 prompt、核对卡牌实现、生成测试清单或推送大版本前，应提醒检查该表是否有新版本，并尽量保持代码、本地化、日志与表内设计一致。

### 2026-08-27 土豆番薯条件消耗规则

- 【土豆番薯】不是固定消耗牌。
- 当前规则：先获得 6/8 层苦痛施予；若此后苦痛施予达到 20/16，则移除 20/16 层苦痛施予，眩晕所有敌人，并通过 `CardCmd.Exhaust(choiceContext, this)` 消耗此牌。
- 未达标时不移除苦痛施予，也不消耗此牌。

### 2026-08-27 五张战斗性格卡 Power 拆分类规则

- 不要再让 `PersonalitySelectPure/Calm/Frenzy/Lively/MelancholyCard` 共用同一种注册 Power，否则运行时图标/合并/缓存容易表现为同一个 buff。
- 重要边界：`PersonalitySelect*Card` 是战斗用性格卡；`PersonalityChoice*Card` 是开局五选二 Choice 卡。修战斗性格卡时不要改动开局 Choice 卡图或逻辑。
- 当前范式：
  - `PersonalityCardFetchPower` 作为未注册共享基类，包含通用抽牌堆移动逻辑。
  - 五个注册子类分别固定 `FetchTag` 与 `PersonalityIconPath`：
    - `PersonalityCardFetchPurePower`
    - `PersonalityCardFetchCalmPower`
    - `PersonalityCardFetchFrenzyPower`
    - `PersonalityCardFetchLivelyPower`
    - `PersonalityCardFetchMelancholyPower`
  - 五张性格卡分别 `PowerCmd.Apply<对应子类Power>`。
  - `CardHoverTipsPatch` 也应映射到对应子类 Power。
- 如果之后仍同图标，优先检查 Godot 导入/PCK/资源缓存，而不是再回到单 Power + Configure 的写法。
