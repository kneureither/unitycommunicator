using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPExtensionMethods
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

//Json Object Templates

[System.Serializable]
public class TcpConfigParameters
{
    public string host;
    public int[] ports;
}

[System.Serializable]
public class JSONCaptureParameters
{
    public string message;
    public int sceneID;
    public JSONCaptureCamObject Object0;
    public JSONCaptureLightObject Object1;
    public JSONCaptureLightObject Object2;
    public JSONCaptureLightObject Object3;
    public JSONCaptureGameObject Object4;
    public JSONCaptureGameObject Object5;
    public JSONCaptureGameObject Object6;

}

[System.Serializable]
public class JSONCaptureObject
{
    public float[] position;
    public float[] scale;
    public float[] rotation;
}

[System.Serializable]
public class JSONCaptureGameObject : JSONCaptureObject
{
    public float[] color;
    public float intensity;
    public float transparency;
    public float metallic;
    public float glossiness;
}

[System.Serializable]
public class JSONCaptureLightObject : JSONCaptureObject
{
    public float[] color;
    public float intensity;
    public bool active;
    public float range;
    public float spotAngle;
}
[System.Serializable]
public class JSONCaptureCamObject : JSONCaptureObject
{
    public bool orthographic;
    public float fieldOfView;
}



[System.Serializable]
public class JSONSceneShot
{
    public string status;
    public int sceneID;
    public int width;
    public int height;
    //public string png_string;
    // Multidimensional arrays can't be read by json serializer :-(
    //public int[,,] pixels;
    public int[] r_pixels;
    public int[] g_pixels;
    public int[] b_pixels;
    public int[] a_pixels;

    public JSONSceneShot(string status, int sceneID, int width, int height, int[] r_pixels, int[] g_pixels, int[] b_pixels, int[] a_pixels)
    {
        this.status = status;
        this.sceneID = sceneID;
        this.width = width;
        this.height = height;
        //this.png_string = png_string;
        this.r_pixels = r_pixels;
        this.g_pixels = g_pixels;
        this.b_pixels = b_pixels;
        this.a_pixels = a_pixels;
        //this.pixels = pixels;
    }
}


[System.Serializable]
public class JSONPNGmetadata
{
    public int sceneID;
    public string message;
    //Add whatever data you want or need to send

    public JSONPNGmetadata(int sceneID)
    {
        this.sceneID = sceneID;
        this.message = "This is a test message";
    }
}
