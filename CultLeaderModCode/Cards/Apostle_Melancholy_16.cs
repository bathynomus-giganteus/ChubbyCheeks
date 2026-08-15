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

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_16 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Cards", 2m), new DynamicVar("PainBonus", 0m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/堕落玫瑰.png");

    public Apostle_Melancholy_16()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner;
        var owner = player.Creature;
        var drawPile = PileType.Draw.GetPile(player);
        int maxCards = Math.Min(DynamicVars["Cards"].IntValue, drawPile.Cards.Count);
        if (maxCards <= 0)
            return;

        var selected = await CardSelectCmd.FromCombatPile(
            choiceContext,
            drawPile,
            player,
            new CardSelectorPrefs(base.SelectionScreenPrompt, 0, maxCards)
        );

        int costSum = 0;
        foreach (var card in selected)
        {
            costSum += card.EnergyCost.GetResolved();
            await CardCmd.Exhaust(choiceContext, card);
        }

        int pain = costSum + DynamicVars["PainBonus"].IntValue;
        if (pain > 0)
        {
            await ApostleCardPlayHelpers.ApplyMelancholyPower(
                choiceContext,
                owner,
                pain,
                owner,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PainBonus"].UpgradeValueBy(2m);
    }

}
