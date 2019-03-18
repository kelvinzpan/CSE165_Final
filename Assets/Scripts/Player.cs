using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerCameraContainer;
    public LineRenderer raycastLine;

    private Vector3 initialCameraForward;
    private Vector3 raycastOffset = new Vector3(2.0f, -3.0f, 0.0f);
    private float raycastLength = 100.0f;

    const string LAYER_CASTLE = "Castle";

    // Start is called before the first frame update
    void Start()
    {
        initialCameraForward = playerCameraContainer.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        drawAndFireRaycast();
    }

    void drawAndFireRaycast()
    {
        Vector3 playerLocation = playerCameraContainer.transform.position;
        Vector3 playerLookAt = (playerCameraContainer.transform.rotation * initialCameraForward).normalized;

        raycastLine.SetPosition(0, playerLocation + raycastOffset);
        raycastLine.SetPosition(1, playerLocation + playerLookAt * raycastLength);

        RaycastHit hit;
        Ray ray = new Ray(playerLocation + raycastOffset, playerLookAt);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject currObj = hit.transform.gameObject;

            if (currObj.layer == LayerMask.NameToLayer(LAYER_CASTLE))
            {

            }
        }
    }
}
