using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
[RegisterCharacterStarterCard(typeof(CultLeaderModCharacter), 1)]
public class ApostleAttackCard : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(7m),
            new ExtraDamageVar(1m),
            new CalculationExtraVar(1m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => CountApostleTrios(card)),
            new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => CountApostleTrios(card))
        ];

    public override string? CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/apostles_attack.jpg";

    public ApostleAttackCard()
        : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        decimal damage = DynamicVars.CalculatedDamage.Calculate(target);
        decimal block = DynamicVars.CalculatedBlock.Calculate(target);

        await ApostleCardEffectHelpers.Attack(choiceContext, this, cardPlay, target, damage);
        await CreatureCmd.GainBlock(base.Owner.Creature, block, DynamicVars.CalculatedBlock.Props, cardPlay, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
    }

    private static decimal CountApostleTrios(CardModel card)
    {
        return ApostleCardEffectHelpers.CountDeckCards(card.Owner, ApostlePowerRules.IsApostleCard) / 3;
    }
}