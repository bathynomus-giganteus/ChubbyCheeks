# CultLeaderMod - 教主 Mod

《Slay the Spire 2》自定义角色 Mod，基于 RitsuLib 框架。添加全新角色“教主”，并围绕“使徒性格”构建卡牌与 Buff 体系。

## 版本

- 当前备份标签：`v0.1+基础架构完毕+纯粹使徒测试完毕+文本待修正+20260813-171713`
- 版本阶段：**v0.1**
  - 基础架构已完成
  - 纯粹使徒 25 张卡牌已通过当前效果测试
  - 中文本地化文本待统一修正

## 概述

开局时，初始遗物“咏春的祝福”会让玩家从五种使徒性格中选择两种。选中性格的使徒牌在整局游戏中的出现概率大幅提升，其余三种性格使徒牌出现概率降低。

## 核心系统

### 角色与初始遗物

- 角色：**教主**
- 初始遗物：**咏春的祝福**
  - 在涅奥遗物选择前弹出 5 选 2 界面
  - 选中的两种性格使徒牌出现概率提升（约 85%）
  - 未选中的三种性格使徒牌出现概率降低
  - 遗物描述会动态显示玩家选择的两种性格

### 五种性格与 Buff

每种性格对应一对“基础 Buff → 升级 Buff”：

| 性格 | 颜色 | 基础 Buff | 升级 Buff | 说明 |
|------|------|-----------|-----------|------|
| 纯粹 | 绿 | 再生（Regen） | 生命本源（Life Essence） | 回合结束回复生命；生命本源额外提供临时最大生命值 |
| 冷静 | 蓝 | 覆甲（Plating） | 固若坚冰（Solid Ice） | 固若坚冰为“敏捷 + 回合结束不减少的覆甲” |
| 狂热 | 红 | 活力（Vigor） | 狂热（Fervor） | 为攻击牌提供额外伤害 |
| 活泼 | 黄 | 保留（Retain） | 幸福（Happiness） | 保留手牌；幸福满层后消耗层数并回费抽牌 |
| 忧郁 | 紫 | 苦痛施予（Bitter Pain） | 苦痛爆发（Bitter Pain Burst） | 向敌人施加随机负面效果 |

### 教主的权能与埃尔德形态

- **教主的权能（CultLeaderAuthorityPower）**
  - 使徒牌获得对应性格 Buff 时，额外获得等同于权能层数的 Buff 层数
  - 叠加到 5 层时，自动消耗 5 层并进入埃尔德形态
- **埃尔德形态（ElderFormPower）**
  - 进入时，将当前已有的基础 Buff 转换为对应升级 Buff
  - 形态持续期间，使徒牌获得基础 Buff 时会直接获得升级 Buff

### 卡牌

- 使徒牌：5 种性格 × 25 张，共 125 张
  - 纯粹使徒 25 张效果已实装并测试
  - 其余性格使徒牌已接入卡池，部分仍使用测试效果，待逐步实装
- 基础牌：打击、防御、教主的权现、埃尔德形态、随机招募
- 全性格卡：独立卡池，不受初始 5 选 2 影响

## 测试与调试

### 控制台命令

游戏中按 `~` / `Shift+8` 等键打开控制台，常用命令：

```
card CULT_LEADER_MOD_CARD_APOSTLE_PURE_19
upgrade 0
```

- `card <卡牌ID>`：向手牌添加卡牌
- `upgrade <手牌索引>`：升级手牌，0 为最左侧
- 其他命令：`remove_card`、`draw`、`discard`、`energy` 等

### 日志调试

遇到难以复现的 Buff 触发问题时，可在相关 `Power` 钩子中临时加入日志，观察触发顺序与数值。调试思路记录在 `DEBUG_METHODOLOGY.md`。

## 项目结构

```
CultLeaderMod/
├── CultLeaderModCode/       # C# 源代码
│   ├── Cards/               # 卡牌定义与效果
│   ├── CardTags/            # 自定义标签（使徒、性格）
│   ├── Character/           # 角色与卡池定义
│   ├── Compendium/          # 百科筛选
│   ├── Patches/             # Harmony 补丁与本地化注入
│   ├── Powers/              # Buff / Debuff 状态定义
│   ├── Relics/              # 遗物定义
│   └── Entry.cs             # Mod 入口
├── CultLeaderMod/           # Godot 资源
│   ├── images/              # 卡图、Buff 图标、角色素材等
│   ├── localization/        # 本地化文件
│   ├── materials/           # 卡框材质
│   └── scenes/              # Godot 场景
├── Directory.Build.props    # 构建路径配置
├── Sts2PathDiscovery.props  # 游戏目录发现
├── project.godot
├── CultLeaderMod.csproj
├── CultLeaderMod.json
└── README.md
```

## 构建

### 环境要求

- Windows 10/11
- .NET 9.0 SDK
- Godot 4.5.1（Mono 版）
- Steam 版《Slay the Spire 2》
- STS2 RitsuLib（Steam 创意工坊订阅）

### 路径配置

构建路径在 `Directory.Build.props` 中配置：

```xml
<GodotPath>E:/game/Godot/Godot_v4.5.1-stable_mono_win64.exe</GodotPath>
<Sts2Path>E:/SteamLibrary/steamapps/common/Slay the Spire 2</Sts2Path>
```

若你的安装路径不同，请修改这两个值。

### 构建 DLL

```powershell
dotnet build
```

构建成功后会自动将 `CultLeaderMod.dll` 复制到 `<Sts2Path>/mods/CultLeaderMod/`。

### 导出 PCK

新增或修改图片、场景等资源后需要重新导出 PCK：

```powershell
dotnet build -t:ExportPck
```

也可以直接在 Godot 编辑器中选择对应导出预设导出 PCK。

### 安装

确保以下文件位于 `Slay the Spire 2/mods/CultLeaderMod/`：

- `CultLeaderMod.dll`
- `CultLeaderMod.pck`
- `CultLeaderMod.json`

## 备份与回退

当前稳定备份已推送至 GitHub，标签为：

```
v0.1+基础架构完毕+纯粹使徒测试完毕+文本待修正+20260813-171713
```

回退到该版本：

```powershell
git fetch --tags
git checkout tags/v0.1+基础架构完毕+纯粹使徒测试完毕+文本待修正+20260813-171713
```

## 依赖与致谢

- STS2 RitsuLib：Mod 框架，提供卡牌注册、Harmony 补丁、本地化等能力
- Harmony：运行时方法补丁
- Trickcal Revive 角色设定灵感
- STS2 Modding 社区教程与工具