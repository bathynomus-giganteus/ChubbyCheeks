using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Frenzy_09 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", 4m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/鹿派斩击.png");

    public Apostle_Frenzy_09()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        var attack = await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars["Hits"].IntValue)
            .FromCard(this, cardPlay)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var totalDamage = attack.Results
            .SelectMany(hit => hit)
            .Where(result => result.Receiver == target)
            .Sum(result => Math.Max(0m, result.TotalDamage));

        if (totalDamage > 0m)
            await ApostleCardPlayHelpers.ApplyFrenzyPower(
                choiceContext,
                base.Owner.Creature,
                totalDamage,
                base.Owner.Creature,
                this
            );

        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Random, this, false);
    }

    protected override void OnUpgrade() { }
}
