using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Gamepads;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace amongus3902.Utils
{
    internal static class LinkUtil
    {
        const int HEALTH_PER_LINK = 2;
        const int TOTAL_MIN_HEALTH = 6;

        const int LINK_INITIAL_FRAME = 3;
        static readonly Vector2 LINK_START_POS = 3 * Vector2.One;

        static readonly ZeldaSpriteSheet[] LINK_SHEETS =
        {
            ZeldaSpriteSheet.LinkGreen,
            ZeldaSpriteSheet.LinkBlue,
            ZeldaSpriteSheet.LinkPurple,
            ZeldaSpriteSheet.LinkRed,
            ZeldaSpriteSheet.LinkPink,
        };

        static readonly IGamepad[] LINK_CONTROLS =
        {
            new P1Gamepad(),
            new P2Gamepad(),
            new P3Gamepad(),
            new P4Gamepad(),
            new P5Gamepad(),
        };

        public static List<Entity> MakeLinks(
            int linkCount,
            Func<ZeldaSpriteSheet, SpriteSheet> getSheet
        )
        {
            Debug.Assert(linkCount <= LINK_CONTROLS.Length && linkCount <= LINK_SHEETS.Length);

            List<Entity> links = new();

            for (int i = 0; i < linkCount; i++)
            {
                Entity link = MakeLink(getSheet(LINK_SHEETS[i]), LINK_CONTROLS[i]);

                int totalHealth = Math.Max(TOTAL_MIN_HEALTH, HEALTH_PER_LINK * linkCount);

                link.Get<HurtBox>().MaxHealth = totalHealth;
                link.Get<HurtBox>().Health = totalHealth;

                links.Add(link);
            }

            SynchronizeHealth(links);

            return links;
        }

        private static Entity MakeLink(SpriteSheet linkSheet, IGamepad controls)
        {
            return new Entity()
                .Attach(new Transform(LINK_START_POS, TransformData.PLAYER_DEPTH))
                .Attach(new Sprite(linkSheet, LINK_INITIAL_FRAME))
                .Attach(AnimationData.LINK_ANIMATIONS[(CharacterAction.Look, Directions.Up)])
                .Attach(
                    new PhysicsBody(true, new Vector2(12, 8), new Vector2(2, 8), Collidables.Player)
                )
                .Attach(
                    new HurtBox(
                        RoomConstants.TILE_SIZE * Vector2.One,
                        Vector2.Zero,
                        TOTAL_MIN_HEALTH
                    )
                )
                .Attach(new Team(TeamType.Friend))
                .Attach(new CharacterController(CharacterAction.Look, Directions.Up, controls));
        }

        // i live in spain without the a
        public static Entity MakeSpinnyLink(SpriteSheet linkSheet)
        {
            return new Entity()
                .Attach(new Sprite(linkSheet, 32))
                .Attach(new Animation(32, 35, 100, true));
        }

        public static Entity MakeWhiteLink(SpriteSheet whiteLinkSheet)
        {
            return new Entity().Attach(new Sprite(whiteLinkSheet));
        }

        private static void SynchronizeHealth(List<Entity> links)
        {
            int masterHealth = links.First().Get<HurtBox>().Health;

            void syncHealth(HurtBox healthToSyncTo)
            {
                links.ForEach(l =>
                {
                    l.Get<HurtBox>().MaxHealth = healthToSyncTo.MaxHealth;
                    l.Get<HurtBox>().Health = healthToSyncTo.Health;
                });
            }

            foreach (Entity link in links)
            {
                link.Get<HurtBox>().Healed += () => syncHealth(link.Get<HurtBox>());
                link.Get<HurtBox>().Damaged += (Entity _) => syncHealth(link.Get<HurtBox>());
            }
        }
    }
}
