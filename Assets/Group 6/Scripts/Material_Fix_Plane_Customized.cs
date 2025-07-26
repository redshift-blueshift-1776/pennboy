using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Fix_Plane_Customized : MonoBehaviour
{
    [SerializeField] public float tileSizeX;
    [SerializeField] public float tileSizeZ;
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Vector3 scale = transform.lossyScale;

        renderer.material = new Material(renderer.material);

        renderer.material.mainTextureScale = new Vector2(scale.x / tileSizeX, scale.z / tileSizeZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
