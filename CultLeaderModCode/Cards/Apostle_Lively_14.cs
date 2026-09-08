using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_14 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainPerCard", 2m), new DynamicVar("DrawPerCard", 1m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_14.png");

    public Apostle_Lively_14()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var hand = PileType.Hand.GetPile(base.Owner);
        var drawPile = PileType.Draw.GetPile(base.Owner);
        var discarded = hand.Cards
            .Where(card => card != this)
            .Concat(drawPile.Cards)
            .ToList();

        if (discarded.Count > 0)
            await CardCmd.Discard(choiceContext, discarded);

        int retain = discarded.Count * DynamicVars["RetainPerCard"].IntValue;
        if (retain > 0)
        {
            await ApostleCardPlayHelpers.ApplyLivelyPower(
                choiceContext,
                owner,
                retain,
                owner,
                this
            );
        }

        int draw = discarded.Count * DynamicVars["DrawPerCard"].IntValue;
        if (draw > 0)
            await CardPileCmd.Draw(choiceContext, draw, base.Owner);
    }

    protected override void OnUpgrade() { }
}
