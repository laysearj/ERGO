using UnityEngine;
using System.Collections.Generic;

namespace Voxar
{
    public class MovementAnalyzer
    {
        private static bool IsAngleWithinInterval(float angle, float target, float margin)
        {
            bool result = false;
            float secondaryTarget = Mathf.Infinity;

            if (target == 0.0f)
            {
                secondaryTarget = 180.0f;
            }

            if ((target - margin) <= angle && angle <= (target + margin))
            {
                result = true;
            }
            else
            {
                if(secondaryTarget != Mathf.Infinity)
                {
                    result = IsAngleWithinInterval(angle, secondaryTarget, margin);
                }
            }

            return result;
        }

        //returns the angle between 2 bones in the 3D space, not considering any plane
        public static float Angle(Bone first, Bone second)
        {
            return Vector3.Angle(first.worldVector.normalized, second.worldVector.normalized);
        }

        //returns the angle between 2 bones in a specific plane
        public static float Angle(Bone first, Bone second, Vector3 planeNormal)
        {
            var firstProjection = Vector3.ProjectOnPlane(first.worldVector, planeNormal).normalized;
            var secondProjection = Vector3.ProjectOnPlane(second.worldVector, planeNormal).normalized;

            return Vector3.Angle(firstProjection, secondProjection);
        }

        public static Dictionary<BoneType, Bone> ExtractBonesFromBodyJoints(BodyJoints body)
        {
            var bones = new Dictionary<BoneType, Bone>();
            foreach (var entry in body.joints)
            {
                if (entry.Value.status == Status.Tracking)
                {
                    Bone bone;
                    switch (entry.Key)
                    {
                        case Voxar.JointType.Head:
                            if (body.joints.ContainsKey(JointType.Neck))
                            {
                                var neck = body.joints[JointType.Neck];

                                bone = new Bone(neck, entry.Value);
                                bones[BoneType.UpperNeck] = bone;
                            }
                            break;
                        case Voxar.JointType.Neck:
                            if (body.joints.ContainsKey(JointType.ShoulderSpine))
                            {
                                var shoulderSpine = body.joints[JointType.ShoulderSpine];

                                bone = new Bone(shoulderSpine, entry.Value);
                                bones[BoneType.LowerNeck] = bone;
                            }
                            break;
                        case Voxar.JointType.ShoulderSpine:
                            if (body.joints.ContainsKey(JointType.LeftShoulder))
                            {
                                var leftShoulder = body.joints[JointType.LeftShoulder];

                                bone = new Bone(entry.Value, leftShoulder);
                                bones[BoneType.LeftClavicule] = bone;
                            }
                            if (body.joints.ContainsKey(JointType.RightShoulder))
                            {
                                var rightShoulder = body.joints[JointType.RightShoulder];

                                bone = new Bone(entry.Value, rightShoulder);
                                bones[BoneType.RightClavicule] = bone;
                            }
                            if (body.joints.ContainsKey(JointType.MiddleSpine))
                            {
                                var middleSpine = body.joints[JointType.MiddleSpine];

                                bone = new Bone(middleSpine, entry.Value);
                                bones[BoneType.UpperBody] = bone;
                            }
                            break;
                        case Voxar.JointType.LeftShoulder:
                            if (body.joints.ContainsKey(JointType.LeftElbow))
                            {
                                var leftElbow = body.joints[JointType.LeftElbow];

                                bone = new Bone(entry.Value, leftElbow);
                                bones[BoneType.LeftArm] = bone;
                            }
                            break;
                        case Voxar.JointType.RightShoulder:
                            if (body.joints.ContainsKey(JointType.RightElbow))
                            {
                                var rightElbow = body.joints[JointType.RightElbow];

                                bone = new Bone(entry.Value, rightElbow);
                                bones[BoneType.RightArm] = bone;
                            }
                            break;
                        case Voxar.JointType.LeftElbow:
                            if (body.joints.ContainsKey(JointType.LeftWrist))
                            {
                                var leftWrist = body.joints[Voxar.JointType.LeftWrist];

                                bone = new Bone(entry.Value, leftWrist);
                                bones[BoneType.LeftForearm] = bone;
                            }
                            break;
                        case Voxar.JointType.RightElbow:
                            if (body.joints.ContainsKey(JointType.RightWrist))
                            {
                                var rightWrist = body.joints[JointType.RightWrist];

                                bone = new Bone(entry.Value, rightWrist);
                                bones[BoneType.RightForearm] = bone;
                            }
                            break;
                        case Voxar.JointType.LeftWrist:
                            if (body.joints.ContainsKey(JointType.LeftHand))
                            {
                                var leftHand = body.joints[JointType.LeftHand];

                                bone = new Bone(entry.Value, leftHand);
                                bones[BoneType.LeftWrist] = bone;
                            }
                            break;
                        case Voxar.JointType.RightWrist:
                            if (body.joints.ContainsKey(JointType.RightHand))
                            {
                                var rightHand = body.joints[JointType.RightHand];

                                bone = new Bone(entry.Value, rightHand);
                                bones[BoneType.RightWrist] = bone;
                            }
                            break;
                        case Voxar.JointType.MiddleSpine:
                            if (body.joints.ContainsKey(JointType.BaseSpine))
                            {
                                var baseSpine = body.joints[JointType.BaseSpine];

                                bone = new Bone(baseSpine, entry.Value);
                                bones[BoneType.LowerBody] = bone;
                            }
                            break;
                        case JointType.BaseSpine:
                            if (body.joints.ContainsKey(JointType.LeftHip))
                            {
                                var leftHip = body.joints[JointType.LeftHip];

                                bone = new Bone(entry.Value, leftHip);
                                bones[BoneType.LeftHipbone] = bone;
                            }
                            if (body.joints.ContainsKey(JointType.RightHip))
                            {
                                var rightHip = body.joints[JointType.RightHip];

                                bone = new Bone(entry.Value, rightHip);
                                bones[BoneType.RightHipbone] = bone;
                            }
                            break;
                        case JointType.LeftHip:
                            if (body.joints.ContainsKey(JointType.LeftKnee))
                            {
                                var leftKnee = body.joints[JointType.LeftKnee];

                                bone = new Bone(entry.Value, leftKnee);
                                bones[BoneType.LeftThigh] = bone;
                            }
                            break;
                        case JointType.RightHip:
                            if (body.joints.ContainsKey(JointType.RightKnee))
                            {
                                var rightKnee = body.joints[JointType.RightKnee];

                                bone = new Bone(entry.Value, rightKnee);
                                bones[BoneType.RightThigh] = bone;
                            }
                            break;
                        case JointType.LeftKnee:
                            if (body.joints.ContainsKey(JointType.LeftAnkle))
                            {
                                var leftAnkle = body.joints[JointType.LeftAnkle];

                                bone = new Bone(entry.Value, leftAnkle);
                                bones[BoneType.LeftLeg] = bone;
                            }
                            break;
                        case JointType.RightKnee:
                            if (body.joints.ContainsKey(JointType.RightAnkle))
                            {
                                var rightAnkle = body.joints[JointType.RightAnkle];

                                bone = new Bone(entry.Value, rightAnkle);
                                bones[BoneType.RightLeg] = bone;
                            }
                            break;
                        case JointType.LeftAnkle:
                            if (body.joints.ContainsKey(JointType.LeftFoot))
                            {
                                var leftFoot = body.joints[JointType.LeftFoot];

                                bone = new Bone(entry.Value, leftFoot);
                                bones[BoneType.LeftAnkle] = bone;
                            }
                            break;
                        case JointType.RightAnkle:
                            if (body.joints.ContainsKey(JointType.RightFoot))
                            {
                                var rightFoot = body.joints[JointType.RightFoot];

                                bone = new Bone(entry.Value, rightFoot);
                                bones[BoneType.RightAnkle] = bone;
                            }
                            break;
                        case JointType.LeftHand:
                        case JointType.RightHand:
                        case JointType.LeftFoot:
                        case JointType.RightFoot:
                        default:
                            //This joints have no more bones associeted
                            break;
                    }
                }
            }

            return bones;
        }

        //returns the angles between the two bones in the 3 default planes (frontal, sagital, horizontal) 
        public static Dictionary<BasePlanes, float> CalculateAngleForBones(BodyJoints body, BoneType first, BoneType second)
        {
            var bones = ExtractBonesFromBodyJoints(body);
            var angles = new Dictionary<BasePlanes, float>();
            Dictionary<BasePlanes, Vector3> planes;
            switch(first)
            {
                case BoneType.UpperNeck:
                case BoneType.LowerNeck:
                case BoneType.UpperBody:
                case BoneType.LeftClavicule:
                case BoneType.RightClavicule:
                    planes = GenerateUpperBodyPlanes(body);
                    break;
                case BoneType.LowerBody:
                case BoneType.LeftHipbone:
                case BoneType.RightHipbone:
                    planes = GenerateUpperBodyPlanes(body);
                    break;
                default:
                    Debug.Log("Devia ter implementado esse caso ein");
                    planes = new Dictionary<BasePlanes, Vector3>();
                    break;
            }

            angles.Add(BasePlanes.Frontal, Angle(bones[first], bones[second], planes[BasePlanes.Frontal]));
            angles.Add(BasePlanes.Sagittal, Angle(bones[first], bones[second], planes[BasePlanes.Sagittal]));
            angles.Add(BasePlanes.Horizontal, Angle(bones[first], bones[second], planes[BasePlanes.Horizontal]));

            return angles;
        }


        //compares all the bones from the passed body to the BodyAngles(a targetBodyAngles in all 3 default plane)
        //returns a List of PoseError that tells which bones were off and the default plane they were judged
        public static List<PoseError> CompareBodyAngles(BodyAngles currentBodyAngles, BodyAngles targetBodyAngles, float tolerance)
        {
            var totalErrors = new List<PoseError>();
            List<PoseError> partialErrors;

            partialErrors = ComparePlaneBasedBodyAngles(currentBodyAngles.FrontalAngles, targetBodyAngles.FrontalAngles, tolerance);
            foreach (var error in partialErrors)
            {
                totalErrors.Add(error);
            }

            partialErrors = ComparePlaneBasedBodyAngles(currentBodyAngles.SagittalAngles, targetBodyAngles.SagittalAngles, tolerance);
            foreach (var error in partialErrors)
            {
                totalErrors.Add(error);
            }

            partialErrors = ComparePlaneBasedBodyAngles(currentBodyAngles.HorizontalAngles, targetBodyAngles.HorizontalAngles, tolerance);
            foreach (var error in partialErrors)
            {
                totalErrors.Add(error);
            }

            return totalErrors;
        }

        //compares all the bones from the passed body to the PlaneBasedBodyPose(a targetBodyAngles in only one default plane)
        //returns a List of PoseError that tells which bones were off and the default plane they were judged
        public static List<PoseError> ComparePlaneBasedBodyAngles(PlaneBasedBodyAngles currentBodyAngles, PlaneBasedBodyAngles targetBodyAngles, float tolerance)
        {

            List<PoseError> totalErrors = new List<PoseError>();

            foreach (var entry in targetBodyAngles.angles)
            {
                var type = entry.Key;

                List<PoseError> localErrors;
                localErrors = CompareAngleForBone(type, currentBodyAngles, targetBodyAngles, tolerance);

                foreach (var error in localErrors)
                {
                    totalErrors.Add(error);
                }
            }

            return totalErrors;
        }

        private static List<PoseError> CompareAngleForBone(BoneType reference, PlaneBasedBodyAngles current, PlaneBasedBodyAngles target, float tolerance)
        {
            var ret = new List<PoseError>();

            foreach (var entry in target.angles[reference])
            {
                var targetAngle = entry.Value;
                var actualAngle = current.GetAngle(reference, entry.Key);
                if (!IsAngleWithinInterval(actualAngle, targetAngle, tolerance))
                {
                    //TODO calculate the error margin
                    float margin = (targetAngle - tolerance);

                    //mark the pair of bones as an error
                    var error = new PoseError(target.plane, reference, entry.Key, margin);
                    ret.Add(error);
                }
                else
                {
                    //if we need to do something when its right
                }
            }

            return ret;
        }

        public static Dictionary<BasePlanes, Vector3> GenerateUpperBodyPlanes(BodyJoints body)
        {
            var planes = new Dictionary<BasePlanes, Vector3>();

            var sagitalPlaneNormal = body.joints[JointType.RightShoulder].worldPosition - body.joints[JointType.LeftShoulder].worldPosition;
            planes.Add(BasePlanes.Sagittal, sagitalPlaneNormal);

            var horizontalPlaneNormal = body.joints[JointType.MiddleSpine].worldPosition - body.joints[JointType.ShoulderSpine].worldPosition;
            planes.Add(BasePlanes.Horizontal, horizontalPlaneNormal);

            var frontalPlaneNormal = Vector3.Cross(sagitalPlaneNormal, horizontalPlaneNormal);
            planes.Add(BasePlanes.Frontal, frontalPlaneNormal);

            return planes;
        }

        public static Dictionary<BasePlanes, Vector3> GenerateLowerBodyPlanes(BodyJoints body)
        {
            var planes = new Dictionary<BasePlanes, Vector3>();

            var sagitalPlaneNormal = body.joints[JointType.RightHip].worldPosition - body.joints[JointType.LeftHip].worldPosition;
            planes.Add(BasePlanes.Sagittal, sagitalPlaneNormal);

            var horizontalPlaneNormal = body.joints[JointType.BaseSpine].worldPosition - body.joints[JointType.MiddleSpine].worldPosition;
            planes.Add(BasePlanes.Horizontal, horizontalPlaneNormal);

            var frontalPlaneNormal = Vector3.Cross(sagitalPlaneNormal, horizontalPlaneNormal);
            planes.Add(BasePlanes.Frontal, frontalPlaneNormal);

            return planes;
        }

        public static Dictionary<BasePlanes, Vector3> GenerateCentralTrunkPlanes(BodyJoints body)
        {
            var planes = new Dictionary<BasePlanes, Vector3>();

            var sagitalPlaneNormal = body.joints[JointType.RightHip].worldPosition - body.joints[JointType.LeftHip].worldPosition;
            planes.Add(BasePlanes.Sagittal, sagitalPlaneNormal);

            var horizontalPlaneNormal = Vector3.up;
            planes.Add(BasePlanes.Horizontal, horizontalPlaneNormal);

            var frontalPlaneNormal = Vector3.Cross(sagitalPlaneNormal, horizontalPlaneNormal);
            planes.Add(BasePlanes.Frontal, frontalPlaneNormal);

            return planes;
        }
    }
}
