using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody myRB;

    public float MoveSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            myRB.AddForce(transform.forward * MoveSpeed) ;

        }
    }
}
