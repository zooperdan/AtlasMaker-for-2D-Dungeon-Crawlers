using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zooperdan.AtlasMaker
{

    [Serializable]
    public enum LightMode
    {
        None,
        Point,
        Directional
    }

    [Serializable]
    public struct ScreenSize
    {
        public int width;
        public int height;
    }

    [CreateAssetMenu(fileName = "AtlasMakerSettings", menuName = "zooperdan/AtlasMaker/AtlasMakerSettings", order = 1)]
    public class AtlasMakerSettings : ScriptableObject
    {
        public FilterMode filterMode = FilterMode.Point;
        public string outputPath = "";
        public int dungeonDepth = 5;
        public int dungeonWidth = 3;
        //public Size screenSize = new Size();
        [SerializeField]
        public ScreenSize screenSize = new ScreenSize { width = 320, height = 256 };
        public List<Atlas> atlases = new List<Atlas>();

        // camera settings

        public float fov = 35f;
        public float offsetY = 0.6f;
        public float offsetZ = 1.0f;
        public float lensShiftY = -0.2f;

        // light settings

        public LightMode lightMode = LightMode.None;

        // point light settings

        public Vector3 pointLightPosition = new Vector3(0f, 0.5f, 0f);
        public float pointLightRange = 10f;
        public float pointLightIntensity = 1f;
        public Color pointLightColor = Color.white;
        public LightShadows pointLightShadows = LightShadows.None;
        public float pointLightShadowStrength = 1f;

        // directional light settings

        public Vector3 directionalLightRotation = new Vector3(45f, -15f, 0f);
        public float directionalLightIntensity = 1f;
        public Color directionalLightColor = Color.white;
        public LightShadows directionalLightShadows = LightShadows.None;
        public float directionalLightShadowStrength = 1f;

        // render settings

        public bool fog;
        public Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        public FogMode fogMode = FogMode.ExponentialSquared;
        public float fogDensity = 0.01f;
        public float linearFogStart = 0.0f;
        public float linearFogEnd = 300.0f;

        public Color ambientColor = Color.white;

        // other settings

        public Color previewBackgroundColor = Color.black;
        public bool previewFiltering = false;
        public Texture2D paletteTexture;
        public bool ditherAtlas = true;
        public float ditherAmount = 500f;

    }

}