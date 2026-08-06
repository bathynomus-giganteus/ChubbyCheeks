using System.Reflection;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile => new(PortraitPath: "res://CultLeaderMod/images/card_portraits/test_add_cards.png");

    public TestAddApostleCards() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var runState = base.Owner.RunState;
        var assembly = Assembly.GetExecutingAssembly();
        var apostleCardTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ModCardTemplate))
                && t.GetCustomAttribute<RegisterCardAttribute>() != null
                && t.Name.StartsWith("Apostle_"))
            .ToList();

        if (apostleCardTypes.Count == 0)
        {
            Entry.Logger.Warn("[TestAddApostleCards] No apostle card types found!");
            return;
        }

        var createCardMethod = typeof(RunState).GetMethod("CreateCard", System.Type.EmptyTypes);
        var rng = new Random();
        var cardsToAdd = new List<CardModel>();

        for (int i = 0; i < 100; i++)
        {
            var cardType = apostleCardTypes[rng.Next(apostleCardTypes.Count)];
            try
            {
                var genericMethod = createCardMethod!.MakeGenericMethod(cardType);
                var card = (CardModel)genericMethod.Invoke(runState, [base.Owner])!;
                cardsToAdd.Add(card);
            }
            catch (Exception ex)
            {
                Entry.Logger.Error($"[TestAddApostleCards] Failed to create {cardType.Name}: {ex}");
            }
        }

        if (cardsToAdd.Count > 0)
        {
            await CardPileCmd.Add(cardsToAdd, PileType.Draw, CardPilePosition.Top, null, false);
        }
    }
}
