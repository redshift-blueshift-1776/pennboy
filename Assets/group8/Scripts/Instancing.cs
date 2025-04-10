using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instancing : MonoBehaviour
{
    public Transform prefab;

	public int instances = 5000;

	public float radius = 50f;
    public float inner_radius = 10f;

    public float height = 5f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < instances; i++) {
            Vector2 pos = Random.insideUnitCircle * radius;
            float dist = pos.magnitude;

            if (dist < inner_radius) {
                i--;
                continue;
            } else {
                Transform t = Instantiate(prefab);
                t.localPosition = new Vector3(pos.x, height, pos.y);
                t.localRotation = Quaternion.Euler(0, 0, 0);
                t.SetParent(transform);
            }

		} 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
