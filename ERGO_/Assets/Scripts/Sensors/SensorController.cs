using System.Collections.Generic;
using UnityEngine;
using Voxar;


public class SensorController : MonoBehaviour
{
    [Space]
    public TrackingSDK trackingSDK = TrackingSDK.Astra;

    [Header("RGB Options")]
    public bool rgb = true;
    public Vector2Int rgbResolution;
    public List<GameObject> rgbReceivers;

    [Header("Depth Options")]
    public bool depth = true;
    public Vector2Int depthResolution;
    public List<GameObject> depthReceivers;

    [Header("Bodytracking Options")]
    public bool bodyTracking = true;
    [Range(1, 3)]
    public int numberOfBodies = 1;
    public List<GameObject> bodyReceivers;

    private ISource source;

    private void InitializeSensor()
    {
        //if (bodyTracking)
        //{
        //    depth = true;
        //}

        switch (trackingSDK)
        {
            case Voxar.TrackingSDK.Astra:
                var controller = gameObject.AddComponent<AstraController>();

                controller._shouldEnableColor = rgb;
                controller._shouldEnableDepth = depth;
                controller._shouldEnableBody = bodyTracking;

                controller.depthResolution = depthResolution;
                controller.rgbResolution = rgbResolution;

                source = new AstraSource(rgbResolution, depthResolution, numberOfBodies);
                controller.NewBodyFrameEvent.AddListener(source.OnNewBodyFrame);
                controller.NewColorFrameEvent.AddListener(source.OnNewRGBFrame);
                controller.NewDepthFrameEvent.AddListener(source.OnNewDepthFrame);
                break;


            case Voxar.TrackingSDK.Kinect:
                KinectController kinectController = gameObject.AddComponent<KinectController>();

                source = new KinectSource(rgbResolution, depthResolution, numberOfBodies);

                kinectController.OnNewBodyFrame.AddListener(source.OnNewBodyFrame);
                kinectController.OnNewColorFrame.AddListener(source.OnNewRGBFrame);
                kinectController.OnNewDepthFrame.AddListener(source.OnNewDepthFrame);

                break;

            default:
                Debug.Log("SDK NOT SUPPORTED");
                break;
        }

        if (bodyTracking)
        {
            foreach (var receiver in bodyReceivers)
            {
                source.AddBodyReceiver(receiver.GetComponent<IReceiver<BodyJoints[]>>());
            }
        }
        if (rgb)
        {
            foreach (var receiver in rgbReceivers)
            {
                source.AddRGBReceiver(receiver.GetComponent<IImageReceiver<Texture2D>>());
            }
        }
        if (depth)
        {
            foreach (var receiver in depthReceivers)
            {
                source.AddDepthReceiver(receiver.GetComponent<IImageReceiver<Texture2D>>());
            }
        }
    }

    private void Awake()
    {
        InitializeSensor();
    }
}
