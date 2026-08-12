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

        // 13张测试卡牌（未升级）：10张新卡 + 3张基础再生卡
        var testCardTypes = new Type[]
        {
            // 基础再生提供
            typeof(Apostle_Pure_06), // 南瓜魔术
            typeof(Apostle_Pure_07), // 我来保护你
            typeof(Apostle_Pure_25), // 黄瓜油
            // 新测试卡牌
            typeof(Apostle_Pure_08), // 调皮的笑容
            typeof(Apostle_Pure_09), // 玛戈玛恢复
            typeof(Apostle_Pure_10), // 魔女档案
            typeof(Apostle_Pure_11), // 谢绝(Non grata)
            typeof(Apostle_Pure_12), // 接受水的洗礼吧！
            typeof(Apostle_Pure_13), // 汁液泵机发射！
            typeof(Apostle_Pure_14), // 钻石穿刺
            typeof(Apostle_Pure_15), // 快躲开啊啊!!!噫…?
            typeof(Apostle_Pure_17), // 远程充电
            typeof(Apostle_Pure_18), // 突发惊吓
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