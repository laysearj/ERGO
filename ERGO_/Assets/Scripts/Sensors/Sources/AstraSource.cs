using System.Collections.Generic;
using UnityEngine;
using Voxar;
using Joint = Voxar.Joint;

public class AstraSource : ISource
{
    public List<IImageReceiver<Texture2D>> RgbReceivers;
    public List<IImageReceiver<Texture2D>> DepthReceivers;
    public List<IReceiver<BodyJoints[]>> BodyReceivers;

    private long rgbFrameIndex = 0;
    private long depthFrameIndex = 0;
    private long bodyFrameIndex = 0;

    private byte[] rgbData;
    private short[] depthData;
    private Astra.Body[] astraBodies;

    private Color[] depthTextureBuffer;

    private Texture2D rgbTexture;
    private Texture2D depthTexture;

    private Vector2Int rgbRes;
    private Vector2Int depthRes;
    private int numberOfBodies;

    public AstraSource(Vector2Int rgbResolution, Vector2Int depthResolution, int bodyCount)
    {
        rgbRes = rgbResolution;
        depthRes = depthResolution;

        rgbData = new byte[rgbRes.x * rgbRes.y];
        depthData = new short[depthRes.x * depthRes.y];
        depthTextureBuffer = new Color[depthRes.x * depthRes.y];

        rgbTexture = new Texture2D(rgbRes.x, rgbRes.y, TextureFormat.RGB24, false);
        depthTexture = new Texture2D(depthRes.x, depthRes.y);

        numberOfBodies = bodyCount;
        astraBodies = new Astra.Body[numberOfBodies];

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
        var astra = source as Astra.Body;

        var id = astra.Id;
        Status status = Status.NotTracking;
        Dictionary<JointType, Joint> joints = new Dictionary<JointType, Joint>();

        switch (astra.Status)
        {
            case Astra.BodyStatus.Tracking:
                status = Status.Tracking;
                break;
            default:
                status = Status.NotTracking;
                break;
        }

        if (status == Status.Tracking)
        {
            for (int i = 0; i < astra.Joints.Length; i++)
            {
                var joint = GetJoint(astra.Joints[i]);
                joints.Add(joint.type, joint);
            }
        }

        var body = new BodyJoints(id, status, joints);

        return body;
    }

    public Voxar.Joint GetJoint<T>(T source)
    {
        var astra = source as Astra.Joint;
        Status status = Status.NotTracking;
        JointType type = JointType.Head;

        switch (astra.Status)
        {
            case Astra.JointStatus.NotTracked:
                status = Status.NotTracking;
                break;
            case Astra.JointStatus.LowConfidence:
                status = Status.Inferred;
                break;
            case Astra.JointStatus.Tracked:
                status = Status.Tracking;
                break;
        }

        switch (astra.Type)
        {
            case Astra.JointType.Head:
                type = JointType.Head;
                break;
            case Astra.JointType.Neck:
                type = JointType.Neck;
                break;
            case Astra.JointType.ShoulderSpine:
                type = JointType.ShoulderSpine;
                break;
            case Astra.JointType.LeftShoulder:
                type = JointType.LeftShoulder;
                break;
            case Astra.JointType.RightShoulder:
                type = JointType.RightShoulder;
                break;
            case Astra.JointType.MidSpine:
                type = JointType.MiddleSpine;
                break;
            case Astra.JointType.BaseSpine:
                type = JointType.BaseSpine;
                break;
            case Astra.JointType.LeftHip:
                type = JointType.LeftHip;
                break;
            case Astra.JointType.RightHip:
                type = JointType.RightHip;
                break;
            case Astra.JointType.LeftElbow:
                type = JointType.LeftElbow;
                break;
            case Astra.JointType.LeftWrist:
                type = JointType.LeftWrist;
                break;
            case Astra.JointType.LeftHand:
                type = JointType.LeftHand;
                break;
            case Astra.JointType.RightElbow:
                type = JointType.RightElbow;
                break;
            case Astra.JointType.RightWrist:
                type = JointType.RightWrist;
                break;
            case Astra.JointType.RightHand:
                type = JointType.RightHand;
                break;
            case Astra.JointType.LeftKnee:
                type = JointType.LeftKnee;
                break;
            case Astra.JointType.LeftFoot:
                type = JointType.LeftFoot;
                break;
            case Astra.JointType.RightKnee:
                type = JointType.RightKnee;
                break;
            case Astra.JointType.RightFoot:
                type = JointType.RightFoot;
                break;
        }

        var worldPosition = new Vector3(astra.WorldPosition.X, astra.WorldPosition.Y, astra.WorldPosition.Z);
        var depthPosition = new Vector2(astra.DepthPosition.X, astra.DepthPosition.Y);

        //ASTRA SDK CheatSheet
        //skel.Joints[i].Orient.Matrix:
        // 0, 			1,	 		2,
        // 3, 			4, 			5,
        // 6, 			7, 			8
        // -------
        // right(X),	up(Y), 		forward(Z)

        //Vector3 jointRight = new Vector3(
        //    bodyJoint.Orientation.M00,
        //    bodyJoint.Orientation.M10,
        //    bodyJoint.Orientation.M20);

        //Vector3 jointUp = new Vector3(
        //    bodyJoint.orientation.M01,
        //    bodyJoint.orientation.M11,
        //    bodyJoint.orientation.M21);

        //Vector3 jointForward = new Vector3(
        //    bodyJoint.orientation.M02,
        //    bodyJoint.orientation.M12,
        //    bodyJoint.orientation.M22);

        var upwards = new Vector3(astra.Orientation.M01, astra.Orientation.M11, astra.Orientation.M21);
        var forward = new Vector3(-astra.Orientation.M02, -astra.Orientation.M12, -astra.Orientation.M22);

        var joint = new Joint(type, status, worldPosition, depthPosition, forward, upwards);

        return joint;
    }

    public void OnNewBodyFrame<S, T>(S bodyStream, T bodyFrame)
    {
        var frame = bodyFrame as Astra.BodyFrame;
        var stream = bodyStream as Astra.BodyStream;

        if (frame != null && stream != null)
        {
            if (frame.Width == 0 ||
            frame.Height == 0)
            {
                return;
            }

            if (bodyFrameIndex == frame.FrameIndex)
            {
                return;
            }

            bodyFrameIndex = frame.FrameIndex;

            frame.CopyBodyData(ref astraBodies);

            var bodies = new BodyJoints[numberOfBodies];

            for (int i = 0; i < this.numberOfBodies; i++)
            {
                bodies[i] = GetBody(astraBodies[i]);
            }

            foreach (var receiver in BodyReceivers)
            {
                receiver.ReceiveData(bodies);
            }
        }
    }

    public void OnNewDepthFrame<T>(T depthFrame)
    {
        var frame = depthFrame as Astra.DepthFrame;

        if (frame != null)
        {
            if (frame.Width == 0 || frame.Height == 0)
            {
                return;
            }

            if (depthFrameIndex == frame.FrameIndex)
            {
                return;
            }

            depthFrameIndex = frame.FrameIndex;

            short[] buffer = new short[depthRes.x * depthRes.y];
            frame.CopyData(ref depthData);

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
        var frame = rgbFrame as Astra.ColorFrame;

        if (frame != null)
        {
            if (frame.Width == 0 || frame.Height == 0)
            {
                return;
            }

            if (rgbFrameIndex == frame.FrameIndex)
            {
                return;
            }

            rgbFrameIndex = frame.FrameIndex;

            frame.CopyData(ref rgbData);

            rgbTexture.LoadRawTextureData(rgbData);

            foreach (var receiver in RgbReceivers)
            {
                receiver.ReceiveData(rgbTexture);
            }
        }
    }

    private void EnsureBuffers(Texture2D target, short[] data)
    {
        int length = depthRes.x * depthRes.y;
        if (depthTextureBuffer.Length != length)
        {
            depthTextureBuffer = new Color[length];
        }

        if (data.Length != length)
        {
            data = new short[length];
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

    void MapDepthToTexture(short[] depthPixels, Texture2D target)
    {
        int length = depthPixels.Length;
        for (int i = 0; i < length; i++)
        {
            short depth = depthPixels[i];

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
