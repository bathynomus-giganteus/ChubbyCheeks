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
public class Apostle_Frenzy_15 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("VigorAmt", 15m), new DynamicVar("StrengthLoss", 15m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/时间中断.png");

    public Apostle_Frenzy_15()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await ApostleCardPlayHelpers.ApplyFrenzyPower(choiceContext, owner, DynamicVars["VigorAmt"].BaseValue, owner, this);
        var target = cardPlay.Target;
        if (target == null)
            return;

        if (ApostlePowerRules.HasElderForm(owner))
        {
            await CreatureCmd.Stun(target);
        }
        else
        {
            await ApostleCardEffectHelpers.ApplyTemporaryStrengthLoss(
                choiceContext,
                target,
                DynamicVars["StrengthLoss"].BaseValue,
                owner,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
