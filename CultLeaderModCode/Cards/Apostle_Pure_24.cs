using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

/// <summary>
/// 铁锹击 — 造成最大生命值一定百分比的伤害。
/// </summary>
[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Pure_24 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("DamagePct", 20m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/铁锹击.png");

    public Apostle_Pure_24()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null) return;

        decimal pct = DynamicVars["DamagePct"].BaseValue / 100m;
        decimal dmg = Math.Floor(base.Owner.Creature.MaxHp * pct);
        if (dmg < 1) dmg = 1;

        await ApostleCardEffectHelpers.Attack(choiceContext, this, cardPlay, target, dmg);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DamagePct"].UpgradeValueBy(5m);
    }
}