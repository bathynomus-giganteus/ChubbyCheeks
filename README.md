# CultLeaderMod - 教主 Mod

杀戮尖塔2（Slay the Spire 2）的自定义角色 Mod。添加全新角色"教主"以及基于使徒性格的卡牌系统。

## 角色：教主

掌控使徒之力的神秘教主。通过"咏春的祝福"选择两种使徒性格，在游戏中收集对应性格的使徒卡牌。

### 初始遗物：咏春的祝福

- 开局选择两种使徒性格（5选2）
- 被选中的性格使徒牌出现概率大幅提升（约85%）
- 未被选中的性格使徒牌出现概率降低

## 性格系统

游戏包含五种使徒性格，每种对应不同的核心 Buff：

| 性格 | 颜色 | 核心 Buff | 升级 Buff | 说明 |
|------|------|-----------|-----------|------|
| 纯粹 | 绿 | 再生 (Regen) | 生命本源 (Life Essence) | 回合结束回复生命 |
| 冷静 | 蓝 | 覆甲 (Plating) | 固若坚冰 (Solid Ice) | 回合结束获得格挡 |
| 狂热 | 红 | 活力 (Vigor) | 狂热 (Fervor) | 攻击时附加伤害 |
| 活泼 | 黄 | 人工制品 (Artifact) | 幸福 (Happiness) | 抵挡负面效果 |
| 忧郁 | 紫 | 苦痛 (Bitter Pain) | 苦痛爆发 (Bitter Pain Burst) | 回合结束施加随机负面 |

每种性格 25 张使徒牌，共 125 张使徒牌（效果陆续实装中）。

## 特殊 Buff

| Buff | 说明 |
|------|------|
| 教主的权能 | 核心机制 Buff，叠加到 5 层后触发埃尔德形态 |
| 埃尔德形态 | 将基础 Buff 转化为升级版 Buff |

## 卡牌类型

- **使徒牌 (Apostle Cards)**：五种性格的战斗卡牌，带有性格标签
- **基础牌**：打击、防御、教主的权现、埃尔德形态、TEST
- **全属性牌**：同时属于五种性格的特殊使徒牌

## 项目结构

```
CultLeaderMod/
├── CultLeaderModCode/       # C# 源代码
│   ├── Cards/               # 卡牌定义
│   ├── CardTags/            # 自定义标签
│   ├── Character/           # 角色和卡池定义
│   ├── Compendium/          # 百科筛选
│   ├── Extensions/          # 扩展方法
│   ├── Patches/             # Harmony 补丁
│   ├── Potions/             # 药水
│   ├── Powers/              # Buff/状态定义
│   ├── Relics/              # 遗物定义
│   ├── Entry.cs             # Mod 入口
│   └── MainFile.cs          # Godot 主文件
├── CultLeaderMod/           # Godot 资源
│   ├── images/              # 图片素材
│   │   ├── badges/          # 使徒头像徽章
│   │   ├── card_portraits/  # 卡牌卡图
│   │   ├── characters/      # 角色素材
│   │   ├── charui/          # 角色 UI 素材
│   │   ├── powers/          # Buff 图标
│   │   └── relics/          # 遗物图标
│   ├── localization/        # 本地化文件
│   ├── materials/           # 卡框材质
│   └── scenes/              # Godot 场景
├── project.godot            # Godot 项目文件
├── CultLeaderMod.csproj     # C# 项目文件
└── CultLeaderMod.json       # Mod 元数据
```

## 构建

### 环境要求

- Windows 10/11
- .NET 9.0 SDK
- Godot 4.5.1 (Mono 版本)
- [STS2 RitsuLib](https://github.com/Alchyr/ModTemplate-StS2) (Steam 订阅)

### 构建 DLL

```powershell
dotnet build
```

### 导出 PCK

```powershell
& "E:\game\Godot\Godot_v4.5.1-stable_mono_win64.exe" --headless --export-pack "Windows Desktop" "E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\CultLeaderMod.pck"
```

### 安装

将以下文件放入 `Slay the Spire 2/mods/CultLeaderMod/`：
- `CultLeaderMod.dll`
- `CultLeaderMod.pck`
- `CultLeaderMod.json`

## 依赖

- **STS2 RitsuLib**：Mod 框架库，提供卡牌注册、Harmony 补丁、本地化等功能
- **Harmony**：运行时方法补丁

## 开发状态

- [x] 角色基础框架
- [x] 性格系统（5种性格标签）
- [x] 初始遗物"咏春的祝福"
- [x] 5选2 性格选择界面
- [x] 125 张使徒牌（五性格卡池已接入；纯粹/狂热部分具体效果已逐步实装，其余性格仍以测试效果为主）
- [x] 教主的权能 / 埃尔德形态 Buff
- [x] 再生 / 覆甲 / 活力 / 计划妥当 / 苦痛基础 Buff
- [x] 苦痛 / 苦痛爆发 Buff
- [x] 生命本源 Buff（临时最大生命值）
- [x] 卡框颜色区分
- [x] 使徒头像徽章
- [x] 卡池概率筛选
- [x] 卡牌升级描述系统（魔力乱打、阿卡那已实装）
- [ ] 使徒牌具体效果（按性格陆续实装中）
- [ ] 百科性格筛选
- [ ] 平衡性调整

## 致谢

- Trickcal Revive 角色设定灵感
- STS2 Modding 社区
- RitsuLib 框架
