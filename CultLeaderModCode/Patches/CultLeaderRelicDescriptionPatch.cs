using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.DynamicDescription), MethodType.Getter)]
public static class CultLeaderRelicDescriptionPatch
{
    private const string RelicLocPrefix = "CULTLEADERMOD-CULT_LEADER_STARTING_RELIC";

    private static void Postfix(RelicModel __instance, ref LocString __result)
    {
        if (__instance is not CultLeaderStartingRelic relic)
            return;

        ApostlePersonality[] selected = Enum.GetValues<ApostlePersonality>()
            .Where(relic.IsPersonalitySelected)
            .Take(2)
            .ToArray();

        if (selected.Length < 2)
        {
            __result = new LocString("relics", $"{RelicLocPrefix}.unselectedDescription");
            return;
        }

        __result.Add("PersonalityOne", PersonalityName(selected[0]));
        __result.Add("PersonalityTwo", PersonalityName(selected[1]));
    }

    private static LocString PersonalityName(ApostlePersonality personality) =>
        new("relics", $"{RelicLocPrefix}.{personality.ToString().ToLowerInvariant()}Personality");
}
