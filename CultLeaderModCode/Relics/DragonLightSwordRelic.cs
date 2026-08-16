using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;
using MegaCrit.Sts2.Core.Entities.Cards;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class DragonLightSwordRelic : CultLeaderModRelic
{
    private const int InitialThreshold = 15;
    private const int MinThreshold = 5;

    private int _threshold = InitialThreshold;
    private int _livelyCardsThisCombat;

    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/dragon_light_sword.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/dragon_light_sword.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/dragon_light_sword.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => _livelyCardsThisCombat;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Threshold", InitialThreshold)];

    [SavedProperty]
    public int Threshold
    {
        get => _threshold;
        set
        {
            AssertMutable();
            _threshold = Math.Max(MinThreshold, value);
            SyncThresholdVar();
            InvokeDisplayAmountChanged();
        }
    }

    public override Task BeforeCombatStart()
    {
        _livelyCardsThisCombat = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (Threshold > MinThreshold)
            Threshold--;

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner || !cardPlay.Card.Tags.Contains(CultLeaderCardTags.Lively))
            return;

        _livelyCardsThisCombat++;
        InvokeDisplayAmountChanged();

        if (_livelyCardsThisCombat < Threshold)
            return;

        _livelyCardsThisCombat = 0;
        InvokeDisplayAmountChanged();
        Flash();
        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            base.Owner.Creature,
            2m,
            base.Owner.Creature,
            null
        );
    }

    private void SyncThresholdVar()
    {
        if (DynamicVars.TryGetValue("Threshold", out var thresholdVar))
            thresholdVar.BaseValue = _threshold;
    }
}
