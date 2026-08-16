using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class EldonLanternRelic : CultLeaderModRelic
{
    private bool _isAddingBonusHeal;

    public override RelicRarity Rarity => RelicRarity.Common;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/eldon_lantern.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/eldon_lantern.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/eldon_lantern.png";

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (_isAddingBonusHeal || creature != base.Owner.Creature || delta <= 0m)
            return;

        _isAddingBonusHeal = true;
        try
        {
            Flash();
            await CreatureCmd.Heal(creature, 1m);
        }
        finally
        {
            _isAddingBonusHeal = false;
        }
    }
}
