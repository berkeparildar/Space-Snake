using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject snake;
    [SerializeField] private bool justTeleportedHorizontally;
    [SerializeField] private bool justTeleportedVertically;
    [SerializeField] private GameObject foodPrefab;
    public static int foodCount = 1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckSnakeTeleportation();
    }

    private void CheckSnakeTeleportation()
    {
        var currentSnakePos = snake.transform.position;
        if (snake.transform.position.x is >= 6 or <= -6 && !justTeleportedHorizontally)
        {
            justTeleportedHorizontally = true;
            snake.transform.position = new Vector3(-currentSnakePos.x, currentSnakePos.y, 0);
            StartCoroutine(TeleportCooldown(true));
        }
        else if (snake.transform.position.y is >= 10.38f or <= -10.38f && !justTeleportedVertically)
        {
            justTeleportedVertically = true;
            snake.transform.position = new Vector3(currentSnakePos.x, -currentSnakePos.y, 0);
            StartCoroutine(TeleportCooldown(false));
        }
    }

    private IEnumerator TeleportCooldown(bool isHorizontal)
    {
        yield return new WaitForSeconds(0.1f);
        if (isHorizontal)
        {
            justTeleportedHorizontally = false;    
        }
        else
        {
            justTeleportedVertically = false;    
        }
    }

    private void SpawnFood()
    {
        if (foodCount == 0)
        {
            var randomXPos = Random.Range(-5, 5);
            var randomYPos = Random.Range(-9, 9);
            Instantiate(foodPrefab, new Vector2(randomXPos, randomYPos), Quaternion.identity);
            foodCount++;
        }
    }
}
