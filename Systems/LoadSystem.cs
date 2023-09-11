using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class LoadSystem : IAlwaysActiveSystem
    {
        private readonly ContentManager _content;
        private readonly Dictionary<
            ZeldaSpriteSheet,
            (Texture2D texture, int rows, int cols)
        > _spriteSheetDetails = new();
        private readonly Dictionary<ZeldaSound, (SoundEffect Audio, bool looping)> _soundDetails =
            new();

        public LoadSystem(ContentManager content)
        {
            _content = content;
        }

        private void LoadSprite(ZeldaSpriteSheet s, string fileName, int rows, int cols)
        {
            _spriteSheetDetails.Add(s, (_content.Load<Texture2D>(fileName), rows, cols));
        }

        private void LoadSound(ZeldaSound s, string fileName, bool looping)
        {
            _soundDetails.Add(s, (_content.Load<SoundEffect>(fileName), looping));
        }

        public void Start(World world)
        {
            SpriteData.SheetDetailsRaw.ForEach(
                t => LoadSprite(t.sheet, t.fileName, t.rows, t.cols)
            );

            SoundData.SoundDetailsRaw.ForEach(t => LoadSound(t.sound, t.fileName, t.looping));
        }

        public SpriteSheet GetSheet(ZeldaSpriteSheet spriteSheet)
        {
            var (texture, rows, cols) = _spriteSheetDetails[spriteSheet];

            return new(texture, rows, cols);
        }

        public Sound GetSound(ZeldaSound sound)
        {
            var (soundEffect, looping) = _soundDetails[sound];

            return new(soundEffect, sound, looping);
        }

        public RoomData LoadRoomData(RoomName roomName)
        {
            return RoomParser.GetRoomData(roomName);
        }
    }
}
