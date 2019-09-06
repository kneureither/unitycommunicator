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
    //public JSONCaptureCamObject Object0;
    //public JSONCaptureLightObject Object1;
    //public JSONCaptureLightObject Object2;
    //public JSONCaptureLightObject Object3;
    //public JSONCaptureGeomObject Object4;
    //public JSONCaptureGeomObject Object5;
    //public JSONCaptureGeomObject Object6;

    public JSONCaptureObjectGeneric Object0;
    public JSONCaptureObjectGeneric Object1;
    public JSONCaptureObjectGeneric Object2;
    public JSONCaptureObjectGeneric Object3;
    public JSONCaptureObjectGeneric Object4;
    public JSONCaptureObjectGeneric Object5;
    public JSONCaptureObjectGeneric Object6;

}

[System.Serializable]
public class JSONCaptureObject
{
    public float[] position;
    public float[] scale;
    public float[] rotation;
}

[System.Serializable]
public class JSONCaptureGeomObject : JSONCaptureObject
{
    public float[] color;
    public float? intensity;
    public float? transparency;
    public float? metallic;
    public float? glossiness;
}

[System.Serializable]
public class JSONCaptureLightObject : JSONCaptureObject
{
    public float[] color;
    public float? intensity;
    public bool? active;
    public float? range;
    public float? spotAngle;
}
[System.Serializable]
public class JSONCaptureCamObject : JSONCaptureObject
{
    public bool? orthographic;
    public float? fieldOfView;
}

[System.Serializable]
public class JSONCaptureObjectGeneric
{
    public float[] position;
    public float[] scale;
    public float[] rotation;
    public float[] color;
    public float[] colorOfLight;
    public float? intensity;
    public float? intensityOfLight;
    public float? transparency;
    public float? metallic;
    public float? glossiness;
    public int? active; //to be converted to bool, 0 = false, anything else is true
    public float? range;
    public float? spotAngle;
    public int? orthographic; //to be converted to bool, 0 = false, anything else is true
    public float? orthographicSize;
    public float? fieldOfView;

    public void SetParameters(GameObject targetObject)
    {
        Renderer rend = targetObject.GetComponent<Renderer>();
        Light light = targetObject.GetComponent<Light>();
        Camera camera = targetObject.GetComponent<Camera>();
        Vector3 rgb;
        float transp;

        Debug.Log("Set Parameters entered");

        //set transform
        if (this.position != null)
        {
            targetObject.transform.position = new Vector3(this.position[0], this.position[1], this.position[2]);
            Debug.Log("Position applied to " + targetObject.transform.position.ToString());
        }
        if (this.rotation != null)
        {
            targetObject.transform.eulerAngles = new Vector3(this.rotation[0], this.rotation[1], this.rotation[2]);
        }
        if (this.scale != null)
        {
            targetObject.transform.localScale = new Vector3(this.scale[0], this.scale[1], this.scale[2]);
        }


        if (rend != null)
        {
            //Set Color and Intensity
            if (this.color != null)
            {
                rgb = new Vector3(this.color[0], this.color[1], this.color[2]);
            }
            else
            {
                rgb = new Vector3(rend.material.color[0], rend.material.color[1], rend.material.color[2]);
            }
            if (this.transparency != null)
            {
                transp = (float)this.transparency;
            }
            else
            {
                transp = rend.material.color[3];
            }
            rend.material.color = new Color(rgb.x, rgb.y, rgb.z, transp);

            //metallic and glossiness
            if (this.metallic != null)
            {
                rend.material.SetFloat("_Metallic", (float)this.metallic);
            }
            if (this.glossiness != null)
            {
                rend.material.SetFloat("_Glossiness", (float)this.glossiness);
            }
        }
        else if(light != null)
        {
            //Set color and intenstity of light
            if (this.colorOfLight != null)
            {
                light.color = new Color(this.colorOfLight[0], this.colorOfLight[1], this.colorOfLight[2], 1);
            }
            if (this.intensityOfLight != null)
            {
                light.intensity = (float)intensityOfLight;
            }

            //Set active
            if (this.active != null)
            {
                if((int)this.active == 0)
                {
                    light.enabled = false;
                } else
                {
                    light.enabled = true;
                }

                
            }

            //light parameters
            if (this.range != null && (light.type == LightType.Spot || light.type == LightType.Point))
            {
                light.range = (float)this.range;
            }

            if (this.spotAngle != null && light.type == LightType.Spot)
            {
                light.spotAngle = (float)this.spotAngle;
            }
        }

        //camera parameters
        else if(camera != null)
        {
            Debug.Log("CAMERA");
            //Debug.Log(((int)this.orthographic).ToString());
            if (this.orthographic.Value != 0)
            {
                if ((int)this.orthographic == 0)
                {
                    camera.orthographic = false;
                }
                else
                {
                    camera.orthographic = true;
                }
                
                Debug.Log("orthographic set");
            }
            if (camera.orthographic == true && this.orthographicSize != null)
            {
                camera.orthographicSize = (float)this.orthographicSize;
            }
            if (camera.orthographic == false && this.fieldOfView != null)
            {
                camera.fieldOfView = (float)this.fieldOfView;
                Debug.Log("fov set");
            }
        }
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
        this.message = "This is the dafault message";
    }

    public JSONPNGmetadata(int sceneID, string message)
    {
        this.sceneID = sceneID;
        this.message = message;
    }
}
