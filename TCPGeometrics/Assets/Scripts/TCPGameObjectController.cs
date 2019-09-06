using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TCPGameObjectController : MonoBehaviour
{
    public int objectID = -1;
    public GameObject manageCapture;
    private TCPCaptureChanges manager;
    public GameObject targetObject;

    // Start is called before the first frame update
    void Awake()
    {
        if (this.enabled == true)
        {
            //get ManageCapture (to use its public variables)
            //ManageCapture must be assigned as public variable in Inspector (Drag n Drop)
            manager = manageCapture.GetComponent<TCPCaptureChanges>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetObject = this.gameObject;
        if (manager.captureChangeRequest == true) //future: also self set check
        {
            SetJSONparamters();
        }
    }



    // Implementation of all parameter changes of objects
    void SetJSONparamters()
    {
        switch(objectID)
        {
            case -1:
                Debug.Log("ObjectID not initialized");
                break;
            case 0:
                Debug.Log("SetJSONparamters of Object 0");
                manager.CaptureParameters.Object0.SetParameters(targetObject);
                manager.objectParametersSet[0] = true;
                break;
            case 1:
                Debug.Log("SetJSONparamters of Object 1");
                manager.CaptureParameters.Object1.SetParameters(targetObject);
                manager.objectParametersSet[1] = true;
                break;
            case 2:
                Debug.Log("SetJSONparamters of Object 2");
                manager.CaptureParameters.Object2.SetParameters(targetObject);
                manager.objectParametersSet[2] = true;
                break;
            case 3:
                Debug.Log("SetJSONparamters of Object 3");
                manager.CaptureParameters.Object3.SetParameters(targetObject);
                manager.objectParametersSet[3] = true;
                break;
            case 4:
                Debug.Log("SetJSONparamters of Object 4");
                manager.CaptureParameters.Object4.SetParameters(targetObject);
                manager.objectParametersSet[4] = true;
                break;
            case 5:
                Debug.Log("SetJSONparamters of Object 5");
                manager.CaptureParameters.Object5.SetParameters(targetObject);
                manager.objectParametersSet[5] = true;
                break;
            case 6:
                Debug.Log("SetJSONparamters of Object 6");
                manager.CaptureParameters.Object6.SetParameters(targetObject);
                manager.objectParametersSet[6] = true; 
                break;
            default:
                Debug.Log("unknown Object ID");
                break;
        }
    }

}





//void SetTransform(GameObject targetObject, JSONCaptureObject TrafoParameters)
//{
//    if (TrafoParameters.position != null)
//    {
//        transform.position = new Vector3(TrafoParameters.position[0], TrafoParameters.position[1], TrafoParameters.position[2]);
//    }
//    if (TrafoParameters.rotation != null)
//    {
//        transform.eulerAngles = new Vector3(TrafoParameters.rotation[0], TrafoParameters.rotation[1], TrafoParameters.rotation[2]);
//    }
//    if (TrafoParameters.scale != null)
//    {
//        transform.localScale = new Vector3(TrafoParameters.scale[0], TrafoParameters.scale[1], TrafoParameters.scale[2]);
//    }
//}



//void SetCamParameters(GameObject targetObject, JSONCaptureCamObject CamParameters)
//{
//    Camera camera = targetObject.GetComponent<Camera>();
//}
//void SetLightParameters(GameObject targetObject, JSONCaptureLightObject LightParameters)
//{
//    Light light = targetObject.GetComponent<Light>();
//}


//void SetGeomParamters(GameObject targetObject, JSONCaptureGeomObject GeomParameters)
//{
//    Renderer rend = targetObject.GetComponent<Renderer>();

//    Vector3 rgb;
//    float transparency;

//    if (GeomParameters.color != null)
//    {
//        rgb = new Vector3(GeomParameters.color[0], GeomParameters.color[0], GeomParameters.color[0]);
//    }
//    else
//    {
//        rgb = new Vector3(rend.material.color[0], rend.material.color[1], rend.material.color[2]);
//    }
//    if (GeomParameters.transparency != null)
//    {
//        transparency = (float)GeomParameters.transparency;

//    }
//    else
//    {
//        transparency = rend.material.color[3];
//    }

//    rend.material.color = new Color(rgb.x, rgb.y, rgb.z, transparency);

//    if (GeomParameters.intensity != null)
//    {
//        rend.material.SetFloat("_Intensity", (float)GeomParameters.intensity);
//    }
//    if (GeomParameters.metallic != null)
//    {
//        rend.material.SetFloat("_Metallic", (float)GeomParameters.metallic);
//    }
//    if (GeomParameters.glossiness != null)
//    {
//        rend.material.SetFloat("_Glossiness", (float)GeomParameters.glossiness);
//    }
//}
