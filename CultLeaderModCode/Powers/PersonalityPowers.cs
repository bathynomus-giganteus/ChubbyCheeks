using MegaCrit.Sts2.Core.Entities.Powers;

namespace CultLeaderMod.CultLeaderModCode.Powers;

public abstract class PersonalityPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
}

public sealed class LifeEssencePower : PersonalityPower;
public sealed class FrozenFortitudePower : PersonalityPower;
public sealed class FanaticismPower : PersonalityPower;
public sealed class PainPower : PersonalityPower;
public sealed class HappinessPower : PersonalityPower;
