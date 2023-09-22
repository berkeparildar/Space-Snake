using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHitBox : MonoBehaviour
{
    [SerializeField] private BoxCollider2D hitBox;
    [SerializeField] private SnakeMovement snakeHead;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Body") || other.CompareTag("Tail"))
        {
            snakeHead.IsBitten();
        }
    }
}
