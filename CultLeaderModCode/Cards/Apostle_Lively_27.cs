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
public class Apostle_Lively_27 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainAmt", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_27.png");

    public Apostle_Lively_27()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner;
        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            owner.Creature,
            DynamicVars["RetainAmt"].BaseValue,
            owner.Creature,
            this
        );

        var combatState = owner.Creature.CombatState;
        if (combatState == null)
            return;

        var candidates = PileType.Draw.GetPile(owner).Cards
            .Where(card => card.Type == CardType.Attack)
            .ToList();
        if (candidates.Count == 0)
            return;

        var selected = candidates[Random.Shared.Next(candidates.Count)];
        await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Top, this, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RetainAmt"].UpgradeValueBy(3m);
    }
}
