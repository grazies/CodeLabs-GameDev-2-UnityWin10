To build the Win10 solution, 

1) Open the project in Unity first 
[Open in Unity targeting the folder above this Win10Solution, namely the End folder] 


2) Buld to target UWP from within Unity, give it Win10Solution as the output folder. 
[Ensure you click the "C# projects" option when building from Unity] 
[This step will generate the UnityPlayer and other required files] 
[You are targeting Win10Solution as Unity does not override existing C# project, App.xaml.cs, etc.] 

3) After building in Unity, open in Visual Studio and [Rebuild all] 
[Now you should be able to run] 
