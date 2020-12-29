using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HumanBodyTracking : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce frame events.")]
    private ARHumanBodyManager humanBodyManager;

    [SerializeField] private GameObject jointPrefab;

    [SerializeField] private GameObject lineRendererPrefab;

    [SerializeField] private GameObject bodyHitBoxGM;
    [SerializeField] private GameObject headHitBoxGM;
    [SerializeField] private BarScript healthBarGM;



    [SerializeField] private bool bodyOnly;

    private Dictionary<JointIndices3D, Transform> bodyJoints;

    private LineRenderer[] lineRenderers;
    private Transform[][] lineRendererTransforms;
    private Transform bodyHitBox;
    private Transform headHitBox;
    private Transform bar;
    


    private const float jointScaleModifier = .4f;

    void OnEnable()
    {
        Debug.Assert(humanBodyManager != null, "Human body manager is required");
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        if (humanBodyManager != null)
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    private void InitialiseObjects(Transform arBodyT)
    {
        if (bodyJoints == null)
        {
            if (!bodyOnly) {
                bodyJoints = new Dictionary<JointIndices3D, Transform>
                {
                    { JointIndices3D.Head, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.Neck1, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftArm, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightArm, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftForearm, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightForearm, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftHand, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightHand, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftUpLeg, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightUpLeg, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftLeg, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightLeg, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftFoot, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightFoot, GetNewJointPrefab(arBodyT) }
                };
                lineRenderers = new LineRenderer[]
                {
                    Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // head neck
                    Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // upper
                    Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // lower
                    Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // right
                    Instantiate(lineRendererPrefab).GetComponent<LineRenderer>() // left
                };
                lineRendererTransforms = new Transform[][]
                {
                    new Transform[] { bodyJoints[JointIndices3D.Head], bodyJoints[JointIndices3D.Neck1] },
                    new Transform[] { bodyJoints[JointIndices3D.RightHand], bodyJoints[JointIndices3D.RightForearm], bodyJoints[JointIndices3D.RightArm], bodyJoints[JointIndices3D.Neck1], bodyJoints[JointIndices3D.LeftArm], bodyJoints[JointIndices3D.LeftForearm], bodyJoints[JointIndices3D.LeftHand]},
                    new Transform[] { bodyJoints[JointIndices3D.RightFoot], bodyJoints[JointIndices3D.RightLeg], bodyJoints[JointIndices3D.RightUpLeg], bodyJoints[JointIndices3D.LeftUpLeg], bodyJoints[JointIndices3D.LeftLeg], bodyJoints[JointIndices3D.LeftFoot] },
                    new Transform[] { bodyJoints[JointIndices3D.RightArm], bodyJoints[JointIndices3D.RightUpLeg] },
                    new Transform[] { bodyJoints[JointIndices3D.LeftArm], bodyJoints[JointIndices3D.LeftUpLeg] }
                };
            } else {
                bodyJoints = new Dictionary<JointIndices3D, Transform>
                {
                    { JointIndices3D.Head, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.Neck1, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftArm, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightArm, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.LeftUpLeg, GetNewJointPrefab(arBodyT) },
                    { JointIndices3D.RightUpLeg, GetNewJointPrefab(arBodyT) },
                };
                lineRenderers = new LineRenderer[]
                {
                    Instantiate(lineRendererPrefab).GetComponent<LineRenderer>(), // head neck
                    Instantiate(lineRendererPrefab).GetComponent<LineRenderer>() // upper
                };
                lineRendererTransforms = new Transform[][]
                {
                    new Transform[] { bodyJoints[JointIndices3D.Head], bodyJoints[JointIndices3D.Neck1] },
                    new Transform[] { bodyJoints[JointIndices3D.RightArm], bodyJoints[JointIndices3D.Neck1], bodyJoints[JointIndices3D.LeftArm], bodyJoints[JointIndices3D.LeftUpLeg], bodyJoints[JointIndices3D.RightUpLeg], bodyJoints[JointIndices3D.RightArm] }
                };

                if (bodyHitBoxGM) {
                    bodyHitBox = Instantiate(bodyHitBoxGM).GetComponent<Transform>();
                }
                if (headHitBoxGM) {
                    headHitBox = Instantiate(headHitBoxGM).GetComponent<Transform>();
                }
                if (headHitBoxGM) {
                    bar = Instantiate(healthBarGM.transform).GetComponent<Transform>();
                    headHitBox.GetComponent<HitBox>().bar = bar.GetComponent<BarScript>();
                    bodyHitBox.GetComponent<HitBox>().bar = bar.GetComponent<BarScript>();
                }

            }


            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].positionCount = lineRendererTransforms[i].Length;
            }
        }
    }

    private Transform GetNewJointPrefab(Transform arBodyT)
    {
        return Instantiate(jointPrefab, arBodyT).transform;
    }

    void UpdateBody(ARHumanBody arBody)
    {
        Transform arBodyT = arBody.transform;

        if (arBodyT == null)
        {
            Debug.Log("No root transform found for ARHumanBody");
            return;
        }

        InitialiseObjects(arBodyT);

        /// Update joint placement
        NativeArray<XRHumanBodyJoint> joints = arBody.joints;
        if(!joints.IsCreated) return;

        /// Update placement of all joints
        foreach (KeyValuePair<JointIndices3D, Transform> item in bodyJoints)
        {
            UpdateJointTransform(item.Value, joints[(int)item.Key]);
        }

        if (bodyHitBox) {
            UpdateHitBoxTransform(bodyHitBox, CalculateBodyHitBoxPosition(), 
                joints[(int)JointIndices3D.Neck1].anchorPose.rotation, joints[(int)JointIndices3D.Neck1].anchorScale);
        }

        if (headHitBox) {
            UpdateHitBoxTransform(headHitBox, CalculateHeadHitBoxPosition(), 
                joints[(int)JointIndices3D.Neck1].anchorPose.rotation, joints[(int)JointIndices3D.Head].anchorScale);
        }

        if (bar) {
            bar.position = CalculateBarPosition();
        }

        /// Update all line renderers.
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i].SetPositions(lineRendererTransforms[i]);
        }
    }

    private Vector3 CalculateBarPosition() {
        var pose = bodyJoints[JointIndices3D.Head].position;
        pose.y += 0.7f;

        return pose;
    }

    private Vector3 CalculateBodyHitBoxPosition() {
        var middleHandsPoint = CalcBetween(bodyJoints[JointIndices3D.LeftArm].position, bodyJoints[JointIndices3D.RightArm].position, 2);
        var middleLegsPoint = CalcBetween(bodyJoints[JointIndices3D.LeftUpLeg].position, bodyJoints[JointIndices3D.RightUpLeg].position, 2);
        var center = CalcBetween(middleHandsPoint, middleLegsPoint, 2);

        return center;
    }

    private Vector3 CalculateHeadHitBoxPosition() {
        // var center = CalcBetween(bodyJoints[JointIndices3D.Neck1].position, bodyJoints[JointIndices3D.Head].position, 2);
        var center = bodyJoints[JointIndices3D.Head].position;

        return center;
    }

    private Vector3 CalcBetween(Vector3 pos1, Vector3 pos2, float divider) {
        var calcX = (pos1.x + pos2.x) / divider;
        var calcY = (pos1.y + pos2.y) / divider;
        var calcZ = (pos1.z + pos2.z) / divider;

        return new Vector3(calcX, calcY, calcZ);
    }

    private void UpdateHitBoxTransform(Transform hitBox, Vector3 position, Quaternion rotation, Vector3 scale) {
        hitBox.localPosition = position;
        // hitBox.localRotation = rotation;
        hitBox.localScale = scale * jointScaleModifier;
    }

    private void UpdateJointTransform(Transform jointT, XRHumanBodyJoint bodyJoint)
    {
        jointT.localPosition = bodyJoint.anchorPose.position;
        jointT.localRotation = bodyJoint.anchorPose.rotation;
        jointT.localScale = bodyJoint.anchorScale * jointScaleModifier;
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        foreach (ARHumanBody humanBody in eventArgs.added)
        {
            UpdateBody(humanBody);
        }

        foreach (ARHumanBody humanBody in eventArgs.updated)
        {
            UpdateBody(humanBody);
        }

        //Debug.Log($"Created {eventArgs.added.Count}, updated {eventArgs.updated.Count}, removed {eventArgs.removed.Count}");
    }
}