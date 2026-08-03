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
using System.Collections.Generic;

namespace CultLeaderMod.CultLeaderModCode.Cards;

// ================================================================
//  Pure Apostle Cards (#1-#9)
// ================================================================

/// <summary>Elfen (King) �� Mana Wild Strike</summary>
public sealed class PureManaWildStrike : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "�����ң�������";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/elfen_king_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/elfen_king_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/elfen_king_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public PureManaWildStrike() : base(1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var regen = Owner.Creature.GetPower<RegenPower>();
        var essence = Owner.Creature.GetPower<LifeEssencePower>();
        int total = (int)((regen?.Amount ?? 0) + (essence?.Amount ?? 0));
        if (total == 0) return;
        if (regen != null) await PowerCmd.Remove(regen);
        if (essence != null) await PowerCmd.Remove(essence);
        var dmg = DynamicVars.Damage.BaseValue;
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        var rng = new Random();
        for (int i = 0; i < total; i++)
        {
            var target = enemies[rng.Next(enemies.Count)];
            await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(target).Execute(choiceContext);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>Viviana �� Come to the Maiden's Side?</summary>
public sealed class PureComeToMaidensSide : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "ޱޱ����";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/viviana_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/viviana_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/viviana_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(25m, ValueProp.Move), new PowerVar<RegenPower>("RegenPower", 4m)];

    public PureComeToMaidensSide() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars["RegenPower"].BaseValue, this, elder, ApostlePersonality.Pure);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
        DynamicVars["RegenPower"].UpgradeValueBy(1m);
    }
}

/// <summary>Ran �� Encirclement Hunt</summary>
public sealed class PureEncirclementHunt : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "�";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/ran_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/ran_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/ran_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public PureEncirclementHunt() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var dmg = DynamicVars.Damage.BaseValue;
        for (int i = 0; i < 3; i++)
            await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        if (cardPlay.Target.CurrentHp < Owner.Creature.MaxHp)
            await DamageCmd.Attack(dmg).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade() => EnergyCost.SetCustomBaseCost(1);
}

/// <summary>Aira �� Vacation Escape</summary>
public sealed class PureVacationEscape : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "������";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/aira_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/aira_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/aira_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<RegenPower>("RegenPower", 3m), new BlockVar(8m, ValueProp.Move)];

    public PureVacationEscape() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars["RegenPower"].BaseValue, this, elder, ApostlePersonality.Pure);
        int totalStacks = ApostleCardHelper.TotalRegenStacks(Owner.Creature);
        await CreatureCmd.GainBlock(Owner.Creature, totalStacks * (int)DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay, false);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["RegenPower"].UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}

/// <summary>Mayo (Super Cool) �� Strongest Collectible</summary>
public sealed class PureStrongestCollectible : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "��Լ";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/mayo_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/mayo_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/mayo_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public PureStrongestCollectible() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        if (ApostleCardHelper.TotalRegenStacks(Owner.Creature) <= 10) return;
        foreach (var enemy in CombatState!.Enemies.Where(e => e != cardPlay.Target && !e.IsDead))
            await CreatureCmd.Stun(enemy, "");
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5m);
}

/// <summary>Spicy �� Pumpkin Magic</summary>
public sealed class PurePumpkinMagic : CultLeaderModCard, IApostleCard
{
    private readonly HashSet<CardKeyword> _keywords = [CardKeyword.Exhaust];

    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "˹Ƥ��";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/spicy_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/spicy_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/spicy_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => _keywords;

    public PurePumpkinMagic() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, 2m, this, elder, ApostlePersonality.Pure);
        await CreatureCmd.GainBlock(Owner.Creature, 5m, ValueProp.Move, cardPlay, false);
    }
    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}

/// <summary>Gavia �� "I'll... protect you..."</summary>
public sealed class PureProtectYou : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "��ά��";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/gavia_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/gavia_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/gavia_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<RegenPower>("RegenPower", 5m), new BlockVar(10m, ValueProp.Move), new DynamicVar("Draw", 1m)];

    public PureProtectYou() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars["RegenPower"].BaseValue, this, elder, ApostlePersonality.Pure);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].BaseValue, Owner, false);
    }
    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}

/// <summary>Sally �� Mischievous Smile</summary>
public sealed class PureMischievousSmile : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "ɯ��";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/sally_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/sally_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/sally_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>(2m)];

    public PureMischievousSmile() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["WeakPower"].BaseValue, Owner.Creature, this, false);
        int debuffCount = cardPlay.Target.Powers.Count(p => p.Type == PowerType.Debuff);
        if (debuffCount > 0)
        {
            bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
            await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, debuffCount, this, elder, ApostlePersonality.Pure);
        }
        if (!IsUpgraded)
            EnergyCost.AddThisCombat(1, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Margo �� Margoma Recovery</summary>
public sealed class PureMargomaRecovery : CultLeaderModCard, IApostleCard
{
    private readonly HashSet<CardKeyword> _keywords = [CardKeyword.Exhaust];
    public ApostlePersonality Personality => ApostlePersonality.Pure;
    public string ApostleName => "���";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/margo_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/margo_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/pure/margo_card.png";
    public override string BetaPortraitPath => PortraitPath;
    public override IEnumerable<CardKeyword> CanonicalKeywords => _keywords;
    private const string RegenVarName = "RegenPower";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RegenPower>(RegenVarName, 3m)];

    public PureMargomaRecovery() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int currentRegen = ApostleCardHelper.TotalRegenStacks(Owner.Creature);
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        if (currentRegen < 5)
        {
            await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, DynamicVars[RegenVarName].BaseValue, this, elder, ApostlePersonality.Pure);
        }
        else
        {
            await ApostleCardHelper.TriggerRegenOrLifeEssence(choiceContext, Owner.Creature, 3, this);
            await CardPileCmd.Draw(choiceContext, 2m, Owner, false);
        }
    }
    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}