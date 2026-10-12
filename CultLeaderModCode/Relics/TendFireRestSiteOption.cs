using CultLeaderMod.CultLeaderModCode.Relics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace CultLeaderMod.CultLeaderModCode.Relics;

public sealed class TendFireRestSiteOption : RestSiteOption
{
    public const string AssetPath = "res://CultLeaderMod/images/relics/fangqi.png";
    public override string OptionId => "TEND_FIRE";
    public override IEnumerable<string> AssetPaths => [AssetPath];
    public override bool IsEnabled => RepresentativeChanceRelic.GetFor(Owner) is { RemainingUses: < RepresentativeChanceRelic.MaxUses };

    public TendFireRestSiteOption(Player owner) : base(owner) { }

    public override Task<bool> OnSelect()
    {
        var relic = RepresentativeChanceRelic.GetFor(Owner);
        if (relic == null || relic.RemainingUses >= RepresentativeChanceRelic.MaxUses)
            return Task.FromResult(false);
        relic.Refill();
        return Task.FromResult(true);
    }
}

[HarmonyPatch(typeof(RestSiteOption), "Icon", MethodType.Getter)]
public static class TendFireRestSiteIconPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RestSiteOption __instance, ref Texture2D __result)
    {
        if (__instance is TendFireRestSiteOption)
        {
            __result = PreloadManager.Cache.GetTexture2D(TendFireRestSiteOption.AssetPath);
            return false;
        }
        return true;
    }
}
