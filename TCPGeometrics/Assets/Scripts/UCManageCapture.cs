using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;


public class UCManageCapture : MonoBehaviour
{
    #region UNITY COMMUNICATOR: ManageCapture members
    public static int numberOfObjects = 7; //This needs to be set for custom project.
    [HideInInspector] public UnityCommunicatorClient UnityCommunicator = new UnityCommunicatorClient(numberOfObjects);
    #endregion

    private void Awake()
    {
        //UNITY COM : add this to ManageCapture.Awake() or Start() in custom unity project
        this.UnityCommunicator.InitTCPConnection();
      
        //////////
    }
    private void Start()
    {
        StartCoroutine(WaitSeconds(3));
    }


    private void Update()
    {
        //UNITY COM : add this to ManageCapture.Update() in custom unity project
        this.UnityCommunicator.ReceiveParameters();
        ////////
    }

    #region UNITY COMMUNICATOR : Capture Scene in Late update
    void LateUpdate()
    {
        //UNITY COM : add this to ManageCapture.LateUpdate() in custom unity project
        if (this.UnityCommunicator.ReadyToCapture())
        {
            StartCoroutine(CapturePNGSendAsBytes());
            Debug.Log("Couroutine Started");
        }
        ///////

    }

    //UNITY COM : add this to ManageCapture Class in custom unity project
    public IEnumerator CapturePNGSendAsBytes()
    {
        Debug.Log("entered Coroutine");
        UnityCommunicator.captureChangeRequest = false;
        //Wait until frame is rendered completely
        yield return new WaitForEndOfFrame();

        //Perform screen capture
        var sceneTexture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytesPNG = sceneTexture.EncodeToPNG();
        Destroy(sceneTexture);

        //Send PNG back to server
        UnityCommunicator.SendPNGAsBytes(bytesPNG);
    }

    public IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    ////////
    #endregion
}
