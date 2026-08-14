using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Hidden tracker counting enemy attacks that were fully blocked by the owner.
/// </summary>
[RegisterPower]
public class FullBlockCounterPower : ModPowerTemplate
{
    private sealed class Data
    {
        public int Count;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;
    public override bool ShouldPlayVfx => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public int Total => GetInternalData<Data>().Count;

    public static async Task EnsureTracker(
        PlayerChoiceContext choiceContext,
        Creature owner,
        Creature? applier,
        CardModel? cardSource)
    {
        if (owner == null || owner.GetPower<FullBlockCounterPower>() != null)
            return;

        await PowerCmd.Apply<FullBlockCounterPower>(
            choiceContext,
            owner,
            1m,
            applier ?? owner,
            cardSource,
            silent: true
        );
    }

    public static int GetTotal(Creature owner)
    {
        return owner?.GetPower<FullBlockCounterPower>()?.Total ?? 0;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        await base.AfterAttack(choiceContext, command);

        if (command.Attacker == null || !command.Attacker.IsMonster)
            return;

        var hitsOnOwner = command.Results
            .SelectMany(hits => hits)
            .Where(result => result.Receiver == base.Owner)
            .ToList();

        if (hitsOnOwner.Count == 0 || !hitsOnOwner.All(result => result.WasFullyBlocked))
            return;

        GetInternalData<Data>().Count++;

        if (base.Owner.Player is { } player)
        {
            foreach (var card in PileType.Hand.GetPile(player).Cards)
            {
                NCard.FindOnTable(card)?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
            }
        }
    }
}