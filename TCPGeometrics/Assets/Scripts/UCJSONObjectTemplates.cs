using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCJSONObjectTemplates
{
}

#region UNITY COM: JSON object templates

[System.Serializable]
public class TcpConfigParameters
{
    public string host;
    public int[] ports;
}

[System.Serializable]
public class UnityInitResolution
{
    public int width;
    public int height;
}


//JSON representation for Serializing of received parameters
[System.Serializable]
public class JSONCaptureParameters
{
    public string message;
    public int sceneID;

    public JSONCaptureGenericObject[] Objects;

}


// representation of Generic GameObject (Camera, Light, 3Dobject)
// void SetParamters(targetObject, objectID) sets parameters of targetObject
[System.Serializable]
public class JSONCaptureGenericObject
{
    //When performing changes in member region, also SetParamters() must be updated

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



    public void SetParameters(GameObject targetObject, int objectID)
    //Method to set all class paramters of the Gameobject to the targetObject
    {
        Renderer rend = targetObject.GetComponent<Renderer>();
        Light light = targetObject.GetComponent<Light>();
        Camera camera = targetObject.GetComponent<Camera>();
        Vector3 rgb;

        

        //set Kinematics (pos, scale, rot)
        try
        {
            Debug.Log("Set Parameters entered, info: " + this.info);
            targetObject.transform.position = new Vector3(this.position[0], this.position[1], this.position[2]);
            targetObject.transform.eulerAngles = new Vector3(this.rotation[0], this.rotation[1], this.rotation[2]);
            targetObject.transform.localScale = new Vector3(this.scale[0], this.scale[1], this.scale[2]);
        }
        catch (System.NullReferenceException NullException)
        {
            Debug.Log("ERROR : Please check info, pos, rot, scale parameters in json dict for objectID " + objectID.ToString()
                + "\n" + NullException);
        }

        Debug.Log("Position set to " + targetObject.transform.position.ToString() +
            " ; Rotation set to " + targetObject.transform.eulerAngles.ToString() +
            " ; Scale set to " + targetObject.transform.localScale.ToString());


        if (rend != null)
        //set paramters 3DGameObject
        { 
            Debug.Log("Set parameters of 3DGameObject...");
            try
            {
                rgb = new Vector3(this.color[0], this.color[1], this.color[2]);
                rend.material.color = new Color(rgb.x, rgb.y, rgb.z, this.transparency);
                rend.material.SetFloat("_Metallic", (float)this.metallic);
                rend.material.SetFloat("_Glossiness", (float)this.glossiness);
            }
            catch (System.NullReferenceException NullException)
            {
                Debug.Log("ERROR : Please check 3DGameObject parameters in json dict for objectID "+ objectID.ToString()
                    + "\n" + NullException);
            }
            
        }
        else if (light != null)
        //set parameters of light
        {
            Debug.Log("Set parameters of LIGHT...");
            try
            {
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
            catch (System.NullReferenceException NullException)
            {
                Debug.Log("ERROR : Please check light parameters in json dict for objectID " + objectID.ToString()
                    + "\n" + NullException);
            }
            


        }
        else if (camera != null)
        //set camera parameters
        {
            Debug.Log("Set parameters of CAMERA...");
            try
            {
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
            catch (System.NullReferenceException NullException)
            {
                Debug.Log("ERROR : Please check camera parameters in json dict for objectID " + objectID.ToString()
                    + "\n" + NullException);
            }

        }
    }
}


[System.Serializable]
//JSON representation of meta data of response message
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

#endregion