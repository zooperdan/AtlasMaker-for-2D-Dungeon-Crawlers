using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    public GameObject frontWall;
    public GameObject sideWall;


    public void ShowFrontWall()
    {
        frontWall.SetActive(true);
        sideWall.SetActive(false);
    }

    public void ShowSideWall()
    {
        sideWall.SetActive(true);
        frontWall.SetActive(false);
    }

}
