# 调试方法论：日志诊断流程

## 触发条件
同一个 bug 修改两次仍未解决时，强制执行以下流程。

## 步骤

### 1. 读取游戏日志
- 路径: C:\Users\888\AppData\Roaming\SlayTheSpire2\logs\godot.log
- 命令: Get-Content <path> -Tail 200 -Encoding UTF8
- 关注: [ERROR]、[WARN]、异常堆栈、卡牌/能力相关日志

### 2. 交叉比对
- 对照日志中的异常信息和相关源码文件
- 确认 DLL 是否已正确构建并部署到 Steam 目录
- 检查本地化文件是否为空或损坏

### 3. 定位根因
- 根据日志中的错误类型反查代码
- 必要时反编译游戏文件对比 API 签名

### 4. 验证修复
- dotnet build 确认编译通过
- 确认 DLL 已复制到 Steam mods 文件夹
- 如涉及图片资源，确认已在 Godot 中并导出 PCK

## 关键路径
- 源码: C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderModCode\
- 本地化: C:\Users\888\OneDrive\codex\sts2-mods\CultLeaderMod\CultLeaderMod\localization\
- Steam 部署: E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod\
- DLL 文件: 上述路径下的 CultLeaderMod.dll
- Godot: E:\game\Godot\Godot_v4.5.1-stable_mono_win64.exe