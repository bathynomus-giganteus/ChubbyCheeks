using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Events;

public abstract class CultLeaderModEventBase : ModEventTemplate
{
    private static readonly object LayoutSceneLock = new();
    private static PackedScene? _sharedEventLayoutScene;

    protected bool HasSeenEvent(IRunState runState)
    {
        return runState.MapPointHistory
            .SelectMany(act => act)
            .SelectMany(entry => entry.Rooms)
            .Any(room => room.RoomType == RoomType.Event && room.ModelId == Id);
    }

    protected bool HasEnoughPersonalityCards(IRunState runState, CardTag personalityTag)
    {
        return runState.Players.Any(player =>
            player.Character is CultLeaderModCharacter &&
            player.Deck.Cards.Count(card => card.Tags.Contains(personalityTag)) >= 5);
    }

    protected bool HasCultLeaderPlayer(IRunState runState)
    {
        return runState.Players.Any(player => player.Character is CultLeaderModCharacter);
    }

    protected override PackedScene? TryCreateLayoutPackedScene()
    {
        if (_sharedEventLayoutScene is not null)
        {
            return _sharedEventLayoutScene;
        }

        lock (LayoutSceneLock)
        {
            if (_sharedEventLayoutScene is not null)
            {
                return _sharedEventLayoutScene;
            }

            PackedScene? defaultScene = GD.Load<PackedScene>(NEventLayout.defaultScenePath);
            if (defaultScene is null)
            {
                return null;
            }

            NEventLayout? layout = defaultScene.Instantiate<NEventLayout>(PackedScene.GenEditState.Disabled);
            if (layout is null)
            {
                return null;
            }

            TextureRect? portrait = layout.GetNodeOrNull<TextureRect>("%Portrait");
            if (portrait is not null)
            {
                const float portraitWidth = 800f;
                const float portraitHeight = 360f;
                const float leftMargin = 70f;

                portrait.AnchorLeft = 0f;
                portrait.AnchorTop = 0.5f;
                portrait.AnchorRight = 0f;
                portrait.AnchorBottom = 0.5f;
                portrait.OffsetLeft = leftMargin;
                portrait.OffsetTop = -portraitHeight * 0.5f;
                portrait.OffsetRight = leftMargin + portraitWidth;
                portrait.OffsetBottom = portraitHeight * 0.5f;
                portrait.GrowHorizontal = Control.GrowDirection.End;
                portrait.GrowVertical = Control.GrowDirection.Both;
                portrait.ExpandMode = (TextureRect.ExpandModeEnum)1;
                portrait.StretchMode = (TextureRect.StretchModeEnum)5;
                portrait.Scale = Vector2.One;
            }

            var packedScene = new PackedScene();
            try
            {
                if (packedScene.Pack(layout) != Error.Ok)
                {
                    return null;
                }

                _sharedEventLayoutScene = packedScene;
                return packedScene;
            }
            finally
            {
                layout.Free();
            }
        }
    }
}
