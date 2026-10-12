using System.Reflection;
using System.Reflection.Emit;
using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(NRelicCollectionCategory), nameof(NRelicCollectionCategory.LoadRelics))]
public static class RelicCollectionStarterPatch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AddRepresentativeChanceToStarterRelics(
        IEnumerable<CodeInstruction> instructions)
    {
        var appendMethod = AccessTools.Method(typeof(RelicCollectionStarterPatch), nameof(AppendSpecialStarterRelics));
        var patched = false;

        foreach (var instruction in instructions)
        {
            yield return instruction;
            if (patched || (instruction.opcode != OpCodes.Call && instruction.opcode != OpCodes.Callvirt) ||
                instruction.operand is not MethodInfo method || method.Name != nameof(Enumerable.ToList) ||
                method.ReturnType != typeof(List<RelicModel>))
                continue;

            yield return new CodeInstruction(OpCodes.Call, appendMethod);
            patched = true;
        }
    }

    private static List<RelicModel> AppendSpecialStarterRelics(List<RelicModel> relics)
    {
        var representativeChance = ModelDb.Relic<RepresentativeChanceRelic>();
        if (!relics.Contains(representativeChance))
            relics.Add(representativeChance);
        return relics;
    }
}
