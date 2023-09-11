using System.Collections.Generic;

namespace amongus3902.ContentMetadata
{
    public enum ZeldaSound
    {
        //music
        DungeonMusic,
        DungeonDrip,

        //projectiles
        ArrowShoot,
        BoomerangFly,
        BombDrop,
        BombBlow,
        Candle,
        SwordSwing,
        SwordBeam,
        Block,

        //enemies/player
        LinkHurt,
        LinkDie,
        EnemyHurt,
        EnemyDie,
        BossScream,
        LowHealth,

        //jingles
        NewItemFanfare,
        SecretFind,
        KeyAppear,

        //pickups
        GetHeart,
        GetItem,
        GetRupee,
        RefillLoop,

        //environmental
        DoorUnlock,
        Stairs,
        TextLoop
    }

    internal class SoundData
    {
        public readonly static List<(
            ZeldaSound sound,
            string fileName,
            bool looping
        )> SoundDetailsRaw =
            new()
            {
                (ZeldaSound.DungeonMusic, "Sounds/DungeonMusic", true),
                (ZeldaSound.DungeonDrip, "Sounds/DungeonDrip", true),
                (ZeldaSound.ArrowShoot, "Sounds/LOZ_Arrow_Boomerang", false),
                (ZeldaSound.BoomerangFly, "Sounds/LOZ_Arrow_Boomerang", true),
                (ZeldaSound.BombDrop, "Sounds/LOZ_Bomb_Drop", false),
                (ZeldaSound.BombBlow, "Sounds/LOZ_Bomb_Blow", false),
                (ZeldaSound.Candle, "Sounds/LOZ_Candle", false),
                (ZeldaSound.SwordSwing, "Sounds/LOZ_Sword_Slash", false),
                (ZeldaSound.SwordBeam, "Sounds/LOZ_Sword_Shoot", false),
                (ZeldaSound.Block, "Sounds/LOZ_Shield", false),
                (ZeldaSound.LinkHurt, "Sounds/LOZ_Link_Hurt", false),
                (ZeldaSound.LinkDie, "Sounds/LOZ_Link_Die", false),
                (ZeldaSound.EnemyHurt, "Sounds/LOZ_Enemy_Hit", false),
                (ZeldaSound.EnemyDie, "Sounds/LOZ_Enemy_Die", false),
                (ZeldaSound.BossScream, "Sounds/LOZ_Boss_Scream1", false),
                (ZeldaSound.LowHealth, "Sounds/LOZ_LowHealth", true),
                (ZeldaSound.NewItemFanfare, "Sounds/LOZ_Fanfare", false),
                (ZeldaSound.SecretFind, "Sounds/LOZ_Secret", false),
                (ZeldaSound.KeyAppear, "Sounds/LOZ_Key_Appear", false),
                (ZeldaSound.GetHeart, "Sounds/LOZ_Get_Heart", false),
                (ZeldaSound.GetItem, "Sounds/LOZ_Get_Item", false),
                (ZeldaSound.GetRupee, "Sounds/LOZ_Get_Rupee", false),
                (ZeldaSound.RefillLoop, "Sounds/LOZ_Refill_Loop", true),
                (ZeldaSound.DoorUnlock, "Sounds/LOZ_Door_Unlock", false),
                (ZeldaSound.Stairs, "Sounds/LOZ_Stairs", false),
                (ZeldaSound.TextLoop, "Sounds/LOZ_Text", true),
            };
    }
}
