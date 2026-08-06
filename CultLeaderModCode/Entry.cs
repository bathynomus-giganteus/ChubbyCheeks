using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Scaffolding.Content;
using CultLeaderMod.CultLeaderModCode.CardTags;

namespace CultLeaderMod.CultLeaderModCode;

[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "CultLeaderMod";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        CultLeaderCardTags.RegisterAll(ModId, Logger);


        try
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll(assembly);
            Logger.Info("Harmony patches applied");
        }
        catch (Exception ex)
        {
            Logger.Error($"Harmony patch failed: {ex}");
        }
    }
}
