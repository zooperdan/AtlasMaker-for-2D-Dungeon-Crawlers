using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{

    public GameObject model;

    public void EnableDithering(bool value)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.sharedMaterial.SetInt("Boolean_989138A6", value ? 1 : 0);
        }

    }

    public void SetTexture(Texture2D texture)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.sharedMaterial.SetTexture("Texture2D_267808DD", texture);
        }

    }

}
