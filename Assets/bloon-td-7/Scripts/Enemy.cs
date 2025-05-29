using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public GameObject body;
    [SerializeField] public GameObject model;
    [SerializeField] public GameObject parent;

    public float moveSpeed;
    public int waypointIndex = 0;
    protected List<Vector3> waypoints;
    protected Rigidbody rb;
    public int dmg;
    public int health;
    public int id;
    public int moneyWorth;
    public string modifiers;
    private const float WAYPOINT_CHANGE_DISTANCE = 0.01f;
    private Color32 originalColor;
    public bool isCamo { get; protected set; }
    public float size;
    private bool canTeleport;
    private Color32 teleportColor = new Color32(255,255,0,255);

    private float renderSizeY;
    private bool canStart = false;
    private Vector3 targetPos;

    public float DistanceTravelled { get; protected set; }

    public int originalHealth;


    private float teleportColorChangeTimer = 0;
    private float teleportColorChangeInterval = 0.05f;
    private bool isTeleporting = false;

    private AudioSource audioSource;
    private AudioClip teleportSound;
    private AudioClip explosionSound;

    float timer;

    [SerializeField] float dx;
    [SerializeField] float dz;

    private void Start()
    {
        //Initialize(15f, 1, 3, 0, 1, false, 5);
        Initialize(moveSpeed, dmg, health, id, moneyWorth, originalColor, isCamo, size, canTeleport);
        audioSource = BTD7.GameManager.instance.waveManager.GetComponent<AudioSource>();
        teleportSound = BTD7.GameManager.instance.teleportSound;
        explosionSound = BTD7.GameManager.instance.explosionSound;
    }

    /// <summary>
    /// Initializes a new enemy with the given parameters. To be called when instatiating new enemies.
    /// </summary>
    public void Initialize(float moveSpeed, int dmg, int health, int id, int moneyWorth, Color32 color, bool isCamo, float size, bool canTeleport)
    {
        this.health = health;
        this.originalHealth = health;
        this.id = id;
        this.dmg = dmg;
        this.moveSpeed = moveSpeed;
        this.originalColor = color;
        this.isCamo = isCamo;
        this.moneyWorth = moneyWorth;
        this.size = size + Random.Range(-0.3f,0.3f);
        this.canTeleport = canTeleport;

        body.GetComponent<Renderer>().material.color = originalColor;

        //height given random deviations to prevent ui glitching
        Vector3 scaleVector = new Vector3(size, size + Random.Range(-1f,4f), size);
        transform.localScale = scaleVector;
        model.transform.localScale = scaleVector;
        renderSizeY = size + Random.Range(-1f, 4f);
        waypoints = new List<Vector3>();
        parent.layer = 2;

        Transform waypointListTransform = GameObject.Find("EnemyWaypoints").transform;

        // Temp script to get all waypoints until game manager is implemented
        foreach (Transform child in waypointListTransform)
        {
            waypoints.Add(child.position);
        }

        parent.transform.position = waypoints[0];

        rb = GetComponent<Rigidbody>();

        DistanceTravelled = Mathf.PI * (Random.Range(1, 100) / 100f);
        targetPos = parent.transform.position;
        canStart = true;
    }

    protected void Update()
    {
        if (!canStart) return;
        DistanceTravelled += Time.deltaTime * Mathf.Sqrt(moveSpeed) * 1.5f;
        if (Vector3.Distance(targetPos, waypoints[waypointIndex+1]) <= WAYPOINT_CHANGE_DISTANCE)
        {
            targetPos = waypoints[waypointIndex + 1];
            waypointIndex++;
            if (waypointIndex >= waypoints.Count-1)
            {
                // Deal dmg damage to player health
                BTD7.GameManager.instance.healthManager.TakeDamage(dmg);
                Die();
                BTD7.GameManager.instance.moneyManager.SpendMoney(moneyWorth);
            }
        }
    }

    private void FixedUpdate()
    {
        float randomNum = Random.Range(0, 1000);
        float randomMovement = 0;
        timer += Time.fixedDeltaTime;


        if (teleportColorChangeTimer < teleportColorChangeInterval && isTeleporting)
        {
            teleportColorChangeTimer += Time.fixedDeltaTime;
        } else
        {
            isTeleporting = false;
            body.GetComponent<Renderer>().material.color = originalColor;
            teleportColorChangeTimer = 0;
        }

        if (canTeleport)
        {
            if (randomNum >= 970 && !isTeleporting)
            {
                randomMovement = moveSpeed * Random.Range(0.5f,1.5f) + 2;
                body.GetComponent<Renderer>().material.color = teleportColor;
                isTeleporting = true;
                audioSource.clip = teleportSound;
                audioSource?.Play();
            }
        }
        targetPos = Vector3.MoveTowards(targetPos, waypoints[waypointIndex + 1], moveSpeed * Time.fixedDeltaTime + randomMovement);
        float sinpos = Mathf.Abs(Mathf.Sin(timer * 5f));
        float sinsize = Mathf.Abs(Mathf.Sin(DistanceTravelled - (Mathf.PI / 5)));
        model.transform.localScale = new Vector3(size, (renderSizeY * .8f) + (sinsize * renderSizeY * .2f), size);
        model.transform.localPosition = new Vector3(0, (sinpos * 5f) + (renderSizeY/4), 0);
        parent.transform.position = targetPos;

        // Handle rotation
        dx = waypoints[waypointIndex + 1].x - targetPos.x;
        dz = waypoints[waypointIndex + 1].z - targetPos.z;
        if (dx > 0) {
            if (dz > 0) {
                if (dx > dz) {
                    model.transform.localEulerAngles = new Vector3(0,0,0);
                } else {
                    model.transform.localEulerAngles = new Vector3(0,90,0);
                }
            } else {
                if (dx > -1 * dz) {
                    model.transform.localEulerAngles = new Vector3(0,0,0);
                } else {
                    model.transform.localEulerAngles = new Vector3(0,-90,0);
                }
            }
        } else {
            if (dz > 0) {
                if (-1 * dx > dz) {
                    model.transform.localEulerAngles = new Vector3(0,180,0);
                } else {
                    model.transform.localEulerAngles = new Vector3(0,-90,0);
                }
            } else {
                if (-1 * dx > -1 * dz) {
                    model.transform.localEulerAngles = new Vector3(0,180,0);
                } else {
                    model.transform.localEulerAngles = new Vector3(0,90,0);
                }
            }
        }
    }

    /// <summary>
    /// Destroys this enemy's game object
    /// </summary>
    public void Die()
    {
        int moneyToGet = (int) Mathf.Ceil(moneyWorth
            * IncomeMultiplier(BTD7.GameManager.instance.waveManager.waveIndex));
        Debug.Log(moneyToGet);
        BTD7.GameManager.instance.moneyManager.EarnMoney(moneyToGet);
        Destroy(parent);
    }

    float IncomeMultiplier(int round)
    {
        if (round <= 20) return 1f;
        Debug.Log(Mathf.Clamp01(0.99f - (round - 18) * 0.05f) + 0.01f);
        return Mathf.Clamp01(0.99f - (round - 18) * 0.05f) + 0.01f; // 5% less per round after 18
    }


    public void Damage(int dmg)
    {
        health -= dmg;
        audioSource.clip = explosionSound;
        audioSource?.Play();
        if (health <= 0)
        {
            // Add money count to game manager
            Die();
        }
    }

}
