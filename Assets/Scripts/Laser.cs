using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private SnakeMovement snake;

    private void Start()
    {
        snake = GameObject.Find("Snake").GetComponent<SnakeMovement>();
    }

    private void Update()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Body"))
        {
            StartCoroutine(snake.CutSnakeAtPoint(other.gameObject));
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Head"))
        {
            snake.IsBitten();
            Destroy(gameObject);
        }
    }
}
