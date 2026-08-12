using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 临时最大生命值 — 视觉标记。HP变更由LifeEssencePower直接管理。
/// </summary>
[RegisterPower]
public class TempMaxHpPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/maxHP.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/maxHP.png";
}