using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;

namespace CultLeaderMod.CultLeaderModCode.Cards;

/// <summary>
/// Harmony patch on NCard.Reload to show/hide apostle portrait badge.
/// Only cards with the Apostle tag get a badge.
/// Non-apostle cards never show a badge.
/// </summary>
[HarmonyPatch(typeof(NCard), "Reload")]
public static class ApostleBadgePatch
{
    private static readonly Vector2 BadgeSize = new(56f, 56f);
    private const float RightMargin = 8f;
    private const float TopMargin = 6f;

    private const string FallbackBadgePath = "res://CultLeaderMod/images/badges/test_badge.png";
    private const string PortraitDir = "res://CultLeaderMod/images/badges/portraits/";
    private const string BadgeNodeName = "CultLeaderApostleBadge";

    private static readonly Dictionary<string, string> PortraitMap = new()
    {
        ["Apostle_Pure_01"] = "纯粹_埃尔芬（王道）",
        ["Apostle_Pure_02"] = "纯粹_薇薇安娜·阿尔根图姆",
        ["Apostle_Pure_03"] = "纯粹_岚",
        ["Apostle_Pure_04"] = "纯粹_阿伊拉",
        ["Apostle_Pure_05"] = "纯粹_玛约(超帅)",
        ["Apostle_Pure_06"] = "纯粹_斯皮奇",
        ["Apostle_Pure_07"] = "纯粹_加维亚",
        ["Apostle_Pure_08"] = "纯粹_莎莉",
        ["Apostle_Pure_09"] = "纯粹_玛戈",
        ["Apostle_Pure_10"] = "纯粹_谢伦",
        ["Apostle_Pure_11"] = "纯粹_海莉",
        ["Apostle_Pure_12"] = "纯粹_奈亚",
        ["Apostle_Pure_13"] = "纯粹_卡罗特",
        ["Apostle_Pure_14"] = "纯粹_达雅",
        ["Apostle_Pure_15"] = "纯粹_埃尔芬",
        ["Apostle_Pure_16"] = "纯粹_欧珀",
        ["Apostle_Pure_17"] = "纯粹_莱卡",
        ["Apostle_Pure_18"] = "纯粹_凯茜",
        ["Apostle_Pure_19"] = "纯粹_缪特",
        ["Apostle_Pure_20"] = "纯粹_黛莉娅",
        ["Apostle_Pure_21"] = "纯粹_伊德（康复）",
        ["Apostle_Pure_22"] = "纯粹_大木头",
        ["Apostle_Pure_23"] = "纯粹_洛涅",
        ["Apostle_Pure_24"] = "纯粹_阿莱特",
        ["Apostle_Pure_25"] = "纯粹_乔伊",
        ["TestRainbowCard"] = "乌洛斯",
        // ── 冷静 Calm ──
        ["Apostle_Calm_01"] = "冷静_01", ["Apostle_Calm_02"] = "冷静_02",
        ["Apostle_Calm_03"] = "冷静_03", ["Apostle_Calm_04"] = "冷静_04",
        ["Apostle_Calm_05"] = "冷静_05", ["Apostle_Calm_06"] = "冷静_06",
        ["Apostle_Calm_07"] = "冷静_07", ["Apostle_Calm_08"] = "冷静_08",
        ["Apostle_Calm_09"] = "冷静_09", ["Apostle_Calm_10"] = "冷静_10",
        ["Apostle_Calm_11"] = "冷静_11", ["Apostle_Calm_12"] = "冷静_12",
        ["Apostle_Calm_13"] = "冷静_13", ["Apostle_Calm_14"] = "冷静_14",
        ["Apostle_Calm_15"] = "冷静_15", ["Apostle_Calm_16"] = "冷静_16",
        ["Apostle_Calm_17"] = "冷静_17", ["Apostle_Calm_18"] = "冷静_18",
        ["Apostle_Calm_19"] = "冷静_19", ["Apostle_Calm_20"] = "冷静_20",
        ["Apostle_Calm_21"] = "冷静_21", ["Apostle_Calm_22"] = "冷静_22",
        ["Apostle_Calm_23"] = "冷静_23", ["Apostle_Calm_24"] = "冷静_24",
        ["Apostle_Calm_25"] = "冷静_25",
        ["Apostle_Calm_26"] = "冷静_25",
        // ── 狂热 Frenzy ──
        ["Apostle_Frenzy_01"] = "狂热_01", ["Apostle_Frenzy_02"] = "狂热_02",
        ["Apostle_Frenzy_03"] = "狂热_03", ["Apostle_Frenzy_04"] = "狂热_04",
        ["Apostle_Frenzy_05"] = "狂热_05", ["Apostle_Frenzy_06"] = "狂热_06",
        ["Apostle_Frenzy_07"] = "狂热_07", ["Apostle_Frenzy_08"] = "狂热_08",
        ["Apostle_Frenzy_09"] = "狂热_09", ["Apostle_Frenzy_10"] = "狂热_10",
        ["Apostle_Frenzy_11"] = "狂热_11", ["Apostle_Frenzy_12"] = "狂热_12",
        ["Apostle_Frenzy_13"] = "狂热_13", ["Apostle_Frenzy_14"] = "狂热_14",
        ["Apostle_Frenzy_15"] = "狂热_15", ["Apostle_Frenzy_16"] = "狂热_16",
        ["Apostle_Frenzy_17"] = "狂热_17", ["Apostle_Frenzy_18"] = "狂热_18",
        ["Apostle_Frenzy_19"] = "狂热_19", ["Apostle_Frenzy_20"] = "狂热_20",
        ["Apostle_Frenzy_21"] = "狂热_21", ["Apostle_Frenzy_22"] = "狂热_22",
        ["Apostle_Frenzy_23"] = "狂热_23", ["Apostle_Frenzy_24"] = "狂热_24",
        ["Apostle_Frenzy_25"] = "狂热_25", ["Apostle_Frenzy_26"] = "狂热_26",
        // ── 活泼 Lively ──
        ["Apostle_Lively_01"] = "活泼_01", ["Apostle_Lively_02"] = "活泼_02",
        ["Apostle_Lively_03"] = "活泼_03", ["Apostle_Lively_04"] = "活泼_04",
        ["Apostle_Lively_05"] = "活泼_05", ["Apostle_Lively_05_1"] = "活泼_05",
        ["Apostle_Lively_06"] = "活泼_06",
        ["Apostle_Lively_07"] = "活泼_07", ["Apostle_Lively_08"] = "活泼_08",
        ["Apostle_Lively_08_1"] = "活泼_08", ["Apostle_Lively_08_2"] = "活泼_08", ["Apostle_Lively_08_3"] = "活泼_08",
        ["Apostle_Lively_09"] = "活泼_09", ["Apostle_Lively_10"] = "活泼_10",
        ["Apostle_Lively_11"] = "活泼_11", ["Apostle_Lively_12"] = "活泼_12",
        ["Apostle_Lively_13"] = "活泼_13", ["Apostle_Lively_14"] = "活泼_14",
        ["Apostle_Lively_15"] = "活泼_15", ["Apostle_Lively_16"] = "活泼_16",
        ["Apostle_Lively_17"] = "活泼_17", ["Apostle_Lively_18"] = "活泼_18",
        ["Apostle_Lively_19"] = "活泼_19", ["Apostle_Lively_20"] = "活泼_20",
        ["Apostle_Lively_21"] = "活泼_21", ["Apostle_Lively_22"] = "活泼_22",
        ["Apostle_Lively_23"] = "活泼_23", ["Apostle_Lively_24"] = "活泼_24",
        ["Apostle_Lively_25"] = "活泼_25", ["Apostle_Lively_26"] = "活泼_26",
        ["Apostle_Lively_27"] = "活泼_27",
        // ── 忧郁 Melancholy ──
        ["Apostle_Melancholy_01"] = "忧郁_01", ["Apostle_Melancholy_02"] = "忧郁_02",
        ["Apostle_Melancholy_02_1"] = "忧郁_02", ["Apostle_Melancholy_02_2"] = "忧郁_02",
        ["Apostle_Melancholy_03"] = "忧郁_03", ["Apostle_Melancholy_04"] = "忧郁_04",
        ["Apostle_Melancholy_05"] = "忧郁_05", ["Apostle_Melancholy_06"] = "忧郁_06",
        ["Apostle_Melancholy_07"] = "忧郁_07", ["Apostle_Melancholy_08"] = "忧郁_08",
        ["Apostle_Melancholy_09"] = "忧郁_09", ["Apostle_Melancholy_10"] = "忧郁_10",
        ["Apostle_Melancholy_11"] = "忧郁_11", ["Apostle_Melancholy_12"] = "忧郁_12",
        ["Apostle_Melancholy_13"] = "忧郁_13", ["Apostle_Melancholy_14"] = "忧郁_14",
        ["Apostle_Melancholy_15"] = "忧郁_15", ["Apostle_Melancholy_16"] = "忧郁_16",
        ["Apostle_Melancholy_17"] = "忧郁_17", ["Apostle_Melancholy_18"] = "忧郁_18",
        ["Apostle_Melancholy_19"] = "忧郁_19", ["Apostle_Melancholy_20"] = "忧郁_20",
        ["Apostle_Melancholy_21"] = "忧郁_21", ["Apostle_Melancholy_22"] = "忧郁_22",
        ["Apostle_Melancholy_23"] = "忧郁_23", ["Apostle_Melancholy_24"] = "忧郁_24",
        ["Apostle_Melancholy_25"] = "忧郁_25",
    };

    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        try
        {
            var model = __instance.Model;
            if (model == null) return;

            var tags = model.Tags;
            bool isApostle = tags != null && tags.Contains(CultLeaderCardTags.Apostle);

            // Find or create badge node
            var badge = __instance.GetNodeOrNull<TextureRect>(BadgeNodeName);

            if (!isApostle)
            {
                // Not an apostle card — hide badge
                if (badge != null) badge.Visible = false;
                return;
            }

            // Apostle card — create badge if needed
            if (badge == null)
            {
                badge = new TextureRect
                {
                    Name = BadgeNodeName,
                    ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                    };
                __instance.AddChild(badge);
            }

            // Load correct texture
            var cardTypeName = model.GetType().Name;
            string texturePath;
            if (PortraitMap.TryGetValue(cardTypeName, out var portraitName))
                texturePath = $"{PortraitDir}{portraitName}.png";
            else
                texturePath = FallbackBadgePath;

            var texture = GD.Load<Texture2D>(texturePath);
            if (texture == null)
            {
                badge.Visible = false;
                return;
            }

            badge.Texture = texture;
            badge.CustomMinimumSize = BadgeSize;
            badge.Size = BadgeSize;

            // Position: top-right inside the card bounds
            var cardSize = __instance.GetCurrentSize();
            badge.Position = new Vector2(
                cardSize.X - BadgeSize.X - RightMargin,
                TopMargin);

            badge.Visible = true;
        }
        catch (Exception ex)
        {
            Entry.Logger.Error($"[ApostleBadgePatch] Error: {ex}");
        }
    }
}
