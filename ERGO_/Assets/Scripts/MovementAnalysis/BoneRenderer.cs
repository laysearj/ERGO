using UnityEngine;
using System.Collections.Generic;
using Voxar;

public class BoneRenderer : MonoBehaviour, IReceiver<Voxar.BodyJoints[]>
{
    private Dictionary<BoneType, Bone> bones;
    private Dictionary<BoneType, GameObject> bonesGO;

    void Start()
    {
        bones = new Dictionary<BoneType, Bone>();
        bonesGO = new Dictionary<BoneType, GameObject>();
    }

    public void ReceiveData(BodyJoints[] data)
    {
        foreach (var body in data)
        {
            if (body.status != Status.Tracking)
            {
                continue;
            }

            bones = MovementAnalyzer.ExtractBonesFromBodyJoints(body);
            RenderBones();
        }
    }

    private void RenderBones()
    {
        foreach (var entry in bones)
        {
            GameObject cylinder;

            if (!bonesGO.ContainsKey(entry.Key))
            {
                bonesGO[entry.Key] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            }

            cylinder = bonesGO[entry.Key];

            //cylinder pivot point is in its center, so i have to adjust the bone position to compensate this with "+ (bones[i].worldVector / 2.0f)"
            cylinder.transform.position = (bones[entry.Key].worldPoint + (bones[entry.Key].worldVector / 2.0f)) / 1000.0f;
            cylinder.transform.rotation = Quaternion.LookRotation(Vector3.back, bones[entry.Key].worldVector.normalized);
            cylinder.transform.localScale = new Vector3(0.05f, 0.1f, 0.05f);
            cylinder.transform.parent = transform;
        }
    }
}
