using UnityEngine;
using UnityEngine.Events;
using Voxar;
using Joint = Voxar.Joint;

[System.Serializable]
public class ResolutionChangeListener : UnityEvent<Vector2Int> { }

public interface ISource
{
    void OnNewRGBFrame<T>(T rgbFrame);

    void OnNewDepthFrame<T>(T depthFrame);

    void OnNewBodyFrame<S, T>(S bodyStream, T bodyFrame);

    BodyJoints GetBody<T>(T source);

    //Joint GetJoint<T>(T source);

    void AddRGBReceiver(IImageReceiver<Texture2D> receiver);

    void AddDepthReceiver(IImageReceiver<Texture2D> receiver);

    void AddBodyReceiver(IReceiver<BodyJoints[]> receiver);
}
