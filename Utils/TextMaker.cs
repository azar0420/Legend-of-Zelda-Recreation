using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using Microsoft.Xna.Framework;

namespace amongus3902.Utils
{
    public enum TextType
    {
        OldManText,
        GameOverText,
        GameWinText,
        RetirementText
    }

    internal class TextMaker
    {
        private readonly LoadSystem _loader;
        private readonly SoundSystem _sound;

        public TextMaker(World world)
        {
            _loader = world.GetSystem<LoadSystem>();
            _sound = world.GetSystem<SoundSystem>();
        }

        public Entity CreateText(TextType textType)
        {
            return textType switch
            {
                TextType.OldManText => CreateOldManText(),
                TextType.GameOverText => CreateGameOverText(),
                TextType.GameWinText => CreateGameWinText(),
                TextType.RetirementText => CreateRetirementText(),
                _ => throw new System.NotImplementedException(),
            };
        }

        public Entity CreateOldManText()
        {
            Entity text = new();
            SpriteSheet sheet = _loader.GetSheet(ZeldaSpriteSheet.OldManText);
            text.Attach(new Sprite(sheet, 0, Directions.None));
            text.Attach(new Transform(Vector2.Zero, TransformData.TEXT_DEPTH));
            Animation anim = new Animation(0, 32, 100, false, false);
            anim.AnimEnded += () =>
            {
                _sound.RemoveAudioSourceOfType(ZeldaSound.TextLoop);
            };
            text.Attach(anim);
            text.Attach(
                new PhysicsBody(true, new Vector2(192, 48), new Vector2(-16, 0), Collidables.Pit)
            );
            text.OnAdd += () =>
            {
                if (!anim.ReachedEnd)
                {
                    _sound.PlaySound(ZeldaSound.TextLoop);
                }
            };
            text.OnRemove += () =>
            {
                _sound.RemoveAudioSourceOfType(ZeldaSound.TextLoop);
            };
            return text;
        }

        public Entity CreateRetirementText()
        {
            SpriteSheet sheet = _loader.GetSheet(ZeldaSpriteSheet.RetirementText);
            Animation anim = new Animation(0, 31, false, false);
            anim.AnimEnded += () =>
            {
                _sound.RemoveAudioSourceOfType(ZeldaSound.TextLoop);
            };
            Entity text = new Entity()
                .Attach(new Sprite(sheet, 0, Directions.None))
                .Attach(new Transform(new Vector2(150, 300), (float).3, 3))
                .Attach(anim)
                .Attach(
                    new PhysicsBody(true, new Vector2(192, 48), new Vector2(-16, 0), Collidables.Pit));
            text.OnAdd += () =>
            {
                if (!anim.ReachedEnd)
                {
                    _sound.PlaySound(ZeldaSound.TextLoop);
                }
            };
            text.OnRemove += () =>
            {
                _sound.RemoveAudioSourceOfType(ZeldaSound.TextLoop);
            };
            return text;
        }

        public Entity CreateGameOverText()
        {
            Entity text = new();
            SpriteSheet sheet = _loader.GetSheet(ZeldaSpriteSheet.GameOverText);
            text.Attach(new Sprite(sheet, 0, Directions.None));
            text.Attach(new Transform(Vector2.Zero, TransformData.DEPTH_OVERRIDE));

            return text;
        }

        public Entity CreateGameWinText()
        {
            Entity text = new();
            SpriteSheet sheet = _loader.GetSheet(ZeldaSpriteSheet.GameWinText);
            text.Attach(new Sprite(sheet, 0, Directions.None));
            text.Attach(new Transform(Vector2.Zero, TransformData.DEPTH_OVERRIDE));

            return text;
        }
    }
}
