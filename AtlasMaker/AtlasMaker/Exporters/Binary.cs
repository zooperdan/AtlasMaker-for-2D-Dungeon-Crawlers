using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zooperdan.AtlasMaker
{

    public class Binary
    {

        public static void Save(string filename, JSONResult jsonResult)
        {

            List<byte> bytesList = new List<byte>();

            /*
                ----------------------------------------------
                Header
                ----------------------------------------------
                00  dungeon depth
                00  dungeon width
                00  screen width
                00
                00  screen height
                00
                00  layer count
                ----------------------------------------------
            */

            bytesList.Add(System.Convert.ToByte(jsonResult.depth));
            bytesList.Add(System.Convert.ToByte(jsonResult.width));
            bytesList.AddRange(IntToBytes(jsonResult.resolution.width));
            bytesList.AddRange(IntToBytes(jsonResult.resolution.height));
            bytesList.Add(System.Convert.ToByte(jsonResult.layers.Count));


            /*
                ----------------------------------------------
                Layer
                ----------------------------------------------
                00  Layer ID
                00  Layer Name String Length
                00  Layer Name String
                ..
                00  Layer Render Mode
                00  Layer Type String Length
                00  Layer Type
                ..
                00  Tiles count
                ----------------------------------------------
            */

            for (int layerIndex = 0; layerIndex < jsonResult.layers.Count; layerIndex++)
            {
                JSONLayer layer = jsonResult.layers[layerIndex];

                bytesList.Add(System.Convert.ToByte(layer.id));
                bytesList.Add(System.Convert.ToByte(layer.name.Length));
                bytesList.AddRange(StrToBytes(layer.name));
                bytesList.Add(System.Convert.ToByte(layer.mode));
                bytesList.Add(System.Convert.ToByte(layer.type.Length));
                bytesList.AddRange(StrToBytes(layer.type));
                bytesList.Add(System.Convert.ToByte(layer.tiles.Count));

                /*
                    ----------------------------------------------
                    Tile
                    ----------------------------------------------
                    00  Tile Type String Length
                    00  Tile Type String
                    ..
                    00  Tile Map X
                    00  Tile Map Y
                    00  Tile Screen X
                    00  
                    00  Tile Screen Y
                    00  
                    00  Tile Coords X
                    00  
                    00  Tile Coords Y
                    00  
                    00  Tile Coords Width
                    00  
                    00  Tile Coords Height
                    00  
                    ----------------------------------------------
                */

                for (int tileIndex = 0; tileIndex < layer.tiles.Count; tileIndex++)
                {

                    JSONTile tile = layer.tiles[tileIndex];

                    bytesList.Add(System.Convert.ToByte(tile.type.Length));
                    bytesList.AddRange(StrToBytes(tile.type));
                    bytesList.Add(IntToByte(tile.tile.x));
                    bytesList.Add(IntToByte(tile.tile.y));
                    bytesList.AddRange(IntToBytes(tile.screen.x));
                    bytesList.AddRange(IntToBytes(tile.screen.y));
                    bytesList.AddRange(IntToBytes(tile.coords.x));
                    bytesList.AddRange(IntToBytes(tile.coords.y));
                    bytesList.AddRange(IntToBytes(tile.coords.w));
                    bytesList.AddRange(IntToBytes(tile.coords.h));

                }
            }

            byte[] byteArray = bytesList.ToArray();

            System.IO.File.WriteAllBytes(filename, byteArray);

        }

        private static byte IntToByte(int value)
        {

            short vOut = System.Convert.ToInt16(value);

            byte[] result = System.BitConverter.GetBytes(vOut);

            if (System.BitConverter.IsLittleEndian)
                System.Array.Reverse(result);

            return result[1];

        }

        private static byte[] StrToBytes(string value)
        {

            byte[] result = System.Text.Encoding.ASCII.GetBytes(value);

            return result;
        }

        private static byte[] IntToBytes(int value)
        {

            ushort vOut = System.Convert.ToUInt16(value);
            
            byte[] result = System.BitConverter.GetBytes(vOut);

            if (System.BitConverter.IsLittleEndian)
                System.Array.Reverse(result);

            return result;

        }
    }


}