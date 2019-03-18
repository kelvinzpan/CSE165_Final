using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAndKeyboardCamera : MonoBehaviour
{
    public GameObject cameraContainer;
    public GameObject floor;
    public float moveSpeed;
    public float mouseXSens;
    public float mouseYSens;
    public float playerHeight;

    private float mouseRotY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // WASD movement
        if (Input.GetKey(KeyCode.W)) cameraContainer.transform.position += cameraContainer.transform.forward * moveSpeed;
        if (Input.GetKey(KeyCode.S)) cameraContainer.transform.position -= cameraContainer.transform.forward * moveSpeed;
        if (Input.GetKey(KeyCode.A)) cameraContainer.transform.position -= cameraContainer.transform.right * moveSpeed;
        if (Input.GetKey(KeyCode.D)) cameraContainer.transform.position += cameraContainer.transform.right * moveSpeed;

        float floorY = floor.transform.position.y + playerHeight;
        if (cameraContainer.transform.position.y < floorY)
        {
            cameraContainer.transform.position = new Vector3(cameraContainer.transform.position.x, floorY, cameraContainer.transform.position.z);
        }

        // Mouse orientation
        float mouseRotX = cameraContainer.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseXSens;
        mouseRotY += Input.GetAxis("Mouse Y") * mouseYSens;
        mouseRotY = Mathf.Clamp(mouseRotY, -90.0f, 90.0f);
        cameraContainer.transform.localEulerAngles = new Vector3(-mouseRotY, mouseRotX, 0);
    }
}
