using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public float fireRate;

    public static bool fireLocked = false;
    public static bool isFiring = false;
    private float nextFire;
    private bool doShoot;
    private Transform myTransform;

    private Camera myCam;

    private void Awake()
    {
        // Setting up references.
        myTransform = GetComponent<Transform>();
        myCam = Camera.main;
        nextFire = Time.time;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButton(0))
        {
            if (isLocalPlayer && !fireLocked)
            {
                doShoot = true;
                isFiring = true;
            }
        }
        else
        {
            isFiring = false;
        }
    }

    void FixedUpdate()
    {
        if (doShoot)
        {
            CmdDoShoot(myCam.ScreenToWorldPoint(Input.mousePosition));
        }
        doShoot = false;
    }

    [Command (channel=0)] void CmdDoShoot(Vector3 mousePos)
    {
        Vector3 shootDirection;
        //MUST CHECK RATE OF FIRE AND OTHER ITEMS FROM SERVER STORED CHARACTER STATS
        if (Time.time < nextFire)
            return; //fired too recently

        nextFire = Time.time + fireRate;

        float accuracy = .95f;
        float gunRange = 4.0f;
        int BulletSpeed = 30;

        float accMiss = 1 - accuracy;
        accMiss = Random.Range(accMiss * -1, accMiss);
        accMiss = accMiss * (1 / gunRange);

        shootDirection = mousePos;

        //...setting shoot direction
        shootDirection = shootDirection - transform.position;
        shootDirection.z = 0;
        shootDirection.Normalize();
        shootDirection = new Vector3(shootDirection.x + accMiss, shootDirection.y + accMiss, 0.0f);
        shootDirection.Normalize();

        Quaternion fireRot = Quaternion.Euler(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);

        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            myTransform.position,
            fireRot);

        bullet.GetComponent<Rigidbody2D>().velocity = shootDirection * BulletSpeed;

        NetworkServer.Spawn(bullet);
        Destroy(bullet, 2.0f);
    }

}
