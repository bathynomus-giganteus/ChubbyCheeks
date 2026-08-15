using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_08 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_08.png");

    public Apostle_Lively_08()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner;
        var owner = player.Creature;
        var scope = base.CardScope;
        if (scope == null)
            return;

        int retainStacks = (int)(
            (owner.GetPower<RetainPower>()?.Amount ?? 0m)
            + (owner.GetPower<HappinessPower>()?.Amount ?? 0m)
        );

        var options = new List<CardModel>
        {
            scope.CreateCard<Apostle_Lively_08_1>(player),
            scope.CreateCard<Apostle_Lively_08_2>(player),
            scope.CreateCard<Apostle_Lively_08_3>(player),
        };

        if (retainStacks >= 10)
        {
            foreach (var option in options)
            {
                await CardPileCmd.Add(option, PileType.Hand, CardPilePosition.Top, this, false);
            }
            return;
        }

        var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, player);
        if (chosen != null)
        {
            await CardPileCmd.Add(chosen, PileType.Hand, CardPilePosition.Top, this, false);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
