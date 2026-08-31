using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;
using MegaCrit.Sts2.Core.Localization;

namespace CultLeaderMod.CultLeaderModCode.Vfx;

public static class CultLeaderSettingsPage
{
    private const string PageId = "visual";
    private const string AnimationModeKey = "card_animation_mode";

    public static void Register()
    {
        try
        {
            var animationModeBinding = ModSettingsBindings.Callback(
                Entry.ModId,
                AnimationModeKey,
                () => CultLeaderAnimationSettings.Mode,
                value => CultLeaderAnimationSettings.Mode = value,
                CultLeaderAnimationSettings.Save,
                SaveScope.Global
            );

            RitsuLibFramework.RegisterModSettings(Entry.ModId, page =>
            {
                page.WithModDisplayName(Text("modDisplayName"))
                    .WithTitle(Text("title"))
                    .WithDescription(Text("description"))
                    .AddSection("visuals", section =>
                    {
                        section
                            .WithTitle(Text("visualsTitle"))
                            .WithDescription(Text("visualsDescription"))
                            .AddChoice(
                                AnimationModeKey,
                                Text("cardAnimations"),
                                animationModeBinding,
                                new[]
                                {
                                    new ModSettingsChoiceOption<CardAnimationMode>(
                                        CardAnimationMode.Off,
                                        Text("off")
                                    ),
                                    new ModSettingsChoiceOption<CardAnimationMode>(
                                        CardAnimationMode.RareOnly,
                                        Text("rareOnly")
                                    ),
                                    new ModSettingsChoiceOption<CardAnimationMode>(
                                        CardAnimationMode.Full,
                                        Text("full")
                                    ),
                                },
                                Text("cardAnimationsDescription"),
                                ModSettingsChoicePresentation.Dropdown
                            );
                    });
            }, PageId);

            Entry.Logger.Info("[CultLeaderSettingsPage] Settings page registered.");
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CultLeaderSettingsPage] Failed to register settings page: {ex}");
        }
    }

    private static ModSettingsText Text(string key) => ModSettingsText.Literal(GetLocalizedText(key));

    private static string GetLocalizedText(string key)
    {
        var language = "eng";
        try
        {
            language = LocManager.Instance?.Language ?? "eng";
        }
        catch
        {
            // Settings may be registered before LocManager is fully available.
        }

        var normalizedLanguage = NormalizeLanguage(language);
        return Strings.TryGetValue(normalizedLanguage, out var table) && table.TryGetValue(key, out var value)
            ? value
            : Strings["eng"][key];
    }

    private static string NormalizeLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return "eng";

        var normalized = language.Trim().Replace('-', '_').ToLowerInvariant();
        return normalized switch
        {
            "zhs" or "zh" or "zh_cn" or "zh_hans" or "chs" or "chinese" or "simplified_chinese" => "zhs",
            "eng" or "en" or "en_us" or "en_gb" or "english" => "eng",
            "jpn" or "ja" or "jp" or "ja_jp" or "japanese" => "jpn",
            "kor" or "ko" or "kr" or "ko_kr" or "korean" => "kor",
            _ => normalized,
        };
    }

    private static readonly Dictionary<string, Dictionary<string, string>> Strings = new()
    {
        ["zhs"] = new()
        {
            ["modDisplayName"] = "教主",
            ["title"] = "教主 Mod 设置",
            ["description"] = "调整教主 Mod 的显示与性能相关选项。",
            ["visualsTitle"] = "视觉效果",
            ["visualsDescription"] = "卡牌动画使用外部资源读取。若机器较慢或想减少干扰，可以在这里降低动画显示范围。",
            ["cardAnimations"] = "卡牌动画",
            ["off"] = "完全关闭",
            ["rareOnly"] = "仅保留稀有卡",
            ["full"] = "完全保留",
            ["cardAnimationsDescription"] = "控制使徒牌打出时的战斗动画，以及卡牌查看页/百科中的使徒立绘动画。",
        },
        ["eng"] = new()
        {
            ["modDisplayName"] = "Cult Leader",
            ["title"] = "Cult Leader Mod Settings",
            ["description"] = "Adjust display and performance-related options for the Cult Leader Mod.",
            ["visualsTitle"] = "Visual Effects",
            ["visualsDescription"] = "Card animations use external resources. If your machine is slow or you want less visual clutter, reduce the animation display range here.",
            ["cardAnimations"] = "Card Animations",
            ["off"] = "Off",
            ["rareOnly"] = "Rare cards only",
            ["full"] = "Full",
            ["cardAnimationsDescription"] = "Controls apostle battle animations when cards are played, as well as apostle portrait animations on the card inspection and encyclopedia screens.",
        },
        ["jpn"] = new()
        {
            ["modDisplayName"] = "教主",
            ["title"] = "教主Mod設定",
            ["description"] = "教主Modの表示とパフォーマンス関連オプションを調整します。",
            ["visualsTitle"] = "視覚効果",
            ["visualsDescription"] = "カードアニメーションは外部リソースを使用します。動作が重い場合や演出を抑えたい場合は、ここで表示範囲を下げてください。",
            ["cardAnimations"] = "カードアニメーション",
            ["off"] = "完全にオフ",
            ["rareOnly"] = "レアカードのみ",
            ["full"] = "すべて表示",
            ["cardAnimationsDescription"] = "カード使用時の使徒戦闘アニメーション、およびカード確認画面/百科画面の使徒立ち絵アニメーションを制御します。",
        },
        ["kor"] = new()
        {
            ["modDisplayName"] = "교주",
            ["title"] = "교주 모드 설정",
            ["description"] = "교주 모드의 표시 및 성능 관련 옵션을 조정합니다.",
            ["visualsTitle"] = "시각 효과",
            ["visualsDescription"] = "카드 애니메이션은 외부 리소스를 사용합니다. 컴퓨터가 느리거나 연출을 줄이고 싶다면 여기에서 표시 범위를 낮출 수 있습니다.",
            ["cardAnimations"] = "카드 애니메이션",
            ["off"] = "완전히 끄기",
            ["rareOnly"] = "희귀 카드만 유지",
            ["full"] = "모두 유지",
            ["cardAnimationsDescription"] = "카드를 사용할 때 재생되는 사도 전투 애니메이션과 카드 확인/백과 화면의 사도 일러스트 애니메이션을 제어합니다.",
        },
    };
}
