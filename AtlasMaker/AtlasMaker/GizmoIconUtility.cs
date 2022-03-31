using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Callbacks;

namespace zooperdan.AtlasMaker
{

    public class GizmoIconUtility
    {
        [DidReloadScripts]
        static GizmoIconUtility()
        {
            EditorApplication.projectWindowItemOnGUI = ItemOnGUI;
        }

        static void ItemOnGUI(string guid, Rect rect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            Atlas obj1 = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Atlas)) as Atlas;

            if (obj1 != null)
            {
                rect.width = rect.height + 3;
                GUI.DrawTexture(rect, (Texture2D)Resources.Load("AtlasIcon", typeof(Texture2D)));
            } else
            {

                AtlasLayer obj2 = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AtlasLayer)) as AtlasLayer;

                if (obj2 != null)
                {
                    rect.width = rect.height + 3;
                    GUI.DrawTexture(rect, (Texture2D)Resources.Load("AtlasLayerIcon", typeof(Texture2D)));
                }

            }


        }
    }
}