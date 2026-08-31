using CultLeaderMod.CultLeaderModCode.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System.Text.Json;

namespace CultLeaderMod.CultLeaderModCode.Vfx;

/// <summary>
/// Small Spine runtime prototype for apostle card battle VFX and inspect previews.
/// The first batch intentionally stays profile-driven and narrow while the Spine
/// pipeline is being validated in-game.
/// </summary>
public static class ApostleSpinePrototype
{
    private const string SpineRoot = @"E:\work\Cult_leader_mod\SPINE_4_2_TEST";
    private const int BaseVfxLayer = 180;
    private const int VfxLayerCycle = 1000;
    private const string PreviewNodeName = "CultLeaderInspectApostleSpinePreview";
    private const string PreviewGroupName = "CultLeaderInspectApostleSpinePreviews";
    private const string PreviewAliveMetaKey = "CultLeaderSpinePreviewAlive";
    private const string PreviewCardMetaKey = "CultLeaderSpinePreviewCard";
    private const string RequiredRuntimeMajorMinor = "4.2";
    private const float BattleFadeOutSeconds = 0.5f;
    private const float MinimumBattleAnimationSeconds = 0.1f;
    private static readonly Vector2 PlayerLowerCenterAnchor = new(0.30f, 0.63f);
    private static readonly Vector2 PlayerBesideJitterRange = new(140f, 12f);
    private static readonly Dictionary<string, float?> AnimationDurationCache = [];
    private static int _vfxSequence;
    private static bool _warnedIncompatibleSkeleton;
    private static readonly HashSet<string> FrameFallbackCardTypeNames =
    [
        nameof(Apostle_Calm_23),       // 蕾特：converted Spine preview has persistent atlas/mesh scrambling.
        nameof(Apostle_Lively_13),     // 修罗：converted Spine preview has persistent atlas/mesh scrambling.
        nameof(Apostle_Melancholy_10), // 洛涅（市长）：converted Spine preview has persistent atlas/mesh scrambling.
        nameof(Apostle_Melancholy_25), // 乔菲：converted Spine preview remains partially broken after atlas/UV repair attempts.
        nameof(Apostle_Melancholy_26), // 欧若拉：converted Spine preview and battle animation have persistent atlas/mesh scrambling.
    ];
    private static readonly Dictionary<string, SpineApostleProfile> Profiles = new()
    {
        [nameof(Apostle_Pure_01)] = new(nameof(Apostle_Pure_01), "ErpinRoyale", "Normal", "Idle_1", ["Skill1_1", "Skill1_1_Loop", "Skill1_1_End"], [1.67f, 0.80f, 1.33f], 4.15f, "埃尔芬（王道）"),
        [nameof(Apostle_Pure_02)] = new(nameof(Apostle_Pure_02), "Vivi", "Normal", "Blank_1", ["Skill1_1"], [], 1.72f, "薇薇安娜·阿尔根图姆"),
        [nameof(Apostle_Pure_03)] = new(nameof(Apostle_Pure_03), "Ran", "Normal", "Happy_7", ["Attack2_1"], [], 2.82f, "岚"),
        [nameof(Apostle_Pure_04)] = new(nameof(Apostle_Pure_04), "Ayla", "Normal", "Lazy_1", ["Victory"], [], 4.00f, "阿伊拉"),
        [nameof(Apostle_Pure_05)] = new(nameof(Apostle_Pure_05), "MayoCool", "Normal", "Happy_9", ["Skill1_1_Start2"], [], 3.25f, "玛约(超帅)"),
        [nameof(Apostle_Pure_06)] = new(nameof(Apostle_Pure_06), "Speaki", "Normal", "Happy_5", ["Skill1_1"], [], 2.08f, "斯皮奇"),
        [nameof(Apostle_Pure_07)] = new(nameof(Apostle_Pure_07), "Gabia", "Normal", "Happy_1", ["Attack1_1"], [], 1.72f, "加维亚"),
        [nameof(Apostle_Pure_08)] = new(nameof(Apostle_Pure_08), "Sari", "Normal", "Happy_6", ["Attack1_1"], [], 1.52f, "莎莉"),
        [nameof(Apostle_Pure_09)] = new(nameof(Apostle_Pure_09), "Mago", "Normal", "Taunt_2", ["Attack1_1"], [], 1.82f, "玛戈"),
        [nameof(Apostle_Pure_10)] = new(nameof(Apostle_Pure_10), "Sherum", "Normal", "Happy_5", ["Skill1_1"], [], 4.00f, "谢伦"),
        [nameof(Apostle_Pure_11)] = new(nameof(Apostle_Pure_11), "Haley", "Normal", "Idle_1", ["Attack1_1"], [], 3.32f, "海莉"),
        [nameof(Apostle_Pure_12)] = new(nameof(Apostle_Pure_12), "Naia", "Normal", "Happy_1", ["Skill1_3", "Skill1_3", "Skill1_3", "Skill1_3"], [0.33f, 0.33f, 0.33f, 0.33f], 1.70f, "奈亚"),
        [nameof(Apostle_Pure_13)] = new(nameof(Apostle_Pure_13), "Kyarot", "Normal", "Melong_1", ["Ultimate1_1"], [], 4.00f, "卡罗特"),
        [nameof(Apostle_Pure_14)] = new(nameof(Apostle_Pure_14), "Daya", "Normal", "Shy_2", ["Attack2_1"], [], 2.92f, "达雅"),
        [nameof(Apostle_Pure_15)] = new(nameof(Apostle_Pure_15), "Erpin", "Normal", "Eat_1", ["Ultimate1_1", "Ultimate1_2_Loop", "Ultimate1_3"], [1.17f, 0.53f, 3.00f], 4.90f, "埃尔芬"),
        [nameof(Apostle_Pure_16)] = new(nameof(Apostle_Pure_16), "Opal", "Normal", "Sad_5", ["Skill1_1"], [], 2.72f, "欧珀"),
        [nameof(Apostle_Pure_17)] = new(nameof(Apostle_Pure_17), "Laika", "Normal", "Happy_5", ["Skill1_1_Change"], [], 2.25f, "莱卡"),
        [nameof(Apostle_Pure_18)] = new(nameof(Apostle_Pure_18), "Kathy", "Normal", "Idle_1", ["Skill1_1"], [], 3.25f, "凯茜"),
        [nameof(Apostle_Pure_19)] = new(nameof(Apostle_Pure_19), "Mute", "Normal", "Happy_1", ["Skill_4"], [], 2.42f, "缪特"),
        [nameof(Apostle_Pure_20)] = new(nameof(Apostle_Pure_20), "Delia", "Normal", "Happy_5", ["Ultimate1_1"], [], 3.58f, "黛莉娅"),
        [nameof(Apostle_Pure_21)] = new(nameof(Apostle_Pure_21), "EdRehab", "Normal", "Gao_1", ["Attack1_1"], [], 2.98f, "伊德（康复）"),
        [nameof(Apostle_Pure_22)] = new(nameof(Apostle_Pure_22), "BigWood", "Normal", "Happy_3", ["Victory"], [], 1.45f, "大木头"),
        [nameof(Apostle_Pure_23)] = new(nameof(Apostle_Pure_23), "Rohne", "Normal", "Taunt_1", ["Ultimate1_1"], [], 4.00f, "洛涅"),
        [nameof(Apostle_Pure_24)] = new(nameof(Apostle_Pure_24), "Allet", "Normal", "Idle_1", ["Skill1_1"], [], 1.75f, "阿莱特"),
        [nameof(Apostle_Pure_25)] = new(nameof(Apostle_Pure_25), "Cuee", "Normal", "Happy_7", ["Attack1_1"], [], 1.48f, "乔伊"),
        [nameof(Apostle_Pure_26)] = new(nameof(Apostle_Pure_26), "Sparrot", "Normal", "Parrot_1", ["Attack2_1"], [], 2.98f, "斯帕洛特"),
        [nameof(Apostle_Lively_01)] = new(nameof(Apostle_Lively_01), "Ui", "Normal", "Happy_4", ["Ultimate1_1"], [], 4.00f, "雨伊"),
        [nameof(Apostle_Lively_02)] = new(nameof(Apostle_Lively_02), "Rude", "Normal", "Happy_1", ["Skill1_1"], [], 2.22f, "鲁德"),
        [nameof(Apostle_Lively_03)] = new(nameof(Apostle_Lively_03), "Rufo", "Normal", "Happy_1", ["Attack1_1"], [], 1.62f, "卢波"),
        [nameof(Apostle_Lively_04)] = new(nameof(Apostle_Lively_04), "Canna", "Normal", "Happy_6", ["Attack2_1"], [], 2.25f, "康娜"),
        [nameof(Apostle_Lively_05)] = new(nameof(Apostle_Lively_05), "Butter", "Normal", "Happy_2", ["Attack1_1"], [], 1.75f, "黄油"),
        [nameof(Apostle_Lively_05_1)] = new(nameof(Apostle_Lively_05_1), "Butter", "Normal", "Happy_2", ["Attack1_1"], [], 1.75f, "黄油"),
        [nameof(Apostle_Lively_06)] = new(nameof(Apostle_Lively_06), "Tig", "Normal", "Idle_1", ["Skill1_1"], [], 1.42f, "提格"),
        [nameof(Apostle_Lively_07)] = new(nameof(Apostle_Lively_07), "Selline", "Normal", "Happy_2", ["OW1_Ultimate1_1"], [], 3.38f, "赛琳娜"),
        [nameof(Apostle_Lively_08)] = new(nameof(Apostle_Lively_08), "Epica", "Normal", "Happy_1", ["Attack2_1"], [], 2.25f, "埃皮卡"),
        [nameof(Apostle_Lively_08_1)] = new(nameof(Apostle_Lively_08_1), "Epica", "Normal", "Happy_1", ["Attack1_1"], [], 2.08f, "埃皮卡", BattleResourceCode: "Epicon", BattleCategory: @"其余\召唤物"),
        [nameof(Apostle_Lively_08_2)] = new(nameof(Apostle_Lively_08_2), "Epica", "Normal", "Happy_1", ["Attack2_1"], [], 2.42f, "埃皮卡", BattleResourceCode: "Epicon", BattleCategory: @"其余\召唤物"),
        [nameof(Apostle_Lively_08_3)] = new(nameof(Apostle_Lively_08_3), "Epica", "Normal", "Happy_1", ["Idle1_5"], [], 1.45f, "埃皮卡", BattleResourceCode: "Epicon", BattleCategory: @"其余\召唤物"),
        [nameof(Apostle_Lively_09)] = new(nameof(Apostle_Lively_09), "Miro", "Normal", "Idle_1", ["Ultimate1_1"], [], 3.78f, "米洛"),
        [nameof(Apostle_Lively_10)] = new(nameof(Apostle_Lively_10), "Makasha", "Normal", "Dance_2", ["Skill1_1"], [], 3.25f, "玛卡莎"),
        [nameof(Apostle_Lively_11)] = new(nameof(Apostle_Lively_11), "Arco", "Normal", "Dance_1", ["Attack1_2"], [], 0.80f, "阿尔柯"),
        [nameof(Apostle_Lively_12)] = new(nameof(Apostle_Lively_12), "Vela", "Normal", "Blank_2", ["Attack1_2"], [], 2.85f, "贝拉"),
        [nameof(Apostle_Lively_13)] = new(nameof(Apostle_Lively_13), "Suro", "Normal", "Shy_1", ["Attack1_2"], [], 2.98f, "修罗"),
        [nameof(Apostle_Lively_14)] = new(nameof(Apostle_Lively_14), "SpeakiMaid", "Normal", "Clean_1", ["Attack2_1"], [], 3.25f, "斯碧琪（女仆）"),
        [nameof(Apostle_Lively_15)] = new(nameof(Apostle_Lively_15), "Momo", "Normal", "Serious_1", ["Ultimate1_1"], [], 1.05f, "莫莫"),
        [nameof(Apostle_Lively_16)] = new(nameof(Apostle_Lively_16), "Shoupan", "Normal", "Taunt_2", ["Ultimate1_1"], [], 2.28f, "舒胖"),
        [nameof(Apostle_Lively_17)] = new(nameof(Apostle_Lively_17), "ShadyTwisted", "Normal", "Dance_1", ["Attack1_1"], [], 2.48f, "谢迪（逆转）"),
        [nameof(Apostle_Lively_18)] = new(nameof(Apostle_Lively_18), "NerRage", "Normal", "Pray_1", ["Attack2_1"], [], 2.98f, "涅尔（愤怒）"),
        [nameof(Apostle_Lively_19)] = new(nameof(Apostle_Lively_19), "LeviGraduate", "Normal", "Happy_7", ["Skill1_1"], [], 3.58f, "莱薇（毕业）"),
        [nameof(Apostle_Lively_20)] = new(nameof(Apostle_Lively_20), "AshurMagi", "Normal", "Idle_1", ["Attack2_1"], [], 2.92f, "艾舒尔（魔道）"),
        [nameof(Apostle_Lively_21)] = new(nameof(Apostle_Lively_21), "Bana", "Normal", "Idle_1", ["Skill1_1"], [], 3.78f, "芭娜"),
        [nameof(Apostle_Lively_22)] = new(nameof(Apostle_Lively_22), "Jubee", "Normal", "Happy_3", ["Attack1_1"], [], 1.75f, "茱比"),
        [nameof(Apostle_Lively_23)] = new(nameof(Apostle_Lively_23), "Beni", "Normal", "Excited_3", ["Skill1_1"], [], 2.65f, "班尼"),
        [nameof(Apostle_Lively_24)] = new(nameof(Apostle_Lively_24), "Marie", "Normal", "Happy_2", ["Attack1_1"], [], 1.75f, "玛丽"),
        [nameof(Apostle_Lively_25)] = new(nameof(Apostle_Lively_25), "Carren", "Normal", "Happy_7", ["Skill1_1"], [], 2.35f, "卡伦"),
        [nameof(Apostle_Lively_26)] = new(nameof(Apostle_Lively_26), "Taida", "Normal", "Dance_2", ["Attack1_1"], [], 1.72f, "泰达"),
        [nameof(Apostle_Lively_27)] = new(nameof(Apostle_Lively_27), "Mynx", "Normal", "Happy_4", ["Skill1_1"], [], 1.25f, "米雪"),
        [nameof(Apostle_Frenzy_01)] = new(nameof(Apostle_Frenzy_01), "Chloe", "Normal", "Idle_1", ["Skill1_1"], [], 2.50f, "克萝伊", PreviewEnabled: false),
        [nameof(Apostle_Frenzy_02)] = new(nameof(Apostle_Frenzy_02), "Diana", "Normal", "Happy_1", ["Ultimate1_1"], [], 4.00f, "黛安娜"),
        [nameof(Apostle_Frenzy_03)] = new(nameof(Apostle_Frenzy_03), "Shady", "Normal", "Joke_3", ["Attack1_1"], [], 2.25f, "谢迪"),
        [nameof(Apostle_Frenzy_04)] = new(nameof(Apostle_Frenzy_04), "Ner", "Normal", "Angry_6", ["Skill1_1"], [], 2.58f, "尼尔"),
        [nameof(Apostle_Frenzy_05)] = new(nameof(Apostle_Frenzy_05), "Sist", "Normal", "Happy_1", ["Skill1_1"], [], 2.58f, "西斯特"),
        [nameof(Apostle_Frenzy_06)] = new(nameof(Apostle_Frenzy_06), "Belita", "Normal", "Happy_1", ["Skill1_1"], [], 2.92f, "贝丽塔"),
        [nameof(Apostle_Frenzy_07)] = new(nameof(Apostle_Frenzy_07), "Alice", "Normal", "Happy_3", ["Attack1_1"], [], 2.50f, "爱丽丝"),
        [nameof(Apostle_Frenzy_08)] = new(nameof(Apostle_Frenzy_08), "Leets", "Normal", "Happy_1", ["Attack1_1"], [], 4.00f, "丽兹"),
        [nameof(Apostle_Frenzy_09)] = new(nameof(Apostle_Frenzy_09), "TigHero", "Normal", "Happy_1", ["Skill1_1"], [], 3.52f, "提格（英雄）"),
        [nameof(Apostle_Frenzy_10)] = new(nameof(Apostle_Frenzy_10), "Arnet", "Normal", "Happy_6", ["Attack1_1"], [], 2.25f, "阿妮特"),
        [nameof(Apostle_Frenzy_11)] = new(nameof(Apostle_Frenzy_11), "Neti", "Normal", "Drill_2", ["Ultimate1_1"], [], 4.00f, "涅缇"),
        [nameof(Apostle_Frenzy_12)] = new(nameof(Apostle_Frenzy_12), "RimChaos", "Normal", "Heart_1", ["Attack1_1"], [], 2.58f, "琳（混沌）"),
        [nameof(Apostle_Frenzy_13)] = new(nameof(Apostle_Frenzy_13), "Polan", "Normal", "Happy_1", ["Skill1_1"], [], 4.00f, "破朗"),
        [nameof(Apostle_Frenzy_14)] = new(nameof(Apostle_Frenzy_14), "Pira", "Normal", "Happy_2", ["Attack2_1"], [], 3.02f, "皮拉"),
        [nameof(Apostle_Frenzy_15)] = new(nameof(Apostle_Frenzy_15), "RenewaAwaken", "Normal", "Dance_1", ["Attack2_1"], [], 3.12f, "莉纽瓦"),
        [nameof(Apostle_Frenzy_16)] = new(nameof(Apostle_Frenzy_16), "Rollett", "Normal", "Happy_1", ["Experience_2"], [], 3.35f, "罗莱特"),
        [nameof(Apostle_Frenzy_17)] = new(nameof(Apostle_Frenzy_17), "Heidi", "Normal", "Camera_2", ["Attack1_1"], [], 2.58f, "海蒂"),
        [nameof(Apostle_Frenzy_18)] = new(nameof(Apostle_Frenzy_18), "DayaPureShine", "Normal", "Happy_3", ["Attack2_1"], [], 3.05f, "达雅（纯真闪耀）"),
        [nameof(Apostle_Frenzy_19)] = new(nameof(Apostle_Frenzy_19), "HaleySane", "Normal", "Idle_2", ["Ultimate1_1"], [], 4.00f, "海莉（清醒）"),
        [nameof(Apostle_Frenzy_20)] = new(nameof(Apostle_Frenzy_20), "Silvia", "Normal", "Happy_6", ["Ultimate1_1"], [], 2.68f, "西尔维娅"),
        [nameof(Apostle_Frenzy_21)] = new(nameof(Apostle_Frenzy_21), "Skea", "Normal", "Happy_2", ["Attack1_1"], [], 3.05f, "斯琪娅"),
        [nameof(Apostle_Frenzy_22)] = new(nameof(Apostle_Frenzy_22), "MaestroMK2", "Normal", "Happy_4", ["Skill1_1"], [], 2.85f, "大师2号"),
        [nameof(Apostle_Frenzy_23)] = new(nameof(Apostle_Frenzy_23), "Mayo", "Normal", "Happy_7", ["Attack1_1"], [], 1.72f, "玛约"),
        [nameof(Apostle_Frenzy_24)] = new(nameof(Apostle_Frenzy_24), "Ifrit", "Normal", "Happy_3", ["Ultimate1_1"], [], 4.00f, "伊芙利特"),
        [nameof(Apostle_Frenzy_25)] = new(nameof(Apostle_Frenzy_25), "Maison", "Normal", "Happy_2", ["Skill1_1"], [], 2.02f, "梅森"),
        [nameof(Apostle_Frenzy_26)] = new(nameof(Apostle_Frenzy_26), "Yumimi", "Normal", "Happy_5", ["Attack1_1"], [], 1.88f, "刘美美"),
        [nameof(Apostle_Calm_01)] = new(nameof(Apostle_Calm_01), "Aya", "Normal", "Idle_2", ["Attack1_1"], [], 1.75f, "阿雅"),
        [nameof(Apostle_Calm_02)] = new(nameof(Apostle_Calm_02), "Sylla", "Normal", "Dance_1", ["Skill1_1"], [], 2.75f, "希拉"),
        [nameof(Apostle_Calm_03)] = new(nameof(Apostle_Calm_03), "Elena", "Normal", "Happy_4", ["Attack2_1"], [], 1.95f, "埃蕾娜"),
        [nameof(Apostle_Calm_04)] = new(nameof(Apostle_Calm_04), "Amelia", "Normal", "Serious_3", ["OW1_Skill1_1"], [], 4.00f, "阿梅利亚"),
        [nameof(Apostle_Calm_05)] = new(nameof(Apostle_Calm_05), "Meluna", "Normal", "Happy_3", ["Attack2_1"], [], 1.75f, "梅露娜"),
        [nameof(Apostle_Calm_06)] = new(nameof(Apostle_Calm_06), "Fricle", "Normal", "Angry_1", ["Ultimate1_1"], [], 2.75f, "芙莉克尔"),
        [nameof(Apostle_Calm_07)] = new(nameof(Apostle_Calm_07), "Jade", "Normal", "Idle_3", ["Attack1_1"], [], 1.75f, "杰德"),
        [nameof(Apostle_Calm_08)] = new(nameof(Apostle_Calm_08), "Velvet", "Normal", "Angry_6", ["Ultimate1_1"], [], 4.00f, "薇尔薇特"),
        [nameof(Apostle_Calm_09)] = new(nameof(Apostle_Calm_09), "KommySwim", "Normal", "Melong_3", ["Ultimate1_1"], [], 4.00f, "柯米(泳装)"),
        [nameof(Apostle_Calm_10)] = new(nameof(Apostle_Calm_10), "Picora", "Normal", "Happy_1", ["Skill1_1"], [], 2.45f, "皮可拉"),
        [nameof(Apostle_Calm_11)] = new(nameof(Apostle_Calm_11), "Ed", "Normal", "Idle_1", ["Ultimate1_1"], [], 4.00f, "伊德"),
        [nameof(Apostle_Calm_12)] = new(nameof(Apostle_Calm_12), "Barong", "Normal", "Smash_End_2", ["Attack1_1"], [], 2.58f, "巴隆"),
        [nameof(Apostle_Calm_13)] = new(nameof(Apostle_Calm_13), "Guin", "Normal", "Dance_2", ["Skill1_1"], [], 3.52f, "格温"),
        [nameof(Apostle_Calm_14)] = new(nameof(Apostle_Calm_14), "Eisia", "Normal", "Serious_2", ["Experience_1"], [], 3.92f, "艾西亚", SecondaryBattleProfiles: [new("EisiaFridge", @"战斗模型", "Normal", ["Attack1_1"], [], 1.38f, 3.00f, new Vector2(150f, 0f), 0.34f)]),
        [nameof(Apostle_Calm_15)] = new(nameof(Apostle_Calm_15), "Ricota", "Normal", "Idle_1", ["Skill1_1"], [], 4.00f, "里科塔"),
        [nameof(Apostle_Calm_16)] = new(nameof(Apostle_Calm_16), "DianaYester", "Normal", "Angry_2", ["Ultimate1_1"], [], 4.00f, "黛安娜（往昔）"),
        [nameof(Apostle_Calm_17)] = new(nameof(Apostle_Calm_17), "Scizor", "Normal", "Taunt_2", ["Attack2_1"], [], 3.82f, "凯撒"),
        [nameof(Apostle_Calm_18)] = new(nameof(Apostle_Calm_18), "BeniBeni", "Normal", "Excited_3", ["Attack2_1"], [], 3.98f, "班尼（班尼）"),
        [nameof(Apostle_Calm_19)] = new(nameof(Apostle_Calm_19), "Inkle", "Normal", "Happy_2", ["Ultimate1_1"], [], 3.62f, "茵刻尔"),
        [nameof(Apostle_Calm_20)] = new(nameof(Apostle_Calm_20), "Aragnia", "Normal", "Spit_1", ["Skill1_1"], [], 3.52f, "阿拉戈尼娅"),
        [nameof(Apostle_Calm_21)] = new(nameof(Apostle_Calm_21), "Nicole", "Normal", "Angry_5", ["Skill1_1"], [], 4.00f, "妮可"),
        [nameof(Apostle_Calm_22)] = new(nameof(Apostle_Calm_22), "Espi", "Normal", "Question_2", ["Skill1_1"], [], 2.25f, "埃斯皮"),
        [nameof(Apostle_Calm_23)] = new(nameof(Apostle_Calm_23), "Lethe", "Normal", "Happy_1", ["Skill1_1"], [], 4.00f, "蕾特"),
        [nameof(Apostle_Calm_24)] = new(nameof(Apostle_Calm_24), "Patula", "Normal", "Happy_1", ["Skill1_1"], [], 2.22f, "帕特拉"),
        [nameof(Apostle_Calm_25)] = new(nameof(Apostle_Calm_25), "Lazy", "Normal", "Idle_2", ["Attack1_1"], [], 1.72f, "雷吉"),
        [nameof(Apostle_Calm_26)] = new(nameof(Apostle_Calm_26), "Canta", "Normal", "Happy_1", ["Attack2_1"], [], 2.92f, "康塔"),
        [nameof(Apostle_Melancholy_01)] = new(nameof(Apostle_Melancholy_01), "Kommy", "Normal", "Taunt_1", ["Skill1_1"], [], 2.50f, "科米"),
        [nameof(Apostle_Melancholy_02)] = new(nameof(Apostle_Melancholy_02), "xXionx", "Normal", "Angry_8", ["Attack2_1"], [], 2.50f, "x锡安x"),
        [nameof(Apostle_Melancholy_02_1)] = new(nameof(Apostle_Melancholy_02_1), "xXionx", "Normal", "Angry_8", ["Attack2_1"], [], 2.50f, "x锡安x"),
        [nameof(Apostle_Melancholy_02_2)] = new(nameof(Apostle_Melancholy_02_2), "xXionx", "Normal", "Angry_8", ["Attack2_1"], [], 2.50f, "x锡安x"),
        [nameof(Apostle_Melancholy_03)] = new(nameof(Apostle_Melancholy_03), "Posher", "Normal", "Happy_2", ["Skill1_1"], [], 2.50f, "珀榭"),
        [nameof(Apostle_Melancholy_04)] = new(nameof(Apostle_Melancholy_04), "Kidian", "Normal", "Dance_1", ["Attack1_1"], [], 2.50f, "基迪恩"),
        [nameof(Apostle_Melancholy_05)] = new(nameof(Apostle_Melancholy_05), "Rim", "Normal", "Happy_4", ["Attack1_1"], [], 2.50f, "琳"),
        [nameof(Apostle_Melancholy_06)] = new(nameof(Apostle_Melancholy_06), "Ashur", "Normal", "Happy_7", ["Attack2_1"], [], 2.50f, "艾舒尔"),
        [nameof(Apostle_Melancholy_07)] = new(nameof(Apostle_Melancholy_07), "Hilde", "Normal", "Happy_3", ["Attack2_1"], [], 2.50f, "希尔德"),
        [nameof(Apostle_Melancholy_08)] = new(nameof(Apostle_Melancholy_08), "Risty", "Normal", "Blank_3", ["Experience_1"], [], 2.50f, "莉斯缇"),
        [nameof(Apostle_Melancholy_09)] = new(nameof(Apostle_Melancholy_09), "Asana", "Normal", "Yoga_2", ["Attack2_2"], [], 2.50f, "阿萨娜"),
        [nameof(Apostle_Melancholy_10)] = new(nameof(Apostle_Melancholy_10), "RohneMayor", "Normal", "Happy_8", ["Attack1_1"], [], 2.50f, "洛涅（市长）"),
        [nameof(Apostle_Melancholy_11)] = new(nameof(Apostle_Melancholy_11), "Orr", "Normal", "Sweat_1", ["Skill1_1"], [], 2.50f, "欧尔"),
        [nameof(Apostle_Melancholy_12)] = new(nameof(Apostle_Melancholy_12), "Shasha", "Normal", "Happy_1", ["Ultimate1_1"], [], 2.50f, "莎莎"),
        [nameof(Apostle_Melancholy_13)] = new(nameof(Apostle_Melancholy_13), "Lion", "Normal", "Joke_2", ["Skill1_1"], [], 2.50f, "里昂"),
        [nameof(Apostle_Melancholy_14)] = new(nameof(Apostle_Melancholy_14), "Snorky", "Normal", "Happy_3", ["Skill1_2"], [], 2.50f, "斯诺基"),
        [nameof(Apostle_Melancholy_15)] = new(nameof(Apostle_Melancholy_15), "Joanne", "Normal", "Dance_1", ["Experience_2"], [], 2.50f, "琼安"),
        [nameof(Apostle_Melancholy_16)] = new(nameof(Apostle_Melancholy_16), "Blanchet", "Normal", "Greeting_1", ["Skill1_1"], [], 2.50f, "布蓝琪"),
        [nameof(Apostle_Melancholy_17)] = new(nameof(Apostle_Melancholy_17), "Yomi", "Normal", "Dance_3", ["Attack1_1"], [], 2.50f, "优米"),
        [nameof(Apostle_Melancholy_18)] = new(nameof(Apostle_Melancholy_18), "AmeliaR41", "Normal", "Dance_3", ["Attack1_1"], [], 2.50f, "阿梅利亚（R41）"),
        [nameof(Apostle_Melancholy_19)] = new(nameof(Apostle_Melancholy_19), "Kishya", "Normal", "Kisya_1", ["Victory"], [], 2.50f, "绮莎"),
        [nameof(Apostle_Melancholy_20)] = new(nameof(Apostle_Melancholy_20), "Silphir", "Normal", "Dance_1", ["Ultimate1_1"], [], 2.50f, "希菲尔"),
        [nameof(Apostle_Melancholy_21)] = new(nameof(Apostle_Melancholy_21), "Barie", "Normal", "Glasses_2", ["Skill1_3"], [], 2.50f, "巴丽叶"),
        [nameof(Apostle_Melancholy_22)] = new(nameof(Apostle_Melancholy_22), "Levi", "Normal", "Happy_1", ["Skill1_1"], [], 2.50f, "莱薇"),
        [nameof(Apostle_Melancholy_23)] = new(nameof(Apostle_Melancholy_23), "Festa", "Normal", "Rock_1", ["Ultimate1_1"], [], 2.50f, "菲斯塔"),
        [nameof(Apostle_Melancholy_24)] = new(nameof(Apostle_Melancholy_24), "Veroo", "Normal", "Happy_7", ["Skill1_1"], [], 2.50f, "贝鲁"),
        [nameof(Apostle_Melancholy_25)] = new(nameof(Apostle_Melancholy_25), "ChopiAllRotateUvA", "Normal", "Happy_3", ["Skill1_1"], [], 2.50f, "乔菲", BattleResourceCode: "Chopi"),
        [nameof(Apostle_Melancholy_26)] = new(nameof(Apostle_Melancholy_26), "Aurora", "Normal", "Cute_3", ["Ultimate1_1"], [], 2.50f, "欧若拉"),
        [nameof(TestRainbowCard)] = new(nameof(TestRainbowCard), "Uros", "Normal", "Tickle_End", ["Skill1_1"], [], 2.25f, "乌洛斯"),
        [nameof(DualRivalsCard)] = new(nameof(DualRivalsCard), "Uros", "Normal", "Tickle_End", ["Ultimate1_1"], [], 4.00f, "双雄相争", SecondaryBattleProfiles: [new("TigHero", "战斗模型", "Normal", ["Ultimate1_1"], [], 4.00f, 0.00f, new Vector2(190f, 0f), 0.34f, AnchorsToTarget: true, FaceRight: false)], BattlePositionOffset: new Vector2(-190f, 0f), PreviewEnabled: false, BattleAnchorsToTarget: true, BattleFaceRight: true),
        [nameof(ForElruienCard)] = new(nameof(ForElruienCard), "NerRage", "Normal", "Pray_1", ["Skill1_1"], [], 3.35f, "为了艾鲁皮恩", SecondaryBattleProfiles: [new("ErpinRoyale", "战斗模型", "Normal", ["Spawn"], [], 2.52f, 0.00f, new Vector2(130f, 0f), 0.34f)], BattlePositionOffset: new Vector2(-130f, 0f), PreviewEnabled: false),
    };

    public static bool IsPrototypeCard(Type cardType) =>
        Profiles.ContainsKey(cardType.Name) && !FrameFallbackCardTypeNames.Contains(cardType.Name);

    public static bool TryPlayBattle(Type cardType)
    {
        return TryPlayBattle(cardType, null);
    }

    public static bool TryPlayBattle(Type cardType, Creature? target)
    {
        if (FrameFallbackCardTypeNames.Contains(cardType.Name))
            return false;

        if (!Profiles.TryGetValue(cardType.Name, out var profile))
            return false;

        if (!HasCompatibleAssetSet(GetBattleAssetSet(profile), profile, "battle"))
            return false;

        _ = PlayBattleAsync(profile, target);
        return true;
    }

    public static bool TryEnsurePreview(Node screen, Type cardType)
    {
        if (FrameFallbackCardTypeNames.Contains(cardType.Name))
            return false;

        if (!Profiles.TryGetValue(cardType.Name, out var profile))
            return false;
        if (!profile.PreviewEnabled)
        {
            RemoveAllPreviews();
            return false;
        }

        try
        {
            RemoveStalePreviews(screen, profile);

            var existing = FindCurrentPreview(screen, profile);
            if (existing != null)
            {
                PositionPreview(screen, existing);
                if (existing is CanvasItem canvasItem)
                    canvasItem.Visible = true;
                return true;
            }

            var spine = CreateSpineSprite(
                GetPreviewAssetSet(profile),
                profile.PreviewAnimationName,
                loop: true,
                profile,
                debugLabel: "preview"
            );
            if (spine == null)
                return false;

            spine.Name = PreviewNodeName;
            spine.SetMeta(PreviewAliveMetaKey, true);
            spine.SetMeta(PreviewCardMetaKey, profile.CardTypeName);
            spine.AddToGroup(PreviewGroupName);

            ConfigurePreviewNode(spine);
            PositionPreview(screen, spine);
            screen.AddChild(spine);
            Entry.Logger.Info($"[SPINE_PROTO] Inspect preview mounted for {profile.CardTypeName} / {profile.DisplayName}.");
            return true;
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to mount inspect preview: {ex}");
            return false;
        }
    }

    public static void RemoveAllPreviews()
    {
        try
        {
            if (Engine.GetMainLoop() is not SceneTree tree || tree.Root == null)
                return;

            foreach (var node in tree.GetNodesInGroup(PreviewGroupName))
                MarkPreviewForRemoval(node as Node);

            RemoveNamedPreviewsRecursive(tree.Root);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to remove Spine previews: {ex}");
        }
    }

    private static async Task PlayBattleAsync(SpineApostleProfile profile, Creature? target)
    {
        CanvasLayer? layer = null;
        try
        {
            if (Engine.GetMainLoop() is not SceneTree tree || tree.Root == null)
                return;

            var spine = CreateSpineSprite(
                GetBattleAssetSet(profile),
                profile.BattleAnimationNames,
                loop: false,
                profile,
                debugLabel: "battle"
            );
            if (spine == null)
                return;

            var sequence = Interlocked.Increment(ref _vfxSequence);
            layer = new CanvasLayer
            {
                Name = $"CultLeader{profile.ResourceCode}SpineVfxLayer_{sequence}",
                Layer = BaseVfxLayer + sequence % VfxLayerCycle,
            };

            ConfigureBattleNode(spine, tree, profile.BattlePositionOffset, profile.BattleScale, target, profile.BattleAnchorsToTarget, profile.BattleFaceRight);

            layer.AddChild(spine);
            tree.Root.AddChild(layer);
            var primaryAssetSet = GetBattleAssetSet(profile);
            if (profile.BattleAnimationNames.Count > 1)
                _ = PlayBattleSequenceAsync(spine, tree, primaryAssetSet, profile);
            foreach (var secondary in profile.SecondaryBattleProfiles ?? [])
                _ = PlaySecondaryBattleAsync(layer, tree, profile, secondary, target);
            Entry.Logger.Info($"[SPINE_PROTO] Battle animation mounted for {profile.CardTypeName} / {profile.DisplayName}.");

            await layer.ToSignal(tree.CreateTimer(Math.Max(0f, GetTotalBattlePlaySeconds(profile) - BattleFadeOutSeconds)), SceneTreeTimer.SignalName.Timeout);
            await FadeOutLayerAsync(layer, tree);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to play battle animation: {ex}");
        }
        finally
        {
            if (layer != null && GodotObject.IsInstanceValid(layer))
                layer.QueueFree();
        }
    }

    private static async Task PlaySecondaryBattleAsync(CanvasLayer layer, SceneTree tree, SpineApostleProfile parentProfile, SecondarySpineBattleProfile secondary, Creature? target)
    {
        try
        {
            await layer.ToSignal(tree.CreateTimer(secondary.DelaySeconds), SceneTreeTimer.SignalName.Timeout);
            if (!GodotObject.IsInstanceValid(layer) || !layer.IsInsideTree())
                return;

            var spine = CreateSpineSprite(
                GetAssetSet(secondary.ResourceCode, secondary.Category),
                secondary.AnimationNames,
                loop: false,
                parentProfile,
                debugLabel: $"secondary battle {secondary.ResourceCode}"
            );
            if (spine == null)
                return;

            ConfigureBattleNode(spine, tree, secondary.PositionOffset, secondary.Scale, target, secondary.AnchorsToTarget, secondary.FaceRight);
            SafeSet(spine, "z_index", 320);
            layer.AddChild(spine);

            if (secondary.AnimationNames.Count > 1)
                _ = PlayBattleSequenceAsync(spine, tree, GetAssetSet(secondary.ResourceCode, secondary.Category), secondary);

            Entry.Logger.Info($"[SPINE_PROTO] Secondary battle animation mounted for {parentProfile.CardTypeName}: {secondary.ResourceCode}.");
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to play secondary battle animation for {parentProfile.CardTypeName}: {ex}");
        }
    }

    private static Node? CreateSpineSprite(SpineAssetSet assetSet, string animationName, bool loop, SpineApostleProfile profile, string debugLabel) =>
        CreateSpineSprite(assetSet, [animationName], loop, profile, debugLabel);

    private static Node? CreateSpineSprite(SpineAssetSet assetSet, IReadOnlyList<string> animationNames, bool loop, SpineApostleProfile profile, string debugLabel)
    {
        try
        {
            if (!ClassDB.ClassExists("SpineSprite") || !ClassDB.CanInstantiate("SpineSprite"))
            {
                Entry.Logger.Warn("[SPINE_PROTO] SpineSprite class is not registered. The STS2 Spine GDExtension may not be available to C#.");
                return null;
            }

            if (!HasCompatibleAssetSet(assetSet, profile, debugLabel))
                return null;

            var atlas = ClassDB.Instantiate("SpineAtlasResource").AsGodotObject() as Resource;
            var skeletonFile = ClassDB.Instantiate("SpineSkeletonFileResource").AsGodotObject() as Resource;
            var skeletonData = ClassDB.Instantiate("SpineSkeletonDataResource").AsGodotObject() as Resource;
            var spine = ClassDB.Instantiate("SpineSprite").AsGodotObject() as Node;
            if (atlas == null || skeletonFile == null || skeletonData == null || spine == null)
            {
                Entry.Logger.Warn("[SPINE_PROTO] Failed to instantiate one or more Spine runtime objects.");
                return null;
            }

            var atlasLoadResult = atlas.Call("load_from_atlas_file", assetSet.AtlasPath);
            var skeletonLoadResult = skeletonFile.Call("load_from_file", assetSet.SkeletonPath);
            Entry.Logger.Info($"[SPINE_PROTO] load {debugLabel}: atlas={atlasLoadResult}, skeleton={skeletonLoadResult}, atlasPath={assetSet.AtlasPath}, skeletonPath={assetSet.SkeletonPath}");
            skeletonData.Set("atlas_res", atlas);
            skeletonData.Set("skeleton_file_res", skeletonFile);
            // Journey Studio exports often split attachments between the Spine
            // "default" skin and the named runtime skin (usually "Normal").
            // Setting default_skin directly to "Normal" makes attachments that
            // only live under "default" disappear, which looks like missing body
            // parts in-game. Keep default as the fallback and apply the named
            // skin on the live skeleton after the sprite is initialized.
            skeletonData.Set("default_skin", "default");
            spine.Set("skeleton_data_res", skeletonData);
            TryApplySkin(spine, assetSet, profile, debugLabel);

            var animationState = spine.Call("get_animation_state").AsGodotObject();
            if (animationState == null)
            {
                Entry.Logger.Warn($"[SPINE_PROTO] get_animation_state returned null for {debugLabel}.");
                spine.QueueFree();
                return null;
            }

            if (animationNames.Count == 0)
            {
                Entry.Logger.Warn($"[SPINE_PROTO] No animations configured for {debugLabel}.");
                spine.QueueFree();
                return null;
            }

            animationState.Call("set_animation", animationNames[0], loop && animationNames.Count == 1, 0);

            Entry.Logger.Info($"[SPINE_PROTO] Created {debugLabel} SpineSprite and started animation: {assetSet.SkeletonPath}, animations={string.Join(" -> ", animationNames)}, loop={loop}.");
            return spine;
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to create {debugLabel} SpineSprite: {ex}");
            return null;
        }
    }

    private static void TryApplySkin(Node spine, SpineAssetSet assetSet, SpineApostleProfile profile, string debugLabel)
    {
        if (string.IsNullOrWhiteSpace(profile.SkinName) || profile.SkinName == "default")
            return;

        try
        {
            if (!NamedSkinHasAttachments(assetSet.SkeletonPath, profile.SkinName))
            {
                Entry.Logger.Info($"[SPINE_PROTO] Skin {profile.SkinName} has no attachments for {debugLabel}/{profile.DisplayName}; leaving loaded default skin and setup pose untouched.");
                return;
            }

            // preview_skin is exposed by the Godot Spine extension and is harmless
            // if the runtime chooses to ignore it for non-editor contexts.
            SafeSet(spine, "preview_skin", profile.SkinName);

            var skeleton = spine.Call("get_skeleton").AsGodotObject();
            if (skeleton == null)
            {
                Entry.Logger.Warn($"[SPINE_PROTO] get_skeleton returned null while applying skin {profile.SkinName} for {debugLabel}/{profile.DisplayName}.");
                return;
            }

            skeleton.Call("set_skin_by_name", profile.SkinName);
            skeleton.Call("set_bones_to_setup_pose");
            skeleton.Call("set_slots_to_setup_pose");
            Entry.Logger.Info($"[SPINE_PROTO] Applied skin {profile.SkinName} with default fallback for {debugLabel}/{profile.DisplayName}.");
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to apply skin {profile.SkinName} for {debugLabel}/{profile.DisplayName}: {ex.Message}");
        }
    }

    private static void TrySetSkinAndSetupPose(Node spine, string skinName, SpineApostleProfile profile, string debugLabel)
    {
        try
        {
            var skeleton = spine.Call("get_skeleton").AsGodotObject();
            if (skeleton == null)
            {
                Entry.Logger.Warn($"[SPINE_PROTO] get_skeleton returned null while applying setup skin {skinName} for {debugLabel}/{profile.DisplayName}.");
                return;
            }

            skeleton.Call("set_skin_by_name", skinName);
            skeleton.Call("set_bones_to_setup_pose");
            skeleton.Call("set_slots_to_setup_pose");
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to apply setup skin {skinName} for {debugLabel}/{profile.DisplayName}: {ex.Message}");
        }
    }

    private static void TryResetSetupPose(Node spine, SpineApostleProfile profile, string debugLabel)
    {
        try
        {
            var skeleton = spine.Call("get_skeleton").AsGodotObject();
            if (skeleton == null)
            {
                Entry.Logger.Warn($"[SPINE_PROTO] get_skeleton returned null while resetting setup pose for {debugLabel}/{profile.DisplayName}.");
                return;
            }

            skeleton.Call("set_bones_to_setup_pose");
            skeleton.Call("set_slots_to_setup_pose");
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to reset setup pose for {debugLabel}/{profile.DisplayName}: {ex.Message}");
        }
    }

    private static bool NamedSkinHasAttachments(string skeletonPath, string skinName)
    {
        var metadataPath = GetSkeletonMetadataPath(skeletonPath);
        try
        {
            using var document = JsonDocument.Parse(File.ReadAllBytes(metadataPath));
            if (!document.RootElement.TryGetProperty("skins", out var skins) || skins.ValueKind != JsonValueKind.Array)
                return true;

            foreach (var skin in skins.EnumerateArray())
            {
                if (!skin.TryGetProperty("name", out var nameProperty)
                    || nameProperty.GetString() != skinName)
                    continue;

                if (!skin.TryGetProperty("attachments", out var attachments) || attachments.ValueKind != JsonValueKind.Object)
                    return false;

                foreach (var slot in attachments.EnumerateObject())
                {
                    if (slot.Value.ValueKind == JsonValueKind.Object && slot.Value.EnumerateObject().Any())
                        return true;
                }

                return false;
            }
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to inspect skin {skinName} in {metadataPath} for skeleton {skeletonPath}: {ex.Message}");
        }

        return true;
    }

    private static async Task PlayBattleSequenceAsync(Node spine, SceneTree tree, SpineAssetSet assetSet, SpineApostleProfile profile)
    {
        await PlayBattleSequenceAsync(spine, tree, assetSet, profile.BattleAnimationNames, profile.BattleAnimationDurations, profile.DisplayName);
    }

    private static async Task PlayBattleSequenceAsync(Node spine, SceneTree tree, SpineAssetSet assetSet, SecondarySpineBattleProfile profile)
    {
        await PlayBattleSequenceAsync(spine, tree, assetSet, profile.AnimationNames, profile.AnimationDurations, profile.ResourceCode);
    }

    private static async Task PlayBattleSequenceAsync(Node spine, SceneTree tree, SpineAssetSet assetSet, IReadOnlyList<string> animationNames, IReadOnlyList<float> animationDurations, string debugName)
    {
        try
        {
            for (var i = 1; i < animationNames.Count; i++)
            {
                var waitSeconds = GetSequenceSegmentSeconds(assetSet, animationNames, animationDurations, i - 1);
                await spine.ToSignal(tree.CreateTimer(waitSeconds), SceneTreeTimer.SignalName.Timeout);

                if (!GodotObject.IsInstanceValid(spine) || !spine.IsInsideTree())
                    return;

                var animationState = spine.Call("get_animation_state").AsGodotObject();
                if (animationState == null)
                {
                    Entry.Logger.Warn($"[SPINE_PROTO] battle sequence get_animation_state returned null before {animationNames[i]} for {debugName}.");
                    return;
                }

                animationState.Call("set_animation", animationNames[i], false, 0);
                Entry.Logger.Info($"[SPINE_PROTO] battle sequence switched to {animationNames[i]} for {debugName}.");
            }
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed during manual battle animation sequence: {ex}");
        }
    }

    private static bool HasCompatibleAssetSet(SpineAssetSet assetSet, SpineApostleProfile profile, string debugLabel)
    {
        foreach (var requiredPath in new[] { assetSet.SkeletonPath, assetSet.AtlasPath, assetSet.TexturePath })
        {
            if (!File.Exists(requiredPath))
            {
                Entry.Logger.Warn($"[SPINE_PROTO] Missing {debugLabel} Spine file: {requiredPath}");
                return false;
            }
        }

        return IsSkeletonVersionCompatible(assetSet.SkeletonPath);
    }

    private static bool IsSkeletonVersionCompatible(string skeletonPath)
    {
        var version = TryReadSkeletonVersion(skeletonPath);
        if (version == null || version.StartsWith(RequiredRuntimeMajorMinor, StringComparison.Ordinal))
            return true;

        if (!_warnedIncompatibleSkeleton)
        {
            _warnedIncompatibleSkeleton = true;
            Entry.Logger.Warn(
                $"[SPINE_PROTO] Disabled Spine prototype fallback: skeleton version {version} does not match STS2 Spine runtime {RequiredRuntimeMajorMinor}.x. PNG-frame VFX will be used instead."
            );
        }

        return false;
    }

    private static string? TryReadSkeletonVersion(string skeletonPath)
    {
        try
        {
            var bytes = File.ReadAllBytes(skeletonPath);
            var text = System.Text.Encoding.ASCII.GetString(bytes);
            var match = System.Text.RegularExpressions.Regex.Match(text, @"\b4\.\d+\.\d+\b");
            return match.Success ? match.Value : null;
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to read skeleton version from {skeletonPath}: {ex.Message}");
            return null;
        }
    }

    private static SpineAssetSet GetBattleAssetSet(SpineApostleProfile profile) =>
        GetAssetSet(profile.BattleResourceCode ?? profile.ResourceCode, profile.BattleCategory, profile.PreferBinaryBattleSkeleton);

    private static SpineAssetSet GetPreviewAssetSet(SpineApostleProfile profile) =>
        GetAssetSet(profile.ResourceCode, "正常使徒", profile.PreferBinaryPreviewSkeleton);

    private static SpineAssetSet GetAssetSet(SpineApostleProfile profile, string category) =>
        GetAssetSet(profile.ResourceCode, category);

    private static SpineAssetSet GetAssetSet(string resourceCode, string category, bool preferBinarySkeleton = false)
    {
        var directory = Path.Combine(SpineRoot, category, resourceCode.ToLowerInvariant());
        var jsonSkeletonPath = Path.Combine(directory, $"{resourceCode}.spine-json");
        var binarySkeletonPath = Path.Combine(directory, $"{resourceCode}.skel");
        return new SpineAssetSet(
            preferBinarySkeleton && File.Exists(binarySkeletonPath) ? binarySkeletonPath : jsonSkeletonPath,
            Path.Combine(directory, $"{resourceCode}.atlas"),
            Path.Combine(directory, $"{resourceCode}.png")
        );
    }

    private static string GetSkeletonMetadataPath(string skeletonPath)
    {
        if (skeletonPath.EndsWith(".spine-json", StringComparison.OrdinalIgnoreCase))
            return skeletonPath;

        var directory = Path.GetDirectoryName(skeletonPath);
        var resourceCode = Path.GetFileNameWithoutExtension(skeletonPath);
        if (!string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(resourceCode))
        {
            var jsonSkeletonPath = Path.Combine(directory, $"{resourceCode}.spine-json");
            if (File.Exists(jsonSkeletonPath))
                return jsonSkeletonPath;
        }

        return skeletonPath;
    }

    private static float GetTotalBattlePlaySeconds(SpineApostleProfile profile)
    {
        var total = GetAnimationSequenceSeconds(
            GetBattleAssetSet(profile),
            profile.BattleAnimationNames,
            profile.BattleAnimationDurations,
            profile.BattleDisplaySeconds
        );
        foreach (var secondary in profile.SecondaryBattleProfiles ?? [])
        {
            total = Math.Max(
                total,
                secondary.DelaySeconds + GetAnimationSequenceSeconds(
                    GetAssetSet(secondary.ResourceCode, secondary.Category),
                    secondary.AnimationNames,
                    secondary.AnimationDurations,
                    secondary.DisplaySeconds
                )
            );
        }
        return total;
    }

    private static float GetAnimationSequenceSeconds(SpineAssetSet assetSet, IReadOnlyList<string> animationNames, IReadOnlyList<float> animationDurations, float fallbackSeconds)
    {
        if (animationNames.Count == 0)
            return Math.Max(MinimumBattleAnimationSeconds, fallbackSeconds);

        var total = 0f;
        for (var i = 0; i < animationNames.Count; i++)
            total += GetSequenceSegmentSeconds(assetSet, animationNames, animationDurations, i);

        if (total <= MinimumBattleAnimationSeconds)
            total = Math.Max(MinimumBattleAnimationSeconds, fallbackSeconds);

        return total;
    }

    private static float GetSequenceSegmentSeconds(SpineAssetSet assetSet, IReadOnlyList<string> animationNames, IReadOnlyList<float> animationDurations, int index)
    {
        if (index < 0 || index >= animationNames.Count)
            return MinimumBattleAnimationSeconds;

        if (index < animationDurations.Count && animationDurations[index] > 0f)
            return Math.Max(MinimumBattleAnimationSeconds, animationDurations[index]);

        var duration = TryGetAnimationDuration(assetSet.SkeletonPath, animationNames[index]);
        return duration.HasValue
            ? Math.Max(MinimumBattleAnimationSeconds, duration.Value)
            : 0.75f;
    }

    private static float? TryGetAnimationDuration(string skeletonPath, string animationName)
    {
        var metadataPath = GetSkeletonMetadataPath(skeletonPath);
        var cacheKey = $"{metadataPath}|{animationName}";
        if (AnimationDurationCache.TryGetValue(cacheKey, out var cached))
            return cached;

        try
        {
            using var document = JsonDocument.Parse(File.ReadAllBytes(metadataPath));
            if (!document.RootElement.TryGetProperty("animations", out var animations)
                || animations.ValueKind != JsonValueKind.Object
                || !animations.TryGetProperty(animationName, out var animation))
            {
                AnimationDurationCache[cacheKey] = null;
                return null;
            }

            var maxTime = FindMaxTimelineTime(animation);
            AnimationDurationCache[cacheKey] = maxTime > 0f ? maxTime : null;
            return AnimationDurationCache[cacheKey];
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to read animation duration {animationName} from {metadataPath} for skeleton {skeletonPath}: {ex.Message}");
            AnimationDurationCache[cacheKey] = null;
            return null;
        }
    }

    private static float FindMaxTimelineTime(JsonElement element)
    {
        var maxTime = 0f;
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    if (property.NameEquals("time") && property.Value.ValueKind == JsonValueKind.Number)
                    {
                        if (property.Value.TryGetSingle(out var time))
                            maxTime = Math.Max(maxTime, time);
                    }
                    else
                    {
                        maxTime = Math.Max(maxTime, FindMaxTimelineTime(property.Value));
                    }
                }
                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                    maxTime = Math.Max(maxTime, FindMaxTimelineTime(item));
                break;
        }

        return maxTime;
    }

    private static async Task FadeOutLayerAsync(Node layer, SceneTree tree)
    {
        if (!GodotObject.IsInstanceValid(layer) || !layer.IsInsideTree())
            return;

        const int steps = 8;
        for (var i = 1; i <= steps; i++)
        {
            var alpha = 1f - i / (float)steps;
            SetModulateAlphaRecursive(layer, alpha);
            await layer.ToSignal(tree.CreateTimer(BattleFadeOutSeconds / steps), SceneTreeTimer.SignalName.Timeout);

            if (!GodotObject.IsInstanceValid(layer) || !layer.IsInsideTree())
                return;
        }
    }

    private static void SetModulateAlphaRecursive(Node node, float alpha)
    {
        if (node is CanvasItem canvasItem)
            canvasItem.Modulate = new Color(canvasItem.Modulate.R, canvasItem.Modulate.G, canvasItem.Modulate.B, alpha);

        foreach (var child in node.GetChildren())
        {
            if (child is Node childNode)
                SetModulateAlphaRecursive(childNode, alpha);
        }
    }

    private static void ConfigurePreviewNode(Node spine)
    {
        spine.Name = PreviewNodeName;
        SafeSet(spine, "mouse_filter", (int)Control.MouseFilterEnum.Ignore);
        SafeSet(spine, "z_index", 150);
        SafeSet(spine, "size", new Vector2(360f, 360f));
        SafeSet(spine, "pivot_offset", new Vector2(180f, 180f));
        SafeSet(spine, "scale", new Vector2(0.42f, 0.42f));
        SafeSet(spine, "visible", true);
    }

    private static void ConfigureBattleNode(Node spine, SceneTree tree) =>
        ConfigureBattleNode(spine, tree, Vector2.Zero, 0.34f, null, false, true);

    private static void ConfigureBattleNode(Node spine, SceneTree tree, Vector2 positionOffset, float scale, Creature? target, bool anchorsToTarget, bool faceRight)
    {
        spine.Name = "CultLeaderApostleSpineVfx";
        SafeSet(spine, "mouse_filter", (int)Control.MouseFilterEnum.Ignore);
        SafeSet(spine, "z_index", 300);
        SafeSet(spine, "size", new Vector2(800f, 800f));
        SafeSet(spine, "pivot_offset", new Vector2(400f, 400f));
        SafeSet(spine, "scale", new Vector2(faceRight ? -scale : scale, scale));
        SafeSet(spine, "visible", true);

        var viewportSize = tree.Root.GetVisibleRect().Size;
        if (viewportSize.X <= 0f || viewportSize.Y <= 0f)
            viewportSize = new Vector2(1920f, 1080f);

        var anchoredPosition = anchorsToTarget && target != null && TryGetCreatureScreenPosition(tree, target, out var targetPosition)
            ? targetPosition + positionOffset
            : GetPlayerBesidePosition(viewportSize, positionOffset);

        SafeSet(spine, "position", new Vector2(
            Math.Clamp(anchoredPosition.X, 80f, Math.Max(80f, viewportSize.X - 80f)),
            Math.Clamp(anchoredPosition.Y, 60f, Math.Max(60f, viewportSize.Y - 60f))
        ));
    }

    private static Vector2 GetPlayerBesidePosition(Vector2 viewportSize, Vector2 positionOffset)
    {
        var jitter = new Vector2(
            (float)((Random.Shared.NextDouble() * 2d - 1d) * PlayerBesideJitterRange.X),
            (float)((Random.Shared.NextDouble() * 2d - 1d) * PlayerBesideJitterRange.Y)
        );
        return new Vector2(
            viewportSize.X * PlayerLowerCenterAnchor.X + jitter.X + positionOffset.X,
            viewportSize.Y * PlayerLowerCenterAnchor.Y + jitter.Y + positionOffset.Y
        );
    }

    private static bool TryGetCreatureScreenPosition(SceneTree tree, Creature target, out Vector2 position)
    {
        position = Vector2.Zero;
        foreach (var creatureNode in FindCreatureNodes(tree.Root))
        {
            if (!GodotObject.IsInstanceValid(creatureNode) || creatureNode.Entity == null)
                continue;

            var sameCreature = ReferenceEquals(creatureNode.Entity, target)
                || creatureNode.Entity.CombatId == target.CombatId
                || creatureNode.Entity.ModelId == target.ModelId && creatureNode.Entity.SlotName == target.SlotName;
            if (!sameCreature)
                continue;

            position = creatureNode.GlobalPosition + new Vector2(creatureNode.Size.X * 0.5f, creatureNode.Size.Y * 0.62f);
            return true;
        }

        return false;
    }

    private static IEnumerable<NCreature> FindCreatureNodes(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is NCreature creature)
                yield return creature;

            if (child is Node childNode)
            {
                foreach (var descendant in FindCreatureNodes(childNode))
                    yield return descendant;
            }
        }
    }

    private static void PositionPreview(Node screen, Node preview)
    {
        var viewportSize = screen.GetViewport().GetVisibleRect().Size;
        if (viewportSize.X <= 0f || viewportSize.Y <= 0f)
            viewportSize = new Vector2(1920f, 1080f);

        var size = new Vector2(360f, 360f);

        SafeSet(preview, "position", new Vector2(
            Math.Clamp(viewportSize.X * 0.23f - size.X / 2f, 20f, Math.Max(20f, viewportSize.X - size.X - 40f)),
            Math.Clamp(viewportSize.Y * 0.66f - size.Y / 2f, 30f, Math.Max(30f, viewportSize.Y - size.Y - 60f))
        ));
    }

    private static Node? FindCurrentPreview(Node screen, SpineApostleProfile profile)
    {
        foreach (var child in screen.GetChildren())
        {
            if (child is Node node
                && node.Name == PreviewNodeName
                && node.GetMeta(PreviewCardMetaKey, string.Empty).AsString() == profile.CardTypeName
                && node.GetMeta(PreviewAliveMetaKey, false).AsBool())
            {
                return node;
            }
        }

        return null;
    }

    private static void RemoveStalePreviews(Node currentScreen, SpineApostleProfile currentProfile)
    {
        if (Engine.GetMainLoop() is not SceneTree tree || tree.Root == null)
            return;

        foreach (var node in tree.GetNodesInGroup(PreviewGroupName))
        {
            if (node is not Node preview)
                continue;

            var previewCardTypeName = preview.GetMeta(PreviewCardMetaKey, string.Empty).AsString();
            var isCurrent = preview.GetParent() == currentScreen
                && previewCardTypeName == currentProfile.CardTypeName
                && preview.GetMeta(PreviewAliveMetaKey, false).AsBool();

            if (!isCurrent)
                MarkPreviewForRemoval(preview);
        }

        RemoveNamedPreviewsRecursive(tree.Root, currentScreen, currentProfile.CardTypeName);
    }

    private static void RemoveNamedPreviewsRecursive(Node node, Node? keepParent = null, string? keepCardTypeName = null)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is Node preview && preview.Name == PreviewNodeName)
            {
                var previewCardTypeName = preview.GetMeta(PreviewCardMetaKey, string.Empty).AsString();
                var keep = keepParent != null
                    && preview.GetParent() == keepParent
                    && keepCardTypeName != null
                    && previewCardTypeName == keepCardTypeName
                    && preview.GetMeta(PreviewAliveMetaKey, false).AsBool();

                if (!keep)
                    MarkPreviewForRemoval(preview);

                continue;
            }

            if (child is Node childNode)
                RemoveNamedPreviewsRecursive(childNode, keepParent, keepCardTypeName);
        }
    }

    private static void MarkPreviewForRemoval(Node? preview)
    {
        if (preview == null || !GodotObject.IsInstanceValid(preview))
            return;

        preview.SetMeta(PreviewAliveMetaKey, false);
        preview.RemoveFromGroup(PreviewGroupName);
        SafeSet(preview, "visible", false);
        preview.QueueFree();
    }

    private static void SafeSet(GodotObject obj, string property, Variant value)
    {
        try
        {
            obj.Set(property, value);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[SPINE_PROTO] Failed to set {property} on {obj.GetClass()}: {ex.Message}");
        }
    }

    private sealed record SpineAssetSet(string SkeletonPath, string AtlasPath, string TexturePath);

    private sealed record SpineApostleProfile(
        string CardTypeName,
        string ResourceCode,
        string SkinName,
        string PreviewAnimationName,
        IReadOnlyList<string> BattleAnimationNames,
        IReadOnlyList<float> BattleAnimationDurations,
        float BattleDisplaySeconds,
        string DisplayName,
        string? BattleResourceCode = null,
        string BattleCategory = "战斗模型",
        IReadOnlyList<SecondarySpineBattleProfile>? SecondaryBattleProfiles = null,
        Vector2 BattlePositionOffset = default,
        float BattleScale = 0.34f,
        bool PreviewEnabled = true,
        bool BattleAnchorsToTarget = false,
        bool BattleFaceRight = true,
        bool PreferBinaryPreviewSkeleton = false,
        bool PreferBinaryBattleSkeleton = false
    );

    private sealed record SecondarySpineBattleProfile(
        string ResourceCode,
        string Category,
        string SkinName,
        IReadOnlyList<string> AnimationNames,
        IReadOnlyList<float> AnimationDurations,
        float DisplaySeconds,
        float DelaySeconds,
        Vector2 PositionOffset,
        float Scale,
        bool AnchorsToTarget = false,
        bool FaceRight = true
    );
}

