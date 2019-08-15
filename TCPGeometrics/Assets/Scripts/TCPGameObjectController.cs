using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TCPGameObjectController : MonoBehaviour
{
    public int objectID = -1;
    public GameObject manageCapture;
    private TCPCaptureChanges manager;

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
                JSONCaptureCamObject Object0 = manager.CaptureParameters.Object0;
                transform.position = new Vector3(Object0.position[0], Object0.position[1], Object0.position[2]);
                transform.eulerAngles = new Vector3(Object0.rotation[0], Object0.rotation[1], Object0.rotation[2]);
                transform.localScale = new Vector3(Object0.scale[0], Object0.scale[1], Object0.scale[2]);

                //Todo Implement all parameters of JSON

                Debug.Log(Object0.position[0].ToString());

                manager.objectParametersSet[0] = true;
                break;
            case 1:
                Debug.Log("SetJSONparamters of Object 1");
                JSONCaptureLightObject Object1 = manager.CaptureParameters.Object1;
                transform.position = new Vector3(Object1.position[0], Object1.position[1], Object1.position[2]);
                transform.eulerAngles = new Vector3(Object1.rotation[0], Object1.rotation[1], Object1.rotation[2]);
                transform.localScale = new Vector3(Object1.scale[0], Object1.scale[1], Object1.scale[2]);

                //Todo Implement all parameters of JSON

                manager.objectParametersSet[1] = true;
                break;
            case 2:
                Debug.Log("SetJSONparamters of Object 2");
                //Todo Set Parameters
                manager.objectParametersSet[2] = true;
                break;
            case 3:
                Debug.Log("SetJSONparamters of Object 3");
                //Todo Set Parameters
                manager.objectParametersSet[3] = true;
                break;
            case 4:
                Debug.Log("SetJSONparamters of Object 4");
                //Todo Set Parameters
                manager.objectParametersSet[4] = true;
                break;
            case 5:
                Debug.Log("SetJSONparamters of Object 5");
                //Todo Set Parameters
                manager.objectParametersSet[5] = true;
                break;
            case 6:
                Debug.Log("SetJSONparamters of Object 6");
                //Todo Set Parameters
                manager.objectParametersSet[6] = true; 
                break;
            default:
                Debug.Log("unknown Object ID");
                break;
        }
    }
}




