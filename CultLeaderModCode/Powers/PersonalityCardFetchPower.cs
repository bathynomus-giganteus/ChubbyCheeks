using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class PersonalityCardFetchPower : ModPowerTemplate
{
    private sealed class Data
    {
        public CardTag Tag;
        public bool UpgradeFetchedCard;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_pure.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_pure.png";

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void Configure(CardTag tag, bool upgradeFetchedCard)
    {
        GetInternalData<Data>().Tag = tag;
        GetInternalData<Data>().UpgradeFetchedCard = upgradeFetchedCard;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Amount <= 0)
            return;

        var data = GetInternalData<Data>();
        var tag = data.Tag;
        var deck = PileType.Deck.GetPile(player).Cards
            .Where(card => ApostlePowerRules.IsApostleCard(card) && card.Tags.Contains(tag))
            .ToList();

        if (deck.Count == 0)
            return;

        var selected = deck[Random.Shared.Next(deck.Count)];
        if (data.UpgradeFetchedCard && selected.IsUpgradable)
            CardCmd.Upgrade(new[] { selected }, CardPreviewStyle.None);

        var combatState = player.Creature.CombatState;
        if (combatState == null)
            return;

        var combatCopy = combatState.CloneCard(selected);
        await CardPileCmd.Add(combatCopy, PileType.Hand, CardPilePosition.Top, this, false);
    }
}
