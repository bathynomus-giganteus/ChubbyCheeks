using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CultLeaderMod.CultLeaderModCode.Cards;

// ================================================================
//  Pure Apostle Cards batch 2 (#10-#25)
// ================================================================

/// <summary>Sherren �� Witch Archive</summary>
public sealed class PureWitchArchive : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "л��";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/sherren_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/sherren_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/sherren_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    private const string RegenVarName = "RegenPower";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RegenPower>(RegenVarName, 0m)];

    public PureWitchArchive() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = Owner.Deck.Cards.Count(c => c is IApostleCard a && a.Personality == ApostlePersonality.Pure);
        if (count <= 0) return;
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, count, this, elder, ApostlePersonality.Pure);
    }
    protected override void OnUpgrade() => EnergyCost.SetCustomBaseCost(2);
}

/// <summary>Hailey �� Non Grata</summary>
public sealed class PureNonGrata : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/hailey_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/hailey_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/hailey_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DamageVar("BonusDamage", 8m, ValueProp.Move)];

    public PureNonGrata() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        int stacks = ApostleCardHelper.TotalRegenStacks(Owner.Creature);
        if (stacks >= 2)
        {
            var regen = Owner.Creature.GetPower<RegenPower>();
            if (regen != null && regen.Amount >= 2)
                await PowerCmd.ModifyAmount(choiceContext, regen, -2m, Owner.Creature, this, false);
            else
            {
                var le = Owner.Creature.GetPower<LifeEssencePower>();
                if (le != null && le.Amount >= 2)
                    await PowerCmd.ModifyAmount(choiceContext, le, -2m, Owner.Creature, this, false);
                else
                {
                    if (regen != null && regen.Amount > 0)
                    {
                        decimal fromRegen = Math.Min(2m, regen.Amount);
                        await PowerCmd.ModifyAmount(choiceContext, regen, -fromRegen, Owner.Creature, this, false);
                        decimal remaining = 2m - fromRegen;
                        var le2 = Owner.Creature.GetPower<LifeEssencePower>();
                        if (le2 != null && remaining > 0)
                            await PowerCmd.ModifyAmount(choiceContext, le2, -remaining, Owner.Creature, this, false);
                    }
                }
            }
            await DamageCmd.Attack(DynamicVars["BonusDamage"].BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["BonusDamage"].UpgradeValueBy(2m);
    }
}

/// <summary>Naya �� Accept the Water's Baptism!</summary>
public sealed class PureWatersBaptism : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/naya_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/naya_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/naya_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public PureWatersBaptism() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int times = IsUpgraded ? 3 : 2;
        decimal totalHealed = 0m;
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        if (elder)
        {
            var le = Owner.Creature.GetPower<LifeEssencePower>();
            if (le != null)
            {
                for (int i = 0; i < times; i++)
                {
                    if (le.Amount <= 0) break;
                    await CreatureCmd.Heal(Owner.Creature, 5m);
                    totalHealed += 5m;
                    await PowerCmd.Decrement(le);
                }
                await ApostleCardHelper.SyncLifeEssenceHp(choiceContext, Owner.Creature);
            }
        }
        else
        {
            for (int i = 0; i < times; i++)
            {
                var regen = Owner.Creature.GetPower<RegenPower>();
                if (regen == null || regen.Amount <= 0) break;
                await CreatureCmd.Heal(Owner.Creature, regen.Amount);
                totalHealed += regen.Amount;
                await PowerCmd.Decrement(regen);
            }
        }
        if (totalHealed > 0)
        {
            var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
            foreach (var enemy in enemies)
                await DamageCmd.Attack(totalHealed).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
        }
    }
    protected override void OnUpgrade() { }
}

/// <summary>Carrot �� Sap Pump Fire!</summary>
public sealed class PureSapPump : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "������";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/carrot_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/carrot_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/carrot_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    public PureSapPump() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal dmg = DynamicVars.Damage.BaseValue;
        await PowerCmd.Apply<HealDamagePower>(choiceContext, Owner.Creature, dmg, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>Daya �� Diamond Pierce</summary>
public sealed class PureDiamondPierce : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/daya_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/daya_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/daya_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public PureDiamondPierce() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int bonus = ApostleCardHelper.TotalRegenStacks(Owner.Creature);
        decimal dmg = DynamicVars.Damage.BaseValue + bonus;
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        bool killedAny = false;
        foreach (var enemy in enemies)
        {
            await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
            if (enemy.IsDead) killedAny = true;
        }
        if (killedAny)
        {
            foreach (var enemy in CombatState!.Enemies.Where(e => !e.IsDead).ToList())
                await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>Elfen �� Dodge This!!! Eh...?</summary>
public sealed class PureDodgeThis : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "������";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/elfen_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/elfen_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/elfen_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(50m, ValueProp.Move)];

    public PureDodgeThis() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal dmg = DynamicVars.Damage.BaseValue;
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        foreach (var enemy in enemies)
            await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
        await PowerCmd.Apply<SelfStunPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(15m);
}

/// <summary>Opal �� Opal Dust</summary>
public sealed class PureOpalDust : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "ŷ��";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/opal_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/opal_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/opal_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override bool GainsBlock => true;

    public PureOpalDust() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int stacks = ApostleCardHelper.TotalRegenStacks(Owner.Creature);
        int blockPerStack = IsUpgraded ? 2 : 1;
        await CreatureCmd.GainBlock(Owner.Creature, stacks * blockPerStack, ValueProp.Move, cardPlay, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Laika �� Remote Charging</summary>
public sealed class PureRemoteCharging : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/laika_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/laika_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/laika_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public PureRemoteCharging() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int threshold = IsUpgraded ? 3 : 4;
        await PowerCmd.Apply<HealEnergyPower>(choiceContext, Owner.Creature, threshold, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Cathy �� Sudden Shock</summary>
public sealed class PureSuddenShock : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/cathy_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/cathy_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/cathy_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public PureSuddenShock() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BufferPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
        if (ApostleCardHelper.TotalRegenStacks(Owner.Creature) >= 5)
            await PowerCmd.Apply<BufferPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() => EnergyCost.SetCustomBaseCost(1);
}

/// <summary>Mute �� Basic Hack Attack</summary>
public sealed class PureBasicHack : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/mute_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/mute_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/mute_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public PureBasicHack() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int stacks = ApostleCardHelper.TotalRegenStacks(Owner.Creature);
        if (stacks > 0)
        {
            await DamageCmd.Attack((decimal)stacks).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
            // Enemy loses equal Strength this turn via temp strength debuff
            await PowerCmd.Apply<BasicHackTempStrengthPower>(choiceContext, cardPlay.Target, (decimal)-stacks, Owner.Creature, this, false);
        }
    }
    protected override void OnUpgrade() => EnergyCost.SetCustomBaseCost(0);
}

/// <summary>Delia �� Help Me, Friends!</summary>
public sealed class PureHelpMeFriends : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "�����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/delia_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/delia_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/delia_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public PureHelpMeFriends() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int stacks = ApostleCardHelper.TotalRegenStacks(Owner.Creature);
        if (stacks > 0)
            await PowerCmd.Apply<HelpMeFriendsTempStrengthPower>(choiceContext, Owner.Creature, stacks, Owner.Creature, this, false);
        decimal dmg = DynamicVars.Damage.BaseValue;
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        foreach (var enemy in enemies)
            await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>Id (Recovery) �� Clear Boundaries</summary>
public sealed class PureClearBoundaries : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/id_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/id_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/id_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public PureClearBoundaries() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int draw = IsUpgraded ? 2 : 1;
        await CardPileCmd.Draw(choiceContext, draw, Owner, false);
        await ApostleCardHelper.TriggerRegenOrLifeEssence(choiceContext, Owner.Creature, draw, this);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Big Wood �� Look~ Look at Me~</summary>
public sealed class PureLookAtMe : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "��ľͷ";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/bigwood_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/bigwood_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/bigwood_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public PureLookAtMe() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        decimal steal = IsUpgraded ? 15m : 10m;
        int actual = (int)Math.Min(steal, cardPlay.Target.MaxHp - 1);
        if (actual <= 0) return;
        await CreatureCmd.SetMaxHp(cardPlay.Target, cardPlay.Target.MaxHp - actual);
        await CreatureCmd.SetMaxHp(Owner.Creature, Owner.Creature.MaxHp + actual);
        await CreatureCmd.Heal(Owner.Creature, actual);
        var tracker = Owner.Creature.GetPower<TempHpTrackerPower>();
        if (tracker == null)
            await PowerCmd.Apply<TempHpTrackerPower>(choiceContext, Owner.Creature, actual, Owner.Creature, this, false);
        else
            await PowerCmd.ModifyAmount(choiceContext, tracker, actual, Owner.Creature, this, false);
    }
}

/// <summary>Lonie �� Surrender! I Surrendered...</summary>
public sealed class PureSurrender : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/lonie_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/lonie_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/lonie_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public PureSurrender() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal amount = IsUpgraded ? 20m : 15m;
        await PowerCmd.Apply<PureSurrenderTempStrengthPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this, false);
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        await PowerCmd.Apply<PureSurrenderTempStrengthPower>(choiceContext, (IEnumerable<Creature>)enemies, amount, Owner.Creature, this, false);
    }
}

/// <summary>Alette �� Shovel Strike</summary>
public sealed class PureShovelStrike : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "������";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/alette_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/alette_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/alette_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0m, ValueProp.Move)];

    public PureShovelStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        decimal fraction = IsUpgraded ? 0.25m : 0.20m;
        decimal dmg = Math.Floor(Owner.Creature.MaxHp * fraction);
        if (dmg > 0)
            await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }
}

/// <summary>Joey �� Cucumber Oil</summary>
public sealed class PureCucumberOil : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "����";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/joey_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/joey_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/joey_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public PureCucumberOil() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, 2m, this, elder, ApostlePersonality.Pure);
        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1m, Owner, false);
    }
}

// �T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T
//  Temporary Strength powers for Pure cards
// �T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T�T

public sealed class HelpMeFriendsTempStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<PureHelpMeFriends>();
    protected override bool IsPositive => true;
}

public sealed class PureSurrenderTempStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<PureSurrender>();
    protected override bool IsPositive => false;
}

public sealed class BasicHackTempStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<PureBasicHack>();
    protected override bool IsPositive => false;
}