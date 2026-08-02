using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>Mapping and conversion between base buffs and elder buffs.</summary>
public static class ElderFormHelper
{
    private static readonly Dictionary<Type, Type> BaseToElder = new()
    {
        [typeof(RegenPower)] = typeof(LifeEssencePower),
        [typeof(PlatingPower)] = typeof(FrozenFortitudePower),
        [typeof(VigorPower)] = typeof(FanaticismPower),
        [typeof(BitterPainPower)] = typeof(BitterPainBurstPower),
        [typeof(ArtifactPower)] = typeof(HappinessPower),
    };

    private static readonly Dictionary<Type, Type> ElderToBase = BaseToElder
        .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    public static bool IsBaseBuff(PowerModel power) => BaseToElder.ContainsKey(power.GetType());
    public static bool IsElderBuff(PowerModel power) => ElderToBase.ContainsKey(power.GetType());

    public static Type? GetElderType(Type baseType) =>
        BaseToElder.TryGetValue(baseType, out var elder) ? elder : null;

    public static Type? GetBaseType(Type elderType) =>
        ElderToBase.TryGetValue(elderType, out var bas) ? bas : null;

    private static readonly MethodInfo PowerCmdApplyTyped = typeof(PowerCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == "Apply" && m.IsGenericMethodDefinition
            && m.GetParameters().Length == 6
            && m.GetParameters()[1].ParameterType == typeof(Creature));

    /// <summary>
    /// Convert all base buffs on the creature to their elder versions.
    /// Removes base buffs and grants equal amounts of elder buffs.
    /// </summary>
    public static async Task ConvertBaseBuffsToElder(PlayerChoiceContext ctx, Creature creature)
    {
        var toConvert = creature.Powers
            .Where(p => IsBaseBuff(p))
            .ToList();

        foreach (var power in toConvert)
        {
            int stacks = (int)power.Amount;
            if (stacks <= 0) continue;

            var elderType = GetElderType(power.GetType());
            if (elderType == null) continue;

            var typedApply = PowerCmdApplyTyped.MakeGenericMethod(elderType);
            await (Task)typedApply.Invoke(null, [ctx, creature, (decimal)stacks, creature, null!, false])!;
            await PowerCmd.Remove(power);
        }
    }
}