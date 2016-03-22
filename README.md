<a name="HOLTop" ></a> 
# Integrating Unity Games with Windows 10#
 

<a name="Overview"></a> 
## Overview ##
In this module, you will learn about different ways to integrate with the Universal Windows Platform from within a Unity game. 
 
<a name="Objectives" ></a> 
### Objectives ###

In this module, you will learn about these concepts: 

- Understand the different #define pre-processors for writing native code on a Windows game.  
- Integrate with Windows 10 by inlining some code from within your Unity game 
- Integrate with Windows 10 by writing and properly configuring Unity plugins 
 
> **Note:** This module is optimized to show you integration techniques. Most of the Unity code (menus, event handlers) has been coded for you; you will need to inject the native code at right places, but the lab does not explain the 'glue' code we wrote in Unity. 
> 
> You will be able to walk through that on your own (as you have all the source), at your own leisure.
> 
> You can find very detailed step by step and video tutorials for building the game on [Unity's learning site](https://unity3d.com/learn/tutorials/projects/tanks-tutorial).   

<a name="Prerequisites"></a>
### Prerequisites ###
The following software is required to complete this module:

- [Visual Studio Community 2015](https://www.visualstudio.com/products/visual-studio-community-vs) or greater
- [Unity 5.3](http://unity3d.com/get-unity/update) or later
- The 'starter' solution for this lab pre-downloaded. You can get it from [Microsoft-Build-2016/CodeLabs-GameDev-2-UnityWin10](https://github.com/Microsoft-Build-2016/CodeLabs-GameDev-2-UnityWin10.git) github repository.

The following software packages are used within the lab. These can be optional, as you can choose to skip the steps, but are highly recommended for a comprehensive coverage of different approaches to integrate with Windows:  

- Vungle's Unity plugin. For _//build_ event, we have included this in the 'extra files' in the repo.  Post _//build_ event, you should get it from [Vungle's github repo](https://github.com/Vungle/Unity-Plugin)
- [Microsoft Universal Ads SDK](https://visualstudiogallery.msdn.microsoft.com/401703a0-263e-4949-8f0f-738305d6ef4b )
 

<a name="Exercises"></a>
## Exercises ##
This module includes the following exercises:

1. [Exporting Unity Project to Windows Store w/ C# Projects] (#Exercise1)
1. [Using Unity's native integration helper libraries] (#Exercise2)
1. [Adding WinRT inline code to a Unity game] (#Exercise3) 
1. [Configuring a reference to a Unity plugin] (#Exercise4) 
1. [Taking a bridge approach to integration] (#Exercise5) 

Estimated time to complete this module: **60 minutes**
 
<a name="Exercise1"></a> 
### Exercise 1: Exporting Unity Project to Windows Store w/ C# Projects ###

1. Open the **Ex1/Begin** Tanks project in Unity.

	> **Note:** If using default setup at the event, the folder is at 
**C:\labs\CodeLabs-GameDev-2-UnityWin10\Source\Ex1\Begin**.


1. From Unity, let's prepare our project so we can build a player targeting UWP. You can do this via **File-> Build Settings** menu.  Don't click **Build** yet, we need to configure a few options.

1. In the **Build Settings** dialog box, select **Universal 10** as target SDK.

1. Select **XAML** as the **UWP Build Type**. XAML is default for most scenarios as it allows you to bring XAML UI into your game. In this case, we need XAML UI for showing ads in a later task.   

1. For this module, make sure you select the option to Export **Unity C# projects**; we will explain what this does a few steps down when we walk through the output.

	![Build Action](./Images/BuildAction.png "Build Action")

	_Build Action_

1. Before we click **Build** we should also set our **Player Settings**. Click the **Player Settings** button.

1. In player settings you can configure the name of your app, the default splash screen and icons, orientation, rendering options, sensors, and many other options. Explore all of these and feel free to change these.

	![Player Settings](./Images/PlayerSettings.png "Player Settings")

	_Player Settings_

1. The setting that we are after in our case is under **capabilities** in the **Publishing Settings** section. We need to add the **InternetClient** capability to our project so that later when we show ads these can be downloaded from the internet.

	![Player Settings Capabilities](./Images/PlayerSettingsCapabilities.png "Player Settings Capabilities")

	_Player Settings Capabilities_

1. Now, we can click **Build**. You will be prompted to select a folder you are building to. Give it any project name. If you want to mimic our solution (not required) you can call it _Win10Solution_.

1. The Unity build process will take a few minutes, once it is done, Unity will launch a **Windows Explorer** window to your build folder. Locate the **Tanks.sln** in that folder and open it in Visual Studio.

1. Let's now explore what Unity created.
    
	![Visual Studio Solution Explorer](./Images/VisualStudioSolutionExplorer.png "Visual Studio Solution Explorer")

	_Visual Studio Solution Explorer_

	- **Tanks (Universal Windows)** is our game; the one we will submit to the store.
 
	- **Tanks (Universal Windows)** reference **Assembly-CSharp.dll**, which is the game generated by Unity. This assembly has all our MonoBehaviours and logic.

	- The data folder in Tanks project is where Unity put the projects, assets, levels, etc. for the game.

	- **Package.appxmanifest** is the manifest (configuration file) for our project. In this file, you will find what we configured under **Player Settings** and **Build Settings** in Unity.

	- The **Assembly-Csharp** and **Assemby-CSharp** first pass projects is normally what Unity builds in the **Editor**. In this case, Unity generated them because we chose the option to create **Unity C# projects**. These will be handy for our lab as they allow us to rebuild the project (and game) from within Visual Studio without having to rebuild from Unity. Of course that works if all we modify is code (that goes into **Assembly-CSharp**); if we modify scenes, then we must rebuild from Unity. If you rebuild from Unity and output to same build folder, Unity will not override the code and settings for your games project (Tanks.csproj). Unity preserves these so that any changes you make to your solution are preserved. 

<a name="Exercise2"></a> 
### Exercise 2: Using Unity's native integration helper libraries ###

For native integration, Unity includes a few wrappers for Windows Store features like tiles, toast notifications, and launchers.  These features are in the **UnityEngine.WSA** namespace.

Let's use the Launcher APIs, to launch Help for our game, and to add a "rate us" feature to our game. 

1. In **GameManager.cs**, there is already an Input handler for F1 key in the Update loop, so we can add the code to launch our help screen in the browser.

	````C# 
	void Update ()
	{ 
		if (Input.GetKeyUp(KeyCode.F1))
		{
			UnityEngine.WSA.Launcher.LaunchUri("ms-windows-store:REVIEW?PFN=Microsoft.Channel9_8wekyb3d8bbwe", 
				false); 
		} 
		...
	} 
	````

1. This same technique can be used to implement the 'Rate us' functionality. There is already a "Rate us" button in the game. For demo purposes, it is coded to come up every 4th time you finish a round. So all we have to do is add code the **OnRateClicked** in the **SocialDialogManager** Behaviour: 

	````C# 
	public void OnRateClicked ()
	{
		UnityEngine.WSA.Launcher.LaunchUri("ms-windows-store:REVIEW?PFN=Microsoft.Channel9_8wekyb3d8bbwe", 
								false); 
		DismissDialog(); 
	}
	````

1. For our final example of this technique, and to illustrate a little more immersive integration, let's add live tiles to our game.  At the end of each round, we can add a teaser message so users can come back if they quit game in middle of a round. Our game already has a **SetLiveTile** method that gets called at end of each round.  This method will update the text on our main live tile, and set an image (to make pop with more interactivity).   

	````C# 
	void SetLiveTile ( string textmessage )
	{
		 UnityEngine.WSA.Tile.main.Update( "ms-appx:///Data/StreamingAssets/TanksIcon_150x150.png" ,
				"ms-appx:///Data/StreamingAssets/TanksIcon_310x150.png", string.Empty, textmessage);
	}
	````

#### Show Me!####

We have added three features to our game. Let's now go see them in action! 

1. Viewing the Help file is easy. Any time during the game, press **F1**. 
1. To see the **social dialog** that prompt the user to rate the game, just play a few rounds. At the end of a round you will be prompted with a dialog to rate the app. 
1. The live tile feature also happens at end of a round; there is no UI to ask user if they want to update tile, etc. so play a round and if you want to see the code, set a break point in it. 
To see the live tile, you do need to have it pinned to your start menu. You can do it before, or after you have ran the game.  


#### Discussion around Unity WSA APIs ####

These APIs are convenient and easy to use. You did not notice this yet (we will cover it next), but there are threading requirements the API is abstracting and handling for you. 

Unfortunately, the APIs are also limited. They do not handle all native integration scenarios. No worries though, we can just inline code on Unity projects to access other the APIs.    


<a name="Exercise3"></a> 
### Exercise 3: Adding WinRT inline code to a Unity game ###

When Unity games target Windows Store (with a .NET backend, not IL2CPP), they are compiled using the .NET compiler and therefore can access WinRT APIs, since as part of the build process, Unity links against WinRT libraries.  

To inline WinRT code into your Unity project all you need to do is protect yourself so that Unity does not try to compile that when targeting other platforms; for this, we will use the **NETFX_CORE** pre-processor. 

Unity's documentation has more guidance on [pre-processors defines for platform specific compilation](http://docs.unity3d.com/Manual/PlatformDependentCompilation.html).  For this exercises we will use these directives:

- **NETFX_CORE** to filter on WinRT code.
- **WINDOWS_UWP** to ensure code is used only on Windows 10, when APIs are 10 specific. 
- **UNITY_EDITOR** to filter out code that only runs in the editor.

To exercise inlining code, we want to add support to enter and exit full screen mode when the user presses **F11** in our game. The game is already listening for keyboard input in the update loop, so we can add the code to enter/exit full screen there.

````C# 	
else if (Input.GetKeyUp (KeyCode.F11))
{
#if NETFX_CORE && WINDOWS_UWP
	 //Dispatch from App to UI Thread 
	 UnityEngine.WSA.Application.InvokeOnUIThread( ()=>
	 {
		  var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
		  if (appView.IsFullScreen)
				appView.ExitFullScreenMode(); 
		  else
				appView.TryEnterFullScreenMode(); 
	 } , false); 
#endif
}
````

Here are the relevant details to notice from our snippet:

- It is inside a `#if NETFX_CORE && WINDOWS_UWP` so other platforms don't call into it and Unity does not try to compile when targeting other platforms. 
- It is inside a delegate invoked by **UnityEngine.WSA.Application.InvokeOnUIThread**. Two of the most useful functions in the **UnityEngine.WSA** namespaces are: 
	- **Application.InvokeOnUIThread** used to dispatch a call from Unity's app thread to Windows' UI thread
	- **Application.InvokeOnAppThread** used to dispatch calls from any thread to Unity's app thread. 

In our case, **ApplicationView.TryEnterFullScreenMode** requires that it be called from the UI thread, that is why we dispatched the call, but not all WinRT calls need to be dispatched.    
   

#### Show Me!####

To see full screen running, just run the game and press **F11**. It should work. 
If you are curious, comment out the dispatcher call and call TryEnterFullScreenMode without dispatching and see what happens (ahem, crash).  

#### Discussion around inlining code ####

You have now seen how easy it is to inline WinRT code. Just understand and pay attention to your threading models, and you will be fine; it works great for WinRT and .NET APIs. However, what about calling custom libraries (such as an analytics library, or an ads SDK)? Of course, Unity allows that, via plugins. Let's cover that next.

<a name="Exercise4"></a> 
### Exercise 4: Configuring a reference to a Unity plugin ###

This section discusses referencing a managed Unity plugin; it focuses on the Windows' specific options to configure it and reference it. For practical purposes here, let's summarize a plugin as a .NET assembly or Winrt component (winmd) that we need to reference from within our Unity code.

If you want to dive deeper into plugins within Unity, please see this [reference](http://docs.unity3d.com/Manual/Plugins.html) in Unity's documentation. 

 
1. Go back into **Unity Editor**.

1. Add a reference to Vungle's Unity Package, which includes a plugin. To do this, select **Assets -> Import New Package -> Custom Package**.
	
	![Import Custom Package in Unity Editor ](./Images/UnityEditorImportCustomPackage.png "Import Custom Package in Unity Editor")

	_Import Custom Package in Unity Editor_

1. Navigate to the folder where your **VungleSDK.UnityPackage** is located. If you are using the one pulled with this repo, it is in the **Source/ExtraFiles/Vungle** folder.
 
1. Click **Open** to add it to our Unity project.

1. After selecting the package, you will see the **Unity Import Dialog**.  In Vungle's case, they ship one plugin for all platforms (iOS, Android, Windows). This is fine for us. Click **Import** to import all the files. Let's now review the important ones (for Windows Store and Windows Phone) configurations. 

	 - Notice the files are under a **Plugins/Metro** folder structure. Unity follows a specific folder structure and naming convention for different platforms. Starting with Unity 5, the folder hierarchy is not required, as Unity allows you to configure it manually via the new Plugin inspector, but for ease of use, it is still handy to follow. You can find out more about these folder names in Unity's [documentation](http://docs.unity3d.com/Manual/PluginInspector.html).
      
	 - There is a **UWP** folder. This is used for plugins for Universal (Windows 10) Projects. 

	 - There is a **WindowsPhone81** folder, since the plugin supports phone 8.1. We are not going to need that today, so <u>remove that Windows Phone 81 folder and its contents</u>.


	> **Note:** If you want more details around these imported settings, Unity's documentation has a [good overview of the import settings](http://docs.unity3d.com/Manual/windowsstore-plugins.html). 

1. Now that we have our references configured within Unity, we can call the code to show the ads. To save you a little typing time (and since the logic is simple and not critical to Windows integration, the code is written, but excluded via a `#if USE_VUNGLE_ADS` pre-processors. Go to the top of the **GameManager.cs** file, and uncomment out the `#define USE_VUNGLE_ADS` line.

1. Let's now review how Vungle is getting called. In the **Start** function,  we initialize Vungle and subscribe to adCompleted event:

	````C#
	Vungle.init("com.prime31.Vungle", "vungleTest", "vungleTest");
	Vungle.onAdFinishedEvent += OnAdFinished;  
	````
1. Then, within **OnAdFinished**, we call **OnAdCompleted()**.

1. Within **OnAdCompleted** we will reset the volume (since game background music gets muted prior to playing the ads). This **OnAdCompleted** event is worth looking at because it demonstrates using **UnityEngine.WSA.Application.InvokeOnAppThread()** to unmute the background music.

	````C#
	void OnAdCompleted()
	{
		 m_isDisplayingAd = false;
		 if (!UnityEngine.WSA.Application.RunningOnAppThread())
		 {
			  UnityEngine.WSA.Application.InvokeOnAppThread(() =>
			  {
					ToggleMute(false);
			  }, false);
		 }
		 else
			  ToggleMute(false);
	}
	````

1. In GameManager's **RoundEnding** function we call the **ShowAd** method, which in Vungle's case first mutes the background music, then calls **Vungle.showAd** and sets the flag so the game loop knows to wait for ad to complete (**m_isDisplayingAd**). 

With that, we are now ready to test ads in our game. 

#### Show Me!####

To see Vungle ads, just run the game and win two rounds. After the second round, the ad will show up.  

#### Discussion around plugin configuration ####

You have now seen how easy it is to integrate a managed Unity plugin or a WinRT component into your Unity project. Plugins are a critical part of code/logic reuse and a great integration and extensibility point for games, as most games leverage shared analytics, ads, social networks, etc. All of these are powered by reusable plugins you will consume from Unity.   

<a name="Exercise5"></a> 
### Exercise 5: Taking a bridge approach to integration ####

This exercise is not yet written. 


<a name="Summary" />
## Summary ##

By completing this module, you should have:

- Understood the different #define pre-processors for writing native code on a Windows game.  
- Integrated with Windows 10 by inlining some code from within your Unity game 
- Integrated with Windows 10 by writing and properly configuring Unity plugins 
