using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CultLeaderMod.CultLeaderModCode.Patches;

/// <summary>
/// Adds card hover tips for Cult Leader-specific terms, powers, and generated cards.
/// Vanilla/RitsuLib already shows normal card keywords; this patch covers the parts
/// that appear only as card tags, custom Power text, or derived-card references.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public static class CardHoverTipsPatch
{
    private const string CultLeaderCardNamespace = "CultLeaderMod.CultLeaderModCode.Cards";
    private const string KeywordLocTable = "card_keywords";
    private const string ApostleKeywordTitleKey = "CULT_LEADER_MOD_KEYWORD_APOSTLE.title";
    private const string PersonalityKeywordTitleKey = "CULT_LEADER_MOD_KEYWORD_PURE.title";
    private const string GameplayUiLocTable = "gameplay_ui";
    private const string ApostleNameKeyPrefix = "CULT_LEADER_APOSTLE_NAME.";
    private const string RelatedStatusesHoverTitleKey = "CULT_LEADER_HOVER_RELATED_STATUSES.title";

    private static readonly Dictionary<Type, Type[]> PowerTipsByCard = new()
    {
        [typeof(Apostle_Calm_03)] = [typeof(OnAttackedGainPlatingPower)],
        [typeof(Apostle_Calm_04)] = [typeof(OutOfTurnPlatingDrawPower)],
        [typeof(Apostle_Calm_06)] = [typeof(PlatingReboundDamagePower)],
        [typeof(Apostle_Calm_09)] = [typeof(PoolCandidatePower)],
        [typeof(Apostle_Calm_12)] = [typeof(WeakPower)],
        [typeof(Apostle_Calm_13)] = [typeof(FlatDamageReductionPower)],
        [typeof(Apostle_Calm_18)] = [typeof(TempStrengthLossPower)],
        [typeof(Apostle_Calm_19)] = [typeof(InkWashPower)],
        [typeof(Apostle_Calm_21)] = [typeof(StrengthPower), typeof(TempMaxHpLossPower), typeof(TempMaxHpPower)],

        [typeof(Apostle_Frenzy_01)] = [typeof(SebastianPower)],
        [typeof(Apostle_Frenzy_06)] = [typeof(VigorPower), typeof(DimensionPositionPower)],
        [typeof(Apostle_Frenzy_19)] = [typeof(ForwardResolvePower)],
        [typeof(Apostle_Frenzy_20)] = [typeof(FrenzyOnHealPower)],
        [typeof(Apostle_Frenzy_21)] = [typeof(PlatingPower), typeof(VigorPerTurnPower)],
        [typeof(Apostle_Frenzy_24)] = [typeof(BonfireVigorPower)],

        [typeof(Apostle_Lively_01)] = [typeof(FrogRainPower)],
        [typeof(Apostle_Lively_07)] = [typeof(LoveEnergyPower)],
        [typeof(Apostle_Lively_08_1)] = [typeof(EpiconAssistantPower)],
        [typeof(Apostle_Lively_08_3)] = [typeof(DexterityPower), typeof(StrengthPower)],
        [typeof(Apostle_Lively_09)] = [typeof(ReflectNextDamagePower)],
        [typeof(Apostle_Lively_10)] = [typeof(FateClockPower)],
        [typeof(Apostle_Lively_12)] = [typeof(ExtantPower)],
        [typeof(Apostle_Lively_13)] = [typeof(AdjustPower)],
        [typeof(Apostle_Lively_19)] = [typeof(RookieCardPower)],
        [typeof(Apostle_Lively_21)] = [typeof(WalnutMasterPower)],
        [typeof(Apostle_Lively_22)] = [typeof(BeePower)],
        [typeof(Apostle_Lively_24)] = [typeof(BombComingPower)],

        [typeof(Apostle_Melancholy_02)] = [typeof(MagicBulletPower)],
        [typeof(Apostle_Melancholy_10)] = [typeof(DebuffApplyCounterPower)],
        [typeof(Apostle_Melancholy_17)] = [typeof(MoonFieldPower)],
        [typeof(Apostle_Melancholy_18)] = [typeof(OnAttackedGainBitterPainPower)],
        [typeof(Apostle_Melancholy_20)] = [typeof(SkyRulerPower), typeof(DebilitatePower)],
        [typeof(Apostle_Melancholy_23)] = [typeof(VulnerablePower)],
        [typeof(Apostle_Melancholy_24)] = [typeof(WeakPower)],

        [typeof(Apostle_Pure_01)] = [typeof(HealingPower), typeof(LifeEssencePower)],
        [typeof(Apostle_Pure_03)] = [typeof(AbilityDamageTakenBonusPower)],
        [typeof(Apostle_Pure_26)] = [typeof(PirateMarkPower)],
        [typeof(Apostle_Melancholy_26)] = [typeof(PrecisionLightSwarmPower)],
        [typeof(Apostle_Pure_08)] = [typeof(WeakPower)],
        [typeof(Apostle_Pure_13)] = [typeof(SapLauncherPower)],
        [typeof(Apostle_Pure_15)] = [typeof(SelfStunPower)],
        [typeof(Apostle_Pure_17)] = [typeof(RemoteChargePower)],
        [typeof(Apostle_Pure_18)] = [typeof(BufferPower)],
        [typeof(Apostle_Pure_19)] = [typeof(HackMarkPower)],
        [typeof(Apostle_Pure_22)] = [typeof(TempMaxHpLossPower), typeof(TempMaxHpPower)],
        [typeof(Apostle_Pure_23)] = [typeof(TempStrengthLossPower)],
        [typeof(Apostle_Resonance_01)] = [typeof(LittleWishBecomesLightPower), typeof(HolyPower)],
        [typeof(Apostle_Calm_27)] = [typeof(PlatingPower), typeof(RetainPower), typeof(HealingPower)],

        [typeof(CultLeaderManifestationCard)] = [typeof(CultLeaderAuthorityPower)],
        [typeof(DualRivalsCard)] = [typeof(CultLeaderAuthorityPower)],
        [typeof(ElderFormCard)] = [typeof(ElderFormPower)],
        [typeof(ForElruienCard)] = [typeof(CultLeaderAuthorityPower)],
        [typeof(HundredDaysBlessingCard)] = [typeof(CultLeaderAuthorityPower)],
        [typeof(PersonalitySelectCalmCard)] = [typeof(PersonalityCardFetchCalmPower)],
        [typeof(PersonalitySelectFrenzyCard)] = [typeof(PersonalityCardFetchFrenzyPower)],
        [typeof(PersonalitySelectLivelyCard)] = [typeof(PersonalityCardFetchLivelyPower)],
        [typeof(PersonalitySelectMelancholyCard)] = [typeof(PersonalityCardFetchMelancholyPower)],
        [typeof(PersonalitySelectPureCard)] = [typeof(PersonalityCardFetchPurePower)],
        [typeof(SaviorDescendsCard)] = [typeof(CultLeaderAuthorityPower)],
        [typeof(TestRainbowCard)] = [typeof(CultLeaderMod.CultLeaderModCode.Powers.LoopPower)],
    };

    private static readonly Dictionary<Type, Type[]> CardTipsByCard = new()
    {
        [typeof(Apostle_Lively_05)] = [typeof(Apostle_Lively_05_1)],
        [typeof(Apostle_Lively_08)] = [typeof(Apostle_Lively_08_1), typeof(Apostle_Lively_08_2), typeof(Apostle_Lively_08_3)],
        [typeof(Apostle_Melancholy_02)] = [typeof(Apostle_Melancholy_02_1), typeof(Apostle_Melancholy_02_2)],
        [typeof(Apostle_Melancholy_02_1)] = [typeof(Apostle_Melancholy_02_2)],
    };

    private static readonly Dictionary<Type, string> CompactStatusTipsByCard = new()
    {
        [typeof(TestRainbowCard)] = "CULT_LEADER_HOVER_RELATED_STATUSES.test_rainbow",
        [typeof(Apostle_Lively_08_1)] = "CULT_LEADER_HOVER_RELATED_STATUSES.epicon_assistant",
        [typeof(Apostle_Melancholy_11)] = "CULT_LEADER_HOVER_RELATED_STATUSES.orr",
    };

    private static readonly (CardTag Tag, string DescriptionKey)[] PersonalityTagNames =
    [
        (CultLeaderCardTags.Pure, "CULT_LEADER_MOD_KEYWORD_PURE.description"),
        (CultLeaderCardTags.Calm, "CULT_LEADER_MOD_KEYWORD_CALM.description"),
        (CultLeaderCardTags.Frenzy, "CULT_LEADER_MOD_KEYWORD_FRENZY.description"),
        (CultLeaderCardTags.Lively, "CULT_LEADER_MOD_KEYWORD_LIVELY.description"),
        (CultLeaderCardTags.Melancholy, "CULT_LEADER_MOD_KEYWORD_MELANCHOLY.description"),
    ];

    private static readonly (string TermKey, Type PowerType)[] DescriptionPowerTerms =
    [
        ("healing", typeof(HealingPower)),
        ("pirate_mark", typeof(PirateMarkPower)),
        ("precision_light_swarm", typeof(PrecisionLightSwarmPower)),
        ("plating", typeof(PlatingPower)),
        ("vigor", typeof(VigorPower)),
        ("retain", typeof(RetainPower)),
        ("bitter_pain", typeof(BitterPainPower)),
        ("well_laid_plans", typeof(RetainHandPower)),
    ];

    private static readonly string[] ReplacedKeywordTipIds =
    [
        "CULT_LEADER_MOD_KEYWORD_APOSTLE",
        "CULT_LEADER_MOD_KEYWORD_PURE",
        "CULT_LEADER_MOD_KEYWORD_CALM",
        "CULT_LEADER_MOD_KEYWORD_FRENZY",
        "CULT_LEADER_MOD_KEYWORD_LIVELY",
        "CULT_LEADER_MOD_KEYWORD_MELANCHOLY",
    ];

    private static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.GetType().Namespace != CultLeaderCardNamespace)
            return;

        var baseTips = __result.Where(tip => !IsReplacedCultLeaderKeywordTip(tip)).ToList();
        var extraTips = BuildCultLeaderHoverTips(__instance).ToList();
        if (extraTips.Count == 0)
            return;

        __result = baseTips.Concat(extraTips);
    }

    private static IEnumerable<IHoverTip> BuildCultLeaderHoverTips(CardModel card)
    {
        var emitted = new HashSet<string>();

        foreach (var tagTip in BuildTagKeywordTips(card, emitted))
            yield return tagTip;

        foreach (var termTip in BuildDescriptionTermTips(card, emitted))
            yield return termTip;

        var cardType = card.GetType();
        if (PowerTipsByCard.TryGetValue(cardType, out var powerTypes))
        {
            foreach (var powerType in powerTypes)
            {
                if (!emitted.Add($"power:{powerType.FullName}"))
                    continue;

                var tip = TryCreatePowerTip(powerType);
                if (tip != null)
                    yield return tip;
            }
        }

        if (CardTipsByCard.TryGetValue(cardType, out var cardTypes))
        {
            foreach (var previewCardType in cardTypes)
            {
                if (!emitted.Add($"card:{previewCardType.FullName}"))
                    continue;

                var tip = TryCreateCardTip(previewCardType);
                if (tip != null)
                    yield return tip;
            }
        }
    }

    private static IEnumerable<IHoverTip> BuildTagKeywordTips(CardModel card, HashSet<string> emitted)
    {
        var tags = card.Tags.ToHashSet();
        if (tags.Contains(CultLeaderCardTags.Apostle) && emitted.Add("keyword:apostle"))
        {
            var apostleName = LocString.GetIfExists(GameplayUiLocTable, ApostleNameKeyPrefix + card.GetType().Name);
            var description = new LocString(
                GameplayUiLocTable,
                apostleName == null
                    ? "CULT_LEADER_HOVER_APOSTLE_NAME.unknown"
                    : "CULT_LEADER_HOVER_APOSTLE_NAME.description");
            if (apostleName != null)
                description.Add("Name", apostleName);

            yield return new HoverTip(
                new LocString(KeywordLocTable, ApostleKeywordTitleKey),
                description.GetFormattedText());
        }

        var hasAllPersonalities = PersonalityTagNames.All(entry => tags.Contains(entry.Tag));
        List<string> personalities = hasAllPersonalities
            ? [new LocString(GameplayUiLocTable, "CULT_LEADER_PERSONALITY_RESONANCE.name").GetFormattedText()]
            : PersonalityTagNames
                .Where(entry => tags.Contains(entry.Tag))
                .Select(entry => new LocString(KeywordLocTable, entry.DescriptionKey).GetFormattedText())
                .ToList();

        if (personalities.Count > 0 && emitted.Add("keyword:personality"))
        {
            yield return new HoverTip(
                new LocString(KeywordLocTable, PersonalityKeywordTitleKey),
                string.Join(" ", personalities));
        }
    }

    private static bool IsReplacedCultLeaderKeywordTip(IHoverTip tip)
    {
        return ReplacedKeywordTipIds.Any(id => tip.Id.Contains(id, StringComparison.Ordinal));
    }

    private static IHoverTip? TryCreatePowerTip(Type powerType)
    {
        try
        {
            var power = ModelDb.GetById<PowerModel>(ModelDb.GetId(powerType));
            return HoverTipFactory.FromPower(power);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"Failed to create power hover tip for {powerType.FullName}: {ex.Message}");
            return null;
        }
    }

    private static IEnumerable<IHoverTip> BuildDescriptionTermTips(CardModel card, HashSet<string> emitted)
    {
        if (CompactStatusTipsByCard.TryGetValue(card.GetType(), out var compactDescriptionKey)
            && emitted.Add($"compact-statuses:{card.GetType().FullName}"))
        {
            yield return new HoverTip(
                new LocString(GameplayUiLocTable, RelatedStatusesHoverTitleKey),
                new LocString(GameplayUiLocTable, compactDescriptionKey).GetFormattedText());
            yield break;
        }

        string description;
        try
        {
            description = card.Description.GetRawText();
        }
        catch
        {
            yield break;
        }

        foreach (var powerType in GetPowerTermsFromDescription(description))
        {
            if (!emitted.Add($"power:{powerType.FullName}"))
                continue;

            var tip = TryCreatePowerTip(powerType);
            if (tip != null)
                yield return tip;
        }
    }

    private static IEnumerable<Type> GetPowerTermsFromDescription(string description)
    {
        foreach (var (termKey, powerType) in DescriptionPowerTerms)
        {
            var term = new LocString(GameplayUiLocTable, $"CULT_LEADER_HOVER_TERM.{termKey}").GetFormattedText();
            if (!string.IsNullOrWhiteSpace(term)
                && description.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                yield return powerType;
            }
        }
    }

    private static IHoverTip? TryCreateCardTip(Type cardType)
    {
        try
        {
            var card = ModelDb.GetById<CardModel>(ModelDb.GetId(cardType));
            return HoverTipFactory.FromCard(card, false);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"Failed to create card hover tip for {cardType.FullName}: {ex.Message}");
            return null;
        }
    }
}
