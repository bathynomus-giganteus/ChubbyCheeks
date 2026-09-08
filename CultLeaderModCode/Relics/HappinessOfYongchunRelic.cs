using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class HappinessOfYongchunRelic : CultLeaderModRelic
{
    [SavedProperty]
    public int PersonalityMask { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsStackable => true;
    public override bool ShowCounter => false;

    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";

    public override async Task AfterObtained()
    {
        if (PersonalityMask == 0 && GumBlessRelic.GetSelectedTags(Owner) is { Count: 2 } selected)
            PersonalityMask = GumBlessRelic.EncodeSelection(selected);
        await OfferRareCardRewards();
    }

    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        if (player != Owner || !GumBlessRelic.HasSelection(player))
            return options;

        var existingFilter = options.CardPoolFilter;
        return options.WithFilter(card =>
        {
            if (existingFilter != null && !existingFilter(card))
                return false;
            if (GumBlessRelic.IsUnselectedPersonalityCard(card, player))
                return player.PlayerRng.Rewards.NextDouble() >= 0.85;
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

}
