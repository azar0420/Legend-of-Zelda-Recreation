using amongus3902.Utils;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.ContentMetadata
{
    internal class CheatCodeData
    {
        public static List<Keys> DancingCheatCode = new(){ Keys.R, Keys.I, Keys.Z, Keys.Z };
        public static List<Keys> CoinCheatCode = new() { Keys.C, Keys.O, Keys.I, Keys.N };
        public static List<Keys> DieCheatCode = new() { Keys.L, Keys.P, Keys.L, Keys.U, Keys.S, Keys.R, Keys.A, Keys.T, Keys.I, Keys.O };
        public static List<CheatCode> CheatCodes;
    }
}