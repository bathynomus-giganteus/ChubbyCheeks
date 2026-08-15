using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Acts;
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
        runState.CurrentActIndex >= 1 && !HasSeenEvent(runState);

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
            new EventOption(this, () => ChoosePersonality<PersonalitySelectFrenzyCard>(), ModOptionKey("PERSONALITY", "FRENZY")),
            new EventOption(this, () => ChoosePersonality<PersonalitySelectCalmCard>(), ModOptionKey("PERSONALITY", "CALM")),
            new EventOption(this, () => ChoosePersonality<PersonalitySelectMelancholyCard>(), ModOptionKey("PERSONALITY", "MELANCHOLY")),
            new EventOption(this, () => ChoosePersonality<PersonalitySelectLivelyCard>(), ModOptionKey("PERSONALITY", "LIVELY"))
        });
    }

    private async Task ChoosePersonality<T>() where T : CardModel
    {
        var card = Owner!.RunState.CreateCard<T>(Owner);
        await CardPileCmd.Add(card, PileType.Deck);
        SetEventFinished(L10NLookup(Id.Entry + ".pages.RESULT.description"));
    }
}