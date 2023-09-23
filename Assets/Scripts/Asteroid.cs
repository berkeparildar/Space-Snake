using UnityEngine;

public class Asteroid : MonoBehaviour
{
    
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private GameObject asteroidImage;
    [SerializeField] private SnakeMovement snake;
    [SerializeField] private Vector3 movementDirection;
    [SerializeField] private bool changedDirectionOnce;
    [SerializeField] private Animator anim;
    [SerializeField] private PolygonCollider2D polygonCollider2D;

    private void Start()
    {
        snake = GameObject.Find("Snake").GetComponent<SnakeMovement>();
        movementDirection = Vector2.up;
    }

    private void Update()
    {
        if (!GameManager.GameOver)
        {
            asteroidImage.transform.Rotate(transform.forward * (rotationSpeed * Time.deltaTime));
            transform.Translate(movementDirection * (speed * Time.deltaTime));
            DestroyIfOutOfScreen();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag is "Tail" or "Body" or "Head")
        {
            snake.IsBitten();
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyShip>().DestroyShip();
            DestroyAsteroid();
        }
        else if (other.gameObject.CompareTag("Asteroid"))
        {
            ChangeAsteroidDirection();
            other.GetComponent<Asteroid>().ChangeAsteroidDirection();
        }
        else if (other.gameObject.CompareTag("Shield"))
        {
            var shieldsInGame = GameObject.FindGameObjectsWithTag("Shield");
            foreach (var shield in shieldsInGame)
            {
                Destroy(shield);
            }
            DestroyAsteroid();
        }
    }

    public void ChangeAsteroidDirection()
    {
        if (!changedDirectionOnce)
        {
            changedDirectionOnce = true;
            var currentRot = transform.rotation.eulerAngles;
            Debug.Log(currentRot);
            currentRot.z *= -1;
            Debug.Log(currentRot);
            transform.rotation = Quaternion.Euler(currentRot);
        }
        // movementDirection = Vector2.down;
    }

    public void DestroyAsteroid()
    {
        anim.SetTrigger("death");
        asteroidImage.SetActive(false);
        polygonCollider2D.enabled = false;
        Destroy(gameObject, 1);
    }
    
    private void DestroyIfOutOfScreen()
    {
        if (transform.position.x is >= 9 or <= -9 || transform.position.y is >= 13 or <= -13 )
        {
            Destroy(gameObject);
        }
    }
}
