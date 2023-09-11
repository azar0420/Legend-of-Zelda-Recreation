using amongus3902.Data;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace amongus3902.Systems
{
    internal class DebugRoomSwitcher : ISystem
    {
        private RoomGenerationSystem _roomGenerator;
        public readonly RoomName[] _rooms =
        {
            RoomName.e2_5,
            RoomName.e1_0,
            RoomName.e1_2,
            RoomName.e1_3,
            RoomName.e1_5,
            RoomName.e2_0,
            RoomName.e2_1,
            RoomName.e2_2,
            RoomName.e2_3,
            RoomName.e2_4,
            RoomName.e0_2,
            RoomName.e3_2,
            RoomName.e3_3,
            RoomName.e3_5,
            RoomName.e4_1,
            RoomName.e4_2,
            RoomName.e5_1,
            RoomName.debug,
            RoomName.basement,
            RoomName.portalroom,
            RoomName.upgradeTest,
            RoomName.em1_2,
        };
        int _activeIndex = 0;

        private event Action _onDisable;

        public void Start(World world)
        {
            _roomGenerator = world.GetSystem<RoomGenerationSystem>();
            InputSystem input = world.GetSystem<InputSystem>();

            _roomGenerator.SwitchRoom(_rooms[_activeIndex], Vector2.UnitY * 0.9f);

            if (world.MODE == WorldMode.Debug)
            {
                _onDisable += input.Bind(NextRoom, MouseButtons.left);
                _onDisable += input.Bind(PrevRoom, MouseButtons.right);
            }
        }

        public int GetIndex()
        {
            return _activeIndex;
        }

        private void NextRoom()
        {
            _activeIndex = (_activeIndex + 1) % _rooms.Length;
            _roomGenerator.SwitchRoom(_rooms[_activeIndex], Vector2.UnitY * 0.9f);
        }

        private void PrevRoom()
        {
            // adding _rooms.Length because % is a remainder operator, not a modulo
            _activeIndex = (_activeIndex - 1 + _rooms.Length) % _rooms.Length;
            _roomGenerator.SwitchRoom(_rooms[_activeIndex], Vector2.UnitY * 0.9f);
        }

        public void Disable()
        {
            _onDisable?.Invoke();
        }
    }
}
