using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using CultLeaderMod.CultLeaderModCode.Powers;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Patches;

/// <summary>
/// Harmony patch that adds +1 Block gained per stack of FrozenFortitudePower.
/// Intercepts CreatureCmd.GainBlock and increases the amount by the power's stack count.
/// </summary>
[HarmonyPatch]
public static class FrozenFortitudeBlockPatch
{
    private static MethodBase TargetMethod() =>
        typeof(CreatureCmd).GetMethod(
            "GainBlock",
            [typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(CardPlay), typeof(bool)]
        )!;

    /// <summary>
    /// Prefix: runs before GainBlock, adds FrozenFortitude stacks to the block amount.
    /// </summary>
    private static void Prefix(Creature creature, ref decimal amount)
    {
        var ff = creature.GetPower<FrozenFortitudePower>();
        if (ff != null && ff.Amount > 0)
            amount += ff.Amount;
    }
}


