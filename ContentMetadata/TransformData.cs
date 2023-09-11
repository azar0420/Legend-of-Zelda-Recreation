namespace amongus3902.ContentMetadata
{
    internal class TransformData
    {
        #region Layers
        //min difference between layers, can tweak to be finer or coarser
        private const float LOCAL_SEPERATION = 0.01f;

        //lower environment
        private const float LOWER_ENVIRONMENT_BASE = 0;

        public const float BG_DEPTH = LOWER_ENVIRONMENT_BASE;
        public const float DOOR_FRAME_DEPTH = LOWER_ENVIRONMENT_BASE + LOCAL_SEPERATION;

        //blocks
        private const float BLOCK_BASE = 0.15f;

        public const float BLOCK_DEPTH = BLOCK_BASE;
        public const float PUSHABLE_BLOCK_DEPTH = BLOCK_BASE + LOCAL_SEPERATION;

        //text
        private const float TEXT_BASE = 0.2f;

        public const float TEXT_DEPTH = TEXT_BASE;

        //items
        private const float ITEM_BASE = 0.4f;

        public const float ITEM_DEPTH = ITEM_BASE;

        //enemies
        private const float ENEMY_BASE = 0.45f;

        public const float ENEMY_PROJECTILE_DEPTH = ENEMY_BASE;
        public const float ENEMY_DEPTH = ENEMY_BASE + LOCAL_SEPERATION;

        //player
        private const float PLAYER_BASE = 0.5f;

        public const float PLAYER_PROJECTILE_DEPTH = PLAYER_BASE;
        public const float PLAYER_DEPTH = PLAYER_BASE + LOCAL_SEPERATION;

        //upper environment
        private const float UPPER_ENVIRONMENT_BASE = 0.8f;

        public const float DOOR_TOP_DEPTH = UPPER_ENVIRONMENT_BASE;

        //HUD
        private const float HUD_BASE = 0.9f;

        public const float NON_MENU_DEPTH_OVERRIDE = HUD_BASE - LOCAL_SEPERATION;

        public const float HUD_BG_DEPTH = HUD_BASE;
        public const float HUD_ADDONS_DEPTH = HUD_BASE + LOCAL_SEPERATION;
        public const float HUD_MAP_PLAYER_DEPTH = HUD_BASE + 2 * LOCAL_SEPERATION;
        public const float HUD_MAP_COMPASS_DEPTH = HUD_BASE + 3 * LOCAL_SEPERATION;

        //pause menu
        private const float PAUSE_MENU_BASE = 0.95f;

        public const float PAUSE_MENU_BG_DEPTH = PAUSE_MENU_BASE;
        public const float PAUSE_MENU_ADDONS_DEPTH = PAUSE_MENU_BASE + LOCAL_SEPERATION;
        public const float PAUSE_MENU_RETICLE_DEPTH = PAUSE_MENU_BASE + 2* LOCAL_SEPERATION;

        //override
        public const float DEPTH_OVERRIDE = 1;

        #endregion
    }
}
