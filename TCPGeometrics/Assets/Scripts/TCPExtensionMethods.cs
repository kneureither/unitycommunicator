using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;

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
public class JSONCaptureObjectGeneric
{
    //Kinematics & Co
    public float[] position;
    public float[] scale;
    public float[] rotation;
    public string info;


    //Light Object
    public float[] colorOfLight;
    public float intensity;
    public bool active;
    public float range;
    public float spotAngle;


    //Camera Object
    public bool orthographic;
    public float orthographicSize;
    public float fieldOfView;


    //3D Game Object
    public float transparency;
    public float[] color;
    public float metallic;
    public float glossiness;



    public void SetParameters(GameObject targetObject)
    {
        Renderer rend = targetObject.GetComponent<Renderer>();
        Light light = targetObject.GetComponent<Light>();
        Camera camera = targetObject.GetComponent<Camera>();
        Vector3 rgb;

        Debug.Log("Set Parameters entered, info: " + this.info);

        //set Kinematics (pos, scale, rot)
        targetObject.transform.position = new Vector3(this.position[0], this.position[1], this.position[2]);
        targetObject.transform.eulerAngles = new Vector3(this.rotation[0], this.rotation[1], this.rotation[2]);
        targetObject.transform.localScale = new Vector3(this.scale[0], this.scale[1], this.scale[2]);

        Debug.Log("Position set to " + targetObject.transform.position.ToString() +
            " ; Rotation set to " + targetObject.transform.eulerAngles.ToString() +
            " ; Scale set to " + targetObject.transform.localScale.ToString());


        if (rend != null)
        //set paramters 3DGameObject
        {
            Debug.Log("3DGameObject");
            rgb = new Vector3(this.color[0], this.color[1], this.color[2]);
            rend.material.color = new Color(rgb.x, rgb.y, rgb.z, this.transparency);
            rend.material.SetFloat("_Metallic", (float)this.metallic);
            rend.material.SetFloat("_Glossiness", (float)this.glossiness);
        }
        else if (light != null)
        //set parameters of light
        {
            Debug.Log("LIGHT");
            //Set color and intenstity of light
            light.color = new Color(this.colorOfLight[0], this.colorOfLight[1], this.colorOfLight[2], 1);
            light.intensity = this.intensity;

            //Set active
            light.enabled = this.active;

            //light parameters
            if ((light.type == LightType.Spot || light.type == LightType.Point))
            {
                light.range = this.range;
            }

            if (light.type == LightType.Spot)
            {
                light.spotAngle = this.spotAngle;
            }
        }
        else if (camera != null)
        //set camera parameters
        {
            Debug.Log("CAMERA");

            camera.orthographic = this.orthographic;

            if (camera.orthographic == true)
            {
                camera.orthographicSize = this.orthographicSize;
            }
            else
            {
                camera.fieldOfView = this.fieldOfView;
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


