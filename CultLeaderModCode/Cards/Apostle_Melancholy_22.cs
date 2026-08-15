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
public class Apostle_Melancholy_22 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DynamicVar("StackThreshold", 5m), new DynamicVar("MaxDraw", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/rapid_cut.png");

    public Apostle_Melancholy_22()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await ApostleCardEffectHelpers.AttackAll(
            choiceContext,
            this,
            cardPlay,
            owner,
            DynamicVars.Damage.BaseValue
        );

        int debuffStacks = 0;
        foreach (var enemy in ApostleCardEffectHelpers.AliveEnemies(owner))
        {
            var debuffs = enemy.Powers
                .Where(power => power.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Debuff)
                .ToList();
            debuffStacks += debuffs.Sum(power => (int)power.Amount);

            foreach (var power in debuffs)
                await PowerCmd.Remove(power);
        }

        int draw = Math.Min(
            DynamicVars["MaxDraw"].IntValue,
            debuffStacks / DynamicVars["StackThreshold"].IntValue
        );
        if (draw > 0)
            await CardPileCmd.Draw(choiceContext, draw, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["MaxDraw"].UpgradeValueBy(2m);
    }

}
