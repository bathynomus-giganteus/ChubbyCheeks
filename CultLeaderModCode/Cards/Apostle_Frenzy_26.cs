using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Frenzy_26 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("Cards", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/发射啊biu.png");

    public Apostle_Frenzy_26()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        await ApostleCardEffectHelpers.Attack(
            choiceContext,
            this,
            cardPlay,
            target,
            DynamicVars.Damage.BaseValue
        );

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

        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}