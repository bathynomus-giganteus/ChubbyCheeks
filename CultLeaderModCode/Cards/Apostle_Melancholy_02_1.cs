using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_02_1 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];

    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DynamicVar("Hits", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/magic_bullet_shooter.png");

    public Apostle_Melancholy_02_1()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars["Hits"].IntValue)
            .FromCard(this, cardPlay)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var owner = base.Owner.Creature;
        var bullet = owner.GetPower<MagicBulletPower>();
        if (bullet != null && bullet.Amount > 0m)
        {
            await bullet.TriggerMagicBullet(choiceContext, target, owner, this);
            bullet = owner.GetPower<MagicBulletPower>();
        }

        if (bullet != null && bullet.Amount > 0m)
        {
            await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Random, this, false);
            return;
        }

        await ExhaustAllMagicBulletShooters(choiceContext);
        if (base.CardScope == null)
            return;

        var bomb = base.CardScope.CreateCard<Apostle_Melancholy_02_2>(base.Owner);
        await CardPileCmd.Add(bomb, PileType.Draw, CardPilePosition.Random, this, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["Hits"].UpgradeValueBy(1m);
    }

    private async Task ExhaustAllMagicBulletShooters(PlayerChoiceContext choiceContext)
    {
        var player = base.Owner;
        var pileTypes = new[] { PileType.Hand, PileType.Draw, PileType.Discard };
        foreach (var pileType in pileTypes)
        {
            var shooters = pileType.GetPile(player).Cards
                .OfType<Apostle_Melancholy_02_1>()
                .ToList();

            foreach (var shooter in shooters)
                await CardCmd.Exhaust(choiceContext, shooter);
        }

        if (base.Pile?.Type is not (PileType.Hand or PileType.Draw or PileType.Discard))
            await CardCmd.Exhaust(choiceContext, this);
    }

}
