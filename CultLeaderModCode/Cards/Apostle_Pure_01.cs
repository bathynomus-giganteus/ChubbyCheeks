using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Pure_01 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move)];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/魔力乱打.png");

    public Apostle_Pure_01()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var dmg = DynamicVars.Damage.BaseValue;

        // Count and remove Regen
        var regenPower = owner.GetPower<RegenPower>();
        int regenStacks = regenPower?.Amount ?? 0;
        if (regenPower != null)
            await PowerCmd.Remove<RegenPower>(owner);

        var lifePower = owner.GetPower<LifeEssencePower>();
        int lifeStacks = lifePower?.Amount ?? 0;
        if (lifePower != null)
            await PowerCmd.Remove<LifeEssencePower>(owner);

        int totalStacks = regenStacks + lifeStacks;
        if (totalStacks <= 0)
            return;

        // Get combat state; TargetingRandomOpponents picks a fresh target per hit.
        var combatState = owner.CombatState;
        if (combatState == null)
            return;

        await DamageCmd
            .Attack(dmg)
            .FromCard(this, cardPlay)
            .TargetingRandomOpponents(combatState)
            .WithHitCount(totalStacks)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}

