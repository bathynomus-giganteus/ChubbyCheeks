using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Events;

[RegisterActEvent(typeof(Overgrowth))]
[RegisterActEvent(typeof(Underdocks))]
[RegisterActEvent(typeof(Hive))]
[RegisterActEvent(typeof(Glory))]
public class MysteriousConvenienceClerkEvent : CultLeaderModEventBase
{
    public override EventAssetProfile AssetProfile =>
        new(InitialPortraitPath: "res://CultLeaderMod/images/events/mysterious_convenience_clerk.png");

    public override bool IsAllowed(IRunState runState) =>
        !HasSeenEvent(runState) &&
        HasEnoughPersonalityCards(runState, CultLeaderCardTags.Melancholy);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() => new[]
    {
        new EventOption(this, Refuse, InitialOptionKey("REFUSE")),
        new EventOption(this, Accept, InitialOptionKey("ACCEPT"))
    };

    private async Task Refuse()
    {
        await CreatureCmd.Heal(Owner!.Creature, 10m);
        SetEventFinished(L10NLookup(Id.Entry + ".pages.REFUSE.description"));
    }

    private async Task Accept()
    {
        await RelicCmd.Obtain<ZionBlackCloakRelic>(Owner!);
        SetEventFinished(L10NLookup(Id.Entry + ".pages.ACCEPT.description"));
    }
}