using UnityEngine;
using System.Collections;

public class rajiv : MonoBehaviour
{
    private int timer;
    public bool facingClass = true;
    //private int n;
    public float rotationSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        //n = 200;
        facingClass = true;
        StartCoroutine(Turn());
    }

    public IEnumerator Turn()
    {
        yield return new WaitForSeconds(rotationSpeed + Random.Range(-1.5f, 1.5f));
        while (true) {
            Debug.Log("Cycle");
            if (facingClass)
            {
                facingClass = false;
                Debug.Log("Not facing");
                transform.Rotate(0, 180, 0, Space.Self);
            } else
            {
                Debug.Log("Facing");
                facingClass = true;
                //transform.Rotate(0, 0, 0, Space.Self);
                transform.Rotate(0, 180, 0, Space.Self);
            }

            yield return new WaitForSeconds(rotationSpeed + Random.Range(-1.5f, 1.5f));
        }
    }
}
