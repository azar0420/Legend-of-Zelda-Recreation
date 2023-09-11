using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace amongus3902.Utils
{
    internal static class RoomBorder
    {
        public static Entity CreateBackground(
            SpriteSheet backgroundTexture,
            Vector2 screenSize,
            int menuHeight
        )
        {
            Entity background = new();
            Sprite bgSprite = new(backgroundTexture, 0);
            background.Attach(bgSprite);

            Vector2 spriteSize = bgSprite.Sheet.FrameSize;

            int maxWidthMult = ((int)screenSize.X / (int)spriteSize.X);
            int maxHeightMult = ((int)screenSize.Y / (int)spriteSize.Y);

            int scale = Math.Min(maxWidthMult, maxHeightMult);

            Vector2 ScreenMid = new((int)screenSize.X / 2, (int)screenSize.Y / 2);
            Vector2 pos =
                ScreenMid
                - new Vector2(spriteSize.X * scale / 2, spriteSize.Y * scale / 2)
                + Vector2.UnitY * menuHeight * scale / 2;

            background.Attach(new Transform(pos, TransformData.BG_DEPTH, scale));

            return background;
        }

        public static void AddWallCollision(Vector2 tileOrigin, float scale, List<Entity> toAdd, bool isBasement)
        {
            Vector2 XWallSize =
                (
                    Vector2.UnitX
                        * (
                            ((RoomConstants.TILE_GRID_WIDTH - RoomConstants.HORIZ_DOOR_SIZE) / 2)
                            + 2
                        )
                    + Vector2.UnitY * 2
                ) * RoomConstants.TILE_SIZE;
            Vector2 YWallSize =
                (
                    Vector2.UnitY
                        * (((RoomConstants.TILE_GRID_HEIGHT - RoomConstants.VERT_DOOR_SIZE) / 2))
                    + Vector2.UnitX * 2
                ) * RoomConstants.TILE_SIZE;
            Vector2 OneTile = Vector2.One * RoomConstants.TILE_SIZE * scale;
            Vector2 X2Modify =
                XWallSize * Vector2.UnitX * scale
                + new Vector2(RoomConstants.HORIZ_DOOR_SIZE, 0) * OneTile;
            Vector2 Y2Modify =
                YWallSize * Vector2.UnitY * scale
                + new Vector2(0, RoomConstants.VERT_DOOR_SIZE) * OneTile;
            Vector2 TileWidth = OneTile * Vector2.UnitX;
            Vector2 RoomWidth = new Vector2(RoomConstants.TILE_GRID_WIDTH, 0) * OneTile;
            Vector2 RoomHeight = new Vector2(0, RoomConstants.TILE_GRID_HEIGHT) * OneTile;

            Vector2 BasementAdjust = XWallSize;
            if (isBasement)
            {
                BasementAdjust.Y /= 2;
            }

            //top wall
            toAdd.Add(Physics.CreateRectCollision(tileOrigin - OneTile * 2, BasementAdjust, scale));
            toAdd.Add(
                Physics.CreateRectCollision(tileOrigin - OneTile * 2 + X2Modify, XWallSize, scale)
            );
            //left wall
            toAdd.Add(Physics.CreateRectCollision(tileOrigin - TileWidth * 2, YWallSize, scale));
            toAdd.Add(
                Physics.CreateRectCollision(tileOrigin - TileWidth * 2 + Y2Modify, YWallSize, scale)
            );
            //right wall
            toAdd.Add(Physics.CreateRectCollision(tileOrigin + RoomWidth, YWallSize, scale));
            toAdd.Add(
                Physics.CreateRectCollision(tileOrigin + RoomWidth + Y2Modify, YWallSize, scale)
            );
            //bottom wall
            toAdd.Add(
                Physics.CreateRectCollision(
                    tileOrigin + RoomHeight - TileWidth * 2,
                    XWallSize,
                    scale
                )
            );
            toAdd.Add(
                Physics.CreateRectCollision(
                    tileOrigin + RoomHeight - TileWidth * 2 + X2Modify,
                    XWallSize,
                    scale
                )
            );
        }
    }
}
