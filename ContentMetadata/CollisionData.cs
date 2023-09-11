namespace amongus3902.ContentMetadata
{
    public enum Collidables
    {
        Player = 0,
        GroundEnemy = 1,
        Item = 2,
        Projectile = 3,
        Wall = 4,
        Block = 5,
        Pit = 6,
        AirbornEnemy = 7,
        Door = 8,
        PortalProjectile = 9
    }

    internal class CollisionData
    {
        private static readonly bool? I = true;
        private static readonly bool? O = false;
        private static readonly bool? X = null;

        public static bool?[,] CollisionGroupAssignments =
        {
            { O, X, X, X, X, X, X, X, O, O }, // Player
            { O, O, X, X, X, X, X, X, I, I }, // Ground Enemy
            { O, O, O, X, X, X, X, X, X, O }, // Item
            { O, O, O, O, X, X, X, X, O, O }, // Projectile
            { I, I, I, I, I, X, X, X, X, I }, // Wall
            { I, I, O, I, I, I, X, X, X, O }, // Block
            { I, I, I, O, I, I, I, X, X, I }, // Pit
            { O, O, O, O, I, O, O, O, X, I }, // Airborn Enemy
            { O, I, X, O, X, X, X, I, X, I }, // Door
            { O, I, O, O, I, O, I, I, I, X } // Portal Projectile
        };

        public static void FillInGaps()
        {
            int groupNum = CollisionGroupAssignments.GetLength(0);

            for (int i = 0; i < groupNum; i++)
            {
                for (int j = 0; j < groupNum; j++)
                {
                    CollisionGroupAssignments[i, j] ??= CollisionGroupAssignments[j, i];
                }
            }
        }
    }
}
