using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private GameManager gameManager;
    [SerializeField] private bool justAte;
    [SerializeField] private Sprite tailSprite;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject snakeBodyContainer;
    [SerializeField] private float shieldCooldown;
    [SerializeField] private bool justGotShield;
    [SerializeField] private GameObject coinPopUpPrefab;
    [SerializeField] private GameObject shieldPopUpPrefab;
    [SerializeField] private bool pickedCoin;
    [SerializeField] private Color[] snakeColors;
    [SerializeField] private Vector2 sourceTouchPosition;
    private static readonly int Eat = Animator.StringToHash("eat");
    private static readonly int Death = Animator.StringToHash("death");
    [SerializeField] private bool isTouching;

    private void Start()
    {
        SnakeInitialization();
    }

    private void Update()
    {
        if (!hasBitten)
        {
            if (Input.touchCount > 0)
            {
                Debug.Log("There is a touch");
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("BEGUNNN");
                    sourceTouchPosition = touch.position;
                }

                float horizontalMovement = (touch.position.x - sourceTouchPosition.x) * 0.3f * Time.deltaTime;
                transform.Rotate(Vector3.forward * horizontalMovement);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!hasBitten)
        {
            MoveSnakeHead();
            MoveSnakeBody();
        }
        ShieldCooldown();
    }

    private void MoveSnakeHead()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
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
        var colorIndex = PlayerPrefs.GetInt("Color", 0);
        Vector3 spawnPosition;
        var snakeBodyCount = snakeBody.Count;
        if (snakeBody.Count == 1)
        {
            spawnPosition = snakeBody[snakeBodyCount - 1].transform.position;
        }
        else
        {
            spawnPosition = snakeBody[snakeBodyCount - 2].transform.position;
        }

        var body = Instantiate(bodyPrefab, spawnPosition, transform.rotation);
        body.GetComponent<SpriteRenderer>().color = snakeColors[colorIndex];
        snakeBody.Insert(snakeBody.Count - 1, body);
        body.transform.SetParent(snakeBodyContainer.transform);
        StartCoroutine(EatAnimationCoroutine());
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food") && !justAte)
        {
            justAte = true;
            other.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(FoodCooldown());
            other.GetComponent<Animator>().SetTrigger(Death);
            Debug.Log("Yum!");
            GrowSnake();
            gameManager.DecreaseFoodCount();
            Destroy(other.gameObject, 1);
        }
        else if (other.CompareTag("ShieldPowerUp") && !justGotShield)
        {
            var shieldPopUp = Instantiate(shieldPopUpPrefab, transform.position, Quaternion.identity);
            Destroy(shieldPopUp, 1);
            other.GetComponent<Animator>().SetTrigger(Death);
            justGotShield = true;
            StartCoroutine(ShieldPickCooldown());
            if (shieldCooldown <= 0)
            {
                var shieldHead = Instantiate(shield, transform.position, Quaternion.identity);
                shieldHead.transform.SetParent(transform);
                foreach (var body in snakeBody)
                {
                    var shieldInit = Instantiate(shield, body.transform.position, Quaternion.identity);
                    shieldInit.transform.SetParent(body.transform);
                    shieldCooldown = 10;
                }
            }
            else
            {
                foreach (var body in snakeBody)
                {
                    if (transform.childCount == 1)
                    {
                        var shieldHead = Instantiate(shield, transform.position, Quaternion.identity);
                        shieldHead.transform.SetParent(transform);
                    }

                    if (body.transform.childCount == 0)
                    {
                        var shieldInit = Instantiate(shield, body.transform.position, Quaternion.identity);
                        shieldInit.transform.SetParent(body.transform);
                    }
                }

                shieldCooldown += 10;
            }

            gameManager.DecreaseShieldCount();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Coin") && !pickedCoin)
        {
            pickedCoin = true;
            other.GetComponent<Animator>().SetTrigger(Death);
            StartCoroutine(CoinPickCooldown());
            var coinAmount = Random.Range(50, 100);
            var coinPopUp = Instantiate(coinPopUpPrefab, transform.position, quaternion.identity);
            coinPopUp.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+" + coinAmount;
            Destroy(coinPopUp, 1);
            gameManager.IncreaseCoin(coinAmount);
        }
    }

    private void ShieldCooldown()
    {
        if (shieldCooldown >= 0)
        {
            shieldCooldown -= Time.deltaTime;
        }

        if (shieldCooldown <= 0)
        {
            var shiledsInGame = GameObject.FindGameObjectsWithTag("Shield");
            foreach (var sh in shiledsInGame)
            {
                Destroy(sh.gameObject);
            }
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
        var colorIndex = PlayerPrefs.GetInt("Color", 0);
        GetComponent<SpriteRenderer>().color = snakeColors[colorIndex];
        GameObject tail = Instantiate(tailPrefab);
        tail.GetComponent<SpriteRenderer>().color = snakeColors[colorIndex];
        snakeBody.Add(tail);
        GameObject body = Instantiate(bodyPrefab);
        body.GetComponent<SpriteRenderer>().color = snakeColors[colorIndex];
        snakeBody.Insert(snakeBody.Count - 1, body);
        Debug.Log(colorIndex);
    }

    private IEnumerator DeathCoroutine()
    {
        headAnimator.SetTrigger(Death);
        yield return new WaitForSeconds(0.1f);
        foreach (var body in snakeBody)
        {
            var bodyAnim = body.GetComponent<Animator>();
            bodyAnim.SetTrigger(Death);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.4f * snakeBody.Count);
        foreach (var body in snakeBody)
        {
            Destroy(body);
        }

        Destroy(gameObject);
    }

    private IEnumerator FoodCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        justAte = false;
    }

    private IEnumerator ShieldPickCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        justGotShield = false;
    }

    private IEnumerator CoinPickCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        pickedCoin = false;
    }

    public void IsBitten()
    {
        hasBitten = true;
        GameManager.GameOver = true;
        StartCoroutine(DeathCoroutine());
    }

    public void GotHitByLaser(GameObject hitBody)
    {
        var hitBodyIndex = snakeBody.IndexOf(hitBody);
        StartCoroutine(CutSnakeAtPoint(hitBodyIndex + 1));
    }

    public IEnumerator CutSnakeAtPoint(int hitBodyIndex)
    {
        Debug.Log(hitBodyIndex);
        for (int i = hitBodyIndex; i < snakeBody.Count; i++)
        {
            Debug.Log("Started loop");
            var bodyAnim = snakeBody[i].GetComponent<Animator>();
            if (!snakeBody[i].CompareTag("Tail"))
            {
                snakeBody[i].GetComponent<CircleCollider2D>().enabled = false;
            }

            bodyAnim.SetTrigger(Death);
            Debug.Log(snakeBody.Count);
            yield return new WaitForSeconds(0.1f);
            Debug.Log("After waiting");
        }

        var snakeBodyCount = snakeBody.Count;
        yield return new WaitForSeconds(0.2f * (snakeBody.Count - hitBodyIndex));
        for (int i = 0; i < snakeBodyCount - hitBodyIndex; i++)
        {
            var body = snakeBody[hitBodyIndex];
            snakeBody.RemoveAt(hitBodyIndex);
            Destroy(body);
        }

        var newSnakeBodyCount = snakeBody.Count;
        var newTail = snakeBody[newSnakeBodyCount - 1];
        newTail.GetComponent<SpriteRenderer>().sprite = tailSprite;
        newTail.GetComponent<CircleCollider2D>().enabled = false;
        newTail.tag = "Tail";
        yield return null;
    }
}