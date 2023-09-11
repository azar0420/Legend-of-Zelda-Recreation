using amongus3902.ContentMetadata;
using amongus3902.Factories;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using amongus3902.Utils;

namespace amongus3902.Data
{
    //The e in these names are for "Eagle", the name of the first dungeon, in case we ever add more dungeons
    public enum RoomName
    {
        em1_2,
        e0_2,
        e1_0,
        e1_2,
        e1_3,
        e1_5,
        e2_0,
        e2_1,
        e2_2,
        e2_3,
        e2_4,
        e2_5,
        e3_2,
        e3_3,
        e3_5,
        e4_1,
        e4_2,
        e5_1,
        basement,
        debug,
        portalroom,
        upgradeTest,
        none
    }

    public enum RoomText
    {
        oldmantext
    }

    public struct RoomData
    {
        public RoomName Name;
        public Vector2 Coords;
        public Dictionary<string, (DoorState, RoomName)> Doors;
        public Dictionary<BlockType, List<Vector2>> Blocks;
        public Dictionary<Enemys, List<Vector2>> Enemies;
        public Dictionary<ItemType, List<Vector2>> Items;
        public Dictionary<TextType, List<Vector2>> Text;
    }

    //JSON data importing related classes
    public class JSONRoot
    {
        public RoomDataImport RoomDataImport { get; set; }
    }

    public class RoomDataImport
    {
        public List<int> coords { get; set; }
        public Doors doors { get; set; }
        public List<List<string>> blocks { get; set; }
        public List<List<string>> enemies { get; set; }
        public List<List<string>> items { get; set; }
        public List<List<string>> text { get; set; }
    }

    public class Doors
    {
        public string[] north { get; set; }
        public string[] west { get; set; }
        public string[] east { get; set; }
        public string[] south { get; set; }
    }

    public class RoomConstants
    {
        public const int TILE_SIZE = 16;
        public const float TILE_GRID_WIDTH = 12;
        public const float TILE_GRID_HEIGHT = 7;
        public const float HORIZ_DOOR_SIZE = 2;
        public const float VERT_DOOR_SIZE = 1;
    }
}
