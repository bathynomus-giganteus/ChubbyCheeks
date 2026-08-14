using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_10 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("PlatingAmt", 2m), new DynamicVar("Cards", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/限定贴纸.png");

    public Apostle_Calm_10()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await ApostleCardPlayHelpers.ApplyCalmPower(
            choiceContext,
            owner,
            DynamicVars["PlatingAmt"].BaseValue,
            owner,
            this
        );

        var drawPile = PileType.Draw.GetPile(base.Owner);
        int maxCards = Math.Min(DynamicVars["Cards"].IntValue, drawPile.Cards.Count);
        if (maxCards <= 0)
            return;

        var selected = await CardSelectCmd.FromCombatPile(
            choiceContext,
            drawPile,
            base.Owner,
            new CardSelectorPrefs(base.SelectionScreenPrompt, 0, maxCards)
        );

        foreach (var card in selected)
        {
            var replacement = base.CardScope?.CreateCard<Apostle_Calm_10>(base.Owner);
            if (replacement == null)
                continue;
            await CardCmd.Transform(card, replacement, CardPreviewStyle.None);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PlatingAmt"].UpgradeValueBy(1m);
    }
}
