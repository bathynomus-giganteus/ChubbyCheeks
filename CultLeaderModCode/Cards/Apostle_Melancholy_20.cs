using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using CultLeaderMod.CultLeaderModCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_20 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(
            PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/神王zhi尊_希菲尔攻击.png"
        );

    public Apostle_Melancholy_20()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await PowerCmd.Apply<SkyRulerPower>(
            choiceContext,
            owner,
            1m,
            owner,
            this
        );

        foreach (var enemy in ApostleCardEffectHelpers.AliveEnemies(owner))
        {
            if (enemy.GetPower<DebilitatePower>() != null)
                continue;

            await PowerCmd.Apply<DebilitatePower>(
                choiceContext,
                enemy,
                1m,
                owner,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

