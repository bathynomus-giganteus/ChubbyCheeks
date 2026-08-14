using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_21 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StrengthAmt", 3m), new DynamicVar("MaxHpAmt", 20m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/超天才的演出.png");

    public Apostle_Calm_21()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var target = cardPlay.Target;
        if (target == null)
            return;

        decimal strength = DynamicVars["StrengthAmt"].BaseValue;
        decimal maxHp = DynamicVars["MaxHpAmt"].BaseValue;

        await PowerCmd.Apply<StrengthPower>(choiceContext, target, strength, owner, this);
        await PowerCmd.Apply<TempMaxHpLossPower>(choiceContext, target, maxHp, owner, this);

        foreach (var enemy in ApostleCardEffectHelpers.AliveEnemies(owner))
        {
            if (enemy == target)
                continue;
            await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -strength, owner, this);
            await PowerCmd.Apply<TempMaxHpPower>(choiceContext, enemy, maxHp, owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthAmt"].UpgradeValueBy(2m);
        DynamicVars["MaxHpAmt"].UpgradeValueBy(10m);
    }
}
