using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Straight,
    Cone,
}

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject Bullet;
    
    
    [field:SerializeField] private BulletType Type { get; set; }
    [Header("Cone Setting"),Tooltip("Modifiers for cone ")]
    [SerializeField] private  int amountOfProjectiles = 3;
    [Tooltip("Angle between each bullet")]
    [SerializeField] private  float bulletSpreadAngle;
    [Tooltip("Sets the max time allowed before returning")]
    [field:Header("Boomerang Settings")]
    [field:SerializeField] public int MaxTimeBoomerang { get; private set; } = 1;
    
    private float _shootTimer;
    [Header("OtherVariables")]
    [Tooltip("Delay between each shot")]
    public float baseShootTimer;

    public BulletSpawner(bool wave, bool boomerang, bool retardation, bool acceleration, BulletType type)
    {
        Wave = wave;
        Boomerang = boomerang;
        Retardation = retardation;
        Acceleration = acceleration;
        Type = type;
    }

    [field:Header("Bullet Modifiers"), Tooltip("the bullet accesses the different booleans from spawner script, Modifying each bullet")]
    [field:SerializeField] public bool Boomerang { get; private set; }
    [field:SerializeField] public bool Wave { get; private set; }
    [field:SerializeField] public bool Retardation { get; private set; }
    [field:SerializeField] public bool Acceleration { get; private set; }
    [field:SerializeField] public bool None { get; private set; }
    
    void Update()
    {
        SpawnProjectile();
        SetNoModifier();
    }

    void SetNoModifier()
    {
        if (!Boomerang && !Wave && !Acceleration && !Retardation)
        {
            None = true;
        }
        else
        {
            None = false;
        }
    }

    void SpawnProjectile()
    {
        Camera cam = Camera.main;
        Debug.Assert(cam != null, nameof(cam) + " != null");

        Vector3 mousePos = Input.mousePosition;

        Vector3 aim = cam.ScreenToWorldPoint(mousePos);
        aim.z = 0;


        _shootTimer -= Time.deltaTime;


        if (Input.GetKey(KeyCode.Mouse0) && _shootTimer < 0)
        {
            _shootTimer = baseShootTimer;
            if (Type == BulletType.Straight)
            {
                Bullet bullet = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
                bullet.Direction(aim - bullet.transform.position);
            }

            if (Type == BulletType.Cone)
            {
                for (int i = 0; i < amountOfProjectiles; i++)
                {
                    float addedOffset = (i - (amountOfProjectiles /2)) * bulletSpreadAngle;
                    Quaternion newRot = Quaternion.Euler(0, 0, addedOffset);

                    Bullet bullet = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
                    Vector3 direction = (newRot * (aim - bullet.transform.position).normalized);
                    direction.z = 0;
                    bullet.Direction(direction);

                }
            }

        }

    }
    


}