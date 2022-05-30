using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zooperdan.AtlasMaker
{

    public class LUATABLE
    {

        public static void Save(string atlasid, string filename, JSONResult jsonResult)
        {

            string result = atlasid + " = {\n";

            result += string.Format("\t{0}=\"{1}\",\n", "version", jsonResult.version);
            result += string.Format("\t{0}=\"{1}\",\n", "generated", jsonResult.generated);
            result += string.Format("\t{0}={1},\n", "depth", jsonResult.depth);
            result += string.Format("\t{0}={1},\n", "width", jsonResult.width);
            result += "\tresolution={width=" + jsonResult.resolution.width.ToString() + ",height=" + jsonResult.resolution.height.ToString() + "},\n";

            result += "\tlayers={\n";

            for (int layerIndex = 0; layerIndex < jsonResult.layers.Count; layerIndex++)
            {
                JSONLayer layer = jsonResult.layers[layerIndex];

                result += "\t\t[" + layerIndex.ToString() + "]={\n";

                result += string.Format("\t\t\t{0}={1},\n", "id", layer.id);
                result += string.Format("\t\t\t{0}=\"{1}\",\n", "name", layer.name);
                result += string.Format("\t\t\t{0}={1},\n", "mode", layer.mode);
                result += string.Format("\t\t\t{0}=\"{1}\",\n", "type", layer.type);

                result += "\t\t\ttiles={\n";

                for (int tileIndex = 0; tileIndex < layer.tiles.Count; tileIndex++)
                {
                    JSONTile tile = layer.tiles[tileIndex];
                    result += "\t\t\t\t[" + tileIndex.ToString() + "]={\n";
                    result += string.Format("\t\t\t\t\t{0}=\"{1}\",\n", "type", tile.type);
                    result += "\t\t\t\t\tposition={x=" + tile.tile.x.ToString() + ",y=" + tile.tile.y.ToString() + "},\n";
                    result += "\t\t\t\t\tscreen={x=" + tile.screen.x.ToString() + ",y=" + tile.screen.y.ToString() + "},\n";
                    result += "\t\t\t\t\tcoords={x=" + tile.coords.x.ToString() + ",y=" + tile.coords.y.ToString() + ",w=" + tile.coords.w.ToString() + ",h=" + tile.coords.h.ToString() + "},\n";
                    result += "\t\t\t\t},\n";
                }

                result += "\t\t\t},\n";

                result += "\t\t},\n";

            }

            result += "\t}\n";
            result += "}\n";

            System.IO.File.WriteAllText(filename, result);

        }

    }

}