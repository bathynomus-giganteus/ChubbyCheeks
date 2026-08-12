using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CultLeaderMod.CultLeaderModCode.Cards;

internal static class ApostleCardPlayHelpers
{
    public static Task ApplyPurePower(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        return ApostlePowerRules.ApplyApostlePower<RegenPower, LifeEssencePower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }

    public static Task ApplyCalmPower(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        return ApostlePowerRules.ApplyApostlePower<PlatingPower, SolidIcePower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }

    public static Task ApplyFrenzyPower(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        return ApostlePowerRules.ApplyApostlePower<VigorPower, FervorPower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }

    public static Task ApplyLivelyPower(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        return ApostlePowerRules.ApplyApostlePower<RetainHandPower, HappinessPower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }

    public static Task ApplyMelancholyPower(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false
    )
    {
        return ApostlePowerRules.ApplyApostlePower<BitterPainPower, BitterPainBurstPower>(
            choiceContext,
            target,
            amount,
            applier,
            cardSource,
            silent
        );
    }
}
