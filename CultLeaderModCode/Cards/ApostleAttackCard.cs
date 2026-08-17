using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
        [new DamageVar(7m, ValueProp.Move), new BlockVar(7m, ValueProp.Move), new DynamicVar("BonusPerThree", 1m)];

    public override string? CustomPortraitPath => "res://CultLeaderMod/images/card_portraits/apostles_attack.jpg";

    public ApostleAttackCard()
        : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        int apostleCount = ApostleCardEffectHelpers.CountDeckCards(
            base.Owner,
            ApostlePowerRules.IsApostleCard);
        int bonus = (apostleCount / 3) * DynamicVars["BonusPerThree"].IntValue;
        decimal damage = DynamicVars.Damage.BaseValue + bonus;
        decimal block = DynamicVars.Block.BaseValue + bonus;

        await ApostleCardEffectHelpers.Attack(choiceContext, this, cardPlay, target, damage);
        await CreatureCmd.GainBlock(base.Owner.Creature, block, ValueProp.Move, cardPlay, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}