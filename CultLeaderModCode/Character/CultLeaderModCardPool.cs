using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Character;

public class CultLeaderModCardPool : TypeListCardPoolModel
{
    protected override IEnumerable<CardModel> FilterThroughEpochs(
        UnlockState unlockState,
        IEnumerable<CardModel> cards)
    {
        return base.FilterThroughEpochs(unlockState, cards)
            .Where(card => card.CanBeGeneratedInCombat);
    }

    public override string Title => "CultLeaderMod";
    public override string EnergyColorName => "CultLeaderMod";
    public override string? BigEnergyIconPath => "res://CultLeaderMod/images/charui/energy_big.png";
    public override string? TextEnergyIconPath => "res://CultLeaderMod/images/charui/text_energy.png";
    public override Color DeckEntryCardColor => new("fff4dc");
    public override Color EnergyOutlineColor => new("5c3d0e");
    public override bool IsColorless => false;
}
