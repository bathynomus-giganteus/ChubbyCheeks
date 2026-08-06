using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using STS2RitsuLib.CardTags;

namespace CultLeaderMod.CultLeaderModCode.CardTags;

public static class CultLeaderCardTags
{
    public static CardTag Apostle { get; private set; }
    public static CardTag Pure { get; private set; }
    public static CardTag Calm { get; private set; }
    public static CardTag Frenzy { get; private set; }
    public static CardTag Lively { get; private set; }
    public static CardTag Melancholy { get; private set; }

    private static bool _registered;

    public static void RegisterAll(string modId, Logger logger)
    {
        if (_registered) return;
        var registry = ModCardTagRegistry.For(modId);

        Apostle  = registry.RegisterOwned("Apostle").CardTagValue;
        Pure     = registry.RegisterOwned("Pure").CardTagValue;
        Calm     = registry.RegisterOwned("Calm").CardTagValue;
        Frenzy   = registry.RegisterOwned("Frenzy").CardTagValue;
        Lively   = registry.RegisterOwned("Lively").CardTagValue;
        Melancholy = registry.RegisterOwned("Melancholy").CardTagValue;

        _registered = true;
        logger.Info("[CultLeaderCardTags] 6 CardTags registered: Apostle, Pure, Calm, Frenzy, Lively, Melancholy");
    }
}
