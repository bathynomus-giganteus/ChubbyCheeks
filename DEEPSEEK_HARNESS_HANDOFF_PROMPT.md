# DeepSeek Harness 交接 Prompt：CultLeaderMod / 教主 Mod

> 生成时间：2026-08-20  
> 适用场景：在 DeepSeek harness 中继续开发 Slay the Spire 2 的 CultLeaderMod 项目。  
> 重要原则：以当前本地工程文件为最高权威；DeepSeek harness 无法读取 Codex 软件内旧会话，因此旧会话信息必须以本项目内的交接文件/知识库为准。

## 直接复制给 DeepSeek harness 的 Prompt

```text
你现在接手 Slay the Spire 2 的 CultLeaderMod（教主 Mod）项目。这个项目之后会由 Codex 和 DeepSeek 共同维护，因此你的首要任务不是“凭记忆重写”，而是读取当前工程文件、遵守项目范式、小步修改、构建验证，并把你的进度写入共享交接文件。

一、当前工程路径和外部环境

项目根目录：
C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod

Steam 游戏目录：
E:\SteamLibrary\steamapps\common\Slay the Spire 2

Mod 部署目录：
E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod

游戏日志：
C:\Users\888\AppData\Roaming\SlayTheSpire2\logs\godot.log

设计表格：
C:\Users\888\Desktop\新建文件夹\卡牌信息.xlsx
如果这个路径不存在，也检查项目内 tmp\card_info.xlsx、tmp\all_cards.json、tmp\card_mapping.json。

素材根目录：
C:\Users\888\Desktop\新建文件夹\坨坨\

GitHub：
https://github.com/bathynomus-giganteus/ChubbyCheeks

当前最近一次 Codex 发布状态：
- 分支：cultleader-card-tuning-20260820
- 提交：59e716a Balance and refine apostle card mechanics
- 已推送到 GitHub 远端分支。
- Steam Workshop 物品 ID：3784977251
- 工坊 changeNote：v0.2.05

注意：README / CultLeaderMod.json 当前仍可能显示 v0.2.04，但工坊 changeNote 已是 v0.2.05。不要擅自改版本号，除非用户明确要求版本发布。

二、你必须先读取这些文件，不要跳过

请按顺序读取：

1. PROJECT_KNOWLEDGE.md
2. PROJECT_RULES_FOR_AI.md
3. AI_SYNC_LOG.md（如果存在；不存在则本轮结束时创建）
4. TEST_ISSUES.md（了解已知测试问题）
5. README.md（了解对外说明，但注意它可能滞后）
6. 当前任务相关源码文件

以下旧会话存在于当前 Codex 软件中，DeepSeek harness 通常无法读取：
- “教主mod DS”
- “你现在接手 Slay the Spire 2 的 CultLeaderMod 项目。 重要：不要读取完整旧会话 sla…”
- “slay the spire 2 mod （deepseek）”

不要要求 DeepSeek harness 读取这些 Codex app 会话；不要把这些会话当作可访问资料源。旧会话里的重要信息已经尽量提炼到 `PROJECT_KNOWLEDGE.md`、`PROJECT_RULES_FOR_AI.md`、`CONTINUE_PROMPT_STS2_MOD.md`、`HANDOFF_DEEPSEEK_SESSION.md` 和本文件中。凡是旧会话记忆、旧摘要与当前源码冲突，以当前源码、`AI_SYNC_LOG.md`、`PROJECT_KNOWLEDGE.md` 最新追加内容为准。

三、当前项目核心架构

框架：
- RitsuLib only，不使用 BaseLib。
- 入口：CultLeaderModCode\Entry.cs
- 角色：CultLeaderModCode\Character\CultLeaderModCharacter.cs
- 卡池：CultLeaderModCode\Character\CultLeaderModCardPool.cs
- 遗物池：CultLeaderModCode\Character\CultLeaderModRelicPool.cs
- 卡牌目录：CultLeaderModCode\Cards
- Power 目录：CultLeaderModCode\Powers
- 本地化注入：CultLeaderModCode\Patches\LocInjectPatch.cs
- 使徒徽章/头像映射：CultLeaderModCode\Cards\ApostleBadge.cs
- 卡框颜色：CultLeaderModCode\Patches\CardFrameColorPatch.cs
- 奖励/生成过滤：CultLeaderModCode\Character\CultLeaderModCardPool.cs、CombatCardFilterPatch.cs 等

五种性格与体系：
- 纯粹 Pure：基础 RegenPower，再生；升级 LifeEssencePower，生命本源；绿色。
- 冷静 Calm：基础 PlatingPower，覆甲；升级 SolidIcePower，固若坚冰；蓝色。
- 狂热 Frenzy：基础 VigorPower，活力；升级 FervorPower，狂热；红色。
- 活泼 Lively：基础 RetainPower / 计划妥当相关；升级 HappinessPower，幸福；黄色。
- 忧郁 Melancholy：基础 BitterPainPower，苦痛施予；升级 BitterPainBurstPower，苦痛爆发；紫色。

埃尔德形态：
- CultLeaderAuthorityPower 权能到 5 层进入 ElderFormPower。
- 进入埃尔德形态时，要把当前已有五种基础 buff 等量转换为升级 buff。
- 在埃尔德形态期间，获得基础 buff 应被拦截为升级 buff。
- 以前踩过坑：不要用会打断原生卡牌打出流程的方式拦截，否则使徒牌会卡在屏幕中央。正确方向是保留卡牌 OnPlay 流程，只在 Power 应用层做转换。

四、当前已实装/近期完成的关键改动（不要回退）

最近 Codex 已完成并构建通过的 v0.2.05 改动包括：

纯粹：
- 【要来少女的身边吗？】：15/20 格挡，固定 4 再生，已添加消耗。
- 【我来保护你】：获得 3 再生，抽牌；不再获得格挡。
- 【玛戈玛恢复】：低于阈值时获得 2 再生。
- 【谢绝】：满足条件后减少 1 层再生。
- 【欧珀粉】：每点再生提供 2/3 格挡。
- 【快躲开啊啊!!!噫…?】：35/45 伤害；效果已结束回合，描述也补了“结束你的回合”。
- 【魔女档案】：费用 2/1。
- 【循环】：每层伤害 3/4。

狂热：
- 【小小塞巴斯蒂安】：生命池为 5 + 当前活力的 2/4 倍。

冷静：
- 【蜜瓜吖】：额外回复 10/13。
- 【XG-激光】：X 费，对随机敌人造成等同当前覆甲的伤害 X/X+1 次。
- 【雪花蝶舞】：8/11 伤害，次数 = 1 + 本场战斗完全格挡敌方攻击次数。
- 【界限模糊】：固定获得 6 覆甲，触发覆甲 2/3 次。
- 【心中的珍珠】：费用 2/1。
- 【限定贴纸】：获得覆甲 1/2，最多变化 1/2 张牌。

活泼：
- 【次元裂缝】：添加消耗。
- 【圣裁宣告】：移除自带保留关键词，但保留“被保留时依当前保留层数增加伤害”的特效与描述。
- 【蜂蜜鱼~哈姆】：获得保留 2/4。
- 【呱呱雨~】：持续 7 回合，文本中的 7 试用了颜色/动态效果。
- 【戏剧性演出】：只第一个 3 试用颜色/动态效果，第二个 3 保持普通文本。

忧郁：
- 【魔弹装填】：3/2 费；获得 5/8 魔弹；添加 2 张【魔.弹.の.射.手】进入抽牌堆；不消耗苦痛施予。
- 【魔.弹.の.射.手】：如果仍有魔弹，回到抽牌堆；否则消耗，并将【终.末.の.爆.炸】加入抽牌堆。
- 魔弹相关加入抽牌堆都已经从 CardPilePosition.Top 改为 CardPilePosition.Random，不要回退成抽牌堆顶。
- 【终.末.の.爆.炸】：40/55 点 AOE 伤害。
- 【芬多精波动】：3/2 费；根据目标敌人减益层数获得等量再生，不移除减益。
- 【有罪宣言】：50/65 伤害；费用降低计数必须是“每张卡牌实例自己的 DebuffApplied 动态变量”，不是战斗全局计数。新生成的有罪宣言从 0 开始，打出后清零。
- 【急速切割】：移除目标敌人随机 5 层减益，对其造成 5/8 伤害并抽 1 张，最多重复 3/5 次。
- 【月之领域】：攻击具有减益效果的敌人时，伤害 + 当前月之领域层数；Power 描述使用 {Amount} 显示当前层数。

文本强调：
- 之前试过 [b] 加粗，效果不明显，已撤回。
- 目前部分数字试用原版标签：[green]、[red]、[aqua]、[gold]、[pink]、[purple]，并套 [sine] 动态效果。
- 如果游戏内显示标签原文或没有颜色，说明当前卡牌描述渲染路径未启用富文本解析；不要无限换标签，应该转向渲染/解析层调查。

五、项目固定范式 / 写法规范

1. 小步、精准、可回归
- 不要全局重写。
- 不要顺手格式化无关文件。
- 每轮只改用户要求相关文件。
- 改完运行 dotnet build。

2. 本地化
- 主要通过 CultLeaderModCode\Patches\LocInjectPatch.cs 注入。
- C# 字符串换行用 \n，不要用 {NL}，不要写真实换行。
- 动态数字常用：
  - {Damage:diff()}
  - {Block:diff()}
  - {DrawAmt:diff()}
  - {Energy:energyIcons()}
  - Power 层数通常用 {Amount}
- 如果数值升级前后不变，用户偏好不要假装动态变量；可直接写固定数字。
- 但是可变数值必须使用动态变量，不要硬编码。

3. 卡牌代码
- 卡牌继承 ModCardTemplate。
- 标签写法：
  protected override HashSet<CardTag> CanonicalTags =>
      [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure/Calm/Frenzy/Lively/Melancholy];
- 数值写在 CanonicalVars，用 DamageVar、BlockVar、DynamicVar。
- 升级用 DynamicVars["X"].UpgradeValueBy(...) 或 EnergyCost.UpgradeBy(...)。
- 读数值常用 BaseValue / IntValue，不要乱用不存在的 .Value 或 .UpgradeBy。
- 衍生牌设置 CanBeGeneratedInCombat => false，避免进入普通奖励/商店/随机招募；但保留注册，方便控制台测试。

4. Power / Buff 处理
- Power 应用：
  PowerCmd.Apply<T>(choiceContext, target, amount, applier, cardSource, silent: false)
- Power 修改：
  PowerCmd.ModifyAmount(...)、PowerCmd.Remove(power)。
- 玩家 Creature 是 base.Owner.Creature；不要把 Player 当 Creature 传给 PowerCmd.Apply。
- 不要破坏原生卡牌打出流程。

5. 抽牌堆/弃牌堆/消耗
- 普通加入抽牌堆用 CardPilePosition.Random。
- 只有明确要求“抽牌堆顶”时才用 CardPilePosition.Top。
- 本项目以前因为向错误牌堆/流程加牌导致 TEST/洗牌流程异常；新卡加入牌堆后必须尊重原生洗牌流程。

6. 伤害
- 多段攻击优先用 DamageCmd.Attack(...).WithHitCount(...)，不要手写重复攻击，除非需要每次之间插入特殊逻辑。
- AOE 可用项目已有 helper，例如 ApostleCardEffectHelpers.AttackAll。

7. 视觉/资源
- 只改 DLL 不会更新 PCK 里的图片/场景。
- 新增或修改图片、Godot 场景、资源路径时，需要 Godot 导入并导出 PCK：
  dotnet build -t:ExportPck
- 图片文件名尽量英文，避免中文/特殊字符导致 DeepSeek harness 或工具读图报错。
- 已导入 PCK 的图片通常可运行，但源码路径仍建议英文化。
- 卡图模板、使徒徽章、能量图标、卡框颜色系统不要随意重做。

8. 编码
- 不要用 PowerShell Set-Content 写中文源码。
- 小改动用 apply_patch。
- 如果必须脚本写中文，用明确 UTF-8 的方式。
- 避免读取 butter_melt.jpg 等已知可能触发 DeepSeek 新会话错误的图片；优先看路径和导入状态，不要把图片二进制塞进上下文。

六、构建、测试、发布

构建：
dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj"

如果游戏开着，DLL 会被 SlayTheSpire2.exe 锁定。可先临时构建：
dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj" -p:ModsPath="C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\tmp\buildmods\"

当前已知 build warning：
- TempMaxHpPower.cs：CS8604 oldOwner nullable
- TempMaxHpLossPower.cs：CS8604 oldOwner nullable
- LifeEssencePower.cs：CS8604 oldOwner nullable
- Apostle_Melancholy_19.cs：CS1998 async lacks await
这些目前不阻断运行；不要因为它们擅自大改，除非用户要求清 warning。

控制台测试卡牌常用：
card CULT_LEADER_MOD_CARD_APOSTLE_PURE_02
card CULT_LEADER_MOD_CARD_APOSTLE_PURE_15
card CULT_LEADER_MOD_CARD_APOSTLE_CALM_01
card CULT_LEADER_MOD_CARD_APOSTLE_CALM_11
card CULT_LEADER_MOD_CARD_APOSTLE_CALM_25
card CULT_LEADER_MOD_CARD_APOSTLE_FRENZY_01
card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_01
card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_08
card CULT_LEADER_MOD_CARD_APOSTLE_MELANCHOLY_02
card CULT_LEADER_MOD_CARD_APOSTLE_MELANCHOLY_02_1
card CULT_LEADER_MOD_CARD_APOSTLE_MELANCHOLY_02_2
card CULT_LEADER_MOD_CARD_APOSTLE_MELANCHOLY_07
card CULT_LEADER_MOD_CARD_APOSTLE_MELANCHOLY_13
card CULT_LEADER_MOD_CARD_APOSTLE_MELANCHOLY_17
card CULT_LEADER_MOD_CARD_APOSTLE_MELANCHOLY_22

发布：
- 不要 git commit / push / 工坊上传，除非用户明确要求。
- 若用户要求 GitHub：不要 git add .；先 git status、git diff，只 stage 明确相关文件。
- 若在 master/default 分支上，优先新建功能分支。
- 最近已推送分支 cultleader-card-tuning-20260820，提交 59e716a。
- 工坊 workspace：
  C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\release\workshop\CultLeaderModWorkspace
- 上传工具：
  C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\release\workshop\tools\ModUploader.exe

七、Codex × DeepSeek 联动机制

从现在开始，Codex 和 DeepSeek 都必须通过文件同步进度。DeepSeek harness 不能读取 Codex app 旧会话，因此不要依赖会话历史；只依赖项目内同步文件。

共享文件：

1. AI_SYNC_LOG.md
   - 每次开始任务先读。
   - 每次结束任务必须追加一段。
   - 只追加，不删除旧记录。
   - 格式：
     ## YYYY-MM-DD HH:mm - AgentName
     - 任务：
     - 读取依据：
     - 修改文件：
     - 构建结果：
     - 测试/待用户验证：
     - 未解决/下一步：
     - Git/发布状态：

2. PROJECT_KNOWLEDGE.md
   - 只记录长期稳定结论、已确认范式、重要坑点、已完成里程碑。
   - 不写流水账。
   - 遇到用户明确确认的设计规则，追加进去。

3. TEST_ISSUES.md
   - 记录用户测试发现的问题、复现方式、状态。
   - 修复后标注已修复日期和提交/文件。

4. 如果你要交给 Codex 接力，生成一个简短的 “NEXT_AGENT_HANDOFF.md” 或追加到 AI_SYNC_LOG.md，说明下一步从哪里开始。

联动协议：
- 接手前：读 PROJECT_KNOWLEDGE.md、PROJECT_RULES_FOR_AI.md、AI_SYNC_LOG.md、TEST_ISSUES.md、git status。
- 修改前：说明计划，尽量小步。
- 修改中：只碰任务相关文件。
- 修改后：dotnet build；如果游戏锁 DLL，用临时 ModsPath 构建。
- 结束前：追加 AI_SYNC_LOG.md；如产生长期规则，追加 PROJECT_KNOWLEDGE.md。
- 不要覆盖对方未提交工作；发现混合工作树时，先报告并只 stage/修改明确范围。

八、当前工作树/分支注意

当前 Codex 最近操作后：
- 分支：cultleader-card-tuning-20260820
- 已推送 origin/cultleader-card-tuning-20260820
- 未跟踪项可能包括 release/、DEEPSEEK_MEMORY_SAFE_PROMPT.md、mod-uploader.log。
- release/ 是工坊工作目录，通常不要纳入源码提交，除非用户明确要求。

如果你从 GitHub 拉取：
- 先确认是否要基于 master/main 还是 cultleader-card-tuning-20260820。
- 如果用户要继续“当前已发布工坊版本”，优先基于 cultleader-card-tuning-20260820 或包含 59e716a 的分支。
- 不要 reset --hard。

九、你接下来回答用户时的风格

- 中文沟通。
- 先说结论，再列关键文件/风险/下一步。
- 用户喜欢具体、直接、可测试的反馈。
- 不要把旧坑重新踩一遍：尤其是全局计数 vs 卡牌实例计数、抽牌堆顶 vs 抽牌堆、PowerCmd target 类型、LocInjectPatch 换行和中文编码。

十、本轮开始时请先做这些事

1. 读取上述固定文件。
2. 运行 git status --short --branch。
3. 如果用户要你改代码，先定位最小相关文件，不要全局搜索后乱改。
4. 如果任务涉及卡牌效果，优先对照：
   - 相关 Apostle_*.cs
   - LocInjectPatch.cs
   - ApostleCardEffectHelpers.cs
   - 相关 Power
   - PROJECT_KNOWLEDGE.md 最近追加段落
5. 给出短计划，等用户同意或在授权明确时直接小步执行。
```

## 给 Codex / DeepSeek 双方看的附加说明

- 本文件是交接 prompt，不等于最新源码事实本身；任何冲突以源码和最新 `AI_SYNC_LOG.md` / `PROJECT_KNOWLEDGE.md` 为准。
- 旧会话“教主mod DS”、“你现在接手 Slay the Spire 2 的 CultLeaderMod 项目。 重要：不要读取完整旧会话 sla…”、“slay the spire 2 mod （deepseek）”都在 Codex app 内，DeepSeek harness 不应尝试读取；其中重要信息已经大量被提炼到 `PROJECT_KNOWLEDGE.md` 和 `PROJECT_RULES_FOR_AI.md`。不要为了“完整性”把旧会话整段读进上下文。
- 如果 DeepSeek harness 能读图片，请也避免直接读取已知可能触发问题的图片二进制；先读路径、尺寸、导入文件和源码引用。
- 如果需要跨模型接力，最小充分交接是：
  1. 当前 git 分支与提交；
  2. 修改过的文件；
  3. build 结果；
  4. 用户尚未测试/已测试反馈；
  5. 下一步最小任务。
