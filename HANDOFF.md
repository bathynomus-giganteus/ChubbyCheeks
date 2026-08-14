# CultLeaderMod 交接文档

> 长期设计范式、技术规则和完整进度见 PROJECT_KNOWLEDGE.md。

> 由旧会话 `slay the spire 2 mod （deepseek）` 的本地记录恢复生成。
> 新会话只读本文件，不要读取旧会话完整 transcript。

## 项目
- 项目路径：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod`
- 角色：教主（Cult Leader）
- 构建命令：
  `dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj"`
- 构建后 DLL/PDB 复制到：
  `E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\`
- 部署前必须关闭 `SlayTheSpire2.exe`，否则 DLL 被占用无法覆盖。

## 当前进展
- 已实现 150 张使徒测试卡、性格 Power 类、中英文/中文本地化。
- 已完成冷静系 `雪花蝶舞`、`百帕斯卡 挥棒!` 的部分修复。
- 已开始实装活泼系卡牌，并处理 `CanBeGeneratedInCombat`、衍生牌、奖励池过滤。
- 最新一次改动已经“临时构建通过”，但最后部署被游戏占用 DLL 卡住。

## 已完成的关键改动
- `CultLeaderModCode\Cards\Apostle_Calm_01.cs`：雪花蝶舞动态伤害预览已修复；升级值改为 `+5/+8`。
- `CultLeaderModCode\Cards\Apostle_Calm_02.cs`：百帕斯卡 挥棒! 隐藏计数改为只统计可见 Buff：
  `.Where(p => p.Type == PowerType.Buff && p.IsVisible)`。
- `CultLeaderModCode\Cards\Apostle_Lively_06.cs`：`黄油融化` 增加 `CanBeGeneratedInCombat => false`。
- `CultLeaderModCode\Character\CultLeaderModCardPool.cs`：奖励/变换/随机招募路径过滤 `CanBeGeneratedInCombat=false` 的牌，但保留在 `ModelDb.AllCards` 以便控制台测试。
- `CultLeaderModCode\Cards\Apostle_Lively_08.cs`：`戏剧性演出` 选择逻辑已实装，新增三张衍生牌：
  - `Apostle_Lively_08_1.cs`：助手埃皮康
  - `Apostle_Lively_08_2.cs`：埃皮康分身术
  - `Apostle_Lively_08_3.cs`：献给友军
- 已补三张衍生卡和 `戏剧性演出` 的中文本地化，头像映射暂沿用 `活泼_08` / 戏剧性演出卡图占位。

## 上次用户最后要求
1. `循环` 卡没有按 Excel 表格逻辑写，需要重新对照表格检查实现。
2. 从素材文件夹按文件名找到这些卡图：
   - 摸摸头
   - 捏捏脸
   - 爆栗子
   - 黄油融化
3. 尝试在查看 `黄油飞射` 时，鼠标悬浮展示 `黄油融化` 衍生卡预览；如果不好实现就跳过，仅保留在百科。
4. `黄油融化` 当前卡面错误地用了“提格”头像，需要修正。

## 测试指令
控制台逐条输入：
- `card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_05`
- `card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_06`
- `card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_08`
- `card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_08_1`
- `card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_08_2`
- `card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_08_3`

## 关键参考文件
- 卡片来源表：`tmp\all_cards.json`、`tmp\card_mapping.json`
- 本地化注入：`CultLeaderModCode\LocInjectPatch.cs`
- 性格枚举与映射：`CultLeaderModCode\Cards\ApostlePersonality.cs`
- Power 定义：`CultLeaderModCode\Powers\`
- 调试工具：`C:\Users\888\.dotnet\tools\ilspycmd.exe`
- 游戏日志：`C:\Users\888\AppData\Roaming\SlayTheSpire2\logs\godot.log`

## 下一步建议
- 先确认 `SlayTheSpire2.exe` 已关闭。
- 运行构建命令，部署 DLL。
- 再按“上次用户最后要求”逐项处理 `循环`、卡图路径和 `黄油融化` 卡面修正。
- 每完成一项，把结果写回本文件或单独的 `PROGRESS.md`，避免再次把会话上下文撑爆。