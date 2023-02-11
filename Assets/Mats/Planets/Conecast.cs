using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class ConeCast : MonoBehaviour
{
    // The radius of the cone
    public float radius = 1.0f;

    // The angle of the cone (in degrees)
    public float angle = 45.0f;

    // The distance of the cone
    public float distance = 10.0f;

    // The layer mask to use for the cone cast
    public LayerMask layerMask;

    void Update()
    {
        // Calculate the direction of the cone
        Vector3 direction = transform.forward;

        // Perform the cone cast
        //RaycastHit[] hits = ConeCastAll(transform.position, radius, direction, angle, distance, layerMask);

        // Iterate through the hits
        /*foreach (RaycastHit hit in hits)
        {
            // Do something with the hit
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
        }*/
    }
}
