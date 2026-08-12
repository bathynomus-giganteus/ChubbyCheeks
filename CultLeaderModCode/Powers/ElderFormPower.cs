using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 埃尔德形态：进入时把当前已有的五种性格基础 buff 等量转换为升级 buff。
/// 后续使徒牌通过卡牌侧 helper 判断形态并直接给予升级 buff，不再全局拦截 PowerCmd。
/// </summary>
[RegisterPower]
public class ElderFormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/elderform.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/elderform.png";

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        Entry.Logger.Info("[ElderFormPower] Applied; converting existing base personality powers.");
        await ApostlePowerRules.ConvertExistingBasePowersToElderUpgrades(
            new ThrowingPlayerChoiceContext(),
            Owner,
            applier ?? Owner,
            cardSource
        );
    }
}
