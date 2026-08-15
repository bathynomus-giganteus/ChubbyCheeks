using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_15 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("RetainAmt", 1m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_15.png");

    public Apostle_Lively_15()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        var owner = base.Owner.Creature;

        await ApostleCardEffectHelpers.Attack(
            choiceContext,
            this,
            cardPlay,
            cardPlay.Target,
            DynamicVars.Damage.BaseValue
        );

        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            owner,
            DynamicVars["RetainAmt"].BaseValue,
            owner,
            this
        );

        if (base.CardScope == null)
            return;

        var copy = base.CardScope.CreateCard<Apostle_Lively_15>(base.Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(copy, CardPreviewStyle.None);

        await CardPileCmd.Add(copy, PileType.Draw, CardPilePosition.Top, this, false);

        if (!IsUpgraded)
            return;

        var discardCopies = PileType.Discard
            .GetPile(base.Owner)
            .Cards
            .OfType<Apostle_Lively_15>()
            .ToList();

        if (discardCopies.Count > 0)
        {
            var randomCopy = discardCopies[Random.Shared.Next(discardCopies.Count)];
            await CardCmd.Exhaust(choiceContext, randomCopy);
        }
    }

}