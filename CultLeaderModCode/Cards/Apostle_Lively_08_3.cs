using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_08_3 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainAmt", 3m), new DynamicVar("StrengthAmt", 1m), new DynamicVar("DexterityAmt", 1m)];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/dedicated_to_allies.png");

    public Apostle_Lively_08_3()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            owner,
            DynamicVars["RetainAmt"].BaseValue,
            owner,
            this
        );
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            owner,
            DynamicVars["StrengthAmt"].BaseValue,
            owner,
            this
        );
        await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            owner,
            DynamicVars["DexterityAmt"].BaseValue,
            owner,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RetainAmt"].UpgradeValueBy(1m);
        DynamicVars["StrengthAmt"].UpgradeValueBy(1m);
        DynamicVars["DexterityAmt"].UpgradeValueBy(1m);
    }
}
