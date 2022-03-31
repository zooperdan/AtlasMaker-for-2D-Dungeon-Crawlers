using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zooperdan.AtlasMaker
{

    public enum LightMode
    {
        None,
        Point,
        Directional
    }


    [CreateAssetMenu(fileName = "AtlasMakerSettings", menuName = "zooperdan/AtlasMaker/AtlasMakerSettings", order = 1)]
    public class AtlasMakerSettings : ScriptableObject
    {
        public FilterMode filterMode = FilterMode.Point;
        public string outputPath = "";
        public int dungeonDepth = 5;
        public int dungeonWidth = 3;
        public Size screenSize = new Size();
        public List<Atlas> atlases = new List<Atlas>();

        // camera settings

        public float fov = 60f;
        public float offsetY = 0.5f;
        public float offsetZ = 2.05f;
        public float lensShiftY = 0.2f;

        // light settings

        public LightMode lightMode = LightMode.None;

        // point light settings

        public Vector3 pointLightPosition = new Vector3(0f, 0.5f, 0f);
        public float pointLightRange = 10f;
        public float pointLightIntensity = 1f;
        public Color pointLightColor = Color.white;

        // directional light settings

        public Vector3 directionalLightRotation = new Vector3(45f, -15f, 0f);
        public float directionalLightIntensity = 1f;
        public Color directionalLightColor = Color.white;

        // render settings

        public bool fog;
        public Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        public FogMode fogMode = FogMode.ExponentialSquared;
        public float fogDensity = 0.01f;
        public float linearFogStart = 0.0f;
        public float linearFogEnd = 300.0f;

        public Color ambientColor = Color.black;

        // other settings

        public Color previewBackgroundColor = Color.black;
    }

}