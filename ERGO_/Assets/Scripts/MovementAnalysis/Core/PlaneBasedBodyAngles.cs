using UnityEngine;
using System.Collections.Generic;

namespace Voxar
{
    public class PlaneBasedBodyAngles
    {
        //this is a part of a complete BodyAngles that only takes in consideration the angles in the following default plane:
        public BasePlanes plane;
        
        //Example: the angle between Bonetype.UpperBody and BoneType.LowerBody is { float angle = angles[BoneType.UpperBody][BoneType.LowerBody] }
        public Dictionary<BoneType, Dictionary<BoneType, float>> angles;

        public PlaneBasedBodyAngles(BasePlanes targetPlane)
        {
            plane = targetPlane;
            angles = new Dictionary<BoneType, Dictionary<BoneType, float>>();

            for (int i = 0; i < Bone.boneTypeCount; i++)
            {
                angles[(BoneType)i] = new Dictionary<BoneType, float>();
            }
        }

        public PlaneBasedBodyAngles(BodyJoints body, BasePlanes targetPlane)
        {
            plane = targetPlane;
            angles = new Dictionary<BoneType, Dictionary<BoneType, float>>();

            for (int i = 0; i < Bone.boneTypeCount; i++)
            {
                angles.Add((BoneType) i, new Dictionary<BoneType, float>());
            }

            //init a PlaneBaseBodyAngles from the body

            var bones = MovementAnalyzer.ExtractBonesFromBodyJoints(body);

            var upperPlanes = MovementAnalyzer.GenerateUpperBodyPlanes(body);
            var lowerPlanes = MovementAnalyzer.GenerateLowerBodyPlanes(body);
            var centralPlanes = MovementAnalyzer.GenerateCentralTrunkPlanes(body);
            
            foreach (var entry in bones)
            {
                var type = entry.Key;
                var bone = entry.Value;

                switch(type)
                {
                    case BoneType.UpperNeck:
                        CheckAndAddAngle(type, BoneType.LowerNeck, bones, upperPlanes[plane]);
                        break;
                    case BoneType.LowerNeck:
                        CheckAndAddAngle(type, BoneType.UpperNeck, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.UpperBody, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftClavicule, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightClavicule, bones, upperPlanes[plane]);
                        break;
                    case BoneType.UpperBody:
                        CheckAndAddAngle(type, BoneType.LowerNeck, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftClavicule, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightClavicule, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LowerBody, bones, upperPlanes[plane]);
                        break;
                    case BoneType.LeftClavicule:
                        CheckAndAddAngle(type, BoneType.LowerNeck, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightClavicule, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.UpperBody, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftArm, bones, upperPlanes[plane]);
                        break;
                    case BoneType.LeftArm:
                        CheckAndAddAngle(type, BoneType.LeftClavicule, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftForearm, bones, upperPlanes[plane]);
                        break;
                    case BoneType.LeftForearm:
                        CheckAndAddAngle(type, BoneType.LeftArm, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftWrist, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.LeftWrist:
                        CheckAndAddAngle(type, BoneType.LeftForearm, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.RightClavicule:
                        CheckAndAddAngle(type, BoneType.LowerNeck, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftClavicule, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.UpperBody, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightArm, bones, upperPlanes[plane]);
                        break;
                    case BoneType.RightArm:
                        CheckAndAddAngle(type, BoneType.RightClavicule, bones, upperPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightForearm, bones, upperPlanes[plane]);
                        break;
                    case BoneType.RightForearm:
                        CheckAndAddAngle(type, BoneType.RightArm, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightWrist, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.RightWrist:
                        CheckAndAddAngle(type, BoneType.RightForearm, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.LowerBody:
                        CheckAndAddAngle(type, BoneType.UpperBody, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftHipbone, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightHipbone, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.LeftHipbone:
                        CheckAndAddAngle(type, BoneType.LowerBody, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightHipbone, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftThigh, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.LeftThigh:
                        CheckAndAddAngle(type, BoneType.LeftHipbone, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftLeg, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.LeftLeg:
                        CheckAndAddAngle(type, BoneType.LeftThigh, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftAnkle, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.LeftAnkle:
                        CheckAndAddAngle(type, BoneType.LeftLeg, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.RightHipbone:
                        CheckAndAddAngle(type, BoneType.LowerBody, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.LeftHipbone, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightThigh, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.RightThigh:
                        CheckAndAddAngle(type, BoneType.RightHipbone, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightLeg, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.RightLeg:
                        CheckAndAddAngle(type, BoneType.RightThigh, bones, lowerPlanes[plane]);
                        CheckAndAddAngle(type, BoneType.RightAnkle, bones, lowerPlanes[plane]);
                        break;
                    case BoneType.RightAnkle:
                        CheckAndAddAngle(type, BoneType.RightLeg, bones, lowerPlanes[plane]);
                        break;
                }
            }
        }

        public void AddAngle(BoneType first, BoneType second, float angle)
        {
            angles[first].Add(second, angle);
        }

        //returns the angle between first and second bones
        //retuns infinity if there is no angle between them in this PlaneBasedBodyAngles
        public float GetAngle(BoneType first, BoneType second)
        {
            if (angles.ContainsKey(first))
            {
                if (angles[first].ContainsKey(second))
                {
                    return angles[first][second];
                }
            }
            else if (angles.ContainsKey(second))
            {
                if (angles[second].ContainsKey(first))
                {
                    return angles[second][first];
                }
            }

            return Mathf.Infinity;
        }


        //if the second bone is in the bones dictionary, checks if the angle between first and second has already been added
        private void CheckAndAddAngle(BoneType first, BoneType second, Dictionary<BoneType,Bone> bones, Vector3 planeNormal)
        {
            if (bones.ContainsKey(second))
            {
                if (GetAngle(first, second) == Mathf.Infinity)
                {
                    var angle = MovementAnalyzer.Angle(bones[first], bones[second], planeNormal);
                    AddAngle(first, second, angle);
                }
            }
        }
    }
}
