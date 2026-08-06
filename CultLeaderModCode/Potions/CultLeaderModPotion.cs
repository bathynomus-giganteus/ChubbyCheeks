using CultLeaderMod.CultLeaderModCode.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Potions;

[RegisterPotion(typeof(CultLeaderModPotionPool))]
public abstract class CultLeaderModPotion : ModPotionTemplate;
