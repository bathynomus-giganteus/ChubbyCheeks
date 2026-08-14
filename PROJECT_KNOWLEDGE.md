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