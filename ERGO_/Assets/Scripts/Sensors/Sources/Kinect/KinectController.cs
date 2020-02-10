using UnityEngine;
using UnityEngine.Events;
using Kinect = Windows.Kinect;

public class Kinect_BodyFrameEvent : UnityEvent<Kinect.Body[], Kinect.Body[]> { }
public class Kinect_ColorFrameEvent : UnityEvent<Kinect.ColorFrame> { }
public class Kinect_DepthFrameEvent : UnityEvent<Kinect.DepthFrame> { }

public class KinectController : MonoBehaviour
{
    public Kinect_BodyFrameEvent OnNewBodyFrame = new Kinect_BodyFrameEvent();
    public Kinect_ColorFrameEvent OnNewColorFrame = new Kinect_ColorFrameEvent();
    public Kinect_DepthFrameEvent OnNewDepthFrame = new Kinect_DepthFrameEvent();

    BodySourceManager bodySourceManager;

    private Kinect.KinectSensor _Sensor;
    private Kinect.ColorFrameReader _ColorReader;
    private Kinect.DepthFrameReader _DepthReader;

    private void Start()
    {
        _Sensor = Kinect.KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _ColorReader = _Sensor.ColorFrameSource.OpenReader();
            _DepthReader = _Sensor.DepthFrameSource.OpenReader();

            var colorFrameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(Kinect.ColorImageFormat.Rgba);
            var depthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }

        bodySourceManager = gameObject.AddComponent<BodySourceManager>();
    }

    private void UpdateColor()
    {
        if (_ColorReader != null)
        {
            var frame = _ColorReader.AcquireLatestFrame();

            if (frame != null)
            {
                OnNewColorFrame.Invoke(frame);

                frame.Dispose();
                frame = null;
            }
        }
    }

    private void UpdateDepth()
    {
        if (_DepthReader != null)
        {
            var frame = _DepthReader.AcquireLatestFrame();

            if (frame != null)
            {
                OnNewDepthFrame.Invoke(frame);

                frame.Dispose();
                frame = null;
            }
        }
    }

    private void UpdateBody()
    {
        Kinect.Body[] bodies = bodySourceManager.GetData();

        if (bodies != null)
        {
            foreach (Kinect.Body body in bodies)
            {
                if (body.IsTracked)
                {
                    // Debug.Log("> from KinectController:" + body.Joints[Kinect.JointType.Head].Position.X.ToString());
                }
            }

            OnNewBodyFrame?.Invoke(bodies, bodies);
        }

    }

    private void Update()
    {
        UpdateColor();
        UpdateDepth();
        UpdateBody();
    }

    private void OnApplicationQuit()
    {
        if (_ColorReader != null)
        {
            _ColorReader.Dispose();
            _ColorReader = null;
        }

        if (_DepthReader != null)
        {
            _DepthReader.Dispose();
            _DepthReader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}