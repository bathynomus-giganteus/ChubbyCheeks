using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Pure_23 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("StrLoss", 15m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/投降投降了啦.png");

    public Apostle_Pure_23()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var amount = DynamicVars["StrLoss"].BaseValue;
        await PowerCmd.Apply<StrengthPower>(choiceContext, owner, -amount, owner, this);
        await PowerCmd.Apply<TempStrengthLossPower>(choiceContext, owner, amount, owner, this);
        var enemies = owner.CombatState?.GetCreaturesOnSide(CombatSide.Enemy) ?? [];
        foreach (var enemy in enemies)
        {
            if (!enemy.IsDead)
                await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -amount, owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrLoss"].UpgradeValueBy(5m);
    }
}

