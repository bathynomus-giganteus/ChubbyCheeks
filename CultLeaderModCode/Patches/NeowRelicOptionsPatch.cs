using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class NeowRelicOptionsPatch
{
    [HarmonyPatch(typeof(Neow), "PositiveOptions", MethodType.Getter)]
    [HarmonyPostfix]
    private static void Postfix(Neow __instance, ref IEnumerable<EventOption> __result)
    {
        var player = __instance.Owner;
        if (player == null || player.Character is not CultLeaderModCharacter)
            return;

        var options = __result.ToList();
        options.Add(CreateRelicOption<SingleApostleTicketRelic>(__instance, "CULT_LEADER_NEOW_SINGLE_APOSTLE_TICKET"));
        options.Add(CreateRelicOption<SingleWeaponTicketRelic>(__instance, "CULT_LEADER_NEOW_SINGLE_WEAPON_TICKET"));
        options.Add(CreateRelicOption<GoldenCrayonRelic>(__instance, "CULT_LEADER_NEOW_GOLDEN_CRAYON"));
        __result = options;
    }

    private static EventOption CreateRelicOption<T>(Neow neow, string textKey)
        where T : RelicModel
    {
        var relic = ModelDb.Relic<T>().ToMutable();

        return EventOption.FromRelic(
            relic,
            neow,
            async () =>
            {
                var player = neow.Owner;
                if (player == null)
                    return;

                await RelicCmd.Obtain(relic, player);
                neow.StartPreFinished();
            },
            textKey).WithRelic(relic);
    }
}
