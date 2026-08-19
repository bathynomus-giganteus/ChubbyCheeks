using CultLeaderMod.CultLeaderModCode.Cards;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Events;

[RegisterActEvent(typeof(Hive))]
[RegisterActEvent(typeof(Glory))]
public class FortuneTellerCatCushionEvent : CultLeaderModEventBase
{
    public override EventAssetProfile AssetProfile =>
        new(InitialPortraitPath: "res://CultLeaderMod/images/events/fortune_teller_cat_cushion.png");

    public override bool IsAllowed(IRunState runState) =>
        runState.CurrentActIndex >= 1 &&
        HasCultLeaderPlayer(runState) &&
        !HasSeenEvent(runState);

    protected override PackedScene? TryCreateLayoutPackedScene()
    {
        PackedScene? baseScene = base.TryCreateLayoutPackedScene();
        if (baseScene is null)
        {
            return null;
        }

        NEventLayout? layout = baseScene.Instantiate<NEventLayout>(PackedScene.GenEditState.Disabled);
        if (layout is null)
        {
            return baseScene;
        }

        VBoxContainer? optionsContainer = layout.GetNodeOrNull<VBoxContainer>("%OptionsContainer");
        if (optionsContainer?.GetParent() is VBoxContainer column)
        {
            const float shiftUp = 90f;
            column.OffsetTop -= shiftUp;
            column.OffsetBottom -= shiftUp;
        }

        var packedScene = new PackedScene();
        try
        {
            if (packedScene.Pack(layout) == Error.Ok)
            {
                return packedScene;
            }
        }
        finally
        {
            layout.Free();
        }

        return baseScene;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var canPay = Owner is not null && Owner.Gold >= 50;
        var acceptKey = canPay ? "ACCEPT" : "ACCEPT_POOR";

        return new[]
        {
            new EventOption(this, Refuse, InitialOptionKey("REFUSE")),
            new EventOption(this, canPay ? Accept : null, InitialOptionKey(acceptKey))
        };
    }

    private Task Refuse()
    {
        SetEventFinished(L10NLookup(Id.Entry + ".pages.REFUSE.description"));
        return Task.CompletedTask;
    }

    private async Task Accept()
    {
        await PlayerCmd.LoseGold(50m, Owner!, GoldLossType.Spent);

        SetEventState(PageDescription("PERSONALITY"), new[]
        {
            new EventOption(this, () => ChoosePersonality<PersonalitySelectPureCard>(), ModOptionKey("PERSONALITY", "PURE")),
            new EventOption(this, () => ChoosePersonality<PersonalitySelectLivelyCard>(), ModOptionKey("PERSONALITY", "LIVELY")),
            new EventOption(this, () => ChoosePersonality<PersonalitySelectFrenzyCard>(), ModOptionKey("PERSONALITY", "FRENZY")),
            new EventOption(this, () => ChoosePersonality<PersonalitySelectCalmCard>(), ModOptionKey("PERSONALITY", "CALM")),
            new EventOption(this, () => ChoosePersonality<PersonalitySelectMelancholyCard>(), ModOptionKey("PERSONALITY", "MELANCHOLY"))
        });
    }

    private async Task ChoosePersonality<T>() where T : CardModel
    {
        var card = Owner!.RunState.CreateCard<T>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 1.2f, CardPreviewStyle.EventLayout);
        SetEventFinished(L10NLookup(Id.Entry + ".pages.RESULT.description"));
    }
}
