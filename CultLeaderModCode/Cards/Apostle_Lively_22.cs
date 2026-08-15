using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_22 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainCost", 3m), new DynamicVar("BeeMaxHp", 5m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_22.png");

    public Apostle_Lively_22()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int required = DynamicVars["RetainCost"].IntValue;
        int available = ApostleCardEffectHelpers.LivelyStacks(owner);
        int remove = Math.Min(required, available);

        await ApostleCardEffectHelpers.RemoveLivelyStacks(choiceContext, owner, remove, this);

        if (remove >= required)
        {
            await PowerCmd.Apply<BeePower>(
                choiceContext,
                owner,
                DynamicVars["BeeMaxHp"].BaseValue,
                owner,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RetainCost"].UpgradeValueBy(-1m);
        DynamicVars["BeeMaxHp"].UpgradeValueBy(1m);
    }
}