using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;
using MegaCrit.Sts2.Core.Localization;

namespace CultLeaderMod.CultLeaderModCode.Vfx;

public static class CultLeaderSettingsPage
{
    private const string PageId = "visual";
    private const string AnimationModeKey = "card_animation_mode";
    private const string GameplayUiLocTable = "gameplay_ui";
    private const string SettingsKeyPrefix = "CULT_LEADER_SETTINGS.";

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

    private static ModSettingsText Text(string key) =>
        ModSettingsText.Literal(new LocString(GameplayUiLocTable, SettingsKeyPrefix + key).GetFormattedText());
}
