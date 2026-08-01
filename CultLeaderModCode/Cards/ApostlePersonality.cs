using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public enum ApostlePersonality { Pure, Calm, Fanatic, Melancholy, Lively }

public interface IApostleCard { ApostlePersonality Personality { get; } }
public interface IPersonalityChoice { ApostlePersonality Personality { get; } }

public static class ApostlePersonalityMap
{
    public static Type BasePowerType(ApostlePersonality p) => p switch
    {
        ApostlePersonality.Pure        => typeof(RegenPower),
        ApostlePersonality.Calm        => typeof(CultPlatedArmorPower),
        ApostlePersonality.Fanatic     => typeof(VigorPower),
        ApostlePersonality.Melancholy  => typeof(BitterPainPower),
        ApostlePersonality.Lively      => typeof(ArtifactPower),
        _ => throw new ArgumentOutOfRangeException(nameof(p))
    };

    public static Type ElderPowerType(ApostlePersonality p) => p switch
    {
        ApostlePersonality.Pure        => typeof(LifeEssencePower),
        ApostlePersonality.Calm        => typeof(FrozenFortitudePower),
        ApostlePersonality.Fanatic     => typeof(FanaticismPower),
        ApostlePersonality.Melancholy  => typeof(BitterPainBurstPower),
        ApostlePersonality.Lively      => typeof(HappinessPower),
        _ => throw new ArgumentOutOfRangeException(nameof(p))
    };
}
