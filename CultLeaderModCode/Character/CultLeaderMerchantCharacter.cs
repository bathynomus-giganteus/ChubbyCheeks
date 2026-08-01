using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace CultLeaderMod.CultLeaderModCode.Character;

/// <summary>
/// Static merchant-screen representation for the Cult Leader.
/// The base implementation assumes its first child is a Spine sprite, so this
/// subclass deliberately skips that Spine-only setup.
/// </summary>
public partial class CultLeaderMerchantCharacter : NMerchantCharacter
{
    public override void _Ready()
    {
    }
}

[HarmonyPatch(typeof(NMerchantCharacter), nameof(NMerchantCharacter.PlayAnimation))]
internal static class CultLeaderMerchantAnimationPatch
{
    private static bool Prefix(NMerchantCharacter __instance) =>
        __instance is not CultLeaderMerchantCharacter;
}
