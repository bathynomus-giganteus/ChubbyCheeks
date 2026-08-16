using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class GiantPotionRelic : CultLeaderModRelic
{
    private bool _firstAttackRemaining;

    public override RelicRarity Rarity => RelicRarity.Rare;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/giant_potion.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/giant_potion.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/giant_potion.png";

    public override Task BeforeCombatStart()
    {
        _firstAttackRemaining = true;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner && cardPlay.Card.Type == CardType.Attack)
            _firstAttackRemaining = false;

        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (!_firstAttackRemaining || cardSource == null || cardSource.Owner != base.Owner || cardSource.Type != CardType.Attack || dealer != base.Owner.Creature)
            return 1m;

        return 2m;
    }
}
