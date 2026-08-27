# DeepSeek memory-safe handoff prompt

你现在接手 `C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod`，这是一个 Slay the Spire 2 的 Godot/C# mod。

重要背景：DeepSeek 的 memory/上下文已经多次被旧会话、图片 base64、PCK 二进制输出撑爆。所以本轮请只做当前任务，不要重建全部历史。

## 绝对不要做

- 不要读取完整旧会话、rollout JSONL、聊天记录或任何 10MB+ 文件。
- 不要完整读取 `PROJECT_RULES_FOR_AI.md`、`CONTINUE_PROMPT_STS2_MOD.md`。如果必须查规则，只用 `rg` 搜关键词并读取附近几十行。
- 不要使用 `view_image`，不要把图片转 base64 输出；查图片只用文件名、尺寸、PIL metadata。
- 不要 dump `SlayTheSpire2.pck`、二进制、`.import` 大文件、完整 Godot 资源内容。
- 不要 `Get-Content -Raw` 读取大文件；不要输出完整 `git diff`。

## 安全工作方式

- 先进入项目目录：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod`。
- 用 `rg` 搜索目标代码；每次只读相关文件片段。
- PowerShell/Python 输出中文前设置 UTF-8：
  - `$env:PYTHONIOENCODING='utf-8:replace'`
  - 如需要：`[Console]::OutputEncoding=[Text.UTF8Encoding]::new($false)`
- 命令输出必须短；需要统计时只输出摘要。
- 代码修改小步进行，改完运行 `dotnet build`。如果涉及 `.tscn`、图片、Godot 资源或 PCK，需要说明必须重新用 Godot 导入/导出或重新打包，`dotnet build` 只更新 DLL。
- 不要 reset/清空工作区，不要删除用户资产。

## 当前项目状态摘要

- 项目：`CultLeaderMod`
- 角色：教主 / CultLeader。
- 已有大量角色卡、使徒牌、五性格系统、权能、埃尔德形态、事件/遗物/图片资源。
- 使徒牌五性格：
  - 纯粹：绿色，基础 buff 对应生命本源。
  - 冷静：蓝色，基础 buff 对应固若坚冰。
  - 狂热：红色，基础 buff 对应狂热。
  - 活泼：黄色，基础 buff 对应计划妥当。
  - 忧郁：紫色，基础 buff 对应苦痛。
- 埃尔德形态：进入时应把当前五种基础 buff 等量转成对应升级 buff；进入后拦截后续基础 buff 获得，转成升级 buff。实现时不要破坏卡牌打出流程，避免让卡牌卡在屏幕中央。
- TEST/随机加入卡牌类效果：之前稳定方案是加入弃牌堆，让原生洗牌流程处理；不要手写强制洗牌/抽牌流程。

## 另一个新会话的最后进度

另一个会话标题开头是：“你现在接手 Slay the Spire 2 的 CultLeaderMod 项目。重要：不要读取完整旧会话 sla…”

该会话不是代码本身坏掉，而是因为读取了太多大内容后炸了：

- 读取了长交接文档；
- 检查 STS2 PCK 时输出过大；
- 对事件图使用了 `view_image`，导致图片 base64 进入上下文；
- 最后出现类似 `maximum context length is 1048576 tokens, requested 1318698 tokens`。

它做到的关键结论：

- 当时在处理用户反馈项，前 9 项代码/文本修复大体已经完成，并且临时 build 通过：`0 errors / 4 warnings`。
- warnings 大致涉及：`TempMaxHpPower`、`TempMaxHpLossPower`、`LifeEssencePower`、`Apostle_Melancholy_19`，不一定是当前阻断项。
- 用户最后关心的是：“事件图片的显示怎么办？”
- 发现自定义事件似乎只设置了 `InitialPortraitPath`，没有设置 `LayoutScenePath`。
- 原版 `default_event_layout.tscn` 的 `Portrait` 区域很大，2400×1080 图片会被放得像全屏背景。
- 建议方案：新建一个共享的自定义事件 layout scene，例如：
  - `CultLeaderMod/scenes/events/cult_leader_event_layout.tscn`
  - 基于默认事件 layout，但把 `Portrait` 的 TextureRect 改成更小/偏左或合适的位置。
  - 然后给所有自定义事件的 `AssetProfile.LayoutScenePath` 指向这个 layout。
- 可能受影响的事件图片尺寸：
  - `dragon_spartan_training.png` 2400x1080
  - `fairy_surveillance.png` 2400x1080
  - `fortune_teller_cat_cushion.png` 2400x1080
  - `fox_weapon_test.png` 1920x1080
  - `mysterious_convenience_clerk.png` 2400x1080
  - `rolling_floor_elemental.png` 2400x1080
  - `team_level_skip.png` 254x254，可能不使用同一事件大图布局。

## 当前建议任务

请先只处理“事件图片显示过大/位置不合适”的问题：

1. 用 `rg "InitialPortraitPath|LayoutScenePath|AssetProfile|Event" CultLeaderModCode CultLeaderMod -n` 找到自定义事件注册/定义位置。
2. 不要读图片内容，只确认图片路径和尺寸。
3. 检查现有是否已有自定义 `.tscn` 事件 layout；如果没有，创建一个共享 layout。
4. 给需要大图显示的自定义事件设置 `LayoutScenePath`。
5. 运行 `dotnet build` 验证 C#，并说明如果新增/修改 `.tscn` 或资源，需要 Godot 重新导入/导出 PCK 后游戏内才会生效。

请用简短中文回复进展，不要把大段文件内容贴出来。
