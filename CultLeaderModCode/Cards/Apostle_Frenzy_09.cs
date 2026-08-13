using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Frenzy_09 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Cards", 4m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/鹿派斩击.png");

    public Apostle_Frenzy_09()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        var discard = PileType.Discard.GetPile(base.Owner);
        int maxCards = Math.Min(DynamicVars["Cards"].IntValue, discard.Cards.Count);
        if (maxCards <= 0)
            return;

        var selected = await CardSelectCmd.FromCombatPile(
            choiceContext,
            discard,
            base.Owner,
            new CardSelectorPrefs(base.SelectionScreenPrompt, 0, maxCards)
        );

        foreach (var card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
            await ApostleCardEffectHelpers.Attack(
                choiceContext,
                this,
                cardPlay,
                target,
                DynamicVars.Damage.BaseValue
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}