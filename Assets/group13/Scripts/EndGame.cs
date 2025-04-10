using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public GameObject Endtext;
    private PlayerMovement playerMovement;
    private void OnTriggerEnter(Collider p)
    {
        if (p.CompareTag("Player"))
        {
            Endtext.SetActive(true);
            Time.timeScale = 0f;
            playerMovement = p.GetComponent<PlayerMovement>();
            SoundManager soundManager = p.GetComponent<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlayEffect(0);
                soundManager.StopMusicWithFade(1f);
            }
            if (playerMovement != null)
                playerMovement.enabled = false;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
