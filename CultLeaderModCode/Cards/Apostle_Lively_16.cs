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
public class Apostle_Lively_16 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("BaseDrawAmt", 1m),
            new DynamicVar("Threshold", 5m),
            new DynamicVar("RemoveAmt", 3m),
            new DynamicVar("BonusDrawAmt", 2m)
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_16.png");

    public Apostle_Lively_16()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await CardPileCmd.Draw(choiceContext, DynamicVars["BaseDrawAmt"].BaseValue, base.Owner);

        int available = ApostleCardEffectHelpers.LivelyStacks(owner);
        if (available < DynamicVars["Threshold"].IntValue)
            return;

        int remove = Math.Min(DynamicVars["RemoveAmt"].IntValue, available);
        await ApostleCardEffectHelpers.RemoveLivelyStacks(choiceContext, owner, remove, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars["BonusDrawAmt"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade() { }
}
