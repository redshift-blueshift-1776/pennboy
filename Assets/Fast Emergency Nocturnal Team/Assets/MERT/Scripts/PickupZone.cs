using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupZone : MonoBehaviour
{
    public bool isPickupZone; 

    public Transform[] pickupSpots;
    public Transform[] dropoffSpots;

    void Start() {
        pickupSpots = FENTGameManager.Instance.pickupSpots;
        dropoffSpots = FENTGameManager.Instance.dropoffSpots;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPickupZone && !FENTGameManager.Instance.HasPassenger)
        {
            FENTGameManager.Instance.HasPassenger = true;
            Debug.Log("Picked up a passenger!");
            //Destroy(gameObject);
            transform.position = pickupSpots[Random.Range(0, pickupSpots.Length - 1)].position;
        }
        else if (!isPickupZone && FENTGameManager.Instance.HasPassenger)
        {
            FENTGameManager.Instance.HasPassenger = false;
            FENTGameManager.Instance.ScorePoints();
            Debug.Log("Dropped off a passenger!");
            //Destroy(gameObject);
            transform.position = dropoffSpots[Random.Range(0, dropoffSpots.Length - 1)].position;
        }
    }
}
