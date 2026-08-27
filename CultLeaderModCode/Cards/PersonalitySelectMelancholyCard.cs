using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class PersonalitySelectMelancholyCard : ModCardTemplate
{
    public override bool CanBeGeneratedInCombat => false;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];
    public override CardAssetProfile AssetProfile => new(PortraitPath: "res://CultLeaderMod/images/card_portraits/personality/personality_melancholy.png");
    public PersonalitySelectMelancholyCard() : base(0, CardType.Power, CardRarity.Event, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var power = await PowerCmd.Apply<PersonalityCardFetchMelancholyPower>(choiceContext, owner, 1m, owner, this);
        power?.Configure(IsUpgraded);
    }
}


