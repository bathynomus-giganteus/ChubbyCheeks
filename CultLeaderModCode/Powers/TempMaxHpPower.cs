using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 临时最大生命值 — 由生命本源赋予，战斗结束清除。
/// 每层提供5点临时最大生命值。
/// </summary>
[RegisterPower]
public class TempMaxHpPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "CultLeaderMod/images/powers/tempmaxhp.png";
    public override string CustomBigIconPath => "CultLeaderMod/images/powers/big/tempmaxhp.png";
}
