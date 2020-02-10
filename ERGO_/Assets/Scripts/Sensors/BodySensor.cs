using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxar;

public class BodySensor : MonoBehaviour, IReceiver<BodyJoints[]>
{
    [Header("Options")]
    public bool displayTrackedJoints;
    public bool displayInferredJoints;
    public bool displayUntrackedJoints;

    [Header("Joint Objects")]
    public GameObject Head;
    public GameObject Neck;
    public GameObject ShoulderSpine;

    public GameObject LeftShoulder;
    public GameObject LeftElbow;
    public GameObject LeftWrist;
    public GameObject LeftHand;

    public GameObject RightShoulder;
    public GameObject RightElbow;
    public GameObject RightWrist;
    public GameObject RightHand;

    public GameObject MiddleSpine;
    public GameObject BaseSpine;

    public GameObject LeftHip;
    public GameObject LeftKnee;
    public GameObject LeftAnkle;
    public GameObject LeftFoot;

    public GameObject RightHip;
    public GameObject RightKnee;
    public GameObject RightAnkle;
    public GameObject RightFoot;

    public void ReceiveData(BodyJoints[] data)
    {
        foreach (var body in data)
        {
            if (body.status == Status.NotTracking)
            {
                continue;
            }

            foreach (var entry in body.joints)
            {
                var type = entry.Key;
                var joint = entry.Value;
                var orientation = Quaternion.LookRotation(joint.forward, joint.upwards);

                SetPositionAndOrientation(type, joint.worldPosition, orientation);
                switch (joint.status)
                {
                    case Status.Tracking:
                        SetActive(type, displayTrackedJoints);
                        break;
                    case Status.Inferred:
                        SetActive(type, displayInferredJoints);
                        break;
                    case Status.NotTracking:
                        SetActive(type, displayUntrackedJoints);
                        break;
                }
            }
        }
    }

    private void SetPositionAndOrientation(JointType type, Vector3 position, Quaternion quaternion)
    {
        switch (type)
        {
            case JointType.Head:
                Head.transform.position = position / 1000.0f;
                Head.transform.rotation = quaternion;
                break;
            case JointType.Neck:
                Neck.transform.position = position / 1000.0f;
                Neck.transform.rotation = quaternion;
                break;
            case JointType.ShoulderSpine:
                ShoulderSpine.transform.position = position / 1000.0f;
                ShoulderSpine.transform.rotation = quaternion;
                break;
            case JointType.LeftShoulder:
                LeftShoulder.transform.position = position / 1000.0f;
                LeftShoulder.transform.rotation = quaternion;
                break;
            case JointType.LeftElbow:
                LeftElbow.transform.position = position / 1000.0f;
                LeftElbow.transform.rotation = quaternion;
                break;
            case JointType.LeftWrist:
                LeftWrist.transform.position = position / 1000.0f;
                LeftWrist.transform.rotation = quaternion;
                break;
            case JointType.LeftHand:
                LeftHand.transform.position = position / 1000.0f;
                LeftHand.transform.rotation = quaternion;
                break;
            case JointType.RightShoulder:
                RightShoulder.transform.position = position / 1000.0f;
                RightShoulder.transform.rotation = quaternion;
                break;
            case JointType.RightElbow:
                RightElbow.transform.position = position / 1000.0f;
                RightElbow.transform.rotation = quaternion;
                break;
            case JointType.RightWrist:
                RightWrist.transform.position = position / 1000.0f;
                RightWrist.transform.rotation = quaternion;
                break;
            case JointType.RightHand:
                RightHand.transform.position = position / 1000.0f;
                RightHand.transform.rotation = quaternion;
                break;
            case JointType.MiddleSpine:
                MiddleSpine.transform.position = position / 1000.0f;
                MiddleSpine.transform.rotation = quaternion;
                break;
            case JointType.BaseSpine:
                BaseSpine.transform.position = position / 1000.0f;
                BaseSpine.transform.rotation = quaternion;
                break;
            case JointType.LeftHip:
                LeftHip.transform.position = position / 1000.0f;
                LeftHip.transform.rotation = quaternion;
                break;
            case JointType.LeftKnee:
                LeftKnee.transform.position = position / 1000.0f;
                LeftKnee.transform.rotation = quaternion;
                break;
            case JointType.LeftAnkle:
                LeftAnkle.transform.position = position / 1000.0f;
                LeftAnkle.transform.rotation = quaternion;
                break;
            case JointType.LeftFoot:
                LeftFoot.transform.position = position / 1000.0f;
                LeftFoot.transform.rotation = quaternion;
                break;
            case JointType.RightHip:
                RightHip.transform.position = position / 1000.0f;
                RightHip.transform.rotation = quaternion;
                break;
            case JointType.RightKnee:
                RightKnee.transform.position = position / 1000.0f;
                RightKnee.transform.rotation = quaternion;
                break;
            case JointType.RightAnkle:
                RightAnkle.transform.position = position / 1000.0f;
                RightAnkle.transform.rotation = quaternion;
                break;
            case JointType.RightFoot:
                RightFoot.transform.position = position / 1000.0f;
                RightFoot.transform.rotation = quaternion;
                break;
        }
    }

    private void SetActive(JointType type, bool active)
    {
        switch (type)
        {
            case JointType.Head:
                Head.SetActive(active);
                break;
            case JointType.Neck:
                Neck.SetActive(active);
                break;
            case JointType.ShoulderSpine:
                ShoulderSpine.SetActive(active);
                break;
            case JointType.LeftShoulder:
                LeftShoulder.SetActive(active);
                break;
            case JointType.LeftElbow:
                LeftElbow.SetActive(active);
                break;
            case JointType.LeftWrist:
                LeftWrist.SetActive(active);
                break;
            case JointType.LeftHand:
                LeftHand.SetActive(active);
                break;
            case JointType.RightShoulder:
                RightShoulder.SetActive(active);
                break;
            case JointType.RightElbow:
                RightElbow.SetActive(active);
                break;
            case JointType.RightWrist:
                RightWrist.SetActive(active);
                break;
            case JointType.RightHand:
                RightHand.SetActive(active);
                break;
            case JointType.MiddleSpine:
                MiddleSpine.SetActive(active);
                break;
            case JointType.BaseSpine:
                BaseSpine.SetActive(active);
                break;
            case JointType.LeftHip:
                LeftHip.SetActive(active);
                break;
            case JointType.LeftKnee:
                LeftKnee.SetActive(active);
                break;
            case JointType.LeftAnkle:
                LeftAnkle.SetActive(active);
                break;
            case JointType.LeftFoot:
                LeftFoot.SetActive(active);
                break;
            case JointType.RightHip:
                RightHip.SetActive(active);
                break;
            case JointType.RightKnee:
                RightKnee.SetActive(active);
                break;
            case JointType.RightAnkle:
                RightAnkle.SetActive(active);
                break;
            case JointType.RightFoot:
                RightFoot.SetActive(active);
                break;
        }
    }
}
