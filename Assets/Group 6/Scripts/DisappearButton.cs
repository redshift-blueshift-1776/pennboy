using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneChanger.lvl_name is null) {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
