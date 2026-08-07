using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_09 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags => [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(0)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile => new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/我想听你讲个故事.png");

    public Apostle_Lively_09() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ArtifactPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, 1m, base.Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, base.Owner);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }}
