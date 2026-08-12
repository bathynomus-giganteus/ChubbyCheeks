using System.Threading;
using CultLeaderMod.CultLeaderModCode.CardTags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CultLeaderMod.CultLeaderModCode.Powers;

internal static class ApostlePowerRules
{
    private static readonly AsyncLocal<int> ConversionDepth = new();

    private static readonly HashSet<Type> AuthorityScaledPowerTypes =
    [
        typeof(RegenPower),
        typeof(PlatingPower),
        typeof(VigorPower),
        typeof(RetainHandPower),
        typeof(BitterPainPower),
        typeof(LifeEssencePower),
        typeof(SolidIcePower),
        typeof(FervorPower),
        typeof(BitterPainBurstPower),
        typeof(HappinessPower),
        typeof(RegenPerTurnPower),
        typeof(PlatingPerTurnPower),
        typeof(VigorPerTurnPower),
        typeof(BitterPainPerTurnPower),
        typeof(ArtifactPerTurnPower),
    ];

    private static readonly Dictionary<Type, Type> ElderUpgrades = new()
    {
        [typeof(RegenPower)] = typeof(LifeEssencePower),
        [typeof(PlatingPower)] = typeof(SolidIcePower),
        [typeof(VigorPower)] = typeof(FervorPower),
        [typeof(BitterPainPower)] = typeof(BitterPainBurstPower),
        [typeof(RetainHandPower)] = typeof(HappinessPower),
    };

    public static bool IsConverting => ConversionDepth.Value > 0;

    public static bool IsApostleCard(CardModel? card)
    {
        return card?.Tags.Contains(CultLeaderCardTags.Apostle) == true;
    }

    public static bool IsAuthorityScaledPower(PowerModel power)
    {
        return AuthorityScaledPowerTypes.Contains(power.GetType());
    }

    public static bool TryGetElderUpgrade(PowerModel power, out Type upgradedPowerType)
    {
        return TryGetElderUpgrade(power.GetType(), out upgradedPowerType);
    }

    public static bool TryGetElderUpgrade(Type powerType, out Type upgradedPowerType)
    {
        return ElderUpgrades.TryGetValue(powerType, out upgradedPowerType!);
    }

    public static async Task ApplyApostlePower<TBasePower, TUpgradedPower>(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
        where TBasePower : PowerModel, new()
        where TUpgradedPower : PowerModel, new()
    {
        if (HasElderForm(target))
            await PowerCmd.Apply<TUpgradedPower>(choiceContext, target, amount, applier, cardSource, silent);
        else
            await PowerCmd.Apply<TBasePower>(choiceContext, target, amount, applier, cardSource, silent);
    }

    public static bool HasElderForm(Creature? target)
    {
        return target?.Powers?.OfType<ElderFormPower>().Any() == true;
    }

    public static async Task ConvertExistingBasePowersToElderUpgrades(
        PlayerChoiceContext choiceContext,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        if (target.Powers == null)
            return;

        var existingBasePowers = new List<(PowerModel BasePower, decimal Amount, Type UpgradeType)>();
        foreach (var power in target.Powers)
        {
            if (power.Amount <= 0 || !TryGetElderUpgrade(power.GetType(), out var upgradeType))
                continue;

            existingBasePowers.Add((power, power.Amount, upgradeType));
        }

        if (existingBasePowers.Count == 0)
            return;

        using var conversionScope = BeginConversion();
        foreach (var (basePower, amount, upgradeType) in existingBasePowers)
        {
            await ApplyKnownPowerByType(choiceContext, target, upgradeType, amount, applier, cardSource);
            await PowerCmd.ModifyAmount(choiceContext, basePower, -amount, applier, cardSource, silent: true);
        }
    }

    private static async Task ApplyKnownPowerByType(
        PlayerChoiceContext choiceContext,
        Creature target,
        Type powerType,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (powerType == typeof(LifeEssencePower))
            await PowerCmd.Apply<LifeEssencePower>(choiceContext, target, amount, applier, cardSource, silent: true);
        else if (powerType == typeof(SolidIcePower))
            await PowerCmd.Apply<SolidIcePower>(choiceContext, target, amount, applier, cardSource, silent: true);
        else if (powerType == typeof(FervorPower))
            await PowerCmd.Apply<FervorPower>(choiceContext, target, amount, applier, cardSource, silent: true);
        else if (powerType == typeof(BitterPainBurstPower))
            await PowerCmd.Apply<BitterPainBurstPower>(choiceContext, target, amount, applier, cardSource, silent: true);
        else if (powerType == typeof(HappinessPower))
            await PowerCmd.Apply<HappinessPower>(choiceContext, target, amount, applier, cardSource, silent: true);
        else
            Entry.Logger.Warn($"[ApostlePowerRules] Unknown Elder upgrade power: {powerType.FullName}");
    }

    private static IDisposable BeginConversion()
    {
        ConversionDepth.Value++;
        return new ConversionScope();
    }

    private sealed class ConversionScope : IDisposable
    {
        public void Dispose()
        {
            ConversionDepth.Value = Math.Max(0, ConversionDepth.Value - 1);
        }
    }
}
