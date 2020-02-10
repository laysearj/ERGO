using System.Collections.Generic;
using UnityEngine;
using Voxar;

public class BodyRenderer : MonoBehaviour, IReceiver<BodyJoints[]>
{
    public GameObject JointPrefab;

    private Dictionary<int, Dictionary<JointType, GameObject>> bodySkeletons;

    private Dictionary<int, int> BadNeck;
    private Dictionary<int, int> BadClavicule;
    private Dictionary<int, int> BadNeck2;

    private Dictionary<int, float> TimeBadNeck;
    private Dictionary<int, float> TimeBadClavicule;
    private Dictionary<int, float> TimeBadNeck2;

    // private BodyAngles targetBody;
    private BodyAngles targetNeck_Frontal;
    private BodyAngles targetClavicule;

    public int count1 = 0;
    public int count2 = 0;
    public int count3 = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.bodySkeletons = new Dictionary<int, Dictionary<JointType, GameObject>>();
        this.BadNeck = new Dictionary<int, int>();
        this.BadClavicule = new Dictionary<int, int>();
        this.BadNeck2 = new Dictionary<int, int>();
        this.TimeBadNeck = new Dictionary<int, float>();
        this.TimeBadClavicule = new Dictionary<int, float>();
        this.TimeBadNeck2 = new Dictionary<int, float>();

        targetNeck_Frontal = new BodyAngles();
        targetClavicule = new BodyAngles();

        targetNeck_Frontal.FrontalAngles.AddAngle(BoneType.LowerNeck, BoneType.UpperNeck, 0); //ok
        targetClavicule.HorizontalAngles.AddAngle(BoneType.RightClavicule, BoneType.LeftClavicule, 150); //ok
    }

    void timeNeck(int id)
    {
        TimeBadNeck[id] += Time.deltaTime;

        if (TimeBadNeck[id] >= 100) //Apenas teste rapido para entender nossa tecnologia
        {
          //print("Neck");
            TimeBadNeck[id] = 0;
            BadNeck[id]++;
        }

    }


    void timeClavicule(int id)
    {
        TimeBadClavicule[id] += Time.deltaTime;

        if (TimeBadClavicule[id] >= 100) //Apenas teste rapido para entender nossa tecnologia
        {
          //print("Clavicula errada");
            TimeBadClavicule[id] = 0;
            BadClavicule[id]++;
        }

    }


    //ReceiveData is called one per frame
    public void ReceiveData(BodyJoints[] data)
    {

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }


            if (body.status == Status.NotTracking)
            {
                continue;
            }


            Dictionary<JointType, GameObject> joints;

            if (!this.bodySkeletons.ContainsKey(body.id))
            {
                joints = new Dictionary<JointType, GameObject>();

                for (int i = 0; i < Voxar.Joint.JointTypeCount; i++)
                {
                    var go = (GameObject)Instantiate(JointPrefab, Vector3.zero, Quaternion.identity);
                    go.transform.SetParent(transform);
                    go.SetActive(false);
                    joints.Add((JointType)i, go);
                }

                this.bodySkeletons.Add(body.id, joints);
            }
            else
            {
                joints = this.bodySkeletons[body.id];
            }


           //print("o body id é " + body.id);

            var postureNeck = new BodyAngles(body);
            var postureClavicule = new BodyAngles(body);

            //Teste de angulos para melhor atender nosso problema.
            /*var angle = MovementAnalyzer.CalculateAngleForBones(body, BoneType.UpperNeck, BoneType.LowerNeck);
            print(angle[BasePlanes.Horizontal] + "  angulo");
            */

            var errorsNeck = MovementAnalyzer.CompareBodyAngles(postureNeck, targetNeck_Frontal, 10); //ok
            var errorsClavicule = MovementAnalyzer.CompareBodyAngles(postureClavicule, targetClavicule, 10); //ok


            if (!this.BadNeck.ContainsKey(body.id))
            {
                this.BadNeck.Add(body.id, 0);
                this.BadClavicule.Add(body.id, 0);
                this.BadNeck2.Add(body.id, 0);
                this.TimeBadNeck.Add(body.id, 0);
                this.TimeBadClavicule.Add(body.id, 0);
                this.TimeBadNeck2.Add(body.id, 0);
            }
            else
            {

                if (errorsNeck.Count == 0)
                {
                  //print("zerou");
                    TimeBadNeck[body.id] = 0;
                }
                else
                {
                    this.timeNeck(body.id);
                    foreach (var error in errorsNeck)
                    {
                        print("Your neck is in the wrong position!"); //entrou Neck;
                      //print("Teve um erro entre " + error.boneTypes[0] + " e " + error.boneTypes[1] + " no plano " + error.plane); //teste de plano
                    }
                }


                if (errorsClavicule.Count == 0)
                {
                  //print("Posicao correta."); //teste
                    TimeBadClavicule[body.id] = 0;
                }
                else
                {
                    this.timeClavicule(body.id);
                    foreach (var error in errorsClavicule)
                    {
                        print("Your Clavicule is in the wrong position!"); //entrou Clavicule
                      //print("Teve um erro entre " + error.boneTypes[0] + " e " + error.boneTypes[1] + " no plano " + error.plane); //teste de plano
                    }
                }

            }

            //parts that are not interesting for our problems
            foreach (KeyValuePair<JointType, Voxar.Joint> entry in body.joints)
            {

                if (entry.Key == JointType.LeftKnee)
                {
                    continue;
                }

                if (entry.Key == JointType.LeftAnkle)
                {
                    continue;
                }

                if (entry.Key == JointType.LeftFoot)
                {
                    continue;
                }

                if (entry.Key == JointType.RightKnee)
                {
                    continue;
                }

                if (entry.Key == JointType.RightAnkle)
                {
                    continue;
                }


                if (entry.Key == JointType.RightFoot)
                {
                    continue;
                }

                var bodyJoint = entry.Value;
                var skeletonJoint = joints[entry.Key];

                if (bodyJoint.status != Status.NotTracking)
                {
                    if (!skeletonJoint.activeSelf)
                    {
                        skeletonJoint.SetActive(true);
                    }

                    skeletonJoint.transform.localPosition =
                        new Vector3(bodyJoint.worldPosition.x / 1000f,
                                    bodyJoint.worldPosition.y / 1000f,
                                    bodyJoint.worldPosition.z / 1000f);

                    skeletonJoint.transform.rotation = Quaternion.LookRotation(bodyJoint.forward, bodyJoint.upwards);
                }
                else
                {
                    if (skeletonJoint.activeSelf) skeletonJoint.SetActive(false);
                }
            }
        }
    }
}