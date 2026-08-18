using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
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
public class Apostle_Melancholy_04 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(15m, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/内向人斩切.png");

    public Apostle_Melancholy_04()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        if (!ApostleCardEffectHelpers.AliveEnemies(owner).Any())
            return;

        await using AttackContext attackContext = await AttackCommand.CreateContextAsync(base.CombatState!, choiceContext, cardPlay);
        await ExecuteLowestHpHit(choiceContext, cardPlay, attackContext);
    }

    private async Task ExecuteLowestHpHit(PlayerChoiceContext choiceContext, CardPlay cardPlay, AttackContext attackContext)
    {
        var owner = base.Owner.Creature;
        var target = ApostleCardEffectHelpers.AliveEnemies(owner)
            .OrderBy(enemy => enemy.CurrentHp)
            .FirstOrDefault();
        if (target == null)
            return;

        attackContext.AddHit(await CreatureCmd.Damage(
            choiceContext,
            target,
            DynamicVars.Damage.BaseValue,
            DynamicVars.Damage.Props,
            owner,
            this,
            cardPlay
        ));

        if (target.IsDead)
        {
            var next = ApostleCardEffectHelpers.AliveEnemies(owner)
                .OrderBy(enemy => enemy.CurrentHp)
                .FirstOrDefault();
            if (next != null)
            {
                attackContext.AddHit(await CreatureCmd.Damage(
                    choiceContext,
                    next,
                    DynamicVars.Damage.BaseValue,
                    DynamicVars.Damage.Props,
                    owner,
                    this,
                    cardPlay
                ));
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }

}
