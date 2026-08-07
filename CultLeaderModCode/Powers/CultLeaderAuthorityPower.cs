using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 教主的权能 — 使徒牌获得祝福的层数+1（每层权能额外+1）。
/// 权能达到5时，消耗5层进入埃尔德形态。
/// 实际倍率加成和转换逻辑在 PowerInterceptPatch 中实现。
/// </summary>
[RegisterPower]
public class CultLeaderAuthorityPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/cultleaderauthority.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/cultleaderauthority.png";
}
