using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private GameObject snake;
    [SerializeField] private bool justTeleportedHorizontally;
    [SerializeField] private bool justTeleportedVertically;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private int score;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject spaceShipPrefab;
    [SerializeField] private bool spawnedAsteroids;
    [SerializeField] private bool spawnedEnemySpaceShips;
    public static bool GameOver;
    public int foodCount = 1;

    private void Start()
    {
        snake = GameObject.Find("Snake");
        StartCoroutine(ScoreIncreaseRoutine());
    }

    private void Update()
    {
        if (!GameOver)
        {
            CheckSnakeTeleportation();
            SpawnFood();
            UpdateUI();
        }
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

    public void DecreaseFoodCount()
    {
        foodCount--;
        score += 100;
    }

    private IEnumerator ScoreIncreaseRoutine()
    {
        while (!GameOver)
        {
            score++;
            if (score >= 850 && !spawnedAsteroids)
            {
                spawnedAsteroids = true;
                //StartCoroutine(AsteroidSpawnRoutine());
            }

            if (score >= 300 && !spawnedEnemySpaceShips)
            {
                spawnedEnemySpaceShips = true;
                StartCoroutine(EnemySpawnRoutine());
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
    }

    private IEnumerator AsteroidSpawnRoutine()
    {
        while (!GameOver)
        {
            var xPoses= new[] { Random.Range(-8, -6), Random.Range(6, 8) };
            var yPoses = new[] { Random.Range(-12, -10), Random.Range(10, 12) };
            var asteroid = Instantiate(asteroidPrefab, new Vector3(xPoses[Random.Range(0, 2)], yPoses[Random.Range(0, 2)], 0),
                Quaternion.identity);
            asteroid.transform.up = snake.transform.position - asteroid.transform.position;
            yield return new WaitForSeconds(10);
        }
        yield return null;
    }

    private IEnumerator EnemySpawnRoutine()
    {
        while (!GameOver)
        {
            var xPoses= new[] { Random.Range(-8, -6), Random.Range(6, 8) };
            var yPoses = new[] { Random.Range(-12, -10), Random.Range(10, 12) };
            var ship = Instantiate(spaceShipPrefab, new Vector3(xPoses[Random.Range(0, 2)], yPoses[Random.Range(0, 2)], 0),
                Quaternion.identity);
            ship.transform.up = snake.transform.position - ship.transform.position;
            yield return new WaitForSeconds(10);
        }
    }
}
