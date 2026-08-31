using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class TeamLevelSkipCard : ModCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Innate, CardKeyword.Exhaust] : [CardKeyword.Innate];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/team_level_skip.png");

    public TeamLevelSkipCard()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cards;
        if (IsUpgraded)
        {
            cards = base.Owner.PlayerCombatState?.AllCards
                .Where(card => ApostlePowerRules.IsApostleCard(card))
                ?? [];
        }
        else
        {
            cards = PileType.Hand.GetPile(base.Owner).Cards
                .Where(card => ApostlePowerRules.IsApostleCard(card));
        }

        CardCmd.Upgrade(cards.Where(card => card.IsUpgradable), CardPreviewStyle.HorizontalLayout);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(1);
        CardCmd.ApplyKeyword(this, CardKeyword.Exhaust);
    }
}
