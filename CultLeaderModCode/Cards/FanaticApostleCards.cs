using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;

namespace CultLeaderMod.CultLeaderModCode.Cards;

// ================================================================
//  Fanatic Apostle Cards (#1-#26)
// ================================================================

/// <summary>Chloe - Little Sebastian</summary>
public sealed class FanaticLittleSebastian : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "克萝伊";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/chloe_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/chloe_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/chloe_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public FanaticLittleSebastian() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int vigor = (int)Owner.Creature.GetPowerAmount<VigorPower>();
        if (vigor > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, (decimal)(vigor * 2), ValueProp.Move, cardPlay, false);
            var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
            if (enemies.Count > 0)
            {
                var target = enemies[new Random().Next(enemies.Count)];
                await DamageCmd.Attack((decimal)vigor).FromCard(this, cardPlay).Targeting(target).Execute(choiceContext);
            }
        }
    }
    protected override void OnUpgrade() { }
}

/// <summary>Diana - True Healing</summary>
public sealed class FanaticTrueHealing : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "黛安娜";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/diana_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/diana_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/diana_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14m, ValueProp.Move)];

    public FanaticTrueHealing() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var tracker = Owner.Creature.GetPower<VigorConsumedTrackerPower>();
        int consumed = tracker != null ? (int)tracker.Amount : 0;
        int bonusPer = IsUpgraded ? 2 : 1;
        int bonus = consumed / 3 * bonusPer;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Shady - Killing Time</summary>
public sealed class FanaticKillingTime : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "谢迪";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/shady_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/shady_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/shady_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new PowerVar<VigorPower>("VigorPower", 2m)];

    public FanaticKillingTime() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        foreach (var enemy in enemies)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue, this, elder);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["VigorPower"].UpgradeValueBy(1m);
    }
}

/// <summary>Neil - World Tree Revelation</summary>
public sealed class FanaticWorldTreeRevelation : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "尼尔";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/neil_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/neil_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/neil_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VigorPower>("VigorPower", 2m)];

    public FanaticWorldTreeRevelation() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue, this, elder);
        int vigorNow = (int)Owner.Creature.GetPowerAmount<VigorPower>();
        int mult = IsUpgraded ? 3 : 2;
        await CreatureCmd.GainBlock(Owner.Creature, (decimal)(vigorNow * mult), ValueProp.Move, cardPlay, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Sister - Money Gun</summary>
public sealed class FanaticMoneyGun : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "西斯特";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/sister_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/sister_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/sister_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];
    public FanaticMoneyGun() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        if (cardPlay.Target.IsDead) await PlayerCmd.GainGold(IsUpgraded ? 15m : 10m, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>Berita - Crimson Rain</summary>
public sealed class FanaticCrimsonRain : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "贝利塔";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/berita_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/berita_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/berita_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    public FanaticCrimsonRain() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var d = DynamicVars.Damage.BaseValue;
        foreach (var enemy in CombatState!.Enemies.Where(e => !e.IsDead).ToList())
        {
            await DamageCmd.Attack(d).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
            await DamageCmd.Attack(d).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>Alice - Arcana</summary>
public sealed class FanaticArcana : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "爱丽丝";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/alice_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/alice_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/alice_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public FanaticArcana() : base(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int times = IsUpgraded ? 3 : 2;
        for (int i = 0; i < times; i++)
        {
            int roll = new Random().Next(3);
            if (roll == 0) { var el = CombatState!.Enemies.Where(x => !x.IsDead).ToList(); if (el.Count > 0) await DamageCmd.Attack(6m).FromCard(this, cardPlay).Targeting(el[new Random().Next(el.Count)]).Execute(choiceContext); }
            else if (roll == 1) { bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0; await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, 3m, this, elder); }
            else await CreatureCmd.GainBlock(Owner.Creature, 5m, ValueProp.Move, cardPlay, false);
        }
    }
    protected override void OnUpgrade() { }
}

/// <summary>Liz - Quenching Strike</summary>
public sealed class FanaticQuenchingStrike : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "丽兹";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/liz_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/liz_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/liz_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move)];
    public FanaticQuenchingStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int vigor = (int)Owner.Creature.GetPowerAmount<VigorPower>();
        int mult = IsUpgraded ? 5 : 3;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + vigor * mult).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

/// <summary>Tigger (Hero) - Deer Style Slash</summary>
public sealed class FanaticDeerStyleSlash : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "提格（英雄）";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/tigger_hero_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/tigger_hero_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/tigger_hero_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public FanaticDeerStyleSlash() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int max = IsUpgraded ? 5 : 4;
        decimal dmgPer = IsUpgraded ? 5m : 4m;
        foreach (var c in Owner.PlayerCombatState.DiscardPile.Cards.Take(max).ToList())
        {
            await CardCmd.Exhaust(choiceContext, c, false);
            await DamageCmd.Attack(dmgPer).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        }
    }
    protected override void OnUpgrade() { }
}

/// <summary>Annette - Biased Commentary</summary>
public sealed class FanaticBiasedCommentary : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "阿妮特";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/annette_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/annette_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/annette_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public FanaticBiasedCommentary() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int val = IsUpgraded ? 7 : 5;
        await PowerCmd.Apply<FanaticTempStrDownPower>(choiceContext, cardPlay.Target, val, Owner.Creature, this, false);
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, val, this, elder);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Netty - Spiral Drill Charge</summary>
public sealed class FanaticSpiralDrillCharge : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "涅缇";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/netty_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/netty_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/netty_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VigorPower>("VigorPower", 12m)];
    public FanaticSpiralDrillCharge() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue, this, elder);
    }
    protected override void OnUpgrade() => DynamicVars["VigorPower"].UpgradeValueBy(6m);
}

/// <summary>Lynn (Chaos) - Seduced by Chaos</summary>
public sealed class FanaticSeducedByChaos : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "琳（混沌）";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/lynn_chaos_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/lynn_chaos_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/lynn_chaos_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    public FanaticSeducedByChaos() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        int threshold = IsUpgraded ? 4 : 5;
        if ((int)Owner.Creature.GetPowerAmount<VigorPower>() >= threshold)
        {
            bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
            await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, IsUpgraded ? 6m : 5m, this, elder);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

/// <summary>Polang - Salute to Fairy Kingdom</summary>
public sealed class FanaticSaluteFairyKingdom : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "破朗";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/polang_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/polang_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/polang_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move), new PowerVar<VigorPower>("VigorPower", 3m)];
    public FanaticSaluteFairyKingdom() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue, this, elder);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(2m); DynamicVars["VigorPower"].UpgradeValueBy(2m); }
}
