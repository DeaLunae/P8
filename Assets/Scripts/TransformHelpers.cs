
using System;
using UnityEngine;

public static class TransformHelpers
{
    /// <summary>
    /// Scales the target around an arbitrary point by scaleFactor.
    /// This is relative scaling, meaning using  scale Factor of Vector3.one
    /// will not change anything and new Vector3(0.5f,0.5f,0.5f) will reduce
    /// the object size by half.
    /// The pivot is assumed to be the position in the space of the target.
    /// Scaling is applied to localScale of target.
    /// </summary>
    /// <param name="target">The object to scale.</param>
    /// <param name="pivot">The point to scale around in space of target.</param>
    /// <param name="scaleFactor">The factor with which the current localScale of the target will be multiplied with.</param>
    public static void ScaleAroundRelative(this Transform target, Vector3 pivot, Vector3 scaleFactor)
    {
        // pivot
        var pivotDelta = target.localPosition - pivot;
        pivotDelta.Scale(scaleFactor);
        target.transform.localPosition = pivot + pivotDelta;
 
        // scale
        var finalScale = target.localScale;
        finalScale.Scale(scaleFactor);
        target.transform.localScale = finalScale;
    }
 
    /// <summary>
    /// Scales the target around an arbitrary pivot.
    /// This is absolute scaling, meaning using for example a scale factor of
    /// Vector3.one will set the localScale of target to x=1, y=1 and z=1.
    /// The pivot is assumed to be the position in the space of the target.
    /// Scaling is applied to localScale of target.
    /// </summary>
    /// <param name="target">The object to scale.</param>
    /// <param name="pivot">The point to scale around in the space of target.</param>
    /// <param name="scaleFactor">The new localScale the target object will have after scaling.</param>
    public static void ScaleAround(this Transform target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 pivotDelta = target.localPosition - pivot; // diff from object pivot to desired pivot/origin
        Vector3 scaleFactor = new Vector3(
            newScale.x / target.localScale.x,
            newScale.y / target.localScale.y,
            newScale.z / target.localScale.z );
        pivotDelta.Scale(scaleFactor);
        target.transform.localPosition = pivot + pivotDelta;
        target.localScale = newScale;
    }
    
    /// <summary>
    /// Scales the target around an arbitrary pivot.
    /// This is absolute scaling, meaning using for example a scale factor of
    /// Vector3.one will set the localScale of target to x=1, y=1 and z=1.
    /// The pivot is assumed to be the position in the space of the target.
    /// Scaling is applied to localScale of target.
    /// </summary>
    /// <param name="target">The object to scale.</param>
    /// <param name="pivot">The point to scale around in the space of target.</param>
    /// <param name="scaleFactor">The new localScale the target object will have after scaling.</param>


}
