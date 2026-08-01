using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public sealed class CultLeaderStrike() :
    CultLeaderModCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    public override bool IsBasicStrikeOrDefend => true;
    public override string CustomPortraitPath => "cult_leader_strike.png".BigCardImagePath();
    public override string PortraitPath => "cult_leader_strike.png".CardImagePath();
    public override string BetaPortraitPath => PortraitPath;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
