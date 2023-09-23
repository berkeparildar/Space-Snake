using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private SnakeMovement snake;
    [SerializeField] private float spawnTime;

    private void Start()
    {
        if (!GameManager.GameOver)
        {
            snake = GameObject.Find("Snake").GetComponent<SnakeMovement>();    
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
        spawnTime += Time.deltaTime;
        DestroyIfOutOfScreen();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            Debug.Log("SHIELD CONTACT");
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Body"))
        {
            snake.GotHitByLaser(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Head"))
        {
            snake.IsBitten();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy") && spawnTime >= 1.0f)
        {
            other.GetComponent<EnemyShip>().DestroyShip();
        }
        else if (other.CompareTag("Asteroid"))
        {
            other.GetComponent<Asteroid>().DestroyAsteroid();
        }
    }

    private void DestroyIfOutOfScreen()
    {
        if (transform.position.x is >= 6 or <= -6 || transform.position.y is >= 11 or <= -11 )
        {
            Destroy(gameObject);
        }
    }
}
