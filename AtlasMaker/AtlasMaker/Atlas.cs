using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zooperdan.AtlasMaker
{
    
    [CreateAssetMenu(fileName = "Atlas", menuName = "zooperdan/AtlasMaker/Atlas", order = 1)]
    public class Atlas : ScriptableObject
    {
        public string id;
        [HideInInspector]
        public bool enabled = true;
        public List<AtlasLayer> layers = new List<AtlasLayer>();
    }

}