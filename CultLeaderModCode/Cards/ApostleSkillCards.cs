using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Extensions;
using CultLeaderMod.CultLeaderModCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public abstract class ApostleTestCard<TPower>(ApostlePersonality personality, CardRarity rarity) :
    CultLeaderModCard(0, CardType.Skill, rarity, TargetType.Self), IApostleCard
    where TPower : PowerModel
{
    public ApostlePersonality Personality { get; } = personality;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override Material CreateCustomFrameMaterial => ApostleCardVisuals.CreateFrameMaterial(Personality);
    public override string CustomPortraitPath => ApostleCardVisuals.BigPortraitPath(Personality);
    public override string PortraitPath => ApostleCardVisuals.PortraitPath(Personality);
    public override string BetaPortraitPath => PortraitPath;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<TPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
}

public static class ApostleCardVisuals
{
    public static Material CreateFrameMaterial(ApostlePersonality personality)
    {
        Color color = personality switch
        {
            ApostlePersonality.Pure => new Color("35b84a"),
            ApostlePersonality.Calm => new Color("2d8cff"),
            ApostlePersonality.Fanatic => new Color("e33b3b"),
            ApostlePersonality.Lively => new Color("f2c230"),
            ApostlePersonality.Melancholy => new Color("8b5ce6"),
            _ => Colors.White
        };

        return ShaderUtils.GenerateHsv(color.H, color.S, color.V);
    }

    public static string PortraitPath(ApostlePersonality personality) =>
        $"{PersonalityFileName(personality)}.png".CardImagePath();

    public static string BigPortraitPath(ApostlePersonality personality) =>
        $"{PersonalityFileName(personality)}.png".BigCardImagePath();

    private static string PersonalityFileName(ApostlePersonality personality) => personality switch
    {
        ApostlePersonality.Pure => "pure_apostle",
        ApostlePersonality.Calm => "calm_apostle",
        ApostlePersonality.Fanatic => "fanatic_apostle",
        ApostlePersonality.Lively => "lively_apostle",
        ApostlePersonality.Melancholy => "melancholy_apostle",
        _ => "card"
    };
}
