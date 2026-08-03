using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;

namespace CultLeaderMod.CultLeaderModCode.Cards;

// ================================================================
//  Support Powers for Fanatic cards
// ================================================================

/// <summary>Tracks total vigor points consumed by Fanaticism across the combat.</summary>
public sealed class VigorConsumedTrackerPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
}

/// <summary>Temporary negative strength applied to enemies.</summary>
public sealed class FanaticTempStrDownPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<FanaticBiasedCommentary>();
    protected override bool IsPositive => false;
}

/// <summary>At start of next turn, gain stored vigor and remove self.</summary>
public sealed class FanaticNextTurnVigorPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side,
        IReadOnlyList<Creature> participants, MegaCrit.Sts2.Core.Combat.ICombatState combatState)
    {
        if (side != MegaCrit.Sts2.Core.Combat.CombatSide.Player || !participants.Contains(base.Owner)) return;
        int amount = Amount;
        if (amount <= 0) return;
        Flash();
        bool elder = Owner.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(new ThrowingPlayerChoiceContext(), Owner, amount, null!, elder);
        await PowerCmd.Remove(this);
    }
}

/// <summary>At start of each turn, gain vigor. (Ifrit - Campfire)</summary>
public sealed class FanaticCampfirePower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;

    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side,
        IReadOnlyList<Creature> participants, MegaCrit.Sts2.Core.Combat.ICombatState combatState)
    {
        if (side != MegaCrit.Sts2.Core.Combat.CombatSide.Player || !participants.Contains(base.Owner)) return;
        Flash();
        bool elder = Owner.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(new ThrowingPlayerChoiceContext(), Owner, Amount, null!, elder);
    }
}

/// <summary>Whenever you heal, gain vigor equal to amount. (Sylvia - Come See Maiden)</summary>
public sealed class FanaticHealToVigorPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;

    public override async Task AfterHeal(PlayerChoiceContext ctx, decimal amount)
    {
        if (amount <= 0) return;
        Flash();
        bool elder = Owner.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(ctx, Owner, amount, null!, elder);
    }
}

/// <summary>Every N vigor consumed, gain 2 vigor. (Hailey Sober - Determination Advance)</summary>
public sealed class FanaticDeterminationPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    private int _threshold;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _threshold = (int)Amount;
    }

    /// <summary>Called externally whenever a vigor stack is consumed.</summary>
    public async Task OnVigorConsumed(PlayerChoiceContext ctx)
    {
        if (_threshold <= 0) return;
        int current = (int)Amount;
        if (current <= 1)
        {
            await PowerCmd.ModifyAmount(ctx, this, (decimal)(_threshold - current), Owner, null, false);
            Flash();
            bool elder = Owner.GetPowerAmount<ElderFormPower>() > 0;
            await ApostleCardHelper.ApplyWithAuthority(ctx, Owner, 2m, null!, elder);
        }
        else
        {
            await PowerCmd.ModifyAmount(ctx, this, -1m, Owner, null, false);
        }
    }
}

// ================================================================
//  Fanatic Apostle Cards (#14-#26)
// ================================================================

/// <summary>Pira — Time To Collect</summary>
public sealed class FanaticTimeToCollect : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "皮拉";
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/pira_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/pira_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/pira_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticTimeToCollect() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        int count = enemies.Count;
        if (count <= 0) return;
        int blockPer = IsUpgraded ? 15 : 10;
        int vigorPer = IsUpgraded ? 5 : 3;
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await CreatureCmd.GainBlock(Owner.Creature, (decimal)(count * blockPer), ValueProp.Move, cardPlay, false);
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, count * vigorPer, this, elder);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Liniuwa — Time Interrupt</summary>
public sealed class FanaticTimeInterrupt : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "莉纽瓦";
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/liniuwa_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/liniuwa_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/liniuwa_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticTimeInterrupt() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
        await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, 15m, this, elder);
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<FanaticTempStrDownPower>(choiceContext, enemy, 10m, Owner.Creature, this, false);
            if (elder)
                await CreatureCmd.Stun(enemy, "");
        }
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

/// <summary>Rolette — Applause Actor</summary>
public sealed class FanaticApplauseActor : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "罗莱特";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/rolette_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/rolette_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/rolette_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticApplauseActor() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.GainBlock(Owner.Creature, 8m, ValueProp.Move, cardPlay, false);
        await CardPileCmd.Draw(choiceContext, 1, Owner, false);
        if (IsUpgraded)
        {
            bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
            await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, 5m, this, elder);
        }
    }
    protected override void OnUpgrade() { }
}

/// <summary>Heidi — Infiltrating Interview</summary>
public sealed class FanaticInfiltratingInterview : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "海蒂";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/heidi_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/heidi_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/heidi_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticInfiltratingInterview() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = IsUpgraded ? 2 : 1;
        var hand = Owner.PlayerCombatState.Hand.Cards.Where(c => c != this).ToList();
        if (hand.Count == 0) return;
        int toExhaust = Math.Min(count, hand.Count);
        decimal totalCost = 0m;
        // Exhaust rightmost cards from hand (excluding self)
        for (int i = 0; i < toExhaust; i++)
        {
            var card = hand[hand.Count - 1 - i];
            totalCost += card.EnergyCost.Canonical;
            await CardCmd.Exhaust(choiceContext, card, false);
        }
        if (totalCost > 0)
        {
            bool elder = Owner.Creature.GetPowerAmount<ElderFormPower>() > 0;
            await ApostleCardHelper.ApplyWithAuthority(choiceContext, Owner.Creature, totalCost, this, elder);
        }
    }
    protected override void OnUpgrade() { }
}

/// <summary>Daya (Shining) — Innocence Beam</summary>
public sealed class FanaticInnocenceBeam : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "达雅（纯真闪耀）";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/daya_shining_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/daya_shining_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/daya_shining_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public FanaticInnocenceBeam() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.Enemies.Where(e => !e.IsDead).ToList();
        foreach (var enemy in enemies)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
        int vigor = (int)Owner.Creature.GetPowerAmount<VigorPower>();
        int mult = IsUpgraded ? 3 : 2;
        if (vigor > 0)
        {
            foreach (var enemy in enemies)
                await DamageCmd.Attack((decimal)(vigor * mult)).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
        }
    }
    protected override void OnUpgrade() { }
}

/// <summary>Hailey (Sober) — Determination Advance</summary>
public sealed class FanaticDeterminationAdvance : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "海莉（清醒）";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/hailey_sober_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/hailey_sober_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/hailey_sober_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticDeterminationAdvance() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int threshold = IsUpgraded ? 4 : 5;
        await PowerCmd.Apply<FanaticDeterminationPower>(choiceContext, Owner.Creature, threshold, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Sylvia — Come See Maiden</summary>
public sealed class FanaticComeSeeMaiden : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "西尔维娅";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/sylvia_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/sylvia_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/sylvia_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticComeSeeMaiden() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FanaticHealToVigorPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() => EnergyCost.SetCustomBaseCost(1);
}

/// <summary>Skia — Ancient Oath</summary>
public sealed class FanaticAncientOath : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "斯琪娅";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/skia_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/skia_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/skia_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticAncientOath() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int platingAmt = IsUpgraded ? 6 : 4;
        int strDown = IsUpgraded ? 15 : 10;
        int vigorAmt = IsUpgraded ? 15 : 10;
        await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.PlatingPower>(choiceContext, Owner.Creature, platingAmt, Owner.Creature, this, false);
        await PowerCmd.Apply<FanaticTempStrDownPower>(choiceContext, Owner.Creature, strDown, Owner.Creature, this, false);
        await PowerCmd.Apply<FanaticNextTurnVigorPower>(choiceContext, Owner.Creature, vigorAmt, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Master2 — Robot Matrix</summary>
public sealed class FanaticRobotMatrix : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "大师2号";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/master2_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/master2_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/master2_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];

    public FanaticRobotMatrix() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        int vigor = (int)Owner.Creature.GetPowerAmount<VigorPower>();
        int drawThreshold = IsUpgraded ? 4 : 5;
        if (vigor >= drawThreshold)
            await CardPileCmd.Draw(choiceContext, 1, Owner, false);
        int energyThreshold = IsUpgraded ? 16 : 20;
        if (vigor >= energyThreshold)
            await PlayerCmd.GainEnergy(2m, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}

/// <summary>Mayo — That Collectible Is Mine</summary>
public sealed class FanaticThatCollectibleIsMine : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "玛约";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/mayo_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/mayo_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/mayo_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];

    public FanaticThatCollectibleIsMine() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        int vigor = (int)Owner.Creature.GetPowerAmount<VigorPower>();
        int threshold = IsUpgraded ? 20 : 25;
        if (vigor >= threshold)
            await CreatureCmd.Stun(cardPlay.Target, "");
    }
    protected override void OnUpgrade() { }
}

/// <summary>Ifrit — Campfire</summary>
public sealed class FanaticCampfire : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "伊芙利特";
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/ifrit_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/ifrit_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/ifrit_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticCampfire() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FanaticCampfirePower>(choiceContext, Owner.Creature, IsUpgraded ? 5m : 3m, Owner.Creature, this, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Mason — Shuriken Fly</summary>
public sealed class FanaticShurikenFly : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "梅森";
    public override bool GainsBlock => true;
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/mason_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/mason_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/mason_card.png";
    public override string BetaPortraitPath => PortraitPath;

    public FanaticShurikenFly() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int vigor = (int)Owner.Creature.GetPowerAmount<VigorPower>();
        int blockPer = IsUpgraded ? 2 : 1;
        int totalBlock = vigor / 3 * blockPer;
        if (totalBlock > 0)
            await CreatureCmd.GainBlock(Owner.Creature, (decimal)totalBlock, ValueProp.Move, cardPlay, false);
    }
    protected override void OnUpgrade() { }
}

/// <summary>Liumeimei — Fire Ah Biu</summary>
public sealed class FanaticFireAhBiu : CultLeaderModCard, IApostleCard
{
    public ApostlePersonality Personality => ApostlePersonality.Fanatic;
    public string ApostleName => "刘美美";
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override string? StarIconPath => "res://CultLeaderMod/images/apostle_icons/liumeimei_avatar.png";
    public override string CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/big/liumeimei_card.png";
    public override string PortraitPath => "res://CultLeaderMod/images/card_portraits/fanatic/liumeimei_card.png";
    public override string BetaPortraitPath => PortraitPath;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];

    public FanaticFireAhBiu() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
        int count = Math.Min(3, Owner.PlayerCombatState.DiscardPile.Cards.Count);
        for (int i = 0; i < count; i++)
            await CardPileCmd.Shuffle(choiceContext, Owner);
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
