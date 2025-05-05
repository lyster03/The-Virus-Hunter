using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    
    void LateUpdate()
    {
        float pixelsPerUnit = 64f;
        float unitsPerPixel = 1f / pixelsPerUnit;

        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x / unitsPerPixel) * unitsPerPixel;
        pos.y = Mathf.Round(pos.y / unitsPerPixel) * unitsPerPixel;

        transform.position = new Vector3(pos.x, pos.y, -10f);
    }

}
