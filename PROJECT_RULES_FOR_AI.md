# CultLeaderMod AI 开发规则与避坑知识库

> 这个文件是给新 AI 会话接手项目时优先阅读的长期知识库。  
> 它不是当前任务清单；当前任务看 `CONTINUE_PROMPT_STS2_MOD.md`。  
> 目的：保留旧会话里已经确定的设计范式、用户偏好、踩坑记录和重要备份节点，避免新会话反复踩同样的坑。

## 0. 新会话读取顺序

新会话不要读取完整旧会话 `slay the spire 2 mod（deepseek）`。旧会话已经因为上下文过长触发过 API 报错：

```json
{"error":{"message":"This model's maximum context length is 1048576 tokens. However, you requested 1730857 tokens ...","type":"invalid_request_error","code":"invalid_request_error"}}
```

正确读取顺序：

1. 先读本文档：`PROJECT_RULES_FOR_AI.md`
2. 再读当前任务接力：`CONTINUE_PROMPT_STS2_MOD.md`
3. 再按任务只读相关源码、表格、素材
4. 不要把旧 transcript 整个塞进上下文

## 1. 项目基本信息

- 项目目录：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod`
- Steam 游戏目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2`
- Mod 部署目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod`
- GitHub 仓库：`bathynomus-giganteus/ChubbyCheeks`
- 当前本地分支：`master`
- 当前本地状态：本地 `master` 曾显示比 `origin/master` ahead 2，并且工作区有大量未提交改动。不要擅自 reset / checkout / commit。
- 设计表格：`tmp\card_info.xlsx`
- 旧会话抽取：`C:\Users\888\OneDrive\codex\sts2_old_session_extract.txt`
- 游戏日志：`C:\Users\888\AppData\Roaming\SlayTheSpire2\logs\godot.log`
- ILSpy：`C:\Users\888\.dotnet\tools\ilspycmd.exe`

构建命令：

```powershell
dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj"
```

临时构建，不覆盖 Steam mod：

```powershell
dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj" -p:ModsPath="C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\tmp\buildmods\"
```

正式部署前必须关闭 `SlayTheSpire2.exe`，否则 DLL 可能被占用导致复制失败。

## 2. 重要备份与回滚节点

这些节点来自本地 git 记录；哪些已在 GitHub 远端，以实际远端状态为准。

### 2.1 稳定/里程碑标签

- `fd9937e` / tag `v0.1-stable`  
  初始完整项目：教主 Mod v0.1。

- `3c77864`  
  v0.2：升级系统实装 + 卡牌描述修复。

- `c2b08b1`  
  v0.3：126 张使徒牌升级描述统一 + Power 本地化补全 + 5选2 卡面修正 + 黄瓜油改名。

- `e678bed` / tag `backup-v0.4-20260812-gpt-all-cards`  
  v0.4：GPT 全卡效果实装 + Power 系统重构 + 埃尔德形态修复 + Helper 类。  
  这是一个重要回滚点：如果后续卡牌实现大范围写坏，可以对比这个节点找回 Power/Helper 架构。

- `74c92e7` / tag `v0.5-LifeEssence-TempMaxHp-20260812`  
  LifeEssence 最大生命同步、TempMaxHpPower 标记、debug 方法文档。  
  这是“生命本源/临时最大生命”相关的重要回滚点。

- `1fbe268` / tag `v0.1+基础架构完毕+纯粹使徒测试完毕+文本待修正+20260813-171713` / `origin/master` 本地引用曾指向此处  
  基础架构完毕 + 纯粹使徒测试完毕 + 文本待修正。  
  这是 GitHub 上很可能能找到的稳定备份点之一。

### 2.2 本地较新提交

- `bed3d91`  
  狂热 Hard 批次效果 + Vigor 绑定修复 + 基础牌卡图更新。

- `76711dc`  
  狂热中/困难批次实装与描述动态变量化。

注意：本地曾显示 `master...origin/master [ahead 2]`，说明这两个较新提交可能尚未推送到 GitHub。不要假定远端一定有。

### 2.3 Codex 本地备份

- `C:\Users\888\.codex\backups\sts2-deepseek-old-session.jsonl`  
  旧 DeepSeek 会话备份，体积很大，不要整段读入新会话。

- `C:\Users\888\OneDrive\codex\sts2_old_session_extract.txt`  
  由旧会话解析生成的抽取文件。它包含压缩摘要和最后若干消息，但部分中文在 PowerShell 输出里可能显示为乱码。只在必要时查关键词，不要整体喂给模型。

## 3. 用户工作方式与协作偏好

- 用中文回复。
- 一次只改一个小范围；先检查、计划，再改。
- 每轮修改后构建验证，并给出控制台测试指令。
- 不要顺手重构无关文件。
- 不要全局重写本地化。
- 不要擅自 git commit / push / reset / checkout。
- 保留现有视觉系统：卡框颜色、能量图标、使徒头像徽章。
- 用户会频繁在游戏内实测；每次给出改动后要说明“需要重启游戏 / 重新导入 PCK / 只需 build”等。
- 用户不希望模型接管 Godot UI 操作；涉及图片资源时，优先说明需要 Godot 导入和 PCK 导出。

## 4. 编码与文件写入规则

这是本项目最大坑之一。

- PowerShell 直接写中文源码/路径可能把中文写成 `????` 或乱码。
- 小范围改文件优先用 `apply_patch`。
- 批量脚本如需写中文，必须确保 UTF-8；必要时用 Unicode escape。
- 不要用不确定编码的 `Set-Content` 写中文源码。
- 如果脚本里要处理中文路径，优先从 PowerShell 传环境变量给 Python/Node，不要在脚本字面量里直接写中文路径。
- 旧会话里多次出现中文卡图路径、中文描述被写坏的问题；新会话必须主动防范。

## 5. 本地化规则

主要本地化文件：

- `CultLeaderModCode\Patches\LocInjectPatch.cs`

规则：

- 卡牌 key 形如 `CULT_LEADER_MOD_CARD_APOSTLE_PURE_01.title/description`
- Power key 形如 `CULT_LEADER_MOD_POWER_LIFE_ESSENCE_POWER.title/description`
- 换行用 C# 字符串里的 `\n`
- 不要用 `{NL}`
- 不要在 C# 字符串里写真实换行
- 动态变量名必须和代码里的 `CanonicalVars` 名称匹配
- 常见写法：
  - `{Damage:diff()}`
  - `{Block:diff()}`
  - `{DrawAmt:diff()}`
  - `{Amount:diff()}`
  - `{Energy:energyIcons()}`
- 升级限定文本使用：
  - `{IfUpgraded:show:\n获得{Energy:energyIcons()}。|}`
- 如果 `CanonicalKeywords` 已包含 `CardKeyword.Exhaust`，描述里通常不要重复写“消耗”。
- 不要写泛泛的“升级后：xxx”；尽量用动态变量和 `IfUpgraded`。
- 游戏里不能出现原始 key 或英文占位文本。

## 6. 图片与 Godot 资源规则

- 建议资源文件名统一英文，中文只放在卡牌标题/描述里。
- 避免 `res://.../黄油融化.png` 这类中文资源路径继续扩散。
- 推荐命名：
  - `butter_melt.png`
  - `pat_head.png`
  - `pinch_cheek.png`
  - `chestnut_burst.png`
- 中文桌面路径如 `C:\Users\888\Desktop\新建文件夹\...` 在某些工具/模型环境里会被编码成 `?????`，不要让模型直接“读取/看图”这些路径。
- 已验证 `butter_melt.jpg` 本体没有坏：JPEG / RGB / 1920×1080，Pillow 可读。
- 已验证 `摸摸头.jpg`、`捏捏脸.jpg`、`敲爆栗.jpg` 本体也没有坏。
- 如果把图片复制到项目内并改名为英文，必须让 Godot 重新导入生成 `.import`，并导出 PCK。
- `dotnet build` 只处理 DLL，不保证图片资源进入 PCK。
- 如果项目内有图片但没有 `.import`，游戏里可能仍然加载不到。

## 7. 卡牌与使徒牌设计范式

### 7.1 基本类结构

使徒牌通常继承 `ModCardTemplate`，使用：

- `[RegisterCard(typeof(CultLeaderModCardPool))]`
- `CanonicalTags` 至少包含：
  - `CultLeaderCardTags.Apostle`
  - 对应性格 tag：`Pure / Calm / Frenzy / Lively / Melancholy`
- `CanonicalVars` 放动态数值
- `AssetProfile` 放卡图
- 构造函数里定义费用、类型、稀有度、目标：
  - `base(cost, CardType.Skill, CardRarity.Common, TargetType.Self)`

### 7.2 五性格体系

| 性格 | 基础 Buff | 埃尔德/升级 Buff | 卡框倾向 |
|---|---|---|---|
| 纯粹 | 再生 `RegenPower` | 生命本源 `LifeEssencePower` | 绿色 |
| 冷静 | 覆甲 `PlatingPower` | 固若坚冰 `SolidIcePower` | 蓝色 |
| 狂热 | 活力 `VigorPower` | 狂热 `FervorPower` | 红色 |
| 活泼 | 保留/计划妥当相关 | 幸福 `HappinessPower` | 黄色 |
| 忧郁 | 苦痛 `BitterPainPower` | 苦痛爆发 `BitterPainBurstPower` | 紫色 |

注意：性格是方向标签，不意味着卡牌效果只能使用本性格 buff；以表格和用户描述为准。

### 7.3 教主权能与埃尔德形态

- `CultLeaderAuthorityPower` 最大 5 层。
- 到 5 层时应消耗 5 层并进入 `ElderFormPower`。
- 埃尔德形态不仅拦截之后获得的基础 buff，把它们转为升级 buff；进入瞬间也要把当前已有基础 buff 等量转化为对应升级 buff。
- 旧坑：如果拦截基础 buff 的实现时机不对，会导致使徒牌打出后卡住/不生效。处理思路是：让使徒牌先完成基础效果流程，再由统一 helper/power 规则转换，避免在卡牌流程中间破坏命令队列。
- 新卡获得性格 buff 时优先走 `ApostleCardPlayHelpers.ApplyPurePower/ApplyCalmPower/...` 或 `ApostlePowerRules.ApplyApostlePower`，不要各卡重复写拦截逻辑。

## 8. 衍生牌、奖励池与随机招募规则

旧会话已经确认：

- `CanBeGeneratedInCombat` 会影响战斗内生成/变换，但不天然过滤普通奖励池。
- 衍生牌应保留注册，这样控制台和创建逻辑能找到它。
- 衍生牌应 override：

```csharp
public override bool CanBeGeneratedInCombat => false;
```

- 奖励/变换/随机招募过滤应在卡池或招募逻辑里显式排除 `CanBeGeneratedInCombat == false`。
- 当前相关文件：
  - `CultLeaderModCode\Character\CultLeaderModCardPool.cs`
  - `CultLeaderModCode\Cards\TestAddApostleCards.cs`
- 不要为了过滤衍生牌而取消注册，否则控制台测试和源卡创建衍生牌可能失败。

## 9. 已确认的重要卡牌处理范式

### 9.1 黄油飞射 / 黄油融化

- `Apostle_Lively_05`：黄油飞射。
- `Apostle_Lively_06`：黄油融化，不是音速斩击。
- 黄油飞射应是正常流程中获得黄油融化的唯一渠道。
- 黄油融化不应进入奖励、商店、事件、随机招募等正常卡池。
- 黄油融化应可在百科/控制台测试中存在。
- 用户希望如果好做，查看黄油飞射时悬浮预览黄油融化；不好做可以跳过，不要为此大拆 UI。

### 9.2 音速斩击

- 因 `Apostle_Lively_06` 已用于黄油融化，音速斩击应使用新类，如 `Apostle_Lively_Sonic`。
- 设计：0 费攻击，罕见；丢弃所有手牌，每丢弃一张获得 1 层保留，并对随机敌人造成 6 点伤害；升级后 8 点。
- 这是“每张弃牌各选一个随机敌人攻击一次”，不要误写成固定一个敌人，也不要误用多段攻击语义。

### 9.3 戏剧性演出与三张衍生牌

- `Apostle_Lively_08`：戏剧性演出。
- 三张衍生牌：
  - `Apostle_Lively_08_1`：助手埃皮康
  - `Apostle_Lively_08_2`：埃皮康分身术
  - `Apostle_Lively_08_3`：献给友军
- 三张衍生牌应 `CanBeGeneratedInCombat => false`。
- `EpiconAssistantPower` 旧设计是每回合开始给再生、苦痛施予、活力、覆甲；不要擅自加幸福/保留，除非用户要求。

### 9.4 TEST / 随机加牌 / 洗牌问题

旧坑：

- 曾经把 TEST 添加的牌放入抽牌堆，导致不能正常抽牌/洗牌。
- 后来改成向弃牌堆添加，使原生洗牌能抽出。
- 如果再处理随机生成/加牌，优先遵守原生牌堆流程，不要手写破坏洗牌队列。
- 如果加牌后 UI 卡组数量不实时更新，但用户已决定移除该测试遗物效果，不必继续修这个旧问题。

### 9.5 雪花蝶舞 / 完全格挡计数

- `FullBlockCounterPower` 是隐藏计数器，不要显示在 UI。
- 只统计真正可见 buff 时，不应把隐藏 tracker 算进去。
- 动态伤害预览需要刷新手牌视觉：`NCard.FindOnTable(card)?.UpdateVisuals(...)`。
- 多次完全格挡后的显示数值要和实际伤害一致。

## 10. 常见 API / 实现偏好

- 多段攻击：优先用原生 `WithHitCount`，不要手写循环，除非语义要求每次重新随机目标。
- AOE：旧经验里常用循环敌人逐个 `Targeting(enemy)`，不要随便换成未验证 API。
- 抽牌：`CardPileCmd.Draw(...)`
- 获得能量：`PlayerCmd.GainEnergy(...)`
- 获得 Power：`PowerCmd.Apply<T>(...)`
- 卡牌描述动态变量必须与 `CanonicalVars` 对齐。
- 控制台测试指令格式通常为：

```text
card CULT_LEADER_MOD_CARD_APOSTLE_PURE_19
card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_05
```

## 11. 视觉和卡框规则

- 五性格使徒牌要有不同卡框颜色。
- 用户曾多次反馈卡框太暗、颜色不对；不要随便重写已稳定的卡框逻辑。
- 现有相关文件：
  - `CultLeaderModCode\Patches\CardFrameColorPatch.cs`
  - `CultLeaderModCode\CardTags\CultLeaderFrameColors.cs`
  - `CultLeaderModCode\Cards\ApostleBadge.cs`
- 能量图标、卡框、使徒徽章是独立视觉系统，改卡牌逻辑时不要破坏。

## 12. 当前仍需谨慎的区域

- 活泼、忧郁、冷静仍有不少卡可能是测试模板或半实装。
- 当前工作区有大量未提交文件，包括新 Power、新卡、新图片；不要误删。
- `Apostle_Lively_05/06/Sonic/08` 附近有中文或乱码资源路径风险。
- `黄油融化.png` 当前可能没有 `.import`；若切换英文资源名，必须重新 Godot 导入。
- 旧的 `HANDOFF.md` / `PROJECT_KNOWLEDGE.md` 在某些终端显示乱码；新会话优先读本文档。

## 13. 新会话开场建议

建议对新会话这样说：

```text
请先读取：
1. C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\PROJECT_RULES_FOR_AI.md
2. C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CONTINUE_PROMPT_STS2_MOD.md

不要读取完整旧会话。先总结你理解到的项目规则和当前任务，再检查相关文件。没有确认前不要大范围修改。
```

