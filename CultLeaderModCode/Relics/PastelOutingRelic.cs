using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class PastelOutingRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/pastel_outing.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/pastel_outing.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/pastel_outing.png";

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (cardSource == null || dealer != base.Owner.Creature || !ApostlePowerRules.IsApostleCard(cardSource))
            return 0m;

        return 3m;
    }
}
