using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 埃尔德形态 — 升级所有使徒牌相关形态。
/// 基础buff自动替换为升级版buff（逻辑在 PowerInterceptPatch 中实现）。
/// </summary>
[RegisterPower]
public class ElderFormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "CultLeaderMod/images/powers/elderform.png";
    public override string CustomBigIconPath => "CultLeaderMod/images/powers/big/elderform.png";
}
