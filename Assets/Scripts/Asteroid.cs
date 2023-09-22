using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private GameObject asteroidImage;
    [SerializeField] private SnakeMovement snake;

    private void Start()
    {
        snake = GameObject.Find("Snake").GetComponent<SnakeMovement>();
    }

    private void Update()
    {
        asteroidImage.transform.Rotate(transform.forward * (rotationSpeed * Time.deltaTime));
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag is "Tail" or "Body" or "Head")
        {
            snake.IsBitten();
            
        }
    }
}
