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
    public int id2;
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
        this.id2 = id;
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
        if (dx != 0 || dz != 0) {
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
                        model.transform.localEulerAngles = new Vector3(0,90,0);
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
    }

    /// <summary>
    /// Destroys this enemy's game object
    /// </summary>
    public void Die()
    {
        float multiplier = IncomeMultiplier(BTD7.GameManager.instance.waveManager.waveIndex);
        int moneyToGet = (int) Mathf.Ceil(moneyWorth
            * multiplier);
        //Debug.Log(moneyToGet);
        if ((moneyWorth == 1) && (multiplier < 0.1)) {
            moneyToGet = 0;
        }
        BTD7.GameManager.instance.moneyManager.EarnMoney(moneyToGet);

        // Spawn child enemies (layering)
        // List<int> childIds;
        // if (BTD7.GameManager.instance.waveManager.layeredEnemyBreakdown.TryGetValue(id, out childIds))
        // {
        //     foreach (int childId in childIds)
        //     {
        //         SpawnChildEnemy(childId);
        //     }
        // }

        Destroy(parent);
    }

    // public WaveManager.EnemyInfo[] enemyList =
    // {
    //     new EnemyInfo(10f,1,1,1, new Color32(0,255,0,255)),               //0 - slime
    //     new EnemyInfo(25f,1,1,2, new Color32(125,209,123, 255)),        //1 - goblin
    //     new EnemyInfo(8f,4,5,2, new Color32(21, 92, 20, 255),8),         //2 - orcs
    //     new EnemyInfo(6f,10,15,5,new Color32(70, 89, 70, 255),12),        //3 - ogres
    //     new EnemyInfo(20f,1,1,1,new Color32(255,255,255,255)),       //4 skeleton
    //     new EnemyInfo(10f,10,5,1, new Color32(64, 255, 150,255)),   //5 elf
    //     new EnemyInfo(40f,2,1,1, new Color32(222, 182, 250,255)),  //6 fairy
    //     new EnemyInfo(50f,15,3,5, new Color32(117, 12, 5,255)),    //7 demon
    //     new EnemyInfo(10f,3,4,1, new Color32(100,100,100,255)),    //8 dwarf
    //     new EnemyInfo(20f,20,5,5, new Color32(40, 96, 250,255)),   //9 wizard
    //     new EnemyInfo(35f,40,12,5, new Color32(139, 155, 199,255)), //10 light wizard
    //     new EnemyInfo(35f,40,12,5, new Color32(0, 0, 46,255)), //11 dark wizard
    //     new EnemyInfo(50f,100,25,5, new Color32(114, 0, 252,255),8, false, true), //12 master wizard
    //     new EnemyInfo(100f,1000,100,10,new Color32(255,0,0,255),20), //13 dragon
    //     new EnemyInfo(40f,10,1,1, new Color32(0,0,0,255),4, false, true), //14 the flash
    //     new EnemyInfo(5f, 1000, 5000, 300, new Color32(255,255,255,255), 30), //15 god, G.O.D.
    //     //new EnemyInfo(30f,10,10,100,Color.cyan),        // fast assassain enemy
    //     //new EnemyInfo(100f,0,10000,0,Color.black),       //4 - distraction enemy
    //     new EnemyInfo(150f,1000,200,10,new Color32(255,128,0,255),25), //16 super dragon
    //     // New Super Dragons pop into Dragons
    //     new EnemyInfo(3f,1000,30000,1000,new Color(0, 0, 0),36) //17 - old boss enemy
    //     //new EnemyInfo(3f,1000,11750,1000,new Color(0, 0, 0),36) //17 - new boss enemy, B.O.S.S.
    //     // New Boss enemy spawns: 3 G.O.D.s, 10 super dragons, 10 dragons, and 10 master wizards
    //     // RBE: 11750 + 3(5000) + 10(200) + 10(100) + 10(25)
    // };

    public Dictionary<int, int> layeredEnemyBreakdown = new Dictionary<int, int>()
    {
        { 1, 0 },
        { 2, 0 },
        { 3, 2 },
        { 4, 0 },
        { 5, 0 },
        { 6, 0 },
        { 7, 6 },
        { 8, 4 },
        { 9, 1 },
        { 10, 9 },
        { 11, 9 },
        { 12, 9 },
        { 13, 3 },
        { 15, 13 },
        { 16, 13 },
        { 17, 15 }
    };

    private void BecomeChildEnemy(int childId)
    {
        float multiplier = IncomeMultiplier(BTD7.GameManager.instance.waveManager.waveIndex);
        int moneyToGet = (int) Mathf.Ceil(moneyWorth
            * multiplier);
        //Debug.Log(moneyToGet);
        if ((moneyWorth == 1) && (multiplier < 0.1)) {
            moneyToGet = 0;
        }
        BTD7.GameManager.instance.moneyManager.EarnMoney(moneyToGet);

        WaveManager.EnemyInfo childInfo = BTD7.GameManager.instance.waveManager.enemyList[childId];
        
        this.health = childInfo.health;
        //this.originalHealth = childInfo.health;
        this.id2 = childId;
        this.dmg = childInfo.dmg;
        this.moveSpeed = childInfo.moveSpeed;
        this.originalColor = childInfo.color;
        this.isCamo = childInfo.isCamo;
        //this.moneyWorth = childInfo.moneyWorth;
        this.size = childInfo.size + Random.Range(-0.3f,0.3f);
        this.canTeleport = childInfo.canTeleport;

        body.GetComponent<Renderer>().material.color = originalColor;

        //height given random deviations to prevent ui glitching
        Vector3 scaleVector = new Vector3(size, size + Random.Range(-1f,4f), size);
        transform.localScale = scaleVector;
        model.transform.localScale = scaleVector;
        renderSizeY = size + Random.Range(-1f, 4f);
        parent.layer = 2;
    }


    float IncomeMultiplier(int round)
    {
        if (round <= 20) return 1f;
        Debug.Log(Mathf.Clamp01(0.95f - (round - 10) * 0.05f) + 0.05f);
        return Mathf.Clamp01(0.95f - (round - 10) * 0.05f) + 0.05f; // 5% less per round after 18
    }


    public void Damage(int dmg)
    {
        health -= dmg;
        audioSource.clip = explosionSound;
        audioSource?.Play();
        if (health <= 0) {
            int childId;
            //Debug.Log(layeredEnemyBreakdown.TryGetValue(this.id2, out childId));
            if (layeredEnemyBreakdown.TryGetValue(this.id2, out childId)) {
                Debug.Log(this.id2 + " becoming " + childId);
                BecomeChildEnemy(childId);
            } else {
                // Add money count to game manager
                Die();
            }
        }
    }

}
