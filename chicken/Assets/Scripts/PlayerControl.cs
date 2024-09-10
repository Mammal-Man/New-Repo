using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;

    Vector2 camRot;

    public float MoveSpeed = 10;
    public float sprintMulti 2.5f;
    public float JumpHeight = 5;

    public float mouseSens = 2;
    public float xSens = 2;
    public float ySens = 2;
    public float camRotLim = 90;

    public float groundDetecDist = 1.1f;

    // Start is called before the first frame update
    void Start()
    { 
        // Initialize components
        myRB = GetComponent<Rigidbody>();
        playerCam = transform.GetChild(0).GetComponent<Camera>();

        camRot = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Do the Hokey Pokey and turn yourself around 
        camRot.x += Input.GetAxisRaw("Mouse X") * mouseSens;
        camRot.y += Input.GetAxisRaw("Mouse Y") * mouseSens;

        playerCam.transform.localRotation = Quaternion.AngleAxis(camRot.y, Vector3.left);
        transform.localRotation = Quaternion.AngleAxis(camRot.x, Vector3.up);

        //Neck Brace
        camRot.y = Mathf.Clamp(camRot.y, -camRotLim, camRotLim);
       
        //Movement variables
        Vector3 temp = myRB.velocity;

        temp.x = Input.GetAxisRaw("Vertical") * MoveSpeed;
        temp.z = Input.GetAxisRaw("Horizontal") * MoveSpeed;

        //Run for your life
        if (Input.GetKey(KeyCode.LeftControl))
        {
            temp.x *= sprintMulti;
        }

        //Jump for joy
        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetecDist))
        {
            temp.y = JumpHeight;
        }
        // Use the Force Luke
        myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
    }
}
