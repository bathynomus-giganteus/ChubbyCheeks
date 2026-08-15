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

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_01 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(20m, ValueProp.Move), new DynamicVar("HealAmt", 5m), new DynamicVar("CardsPlayed", 1m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/软乎乎Time.png");

    public Apostle_Melancholy_01()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override bool IsPlayable
    {
        get
        {
            var tracker = base.Owner?.Creature?.GetPower<SoftTimePlayTrackerPower>();
            if (tracker == null)
                return true;
            return tracker.OtherCardsPlayed < DynamicVars["CardsPlayed"].IntValue;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await CreatureCmd.GainBlock(owner, DynamicVars.Block, cardPlay);
        await CreatureCmd.Heal(owner, DynamicVars["HealAmt"].BaseValue, true);
        PlayerCmd.EndTurn(base.Owner, canBackOut: false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CardsPlayed"].UpgradeValueBy(1m);
    }

}
