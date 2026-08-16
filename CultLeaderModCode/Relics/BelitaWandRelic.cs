using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class BelitaWandRelic : CultLeaderModRelic
{
    private int _apostleCardsPlayed;

    public override RelicRarity Rarity => RelicRarity.Rare;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/belita_wand.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/belita_wand.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/belita_wand.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => _apostleCardsPlayed % 10;

    [SavedProperty]
    public int ApostleCardsPlayed
    {
        get => _apostleCardsPlayed;
        set
        {
            AssertMutable();
            _apostleCardsPlayed = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner || !ApostlePowerRules.IsApostleCard(cardPlay.Card))
            return;

        ApostleCardsPlayed++;
        if (ApostleCardsPlayed % 10 == 0)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, 1m, base.Owner);
        }
    }
}
