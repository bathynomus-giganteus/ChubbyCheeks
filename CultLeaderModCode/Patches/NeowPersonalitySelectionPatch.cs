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
    private const string OptionKey = "CULT_LEADER_PERSONALITY_SELECTION";

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
        Entry.Logger.Info("[NeowPersonalitySelectionPatch] Replacing initial Neow options with opening personality selection.");

        __result = new[]
        {
            CreateOpeningSelectionOption(__instance, originalNeowOptions)
        };
    }

    private static EventOption CreateOpeningSelectionOption(Neow neow, IReadOnlyList<EventOption> originalNeowOptions)
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
                RefreshNeowOptions(neow, completed ? originalNeowOptions : new[] { CreateOpeningSelectionOption(neow, originalNeowOptions) });
            },
            new LocString("gameplay_ui", "CULT_LEADER_PERSONALITY_SELECTION.title"),
            new LocString("gameplay_ui", "CULT_LEADER_PERSONALITY_SELECTION.description"),
            OptionKey,
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
