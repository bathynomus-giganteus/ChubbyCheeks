# CultLeaderMod — 教主 Mod

《Slay the Spire 2》自定义角色 Mod，基于 RitsuLib 框架。添加全新可玩角色 **教主**，并围绕“使徒性格”构建卡牌、Buff、遗物与事件体系。

## 当前版本

- Mod 版本：**v0.2.01**
- 最低游戏版本：**0.110.1**
- 框架依赖：STS2 RitsuLib **>= 0.5.10**
- 当前阶段：核心系统已实装，适合开发测试与功能验收；平衡性、文本和部分事件布局仍会继续调整。

## 已实现内容

### 角色

- **教主**：拥有独立卡池、初始卡组、遗物池和事件池。
- 角色专属内容默认只对教主生效，遗物和事件也带有“仅教主”的获取限制。

### 初始遗物与性格选择

开局由初始遗物触发 **5 选 2** 性格选择：

- 纯粹：绿色
- 冷静：蓝色
- 狂热：红色
- 活泼：黄色
- 忧郁：紫色

选中的两种性格，其对应使徒牌在后续卡牌奖励中出现概率大幅提升；未选中的三种性格出现概率降低。

初始 5 选 2 卡图使用独立素材 a.png 至 e.png。

### 使徒牌与五性格体系

| 性格 | 颜色 | 基础 Buff | 升级 Buff |
|------|------|-----------|-----------|
| 纯粹 | 绿色 | 再生体系 | 生命本源 / 临时最大生命 |
| 冷静 | 蓝色 | 覆甲体系 | 固若坚冰 |
| 狂热 | 红色 | 活力体系 | 狂热 |
| 活泼 | 黄色 | 保留体系 | 计划妥当 / 幸福 |
| 忧郁 | 紫色 | 苦痛施予体系 | 苦痛爆发 |

- 计划包含 5 种性格 × 25 张使徒牌，代码层已基本铺开。
- 除使徒牌外，还包含基础牌、特殊牌和埃尔德形态相关卡牌。

### 权能与埃尔德形态

- **教主的权能**：使徒牌获得对应性格 Buff 时，会按权能层数额外叠加。
- 权能达到阈值后进入 **埃尔德形态**。
- **埃尔德形态**：
    - 进入时，把当前基础 Buff 等量转换为对应升级 Buff。
    - 持续期间，使徒牌获得基础 Buff 时直接转换为升级 Buff。
    - 实现上保留原生卡牌打出流程，避免卡牌卡在屏幕中央。

### 遗物

当前包含 20+ 教主专属遗物，其中近期新增三个涅奥遗物：

- **使徒单抽券**：拾起时随机获得一张使徒牌，受初始遗物性格加成影响。
- **卡牌单抽券**：拾起时随机获得一个遗物，并向卡组加入一张“债务”。
- **金蜡笔**：每 5 场战斗后，以战斗奖励形式选择卡组中的 1 张使徒牌强化。

### 事件

已接入多个教主专属事件，包括龙族斯巴达训练、妖精监视请求、狐狸武器测试、神秘便利店店员、地板元素滚动、猫墩占卜师等。

### 失败与主动放弃表现

- 教主战斗失败文案：**“艾里亚斯被无尽的冰雪覆盖”**。
- 战斗失败、事件或火堆主动放弃时，失败界面角色立绘逆时针旋转 90°。

## 项目结构

    CultLeaderMod/
    ├── CultLeaderModCode/
    │   ├── Cards/          # 卡牌定义与效果
    │   ├── CardTags/       # 使徒、性格等自定义标签
    │   ├── Character/      # 教主角色与卡池
    │   ├── Events/         # 自定义事件
    │   ├── Patches/        # Harmony 补丁、本地化注入
    │   ├── Powers/         # Buff / Debuff 状态
    │   └── Relics/         # 遗物定义
    ├── CultLeaderMod/
    │   ├── images/         # 卡图、遗物图、事件图、Buff 图标
    │   ├── localization/   # 本地化资源
    │   ├── materials/      # 卡框材质
    │   └── scenes/         # Godot 场景
    ├── CultLeaderMod.csproj
    ├── CultLeaderMod.json
    ├── project.godot
    └── README.md

## 构建与安装

### 环境要求

- Windows 10/11
- .NET 9.0 SDK
- Godot 4.5.1 Mono 版
- Steam 版《Slay the Spire 2》
- STS2 RitsuLib

### 路径配置

先修改 Directory.Build.props 中的 Godot 与游戏路径：

    <GodotPath>E:/game/Godot/Godot_v4.5.1-stable_mono_win64.exe</GodotPath>
    <Sts2Path>E:/SteamLibrary/steamapps/common/Slay the Spire 2</Sts2Path>

### 构建 DLL

    dotnet build

构建成功后会自动把 CultLeaderMod.dll 复制到游戏的 mods/CultLeaderMod/。

### 导出 PCK

新增或修改图片、场景、Godot 资源后需要重新导出：

    dotnet build -t:ExportPck

### 安装文件

确保游戏 mods/CultLeaderMod/ 下包含：

- CultLeaderMod.dll
- CultLeaderMod.pck
- CultLeaderMod.json

## 测试与调试

### 控制台命令

游戏中打开控制台后可使用：

    card <卡牌ID>
    upgrade <手牌索引>
    remove_card
    draw
    discard
    energy

例如：

    card CULT_LEADER_MOD_CARD_APOSTLE_PURE_19
    upgrade 0

### 当前已知事项

- 当前 dotnet build 为 **0 errors / 4 warnings**。
- 4 个 warning 主要来自 TempMaxHpPower、TempMaxHpLossPower、LifeEssencePower、Apostle_Melancholy_19，不阻断运行。
- 自定义事件大图布局仍在调整，事件图片显示位置可能继续优化。

## 反馈

- 游戏内：设置界面会显示一个 **教主Mod反馈** 按钮，点击后打开 GitHub 问题提交页。
- 网页：https://github.com/bathynomus-giganteus/ChubbyCheeks/issues/new?template=mod_bug_report.yml
- 提交前请尽量填写 Mod 版本、问题模块、复现步骤、实际表现和期望表现。

## 依赖与致谢

- **STS2 RitsuLib**：Mod 框架、卡牌/遗物注册与本地化能力
- **Harmony**：运行时补丁
- Slay the Spire 2 Modding 社区与工具链
