using BaseLib.Abstracts;
using Godot;
using BaseLib.Extensions;
using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace CultLeaderMod.CultLeaderModCode.Cards;

/// <summary>
/// This is the base class for your mod''s cards, which is set up to load the card''s images from your mod''s resources.
/// </summary>
[Pool(typeof(CultLeaderModCardPool))]
public abstract class CultLeaderModCard(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true,
    bool autoAdd = true) :
    CustomCardModel(cost, type, rarity, target, showInCardLibrary, autoAdd)
{
    public override string CustomPortraitPath => "card.png".BigCardImagePath();
    public override string PortraitPath => "card.png".CardImagePath();
    public override string BetaPortraitPath => "card.png".CardImagePath();

    /// <summary>Optional star icon shown below energy orb. Override to set per-card.</summary>
    public virtual string? StarIconPath => null;

    public override Material? CreateCustomFrameMaterial =>
        this is IApostleCard apostle ? ApostleCardVisuals.CreateFrameMaterial(apostle.Personality) : null;
}
