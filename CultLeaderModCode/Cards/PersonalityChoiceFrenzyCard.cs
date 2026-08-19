using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.CardTags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class PersonalityChoiceFrenzyCard : ModCardTemplate
{
    public override bool CanBeGeneratedInCombat => false;
    protected override HashSet<CardTag> CanonicalTags => [CultLeaderCardTags.Frenzy];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile => new(PortraitPath: "res://CultLeaderMod/images/card_portraits/personality/d.png");
    public PersonalityChoiceFrenzyCard() : base(0, CardType.Skill, CardRarity.Event, TargetType.Self, false) { }
    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
