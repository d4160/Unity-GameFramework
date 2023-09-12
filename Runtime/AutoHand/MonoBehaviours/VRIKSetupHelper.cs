using Autohand;
using Autohand.Demo;
using NaughtyAttributes;
using RootMotion.FinalIK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VRIKSetupHelper : MonoBehaviour
{
    public GameObject copyFrom;
    public bool isOpenXR;
    public bool useBoxCollidersForFingers;
    [Range(0.1f, 1.5f)]
    public float collidersScaleMultiplier = 1f;
    public string[] ignoreFingerTransforms = new string[] { "jointItemR", "jointItemL" };

    private Animator _thisAnim;
    private VRIK _thisVRIK;
    private AutoHandVRIK _thisAutoHandVRIK;
    private Rigidbody _thisLeftRb;
    private Rigidbody _thisRightRb;
    private Hand _thisLeftHand;
    private Hand _thisRightHand;
    private XRHandControllerLink _thisLeftHandControllerLink;
    private XRHandControllerLink _thisRightHandControllerLink;

    public float ScaleMultiplier => 1 / transform.localScale.x * collidersScaleMultiplier;

    [Button]
    public void CopyFrom()
    {
        _thisAnim = GetOrAddComponent<Animator>();
        _thisVRIK = GetOrAddComponent<VRIK>();
        _thisAutoHandVRIK = GetOrAddComponent<AutoHandVRIK>();
        var _copyFromVRIK= GetOrAddComponent<VRIK>(copyFrom);
        var _copyFromAutoHandVRIK = GetOrAddComponent<AutoHandVRIK>(copyFrom);
        var _copyFromAnim = GetOrAddComponent<Animator>(copyFrom);

        _thisAnim.runtimeAnimatorController = _copyFromAnim.runtimeAnimatorController;
        _thisAnim.updateMode = _copyFromAnim.updateMode;
        _thisAnim.applyRootMotion = _copyFromAnim.applyRootMotion;
        _thisAnim.cullingMode = _copyFromAnim.cullingMode;

        _thisVRIK.solver = _copyFromVRIK.solver;

        _thisAutoHandVRIK.rightHand = _copyFromAutoHandVRIK.rightHand;
        _thisAutoHandVRIK.leftHand = _copyFromAutoHandVRIK.leftHand;
        _thisAutoHandVRIK.leftTrackedController = _copyFromAutoHandVRIK.leftTrackedController;
        _thisAutoHandVRIK.rightTrackedController = _copyFromAutoHandVRIK.rightTrackedController;

        CopyFromRigidBody(_thisVRIK.references.leftHand.gameObject, _copyFromVRIK.references.leftHand.gameObject, ref _thisLeftRb);
        CopyFromRigidBody(_thisVRIK.references.rightHand.gameObject, _copyFromVRIK.references.rightHand.gameObject, ref _thisRightRb);

        CopyFromHand(_thisVRIK.references.leftHand.gameObject, _copyFromVRIK.references.leftHand.gameObject, ref _thisLeftHand, true);
        CopyFromHand(_thisVRIK.references.rightHand.gameObject, _copyFromVRIK.references.rightHand.gameObject, ref _thisRightHand, false);

        CopyFromHandControllerLink(_thisVRIK.references.leftHand.gameObject, _copyFromVRIK.references.leftHand.gameObject, ref _thisLeftHandControllerLink);
        CopyFromHandControllerLink(_thisVRIK.references.rightHand.gameObject, _copyFromVRIK.references.rightHand.gameObject, ref _thisRightHandControllerLink);

        var autoHandPlayer = FindObjectOfType<AutoHandPlayer>();
        autoHandPlayer.handRight = _thisRightHand;
        autoHandPlayer.handLeft = _thisLeftHand;

        if (!isOpenXR)
        {
            var handPlayerControllerLink = FindObjectOfType<XRHandPlayerControllerLink>();
            handPlayerControllerLink.moveController = _thisLeftHandControllerLink;
            handPlayerControllerLink.turnController = _thisRightHandControllerLink;
        }

        _thisAutoHandVRIK.rightHand = _thisRightHand;
        _thisAutoHandVRIK.leftHand = _thisLeftHand;
    }

    [Button]
    public void CopyFromRightToLeft()
    {
        if (!_thisVRIK) _thisVRIK = GetComponent<VRIK>();

        Transform palmLT = GetOrAddTransform(_thisVRIK.references.leftHand.gameObject, "Palm TransformL");
        Transform palmRT = GetOrAddTransform(_thisVRIK.references.rightHand.gameObject, "Palm TransformR", true);
        CopyTransformFrom(palmLT, palmRT);

        Transform palmLColliderT = GetOrAddTransform(_thisVRIK.references.leftHand.gameObject, "PalmColliderL");
        Transform palmRColliderT = GetOrAddTransform(_thisVRIK.references.rightHand.gameObject, "PalmColliderR", true);
        CopyTransformFrom(palmLColliderT, palmRColliderT);

        BoxCollider palmLBoxColl = GetOrAddComponent<BoxCollider>(palmLColliderT.gameObject);
        BoxCollider palmRBoxColl = GetOrAddComponent<BoxCollider>(palmRColliderT.gameObject);
        palmLBoxColl.isTrigger = palmRBoxColl.isTrigger;
        palmLBoxColl.material = palmRBoxColl.material;
        palmLBoxColl.center = palmRBoxColl.center;
        palmLBoxColl.size = palmRBoxColl.size;

        Hand handL = GetOrAddComponent<Hand>(_thisVRIK.references.leftHand.gameObject);
        Hand handR = GetOrAddComponent<Hand>(_thisVRIK.references.rightHand.gameObject);
        handL.reachDistance = handR.reachDistance;

        for (int i = 0; i < handR.fingers.Length; i++)
        {
            handL.fingers[i].tipRadius = handR.fingers[i].tipRadius;
            CopyTransformFrom(handL.fingers[i].tip, handR.fingers[i].tip);
            SphereCollider tipLColl = GetOrAddComponent<SphereCollider>(handL.fingers[i].tip.gameObject);
            SphereCollider tipRColl = GetOrAddComponent<SphereCollider>(handR.fingers[i].tip.gameObject);
            tipLColl.radius = tipRColl.radius;

            SetFingerBoneColliderRecursive(handL.fingers[i].transform, handR.fingers[i].transform, false, $"Tip {i} L");
        }
    }

    private void CopyTransformFrom(Transform toCopy, Transform fromCopy)
    {
        toCopy.localPosition = fromCopy.localPosition;
        toCopy.localRotation = fromCopy.localRotation;
        toCopy.localScale = fromCopy.localScale;
    }

    private void CopyFromHandControllerLink(GameObject thisObj, GameObject copyFromObj, ref XRHandControllerLink thisHandConLink)
    {
        thisHandConLink = GetOrAddComponent<XRHandControllerLink>(thisObj);
        var copyFromHandConLink = GetOrAddComponent<XRHandControllerLink>(copyFromObj);
        thisHandConLink.hand = _thisLeftHand;
        thisHandConLink.grabButton = copyFromHandConLink.grabButton;
        thisHandConLink.grabAxis = copyFromHandConLink.grabAxis;
        thisHandConLink.squeezeAxis = copyFromHandConLink.squeezeAxis;
        thisHandConLink.squeezeButton = copyFromHandConLink.squeezeButton;

        var thisHandAxisFinger = GetOrAddComponent<XRAutoHandAxisFingerBender>(thisObj);
        var copyFromHandAxisFinger = GetOrAddComponent<XRAutoHandAxisFingerBender>(copyFromObj);
        thisHandAxisFinger.controller = thisHandConLink;
        thisHandAxisFinger.axis = copyFromHandAxisFinger.axis;
    }

    private void CopyFromRigidBody(GameObject thisObj, GameObject copyFromObj, ref Rigidbody thisRb)
    {
        thisRb = GetOrAddComponent<Rigidbody>(thisObj);
        var copyFromRb = GetOrAddComponent<Rigidbody>(copyFromObj);

        thisRb.mass = copyFromRb.mass;
        thisRb.drag = copyFromRb.drag;
        thisRb.angularDrag = copyFromRb.angularDrag;
        thisRb.useGravity = copyFromRb.useGravity;
        thisRb.isKinematic = copyFromRb.isKinematic;
        thisRb.interpolation = copyFromRb.interpolation;
        thisRb.collisionDetectionMode = copyFromRb.collisionDetectionMode;
        thisRb.detectCollisions = copyFromRb.detectCollisions;
    }

    private void CopyFromHand(GameObject thisObj, GameObject copyFromObj, ref Hand thisHand, bool isLeft)
    {
        thisHand = GetOrAddComponent<Hand>(thisObj);
        var copyFromHand = GetOrAddComponent<Hand>(copyFromObj);

        Transform thisPalm = GetOrAddTransform(thisObj, isLeft ? "Palm TransformL" : "Palm TransformR");
        Transform copyFromPalm = GetOrAddTransform(copyFromObj, isLeft ? "Palm TransformL" : "Palm TransformR");
        thisPalm.localPosition = copyFromPalm.localPosition;
        thisPalm.localRotation = copyFromPalm.localRotation;

        Transform thisHandFollow = GetOrAddTransform(thisObj, isLeft ? "HandFollowL" : "HandFollowR");
        Transform copyFromHandFollow = GetOrAddTransform(copyFromObj, isLeft ? "HandFollowL" : "HandFollowR"); ;
        thisHandFollow.localPosition = copyFromHandFollow.localPosition;
        thisHandFollow.localRotation = copyFromHandFollow.localRotation;
        thisHandFollow.localScale = copyFromHandFollow.localScale;

        Transform thisPalmColliderT = GetOrAddTransform(thisObj, isLeft ? "PalmColliderL" : "PalmColliderR");
        Transform copyFromPalmColliderT = GetOrAddTransform(copyFromObj, isLeft ? "PalmColliderL" : "PalmColliderR"); ;
        thisPalmColliderT.localPosition = copyFromPalmColliderT.localPosition;
        thisPalmColliderT.localRotation = copyFromPalmColliderT.localRotation;

        BoxCollider thispPalmBoxColl = GetOrAddComponent<BoxCollider>(thisPalmColliderT.gameObject);
        BoxCollider copyFromPalmBoxColl = GetOrAddComponent<BoxCollider>(copyFromPalmColliderT.gameObject);
        thispPalmBoxColl.isTrigger = copyFromPalmBoxColl.isTrigger;
        thispPalmBoxColl.material = copyFromPalmBoxColl.material;
        thispPalmBoxColl.center = copyFromPalmBoxColl.center;
        thispPalmBoxColl.size = copyFromPalmBoxColl.size * ScaleMultiplier;

        thisHand.left = isLeft;
        thisHand.palmTransform = thisPalm;
        thisHand.reachDistance = copyFromHand.reachDistance * ScaleMultiplier;
        thisHand.enableMovement = copyFromHand.enableMovement;
        thisHand.follow = copyFromHand.follow;
        thisHand.maxFollowDistance = copyFromHand.maxFollowDistance;
        thisHand.throwPower = copyFromHand.throwPower;
        thisHand.swayStrength = copyFromHand.swayStrength;
        thisHand.gripOffset = copyFromHand.gripOffset;
        thisHand.highlightLayers = copyFromHand.highlightLayers;
        thisHand.defaultHighlight = copyFromHand.defaultHighlight;
        thisHand.grabType = copyFromHand.grabType;
        thisHand.minGrabTime = copyFromHand.minGrabTime;
        thisHand.maxGrabTime = copyFromHand.maxGrabTime;
        thisHand.grabCurve = copyFromHand.grabCurve;
        thisHand.gentleGrabSpeed = copyFromHand.gentleGrabSpeed;
        thisHand.poseIndex = copyFromHand.poseIndex;

        //if (thisHand.fingers == null || thisHand.fingers.Length == 0)
        //{
        var ignoreFingersT = new List<string>(ignoreFingerTransforms);
        ignoreFingersT.AddRange(new string[] { "Palm TransformL", "Palm TransformR", "HandFollowL", "HandFollowR", "PalmColliderL", "PalmColliderR" });
        var thisFingersT = GetChildren(thisObj, ignoreFingersT);
        //Debug.Log($"thisFingersT count: {thisFingersT.Count}");
        var copyFromFingers = copyFromHand.fingers;
        var thisFingers = new Autohand.Finger[thisFingersT.Count];
        for (int i = 0; i < thisFingersT.Count; i++)
        {
            string tipName = $"Tip {i} {(isLeft ? 'L' : 'R')}";

            SetFingerBoneColliderRecursive(thisFingersT[i], copyFromFingers[i].transform, true, tipName);

            var thisFinger = GetOrAddComponent<Autohand.Finger>(thisFingersT[i].gameObject);
            Autohand.Finger copyFromFinger = copyFromFingers[i];
            thisFinger.tipRadius = copyFromFinger.tipRadius;
            thisFinger.bendOffset = copyFromFinger.bendOffset;
            thisFinger.fingerSmoothSpeed = copyFromFinger.fingerSmoothSpeed;

            var deepestFinger = FindDeepestTransform(thisFinger.transform);
            Transform thisTip = deepestFinger;
            Transform copyFromTip = copyFromFinger.tip;
            
            if (deepestFinger.name != tipName)
            {
                thisTip = GetOrAddTransform(deepestFinger.gameObject, tipName);
            }
            thisTip.localPosition = copyFromTip.localPosition;
            thisTip.localRotation = copyFromTip.localRotation;
            thisFinger.tip = thisTip;

            var thisTipSphereColl = GetOrAddComponent<SphereCollider>(thisTip.gameObject);
            thisTipSphereColl.radius = thisFinger.tipRadius * ScaleMultiplier;

            thisFingers[i] = thisFinger;
        }

        thisHand.fingers = thisFingers;
        //}
    }

    private void SetFingerBoneColliderRecursive(Transform thisFinger, Transform copyFromFinger, bool useScaleMultiplier = true, params string[] ignoreTransforms)
    {
        if (ignoreTransforms.Contains(thisFinger.name)) return;

        SetFingerBoneCollider(thisFinger, copyFromFinger, useScaleMultiplier);
        if (thisFinger.childCount > 0 && copyFromFinger.childCount > 1)
        {
            SetFingerBoneColliderRecursive(thisFinger.GetChild(0), copyFromFinger.GetChild(0), useScaleMultiplier, ignoreTransforms);
        }
    }

    private void SetFingerBoneCollider(Transform thisFinger, Transform copyFromFinger, bool useScaleMultiplier = true)
    {
        var copyFromBox = copyFromFinger.GetComponent<BoxCollider>();
        var copyFromCapsule = copyFromFinger.GetComponent<CapsuleCollider>();

        if (useBoxCollidersForFingers)
        {
            var capsule = thisFinger.GetComponent<CapsuleCollider>();
            if (capsule) DestroyImmediate(capsule);

            var thisBox = GetOrAddComponent<BoxCollider>(thisFinger.gameObject);

            if (copyFromBox)
            {
                thisBox.isTrigger = copyFromBox.isTrigger;
                thisBox.material = copyFromBox.material;
                thisBox.center = copyFromBox.center;
                thisBox.size = copyFromBox.size * (useScaleMultiplier ? ScaleMultiplier : 1f);
            }
            else if (copyFromCapsule)
            {
                thisBox.isTrigger = copyFromCapsule.isTrigger;
                thisBox.material = copyFromCapsule.material;
                thisBox.center = copyFromCapsule.center;
                thisBox.size = new Vector3(copyFromCapsule.height, copyFromCapsule.radius, copyFromCapsule.radius) * (useScaleMultiplier ? ScaleMultiplier : 1f);
            }
        }
        else
        {
            var box = thisFinger.GetComponent<BoxCollider>();
            if (box) DestroyImmediate(box);

            var thisCapsule = GetOrAddComponent<CapsuleCollider>(thisFinger.gameObject);

            if (copyFromCapsule)
            {
                thisCapsule.isTrigger = copyFromCapsule.isTrigger;
                thisCapsule.material = copyFromCapsule.material;
                thisCapsule.center = copyFromCapsule.center;
                thisCapsule.height = copyFromCapsule.height * (useScaleMultiplier ? ScaleMultiplier : 1f);
                thisCapsule.radius = copyFromCapsule.radius * (useScaleMultiplier ? ScaleMultiplier : 1f);
            }
            else if (copyFromBox)
            {
                thisCapsule.isTrigger = copyFromBox.isTrigger;
                thisCapsule.material = copyFromBox.material;
                thisCapsule.center = copyFromBox.center;
                thisCapsule.height = copyFromBox.size.x * (useScaleMultiplier ? ScaleMultiplier : 1f);
                thisCapsule.radius = copyFromBox.size.y * (useScaleMultiplier ? ScaleMultiplier : 1f);
            }
        }
    }

    private Transform FindDeepestTransform(Transform t)
    {
        Transform child = t;
        while(child.childCount > 0)
        {
            child = child.GetChild(0);
        }

        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;

        return child;
    }

    private List<Transform> GetChildren(GameObject parent, List<string> ignoreTransforms)
    {
        List<Transform> children = new(parent.transform.childCount);
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (ignoreTransforms.Contains(parent.transform.GetChild(i).name)) continue;
            children.Add(parent.transform.GetChild(i));
        }

        return children;
    }

    private List<Transform> GetChildren(GameObject parent, params string[] ignoreTransforms)
    {
        return GetChildren(parent, ignoreTransforms.ToList());
    }

    private Transform GetOrAddTransform(GameObject parent, string name, bool keepTransform = false)
    {
        Transform t = parent.transform.Find(name);
        if (!t)
        {
            t = new GameObject(name).transform;
            t.parent = parent.transform;
        }

        if (!keepTransform)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        return t;
    }

    private T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp == null)
        {
            comp = obj.AddComponent<T>();
        }

        return comp;
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        T comp = GetComponent<T>();
        if (comp == null)
        {
            comp = gameObject.AddComponent<T>();
        }

        return comp;
    }
}
