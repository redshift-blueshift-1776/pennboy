using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject trees;
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject ground;
    [SerializeField] float grassDistance = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Grass generated!");
        
        Transform[] children = trees.GetComponentsInChildren<Transform>();
        foreach (Transform tree in trees.transform)
        {
            if (tree != trees.transform) // Exclude the ground itself
            {
                // Make each child object face the camera
                tree.LookAt(mainCamera.transform);
                tree.Rotate(0, 180, 0); // Rotate to face the camera correctly
            }
        }
        // GenerateGrass();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateGrass() {
        Vector3 size = ground.GetComponent<MeshRenderer>().bounds.size;

        for (float x = -70; x < 70; x += grassDistance)
        {
            for (float z = -70; z < 70; z += grassDistance)
            {
                // Randomly generate grass positions within the bounds of the ground
                float randomX = Random.Range(0, grassDistance);
                float randomZ = Random.Range(0, grassDistance);

                Vector3 pos = new Vector3(x + randomX, 2, z + randomZ);
                GameObject grass = Instantiate(grassPrefab, pos, Quaternion.identity);
                grass.transform.parent = ground.transform;
            }
        }
    }
}
