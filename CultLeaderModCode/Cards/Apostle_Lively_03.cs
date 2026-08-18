using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using System.Linq;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_03 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("MaxCards", 2m), new DynamicVar("RetainAmt", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_03.png");

    public Apostle_Lively_03()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var drawPile = PileType.Draw.GetPile(base.Owner);
        int maxCards = Math.Min(DynamicVars["MaxCards"].IntValue, drawPile.Cards.Count);

        if (maxCards <= 0)
            return;

        var selected = (await CardSelectCmd.FromCombatPile(
            choiceContext,
            drawPile,
            base.Owner,
            new CardSelectorPrefs(base.SelectionScreenPrompt, 0, maxCards)
        )).ToList();

        if (selected.Count == 0)
            return;

        foreach (var card in selected)
            await CardCmd.Exhaust(choiceContext, card);

        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            owner,
            DynamicVars["RetainAmt"].BaseValue * selected.Count,
            owner,
            this
        );

        var combatState = owner.CombatState;
        if (combatState == null)
            return;

        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(selected.Count)
            .FromCard(this, cardPlay)
            .TargetingRandomOpponents(combatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["MaxCards"].UpgradeValueBy(1m);
    }
}

