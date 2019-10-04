using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UCGameObjectController : MonoBehaviour
{
    #region UNITY COMMUNICATOR: GameObject members
    public int objectID = -1; //Must be assigned in Object Inspector
    public GameObject UCManageCapture; //must be assigned in Object Inspector (Drag n Drop)
    #endregion

    void Update()
    {
        //PYTHON COM : add this to all GameObject.Update() in custom unity projects
        UCManageCapture.GetComponent<UCManageCapture>().UnityCommunicator.SetReceivedParamters(this.gameObject, this.objectID);
        ////////
    }
}