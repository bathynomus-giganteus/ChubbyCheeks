using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_22 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("Triggers", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/摇曳幽烛.png");

    public Apostle_Calm_22()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int triggers = DynamicVars["Triggers"].IntValue;
        decimal damage = DynamicVars.Damage.BaseValue;

        AttackContext? attackContext = null;
        try
        {
            for (int i = 0; i < triggers; i++)
            {
                var enemy = ApostleCardEffectHelpers.RandomEnemy(owner);
                if (enemy == null)
                    break;

                bool triggered = await ApostleCardEffectHelpers.TryTriggerCalmStack(choiceContext, owner, this);
                if (!triggered)
                    break;

                attackContext ??= await AttackCommand.CreateContextAsync(base.CombatState!, choiceContext, cardPlay);
                attackContext.AddHit(await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    damage,
                    DynamicVars.Damage.Props,
                    owner,
                    this,
                    cardPlay
                ));
            }
        }
        finally
        {
            if (attackContext != null)
                await attackContext.DisposeAsync();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Triggers"].UpgradeValueBy(1m);
    }
}
