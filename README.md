# ChubbyCheeks / 教主 Mod

为《Slay the Spire 2》制作的自定义角色 Mod。项目内部 ID 目前仍为 <code>CultLeaderMod</code>，基于 [ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2) 与 BaseLib 开发。

> 当前版本是功能原型与卡池概率测试版，不代表最终数值、美术或卡牌设计。

## 当前进展

- 已注册可选角色“教主”，基础生命值为 70，并拥有独立卡池、遗物池和药水池。
- 已接入角色选择背景、角色按钮、战斗模型、地图头像、商店模型和专属能量图标。
- 已建立“使徒牌”类型及纯粹、冷静、狂热、活泼、忧郁五种性格。
- 已实现动态初始遗物“咏春的祝福”：开局选择两种性格，并提升对应使徒牌在卡牌奖励和商店等来源中的出现权重。
- 已实现 150 张测试使徒牌，每种性格 30 张，用于验证性格卡池与抽取概率。
- 已实现简体中文与英文文本。

## 核心机制

### 性格与状态

| 性格 | 对应状态 | 卡牌边框 | 当前状态效果 |
| --- | --- | --- | --- |
| 纯粹 | 生命本源 | 绿色 | 暂无 |
| 冷静 | 固若坚冰 | 蓝色 | 暂无 |
| 狂热 | 狂热 | 红色 | 暂无 |
| 活泼 | 幸福 | 黄色 | 暂无 |
| 忧郁 | 痛苦 | 紫色 | 暂无 |

初始遗物会要求玩家选择其中两种性格。已选性格使徒牌与普通非使徒牌的抽取权重为 6，未选性格使徒牌的权重为 1。该权重目前接入卡牌奖励、商店以及调用原生奖励生成逻辑的来源。

### 测试使徒牌

每种性格目前有 30 张效果相同、名称不同的测试牌：

- 1–8 号：普通，共 40 张。
- 9–20 号：罕见，共 60 张。
- 21–30 号：稀有，共 50 张。
- 全部为 0 费技能牌，具有消耗，打出后获得 1 层对应性格状态。

这些牌是概率与框架测试用占位牌，之后会逐步替换为正式的《Trickcal ReVive》角色使徒牌。

## 初始牌组

教主的初始牌组共 10 张：

- 4 张打击：1 费，造成 6 点伤害。
- 4 张防御：1 费，获得 5 点格挡。
- 1 张随机招募：0 费，消耗；从 3 张随机使徒牌中选择 1 张加入手牌。
- 1 张教主权现：1 费，获得 1 层“教主的权能”。该状态目前无效果，层数范围为 0–5。

角色卡池还包含 5 张早期功能示例牌：教令、护教、不灭信仰、狂信猛攻和神启。

## 开发环境

- 《Slay the Spire 2》最低版本：<code>0.110.1</code>
- .NET SDK 9.0
- BaseLib <code>3.4.0</code> 或更高版本
- MegaDot / Godot <code>4.5.1-m.14</code>，仅打包 <code>.pck</code> 时需要

<code>Sts2PathDiscovery.props</code> 会尝试自动发现 Steam 默认安装目录。如果游戏位于其他 Steam 库，可在仓库根目录创建不提交到 Git 的 <code>Directory.Build.props</code>：

~~~xml
<Project>
  <PropertyGroup>
    <Sts2Path>E:/SteamLibrary/steamapps/common/Slay the Spire 2</Sts2Path>
    <GodotPath>C:/path/to/MegaDot.exe</GodotPath>
  </PropertyGroup>
</Project>
~~~

## 构建与安装

~~~powershell
git clone https://github.com/bathynomus-giganteus/ChubbyCheeks.git
cd ChubbyCheeks
dotnet build .\CultLeaderMod.sln --configuration Debug
~~~

构建会将 <code>CultLeaderMod.dll</code>、<code>CultLeaderMod.pdb</code> 和 <code>CultLeaderMod.json</code> 复制到游戏的 <code>mods/CultLeaderMod</code> 目录。

打包 Godot 资源：

~~~powershell
dotnet publish .\CultLeaderMod.csproj --configuration Debug
~~~

发布会额外生成并安装 <code>CultLeaderMod.pck</code>。启动游戏前还需要确保 BaseLib 已正确安装并启用。

## 已知限制

- 150 张使徒牌目前均为测试占位效果，尚未加入正式角色设计。
- 五种性格状态和“教主的权能”目前只记录层数，没有实际战斗效果。
- 美术资源、动画、文本和数值仍在迭代，暂未进行完整平衡测试。
- 游戏与 BaseLib API 仍可能变化，更新游戏后可能需要重新适配并构建。

## 致谢

- [Alchyr/ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2)
- BaseLib 与《Slay the Spire 2》Modding 社区

本项目为非官方同人 Mod，与 Mega Crit 或 EPIDGames 无关联。
