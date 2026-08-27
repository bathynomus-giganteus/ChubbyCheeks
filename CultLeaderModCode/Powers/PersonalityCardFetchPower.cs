using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

public class PersonalityCardFetchPower : ModPowerTemplate
{
    private sealed class Data
    {
        public bool UpgradeFetchedCard;
    }

    protected virtual CardTag FetchTag => CultLeaderCardTags.Pure;
    protected virtual string PersonalityIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_pure.png";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override string CustomIconPath => PersonalityIconPath;
    public override string CustomBigIconPath => PersonalityIconPath;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        Configure(cardSource?.IsUpgraded == true);
        await base.AfterApplied(applier, cardSource);
    }

    public void Configure(bool upgradeFetchedCard)
    {
        GetInternalData<Data>().UpgradeFetchedCard = upgradeFetchedCard;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Amount <= 0)
            return;

        var data = GetInternalData<Data>();
        var drawPile = PileType.Draw.GetPile(player).Cards
            .Where(card => ApostlePowerRules.IsApostleCard(card) && card.Tags.Contains(FetchTag))
            .ToList();

        if (drawPile.Count == 0)
            return;

        var selected = drawPile[Random.Shared.Next(drawPile.Count)];
        if (data.UpgradeFetchedCard && selected.IsUpgradable)
            CardCmd.Upgrade(new[] { selected }, CardPreviewStyle.None);

        await CardPileCmd.Add(selected, PileType.Hand, CardPilePosition.Top, this, false);
    }
}

[RegisterPower]
public class PersonalityCardFetchPurePower : PersonalityCardFetchPower
{
    protected override CardTag FetchTag => CultLeaderCardTags.Pure;
    protected override string PersonalityIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_pure.png";
}

[RegisterPower]
public class PersonalityCardFetchCalmPower : PersonalityCardFetchPower
{
    protected override CardTag FetchTag => CultLeaderCardTags.Calm;
    protected override string PersonalityIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_calm.png";
}

[RegisterPower]
public class PersonalityCardFetchFrenzyPower : PersonalityCardFetchPower
{
    protected override CardTag FetchTag => CultLeaderCardTags.Frenzy;
    protected override string PersonalityIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_frenzy.png";
}

[RegisterPower]
public class PersonalityCardFetchLivelyPower : PersonalityCardFetchPower
{
    protected override CardTag FetchTag => CultLeaderCardTags.Lively;
    protected override string PersonalityIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_lively.png";
}

[RegisterPower]
public class PersonalityCardFetchMelancholyPower : PersonalityCardFetchPower
{
    protected override CardTag FetchTag => CultLeaderCardTags.Melancholy;
    protected override string PersonalityIconPath => "res://CultLeaderMod/images/card_portraits/personality/personality_melancholy.png";
}
