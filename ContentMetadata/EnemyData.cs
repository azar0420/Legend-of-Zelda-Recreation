using amongus3902.Components;
using amongus3902.Data;
using amongus3902.Utils;
using System.Collections.Generic;

namespace amongus3902.ContentMetadata
{
    public enum Enemys
    {
        Keese,
        Stalfos,
        Goriya,
        Gel,
        WallMaster,
        Trap,
        Aquamentus,
        OldMan,
        HarmlessOldMan,
        AmogusSus
    }

    struct EnemyParams
    {
        public ZeldaSpriteSheet SpriteSheet;
        public int? Damage;
        public int? Health;
        public bool Collides;
        public Animation Animation;
        public Directions InitialDirection;
        public Collidables CollisionGroup;
    }

    internal class EnemyData
    {
        //Enemy health amounts from https://strategywiki.org/wiki/The_Legend_of_Zelda/Enemies
        //1 damage unit = .5 heart
        public readonly static Dictionary<Enemys, EnemyParams> EnemyDetails =
            new()
            {
                {
                    Enemys.Keese,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.Keese,
                        Damage = 1,
                        Health = 1,
                        Collides = true,
                        Animation = new(0, 1, 75),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.AirbornEnemy,
                    }
                },
                {
                    Enemys.Stalfos,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.Stalfos,
                        Damage = 1,
                        Health = 2,
                        Collides = true,
                        Animation = new(0, 1, 200),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                {
                    Enemys.Goriya,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.Goriya,
                        Damage = 1,
                        Health = 3,
                        Collides = true,
                        Animation = new(0, 1, 200),
                        InitialDirection = Directions.Down,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                {
                    Enemys.Gel,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.Gel,
                        Damage = 1,
                        Health = 1,
                        Collides = true,
                        Animation = new(0, 1, 100),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                {
                    Enemys.WallMaster,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.WallMaster,
                        Damage = 1,
                        Health = 1,
                        Collides = false,
                        Animation = new(0, 1, 400),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                {
                    Enemys.Trap,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.Trap,
                        Damage = 1,
                        Collides = true,
                        Animation = new(0, 0),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                {
                    Enemys.Aquamentus,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.Aquamentus,
                        Damage = 2,
                        Health = 6,
                        Collides = false,
                        Animation = new(2, 3, 200),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                {
                    Enemys.OldMan,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.OldMan,
                        Health = 10000,
                        Collides = true,
                        Animation = new(0, 0),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                 {
                    Enemys.HarmlessOldMan,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.OldMan,
                        Damage = 0,
                        Health = 1,
                        Collides = true,
                        Animation = new(0, 0),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
                {
                    Enemys.AmogusSus,
                    new EnemyParams
                    {
                        SpriteSheet = ZeldaSpriteSheet.Sus,
                        Health = 16,
                        Damage = 2,
                        Collides = true,
                        Animation = new(0, 5, 125),
                        InitialDirection = Directions.None,
                        CollisionGroup = Collidables.GroundEnemy,
                    }
                },
            };

        public readonly static List<Enemys> BoomerangDamageable =
            new() { Enemys.Keese, Enemys.Gel, Enemys.AmogusSus };
    }
}
