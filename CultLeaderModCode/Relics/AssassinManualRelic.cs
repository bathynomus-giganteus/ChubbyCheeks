using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class AssassinManualRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/assassin_manual.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/assassin_manual.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/assassin_manual.png";

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<BufferPower>(
            new ThrowingPlayerChoiceContext(),
            base.Owner.Creature,
            1m,
            base.Owner.Creature,
            null
        );
    }
}
