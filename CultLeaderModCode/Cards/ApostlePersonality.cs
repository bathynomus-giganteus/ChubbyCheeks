namespace CultLeaderMod.CultLeaderModCode.Cards;

public enum ApostlePersonality
{
    Pure,
    Calm,
    Fanatic,
    Melancholy,
    Lively
}

public interface IApostleCard
{
    ApostlePersonality Personality { get; }
}

public interface IPersonalityChoice
{
    ApostlePersonality Personality { get; }
}
