using UnityEngine;

namespace Voxar
{
    public enum BoneType
    {
        UpperNeck,
        LowerNeck,
        LeftClavicule,
        RightClavicule,
        LeftArm,
        RightArm,
        LeftForearm,
        RightForearm,
        LeftWrist,
        RightWrist,
        UpperBody,
        LowerBody,
        LeftHipbone,
        RightHipbone,
        LeftThigh,
        RightThigh,
        LeftLeg,
        RightLeg,
        LeftAnkle,
        RightAnkle
    };


    public class Bone
    {
        static public readonly int boneTypeCount = 20;

        //private readonly BoneType type;

        private readonly JointType first;
        private readonly JointType second;

        public readonly Vector3 worldPoint;
        public readonly Vector3 worldVector;

        public readonly Vector2 depthPoint;
        public readonly Vector2 depthVector;

        public Bone(Joint first, Joint second)
        {
            this.first = first.type;
            this.second = second.type;

            worldPoint = first.worldPosition;
            worldVector = second.worldPosition - first.worldPosition;

            depthPoint = first.depthPosition;
            depthVector = second.depthPosition - first.depthPosition;
        }
    }
}
