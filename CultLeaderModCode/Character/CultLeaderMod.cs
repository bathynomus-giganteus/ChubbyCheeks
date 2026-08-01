using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using CultLeaderMod.CultLeaderModCode.Relics;

namespace CultLeaderMod.CultLeaderModCode.Character;

public class CultLeaderMod : PlaceholderCharacterModel
{
    public const string CharacterId = "CultLeaderMod";
    
    // Warm gold is the visual identity for the Cult Leader's card pool and UI.
    public static readonly Color Color = new("d6a84b");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<CultLeaderStrike>(),
        ModelDb.Card<CultLeaderStrike>(),
        ModelDb.Card<CultLeaderStrike>(),
        ModelDb.Card<CultLeaderStrike>(),
        ModelDb.Card<CultLeaderDefend>(),
        ModelDb.Card<CultLeaderDefend>(),
        ModelDb.Card<CultLeaderDefend>(),
        ModelDb.Card<CultLeaderDefend>(),
        ModelDb.Card<RandomRecruitment>(),
        ModelDb.Card<CultLeaderManifestation>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<CultLeaderStartingRelic>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<CultLeaderModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<CultLeaderModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<CultLeaderModPotionPool>();

    // Temporary static artwork used in combat and on the character-select screen.
    public override string CustomVisualPath =>
        "res://CultLeaderMod/scenes/characters/cult_leader.tscn";
    public override string CustomCharacterSelectBg =>
        "res://CultLeaderMod/scenes/screens/char_select_bg_cult_leader.tscn";
    public override string CustomMerchantAnimPath =>
        "res://CultLeaderMod/scenes/characters/cult_leader_merchant.tscn";
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
