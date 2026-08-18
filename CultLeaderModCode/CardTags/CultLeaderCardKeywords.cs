using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace CultLeaderMod.CultLeaderModCode.CardTags;

[RegisterOwnedCardKeyword(nameof(Apostle), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(Pure), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(Calm), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(Frenzy), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(Lively), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(Melancholy), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(PatHead), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription, IncludeInCardHoverTip = false)]
public class CultLeaderCardKeywords
{
    public static readonly CardKeyword Apostle = ModContentRegistry.GetQualifiedKeywordId("CultLeaderMod", nameof(Apostle)).GetModCardKeyword();
    public static readonly CardKeyword Pure = ModContentRegistry.GetQualifiedKeywordId("CultLeaderMod", nameof(Pure)).GetModCardKeyword();
    public static readonly CardKeyword Calm = ModContentRegistry.GetQualifiedKeywordId("CultLeaderMod", nameof(Calm)).GetModCardKeyword();
    public static readonly CardKeyword Frenzy = ModContentRegistry.GetQualifiedKeywordId("CultLeaderMod", nameof(Frenzy)).GetModCardKeyword();
    public static readonly CardKeyword Lively = ModContentRegistry.GetQualifiedKeywordId("CultLeaderMod", nameof(Lively)).GetModCardKeyword();
    public static readonly CardKeyword Melancholy = ModContentRegistry.GetQualifiedKeywordId("CultLeaderMod", nameof(Melancholy)).GetModCardKeyword();
    public static readonly CardKeyword PatHead = ModContentRegistry.GetQualifiedKeywordId("CultLeaderMod", nameof(PatHead)).GetModCardKeyword();
}
