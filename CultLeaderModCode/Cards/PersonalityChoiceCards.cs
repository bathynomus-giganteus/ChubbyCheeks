using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public abstract class PersonalityChoiceCard(ApostlePersonality personality) :
    CultLeaderModCard(
        0,
        CardType.Skill,
        CardRarity.Basic,
        TargetType.Self,
        showInCardLibrary: false),
    IPersonalityChoice
{
    public ApostlePersonality Personality { get; } = personality;

    public override bool CanBeGeneratedInCombat => false;
    public override Godot.Material CreateCustomFrameMaterial => ApostleCardVisuals.CreateFrameMaterial(Personality);
    public override string CustomPortraitPath => ApostleCardVisuals.BigPortraitPath(Personality);
    public override string PortraitPath => ApostleCardVisuals.PortraitPath(Personality);
    public override string BetaPortraitPath => PortraitPath;

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

public sealed class PurePersonalityChoice() : PersonalityChoiceCard(ApostlePersonality.Pure);
public sealed class CalmPersonalityChoice() : PersonalityChoiceCard(ApostlePersonality.Calm);
public sealed class FanaticPersonalityChoice() : PersonalityChoiceCard(ApostlePersonality.Fanatic);
public sealed class LivelyPersonalityChoice() : PersonalityChoiceCard(ApostlePersonality.Lively);
public sealed class MelancholyPersonalityChoice() : PersonalityChoiceCard(ApostlePersonality.Melancholy);
