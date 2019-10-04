# Unity Communicator
## Developed @ Heidelberg Collaboratory for Image Processing
Author: Konstantin Neureither, September 2019

This project delivers an easy to use Python - Unity interface for scene Rendering. 


## Features of pyton interface

- start tcp connection to unity project
- set resolution of the capturing screen
- send json representations of the scene objects properties such as pos, scale, rotation
- receive the rendered scene

## Features of the Unity Project UCGeometrics

- Serves as an example for implementation into an exisiting Unity project
- Receives json data from python, updated its objects with these given parameters, captures a frame and sends it back.

## How to implement it

Examples for the usage of the UnityCommunicator python class are given in the files `example1.py` and `example2.py`.

To implement the Unity Code, please open the project Folder `UCGeometrics` to see an example. First, you should find a json representations of the scene objects properties (as shown in the folder `ParameterFiles`) this representation must then be implemented into the C# file `UCJSONObjectTemplates.cs`and also the `SetParameters(GameObject targetObject, int objectID)` function must be updated as it updates the parameters of game objects. After that you can import the four given C# scripts and attach them to your gameobject, specify the public variables in the inspector and you are good to go.

Please report bugs or feedback to stud@kneureither.de
I hope you enjoy using it! Cheers!
