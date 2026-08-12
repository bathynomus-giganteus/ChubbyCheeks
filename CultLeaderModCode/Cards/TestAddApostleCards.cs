using System.Reflection;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
[RegisterCharacterStarterCard(typeof(CultLeaderModCharacter), 1)]
public class TestAddApostleCards : ModCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/test_add_cards.png");

    public TestAddApostleCards()
        : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner;
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            Entry.Logger.Warn("[TEST] No combat state; cannot create combat cards.");
            return;
        }

        // 10张纯粹测试卡牌
        var testCardTypes = new Type[]
        {
            typeof(Apostle_Pure_02), // 要来少女的身边吗？
            typeof(Apostle_Pure_03), // 围猎
            typeof(Apostle_Pure_04), // 休假中潜逃
            typeof(Apostle_Pure_05), // 最强的收集品
            typeof(Apostle_Pure_06), // 南瓜魔术
            typeof(Apostle_Pure_07), // 我来保护你
            typeof(Apostle_Pure_16), // 欧珀粉
            typeof(Apostle_Pure_21), // 清晰的界限
            typeof(Apostle_Pure_24), // 铁锹击
            typeof(Apostle_Pure_25), // 黄瓜油
        };

        var createCardMethod = typeof(ICombatState)
            .GetMethods()
            .First(m =>
                m.Name == "CreateCard"
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 1
            );

        var cardsToAdd = new List<CardModel>();

        foreach (var cardType in testCardTypes)
        {
            try
            {
                var genericMethod = createCardMethod.MakeGenericMethod(cardType);
                var card = (CardModel)genericMethod.Invoke(combatState, [player])!;

                // 添加升级版本
                CardCmd.Upgrade(card);

                cardsToAdd.Add(card);
            }
            catch (Exception ex)
            {
                Entry.Logger.Error($"[TEST] Failed to create {cardType.Name}: {ex.Message}");
            }
        }

        if (cardsToAdd.Count > 0)
        {
            await CardPileCmd.Add(
                cardsToAdd,
                PileType.Discard,
                CardPilePosition.Bottom,
                null,
                true
            );
        }
    }
}