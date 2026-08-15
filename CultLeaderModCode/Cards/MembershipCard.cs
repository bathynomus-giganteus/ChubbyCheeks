using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using VanillaMembershipCard = MegaCrit.Sts2.Core.Models.Relics.MembershipCard;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class MembershipCard : ModCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Unplayable];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/membership_card.png");

    public MembershipCard()
        : base(-1, CardType.Skill, CardRarity.Rare, TargetType.None) { }

    public override async Task BeforeRoomEntered(AbstractRoom room)
    {
        await base.BeforeRoomEntered(room);

        if (room.RoomType != RoomType.Shop || base.Pile?.Type != PileType.Deck)
            return;

        await CardPileCmd.RemoveFromDeck(this, true);

        if (IsUpgraded)
            await PlayerCmd.GainGold(30m, base.Owner, false);

        await RelicCmd.Obtain<VanillaMembershipCard>(base.Owner);
    }
}
