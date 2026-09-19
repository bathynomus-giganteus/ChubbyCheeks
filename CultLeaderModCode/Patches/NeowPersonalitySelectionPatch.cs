using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Events;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class NeowPersonalitySelectionPatch
{
    private const string PredeterminedOptionKey = "CULT_LEADER_FATE_PREDETERMINED";
    private const string RandomOptionKey = "CULT_LEADER_FATE_RANDOM";
    private const string ChaosRarityOptionKey = "CULT_LEADER_FATE_CHAOS_RARITY";

    [HarmonyPatch(typeof(Neow), "GenerateInitialOptions")]
    [HarmonyPostfix]
    private static void Postfix(Neow __instance, ref IReadOnlyList<EventOption> __result)
    {
        var player = __instance.Owner;
        if (player == null || !GumBlessRelic.ShouldOfferOpeningSelection(player))
        {
            return;
        }

        var originalNeowOptions = __result.ToList();
        Entry.Logger.Info("[NeowPersonalitySelectionPatch] Replacing initial Neow options with three fate choices.");
        __result = CreateFateOptions(__instance, originalNeowOptions);
    }

    private static IReadOnlyList<EventOption> CreateFateOptions(
        Neow neow,
        IReadOnlyList<EventOption> originalNeowOptions)
    {
        return
        [
            CreatePredeterminedFateOption(neow, originalNeowOptions),
            CreateRandomFateOption(neow, originalNeowOptions),
            CreateChaosRarityFateOption(neow, originalNeowOptions)
        ];
    }

    private static EventOption CreatePredeterminedFateOption(
        Neow neow,
        IReadOnlyList<EventOption> originalNeowOptions)
    {
        return new EventOption(
            neow,
            async () =>
            {
                var player = neow.Owner;
                if (player == null)
                {
                    Entry.Logger.Error("[NeowPersonalitySelectionPatch] Neow owner was null while opening personality selection.");
                    return;
                }

                var completed = await GumBlessRelic.TriggerOpeningSelection(player);
                RefreshNeowOptions(
                    neow,
                    completed ? originalNeowOptions : CreateFateOptions(neow, originalNeowOptions));
            },
            new LocString("gameplay_ui", $"{PredeterminedOptionKey}.title"),
            new LocString("gameplay_ui", $"{PredeterminedOptionKey}.description"),
            PredeterminedOptionKey,
            Array.Empty<IHoverTip>());
    }

    private static EventOption CreateRandomFateOption(
        Neow neow,
        IReadOnlyList<EventOption> originalNeowOptions)
    {
        return new EventOption(
            neow,
            () =>
            {
                var player = neow.Owner;
                if (player == null)
                {
                    Entry.Logger.Error("[NeowPersonalitySelectionPatch] Neow owner was null while choosing random fate.");
                    return Task.CompletedTask;
                }

                GumBlessRelic.SelectRandomFate(player);
                RefreshNeowOptions(neow, originalNeowOptions);
                return Task.CompletedTask;
            },
            new LocString("gameplay_ui", $"{RandomOptionKey}.title"),
            new LocString("gameplay_ui", $"{RandomOptionKey}.description"),
            RandomOptionKey,
            Array.Empty<IHoverTip>());
    }

    private static EventOption CreateChaosRarityFateOption(
        Neow neow,
        IReadOnlyList<EventOption> originalNeowOptions)
    {
        return new EventOption(
            neow,
            () =>
            {
                var player = neow.Owner;
                if (player == null)
                {
                    Entry.Logger.Error("[NeowPersonalitySelectionPatch] Neow owner was null while choosing chaos rarity fate.");
                    return Task.CompletedTask;
                }

                GumBlessRelic.SelectChaosRarityFate(player);
                RefreshNeowOptions(neow, originalNeowOptions);
                return Task.CompletedTask;
            },
            new LocString("gameplay_ui", $"{ChaosRarityOptionKey}.title"),
            new LocString("gameplay_ui", $"{ChaosRarityOptionKey}.description"),
            ChaosRarityOptionKey,
            Array.Empty<IHoverTip>());
    }

    private static void RefreshNeowOptions(Neow neow, IEnumerable<EventOption> options)
    {
        var setEventState = FindSetEventStateMethod();

        if (setEventState == null)
        {
            Entry.Logger.Error("[NeowPersonalitySelectionPatch] Could not find EventModel.SetEventState.");
            return;
        }

        setEventState.Invoke(neow, new object[] { neow.InitialDescription, options });
    }

    private static System.Reflection.MethodInfo? FindSetEventStateMethod()
    {
        for (var type = typeof(Neow); type != null; type = type.BaseType)
        {
            var method = AccessTools.DeclaredMethod(
                type,
                "SetEventState",
                new[] { typeof(LocString), typeof(IEnumerable<EventOption>) });

            if (method != null)
            {
                return method;
            }
        }

        return null;
    }
}
