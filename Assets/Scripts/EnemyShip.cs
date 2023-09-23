using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private SnakeMovement snake;
    [SerializeField] private Animator anim;
    [SerializeField] private bool isAlive;
    [SerializeField] private GameObject laser;
    [SerializeField] private Transform laserSpawnPoint;
    private static readonly int Death = Animator.StringToHash("death");

    private void Start()
    {
        isAlive = true;
        snake = GameObject.Find("Snake").GetComponent<SnakeMovement>();
        StartCoroutine(ShootLaserCoroutine());
    }

    private void Update()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag is "Tail" or "Body" or "Head")
        {
            snake.IsBitten();
            isAlive = false;
            anim.SetTrigger(Death);
            Destroy(gameObject, 2);
        }
    }

    private IEnumerator ShootLaserCoroutine()
    {
        while (isAlive)
        {
            var chanceToShoot = Random.Range(0, 2);
            if (chanceToShoot == 1)
            {
                var shotLaser = Instantiate(laser, laserSpawnPoint.position, transform.rotation);
            }
            yield return new WaitForSeconds(Random.Range(2, 5));
        }
    }
}
