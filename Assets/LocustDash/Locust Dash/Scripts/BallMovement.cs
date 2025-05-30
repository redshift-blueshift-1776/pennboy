using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//list of all movement types, including tests
enum MovementType {
    Test_JustForward,
    LeftRight,
    UpDown,
}

public class BallMovement : MonoBehaviour
{
    MovementType my_movement = MovementType.LeftRight;

    //list of movement types that have a chance of being picked, should exclude tests probably
    MovementType[] possible_movement_types = {
        MovementType.Test_JustForward, 
        MovementType.LeftRight,
        MovementType.UpDown,
    };

    float SecondsBeforeAutoDestroyBall = 30f;
    private float FixedTimeWhenIWasSpawned = 0f;

    // Start is called before the first frame update
    void Start()
    {
        int dice = Random.Range(0, possible_movement_types.Length);
        my_movement = possible_movement_types[dice];
        FixedTimeWhenIWasSpawned = Time.fixedTime;
    }


    // Update is called once per frame
    void Update()
    {
        if (my_movement == MovementType.Test_JustForward)
        {
            Vector3 targetPosition = transform.position;
            targetPosition.z += 100f * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 10f*Time.deltaTime);
        }
        else if (my_movement == MovementType.LeftRight)
        {
            Vector3 targetPosition = transform.position;
            targetPosition.x = 10f * Mathf.Sin(Time.fixedTime) * Mathf.Sin(Time.fixedTime);
            transform.position = targetPosition;
        }
        else if (my_movement == MovementType.UpDown)
        {
            Vector3 targetPosition = transform.position;
            targetPosition.y = 3f + 3f * Mathf.Sin(Time.fixedTime) * Mathf.Sin(Time.fixedTime);
            transform.position = targetPosition;
        }

        if (Time.fixedTime - FixedTimeWhenIWasSpawned > SecondsBeforeAutoDestroyBall) {
            Destroy(gameObject);
        }
    }
}
