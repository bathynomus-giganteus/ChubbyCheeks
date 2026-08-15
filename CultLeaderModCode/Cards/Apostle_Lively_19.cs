using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_19 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainAmt", 2m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_19.png");

    public Apostle_Lively_19()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var hand = PileType.Hand.GetPile(base.Owner).Cards.Where(c => c != this).ToList();
        if (hand.Count == 0)
            return;

        var tracker = owner.GetPower<RookieCardPower>();
        if (tracker == null)
        {
            await PowerCmd.Apply<RookieCardPower>(
                choiceContext,
                owner,
                DynamicVars["RetainAmt"].BaseValue,
                owner,
                this
            );
            tracker = owner.GetPower<RookieCardPower>();
        }

        if (tracker == null)
            return;

        foreach (var card in hand)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
            tracker.MarkCard(card);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}