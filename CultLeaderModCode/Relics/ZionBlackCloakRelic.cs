using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class ZionBlackCloakRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/zion_black_cloak.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/zion_black_cloak.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/zion_black_cloak.png";

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (amount <= 0m || power.Type != PowerType.Debuff || applier != base.Owner.Creature || power.Owner == base.Owner.Creature)
            return;

        Flash();
        await CreatureCmd.GainBlock(base.Owner.Creature, 1m, ValueProp.Unpowered, null);
    }
}
