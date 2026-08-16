using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class SingleWeaponTicketRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/single_weapon_ticket.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/single_weapon_ticket.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/single_weapon_ticket.png";

    public override async Task AfterObtained()
    {
        var relic = RelicFactory.PullNextRelicFromFront(base.Owner).ToMutable();
        await RelicCmd.Obtain(relic, base.Owner);
        await CardPileCmd.AddCurseToDeck<Debt>(base.Owner);
    }
}
