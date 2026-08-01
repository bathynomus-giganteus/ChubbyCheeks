# ChubbyCheeks / 教主 Mod

为《Slay the Spire 2》制作的自定义角色 Mod。项目内部 ID 目前仍为 `CultLeaderMod`，基于 [ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2) 与 BaseLib 开发。

> 当前版本为功能原型与卡池概率测试版，不代表最终数值、美术或卡牌设计。

## 当前进展

- 已注册可选角色"教主"，基础生命值为 70，拥有独立卡池、遗物池和药水池。
- 已接入角色选择背景、角色按钮、战斗模型、地图头像、商店模型和专属能量图标。
- 实现动态初始遗物"咏春的祝福"：开局选择两种性格，提升对应使徒牌在卡牌奖励和商店等来源中的出现权重。
- 实现 150 张测试使徒牌，每种性格 30 张，用于验证性格卡池与抽取概率。
- 实现简体中文与英文文本。
- **全部状态效果已实装**，详见下方核心机制。

## 核心机制

### 性格与状态

| 性格 | 基础祝福 | 埃尔德祝福 | 卡牌边框 |
| --- | --- | --- | --- |
| 纯粹 | 再生 | 生命本源 | 绿色 |
| 冷静 | 覆甲 | 固若坚冰 | 蓝色 |
| 狂热 | 活力 | 狂热 | 红色 |
| 活泼 | 人工制品 | 幸福 | 黄色 |
| 忧郁 | 苦痛 | 苦痛爆发 | 紫色 |

### 状态效果

**教主的权能**：使徒牌获得祝福的层数+1（每层权能额外+1层）。权能达到 5 层时，消耗 5 层进入埃尔德形态。

**埃尔德形态**：升级所有使徒牌祝福——再生→生命本源、覆甲→固若坚冰、活力→狂热、苦痛→苦痛爆发、人工制品→幸福。

**生命本源**：每获得一层时 +5 最大生命值，每失去一层时 -5 最大生命值。

**固若坚冰**：每层使格挡获得量 +1，回合结束时每层获得 1 点格挡。

**狂热**：每层使下一张攻击牌伤害 +3，攻击后消耗 1 层并失去 3 点生命。

**苦痛**：不会随回合减少。回合结束时每层对所有敌人和自身随机施加一种负面效果（1 易伤/1 虚弱/1 脆弱/3 中毒/6 厄运）。

**苦痛爆发**：回合结束时每层对所有敌人施加 1 易伤、1 虚弱、1 脆弱、3 中毒和 6 厄运。

**幸福**：层数 ≥3 时，消耗 3 层，获得 1 点能量并抽 2 张牌。

### 测试使徒牌

每种性格目前有 30 张测试牌（共 150 张），效果为：消耗，抽 1 张牌，获得 1 层对应性格的基础祝福（埃尔德形态下自动替换为埃尔德祝福）。

## 初始牌组

教主的初始牌组共 10 张：

- 4 张打击：1 费，造成 6 点伤害。
- 4 张防御：1 费，获得 5 点格挡。
- 1 张随机招募：0 费，消耗；从 3 张随机使徒牌中选择 1 张加入手牌。
- 1 张教主的显现：1 费，获得 1 层"教主的权能"。

## 开发环境

- 《Slay the Spire 2》最低版本：`0.110.1`
- .NET SDK 9.0
- BaseLib `3.4.0` 或更高版本
- MegaDot / Godot `4.5.1-m.14`，仅打包 `.pck` 时需要

`Sts2PathDiscovery.props` 会尝试自动发现 Steam 默认安装目录。如果游戏位于其他 Steam 库，可在仓库根目录创建不提交到 Git 的 `Directory.Build.props`：

```xml
<Project>
  <PropertyGroup>
    <Sts2Path>E:/SteamLibrary/steamapps/common/Slay the Spire 2</Sts2Path>
    <GodotPath>C:/path/to/MegaDot.exe</GodotPath>
  </PropertyGroup>
</Project>
```

## 构建与安装

```powershell
git clone https://github.com/bathynomus-giganteus/ChubbyCheeks.git
cd ChubbyCheeks
dotnet build .\CultLeaderMod.sln --configuration Debug
```

构建会将 `CultLeaderMod.dll`、`CultLeaderMod.pdb` 和 `CultLeaderMod.json` 复制到游戏的 `mods/CultLeaderMod` 目录。

打包 Godot 资源：

```powershell
dotnet publish .\CultLeaderMod.csproj --configuration Debug
```

发布会额外生成并安装 `CultLeaderMod.pck`。启动游戏前还需要确保 BaseLib 已正确安装并启用。

## 待办

- 150 张使徒牌目前均为测试占位效果，尚未加入正式角色设计。
- 美术资源、动画、文本和数值仍在迭代，尚未进行完整平衡测试。
- 游戏中 BaseLib API 仍可能变化，更新游戏后可能需要重新适配并构建。

## 致谢

- [Alchyr/ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2)
- BaseLib 与《Slay the Spire 2》Modding 社区

本项目为非官方同人 Mod，与 Mega Crit 或 EPIDGames 无关联。
