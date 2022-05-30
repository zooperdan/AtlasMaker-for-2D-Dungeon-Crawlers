using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zooperdan.AtlasMaker
{

    public class CSV
    {

        public static void Save(string filename, JSONResult jsonResult)
        {
            string result = "Layer ID, Layer Name, Layer Render Mode, Layer Type, Tile Type, Tile X, Tile Y, Screen X, Screen Y, Atlas X, Atlas Y, Atlas Width, Atlas Height\n";

            for (int layerIndex = 0; layerIndex < jsonResult.layers.Count; layerIndex++)
            {
                JSONLayer layer = jsonResult.layers[layerIndex];

                string linePrefix = string.Format("{0},{1},{2},{3},", layer.id, layer.name, layer.mode, layer.type);

                for (int tileIndex = 0; tileIndex < layer.tiles.Count; tileIndex++)
                {
                    JSONTile tile = layer.tiles[tileIndex];
                    result += linePrefix;
                    result += string.Format("{0},{1},{2},{3},{4},", tile.type, tile.tile.x, tile.tile.y, tile.screen.x, tile.screen.y);
                    result += string.Format("{0},{1},{2},{3}\n", tile.coords.x, tile.coords.y, tile.coords.w, tile.coords.h);
                }
            }

            System.IO.File.WriteAllText(filename, result);

        }

    }


}