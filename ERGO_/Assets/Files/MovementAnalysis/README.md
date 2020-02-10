# Movement Analysis

> A Unity3D package that reconizes gestures and human poses through the use of Movement Analysis unity package.

## Overview
[//]: # (https://github.com/IdentityServer/IdentityServer3#Overview)
Movement Analysis is a private SDK that allow recording and recognition of gestures and human poses using the International Society of Biomachanics' (ISB) Joint Coordinate System (JCS). It uses the SensorsSDK to access the main components of RGBD devices. Sensors currently supports the following devices:

- Orbbec Astra
- Orbbec Astra Pro
- Microsoft Kinect 2

Movement Analysis was developed by [VoxarLabs](https://voxarlabs.cin.ufpe.br/). VoxarLabs is a research lab that specializes in AR and VR technologies located in Recife, Brazil.

### Version
Movement Analysis is currently at version 0.2.

## Features
[//]: # (https://github.com/mRemoteNG/mRemoteNG#Features)




## Getting Started
[//]: # (https://github.com/ExtendRealityLtd/VRTK#Getting-Started)
Clone the repo (for the full package project) or download the unitypackage file.
### Prerequisites
[//]: # (https://github.com/JasonGT/NorthwindTraders#Prerequisites)
- Unity version 2018.4 or superior
- A compatible RGBD device
- The driver for the device 

kinect v2 sdk can be found [>>>HERE<<<](https://www.microsoft.com/en-us/download/details.aspx?id=44561)
astra orbbec drivers can be found [>>>HERE<<<](https://orbbec3d.com/develop/)

### Setting up the project
[//]: # (https://github.com/ExtendRealityLtd/
VRTK#Setting-up-the-project)
Movement Analysis is distributed with the UnityPackage file, making it simple to install:
1. Create a new Unity project or open the one you wish to import the package to (tested with the 3D template).
2. In the Assets dropdown, select "Import Package -> Custom Package..."
3. In the file selector, select the MovementAnalysis' Unity ("MovementAnalysisSDK.unitypackage") package file that you downloaded.
4. Click on the "All" button in the bottom left corner and then click "Import".

NOTE: if you are using any 2019 version of Unity3D you will receive an alert of obsolete api's. Just click continue and it will work normaly.

These steps will import to your project all the files you need to use the package, including the samples!

### Running the example scene
[//]: # (https://github.com/ExtendRealityLtd/VRTK#Running-the-example-scene)
To test if all went well, go to the Unity project Scenes -> "Movement Analysis'Samples" and open the "BonesRender" scene. Connect the compatible device and hit play. If everything worked, you should see your own joints along with your bones in the screen.

## Applications
[//]: # (https://github.com/dotnet/reactive#Applications)
The package contains 3 samples: BonesRender, PoseMatch, and GestureRecognition

### BonesRender Scene
This scene shows Movement Analysis' ability to get the body's bones positions and orientation, and how to render them as simple cylinders. The Scene has the following elements:
1. Sensor: the prefab from Sensors SDK that will handle the implementation of the rgbd device for us.
2. Body: empty object with 2 childs:
	- Joints: the object that has the script "BodyRenderer" to render all body joints. It implements the interface "IReceiver<T>" to receive the joints' data from the Sensors SDK.
	- Bones: the object that has the script "BoneRenderer" to render all bones. It also implements the same interface "IReceiver<T>" to receive the joints and then uses the class "MovementAnalyzer" to calculate the bones from the joints positions. It uses the result to create cylinders to represent the bones.

###PoseMatch Scene
This scene shows how to use MovementAnalyzer to compare the pose from the current body with a pose that you specify. It contains the following objects:
1. Sensor: the prefab from Sensors SDK that will handle the implementation of the rgbd device for us.
2. Body: the object that has the script "BodyRenderer" to render all joints. It implements the interface "IReceiver<T>" to receive the joints' data from the Sensors SDK.
3. Analyzer: the object with the script "PoseMatch" that will compare the current tracked body's pose with a pose that you specify. It uses the classes BodyAngles and MovementAnalyzer to do this. It also implements the interface "IReceiver<T>" to receive the joints' data from the Sensors SDK.

## Usage
[//]: # (https://github.com/shimat/opencvsharp#Usage)
Since MovementAnalysis is built using Sensors, you might want to take a quick look in the Sensors' README to get a stronger grip of how it works and what you may need to implement to access Depth and/or RGB data in your project. With the basics of Movement Analysis figured out, and have seen it work, it's time to begin developing your own applications!

Let's make a simple sample that tells us when the user is doing an "Y" pose! These are the steps you will need to do this:

- Open the SampleScene and place the "SensorsBody" prefab (included in the unitypackage) in the scene (the prefab should be in the projects "prefabs" folder). This prefab will handle the RGB device while also displaying the body joints as a joint prefab.

- Create an empty GameObject in the Hierarchy, create a MonoBehavior Script, call it "PoseComparer", and attach it to the object. Now you will have to make your new script adhere to the Voxar.IReceiver<Voxar.BodyJoints[]> interface. The class declaration should look like this:

```c#
public class HeadFollower : PoseComparer, Voxar.IReceiver<Voxar.BodyJoints[]>
```

- The compiler should warn you that you must implement the interface method "ReceiveData<Voxar.BodyJoints[]>(Voxar.BodyJoints[] data)". No worries, for now just past this code inside your script:


```c#
//this is the method from the IReceiver<Voxar.Body[]> that you have to implement
//this will calculate the current's body pose and compare it with your desired pose.
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

            //gets all angles from the current body
            var currentBodyAngles = new BodyAngles(body);

            //CompareBodyAngles() compares the currentBodyAngles with the YPOSE given an angle error tolerance described in angleTolerance
            //the method returns a List<PoseError> with all the errors the comparer found between the poses.
            var errors = MovementAnalyzer.CompareBodyAngles(currentBodyAngles, YPOSE, angleTolerance);

            //print any errors to tell the user what he is doing wrong
            foreach (var error in errors)
            {
             	Debug.Log("The angle between your " + error.boneTypes[0] + " and your " + error.boneTypes[1] + " is wrong!");
            }
        }
    }
```

- Now your comparer should say that YPOSE and angleTolerance are not defined. Declare them like so:

```c#
	public float angleTolerance;
	//BodyAngles holds angles in the 3 default planes: Frontal, Sagittal, Horizontal.
	//YPOSE.FrontalAngles
	//YPOSE.SagittalAngles
	//YPOSE.HorizontalAngles
	private BodyAngles YPOSE;
```

- Now the only thing missing in this script describing the YPOSE, all you have to do is add the angles you wish to compare to the desired plane. MovementAnalysis is built with three comparison planes: Frontal, Sagittal and Horizontal. Since the "Y" pose is all with frontal angles you will need to add angles to the "YPOSE.FrontalAngles". I have compile the angles you will need to add to describe a "Y" pose. To init the pose just replace the script's "Start" method with this one:

```c#
	//called before the first update of the object, perfect place to define this thing.
	void Start()
	{
		YPOSE = new BodyAngles();

		YPOSE.FrontalAngles.AddAngle(BoneType.UpperNeck, BoneType.LowerNeck, 0);

		YPOSE.FrontalAngles.AddAngle(BoneType.LowerNeck, BoneType.UpperBody, 0);
		YPOSE.FrontalAngles.AddAngle(BoneType.LowerNeck, BoneType.LeftClavicule, 90);
		YPOSE.FrontalAngles.AddAngle(BoneType.LowerNeck, BoneType.RightClavicule, 90);

		YPOSE.FrontalAngles.AddAngle(BoneType.LeftClavicule, BoneType.RightClavicule, 0);
        YPOSE.FrontalAngles.AddAngle(BoneType.LeftClavicule, BoneType.LeftArm, 45);
        YPOSE.FrontalAngles.AddAngle(BoneType.LeftClavicule, BoneType.UpperBody, 90);

        YPOSE.FrontalAngles.AddAngle(BoneType.LeftArm, BoneType.LeftForearm, 0);
        YPOSE.FrontalAngles.AddAngle(BoneType.LeftForearm, BoneType.LeftWrist, 0);

        YPOSE.FrontalAngles.AddAngle(BoneType.RightClavicule, BoneType.RightArm, 45);
        YPOSE.FrontalAngles.AddAngle(BoneType.RightClavicule, BoneType.UpperBody, 90);

        YPOSE.FrontalAngles.AddAngle(BoneType.RightArm, BoneType.RightForearm, 0);
        YPOSE.FrontalAngles.AddAngle(BoneType.RightForearm, BoneType.RightWrist, 0);

        YPOSE.FrontalAngles.AddAngle(BoneType.UpperBody, BoneType.LowerBody, 0);
	}
```

- Go to the Unity Hierarchy, select the "Sensor" prefab (inside the SensorsBody prefab), change the size of "BodyReceivers" list to 2 in the Unity Inspector, then drag and drop the empty GameObject with the PoseComparer script to the bottom of the "BodyReceivers" list.

- In the "Sensor" prefab select the tracking SDK that corresponds to your device (Kinect for Microsoft Kinect v2, Astra for Orbbec Astra or Astra Pro).

- Now just hit play and you should see your sphere following your head!

If you need any help contact us @VoxarLabs through our website!

## Known Issues
[//]: # (https://github.com/OfficeDev/Open-XML-SDK#Known-Issues)
- The UnityPackage project show an "invalid dependencies" error
- If you move any astra file it may cause the AstraSDK to stop working
- The body's angles depend on the tracking sdk of your choosing, the default's upright body angles may vary from device to device.

## Copyright
[//]: # (https://github.com/NancyFx/Nancy#Copyright)
This repository belongs to and is manteined by VoxarLabs, UFPE. The license is specified in the LICENSE.txt file.