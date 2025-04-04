using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinTimer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TMP_Text txt = GetComponent<TMP_Text>();
        txt.text = "Time: " + SceneChanger.lvl_time.ToString(@"hh\:mm\:ss");
    }
}
