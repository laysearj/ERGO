using UnityEngine;

namespace Voxar
{
    public enum JointType
    {
        Head,
        Neck,
        ShoulderSpine,
        LeftShoulder,
        RightShoulder,
        MiddleSpine,
        BaseSpine,
        LeftHip,
        RightHip,
        LeftElbow,
        LeftWrist,
        LeftHand,
        RightElbow,
        RightWrist,
        RightHand,
        LeftKnee,
        LeftAnkle,
        LeftFoot,
        RightKnee,
        RightAnkle,
        RightFoot
    };

    public class Joint
    {
        public static readonly int JointTypeCount = 21;

        public readonly Status status;
        public readonly JointType type;

        public Vector3 worldPosition;
        public Vector2 depthPosition;

        public readonly Vector3 forward;
        public readonly Vector3 upwards;

        public Joint(JointType type, Status status, Vector3 worldPos, Vector3 depthPos, Vector3 forward, Vector3 upwards)
        {
            this.type = type;
            this.status = status;

            this.worldPosition = worldPos;
            this.depthPosition = depthPos;

            this.forward = forward;
            this.upwards = upwards;
        }
    }
}
