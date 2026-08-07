using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 狂热 — 每层：下一张攻击牌伤害+3。触发时消耗1层（非全部），同时消耗3生命。
/// </summary>
[RegisterPower]
public class FervorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/fervor.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/fervor.png";
}
