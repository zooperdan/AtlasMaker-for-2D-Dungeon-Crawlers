using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zooperdan.AtlasMaker
{
    // Examples of DECAL is doors, signs, buttons etc.
    // Examples of objects is barrel, chest, NPC etc.
    public enum AtlasLayerType
    {
        WALL = 0,
        GROUND = 1,
        CEILING = 2,
        DECAL = 3,
        OBJECT = 4
    };

    public enum AtlasLayerRenderMode
    {
        LEFT = 0,
        MIDDLE = 1,
        ALL = 2
    };

    public enum AtlasLayerSide
    {
        FRONT = 0,
        REAR = 1,
        LEFT = 2,
        RIGHT = 3
    };

    [CreateAssetMenu(fileName = "AtlasLayer", menuName = "zooperdan/AtlasMaker/AtlasLayer", order = 1)]
    public class AtlasLayer : ScriptableObject
    {
        public bool enabled = true;
        public GameObject model;
        public string id;
        public AtlasLayerType type = AtlasLayerType.WALL;
        public AtlasLayerRenderMode renderMode = AtlasLayerRenderMode.LEFT;
        public AtlasLayerSide renderSide = AtlasLayerSide.FRONT;

        [Header("Override dungeon depth (0 = Disabled)")]
        public int dungeonDepth = 0;
        [Header("Override dungeon width (0 = Disabled)")]
        public int dungeonWidth = 0;
    }

}