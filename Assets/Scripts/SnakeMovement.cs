using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject tailPrefab;
    [SerializeField] private int gap;
    [SerializeField] private float inputDirection;
    [SerializeField] private List<GameObject> snakeBody = new();
    [SerializeField] private List<Vector3> destinations = new();
    [SerializeField] private Animator headAnimator;
    [SerializeField] private bool hasBitten;
    [SerializeField] private float rayLength;
    private static readonly int Eat = Animator.StringToHash("eat");
    private static readonly int Death = Animator.StringToHash("death");

    private void Start()
    {
        SnakeInitialization();
    }

    // Update is called once per frame
   
    private void FixedUpdate()
    {
        inputDirection = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
        if (!hasBitten)
        {
            ChangeDirection();
            MoveSnakeBody();
        }
        Debug.DrawRay(transform.position, transform.up * rayLength, Color.cyan);
    }

    private void ChangeDirection()
    {
        transform.Rotate(transform.forward * (inputDirection * rotationSpeed * Time.deltaTime));
    }

    private void MoveSnakeBody()
    {
        destinations.Insert(0, transform.position);
        var destinationIndex = 1;
        foreach (var body in snakeBody)
        {
            if (body.CompareTag("Tail") && destinations.Count - 1 < destinationIndex * gap)
            {
                
            }
            else
            {
                var point = destinations[Mathf.Min(destinationIndex * gap, destinations.Count - 1)];
                body.transform.up = (Vector2)point - (Vector2)body.transform.position;
                body.transform.position = point;
                destinationIndex++;
            }
        }
        
        while (destinations.Count > snakeBody.Count * gap)
        {
            destinations.RemoveAt(destinations.Count - 1);
        }
    }

    private void GrowSnake()
    {
        var spawnPosition = Vector3.zero;
        if (snakeBody.Count == 1)
        {
            spawnPosition = snakeBody[snakeBody.Count - 1].transform.position;
        }
        else
        {
            spawnPosition = snakeBody[snakeBody.Count - 2].transform.position;
        }
        GameObject body = Instantiate(bodyPrefab, spawnPosition, transform.rotation);
        snakeBody.Insert(snakeBody.Count - 1, body);
        StartCoroutine(EatAnimationCoroutine());
    }

    private void AddTail()
    {
        GameObject tail = Instantiate(tailPrefab);
        snakeBody.Add(tail);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            Debug.Log("Yum!");
            var snakes = GameObject.FindGameObjectsWithTag("Head");
            for (int i = 0; i < snakes.Length; i++)
            {
                snakes[i].GetComponent<SnakeMovement>().GrowSnake();
            }
            GameManager.foodCount--;
            Destroy(other.gameObject);
        }
    }

    private IEnumerator EatAnimationCoroutine()
    {
        headAnimator.SetTrigger(Eat);
        yield return new WaitForSeconds(0.1f);
        for (var i = 0; i < snakeBody.Count - 1; i++)
        {
            var bodyAnim = snakeBody[i].GetComponent<Animator>();
            bodyAnim.SetTrigger(Eat);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SnakeInitialization()
    {
        GameObject tail = Instantiate(tailPrefab);
        snakeBody.Add(tail);
        GameObject body = Instantiate(bodyPrefab);
        snakeBody.Insert(snakeBody.Count - 1, body);
    }

    private IEnumerator DeathCoroutine()
    {
        headAnimator.SetTrigger(Death);
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < snakeBody.Count - 1; i++)
        {
            if (!snakeBody[i].CompareTag("Tail"))
            {
                var bodyAnim = snakeBody[i].GetComponent<Animator>();
                bodyAnim.SetTrigger(Death);
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(0.4f * snakeBody.Count);
        foreach (var body in snakeBody)
        {
            Destroy(body);
        }
        Destroy(gameObject);
    }

    public void IsBitten()
    {
        hasBitten = true;
        StartCoroutine(DeathCoroutine());
    }
}
