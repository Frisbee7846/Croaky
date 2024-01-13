using UnityEngine;
public class Bullet : MonoBehaviour
{
    [SerializeField] BulletSpawner spawner;
   
  
    [Header("Wave Settings"),Tooltip("settings for cosine and sine wave in wave bullet pattern")]
    public float amplitude = 1f; // Amplitude of the oscillation
    public float frequency = 2f; // Frequency of the oscillation
    [SerializeField] float speed;
    [SerializeField] float accelFactor;

    private float _maxSpeed;
    private float _timeTraveled;
    private Vector3 _direction;
    private float _acceleratedSpeed;

    private bool _reverse;
    void Awake()
    {
        spawner = FindObjectOfType<BulletSpawner>();
        _maxSpeed = speed;
        
    }
    void Update()
    {
        _timeTraveled += Time.deltaTime;
        BulletPatterns();
    }

    private void ModifyBulletSpeed()
    {
        if (spawner.Retardation)
        {
            speed -= accelFactor*Time.deltaTime;
            speed = Mathf.Clamp(speed, _maxSpeed-accelFactor,_maxSpeed);

        }
        if (spawner.Acceleration)
        {
            _acceleratedSpeed += accelFactor*Time.deltaTime;
            speed = Mathf.Clamp(_acceleratedSpeed, 0, _maxSpeed);
        }
    }
    
     // Has the patterns of the different bullets giving the combination to modify each one
     
    private void BulletPatterns()
    {
        ModifyBulletSpeed();
        // Revese direction and not wave in a wave
        if (spawner.Boomerang && !spawner.Wave)
        {
            transform.position += _direction * (speed * Time.deltaTime);
            Reverse();
        }

        /* Calculate new wave pattern and moves the bullet in the given direction oscillating between amplitude x and y
           Does not return 
        */ 
        if (spawner.Wave && !spawner.Boomerang)
        {
            float oscillationX = amplitude * Mathf.Sin(frequency * Time.time);
            float oscillationY = amplitude * Mathf.Cos(frequency / 2 * Time.time);

            Vector3 oscillatingVector = (_direction+
                                       new Vector3(
                                           Mathf.Clamp(oscillationX, -amplitude, amplitude),
                                           Mathf.Clamp(oscillationY, -amplitude, amplitude), 0)).normalized;
            
            
            transform.position +=  oscillatingVector *(speed * Time.deltaTime);
        }
        /* Calculate new wave pattern and moves the bullet in the given direction oscillating between amplitude x and y
           Does return 
        */ 
        if (spawner.Boomerang && spawner.Wave)
        {
            float oscillationX = amplitude * Mathf.Sin(frequency * Time.time);
            float oscillationY = amplitude * Mathf.Cos(frequency / 2 * Time.time);

            
            Vector3 oscillatingVector = (_direction+
                                        new Vector3(
                                            Mathf.Clamp(oscillationX, -amplitude, amplitude),
                                            Mathf.Clamp(oscillationY, -amplitude, amplitude), 0)).normalized;
            
            transform.position += oscillatingVector*(speed * Time.deltaTime);
            Reverse();
        }

        if (spawner.None || spawner.Acceleration || spawner.Retardation)
        {
            transform.position += Time.deltaTime * speed*_direction;
        }
    }

    public void Direction(Vector3 direction)
    {
        _direction = direction.normalized;
    }
    //TODO::Fix Destroy involving Retardation and acceleration destroys too late and too early
    private void Reverse()
    {
        //TODO: Add If colliding with wall reverse
        if (_timeTraveled > spawner.MaxTimeBoomerang && !_reverse)
        {
            _reverse = true;
            _direction = -1 * _direction;
            Destroy(gameObject, spawner.MaxTimeBoomerang);
        }
    }
    
}