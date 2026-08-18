using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class CardLibraryApostleFilterPatch
{
    private const int ModeCount = 8;

    private static readonly string[] Labels =
    [
        "筛选：全部",
        "筛选：非使徒牌",
        "筛选：全性格使徒牌",
        "筛选：纯粹使徒牌",
        "筛选：狂热使徒牌",
        "筛选：冷静使徒牌",
        "筛选：活泼使徒牌",
        "筛选：忧郁使徒牌",
    ];

    private static readonly Func<CardModel, bool>[] Predicates =
    [
        _ => true,
        card => !ApostlePowerRules.IsApostleCard(card),
        card => card is TestRainbowCard,
        card => IsApostleWithTag(card, CultLeaderCardTags.Pure),
        card => IsApostleWithTag(card, CultLeaderCardTags.Frenzy),
        card => IsApostleWithTag(card, CultLeaderCardTags.Calm),
        card => IsApostleWithTag(card, CultLeaderCardTags.Lively),
        card => IsApostleWithTag(card, CultLeaderCardTags.Melancholy),
    ];

    private static readonly ConditionalWeakTable<NCardLibrary, FilterState> States = new();
    private static readonly Dictionary<NButton, FilterState> ButtonStates = new();

    [HarmonyPatch(typeof(NCardLibrary), "_Ready")]
    [HarmonyPostfix]
    private static void AddFilterButton(NCardLibrary __instance)
    {
        try
        {
            var template = __instance.GetNodeOrNull<NCardViewSortButton>("%AlphabetSorter");
            if (template == null)
                return;

            if (template.Duplicate() is not NCardViewSortButton duplicate)
                return;

            duplicate.Name = "CultLeaderApostleFilterButton";
            duplicate.Visible = true;
            duplicate.ZIndex = template.ZIndex + 1;
            duplicate.Size = new Vector2(320f, template.Size.Y);
            duplicate.Scale = template.Scale;

            __instance.AddChild(duplicate);

            duplicate.AnchorLeft = 0f;
            duplicate.AnchorTop = 1f;
            duplicate.AnchorRight = 0f;
            duplicate.AnchorBottom = 1f;
            duplicate.GrowHorizontal = Control.GrowDirection.End;
            duplicate.GrowVertical = Control.GrowDirection.Begin;
            duplicate.OffsetLeft = -8f;
            duplicate.OffsetTop = -204f;
            duplicate.OffsetRight = 312f;
            duplicate.OffsetBottom = -148f;

            var sortIcon = duplicate.FindChild("Image", true, false) as TextureRect;
            if (sortIcon != null)
                sortIcon.Visible = false;

            duplicate.SetLabel(Labels[0]);
            duplicate.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnFilterButtonPressed));

            var menu = new PopupMenu
            {
                Name = "CultLeaderApostleFilterMenu",
                Theme = __instance.Theme,
                HideOnItemSelection = true,
            };
            for (var i = 0; i < Labels.Length; i++)
                menu.AddItem(Labels[i], i);
            duplicate.AddChild(menu);

            var state = new FilterState(__instance, duplicate, menu, 0);
            States.Add(__instance, state);
            ButtonStates[duplicate] = state;
            menu.Connect(PopupMenu.SignalName.IdPressed, Callable.From<long>(id => OnFilterMenuIdPressed(state, id)));

            Entry.Logger.Info("[CardLibraryApostleFilterPatch] Apostle card filter button injected.");
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CardLibraryApostleFilterPatch] Failed to add apostle filter button: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(NCardLibrary), "UpdateFilter")]
    [HarmonyPostfix]
    private static void ApplyApostleFilter(NCardLibrary __instance)
    {
        try
        {
            if (!States.TryGetValue(__instance, out var state))
                return;

            var field = AccessTools.Field(typeof(NCardLibrary), "_filter");
            if (field == null || field.GetValue(__instance) is not Func<CardModel, bool> baseFilter)
                return;

            var apostleFilter = Predicates[state.Index];
            field.SetValue(__instance, (Func<CardModel, bool>)(card => baseFilter(card) && apostleFilter(card)));
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CardLibraryApostleFilterPatch] Failed to apply apostle filter: {ex.Message}");
        }
    }

    private static void OnFilterButtonPressed(NButton button)
    {
        try
        {
            if (!ButtonStates.TryGetValue(button, out var state))
                return;

            ShowFilterMenu(state);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CardLibraryApostleFilterPatch] Failed to show apostle filter menu: {ex.Message}");
        }
    }

    private static void OnFilterMenuIdPressed(FilterState state, long id)
    {
        try
        {
            var index = (int)id;
            if (index < 0 || index >= ModeCount)
                return;

            state.Index = index;
            state.Button.SetLabel(Labels[state.Index]);
            InvokeUpdateFilter(state.Library);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CardLibraryApostleFilterPatch] Failed to apply selected apostle filter: {ex.Message}");
        }
    }

    private static void ShowFilterMenu(FilterState state)
    {
        const int menuItemHeight = 40;
        const int menuWidth = 320;

        var buttonPosition = state.Button.GlobalPosition;
        var buttonSize = state.Button.Size;
        var viewportSize = state.Button.GetViewportRect().Size;
        var menuHeight = Labels.Length * menuItemHeight;

        var x = Mathf.RoundToInt(buttonPosition.X + buttonSize.X + 8f);
        var desiredY = Mathf.RoundToInt(buttonPosition.Y);
        var maxY = Mathf.Max(0, Mathf.RoundToInt(viewportSize.Y - menuHeight));
        var y = Math.Clamp(desiredY, 0, maxY);

        state.Menu.Size = new Vector2I(menuWidth, menuHeight);
        state.Menu.Position = new Vector2I(x, y);
        state.Menu.Popup();
    }

    private static void InvokeUpdateFilter(NCardLibrary library)
    {
        var method = typeof(NCardLibrary).GetMethod("UpdateFilter", BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(library, new object[] { false });
    }

    private static bool IsApostleWithTag(CardModel card, CardTag tag)
    {
        return ApostlePowerRules.IsApostleCard(card) && card.Tags?.Contains(tag) == true;
    }

    private sealed class FilterState
    {
        public FilterState(NCardLibrary library, NCardViewSortButton button, PopupMenu menu, int index)
        {
            Library = library;
            Button = button;
            Menu = menu;
            Index = index;
        }

        public NCardLibrary Library { get; }
        public NCardViewSortButton Button { get; }
        public PopupMenu Menu { get; }
        public int Index { get; set; }
    }
}

