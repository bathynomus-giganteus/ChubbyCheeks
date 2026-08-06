using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Character;

[RegisterCharacter]
public class CultLeaderModCharacter : ModCharacterTemplate<CultLeaderModCardPool, CultLeaderModRelicPool, CultLeaderModPotionPool>
{
    public const string CharacterId = "CultLeaderMod";
    public static readonly Color Color = new("d6a84b");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    public override int StartingGold => 99;
    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    public override string CustomVisualsPath => "res://CultLeaderMod/images/charui/avatar_full.png";
    public override string CustomIconPath => "res://CultLeaderMod/scenes/char_icon.tscn";
    public override string CustomIconTexturePath => "res://CultLeaderMod/images/charui/character_icon_char_name.png";
    public override string CustomCharacterSelectBgPath => "res://CultLeaderMod/scenes/char_select_bg.tscn";
    public override string CustomCharacterSelectIconPath => "res://CultLeaderMod/images/charui/char_select_char_name.png";
    public override string CustomCharacterSelectLockedIconPath => "res://CultLeaderMod/images/charui/char_select_char_name.png";
    public override string CustomMapMarkerPath => "res://CultLeaderMod/images/charui/map_marker_char_name.png";

    public override List<string> GetArchitectAttackVfx()
    {
        return ["vfx/vfx_attack_blunt", "vfx/vfx_heavy_blunt", "vfx/vfx_attack_slash", "vfx/vfx_bloody_impact", "vfx/vfx_rock_shatter"];
    }
}
