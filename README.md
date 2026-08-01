# CultLeaderMod（教主）

《杀戮尖塔 2》的示例角色 Mod，使用 `Alchyr.Sts2.Templates` 与 BaseLib 构建。

## 当前阶段

- 已注册最小角色模型、独立卡池、遗物池和药水池。
- 角色显示名为“教主”，基础生命为 70。
- 暂时使用模板占位资源、铁甲战士初始牌和燃烧之血。
- 已提供英文和简体中文角色基础文本。
- 已加入五张仅从战斗奖励等卡池来源获得的自定义卡牌；它们不在初始牌组中。

## 本机依赖

- .NET SDK 9.0
- MegaDot 4.5.1-m.14
- Slay the Spire 2
- BaseLib

## 构建

```powershell
dotnet build .\CultLeaderMod.sln --configuration Debug
```

构建会将 DLL、PDB 和 manifest 复制到游戏的 `mods/CultLeaderMod` 目录。

## 发布资源

```powershell
dotnet publish .\CultLeaderMod.csproj --configuration Debug
```

发布会额外使用 MegaDot 生成 `CultLeaderMod.pck`。
