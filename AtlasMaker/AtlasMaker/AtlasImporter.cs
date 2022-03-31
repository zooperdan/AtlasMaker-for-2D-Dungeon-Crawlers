using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class AtlasImporter : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D import)
    {
        TextureImporter importer = assetImporter as TextureImporter;
        //importer.textureCompression = TextureImporterCompression.Uncompressed;
        //importer.mipmapEnabled = true;
        //importer.filterMode = FilterMode.Point;
        //importer.npotScale = TextureImporterNPOTScale.None;
        //importer.wrapMode = TextureWrapMode.Repeat;
        //import.filterMode = FilterMode.Point;
    }
}
