using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private GameObject snake;
    [SerializeField] private bool justTeleportedHorizontally;
    [SerializeField] private bool justTeleportedVertically;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private int score;
    [SerializeField] private int initialCoins;
    [SerializeField] private int coins;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject spaceShipPrefab;
    [SerializeField] private bool spawnedAsteroids;
    [SerializeField] private bool spawnedEnemySpaceShips;
    [SerializeField] private GameObject enemyContainer;
    [SerializeField] private GameObject asteroidContainer;
    [SerializeField] private GameObject foodContainer;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject coinContainer;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI snakeLenght;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private TextMeshProUGUI endScore;
    [SerializeField] private TextMeshProUGUI totalScore;
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI newHighScore;
    [SerializeField] private bool shownEndScreen;
    [SerializeField] private GameObject snakeBodyContainer;
    public static bool GameOver;
    public int foodCount = 0;
    public int shieldCount = 0;
    public int coinCount = 0;

    private void Awake()
    {
        snake = GameObject.Find("Snake");
        StartCoroutine(ScoreIncreaseRoutine());
        GameOver = false;
        initialCoins = PlayerPrefs.GetInt("Money", 0);
        coins = initialCoins;
    }

    private void Update()
    {
        if (!GameOver)
        {
            CheckSnakeTeleportation();
            SpawnFood();
            SpawnCoin();
            SpawnShield();
            UpdateUI();
        }
        else if (!shownEndScreen)
        {
            shownEndScreen = true;
            ShowEndScreen();
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
            var food = Instantiate(foodPrefab, new Vector2(randomXPos, randomYPos), Quaternion.identity);
            food.transform.SetParent(foodContainer.transform);
            foodCount++;
        }
    }

    public void DecreaseFoodCount()
    {
        foodCount--;
        score += 100;
    }
    
    public void DecreaseShieldCount()
    {
        shieldCount--;
    }

    private IEnumerator ScoreIncreaseRoutine()
    {
        while (!GameOver)
        {
            score++;
            if (score >= 1000 && !spawnedAsteroids)
            {
                spawnedAsteroids = true;
                StartCoroutine(AsteroidSpawnRoutine());
            }

            if (score >= 500 && !spawnedEnemySpaceShips)
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
        moneyText.text = coins.ToString();
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
            asteroid.transform.SetParent(asteroidContainer.transform);
            yield return new WaitForSeconds(15);
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
            ship.transform.SetParent(enemyContainer.transform);
            yield return new WaitForSeconds(10);
        }
    }
    
    private void SpawnShield()
    {
        if (shieldCount == 0 && score >= 1000)
        {
            var randomXPos = Random.Range(-5, 5);
            var randomYPos = Random.Range(-9, 9);
            var shield = Instantiate(shieldPrefab, new Vector2(randomXPos, randomYPos), Quaternion.identity);
            shield.transform.SetParent(foodContainer.transform);
            shieldCount++;
        }
    }
    
    private void SpawnCoin()
    {
        if (coinCount == 0)
        {
            var randomXPos = Random.Range(-5, 5);
            var randomYPos = Random.Range(-9, 9);
            var coin = Instantiate(coinPrefab, new Vector2(randomXPos, randomYPos), Quaternion.identity);
            coin.transform.SetParent(coinContainer.transform);
            coinCount++;
        }
    }

    public void IncreaseCoin(int coin)
    {
        coins += coin;
        coinCount--;
    }

    private void ShowEndScreen()
    {
        gameUI.SetActive(false);
        endScreen.SetActive(true);
        PlayerPrefs.SetInt("Money", coins);
        var snakeLengthIs = snakeBodyContainer.transform.childCount;
        snakeLenght.text = "Snake Length: " + snakeLengthIs;
        endScore.text = "Score: " + score.ToString();
        totalScore.text = "Total Score: " + (snakeLengthIs * score).ToString();
        var currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if ((snakeLengthIs * score) > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", (snakeLengthIs * score));
            highScore.text = "HighScore: " + (snakeLengthIs * score).ToString();
            newHighScore.transform.gameObject.SetActive(true);
        }
        else
        {
            highScore.text = "HighScore: " + currentHighScore.ToString();
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
