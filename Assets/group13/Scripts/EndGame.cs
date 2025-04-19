using System.Collections;
using System.Collections.Generic;
using SlidingPuzzle;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public GameObject Endtext;
    private BennyPlayerMovement playerMovement;
    private void OnTriggerEnter(Collider p)
    {
        if (p.CompareTag("Player"))
        {
            Endtext.SetActive(true);
            Time.timeScale = 1f;
            playerMovement = p.GetComponentInChildren<BennyPlayerMovement>();
            p.GetComponentInChildren<MouseLook>().noUpdate = true;
            SoundManager soundManager = p.GetComponent<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlayEffect(0);
                soundManager.StopMusicWithFade(1f);
            }
            if (playerMovement != null) {
                playerMovement.enabled = false;
                print("PLAYER MOVEMENT FOUND");
            }
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
