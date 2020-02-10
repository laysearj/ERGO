using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;
using Voxar;

public class KinectSource : ISource
{
    public List<IImageReceiver<Texture2D>> RgbReceivers;
    public List<IImageReceiver<Texture2D>> DepthReceivers;
    public List<IReceiver<BodyJoints[]>> BodyReceivers;

    private byte[] rgbData;
    private ushort[] depthData;
    // private Kinect.Body[] kinectBodies;

    private Color[] depthTextureBuffer;

    private Texture2D rgbTexture;
    private Texture2D depthTexture;

    private Vector2Int rgbRes;
    private Vector2Int depthRes;
    private int numberOfBodies;

    // BodySourceManager bodySourceManager;

    public KinectSource(Vector2Int rgbResolution, Vector2Int depthResolution, int bodyCount)
    {
        rgbRes = rgbResolution;
        depthRes = depthResolution;

        rgbData = new byte[4 * rgbRes.x * rgbRes.y];
        depthData = new ushort[depthRes.x * depthRes.y];
        depthTextureBuffer = new Color[depthRes.x * depthRes.y];

        numberOfBodies = bodyCount;

        // kinectBodies = new Astra.Body[numberOfBodies];
        // kinectBodies = new Kinect.Body[numberOfBodies];

        rgbTexture = new Texture2D(rgbRes.x, rgbRes.y, TextureFormat.RGBA32, false);
        depthTexture = new Texture2D(depthRes.x, depthRes.y);

        RgbReceivers = new List<IImageReceiver<Texture2D>>();
        DepthReceivers = new List<IImageReceiver<Texture2D>>();
        BodyReceivers = new List<IReceiver<BodyJoints[]>>();
    }

    public void AddRGBReceiver(IImageReceiver<Texture2D> receiver)
    {
        RgbReceivers.Add(receiver);
        receiver.SetDataResolution(rgbRes.x, rgbRes.y);
    }

    public void AddDepthReceiver(IImageReceiver<Texture2D> receiver)
    {
        DepthReceivers.Add(receiver);
        receiver.SetDataResolution(depthRes.x, depthRes.y);
    }

    public void AddBodyReceiver(IReceiver<BodyJoints[]> receiver)
    {
        BodyReceivers.Add(receiver);
    }

    public BodyJoints GetBody<T>(T source)
    {
        Kinect.Body kinectBody = source as Kinect.Body;

        int id = (int)kinectBody.TrackingId;
        Status status = Status.NotTracking;
        Dictionary<JointType, Voxar.Joint> joints = new Dictionary<JointType, Voxar.Joint>();

        if (kinectBody.IsTracked)
        {
            status = Voxar.Status.Tracking;

            foreach (Kinect.Joint kinectJoint in kinectBody.Joints.Values)
            {
                // Debug.Log(kinectJoint.JointType);

                if (kinectJoint.JointType == Kinect.JointType.HandTipLeft || kinectJoint.JointType == Kinect.JointType.HandTipRight
                || kinectJoint.JointType == Kinect.JointType.ThumbLeft || kinectJoint.JointType == Kinect.JointType.ThumbRight)
                {
                    continue;
                }
                Voxar.Joint joint = GetJoint(kinectJoint);
                joints.Add(joint.type, joint);
            }
        }
        else
        {
            status = Voxar.Status.NotTracking;
        }

        Voxar.BodyJoints body = new Voxar.BodyJoints(id, status, joints);

        return body;
    }

    public Voxar.Joint GetJoint(Kinect.Joint kinectJoint)
    {
        Voxar.Status status = Voxar.Status.NotTracking;
        JointType type = JointType.BaseSpine;

        switch (kinectJoint.TrackingState)
        {
            case Kinect.TrackingState.NotTracked:
                status = Voxar.Status.NotTracking;
                break;
            case Kinect.TrackingState.Inferred:
                status = Voxar.Status.Inferred;
                break;
            case Kinect.TrackingState.Tracked:
                status = Voxar.Status.Tracking;
                break;
        }

        switch (kinectJoint.JointType)
        {
            case Kinect.JointType.Head:
                type = JointType.Head;
                break;
            case Kinect.JointType.Neck:
                type = JointType.Neck;
                break;
            case Kinect.JointType.SpineShoulder:
                type = JointType.ShoulderSpine;
                break;
            case Kinect.JointType.ShoulderLeft:
                type = JointType.LeftShoulder;
                break;
            case Kinect.JointType.ShoulderRight:
                type = JointType.RightShoulder;
                break;
            case Kinect.JointType.SpineMid:
                type = JointType.MiddleSpine;
                break;
            case Kinect.JointType.SpineBase:
                type = JointType.BaseSpine;
                break;
            case Kinect.JointType.HipLeft:
                type = JointType.LeftHip;
                break;
            case Kinect.JointType.HipRight:
                type = JointType.RightHip;
                break;
            case Kinect.JointType.ElbowLeft:
                type = JointType.LeftElbow;
                break;
            case Kinect.JointType.WristLeft:
                type = JointType.LeftWrist;
                break;
            case Kinect.JointType.HandLeft:
                type = JointType.LeftHand;
                break;
            case Kinect.JointType.ElbowRight:
                type = JointType.RightElbow;
                break;
            case Kinect.JointType.WristRight:
                type = JointType.RightWrist;
                break;
            case Kinect.JointType.HandRight:
                type = JointType.RightHand;
                break;
            case Kinect.JointType.KneeLeft:
                type = JointType.LeftKnee;
                break;
            case Kinect.JointType.FootLeft:
                type = JointType.LeftFoot;
                break;
            case Kinect.JointType.KneeRight:
                type = JointType.RightKnee;
                break;
            case Kinect.JointType.FootRight:
                type = JointType.RightFoot;
                break;
            case Kinect.JointType.AnkleLeft:
                type = JointType.LeftAnkle;
                break;
            case Kinect.JointType.AnkleRight:
                type = JointType.RightAnkle;
                break;
            default:
                throw new System.Exception("invalid jointType");
                break;
        }

        var worldPosition = new Vector3(kinectJoint.Position.X * 1000f, kinectJoint.Position.Y * 1000f, kinectJoint.Position.Z * 1000f);
        var depthPosition = new Vector2(0, 0); // TODO: retrieve depth



        // var upwards = new Vector3(astra.Orientation.M01, astra.Orientation.M11, astra.Orientation.M21);
        // var forward = new Vector3(-astra.Orientation.M02, -astra.Orientation.M12, -astra.Orientation.M22);

        var upwards = new Vector3(0, 1, 0); // TODO: retrieve sensor's coordinates;
        var forward = new Vector3(0, 0, 1); // TODO: retrieve sensor's coordinates;

        Voxar.Joint joint = new Voxar.Joint(type, status, worldPosition, depthPosition, forward, upwards);

        return joint;
    }

    public void OnNewBodyFrame<S, T>(S bodyStream, T bodyFrame)
    {
        Kinect.Body[] kinectBodies = bodyStream as Kinect.Body[];

        // Voxar.Body[] bodies = new Voxar.Body[kinectBodies.Length];
        Voxar.BodyJoints[] bodies = new Voxar.BodyJoints[1];

        // int nBodies = 0;
        foreach (Kinect.Body kinectBody in kinectBodies)
        {
            if (kinectBody.IsTracked)
            {
                // nBodies++;
                bodies[0] = GetBody(kinectBody);
            }
        }

        // Debug.Log(bodies[0].joints[Voxar.Joint.JointType.BaseSpine].worldPosition.x);

        foreach (var receiver in BodyReceivers)
        {
            receiver.ReceiveData(bodies);
            // Debug.Log("end onnewbodyframe");
        }
    }

    public void OnNewDepthFrame<T>(T depthFrame)
    {
        var frame = depthFrame as Kinect.DepthFrame;

        if (frame != null)
        {
            frame.CopyFrameDataToArray(depthData);

            EnsureBuffers(depthTexture, depthData);
            MapDepthToTexture(depthData, depthTexture);

            foreach (var receiver in DepthReceivers)
            {
                receiver.ReceiveData(depthTexture);
            }
        }
    }

    public void OnNewRGBFrame<T>(T rgbFrame)
    {
        var frame = rgbFrame as Kinect.ColorFrame;

        if (frame != null)
        {
            frame.CopyConvertedFrameDataToArray(rgbData, Kinect.ColorImageFormat.Rgba);

            rgbTexture.LoadRawTextureData(rgbData);

            foreach (var receiver in RgbReceivers)
            {
                receiver.ReceiveData(rgbTexture);
            }
        }
    }

    private void EnsureBuffers(Texture2D target, ushort[] data)
    {
        int length = depthRes.x * depthRes.y;
        if (depthTextureBuffer.Length != length)
        {
            depthTextureBuffer = new Color[length];
        }

        if (data.Length != length)
        {
            data = new ushort[length];
        }

        if (target != null)
        {
            if (target.width != depthRes.x ||
                target.height != depthRes.y)
            {
                target.Resize(depthRes.x, depthRes.y);
            }
        }
    }

    void MapDepthToTexture(ushort[] depthPixels, Texture2D target)
    {
        int length = depthPixels.Length;
        for (int i = 0; i < length; i++)
        {
            ushort depth = depthPixels[i];

            float depthScaled = 0.0f;
            if (depth != 0)
            {
                depthScaled = 1.0f - (depth / 10000.0f);
            }

            depthTextureBuffer[i].r = depthScaled;
            depthTextureBuffer[i].g = depthScaled;
            depthTextureBuffer[i].b = depthScaled;
            depthTextureBuffer[i].a = 1.0f;
        }

        target.SetPixels(depthTextureBuffer);
    }
}