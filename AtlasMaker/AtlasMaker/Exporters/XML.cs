using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;

namespace zooperdan.AtlasMaker
{

    public class XML
    {

        public static void Save(string filename, JSONResult jsonResult)
        {

            XDocument xmldoc = new XDocument();
            XElement xmlbody = new XElement("body");
            XElement xmllayers = new XElement("layers");

            for (int layerIndex = 0; layerIndex < jsonResult.layers.Count; layerIndex++)
            {
                JSONLayer layer = jsonResult.layers[layerIndex];
                XElement xmllayer = new XElement("layer");
                XElement xmltiles = new XElement("tiles");

                xmllayer.Add(new XAttribute("id", layer.id));
                xmllayer.Add(new XAttribute("name", layer.name));
                xmllayer.Add(new XAttribute("rendermode", layer.mode));
                xmllayer.Add(new XAttribute("type", layer.type));

                for (int tileIndex = 0; tileIndex < layer.tiles.Count; tileIndex++)
                {
                    JSONTile tile = layer.tiles[tileIndex];
                    XElement xmltile = new XElement("tile");

                    xmltile.Add(new XAttribute("type", tile.type));
                    xmltile.Add(new XAttribute("map", string.Format("{0},{1}", tile.tile.x, tile.tile.y)));
                    xmltile.Add(new XAttribute("screen", string.Format("{0},{1}", tile.screen.x, tile.screen.y)));
                    xmltile.Add(new XAttribute("coords", string.Format("{0},{1},{2},{3}", tile.coords.x, tile.coords.y, tile.coords.w, tile.coords.h)));

                    /*                    xmltile.Add(new XAttribute("mapx", tile.tile.x));
                                        xmltile.Add(new XAttribute("mapy", tile.tile.y));

                                        xmltile.Add(new XAttribute("screeny", tile.screen.x));
                                        xmltile.Add(new XAttribute("screenx", tile.screen.y));

                                        xmltile.Add(new XAttribute("atlasx", tile.coords.x));
                                        xmltile.Add(new XAttribute("atlasy", tile.coords.y));
                                        xmltile.Add(new XAttribute("atlaswidth", tile.coords.w));
                                        xmltile.Add(new XAttribute("atlasheight", tile.coords.h));*/

                    xmltiles.Add(xmltile);
                }

                xmllayer.Add(xmltiles);

                xmllayers.Add(xmllayer);
            }

            xmlbody.Add(xmllayers);
            xmldoc.Add(xmlbody);
            xmldoc.Save(filename);

        }

    }


}