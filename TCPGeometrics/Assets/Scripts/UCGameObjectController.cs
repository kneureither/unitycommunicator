using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UCGameObjectController : MonoBehaviour
{
    #region UNITY COMMUNICATOR: GameObject members
    public int objectID = -1; //Must be assigned in Object Inspector
    public GameObject manageCapture; //must be assigned in Object Inspector (Drag n Drop)
    private UCManageCapture manager;
    #endregion


    void Awake()
    {
        ////PYTHON COM : add this to every GameObject.Awake() in custom unity project
        //get ManageCapture (to use its public variables)
        //ManageCapture must be assigned as public variable in Inspector (Drag n Drop)
        manager = manageCapture.GetComponent<UCManageCapture>();
        ////////
    }

    void Update()
    {
        //PYTHON COM : add this to all GameObject.Update() in custom unity projects
        manager.UnityCommunicator.SetReceivedParamters(this.gameObject, this.objectID);
        ////////
    }
}