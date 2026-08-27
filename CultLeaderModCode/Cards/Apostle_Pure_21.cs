using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Pure_21 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("TriggerAmt", 2m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/清晰的界限.png");

    public Apostle_Pure_21()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int triggerAmt = Math.Min(
            ApostleCardEffectHelpers.PureStacks(owner),
            DynamicVars["TriggerAmt"].IntValue
        );

        for (int i = 0; i < triggerAmt; i++)
        {
            await ApostleCardEffectHelpers.TriggerPureStacks(choiceContext, owner, 1, this);
            await CardPileCmd.Draw(choiceContext, 1m, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["TriggerAmt"].UpgradeValueBy(1m);
    }
}
