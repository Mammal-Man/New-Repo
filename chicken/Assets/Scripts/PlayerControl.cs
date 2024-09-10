using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody myRB;

    public float MoveSpeed = 10;

    public float JumpHeight = 5;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = myRB.velocity;

        temp.x = Input.GetAxisRaw("Vertical") * MoveSpeed;
        temp.z = Input.GetAxisRaw("Horizontal") * MoveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            temp.y = JumpHeight;
        }

        myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
    }
}
