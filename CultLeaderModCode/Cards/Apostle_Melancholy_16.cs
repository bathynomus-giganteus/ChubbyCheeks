using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_16 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags => [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile => new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/堕落玫瑰.png");
    public Apostle_Melancholy_16() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) { await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this); await CardPileCmd.Draw(choiceContext, 1m, base.Owner); }
}
