using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;


public class TCPCaptureChanges : MonoBehaviour
{

    #region members
    //Todo clean up variables
    public int numberOfObjects;
    [HideInInspector] public bool[] objectParametersSet; //tells, wheter all objects are set
    [HideInInspector] public bool captureChangeRequest; //tells, wheter there are new paramteres to render
    [HideInInspector] public bool sceneShotProcessed; //tells, whether next parameters can be received

    private string jsontcpconfig;
    private string jsonparameters;
    private bool readyToCapture;
    private bool endSession;
    private int count_update; //temp, delete after debug
    private int count_captsend;
    TcpClient client;
    NetworkStream stream;
    JSONSceneShot JsonScene;

    [HideInInspector] public JSONCaptureParameters CaptureParameters;
    TcpConfigParameters Tcpconfig;
    //public JSONCaptureParameters CaptureParameters;
    #endregion


    private void Awake()
    {
        objectParametersSet = new bool[numberOfObjects];
    }

    // Called before first frame
    void Start()
    {
        //establish TCP Socket Connection
        InitTCPsocket();

        readyToCapture = false;
        count_update = 0;
        count_captsend = 0;
        endSession = false;
        captureChangeRequest = false;
        sceneShotProcessed = true;
    }



    private void Update()
    {
        Debug.Log("This is update, count: " + count_update.ToString());

        if (sceneShotProcessed)
        {
            ListenFromServer();

            if (endSession == true || Input.GetKey("escape"))
            {
                Debug.Log("Quit Applicaton...");
                Application.Quit();
            }

            if (captureChangeRequest == true)
            {
                Debug.Log("CaptureChange is True");
            }
        }
        count_update++;
    }

    // LateUpdate is called after all Updates are executed
    void LateUpdate()
    {
        readyToCapture = true;
        foreach (bool objectStatus in objectParametersSet)
        {
            if (objectStatus == false)
            {
                readyToCapture = false;
            }
        }

        if (captureChangeRequest && readyToCapture)
        {
            StartCoroutine(CapturePNGSendAsBytes());
            Debug.Log("Couroutine Started");
        }
    }

    void InitTCPsocket()
    {
        //Get Paramters from python common json TCP config File
        string path = Application.streamingAssetsPath + "/tcpconfig.json";
        jsontcpconfig = File.ReadAllText(path);
        Tcpconfig = JsonUtility.FromJson<TcpConfigParameters>(jsontcpconfig);

        //Start TCP Connection
        client = new TcpClient(Tcpconfig.host, Tcpconfig.ports[1]);
        //client = new TcpClient(Tcpconfig.host, 49995);
        Debug.Log("Connecting...");
        Debug.Log("Port: " + Tcpconfig.ports[1].ToString());

        stream = client.GetStream();
        Debug.Log("Connection established!");
    }


    void ListenFromServer()
    {
        //Read data from server
        while (true)
        {
            Byte[] bytes = new Byte[1024];
            int bytesRead;
            while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incommingData = new byte[bytesRead];
                Array.Copy(bytes, 0, incommingData, 0, bytesRead);
                string serverMessage = Encoding.ASCII.GetString(incommingData);
                Debug.Log("server message received");



                if (serverMessage.Substring(serverMessage.Length - 4) == "eod.")
                {
                    if (serverMessage == "END.eod.")
                    {
                        Debug.Log("End Session set");
                        endSession = true;
                    }
                    else
                    {
                        jsonparameters = serverMessage.Substring(0, serverMessage.IndexOf("eod.", StringComparison.Ordinal - 1));
                        captureChangeRequest = true;
                        sceneShotProcessed = false;
                        //Set all object's Render Status to false
                        for (int i = 0; i < numberOfObjects; i++)
                        {
                            objectParametersSet[i] = false;
                        }
                        CaptureParameters = JsonUtility.FromJson<JSONCaptureParameters>(jsonparameters);
                        Debug.Log(CaptureParameters.message);
                    }
                    break;
                }
            }
            Debug.Log("ListenFromServer(): break");
            break;
        }
    }


    void RespondBytesToServer(byte[] bytesPNG, string metaPNG)
    {
        byte[] clientMessageEndTag = { 255, 255, 255, 255, 255, 255, 255, 255 };
        try
        {
            // Get a stream object for writing.
            if (stream.CanWrite)
            {
                byte[] metaBytes = Encoding.ASCII.GetBytes(metaPNG);
                int metaLength = metaBytes.Length;

                // Write byte array to socketConnection stream.
                stream.Write(bytesPNG, 0, bytesPNG.Length);
                // Write Metadata JSON to stream
                stream.Write(metaBytes, 0, metaLength);
                // write Length of MetaData to stream
                stream.Write(BitConverter.GetBytes(metaLength), 0, BitConverter.GetBytes(metaLength).Length);
                // Write End Tag to stream
                stream.Write(clientMessageEndTag, 0, clientMessageEndTag.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }


    private IEnumerator CapturePNGSendAsBytes()
    {
        Debug.Log("entered Coroutine");
        captureChangeRequest = false;
        yield return new WaitForEndOfFrame();

        var sceneTexture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytesPNG = sceneTexture.EncodeToPNG();
        //File.WriteAllBytes(Application.streamingAssetsPath + "/Encoded/SavedScreen" + count_enum.ToString() + ".png", bytesPNG); //Debugging to be deleted
        //count_enum++;

        Destroy(sceneTexture);

        Debug.Log("PNG bytes: " + bytesPNG.Length.ToString());

        //initialize metadata for image
        JSONPNGmetadata jsonPNGmeta = new JSONPNGmetadata(CaptureParameters.sceneID);
        string stringPNGmeta = JsonUtility.ToJson(jsonPNGmeta);

        RespondBytesToServer(bytesPNG, stringPNGmeta);
        Debug.Log("Scene ID " + CaptureParameters.sceneID + " successfully sent back to Server");

        sceneShotProcessed = true;
    }

}