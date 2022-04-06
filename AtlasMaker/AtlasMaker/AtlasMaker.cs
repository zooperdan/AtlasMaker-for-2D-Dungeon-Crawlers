using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace zooperdan.AtlasMaker
{
    [Serializable]
    public struct YUVStruct
    {
        public double y;
        public double u;
        public double v;
    }


    [Serializable]
    public class Palette
    {
        public List<Color> colors = new List<Color>();
        
        public void Clear()
        {
            this.colors.Clear();
        }

        public void ColorsFromTexture(Texture2D texture)
        {
            this.Clear();

            Color[] cols = texture.GetPixels();

            foreach (Color col in cols)
            {
                this.AddColor(col);
            }

        }

        private void AddColor(Color color)
        {

            foreach (Color col in this.colors)
            {
                if ((col.r != color.r) || (col.g != color.g) || (col.b != color.b))
                {
                    this.colors.Add(color);
                    return;
                }
            }

            this.colors.Add(color);

        }
    }

    [Serializable]
    public struct Vector4Int
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public Vector4Int(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
        public override string ToString()
        {
            return this.x + " : " + this.y + " : " + this.w + " : " + this.h;
        }
    }

    [Serializable]
    public struct Vector2Int
    {
        public int x;
        public int y;
        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [Serializable]
    public struct Size
    {
        public int width;
        public int height;
    }

    public class GeneratedImageResult
    {
        public JSONTile tile;
        public GeneratedImage image;
    }

    public class RenderResult
    {
        public Texture2D[] textures;
        public Rect[] rects;
    }

    [Serializable]
    public class JSONTile
    {
        public string type;
        public Vector2Int tile = new Vector2Int();
        public Vector2Int screen = new Vector2Int();
        public Vector4Int coords = new Vector4Int();
    }

    [Serializable]
    public class JSONLayer
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int id;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int mode;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int type;
        [SerializeField]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JSONTile> tiles = new List<JSONTile>();

        // scale
        // offset
    }

    [Serializable]
    public class JSONResult
    {
        public string version;
        public string generated;
        public ScreenSize resolution;
        public int depth;
        public int width;
        [SerializeField]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JSONLayer> layers = new List<JSONLayer>();

    }

    public class GenerateResult
    {
        public const int GENERATE_FAILED = 0;
        public const int GENERATE_SUCCESS = 1;
        public int status;
        public List<string> message = new List<string>();
    }

    public struct GrabResult
    {
        public const int GENERATE_FAILED = 0;
        public const int GENERATE_SUCCESS = 1;
        public int status;
        public Texture2D texture;
        public Vector4Int coords;
    }

    public struct CropResult
    {
        public const int CROP_FAILED = 0;
        public const int CROP_SUCCESS = 1;
        public int status;
        public Texture2D texture;
        public Vector4Int coords;
    }

    [Serializable]
    public class LayerItem
    {
        public AtlasLayer layer;
    }


    [Serializable]
    public class GeneratedImage
    {
        public int layerIndex;
        public Vector2Int position;
        public Vector4Int coords;
        public Texture2D texture;

        public GeneratedImage(int layerIndex, Texture2D texture, Vector2Int position, Vector4Int coords)
        {
            this.layerIndex = layerIndex;
            this.position = position;
            this.coords = coords;
            this.texture = texture;
        }

    }

    [Serializable]
    public class DataContainer
    {

    }

    public class AtlasMaker : EditorWindow
    {
        public static AtlasMaker Instance { get; private set; }

        private AtlasMakerSettings _settings;
        GameObject atlasMakerCamera;

        private const string VERSION_NUMBER = "0.9.3";

        private List<Vector2Int> _squaresToGenerateList = new List<Vector2Int>();
        private DataContainer _dataContainer = new DataContainer();
        private GenerateResult _generateResult = new GenerateResult();
        private List<string> _validateResult = new List<string>();
        private bool _isGenerating = false;
        private bool _doneGenerating = false;
        private string _generateLog = "";
        private bool _showCameraSettings = false;
        private bool _showRenderSettings = false;
        private bool _showLightSettings = false;
        private bool _showMiscSettings = false;
        private Texture2D atlasTexture;
        private static Texture2D _backgroundTex;
        private Palette _palette = new Palette();

        private Camera _viewportCamera;
        private RenderTexture _renderTexture;
        private GameObject _wallTemplate;
        private static Material _previewMaterial;

        [MenuItem("zooperdan/AtlasMaker %g")]
        private static void Init()
        {
            _backgroundTex = MakeTex(1, 1, Color.black);

            AtlasMaker window = (AtlasMaker)EditorWindow.GetWindow(typeof(AtlasMaker), false, "AtlasMaker");
            window.minSize = new Vector2(150.0f, 200.0f);
            window.wantsMouseMove = true;
            window.Show();
            EditorWindow.FocusWindowIfItsOpen(typeof(AtlasMaker));
        }

        void OnGUI()
        {

            if (!atlasMakerCamera)
            {
                atlasMakerCamera = (GameObject)Resources.Load("AtlasMakerCamera") as GameObject;
            }



            if (!_previewMaterial)
            {
                _previewMaterial = new Material(Shader.Find("Unlit/Transparent"));
            }

            float toolbarWidth = 300;

            Rect rect = new Rect(0, 0, toolbarWidth, Screen.height);

            GUILayout.BeginArea(rect);

            EditorGUILayout.Space();
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.padding.left = 0;
            style.margin.left = 0;
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("AtlasMaker " + VERSION_NUMBER.ToString(), style);

            style.padding.left = 0;
            style.margin.left = 0;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Normal;

            EditorGUILayout.LabelField("by zooperdan", style);
            GUILayout.EndVertical();
            EditorGUILayout.Space();


            if (!_isGenerating)
            {
                ShowDefaultControls();
            }
            else
            {
                ShowGenerateControls();
            }

            GUILayout.EndArea();

            float previewWidth = Screen.width - toolbarWidth / 2;

            rect = new Rect(toolbarWidth / 2, 0, previewWidth, Screen.height);

            GUILayout.BeginArea(rect);

            if (_settings)
            {
                _backgroundTex = MakeTex(1, 1, _settings.previewBackgroundColor);
            }
            else
            {
                _backgroundTex = MakeTex(1, 1, Color.black);
            }

            GUI.DrawTexture(rect, _backgroundTex, ScaleMode.StretchToFill);

            if (atlasTexture && _settings)
            {
                atlasTexture.filterMode = _settings.previewFiltering ? FilterMode.Bilinear : FilterMode.Point;
                EditorGUI.DrawPreviewTexture(new Rect((toolbarWidth / 2) + 10, 10, previewWidth - ((toolbarWidth / 2) + 10), Screen.height - 40), atlasTexture, _previewMaterial, ScaleMode.ScaleToFit);
            }

            GUILayout.EndArea();

        }

        private void ShowGenerateControls()
        {

            GUILayout.BeginHorizontal();
            GUILayout.TextField(_generateLog, GUILayout.Width(100), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.EndHorizontal();

            if (_doneGenerating)
            {
                if (GUILayout.Button("Close"))
                {
                    if (_doneGenerating)
                    {
                        _doneGenerating = false;
                        _isGenerating = false;
                        EditorUtility.SetDirty(this);
                    }
                }

            }

        }

        private void ShowDefaultControls()
        {

            if (GUILayout.Button("Make", GUILayout.Height(40)))
            {

                if (!EmptyScene())
                {
                    return;
                }

                atlasTexture = null;
                _validateResult = Validate();
                if (_validateResult.Count == 0)
                {
                    _isGenerating = true;
                    _doneGenerating = false;
                    _generateLog = "";
                    EditorCoroutineUtility.StartCoroutine(GenerateAtlases(), this);
                }
            }

            if (_validateResult.Count > 0)
            {
                string text = string.Join("\n", _validateResult.ToArray());
                GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                style.richText = true;
                style.normal.background = MakeTex(32, 32, Color.black);
                style.fontSize = 12;
                GUILayout.BeginVertical();
                EditorGUILayout.TextArea(text, style);
                GUILayout.EndVertical();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Settings file", GUILayout.Width(100));
            _settings = (AtlasMakerSettings)EditorGUILayout.ObjectField(_settings, typeof(AtlasMakerSettings), true);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (_settings)
            {

                GUILayout.BeginHorizontal();
                GUILayout.Label("Dungeon depth", GUILayout.Width(100));
                _settings.dungeonDepth = EditorGUILayout.IntField(_settings.dungeonDepth);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Dungeon width", GUILayout.Width(100));
                _settings.dungeonWidth = EditorGUILayout.IntField(_settings.dungeonWidth);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Filter Mode", GUILayout.Width(100));
                _settings.filterMode = (FilterMode)EditorGUILayout.EnumPopup(_settings.filterMode);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Screen size", GUILayout.Width(100));
                _settings.screenSize.width = EditorGUILayout.IntField(_settings.screenSize.width);
                _settings.screenSize.height = EditorGUILayout.IntField(_settings.screenSize.height);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Output folder", GUILayout.Width(100));
                _settings.outputPath = EditorGUILayout.TextField(_settings.outputPath);
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                _showCameraSettings = EditorGUILayout.Foldout(_showCameraSettings, "Camera settings");

                if (_showCameraSettings)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.Width(20));
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Field of view");
                    _settings.fov = EditorGUILayout.FloatField(_settings.fov);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Y offset");
                    _settings.offsetY = EditorGUILayout.FloatField(_settings.offsetY);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Z offset");
                    _settings.offsetZ = EditorGUILayout.FloatField(_settings.offsetZ);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Lens shift Y");
                    _settings.lensShiftY = EditorGUILayout.FloatField(_settings.lensShiftY);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }

                _showLightSettings = EditorGUILayout.Foldout(_showLightSettings, "Light settings");

                if (_showLightSettings)
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.Width(20));
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Light Mode");
                    _settings.lightMode = (LightMode)EditorGUILayout.EnumPopup(_settings.lightMode);
                    GUILayout.EndHorizontal();

                    switch (_settings.lightMode)
                    {
                        case LightMode.Point:
                            {

                                GUILayout.BeginHorizontal();

                                GUILayout.Label("Offset");
                                _settings.pointLightPosition = EditorGUILayout.Vector3Field("", _settings.pointLightPosition);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();

                                GUILayout.Label("Color");
                                _settings.pointLightColor = EditorGUILayout.ColorField(_settings.pointLightColor);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();

                                GUILayout.Label("Range");
                                _settings.pointLightRange = EditorGUILayout.FloatField(_settings.pointLightRange);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();

                                GUILayout.Label("Intensity");
                                _settings.pointLightIntensity = EditorGUILayout.FloatField(_settings.pointLightIntensity);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Shadows");
                                _settings.pointLightShadows = (LightShadows)EditorGUILayout.EnumPopup(_settings.pointLightShadows);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Shadow strength");
                                _settings.pointLightShadowStrength = EditorGUILayout.FloatField(_settings.pointLightShadowStrength);
                                GUILayout.EndHorizontal();


                            }
                            break;

                        case LightMode.Directional:
                            {

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Rotation");
                                _settings.directionalLightRotation = EditorGUILayout.Vector3Field("", _settings.directionalLightRotation);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();

                                GUILayout.Label("Intensity");
                                _settings.directionalLightIntensity = EditorGUILayout.FloatField(_settings.directionalLightIntensity);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Color");
                                _settings.directionalLightColor = EditorGUILayout.ColorField(_settings.directionalLightColor);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Shadows");
                                _settings.directionalLightShadows = (LightShadows)EditorGUILayout.EnumPopup(_settings.directionalLightShadows);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Shadow strength");
                                _settings.directionalLightShadowStrength = EditorGUILayout.FloatField(_settings.directionalLightShadowStrength);
                                GUILayout.EndHorizontal();

                            }
                            break;

                        default:
                            {

                            }
                            break;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }

                _showRenderSettings = EditorGUILayout.Foldout(_showRenderSettings, "Render settings");

                if (_showRenderSettings)
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.Width(20));
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Fog");
                    _settings.fog = EditorGUILayout.Toggle(_settings.fog);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Fog color");
                    _settings.fogColor = EditorGUILayout.ColorField(_settings.fogColor);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Fog mode");
                    _settings.fogMode = (FogMode)EditorGUILayout.EnumPopup(_settings.fogMode);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Fog density");
                    _settings.fogDensity = EditorGUILayout.FloatField(_settings.fogDensity);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Linear fog start");
                    _settings.linearFogStart = EditorGUILayout.FloatField(_settings.linearFogStart);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Linear fog end");
                    _settings.linearFogEnd = EditorGUILayout.FloatField(_settings.linearFogEnd);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Ambient color");
                    _settings.ambientColor = EditorGUILayout.ColorField(_settings.ambientColor);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                }

                _showMiscSettings = EditorGUILayout.Foldout(_showMiscSettings, "Other settings");

                if (_showMiscSettings)
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.Width(20));
                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();

                    EditorGUI.BeginChangeCheck();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Preview background color");
                    _settings.previewBackgroundColor = EditorGUILayout.ColorField(_settings.previewBackgroundColor);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        _backgroundTex = MakeTex(1, 1, _settings.previewBackgroundColor);
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Preview filtering");
                    _settings.previewFiltering = EditorGUILayout.Toggle(_settings.previewFiltering);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Palette");
                    _settings.paletteTexture = (Texture2D)EditorGUILayout.ObjectField(_settings.paletteTexture, typeof(Texture2D), true);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Dither output");
                    _settings.ditherAtlas = EditorGUILayout.Toggle(_settings.ditherAtlas);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Dither amount");
                    _settings.ditherAmount = EditorGUILayout.FloatField(_settings.ditherAmount);
                    GUILayout.EndHorizontal();


                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                ShowAtlases();

                EditorUtility.SetDirty(_settings);

            }

        }

        private bool EmptyScene()
        {

            UnityEngine.Object[] objs = GameObject.FindObjectsOfType<GameObject>();

            if (objs.Length > 0)
            {
                if (!EditorUtility.DisplayDialog("Scene is not empty.", "All objects will be removed. Continue?", "Ok", "Cancel"))
                {
                    return false;
                }

            }

            foreach (UnityEngine.Object o in objs)
            {
                DestroyImmediate(o);
            }

            return true;

        }

        public void DoneGenerating()
        {
            _doneGenerating = true;
            _isGenerating = false;
            EditorUtility.SetDirty(this);
            EditorApplication.Beep();
        }

        public void Log(string value)
        {
            _generateLog += value;
            EditorUtility.SetDirty(this);
        }

        IEnumerator GenerateAtlases()
        {

            string destPath = AddTrailingSlash(_settings.outputPath);

            Log("Atlas creation started.\n");
            yield return null;

            if (!_wallTemplate)
            {
                _wallTemplate = (GameObject)Resources.Load("WallTemplate") as GameObject;
            }

            GameObject cameraObject = Instantiate(atlasMakerCamera);
            //cameraObject.transform.position = new Vector3(0f, 0f, -0.5f);

            _viewportCamera = cameraObject.GetComponentInChildren<Camera>();

            _viewportCamera.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            UnityEngine.Rendering.PostProcessing.PostProcessLayer ppl = _viewportCamera.gameObject.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            ppl.volumeLayer = ~0;

            _renderTexture = new RenderTexture(_settings.screenSize.width, _settings.screenSize.height, 24, RenderTextureFormat.Default)
            {
                filterMode = _settings.filterMode
            };

            _viewportCamera.targetTexture = _renderTexture;

            // apply custom values to camera and rendersettings

            _viewportCamera.fieldOfView = _settings.fov;
            _viewportCamera.transform.localPosition = new Vector3(0f, _settings.offsetY, _settings.offsetZ);
            _viewportCamera.lensShift = new Vector2(0, _settings.lensShiftY);

            RenderSettings.skybox = null;
            RenderSettings.fog = _settings.fog;
            RenderSettings.fogColor = _settings.fogColor;
            RenderSettings.fogMode = _settings.fogMode;
            RenderSettings.fogDensity = _settings.fogDensity;
            RenderSettings.fogStartDistance = _settings.linearFogStart;
            RenderSettings.fogEndDistance = _settings.linearFogEnd;
            RenderSettings.ambientEquatorColor = _settings.ambientColor;
            RenderSettings.ambientGroundColor = _settings.ambientColor;
            RenderSettings.ambientSkyColor = _settings.ambientColor;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

            // apply light settings

            AtlasMakerCam camScript = cameraObject.GetComponent<AtlasMakerCam>();

            switch (_settings.lightMode)
            {
                case LightMode.Point:
                    {
                        camScript.pointLight.gameObject.SetActive(true);
                        camScript.directionalLight.gameObject.SetActive(false);

                        camScript.pointLight.transform.localPosition = _settings.pointLightPosition;
                        camScript.pointLight.range = _settings.pointLightRange;
                        camScript.pointLight.intensity = _settings.pointLightIntensity;
                        camScript.pointLight.color = _settings.pointLightColor;
                        camScript.pointLight.shadows = _settings.pointLightShadows;
                        camScript.pointLight.shadowStrength = _settings.pointLightShadowStrength;
                    }
                    break;
                case LightMode.Directional:
                    {
                        camScript.pointLight.gameObject.SetActive(false);
                        camScript.directionalLight.gameObject.SetActive(true);

                        camScript.directionalLight.transform.localEulerAngles = _settings.directionalLightRotation;
                        camScript.directionalLight.intensity = _settings.directionalLightIntensity;
                        camScript.directionalLight.color = _settings.directionalLightColor;
                        camScript.directionalLight.shadows = _settings.directionalLightShadows;
                        camScript.directionalLight.shadowStrength = _settings.directionalLightShadowStrength;
                    }
                    break;
                default:
                    {
                        camScript.pointLight.gameObject.SetActive(false);
                        camScript.directionalLight.gameObject.SetActive(false);
                    }
                    break;
            }

            // iterate through all atlases and their layers

            for (int atlasIndex = 0; atlasIndex < _settings.atlases.Count; atlasIndex++)
            {
                if (_settings.atlases[atlasIndex].enabled)
                {
                    RenderAtlas(_settings.atlases[atlasIndex], destPath);
                }
            }

            RenderTexture.active = null;
            DestroyImmediate(cameraObject);

            Repaint();
            DoneGenerating();

            yield return new WaitForSeconds(1);

        }

        private void setPalette()
        {
            if (!atlasTexture)
            {
                return;
            }

            if (!_settings.paletteTexture)
            {
                return;
            }

            // convert the texture into a palette

            _palette.ColorsFromTexture(_settings.paletteTexture);

            int mipCount = Mathf.Min(3, atlasTexture.mipmapCount);

            Color[] cols = atlasTexture.GetPixels();

            if (_settings.ditherAtlas)
            {
                cols = ditherImageData4x4(cols, atlasTexture.width, atlasTexture.height, _settings.ditherAmount);
            }

            for (int i = 0; i < cols.Length; ++i)
            {
                Color nearestColor = Color.red;
                if (findNearestColor(cols[i], out nearestColor))
                {
                    cols[i] = nearestColor;
                }
            }

            atlasTexture.SetPixels(cols);
            atlasTexture.Apply(false);

        }

        private YUVStruct YUVfromRGB(Color color)
        {
            double y = 0.257 * color.r + 0.504 * color.g + 0.098 * color.b + 16;
            double u = -0.148 * color.r - 0.291 * color.g + 0.439 * color.b + 128;
            double v = 0.439 * color.r - 0.368 * color.g - 0.071 * color.b + 128;
            double weight_y = 5;
            double weight_u = 1;
            double weight_v = 1;
            y *= weight_y;
            u *= weight_u;
            v *= weight_v;
            return new YUVStruct { y = y, u = u, v = v };
        }

        private double GetColorDistanceRGB(Color current, Color match)
        {
            double redDifference;
            double greenDifference;
            double blueDifference;

            redDifference = current.r - match.r;
            greenDifference = current.g - match.g;
            blueDifference = current.b - match.b;

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
        }

        private double GetColorDistanceYUV(Color current, Color match)
        {
            double yDifference;
            double uDifference;
            double vDifference;

            YUVStruct yuvCurrent = this.YUVfromRGB(current);
            YUVStruct yuvMatch = this.YUVfromRGB(match);

            yDifference = yuvCurrent.y - yuvMatch.y;
            uDifference = yuvCurrent.u - yuvMatch.u;
            vDifference = yuvCurrent.v - yuvMatch.v;

            return yDifference * yDifference + uDifference * uDifference + vDifference * vDifference;
        }

        private Color[] ditherImageData4x4(Color[] cols, int width, int height, float ditherAmount)
        {

            int[,] threshold_maps = new int[,]
            {
                { 0, 48, 12, 60, 3, 51, 15, 63},
                {32, 16, 44, 28, 35, 19, 47, 31},
                {8, 56, 4, 52, 11, 59, 7, 55},
                {40, 24, 36, 20, 43, 27, 39, 23},
                {2, 50, 14, 62, 1, 49, 13, 61},
                {34, 18, 46, 30, 33, 17, 45, 29},
                {10, 58, 6, 54, 9, 57, 5, 53},
                {42, 26, 38, 22, 41, 25, 37, 21}
            };

            float depth = 1;
            int moo = 8;
            int x, y;
            int a;
            float b;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    a = (x + (y*width));
                    b = threshold_maps[x % moo,y % moo] / (32 - ditherAmount);
                    cols[a].r = ((cols[a].r + b) / depth) * depth;
                    cols[a].g = ((cols[a].g + b) / depth) * depth;
                    cols[a].b = ((cols[a].b + b) / depth) * depth;
                }
            }

            return cols;

        }

        private bool findNearestColor(Color srcColor, out Color nearestColor)
        {

            double shortestDistance = 9007199254740991;

            nearestColor = Color.black;

            for (var i = 0; i < _palette.colors.Count; i++)
            {
                double distance = GetColorDistanceRGB(srcColor, _palette.colors[i]);

                if (distance < shortestDistance)
                {
                    nearestColor = _palette.colors[i];
                    nearestColor.a = srcColor.a;
                    shortestDistance = distance;
                }
            }

            return true;
        }

        private void RenderAtlas(Atlas atlas, string destPath)
        {

            JSONResult jsonResult = new JSONResult
            {
                version = VERSION_NUMBER.ToString(),
                generated = DateTime.Now.ToString(),
                depth = _settings.dungeonDepth,
                width = _settings.dungeonWidth,
                resolution = _settings.screenSize
            };

            GeneratedImageResult gri;

            List<GeneratedImage> generatedImages = new List<GeneratedImage>();

            for (int layerIndex = 0; layerIndex < atlas.layers.Count; layerIndex++)
            {

                if (!atlas.layers[layerIndex].enabled)
                {
                    continue;
                }


                // override dungeondepth or width?
                int dd = atlas.layers[layerIndex].dungeonDepth > 0 ? atlas.layers[layerIndex].dungeonDepth : _settings.dungeonDepth;
                int dw = atlas.layers[layerIndex].dungeonWidth > 0 ? atlas.layers[layerIndex].dungeonWidth : _settings.dungeonWidth;

                // which tiles to render?
                if (atlas.layers[layerIndex].renderMode == AtlasLayerRenderMode.ALL)
                {

                    _squaresToGenerateList.Clear();

                    for (int b = -dw; b < dw; b++)
                    {
                        for (int a = -dd; a <= 0; a++)
                        {
                            _squaresToGenerateList.Add(new Vector2Int(b, a));
                        }
                    }

                } else {

                    _squaresToGenerateList.Clear();
                    for (int b = 0; b > -dw; b--)
                    {
                        for (int a = 0; a <= dd; a++)
                        {
                            _squaresToGenerateList.Add(new Vector2Int(0 - b, 0 - a));
                        }
                    }

                }

                // create new jsonlayer

                JSONLayer jsonLayer = new JSONLayer();

                // instantiate the model for current layer

                GameObject model = Instantiate(atlas.layers[layerIndex].model);

                // grab the wallscript located on the model

                Wall wallScript = model.GetComponent<Wall>();

                int squareIndex = 0;
                foreach (Vector2Int vec in _squaresToGenerateList)
                {

                    model.transform.position = new Vector3(vec.x, 0.0f, vec.y);

                    if (atlas.layers[layerIndex].type == AtlasLayerType.OBJECT)
                    {
                        switch (atlas.layers[layerIndex].renderSide)
                        {
                            case AtlasLayerSide.FRONT:
                                {
                                    model.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                                }
                                break;
                            case AtlasLayerSide.REAR:
                                {
                                    model.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                                }
                                break;
                            case AtlasLayerSide.LEFT:
                                {
                                    model.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
                                }
                                break;
                            case AtlasLayerSide.RIGHT:
                                {
                                    model.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                                }
                                break;
                        }
                    }


                    if (atlas.layers[layerIndex].renderMode == AtlasLayerRenderMode.MIDDLE)
                    {
                        /* render mode MIDDLE */

                        if (vec.x == 0)
                        {
                            if (wallScript != null)
                            {
                                wallScript.ShowFrontWall();
                            }

                            string typeName = (
                                atlas.layers[layerIndex].type == AtlasLayerType.WALL || atlas.layers[layerIndex].type == AtlasLayerType.DECAL
                                ) ? "front" : atlas.layers[layerIndex].type.ToString().ToLower();

                            float z = (atlas.layers[layerIndex].type == AtlasLayerType.OBJECT) ? -1 : 0;

                            if (vec.y <= z)
                            {
                                gri = GetGeneratedImage(atlas.layers[layerIndex], layerIndex, vec, typeName);
                                if (gri != null)
                                {
                                    jsonLayer.tiles.Add(gri.tile);
                                    generatedImages.Add(gri.image);
                                }
                            }

                        }

                    }
                    else
                    {
                        /* render mode LEFT or ALL */

                        switch (atlas.layers[layerIndex].type)
                        {
                            case AtlasLayerType.WALL:
                            case AtlasLayerType.DECAL:
                                {
                                    if (atlas.layers[layerIndex].renderMode == AtlasLayerRenderMode.ALL)
                                    {
                                        // front
                                        if (wallScript != null)
                                        {
                                            wallScript.ShowFrontWall();
                                        }
                                        gri = GetGeneratedImage(atlas.layers[layerIndex], layerIndex, vec, "front");
                                        if (gri != null)
                                        {
                                            jsonLayer.tiles.Add(gri.tile);
                                            generatedImages.Add(gri.image);
                                        }
                                    }
                                    else
                                    {
                                        if (squareIndex <= _settings.dungeonDepth)
                                        {
                                            // front
                                            if (wallScript != null)
                                            {
                                                wallScript.ShowFrontWall();
                                            }
                                            gri = GetGeneratedImage(atlas.layers[layerIndex], layerIndex, vec, "front");
                                            if (gri != null)
                                            {
                                                jsonLayer.tiles.Add(gri.tile);
                                                generatedImages.Add(gri.image);
                                            }
                                        }
                                    }

                                    // side
                                    if (vec.x >= 0)
                                    {
                                        if (wallScript != null)
                                        {
                                            wallScript.ShowSideWall();
                                        }
                                        gri = GetGeneratedImage(atlas.layers[layerIndex], layerIndex, vec, "side");
                                        if (gri != null)
                                        {
                                            jsonLayer.tiles.Add(gri.tile);
                                            generatedImages.Add(gri.image);
                                        }
                                    }
                                }
                                break;
                            case AtlasLayerType.OBJECT:
                                {
                                    if (!(vec.y == 0 && vec.x == 0))
                                    {

                                        gri = GetGeneratedImage(atlas.layers[layerIndex], layerIndex, vec, atlas.layers[layerIndex].type.ToString().ToLower());
                                        if (gri != null)
                                        {
                                            jsonLayer.tiles.Add(gri.tile);
                                            generatedImages.Add(gri.image);
                                        }

                                    }
                                }
                                break;
                            case AtlasLayerType.GROUND:
                            case AtlasLayerType.CEILING:
                                {
                                    if (vec.y <= 0)
                                    {
                                        gri = GetGeneratedImage(atlas.layers[layerIndex], layerIndex, vec, atlas.layers[layerIndex].type.ToString().ToLower());
                                        if (gri != null)
                                        {
                                            jsonLayer.tiles.Add(gri.tile);
                                            generatedImages.Add(gri.image);
                                        }
                                    }
                                }
                                break;
                        }

                        squareIndex++;
                    }

                }


                // remove model after use

                DestroyImmediate(model);

                // add processed layer to json container

                jsonLayer.id = layerIndex + 1;
                jsonLayer.mode = (int)atlas.layers[layerIndex].renderMode;
                jsonLayer.name = atlas.layers[layerIndex].id;
                jsonResult.layers.Add(jsonLayer);

            }

            // process the generated images and pack into atlas

            if (generatedImages.Count > 0)
            {

                Texture2D[] textures = new Texture2D[generatedImages.Count];
                int j = 0;
                foreach (GeneratedImage gi in generatedImages)
                {
                    textures[j] = gi.texture;
                    j++;
                }

                atlasTexture = new Texture2D(8192, 8192, TextureFormat.ARGB32, false);
                atlasTexture.wrapMode = TextureWrapMode.Clamp;
                atlasTexture.alphaIsTransparency = true;

                Rect[] rects = atlasTexture.PackTextures(textures, 3, 8192);

                setPalette();

                System.IO.File.WriteAllBytes(destPath + atlas.id + ".png", atlasTexture.EncodeToPNG());

                int rectIndex = 0;
                for (int i = 0; i < jsonResult.layers.Count; i++)
                {
                    for (int k = 0; k < jsonResult.layers[i].tiles.Count; k++)
                    {

                        int cw = (int)(atlasTexture.width * rects[rectIndex].width);
                        int ch = (int)(atlasTexture.height * rects[rectIndex].height);
                        int cx = (int)(atlasTexture.width * rects[rectIndex].x);
                        int cy = (int)(atlasTexture.height * rects[rectIndex].y);

                        cy = atlasTexture.height - (cy + ch);

                        Vector4Int coords = new Vector4Int(cx, cy, cw, ch);
                        jsonResult.layers[i].tiles[k].coords = coords;
                        rectIndex++;
                    }
                }

                string json = JsonConvert.SerializeObject(jsonResult);
                
                string jsonFilename = destPath + atlas.id + ".json";
                System.IO.File.WriteAllText(jsonFilename, json);

            }

        }

        private GeneratedImageResult GetGeneratedImage(AtlasLayer atlaslayer, int layerIndex, Vector2Int vec, string typeName)
        {

            GeneratedImageResult result = new GeneratedImageResult();

            GrabResult gr = GrabViewport();

            if (gr.status == GrabResult.GENERATE_SUCCESS)
            {
                result.tile = new JSONTile();
                result.tile.type = typeName;
                result.tile.tile.x = vec.x;
                result.tile.tile.y = vec.y;
                result.tile.screen.x = gr.coords.x;
                result.tile.screen.y = gr.coords.y;
                result.image = new GeneratedImage(
                    layerIndex,
                    gr.texture,
                    vec,
                    gr.coords
                );
                return result;
            }

            return null;

        }

        private void ShowAtlases()
        {

            if (GUILayout.Button("Add atlas"))
            {
                _settings.atlases.Add(null);
            }

            for (int i = 0; i < _settings.atlases.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (_settings.atlases[i])
                {
                    _settings.atlases[i].enabled = EditorGUILayout.Toggle(_settings.atlases[i].enabled, GUILayout.Width(20));
                }
                _settings.atlases[i] = (Atlas)EditorGUILayout.ObjectField(_settings.atlases[i], typeof(Atlas), true);
                if (GUILayout.Button("-", GUILayout.Width(24)))
                {
                    _settings.atlases.RemoveAt(i);
                }
                EditorUtility.SetDirty(_settings);
                GUILayout.EndHorizontal();
            }

        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);
            result.wrapMode = TextureWrapMode.Clamp;
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        bool IsfullTransparent(Texture2D tex)
        {
            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                    if (tex.GetPixel(x, y).a != 0)
                        return false;
            return true;
        }

        private string AddTrailingSlash(string value)
        {
            return value.EndsWith("/") ? value : value + "/";
        }

        private string DeleteAllFiles(string path)
        {

            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                return "";
            }
            catch (Exception e)
            {
                return "Failed to delete files in output folder: " + e.ToString();
            }
            finally { }

        }

        public bool IsValidFolder(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public List<string> Validate()
        {

            Dictionary<string, int> atlasFilenames = new Dictionary<string, int>();
            Dictionary<string, int> atlasLayerIDs = new Dictionary<string, int>();

            List<string> result = new List<string>();

            int index = 0;

            if (!_settings)
            {
                result.Add("You need to assign an Atlas Maker Settings file");
                return result;
            }

            if (_settings.paletteTexture && !_settings.paletteTexture.isReadable)
            {
                result.Add("The palette texture is not readable. You can make the texture readable in the Texture Import Settings.");
                return result;
            }

            if (!(_settings.dungeonDepth >= 1 && _settings.dungeonDepth <= 10))
            {
                result.Add("Valid values for dungeon depth are 1 to 10");
                return result;
            }

            if (!(_settings.dungeonWidth >= 1 && _settings.dungeonWidth <= 10))
            {
                result.Add("Valid values for dungeon width are 1 to 10");
                return result;
            }

            if (!IsValidFolder(_settings.outputPath.Trim()))
            {
                result.Add("You need to specify a valid output folder.");
                return result;
            }


            if (_settings.atlases.Count == 0)
            {
                result.Add("There are no atlases to make.");
                return result;
            }

            foreach (Atlas atlas in _settings.atlases)
            {

                if (atlas == null)
                {
                    result.Add("Unassigned atlas at index " + index);
                }
                else if (atlas.id == "")
                {
                    result.Add("'" + atlas.name + "' > Missing ID.");
                }
                else if (atlas.id != "")
                {
                    if (atlasFilenames.ContainsKey(atlas.id))
                    {
                        result.Add("'" + atlas.name + "' > Duplicate filename: '" + atlas.id + "'");
                    }
                    else
                    {
                        atlasFilenames.Add(atlas.id, 1);
                    }
                }

                if (atlas != null)
                {

                    atlasLayerIDs.Clear();

                    foreach (AtlasLayer layer in atlas.layers)
                    {
                        if (layer == null)
                        {
                            result.Add("'" + atlas.name + "' > One or more unassigned atlas layers");
                        }
                        else if (layer.model == null)
                        {
                            result.Add("'" + atlas.name + "' > Missing model on '" + layer.name + "'");
                        }
                    }

                }

                index++;
            }

            return result;

        }

        private CropResult CropTexture(Texture2D tex)
        {

            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;

            // get left

            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                    if (tex.GetPixel(x, (tex.height - 1) - y).a != 0)
                    {
                        x1 = x;
                        goto foundX1;
                    }

                foundX1:

            // get top

            for (int y = 0; y < tex.height; y++)
                for (int x = 0; x < tex.width; x++)
                    if (tex.GetPixel(x, (tex.height - 1) - y).a != 0)
                    {
                        y1 = y;
                        goto foundY1;
                    }

                foundY1:

            // get right

            for (int x = tex.width - 1; x >= 0; x--)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    if (tex.GetPixel(x, (tex.height - 1) - y).a != 0)
                    {
                        x2 = x;
                        goto foundX2;
                    }
                }
            }

        foundX2:

            // get bottom

            for (int y = 0; y < tex.height; y++)
                for (int x = 0; x < tex.width; x++)
                    if (tex.GetPixel(x, y).a != 0)
                    {
                        y2 = (tex.height - 1) - y;
                        goto foundY2;
                    }

                foundY2:


            int w = (x2 - x1) + 1;
            int h = (y2 - y1) + 1;

            w = (int)Mathf.Clamp(w, 0, 10000);
            h = (int)Mathf.Clamp(h, 0, 10000);

            CropResult result = new CropResult();

            if (w > 0 && h > 0)
            {
                result.texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
                result.texture.wrapMode = TextureWrapMode.Clamp;
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        result.texture.SetPixel(x, (h - 1) - y, tex.GetPixel(x + x1, (tex.height - 1) - (y + y1)));
                result.status = CropResult.CROP_SUCCESS;
                result.coords = new Vector4Int(x1, y1, w, h);
            }
            else
            {
                result.status = CropResult.CROP_FAILED;
            }

            return result;

        }

        private GrabResult GrabViewport()
        {

            GrabResult result = new GrabResult();

            // force a render update

            _viewportCamera.Render();

            // grab the viewport texture

            Texture2D tex = new Texture2D(_viewportCamera.targetTexture.width, _viewportCamera.targetTexture.height, TextureFormat.ARGB32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = _viewportCamera.targetTexture;
            tex.ReadPixels(new Rect(0, 0, _viewportCamera.targetTexture.width, _viewportCamera.targetTexture.height), 0, 0);
            tex.Apply();

            // check if texture is 100% transparent pixels

            if (IsfullTransparent(tex))
            {
                // ignore this texture
                result.status = GenerateResult.GENERATE_FAILED;
                return result;
            }

            // crop texture down to actual pixels

            CropResult cropResult = CropTexture(tex);

            if (cropResult.status == CropResult.CROP_SUCCESS)
            {

                RenderTexture.active = null;

                // return success

                result.status = GenerateResult.GENERATE_SUCCESS;
                result.texture = cropResult.texture;
                result.coords = cropResult.coords;

                return result;

            }
            else
            {
                // CropTexture returned empty texture which should never happen.
                result.status = GenerateResult.GENERATE_FAILED;
                return result;

            }


        }


    }

}
