using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class HappinessOfYongchunRelic : CultLeaderModRelic
{
    private static readonly Random _rng = new();

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsStackable => true;
    public override bool ShowCounter => false;

    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";

    public override async Task AfterObtained()
    {
        UpdateRelicDescription();
        await OfferRareCardRewards();
    }

    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        if (!GumBlessRelic.SelectionMade || GumBlessRelic.UnselectedTags == null)
            return options;

        var existingFilter = options.CardPoolFilter;
        return options.WithFilter(card =>
        {
            if (existingFilter != null && !existingFilter(card))
                return false;
            if (GumBlessRelic.IsUnselectedPersonalityCard(card))
                return _rng.NextDouble() >= 0.85;
            return true;
        });
    }

    private async Task OfferRareCardRewards()
    {
        var rewards = new List<Reward>();
        for (int i = 0; i < 2; i++)
        {
            var options = CardCreationOptions.ForNonCombatWithUniformOdds(
                [base.Owner.Character.CardPool],
                card => card.Rarity == CardRarity.Rare && card.CanBeGeneratedInCombat
            );
            rewards.Add(new CardReward(options, 3, base.Owner));
        }

        await RewardsCmd.OfferCustom(base.Owner, rewards);
    }

    private void UpdateRelicDescription()
    {
        string description = "拾起时获得2次稀有卡牌奖励。";
        if (GumBlessRelic.SelectedTags is { Count: 2 })
        {
            var names = GumBlessRelic.SelectedTags.Select(GetPersonalityName).ToList();
            description = $"{names[0]}和{names[1]}使徒的出现概率提升，拾起时获得2次稀有卡牌奖励。";
        }

        try
        {
            var relicsTable = LocManager.Instance.GetTable("relics");
            relicsTable.MergeWith(new Dictionary<string, string>
            {
                ["CULT_LEADER_MOD_RELIC_HAPPINESS_OF_YONGCHUN_RELIC.description"] = description
            });
        }
        catch (Exception ex)
        {
            Entry.Logger.Error($"[HappinessOfYongchunRelic] Failed to update relic description: {ex}");
        }
    }

    private static string GetPersonalityName(CardTag tag)
    {
        if (tag == CultLeaderCardTags.Pure) return "纯粹";
        if (tag == CultLeaderCardTags.Calm) return "冷静";
        if (tag == CultLeaderCardTags.Frenzy) return "狂热";
        if (tag == CultLeaderCardTags.Lively) return "活泼";
        if (tag == CultLeaderCardTags.Melancholy) return "忧郁";
        return "???";
    }
}
