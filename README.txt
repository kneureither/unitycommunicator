This project delivers an easy to use Python - Unity interface for scene Rendering. 

# Features of pyton interface

- start tcp connection to unity project
- set resolution of the capturing screen
- send json representations of the scene objects properties such as pos, scale, rotation
- receive the rendered scene

# Features of the Unity Project TCP Geometrics

- Serves as an Example for implementation into an exisiting Unity project
- Receives Json data from python, applies these given parameters to its objects, captures a frame and sends it back

# How to implement

Examples for the usage of the UnityCommunicator python class are given in the files `example1.py` and `example2.py`.

To implement the Unity Code, please open the project Folder `TCPGeometrics` to see an example. First, you should find a json representations of the scene objects properties (as shown in the folder `ParanetersFiles`) this representation must then be implemented into the C# file `UCJSONObjectTemplates.cs`and also the `SetParameters(GameObject targetObject, int objectID)` function must be updated. After that you can import the four given C# scripts and attach them to your gameobject, specify the public variables in the inspector and you are good to go.

Please report bugs or feedback to stud@kneureither.de
I hope you enjoy using it! Cheers!
