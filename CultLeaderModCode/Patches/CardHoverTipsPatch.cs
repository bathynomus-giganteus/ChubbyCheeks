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
        [typeof(Apostle_Melancholy_11)] = [typeof(VulnerablePower), typeof(WeakPower), typeof(PoisonPower), typeof(DoomPower), typeof(FrailPower)],
        [typeof(Apostle_Melancholy_17)] = [typeof(MoonFieldPower)],
        [typeof(Apostle_Melancholy_18)] = [typeof(OnAttackedGainBitterPainPower)],
        [typeof(Apostle_Melancholy_20)] = [typeof(SkyRulerPower), typeof(DebilitatePower)],
        [typeof(Apostle_Melancholy_23)] = [typeof(VulnerablePower)],
        [typeof(Apostle_Melancholy_24)] = [typeof(WeakPower)],

        [typeof(Apostle_Pure_01)] = [typeof(HealingPower), typeof(LifeEssencePower)],
        [typeof(Apostle_Pure_03)] = [typeof(AbilityDamageTakenBonusPower)],
        [typeof(Apostle_Pure_26)] = [typeof(PirateMarkPower)],
        [typeof(Apostle_Pure_08)] = [typeof(WeakPower)],
        [typeof(Apostle_Pure_13)] = [typeof(SapLauncherPower)],
        [typeof(Apostle_Pure_15)] = [typeof(SelfStunPower)],
        [typeof(Apostle_Pure_17)] = [typeof(RemoteChargePower)],
        [typeof(Apostle_Pure_18)] = [typeof(BufferPower)],
        [typeof(Apostle_Pure_19)] = [typeof(HackMarkPower)],
        [typeof(Apostle_Pure_22)] = [typeof(TempMaxHpLossPower), typeof(TempMaxHpPower)],
        [typeof(Apostle_Pure_23)] = [typeof(TempStrengthLossPower)],

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
        [typeof(TestRainbowCard)] = "治愈  覆甲  活力  保留  苦痛施予",
        [typeof(Apostle_Lively_08_1)] = "治愈  覆甲  活力  苦痛施予",
    };

    private static readonly (CardTag Tag, string Name)[] PersonalityTagNames =
    [
        (CultLeaderCardTags.Pure, "纯粹"),
        (CultLeaderCardTags.Calm, "冷静"),
        (CultLeaderCardTags.Frenzy, "狂热"),
        (CultLeaderCardTags.Lively, "活泼"),
        (CultLeaderCardTags.Melancholy, "忧郁"),
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

    private static readonly Dictionary<string, string> ApostleNamesByCardTypeName = new()
    {
        [nameof(TestRainbowCard)] = "乌洛斯",
        [nameof(Apostle_Melancholy_01)] = "科米",
        [nameof(Apostle_Melancholy_02)] = "x锡安x",
        [nameof(Apostle_Melancholy_02_1)] = "x锡安x",
        [nameof(Apostle_Melancholy_02_2)] = "x锡安x",
        [nameof(Apostle_Melancholy_03)] = "珀榭",
        [nameof(Apostle_Melancholy_04)] = "基迪恩",
        [nameof(Apostle_Melancholy_05)] = "琳",
        [nameof(Apostle_Melancholy_06)] = "艾舒尔",
        [nameof(Apostle_Melancholy_07)] = "希尔德",
        [nameof(Apostle_Melancholy_08)] = "莉斯缇",
        [nameof(Apostle_Melancholy_09)] = "阿萨娜",
        [nameof(Apostle_Melancholy_10)] = "洛涅（市长）",
        [nameof(Apostle_Melancholy_11)] = "欧尔",
        [nameof(Apostle_Melancholy_12)] = "莎莎",
        [nameof(Apostle_Melancholy_13)] = "里昂",
        [nameof(Apostle_Melancholy_14)] = "斯诺基",
        [nameof(Apostle_Melancholy_15)] = "琼安",
        [nameof(Apostle_Melancholy_16)] = "布蓝琪",
        [nameof(Apostle_Melancholy_17)] = "优米",
        [nameof(Apostle_Melancholy_18)] = "阿梅利亚（R41）",
        [nameof(Apostle_Melancholy_19)] = "绮莎",
        [nameof(Apostle_Melancholy_20)] = "希菲尔",
        [nameof(Apostle_Melancholy_21)] = "巴丽叶",
        [nameof(Apostle_Melancholy_22)] = "莱薇",
        [nameof(Apostle_Melancholy_23)] = "菲斯塔",
        [nameof(Apostle_Melancholy_24)] = "贝鲁",
        [nameof(Apostle_Melancholy_25)] = "乔菲",
        [nameof(Apostle_Lively_01)] = "雨伊",
        [nameof(Apostle_Lively_02)] = "鲁德",
        [nameof(Apostle_Lively_03)] = "卢波",
        [nameof(Apostle_Lively_04)] = "康娜",
        [nameof(Apostle_Lively_05)] = "黄油",
        [nameof(Apostle_Lively_05_1)] = "黄油",
        [nameof(Apostle_Lively_06)] = "提格",
        [nameof(Apostle_Lively_07)] = "赛琳娜",
        [nameof(Apostle_Lively_08)] = "埃皮卡",
        [nameof(Apostle_Lively_08_1)] = "埃皮卡",
        [nameof(Apostle_Lively_08_2)] = "埃皮卡",
        [nameof(Apostle_Lively_08_3)] = "埃皮卡",
        [nameof(Apostle_Lively_09)] = "米洛",
        [nameof(Apostle_Lively_10)] = "玛卡莎",
        [nameof(Apostle_Lively_11)] = "阿尔柯",
        [nameof(Apostle_Lively_12)] = "贝拉",
        [nameof(Apostle_Lively_13)] = "修罗",
        [nameof(Apostle_Lively_14)] = "斯碧琪（女仆）",
        [nameof(Apostle_Lively_15)] = "莫莫",
        [nameof(Apostle_Lively_16)] = "舒胖",
        [nameof(Apostle_Lively_17)] = "谢迪（逆转）",
        [nameof(Apostle_Lively_18)] = "涅尔（愤怒）",
        [nameof(Apostle_Lively_19)] = "莱薇（毕业）",
        [nameof(Apostle_Lively_20)] = "艾舒尔（魔道）",
        [nameof(Apostle_Lively_21)] = "芭娜",
        [nameof(Apostle_Lively_22)] = "茱比",
        [nameof(Apostle_Lively_23)] = "班尼",
        [nameof(Apostle_Lively_24)] = "玛丽",
        [nameof(Apostle_Lively_25)] = "卡伦",
        [nameof(Apostle_Lively_26)] = "泰达",
        [nameof(Apostle_Lively_27)] = "米雪",
        [nameof(Apostle_Frenzy_01)] = "克萝伊",
        [nameof(Apostle_Frenzy_02)] = "黛安娜",
        [nameof(Apostle_Frenzy_03)] = "谢迪",
        [nameof(Apostle_Frenzy_04)] = "尼尔",
        [nameof(Apostle_Frenzy_05)] = "西斯特",
        [nameof(Apostle_Frenzy_06)] = "贝丽塔",
        [nameof(Apostle_Frenzy_07)] = "爱丽丝",
        [nameof(Apostle_Frenzy_08)] = "丽兹",
        [nameof(Apostle_Frenzy_09)] = "提格（英雄）",
        [nameof(Apostle_Frenzy_10)] = "阿妮特",
        [nameof(Apostle_Frenzy_11)] = "涅缇",
        [nameof(Apostle_Frenzy_12)] = "琳（混沌）",
        [nameof(Apostle_Frenzy_13)] = "破朗",
        [nameof(Apostle_Frenzy_14)] = "皮拉",
        [nameof(Apostle_Frenzy_15)] = "莉纽瓦",
        [nameof(Apostle_Frenzy_16)] = "罗莱特",
        [nameof(Apostle_Frenzy_17)] = "海蒂",
        [nameof(Apostle_Frenzy_18)] = "达雅（纯真闪耀）",
        [nameof(Apostle_Frenzy_19)] = "海莉（清醒）",
        [nameof(Apostle_Frenzy_20)] = "西尔维娅",
        [nameof(Apostle_Frenzy_21)] = "斯琪娅",
        [nameof(Apostle_Frenzy_22)] = "大师2号",
        [nameof(Apostle_Frenzy_23)] = "玛约",
        [nameof(Apostle_Frenzy_24)] = "伊芙利特",
        [nameof(Apostle_Frenzy_25)] = "梅森",
        [nameof(Apostle_Frenzy_26)] = "刘美美",
        [nameof(Apostle_Calm_01)] = "阿雅",
        [nameof(Apostle_Calm_02)] = "希拉",
        [nameof(Apostle_Calm_03)] = "埃蕾娜",
        [nameof(Apostle_Calm_04)] = "艾米莉娅",
        [nameof(Apostle_Calm_05)] = "梅露娜",
        [nameof(Apostle_Calm_06)] = "芙莉克尔",
        [nameof(Apostle_Calm_07)] = "杰德",
        [nameof(Apostle_Calm_08)] = "薇尔薇特",
        [nameof(Apostle_Calm_09)] = "柯米(泳装)",
        [nameof(Apostle_Calm_10)] = "皮可拉",
        [nameof(Apostle_Calm_11)] = "伊德",
        [nameof(Apostle_Calm_12)] = "巴隆",
        [nameof(Apostle_Calm_13)] = "格温",
        [nameof(Apostle_Calm_14)] = "艾西亚",
        [nameof(Apostle_Calm_15)] = "里科塔",
        [nameof(Apostle_Calm_16)] = "黛安娜（往昔）",
        [nameof(Apostle_Calm_17)] = "凯撒",
        [nameof(Apostle_Calm_18)] = "班尼（班尼）",
        [nameof(Apostle_Calm_19)] = "茵刻尔",
        [nameof(Apostle_Calm_20)] = "阿拉戈尼娅",
        [nameof(Apostle_Calm_21)] = "妮可",
        [nameof(Apostle_Calm_22)] = "埃斯皮",
        [nameof(Apostle_Calm_23)] = "蕾特",
        [nameof(Apostle_Calm_24)] = "帕特拉",
        [nameof(Apostle_Calm_25)] = "雷吉",
        [nameof(Apostle_Calm_26)] = "康塔",
        [nameof(Apostle_Pure_01)] = "埃尔芬（王道）",
        [nameof(Apostle_Pure_02)] = "薇薇安娜",
        [nameof(Apostle_Pure_03)] = "岚",
        [nameof(Apostle_Pure_04)] = "阿伊拉",
        [nameof(Apostle_Pure_05)] = "玛约(超帅)",
        [nameof(Apostle_Pure_06)] = "斯皮奇",
        [nameof(Apostle_Pure_07)] = "加维亚",
        [nameof(Apostle_Pure_08)] = "莎莉",
        [nameof(Apostle_Pure_09)] = "玛戈",
        [nameof(Apostle_Pure_10)] = "谢伦",
        [nameof(Apostle_Pure_11)] = "海莉",
        [nameof(Apostle_Pure_12)] = "奈亚",
        [nameof(Apostle_Pure_13)] = "卡罗特",
        [nameof(Apostle_Pure_14)] = "达雅",
        [nameof(Apostle_Pure_15)] = "埃尔芬",
        [nameof(Apostle_Pure_16)] = "欧珀",
        [nameof(Apostle_Pure_17)] = "莱卡",
        [nameof(Apostle_Pure_18)] = "凯西",
        [nameof(Apostle_Pure_19)] = "缪特",
        [nameof(Apostle_Pure_20)] = "黛莉娅",
        [nameof(Apostle_Pure_21)] = "伊德（康复）",
        [nameof(Apostle_Pure_22)] = "大木头",
        [nameof(Apostle_Pure_23)] = "洛涅",
        [nameof(Apostle_Pure_24)] = "阿莱特",
        [nameof(Apostle_Pure_25)] = "乔伊",
        [nameof(Apostle_Pure_26)] = "斯帕洛特",
    };

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
            var description = ApostleNamesByCardTypeName.TryGetValue(card.GetType().Name, out var apostleName)
                ? $"使徒名称：{apostleName}"
                : "使徒名称：未知";

            yield return new HoverTip(new LocString(KeywordLocTable, ApostleKeywordTitleKey), description);
        }

        var personalities = PersonalityTagNames
            .Where(entry => tags.Contains(entry.Tag))
            .Select(entry => entry.Name)
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
        if (CompactStatusTipsByCard.TryGetValue(card.GetType(), out var compactDescription)
            && emitted.Add($"compact-statuses:{card.GetType().FullName}"))
        {
            yield return new HoverTip(new LocString(GameplayUiLocTable, RelatedStatusesHoverTitleKey), compactDescription);
            yield break;
        }

        string description;
        try
        {
            description = card.Description.GetFormattedText();
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
        if (description.Contains("治愈", StringComparison.Ordinal))
            yield return typeof(HealingPower);

        if (description.Contains("海盗印记", StringComparison.Ordinal))
            yield return typeof(PirateMarkPower);

        if (description.Contains("覆甲", StringComparison.Ordinal))
            yield return typeof(PlatingPower);

        if (description.Contains("活力", StringComparison.Ordinal))
            yield return typeof(VigorPower);

        if (description.Contains("保留", StringComparison.Ordinal))
            yield return typeof(RetainPower);

        if (description.Contains("苦痛施予", StringComparison.Ordinal))
            yield return typeof(BitterPainPower);

        if (description.Contains("计划妥当", StringComparison.Ordinal))
            yield return typeof(RetainHandPower);
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
