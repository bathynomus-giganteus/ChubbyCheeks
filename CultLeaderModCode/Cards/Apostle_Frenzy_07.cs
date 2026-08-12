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
public class Apostle_Frenzy_07 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move), new DynamicVar("VigorAmt", 3m), new BlockVar(5m, ValueProp.Move), new DynamicVar("Triggers", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/阿卡那.png");

    public Apostle_Frenzy_07()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        for (int i = 0; i < DynamicVars["Triggers"].IntValue; i++)
        {
            switch (Random.Shared.Next(3))
            {
                case 0:
                    var enemy = ApostleCardEffectHelpers.RandomEnemy(owner);
                    if (enemy != null)
                        await ApostleCardEffectHelpers.Attack(choiceContext, this, cardPlay, enemy, DynamicVars.Damage.BaseValue);
                    break;
                case 1:
                    await ApostleCardPlayHelpers.ApplyFrenzyPower(choiceContext, owner, DynamicVars["VigorAmt"].BaseValue, owner, this);
                    break;
                default:
                    await CreatureCmd.GainBlock(owner, DynamicVars.Block, cardPlay);
                    break;
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Triggers"].UpgradeValueBy(1m);
    }
}
