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
            SetJSONparameters();
        }
    }



    // Implementation of all parameter changes of objects with index check and call of JSONCaptureGenericObject.SetParameters()
    void SetJSONparameters()
    {
        if(this.objectID == -1)
        {
            Debug.Log("ObjectID not initialized");
        }
        else if (this.objectID < 0 || objectID >= manager.CaptureParameters.Objects.Length)
        {
            Debug.Log("ObjectID out of bounds of CaptureParameters.Objects[]");
        }
        else
        {
            Debug.Log("SetJSONparamters of Object " + this.objectID.ToString());
            //Function call of JSON base class (TCPExtensionMethods)
            manager.CaptureParameters.Objects[objectID].SetParameters(targetObject, this.objectID);
            //array for handling object status (new parameters set? (bool))
            manager.objectParametersSet[objectID] = true;
        }
    }

}