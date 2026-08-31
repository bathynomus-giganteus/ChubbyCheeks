using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
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
public class Apostle_Frenzy_21 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("PlatingAmt", 4m), new DynamicVar("StrengthLoss", 10m), new DynamicVar("VigorNextTurn", 10m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/古老的誓约.png");

    public Apostle_Frenzy_21()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await ApostleCardPlayHelpers.ApplyCalmPower(choiceContext, owner, DynamicVars["PlatingAmt"].BaseValue, owner, this);
        await ApostleCardEffectHelpers.ApplyTemporaryStrengthLoss(choiceContext, owner, DynamicVars["StrengthLoss"].BaseValue, owner, this);
        await PowerCmd.Apply<VigorPerTurnPower>(choiceContext, owner, DynamicVars["VigorNextTurn"].BaseValue, owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PlatingAmt"].UpgradeValueBy(2m);
        DynamicVars["StrengthLoss"].UpgradeValueBy(5m);
        DynamicVars["VigorNextTurn"].UpgradeValueBy(5m);
    }
}
