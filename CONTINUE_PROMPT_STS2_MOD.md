# CultLeaderMod 新会话接力包

> 用途：在新会话中继续开发 `Slay the Spire 2` 的“教主 / CultLeaderMod”mod。  
> 重要：不要让新会话完整读取旧会话 `slay the spire 2 mod（deepseek）`，那个会话已因上下文过长报错。新会话优先读取本文档和当前工程文件。

## 1. 为什么要开新会话

旧会话最后的报错不是游戏或代码错误，而是模型上下文超限：

```json
{"error":{"message":"This model's maximum context length is 1048576 tokens. However, you requested 1730857 tokens ...","type":"invalid_request_error","code":"invalid_request_error"}}
```

处理原则：

- 不要继续把完整旧聊天历史塞给模型；
- 用本文档作为压缩后的交接材料；
- 让模型直接检查当前工程源码和必要的表格/素材；
- 每完成一小步就构建验证，避免再堆超长上下文。

## 2. 项目路径和构建

- 项目目录：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod`
- Steam 游戏目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2`
- Mod 部署目录：`E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod`
- 设计表格：`C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\tmp\card_info.xlsx`
- 旧会话抽取文件：`C:\Users\888\OneDrive\codex\sts2_old_session_extract.txt`
- 游戏日志：`C:\Users\888\AppData\Roaming\SlayTheSpire2\logs\godot.log`
- ILSpy：`C:\Users\888\.dotnet\tools\ilspycmd.exe`

临时构建，不覆盖 Steam mod：

```powershell
dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj" -p:ModsPath="C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\tmp\buildmods\"
```

正式构建/部署，需要先关闭游戏：

```powershell
dotnet build "C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod.csproj"
```

当前状态：最近一次临时构建通过，`0 错误 / 0 警告`。

## 3. 当前最重要的上下文

用户正在逐个性格实装使徒牌。纯粹、狂热已有较多实装；冷静部分正在测试；活泼部分最近改动较多，尤其是衍生牌机制。

最近要继续处理的重点来自两个会话：

1. `slay the spire 2 mod（deepseek）`
2. 关于上下文超限报错的会话，用户最后补充了几个关键要求

## 4. 用户最后明确提到的待办

### 4.1 重新检查【循环】

用户说：【循环】没有按照 Excel 表格里的逻辑写，可能是因为之前表格没保存。  
下一步应重新读取 `tmp\card_info.xlsx`，对照“活泼”sheet 中【循环】的设计，检查当前实现是否一致。

可能相关文件：

- `CultLeaderModCode\Cards\TestRainbowCard.cs`
- `CultLeaderModCode\Patches\LocInjectPatch.cs`
- 如果【循环】不是 TestRainbowCard，请先用 `rg "循环|TEST_RAINBOW|Rainbow"` 确认真实文件。

### 4.2 找并使用这些卡图素材

用户说素材在素材文件夹里，可以根据文件名找到：

- 摸摸头
- 捏捏脸
- 敲爆栗 / 爆栗子
- 黄油融化

目前已在本机找到：

- `C:\Users\888\Desktop\新建文件夹\摸摸头.jpg`
- `C:\Users\888\Desktop\新建文件夹\捏捏脸.jpg`
- `C:\Users\888\Desktop\新建文件夹\敲爆栗.jpg`
- `C:\Users\888\Desktop\新建文件夹\butter_melt.jpg`
- `C:\Users\888\Desktop\新建文件夹\坨坨\活泼\埃皮卡\戏剧性演出.png`

关于 `butter_melt.jpg` 的特别注意：

- 源文件本身有效：JPEG / RGB / 1920×1080，Pillow 校验通过。
- 当前项目内也有 `CultLeaderMod\images\card_portraits\lively\黄油融化.png`，该 PNG 有效：RGBA / 1920×1080。
- 但 `黄油融化.png` 当前没有对应 `.import` 文件，说明可能还没有被 Godot 正式导入到 PCK。
- 不建议在新会话里让模型直接“读取/看图” `C:\Users\888\Desktop\新建文件夹\butter_melt.jpg`，因为中文目录 `新建文件夹` 在部分工具/模型环境里可能被错误编码成 `?????`，导致路径报错。
- 如果只是改卡图路径，优先使用项目内已存在资源路径，或先复制/转换成英文文件名如 `butter_melt.png` 后再让 Godot 导入。

注意：新增或替换图片后，仅 `dotnet build` 不一定足够；如果是 Godot/PCK 资源，需要用户在 Godot 中导入并导出 PCK，或者明确让助手处理资源流程。

### 4.3 黄油融化与黄油飞射

用户确认：

- 【黄油融化】是 `card CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_06`
- 【黄油飞射】应是正常流程中获得【黄油融化】的唯一渠道
- 【黄油融化】不要正常出现在奖励、商店、随机招募等卡池
- 如果可行，希望鼠标悬浮查看【黄油飞射】时，旁边能预览衍生牌【黄油融化】；如果不好做，可以跳过，只保留百科可见
- 【黄油融化】之前卡面错误用了“提格”的头像，需要改成 `butter_melt.jpg`

当前源码状态：

- `CultLeaderModCode\Cards\Apostle_Lively_05.cs`：黄油飞射，累计受伤次数，达到 100 后 Transform 成 `Apostle_Lively_06`
- `CultLeaderModCode\Cards\Apostle_Lively_06.cs`：黄油融化，已有 `public override bool CanBeGeneratedInCombat => false;`
- `CultLeaderModCode\Character\CultLeaderModCardPool.cs`：`FilterThroughEpochs` 中过滤 `CanBeGeneratedInCombat == false`，避免衍生牌进奖励/随机池
- `CultLeaderModCode\Cards\TestAddApostleCards.cs`：也按 `CanBeGeneratedInCombat` 过滤

需要重点检查：

- `Apostle_Lively_05.cs` 和 `Apostle_Lively_06.cs` 当前 `PortraitPath` 里可能有乱码路径。编译能过，但进游戏可能丢图。
- 【黄油融化】应使用 `butter_melt.jpg` 转入项目资源后的正确路径。

### 4.4 音速斩击

旧会话里确认：【音速斩击】不是 `Apostle_Lively_06`，因为 `Lively_06` 已被用于【黄油融化】。  
当前项目新增了：

- `CultLeaderModCode\Cards\Apostle_Lively_Sonic.cs`

预期设计来自活泼表格：

- 名称：音速斩击
- 类型：攻击
- 费用：0
- 稀有度：罕见
- 效果：丢弃所有手牌。每丢弃一张，获得 1 层保留，并对随机敌人造成 6 点伤害。
- 升级：伤害变为 8。

需要检查：

- 当前 `Apostle_Lively_Sonic.cs` 的逻辑是否真的按表格实现；
- 当前 `PortraitPath` 可能是乱码路径，需要修正；
- 本地化 key 应为 `CULT_LEADER_MOD_CARD_APOSTLE_LIVELY_SONIC.title/description`。

### 4.5 戏剧性演出与三张衍生牌

当前相关文件：

- `CultLeaderModCode\Cards\Apostle_Lively_08.cs`：戏剧性演出
- `CultLeaderModCode\Cards\Apostle_Lively_08_1.cs`：助手埃皮康
- `CultLeaderModCode\Cards\Apostle_Lively_08_2.cs`：埃皮康分身术
- `CultLeaderModCode\Cards\Apostle_Lively_08_3.cs`：献给友军
- `CultLeaderModCode\Powers\EpiconAssistantPower.cs`

旧会话设计方向：

- 【戏剧性演出】：2费技能，消耗；从三张埃皮康衍生牌中选择一张加入手牌；如果当前“保留/幸福”合计不少于 10 层，则三张全部加入手牌；升级后 0 费。
- 三张衍生牌都应 `CanBeGeneratedInCombat => false`，避免正常卡池出现，但保留控制台可测试。
- 三张衍生牌当前可能共用【戏剧性演出】卡图，除非找到对应素材。

需要检查：

- `Apostle_Lively_08.cs` 的选择流程是否符合表格；
- 三张衍生牌是否都被 `ApostleBadge.cs` 正确映射头像；
- 本地化是否完整；
- `EpiconAssistantPower` 是否和表格一致。旧摘要说它每回合开始给再生、苦痛施予、活力、覆甲；不要擅自加幸福/保留，除非用户要求。

## 5. 当前关键源码文件

优先让新会话检查这些：

- `CultLeaderModCode\Cards\Apostle_Lively_05.cs`
- `CultLeaderModCode\Cards\Apostle_Lively_06.cs`
- `CultLeaderModCode\Cards\Apostle_Lively_Sonic.cs`
- `CultLeaderModCode\Cards\Apostle_Lively_08.cs`
- `CultLeaderModCode\Cards\Apostle_Lively_08_1.cs`
- `CultLeaderModCode\Cards\Apostle_Lively_08_2.cs`
- `CultLeaderModCode\Cards\Apostle_Lively_08_3.cs`
- `CultLeaderModCode\Powers\EpiconAssistantPower.cs`
- `CultLeaderModCode\Character\CultLeaderModCardPool.cs`
- `CultLeaderModCode\Cards\TestAddApostleCards.cs`
- `CultLeaderModCode\Cards\ApostleBadge.cs`
- `CultLeaderModCode\Patches\LocInjectPatch.cs`

## 6. 编码和本地化注意事项

这是这个项目里最容易炸的地方：

- PowerShell 直接写中文脚本可能把中文写成乱码。
- 如果用脚本批量改中文，优先用 Python/Node 并确保 UTF-8；必要时用 Unicode escape。
- 不要用会导致编码不确定的 `Set-Content` 直接写中文源码。
- 小改动优先用 `apply_patch`。
- `LocInjectPatch.cs` 中换行用 `\n`，不要用 `{NL}`，也不要在 C# 字符串中写真实换行。
- 能量图标写 `{Energy:energyIcons()}`。
- 如果 `CanonicalKeywords` 已有 `CardKeyword.Exhaust`，描述里通常不要重复写“消耗”。
- 不要全局重写本地化，只改当前任务相关 key。

## 7. 用户偏好

- 中文沟通。
- 一次改一个小点，构建验证后交给用户测试。
- 尽量精准，不要顺手重构无关文件。
- 用户经常在游戏里实测，需要给控制台测试指令。
- 视觉素材用户通常会自己导入 Godot/PCK，但如果修改了资源路径，要明确提醒。
- 不要提交 git，除非用户明确要求。

## 8. 建议的新会话 Prompt

把下面整段复制到新会话即可：

```text
你现在接手 Slay the Spire 2 的 CultLeaderMod 项目。请不要读取完整旧会话，旧会话已经因为上下文过长报错。请按顺序优先读取并遵守：

1. C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\PROJECT_RULES_FOR_AI.md
2. C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CONTINUE_PROMPT_STS2_MOD.md

项目目录：
C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod

当前我最想继续处理的是活泼性格卡牌最近留下的问题：

1. 重新对照 tmp\card_info.xlsx 检查【循环】是否按表格逻辑实现。
2. 检查【黄油飞射】Apostle_Lively_05 与【黄油融化】Apostle_Lively_06：
   - 黄油飞射应是正常流程中获得黄油融化的唯一渠道；
   - 黄油融化不要进奖励、商店、随机招募等正常卡池；
   - 黄油融化卡图应改为 butter_melt.jpg，不要再用提格头像；
   - 如果好做，尝试让查看黄油飞射时能预览黄油融化衍生牌；不好做就先跳过。
3. 检查【音速斩击】Apostle_Lively_Sonic 是否按表格实现：
   - 0费攻击，罕见；
   - 丢弃所有手牌，每丢弃一张获得1层保留，并对随机敌人造成6点伤害；
   - 升级后伤害8。
4. 检查【戏剧性演出】Apostle_Lively_08 和三张衍生牌 Apostle_Lively_08_1/08_2/08_3：
   - 衍生牌不能进入正常奖励/商店/随机招募；
   - 但控制台应能生成测试；
   - 本地化和头像徽章要完整；
   - 先不要大范围重构。
5. 当前源码里部分活泼卡的 PortraitPath 可能是乱码路径，编译会过但游戏内可能丢图。请优先检查并修正相关卡图路径。

请先只做检查和计划，必要时构建验证。修改时要小步、精准、保留现有视觉系统和卡框/能量图标/使徒徽章机制。不要 git commit。
```
