using System.Collections;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private SnakeMovement snake;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject thruster;
    [SerializeField] private bool isAlive;
    [SerializeField] private GameObject laser;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private Transform laserSpawnPoint;
    [SerializeField] private GameObject laserContainer;
    [SerializeField] private AudioSource laserSound;
	[SerializeField] private AudioSource explosionSound;
    private static readonly int Death = Animator.StringToHash("death");

    private void Start()
    {
        isAlive = true;
        laserSound = GameObject.Find("LaserSoundPlayer").GetComponent<AudioSource>();
        explosionSound = GameObject.Find("ExplosionSoundPlayer").GetComponent<AudioSource>();
        laserContainer = GameObject.Find("LaserContainer");
        snake = GameObject.Find("Snake").GetComponent<SnakeMovement>();
        StartCoroutine(ShootLaserCoroutine());
    }

    private void Update()
    {
        if (isAlive && !GameManager.GameOver)
        {
            transform.Translate(Vector2.up * (speed * Time.deltaTime));
            DestroyIfOutOfScreen();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag is "Tail" or "Body" or "Head")
        {
            snake.IsBitten();
            DestroyShip();
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyShip>().DestroyShip();
        }
        else if (other.gameObject.CompareTag("Shield"))
        {
            var shieldsInGame = GameObject.FindGameObjectsWithTag("Shield");
            DestroyShip();
            foreach (var shield in shieldsInGame)
            {
                Destroy(shield);
            }
        }
    }

    public void DestroyShip()
    {
        isAlive = false;
        explosionSound.Play();
        boxCollider2D.enabled = false;
        anim.SetTrigger(Death);
        Destroy(thruster);
        Destroy(gameObject, 2);
    }

    private IEnumerator ShootLaserCoroutine()
    {
        while (isAlive && !GameManager.GameOver)
        {
            var chanceToShoot = Random.Range(0, 2);
            if (chanceToShoot == 1)
            {
                var shotLaser = Instantiate(laser, laserSpawnPoint.position, transform.rotation);
                shotLaser.transform.SetParent(laserContainer.transform);
				laserSound.Play();
            }
            yield return new WaitForSeconds(Random.Range(5, 7));
        }

        yield return null;
    }
    private void DestroyIfOutOfScreen()
    {
        if (transform.position.x is >= 9 or <= -9 || transform.position.y is >= 13 or <= -13 )
        {
            Destroy(gameObject);
        }
    }
}