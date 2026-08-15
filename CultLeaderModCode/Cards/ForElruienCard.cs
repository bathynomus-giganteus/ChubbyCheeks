using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class ForElruienCard : ModCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Cards", 2m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/for_elruien.png");

    public ForElruienCard()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner;
        var combatState = player.Creature.CombatState;
        var scope = base.CardScope;
        if (combatState == null || scope == null)
            return;

        var count = DynamicVars["Cards"].IntValue;
        var candidates = ModelDb.AllCards
            .Where(card => card.Tags.Contains(CultLeaderCardTags.Apostle))
            .Where(card => card.CanBeGeneratedInCombat)
            .Where(card => card.Rarity == CardRarity.Rare)
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .ToList();

        foreach (var candidate in candidates)
        {
            var card = combatState.CreateCard(candidate, player);
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Top, this, false);
        }

        var owner = player.Creature;
        await PowerCmd.Apply<CultLeaderAuthorityPower>(
            choiceContext,
            owner,
            1m,
            owner,
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}