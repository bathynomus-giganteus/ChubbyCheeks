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

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_12 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("StrengthLoss", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/水大炮炮击.png");

    public Apostle_Melancholy_12()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        foreach (var enemy in ApostleCardEffectHelpers.AliveEnemies(owner))
        {
            await ApostleCardEffectHelpers.ApplyTemporaryStrengthLoss(
                choiceContext,
                enemy,
                DynamicVars["StrengthLoss"].BaseValue,
                owner,
                this
            );
        }

        await ApostleCardEffectHelpers.AttackAll(
            choiceContext,
            this,
            cardPlay,
            owner,
            DynamicVars.Damage.BaseValue
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["StrengthLoss"].UpgradeValueBy(1m);
    }

}
