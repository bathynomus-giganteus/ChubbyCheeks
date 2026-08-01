using CultLeaderMod.CultLeaderModCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public sealed class RandomRecruitment() :
    CultLeaderModCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override string CustomPortraitPath => "random_recruitment.png".BigCardImagePath();
    public override string PortraitPath => "random_recruitment.png".CardImagePath();
    public override string BetaPortraitPath => PortraitPath;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> apostlePool = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card is IApostleCard);

        List<CardModel> choices = CardFactory.GetDistinctForCombat(
            Owner,
            apostlePool,
            3,
            Owner.RunState.Rng.CombatCardGeneration).ToList();

        CardModel? selected = await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            choices,
            Owner,
            canSkip: false);

        if (selected is not null)
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, Owner);
    }
}
