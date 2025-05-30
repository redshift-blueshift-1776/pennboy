using PennBoy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class sandwich : MonoBehaviour
{
    [SerializeField] public GameObject teacher;
    private rajiv Rajiv;
    public Vector3 downPosition = new Vector3(0, -5, 0); // The target position when spacebar is not pressed
    public Vector3 originalPosition; // The original position of the object
    public bool eating;

    [SerializeField] public TMP_Text scoreText;
    private float score;

    // Start is called before the first frame update
    void Start()
    {
        Rajiv = teacher.GetComponent<rajiv>();
        originalPosition = transform.position; // Store the initial position of the object
        scoreText.text = "Score: 0";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // When spacebar is pressed, set the object to its original position
            transform.position = originalPosition;
            eating = true;
            if (Rajiv.facingClass)
            {
                StartCoroutine(Lose());
            }

            score += Time.deltaTime;
            
        }
        else
        {
            // Move the object instantly to the downPosition when spacebar is not pressed
            transform.position = downPosition;
            eating = false;
        }

        if (Rajiv.facingClass && eating)
        {
            StartCoroutine(Lose());
        }

        scoreText.text = "Score: " + Mathf.Floor(10 * score);
    }

    public IEnumerator Lose()
    {
        Debug.Log("Game over");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(35);
    }
}
