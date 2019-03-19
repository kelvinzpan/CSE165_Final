using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapBox : MonoBehaviour
{
    bool m_Started;
    public LayerMask m_LayerMask;

    private List<Collider> colliders = new List<Collider>();
    public List<Collider> GetColliders() { return colliders; }

    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;
    }

    void FixedUpdate()
    {
        MyCollisions();
    }

    void MyCollisions()
    {
        colliders.Clear();
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
        int i = 0;
        
        while (i < hitColliders.Length)
        {
            colliders.Add(hitColliders[i]);
            // Debug.Log("Hit : " + hitColliders[i].name + i);
            i++;
        }
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
