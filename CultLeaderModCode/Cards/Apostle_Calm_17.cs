using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_17 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Cards", 1m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/修剪枝条.png");

    public Apostle_Calm_17()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int maxSelect = DynamicVars["Cards"].IntValue;
        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            base.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, maxSelect),
            null,
            this
        );

        int totalCost = 0;
        foreach (var card in cards)
        {
            totalCost += card.EnergyCost.GetResolved();
            await CardCmd.Exhaust(choiceContext, card);
        }

        if (totalCost > 0)
            await ApostleCardPlayHelpers.ApplyCalmPower(choiceContext, owner, totalCost, owner, this);

        int plating = ApostleCardEffectHelpers.CalmStacks(owner);
        if (plating > 0)
        {
            var enemy = ApostleCardEffectHelpers.RandomEnemy(owner);
            if (enemy != null)
                await ApostleCardEffectHelpers.Attack(choiceContext, this, cardPlay, enemy, plating);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
