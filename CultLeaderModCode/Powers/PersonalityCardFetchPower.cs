using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;
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
        public string IconPath = "res://CultLeaderMod/images/card_portraits/personality/personality_pure.png";
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => GetInternalData<Data>().IconPath;
    public override string CustomBigIconPath => GetInternalData<Data>().IconPath;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void Configure(CardTag tag, bool upgradeFetchedCard)
    {
        var data = GetInternalData<Data>();
        data.Tag = tag;
        data.UpgradeFetchedCard = upgradeFetchedCard;
        data.IconPath = tag == CultLeaderCardTags.Pure ? "res://CultLeaderMod/images/card_portraits/personality/personality_pure.png"
            : tag == CultLeaderCardTags.Calm ? "res://CultLeaderMod/images/card_portraits/personality/personality_calm.png"
            : tag == CultLeaderCardTags.Frenzy ? "res://CultLeaderMod/images/card_portraits/personality/personality_frenzy.png"
            : tag == CultLeaderCardTags.Lively ? "res://CultLeaderMod/images/card_portraits/personality/personality_lively.png"
            : tag == CultLeaderCardTags.Melancholy ? "res://CultLeaderMod/images/card_portraits/personality/personality_melancholy.png"
            : "res://CultLeaderMod/images/card_portraits/personality/personality_pure.png";
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Amount <= 0)
            return;

        var data = GetInternalData<Data>();
        var tag = data.Tag;
        var drawPile = PileType.Draw.GetPile(player).Cards
            .Where(card => ApostlePowerRules.IsApostleCard(card) && card.Tags.Contains(tag))
            .ToList();

        if (drawPile.Count == 0)
            return;

        var selected = drawPile[Random.Shared.Next(drawPile.Count)];
        if (data.UpgradeFetchedCard && selected.IsUpgradable)
            CardCmd.Upgrade(new[] { selected }, CardPreviewStyle.None);

        await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Top, this, false);
    }
}
