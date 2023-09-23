using UnityEngine;

public class SnakeHitBox : MonoBehaviour
{
    [SerializeField] private SnakeMovement snakeHead;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Body") || other.CompareTag("Tail"))
        {
            snakeHead.IsBitten();
        }
    }
}
