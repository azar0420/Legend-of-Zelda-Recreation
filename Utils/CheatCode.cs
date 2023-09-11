using amongus3902.Components;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Utils
{
    internal class CheatCode
    {
        List<Keys> _inputTracker = new();

        List<Keys> _keys;
        Action _actionToInvoke;

        public CheatCode(List<Keys> keys, Action toInvoke)
        {
            _keys = keys;
            _actionToInvoke = toInvoke;
        }


        private bool CheckCheatCode()
        {
            for (int i = 0; i < _inputTracker.Count; i++)
            {
                if (_keys[i] != _inputTracker.ElementAt(i))
                    return false;
            }
            return true;
        }

        public void UpdateCheatCode(Keys k)
        {
            _inputTracker.Add(k);

            bool CheatProgress = CheckCheatCode();

            if (!CheatProgress)
            {
                _inputTracker = new();
            }
            else if (CheatProgress && _inputTracker.Count == _keys.Count)
            {
                _actionToInvoke();
                _inputTracker = new();
            }
        }
    }
}
