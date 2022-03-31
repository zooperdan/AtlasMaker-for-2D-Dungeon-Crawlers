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

    [CreateAssetMenu(fileName = "AtlasLayer", menuName = "zooperdan/AtlasMaker/AtlasLayer", order = 1)]
    public class AtlasLayer : ScriptableObject
    {
        public bool enabled = true;
        public GameObject model;
        public string id;
        public AtlasLayerType type = AtlasLayerType.WALL;
        public bool renderBothSides = false;
    }

}