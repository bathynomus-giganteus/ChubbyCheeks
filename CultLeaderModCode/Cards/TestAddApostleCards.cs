using System.Reflection;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
[RegisterCharacterStarterCard(typeof(CultLeaderModCharacter), 1)]
public class TestAddApostleCards : ModCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile => new(PortraitPath: "res://CultLeaderMod/images/card_portraits/test_add_cards.png");

    public TestAddApostleCards() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var runState = base.Owner.RunState;
        var player = base.Owner;

        var apostleTypes = typeof(TestAddApostleCards).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ModCardTemplate))
                && t.GetCustomAttribute<RegisterCardAttribute>() != null
                && t.Name.StartsWith("Apostle_"))
            .ToList();

        if (apostleTypes.Count == 0) return;

        var createCardMethod = typeof(RunState).GetMethods()
            .First(m => m.Name == "CreateCard" && m.IsGenericMethodDefinition && m.GetParameters().Length == 1);

        var rng = new Random();
        var cardsToAdd = new List<CardModel>();

        for (int i = 0; i < 100; i++)
        {
            var cardType = apostleTypes[rng.Next(apostleTypes.Count)];
            try
            {
                var genericMethod = createCardMethod.MakeGenericMethod(cardType);
                var card = (CardModel)genericMethod.Invoke(runState, [player])!;
                
                // If this TEST card is upgraded, upgrade each created card
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(card);
                }
                
                cardsToAdd.Add(card);
            }
            catch (Exception ex)
            {
                Entry.Logger.Error($"[TEST] Failed to create {cardType.Name}: {ex.Message}");
            }
        }

        if (cardsToAdd.Count > 0)
        {
            await CardPileCmd.Add(cardsToAdd, PileType.Draw, CardPilePosition.Top, null, false);
        }
    }
}
