using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace amongus3902.Systems
{
    public enum PressType
    {
        onDown,
        onUp
    }

    public enum MouseButtons
    {
        left,
        right
    }

    internal class InputSystem : IUpdateSystem, IAlwaysActiveSystem
    {
        private readonly Dictionary<(Keys, PressType), Action> _boundKeyActions = new();
        private readonly Dictionary<(MouseButtons, PressType), Action> _boundMouseActions = new();
        private World _world;

        private KeyboardState _oldKBState = new();
        private MouseState _oldMouseState = new();

        public event Action<Keys> OnKeyDown;

        public void Start(World world) { }

        public void Update(GameTime gameTime)
        {
            KeyboardState newKBState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            // invoke key actions
            foreach (var ((key, pressType), action) in _boundKeyActions)
            {
                if (
                    pressType == PressType.onDown && WasKeyPressed(_oldKBState, newKBState, key)
                    || pressType == PressType.onUp && WasKeyReleased(_oldKBState, newKBState, key)
                )
                {
                    action();
                }
            }

            // invoke OnKeyDown actions
            foreach (Keys key in newKBState.GetPressedKeys())
            {
                if (WasKeyPressed(_oldKBState, newKBState, key))
                {
                    OnKeyDown?.Invoke(key);
                }
            }

            // invoke mouse actions
            foreach (var ((button, pressType), action) in _boundMouseActions)
            {
                if (
                    pressType == PressType.onDown
                        && WasButtonPressed(_oldMouseState, newMouseState, button)
                    || pressType == PressType.onUp
                        && WasButtonReleased(_oldMouseState, newMouseState, button)
                )
                {
                    action();
                }
            }

            _oldKBState = newKBState;
            _oldMouseState = newMouseState;
        }

        private static bool WasKeyPressed(KeyboardState oldState, KeyboardState newState, Keys key)
        {
            return newState.IsKeyDown(key) && !oldState.IsKeyDown(key);
        }

        private static bool WasKeyReleased(KeyboardState oldState, KeyboardState newState, Keys key)
        {
            return !newState.IsKeyDown(key) && oldState.IsKeyDown(key);
        }

        private static bool WasButtonPressed(
            MouseState oldState,
            MouseState newState,
            MouseButtons button
        )
        {
            ButtonState newButtonState = newState.LeftButton;
            ButtonState oldButtonState = oldState.LeftButton;

            if (button == MouseButtons.right)
            {
                newButtonState = newState.RightButton;
                oldButtonState = oldState.RightButton;
            }

            return newButtonState == ButtonState.Pressed && oldButtonState != ButtonState.Pressed;
        }

        private static bool WasButtonReleased(
            MouseState oldState,
            MouseState newState,
            MouseButtons button
        )
        {
            ButtonState newButtonState = newState.LeftButton;
            ButtonState oldButtonState = oldState.LeftButton;

            if (button == MouseButtons.right)
            {
                newButtonState = newState.RightButton;
                oldButtonState = oldState.RightButton;
            }

            return newButtonState == ButtonState.Released && oldButtonState != ButtonState.Released;
        }

        private static Action Bind<T>(
            Dictionary<(T, PressType), Action> stateMap,
            T button,
            PressType pressType,
            Action toBind
        )
        {
            (T, PressType) buttonState = (button, pressType);

            if (!stateMap.ContainsKey(buttonState))
            {
                stateMap.Add(buttonState, () => { });
            }

            stateMap[buttonState] += toBind;

            return () =>
            {
                stateMap[buttonState] -= toBind;
            };
        }

        public Action Bind(Action toBind, Keys key, PressType kpt = PressType.onDown)
        {
            return Bind(_boundKeyActions, key, kpt, toBind);
        }

        public Action Bind(Action toBind, PressType kpt, params Keys[] keys)
        {
            Action unbindAll = () => { };
            Array.ForEach(keys, key => unbindAll += Bind(toBind, key, kpt));
            return unbindAll;
        }

        public Action Bind(Action toBind, MouseButtons button, PressType mpt = PressType.onDown)
        {
            return Bind(_boundMouseActions, button, mpt, toBind);
        }

        public Action Bind(Action toBind, PressType mpt, params MouseButtons[] buttons)
        {
            Action unbindAll = () => { };
            Array.ForEach(buttons, button => unbindAll += Bind(toBind, button, mpt));
            return unbindAll;
        }

        public static Vector2 GetMouseCoords()
        {
            return new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }
    }
}
