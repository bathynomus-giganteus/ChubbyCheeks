using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_13 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(50m, ValueProp.Move), new DynamicVar("DebuffApplied", 0m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/有罪宣言.png");

    public Apostle_Melancholy_13()
        : base(15, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    public override void AfterCreated()
    {
        base.AfterCreated();
        DynamicVars["DebuffApplied"].BaseValue = 0m;
        Drawn += RefreshCost;
        RefreshCost();
    }

    private void RefreshCost()
    {
        if (base.Owner == null)
            return;

        int total = Math.Max(0, DynamicVars["DebuffApplied"].IntValue);

        EnergyCost.SetUntilPlayed(
            Math.Max(0, EnergyCost.Canonical - total),
            reduceOnly: false
        );
    }

    public static void RecordDebuffApplied(Player player)
    {
        if (player == null)
            return;

        var pileTypes = new[] { PileType.Draw, PileType.Hand, PileType.Discard, PileType.Exhaust };
        foreach (var pileType in pileTypes)
        {
            foreach (var card in pileType.GetPile(player).Cards.OfType<Apostle_Melancholy_13>())
            {
                card.DynamicVars["DebuffApplied"].BaseValue += 1m;
                card.RefreshCost();
                if (pileType == PileType.Hand)
                    NCard.FindOnTable(card)?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
            }
        }
    }

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

        DynamicVars["DebuffApplied"].BaseValue = 0m;
        RefreshCost();
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(15m);
    }

}
