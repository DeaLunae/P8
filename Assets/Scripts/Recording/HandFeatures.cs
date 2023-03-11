using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class HandFeatures
{
    public const string PIPThumb = "PIPThumb";
    public const string PIPIndex = "PIPIndex";
    public const string PIPMiddle = "PIPMiddle";
    public const string PIPRing = "PIPRing";
    public const string PIPPinky = "PIPPinky";
    public const string KnuckleThumb = "KnuckleThumb";
    public const string KnuckleIndex = "KnuckleIndex";
    public const string KnuckleMiddle = "KnuckleMiddle";
    public const string KnuckleRing = "KnuckleRing";
    public const string KnucklePinky = "KnucklePinky";
    public const string AbductionThumb = "AbductionThumb";
    public const string AbductionIndex = "AbductionIndex";
    public const string AbductionMiddle = "AbductionMiddle";
    public const string AbductionRing = "AbductionRing";
    public const string AbductionPinky = "AbductionPinky";
    public const string DirectionUp = "DirectionUp";
    public const string DirectionForward = "DirectionForward";
    public const string DirectionRight = "DirectionRight";
    
    private Dictionary<string, float> features = new()
    {
        {PIPThumb,0 },
        {PIPIndex,0 },
        {PIPMiddle,0 },
        {PIPRing,0},
        {PIPPinky,0} ,
        {KnuckleThumb ,0},
        {KnuckleIndex,0 },
        {KnuckleMiddle,0 },
        {KnuckleRing ,0 },
        {KnucklePinky,0 },
        {AbductionThumb,0},
        {AbductionIndex,0},
        {AbductionMiddle,0},
        {AbductionRing,0 },
        {AbductionPinky,0},
        {DirectionUp,0 },
        {DirectionForward,0},
        {DirectionRight,0}
    };
    public static void CalculateJointFeatures(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses,Transform head, ref Dictionary<string, float> features)
    {
        features[PIPThumb] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.ThumbTip].Position);
        features[PIPIndex] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.IndexTip].Position);
        features[PIPMiddle] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.MiddleTip].Position);
        features[PIPRing] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.RingTip].Position);
        features[PIPPinky] = Vector3.Distance(jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.PinkyTip].Position);

        features[AbductionThumb] = Vector3.Distance(jointPoses[TrackedHandJoint.ThumbTip].Position, jointPoses[TrackedHandJoint.IndexTip].Position);
        features[AbductionIndex] = Vector3.Distance(jointPoses[TrackedHandJoint.IndexTip].Position, jointPoses[TrackedHandJoint.MiddleTip].Position);
        features[AbductionMiddle] = Vector3.Distance(jointPoses[TrackedHandJoint.MiddleTip].Position, jointPoses[TrackedHandJoint.RingTip].Position);
        features[AbductionRing] = Vector3.Distance(jointPoses[TrackedHandJoint.RingTip].Position, jointPoses[TrackedHandJoint.PinkyTip].Position);
        features[AbductionPinky] = Vector3.Distance(jointPoses[TrackedHandJoint.PinkyTip].Position, jointPoses[TrackedHandJoint.ThumbTip].Position);

        features[KnuckleThumb] = Vector3.Distance(jointPoses[TrackedHandJoint.ThumbProximalJoint].Position, jointPoses[TrackedHandJoint.PinkyKnuckle].Position);
        features[KnuckleIndex] = Vector3.Distance(jointPoses[TrackedHandJoint.IndexMiddleJoint].Position, jointPoses[TrackedHandJoint.Palm].Position);
        features[KnuckleMiddle] = Vector3.Distance(jointPoses[TrackedHandJoint.MiddleMiddleJoint].Position, jointPoses[TrackedHandJoint.Palm].Position);
        features[KnuckleRing] = Vector3.Distance(jointPoses[TrackedHandJoint.RingMiddleJoint].Position, jointPoses[TrackedHandJoint.Palm].Position);
        features[KnucklePinky] = Vector3.Distance(jointPoses[TrackedHandJoint.PinkyMiddleJoint].Position, jointPoses[TrackedHandJoint.Palm].Position);
        
        features[DirectionForward] = Vector3.Dot(head.position - jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.Palm].Forward);
        features[DirectionUp] = Vector3.Dot(jointPoses[TrackedHandJoint.Palm].Up, head.up);
        features[DirectionRight] = Vector3.Dot(head.position - jointPoses[TrackedHandJoint.Palm].Position, jointPoses[TrackedHandJoint.Palm].Right);
    }
}