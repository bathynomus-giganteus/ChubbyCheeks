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
public class Apostle_Pure_18 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Threshold", 5m), new DynamicVar("BufferAmt", 1m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/突发惊吓.png");

    public Apostle_Pure_18()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await PowerCmd.Apply<BufferPower>(choiceContext, owner, DynamicVars["BufferAmt"].BaseValue, owner, this);

        if (ApostleCardEffectHelpers.PureStacks(owner) > DynamicVars["Threshold"].BaseValue)
        {
            await ApostleCardEffectHelpers.TriggerPureStacks(choiceContext, owner, DynamicVars["Threshold"].IntValue, this);
            await PowerCmd.Apply<BufferPower>(choiceContext, owner, DynamicVars["BufferAmt"].BaseValue, owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
