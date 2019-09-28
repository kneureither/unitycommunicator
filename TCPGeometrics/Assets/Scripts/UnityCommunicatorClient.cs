using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class UnityCommunicatorClient
{
    #region members
    [HideInInspector] public bool[] objectParametersSet; //tells if every object is set to new json parameters
    [HideInInspector] public bool captureChangeRequest; //tells if there are new paramteres to render
    [HideInInspector] public bool sceneShotProcessed; //tells if next parameters can be received
    [HideInInspector] public JSONCaptureParameters CaptureParameters; //stores all received parameters for scene (accessed by TCPGameObjectController)

    private int numberOfObjects;
    private string jsontcpconfig;
    private string jsonparameters;
    private bool readyToCapture;
    private bool endSession;
    private bool skipFrame;
    private TcpClient client;
    private NetworkStream stream;
    private TcpConfigParameters Tcpconfig;
    #endregion


    public UnityCommunicatorClient(int numberOfObjects)
    //Class Constructor
    {
        //Set global values
        this.numberOfObjects = numberOfObjects;
        readyToCapture = false;
        endSession = false;
        captureChangeRequest = false;
        sceneShotProcessed = true;
        objectParametersSet = new bool[numberOfObjects];
    }

    public void InitTCPConnection()
    //connects to python server
    {
        //Get Paramters from python common json TCP config File
        string path = Application.streamingAssetsPath + "/tcpconfig.json";
        jsontcpconfig = File.ReadAllText(path);
        Tcpconfig = JsonUtility.FromJson<TcpConfigParameters>(jsontcpconfig);

        //Start TCP Connection
        client = new TcpClient(Tcpconfig.host, Tcpconfig.ports[1]);
        Debug.Log("Connecting...");
        Debug.Log("Port: " + Tcpconfig.ports[1].ToString());

        stream = client.GetStream();
        Debug.Log("Connection established!");

        //Reveive and set resolution
        string serverMessage = this.ListenFromServer();
        string jsonunityresolution = serverMessage.Substring(0, serverMessage.IndexOf("eod.", StringComparison.Ordinal - 1));
        UnityInitResolution unityresolution = JsonUtility.FromJson<UnityInitResolution>(jsonunityresolution);

        Screen.SetResolution(unityresolution.width, unityresolution.height, false);
        Debug.Log("Resolution set to " + unityresolution.width.ToString() + " x " + unityresolution.height.ToString());

        //Sent confirmation to python
        this.RespondStringToServer("Resolution set to " + unityresolution.width.ToString() + " x " + unityresolution.height.ToString());

    }


    public void ReceiveParameters()
    //
    {
        string serverMessage;

        //Check if last paramter set has been captured and sent back to server
        if (this.sceneShotProcessed)
        {
            //receive new set of parameters
            serverMessage = this.ListenFromServer();

            //Handle quit request from python
            if (serverMessage == "END.eod.")
            {
                endSession = true;
                Debug.Log("endSession set to true");
            }
            else
            {
                jsonparameters = serverMessage.Substring(0, serverMessage.IndexOf("eod.", StringComparison.Ordinal - 1));
                captureChangeRequest = true;
                sceneShotProcessed = false;

                //Set every object's Render Status to false
                for (int i = 0; i < numberOfObjects; i++)
                {
                    objectParametersSet[i] = false;
                }
                //parse received json string to C# class JSONCaptureParameters
                CaptureParameters = JsonUtility.FromJson<JSONCaptureParameters>(jsonparameters);
                Debug.Log("STATUS : received parameters of Scene ID : " + CaptureParameters.sceneID + ", JSON Message : " + CaptureParameters.message);
            }

            if (this.endSession == true || Input.GetKey("escape"))
            {
                Debug.Log("Quit Applicaton...");
                Application.Quit();
            }

            if (this.captureChangeRequest == true)
            {
                //new set of paramters received, ready for applying and rendering
                Debug.Log("CaptureChange is True");
            }
        }
    }

    public bool ReadyToCapture()
    //Is used to check if Capture Coroutine can be called.
    //Returns true, when every GameObject was set to the received parameters, else false
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
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetReceivedParamters(GameObject targetObject, int objectID)
    //checks if unity received new paramters and sets the properties of targetObject
    {
        if (this.captureChangeRequest == true)
        {
            if (objectID == -1)
            {
                Debug.Log("ObjectID not initialized");
            }
            else if (objectID < 0 || objectID >= this.CaptureParameters.Objects.Length)
            {
                Debug.Log("ObjectID out of bounds of CaptureParameters.Objects[]");
            }
            else
            {
                Debug.Log("SetJSONparamters of Object " + objectID.ToString());
                //Function call of JSON base class (TCPExtensionMethods)
                this.CaptureParameters.Objects[objectID].SetParameters(targetObject, objectID);
                //array for handling object status (new parameters set? (bool))
                this.objectParametersSet[objectID] = true;
            }
        }
    }

    public void SendPNGAsBytes(byte[] bytesPNG)
    //sends screen capture and meta data json file back to python server
    //Metadata can be modified in "UnityComJSONObjectTemplates"
    {
        Debug.Log("PNG bytes count: " + bytesPNG.Length.ToString());

        //initialize and create metadata for image
        JSONPNGmetadata jsonPNGmeta = new JSONPNGmetadata(CaptureParameters.sceneID);
        string stringPNGmeta = JsonUtility.ToJson(jsonPNGmeta);

        //send img and metadata to pythn server
        this.RespondBytesToServer(bytesPNG, stringPNGmeta);
        Debug.Log("Scene ID " + CaptureParameters.sceneID + " successfully sent back to Server");

        //Ready to receive new set of paramters
        sceneShotProcessed = true;
    }

    private string ListenFromServer()
    {
        string serverMessage = "";

        //Read data from server
        Byte[] bytes = new Byte[1024];
        int bytesRead;
        while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            var incommingData = new byte[bytesRead];
            Array.Copy(bytes, 0, incommingData, 0, bytesRead);
            string serverMessageBuffer = Encoding.ASCII.GetString(incommingData);
            serverMessage += serverMessageBuffer;

            //checks if "end." tag is received. This indicates, that message was received completely
            //If true, this will break the receiving while-loop
            if (serverMessage.Substring(serverMessage.Length - 4) == "eod.")
            {
                Debug.Log("server message received");
                break;
            }
        }
        return serverMessage;
    }

    private void RespondStringToServer(string message)
    {
        byte[] clientMessageEndTag = { 255, 0, 250, 251, 252, 253, 254, 255 };

        try
        {
            // Get a stream object for writing.
            if (stream.CanWrite)
            {
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                int messageLength = messageBytes.Length;

                // Write message String as byte array to stream
                stream.Write(messageBytes, 0, messageLength);
                stream.Write(clientMessageEndTag, 0, clientMessageEndTag.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
            else
            {
                Debug.Log("ERROR: Stream not writable");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void RespondBytesToServer(byte[] bytesPNG, string metaPNG)
    {
        byte[] clientMessageEndTag = { 255, 0, 250, 251, 252, 253, 254, 255 };
        try
        {
            // Get a stream object for writing.
            if (stream.CanWrite)
            {
                byte[] metaBytes = Encoding.ASCII.GetBytes(metaPNG);
                Debug.Log("meta data bytes count: " + metaBytes.Length.ToString());
                int metaLength = metaBytes.Length;

                // Write PNG byte array to socketConnection stream.
                stream.Write(bytesPNG, 0, bytesPNG.Length);
                // Write Metadata JSON as byte array to stream
                stream.Write(metaBytes, 0, metaLength);
                // write Length of MetaData to stream
                stream.Write(BitConverter.GetBytes(metaLength), 0, BitConverter.GetBytes(metaLength).Length);
                // Write End Tag to stream
                stream.Write(clientMessageEndTag, 0, clientMessageEndTag.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
            else
            {
                Debug.Log("ERROR: Stream not writable");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

}
