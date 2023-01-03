using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperStomper.GameScripts.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content;

namespace SuperStomper.GameScripts.Game
{
    internal class MapLoader
    {
        public List<Tile> tiles;
        public List<Hitbox> colliders;
        public List<Gomba> gombas;
        public List<PiranhaPlant> piranhaPlants;
        public List<Pipe> pipes;
        public List<Coin> coins;
        public Mario mario;
        public Castle castle;
        public int levelMaxWidth;

        public MapLoader(ContentManager content, int level)
        {
            tiles = new List<Tile>();
            colliders = new List<Hitbox>();
            gombas = new List<Gomba>();
            piranhaPlants = new List<PiranhaPlant>();
            pipes = new List<Pipe>();
            coins = new List<Coin>();
            mario = null;
            castle = null;
            levelMaxWidth = 0;

            // The default directory for the project is the debug directory
            // where the .exe file exists, but or Level_Name.txt file is in the Content folder
            // located in the project directory, so get the debug directory, trim a piece of it
            // then we get to the Content directory so that we can load the Level_Name.txt file
            string debugDirectory = Environment.CurrentDirectory;
            string levelDirectory = debugDirectory.Substring(0, debugDirectory.LastIndexOf("bin"));
            levelDirectory += "Content\\Levels\\Level " + level.ToString() + ".json";

            string jsonText = File.ReadAllText(levelDirectory);

            Root levelData = JsonConvert.DeserializeObject<Root>(jsonText);
            levelMaxWidth = levelData.width;

            Texture2D tileset = content.Load<Texture2D>(@"Spritesheets\Environment\OverWorld");
            for (int i = 0; i < levelData.layers.Count; i++)
            {
                Layer layer = levelData.layers[i];

                if (layer.name == "Entities")
                {
                    for (int j = 0; j < layer.entities.Count; j++)
                    {
                        Entity entity = layer.entities[j];

                        switch (entity.name)
                        {
                            case "Mario":
                                mario = new Mario(content, new Vector2(entity.x, entity.y));
                                break;
                            case "Collider":
                                colliders.Add(new Hitbox(new Rectangle(entity.x, entity.y, (int)entity.width, (int)entity.height), Vector2.Zero));
                                break;
                            case "Castle":
                                castle = new Castle(content, new Vector2(entity.x, entity.y));
                                break;
                            case "Coin":
                                coins.Add(new Coin(content, new Vector2(entity.x, entity.y)));
                                break;
                            case "Gomba":
                                gombas.Add(new Gomba(content, new Vector2(entity.x, entity.y)));
                                break;
                            case "Pipe":
                                pipes.Add(new Pipe(content, new Vector2(entity.x, entity.y)));
                                break;
                            case "Piranha":
                                piranhaPlants.Add(new PiranhaPlant(content, new Vector2(entity.x, entity.y)));
                                break;
                        }
                    }
                }
                else if (layer.name == "Environment")
                {
                    List<int> data = levelData.layers[i].data;
                    for (int j = 0; j < data.Count; j++)
                    {
                        if (data[j] == -1)
                            continue;

                        tiles.Add(new Tile(tileset, new Rectangle((data[j] * Tile.tileWidth) % tileset.Width, ((data[j] * Tile.tileWidth) / tileset.Width) * Tile.tileHeight, Tile.tileWidth, Tile.tileHeight), new Vector2((j * levelData.layers[i].gridCellWidth) % levelData.width, ((j * levelData.layers[i].gridCellWidth) / levelData.width) * levelData.layers[i].gridCellHeight)));
                    }
                }
            }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Entity
        {
            public string name { get; set; }
            public int id { get; set; }
            public string _eid { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public int originX { get; set; }
            public int originY { get; set; }
            public int? width { get; set; }
            public int? height { get; set; }
        }

        public class Layer
        {
            public string name { get; set; }
            public string _eid { get; set; }
            public int offsetX { get; set; }
            public int offsetY { get; set; }
            public int gridCellWidth { get; set; }
            public int gridCellHeight { get; set; }
            public int gridCellsX { get; set; }
            public int gridCellsY { get; set; }
            public List<Entity> entities { get; set; }
            public string tileset { get; set; }
            public List<int> data { get; set; }
            public int? exportMode { get; set; }
            public int? arrayMode { get; set; }
        }

        public class Root
        {
            public string ogmoVersion { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int offsetX { get; set; }
            public int offsetY { get; set; }
            public List<Layer> layers { get; set; }
        }
    }
}
