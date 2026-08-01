using BaseLib.Abstracts;
using CultLeaderMod.CultLeaderModCode.Extensions;
using Godot;

namespace CultLeaderMod.CultLeaderModCode.Character;

public class CultLeaderModCardPool : CustomCardPoolModel
{
    private static readonly Color Cream = new("fff4dc");

    public override string Title => CultLeaderMod.CharacterId; //This is not a display name.
    
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();


    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0.12f;
    public override float S => 0.08f;
    public override float V => 1.0f;
    public override Color ShaderColor => Cream;
    
    //Alternatively, leave these values at 1 and provide a custom frame image.
    /*public override Texture2D CustomFrame(CustomCardModel card)
    {
        //This will attempt to load CultLeaderMod/images/cards/frame.png
        return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
    }*/

    //Color of small card icons
    public override Color DeckEntryCardColor => Cream;
    
    public override bool IsColorless => false;
}
