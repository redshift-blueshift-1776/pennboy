using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
namespace SlidingPuzzle {
public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    //public float speed = 0.1f;

    // Update is called once per frame
    void Update()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) 
        {
            x = -1f;
        } 
        else if (Input.GetKey(KeyCode.D)) 
        {
            x = 1f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            z = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            z = -1f;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * 5 * Time.deltaTime);
        
    }
}
}