# Sensors

> A Unity3D package that implements RGBD and bodytracking devices to facilitate development of AR applications.

## Overview
[//]: # (https://github.com/IdentityServer/IdentityServer3#Overview)
Sensors is an interface to help developers deal with the continually growing number of RGBD devices. It processes several of the most common SDKs and gives the user a single format to work with, removing the need to learn and implement all the SDKs. Sensors was developed by [VoxarLabs](https://voxarlabs.cin.ufpe.br/). VoxarLabs is a research lab that specializes in AR and VR technologies located in Recife, Brazil.
### Version
Sensors is currently at version 0.5.

## Features
[//]: # (https://github.com/mRemoteNG/mRemoteNG#Features)
Sensors gives an easy way to access the main components of RGBD devices: RGB frames, depth frames, and body joints from tracking algorithms. It currently supports the following devices:

- Orbbec Astra
- Orbbec Astra Pro
- Microsoft Kinect 2

## Getting Started
[//]: # (https://github.com/ExtendRealityLtd/VRTK#Getting-Started)
Clone the repo or download the unitypackage file.
### Prerequisites
[//]: # (https://github.com/JasonGT/NorthwindTraders#Prerequisites)
- Unity version 2018.4 or superior
- A compatible RGBD device
- The driver for the device 

kinect v2 sdk can be found [>>>HERE<<<](https://www.microsoft.com/en-us/download/details.aspx?id=44561)
astra orbbec drivers can be found [>>>HERE<<<](https://orbbec3d.com/develop/)

![orbbec download button](orbbecDownloadButton.PNG)

### Setting up the project
[//]: # (https://github.com/ExtendRealityLtd/
VRTK#Setting-up-the-project)
Sensors is distributed with the UnityPackage file, making it simple to install:
1. Create a new Unity project or open the one you wish to import the package to (tested with the 3D template).
2. In the Assets dropdown, select "Import Package -> Custom Package..."
3. In the file selector, select the Sensor's Unity ("SensorsSDK.unitypackage") package file that you downloaded.
4. Click on the "All" button in the bottom left corner and then click "Import".
5. (OPTIONAL) Due to driver installation, you may need to reboot you computer and/or unplug and plug the device after the intall.

NOTE: if you are using any 2019 version of Unity3D you will receive an alert of obsolete api's. Just click continue and it will work normaly.

These steps will import to your project all the files you need to use the package, including the samples!

### Running the example scene
[//]: # (https://github.com/ExtendRealityLtd/VRTK#Running-the-example-scene)
To test if all went well, go to the Unity project Scenes -> "Sensor's Samples" and open the BasicSample scene. Connect the compatible device and hit play. If everything works, you should see your body joints being tracked along with 2 windows: one for RGB image, another for Depth image.

## Applications
[//]: # (https://github.com/dotnet/reactive#Applications)
The package contains 2 samples in the scenes BasicSample and OverlaySample

### BasicSample Scene
This scene contains the basics of Sensors structure. 4 elements compose the sample, o let's go down one by one and understand what which one does:

1. Sensor: this prefab contains the script "SensorController":
	- SensorController: Script that manages the abstraction of the devices. It generates RGBD and body tracking data through an ISource (the implementation of a device). It has 3 public variables that must be set: RgbReceiver, DepthReceiver, and BodyReceiver. They must be assigned to gameobjects that have a mono behaviour that implements the interface IReceiver<T>. This is the way data will flow from the Sensor to your application. Each IReceiver has a specific type to obey: Rgb demands IImageReceiver<Texture2D>, Depth demands IImageReceiver<Texture2D>, and Body demands IReceiver<Voxar.BodyJoints[]>
2. Body Viewer: this is the object that has a script "BodyRenderer" that receives the data from the tracked bodies for each frame captured by the device. It uses a prefab to display the joint's positions and orientation. It implements IReceiver<Voxar.BodyJoints[]> to receive the data from the device. This gameobject has a child called "Body" that will receive all joints as children.
3. RGB Viewer: this is the first window of the sample. It receives and displays the RGB frames as a texture in a white plane. It implements IImageReceiver<Texture2D>.
4. Depth Viewer: this is the second window of the sample. It receives and displays the Depth frames as a texture in a white plane. It implements IImageReceiver<Texture2D>


### BodyPrefab Scene

This scene uses the SensorsBody to help fast prototyping bodytracking features. The scene has the SensorsBody prefab which is composed of the Sensor prefab and gameobjects to symbolize the joints. In the prefab object, there is a script called "BodySensor" which does all the bodytracking processing for you and sets all the prefab's joints positions so you can get them without a single line of code. In the Unity Inspector you can also configure which joints you want the prefab to show: the tracked ones, the inferred ones or the untracked ones.

## Usage
[//]: # (https://github.com/shimat/opencvsharp#Usage)
Now that you understand the basics of Sensors and have seen it work, it's time to begin developing your own applications!

For a startup project let's make a scene that tracks and displays the position of someone's HEAD. These are the steps to acheive that: 

- Open the SampleScene and place the "Sensor" prefab (included in the unitypackage) in the scene (the prefab should be in the projects "prefabs" folder).

- Create a sphere gameobject in the Hierarchy, create a MonoBehavior Script, call it "HeadFollower", and attach it to the sphere. Now you will have to make your new script adhere to the Voxar.IReceiver<Voxar.BodyJoints[]> interface. The class declaration should look like this:

```c#
public class HeadFollower : MonoBehaviour, Voxar.IReceiver<Voxar.BodyJoints[]>
```

- The compiler should warn you that you must implement the interface method "ReceiveData<Voxar.BodyJoints[]>(Voxar.BodyJoints[] data)". No worries, for now just past this code inside your script:

```c#
//this is the method from the IReceiver<Voxar.Body[]> that you have to implement
//it will make the sphere's follow the head's position
	public void ReceiveData(Voxar.BodyJoints[] data)
    {
    	//by default Sensors only tracks 1 body at a time, but you can change it in the "SensorController" script

    	//if you only wish to track 1 body you can remove this foreach and replace it with:

    	//var body = data[0];
        Debug.Log("Receiving Data!");
        foreach (var body in data)
        {
        	//skips the bodies that are not being tracked
            if (body.status == Status.NotTracking)
            {
                continue;
            }
            Debug.Log("Processing a body!");

            //checks if the current tracked body's head is also being tracked
            if (body.joints.ContainsKey(Voxar.JointType.Head))
            {
                Debug.Log("Updating head position!");
                var head = body.joints[Voxar.JointType.Head];
                this.transform.position = head.worldPosition / 1000.0f;		

                // the world position is divided by 1000 because it is given in milimeters.
            }
        }
    }
```

- Go to the Unity inspector, select the "Sensor" prefab, change the size of "BodyReceivers" list to 1, then drag and drop the sphere to the "BodyReceivers" list.

- In the "Sensor" prefab select the tracking SDK that corresponds to your device (Kinect for Microsoft Kinect v2, Astra for Orbbec Astra or Astra Pro).

- Now just hit play and you should see your sphere following your head!

For more advanced applications you will have to consider which data it will use: Rgb image, depth image, or body tracking and implement the IReceiver<T> for the corresponding ones. Each one has a different IReceiver<T> type that must be respected:
	- RGB needs IImageReceiver<Texture2D>
	- Depth needs IImageReceiver<Texture2D>
	- Bodytracking needs IReceiver<Voxar.BodyJoints[]>
For the RGB and depth, you have to set the resolution default is 1920x1080 / 512x424 for Kinect and 1280x720 / 640x480 for Astra.

If you need any help to display rgb or depth image check the "RgbRenderer" and "DepthRenderer" scripts that have a basic version implemented, or contact us @VoxarLabs through our website!

## Known Issues
[//]: # (https://github.com/OfficeDev/Open-XML-SDK#Known-Issues)
- You MUST set correct resolution to each stream based on the chosen device:
	- Kinect: 
		- RGB (1920, 1080)
		- Depth (512, 424)
	- Astra:  
		- RGB (320, 240) / (640, 480) / (1280, 720)
		- Depth (320, 240) / (640, 480)
- The UnityPackage project show an "invalid dependencies" error
- If you move any astra file it may cause the AstraSDK to stop working
- If you enter an invalid depth resolution it will break the depth stream AND bodytracking. 

## Copyright
[//]: # (https://github.com/NancyFx/Nancy#Copyright)
This repository belongs to and is manteined by VoxarLabs, UFPE. The license is specified in the LICENSE.txt file.