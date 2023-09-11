using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Factories;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace amongus3902.Utils
{
    internal class RoomParser
    {
        public static RoomData GetRoomData(RoomName roomName)
        {
            string projectDirectory = Directory
                .GetParent(Environment.CurrentDirectory)
                .Parent.Parent.FullName;
            string filePath = projectDirectory + "\\Content\\Rooms\\" + roomName + ".json";

            //initialize RoomData struct
            RoomData rd =
                new()
                {
                    Blocks = new Dictionary<BlockType, List<Vector2>>(),
                    Enemies = new Dictionary<Enemys, List<Vector2>>(),
                    Items = new Dictionary<ItemType, List<Vector2>>(),
                    Text = new Dictionary<TextType, List<Vector2>>(),
                    Doors = new Dictionary<string, (DoorState, RoomName)>()
                };

            //deserialize json file
            JSONRoot root;
            using (StreamReader file = File.OpenText(filePath))
            {
                root = JsonConvert.DeserializeObject<JSONRoot>(file.ReadToEnd());
                file.Close();
            }

            //set information about room
            rd.Name = roomName;
            rd.Coords = new Vector2(root.RoomDataImport.coords[0], root.RoomDataImport.coords[1]);
            rd.Doors.Add(
                "north",
                (
                    Enum.Parse<DoorState>(root.RoomDataImport.doors.north[0]),
                    Enum.Parse<RoomName>(root.RoomDataImport.doors.north[1])
                )
            );
            rd.Doors.Add(
                "west",
                (
                    Enum.Parse<DoorState>(root.RoomDataImport.doors.west[0]),
                    Enum.Parse<RoomName>(root.RoomDataImport.doors.west[1])
                )
            );
            rd.Doors.Add(
                "east",
                (
                    Enum.Parse<DoorState>(root.RoomDataImport.doors.east[0]),
                    Enum.Parse<RoomName>(root.RoomDataImport.doors.east[1])
                )
            );
            rd.Doors.Add(
                "south",
                (
                    Enum.Parse<DoorState>(root.RoomDataImport.doors.south[0]),
                    Enum.Parse<RoomName>(root.RoomDataImport.doors.south[1])
                )
            );

            if (root.RoomDataImport.text != null)
            {
                foreach (List<string> strings in root.RoomDataImport.text)
                {
                    Vector2 textPos = new(float.Parse(strings[0]), float.Parse(strings[1]));
                    TextType tt = (TextType)Enum.Parse(typeof(TextType), strings[2]);
                    if (!rd.Text.ContainsKey(tt))
                    {
                        rd.Text.Add(tt, new());
                    }
                    rd.Text[tt].Add(textPos);
                }
            }

            foreach (List<string> strings in root.RoomDataImport.blocks)
            {
                Vector2 blockPos = new(float.Parse(strings[0]), float.Parse(strings[1]));
                BlockType bt = (BlockType)Enum.Parse(typeof(BlockType), strings[2], true);
                if (!rd.Blocks.ContainsKey(bt))
                {
                    rd.Blocks.Add(bt, new());
                }
                rd.Blocks[bt].Add(blockPos);
            }

            foreach (List<string> strings in root.RoomDataImport.enemies)
            {
                Vector2 enemyPos = new(float.Parse(strings[0]), float.Parse(strings[1]));
                Enemys et = (Enemys)Enum.Parse(typeof(Enemys), strings[2], true);
                if (!rd.Enemies.ContainsKey(et))
                {
                    rd.Enemies.Add(et, new());
                }
                rd.Enemies[et].Add(enemyPos);
            }

            foreach (List<string> strings in root.RoomDataImport.items)
            {
                Vector2 itemPos = new(float.Parse(strings[0]), float.Parse(strings[1]));
                ItemType it = (ItemType)Enum.Parse(typeof(ItemType), strings[2], true);
                if (!rd.Items.ContainsKey(it))
                {
                    rd.Items.Add(it, new());
                }
                rd.Items[it].Add(itemPos);
            }

            return rd;
        }
    }
}
