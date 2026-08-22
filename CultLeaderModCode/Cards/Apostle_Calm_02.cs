using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_02 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new DynamicVar("BuffBonus", 8m),
        ModCardVars.Computed("TotalDamage", 12m, card =>
        {
            var owner = card?.Owner?.Creature;
            var buffTypes = owner?.Powers
                ?.Where(p => p.Type == PowerType.Buff && p.IsVisible && p.Amount > 0m)
                .Select(p => p.GetType())
                .Distinct()
                .Count() ?? 0;
            var baseDamage = card?.DynamicVars["Damage"].BaseValue ?? 12m;
            var buffBonus = card?.DynamicVars["BuffBonus"].BaseValue ?? 2m;
            return baseDamage + buffTypes * buffBonus;
        })
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/百帕斯卡_挥棒.png");

    public Apostle_Calm_02()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var owner = base.Owner.Creature;
        int buffTypes = owner.Powers
            ?.Where(p => p.Type == PowerType.Buff && p.IsVisible && p.Amount > 0m)
            .Select(p => p.GetType())
            .Distinct()
            .Count() ?? 0;
        decimal damage = DynamicVars.Damage.BaseValue + buffTypes * DynamicVars["BuffBonus"].BaseValue;
        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BuffBonus"].UpgradeValueBy(4m);
    }
}
