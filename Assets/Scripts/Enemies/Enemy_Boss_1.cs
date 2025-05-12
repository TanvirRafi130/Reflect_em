using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Enemy_Boss_1 : MonoBehaviour, IEnemy
{

    public GameObject enemyObject => this.gameObject;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public float stopDistance = 2f;
    [SerializeField] List<Transform> shootingPosition;
    [SerializeField] public float shootInterval = 2f;
    [SerializeField] Image healthBar;
    GameObject myBullet;
    public float health;
    public float maxHealth = 100;
    Color myColor;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        SetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myBullet = GameManager.Instance.bullets[Random.Range(0, GameManager.Instance.bullets.Count - 1)];
        myColor = GameManager.Instance.colors[Random.Range(0, GameManager.Instance.colors.Count - 1)];
        originalScale = this.transform.localScale;



        transform.DORotate(new Vector3(0f, 0f, 360f), 0.8f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

    }
    void SetHealth()
    {
        healthBar.fillAmount = health / maxHealth;
    }


    // Update is called once per frame
    void Update()
    {
        if (Player.Instance == null || GameManager.Instance.isPaused) return;
        Vector3 direction = Player.Instance.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * 100f * Time.deltaTime);

        // Handle movement and animation
        float distanceToPlayer = direction.magnitude;


        if (distanceToPlayer > stopDistance)
        {

            transform.DOScale(originalScale, 0.5f);
            // Move towards player
            Vector2 moveDirection = direction.normalized;
            if (distanceToPlayer > 25f)
            {
                rb.linearVelocity = moveDirection * moveSpeed * 50;
            }
            else
            {
                rb.linearVelocity = moveDirection * moveSpeed;

            }

            // Set animation parameters

            // Flip sprite based on movement direction
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = moveDirection.x < 0;
            }
        }
        else
        {
            // Stop moving when close enough
            rb.linearVelocity = Vector2.zero;
        }
        if (distanceToPlayer < GameManager.Instance.currentWave.enemyShootStartDistance)
        {

            if (!isShooting)
            {
                StartShootAnimation();
            }
        }


    }
    bool isShooting = false;
    void StartShootAnimation()
    {
        if (isShooting) return;
        isShooting = true;
        for (int i = 0; i < shootingPosition.Count; i++)
        {
            var bullet2 = Instantiate(myBullet);
            bullet2.transform.position = shootingPosition[i].position;
            Vector2 shootDirection = (shootingPosition[i].position - transform.position).normalized;
            bullet2.GetComponent<Bullet>().Shoot(shootDirection, myColor);

        }
        transform.DOPunchScale(originalScale / 1.1f, shootInterval, 0, 0).
       OnComplete(() =>
       {
           isShooting = false;
       });

    }


    public void Hurt(float damage, Vector3 dir, Vector2 damTexPos)
    {
        if (health <= 0) return;
        health -= damage;
        var d = (transform.position - dir).normalized;
        transform.position += d * 0.3f;
        WorldCanvas.Instance.ShowDamageText(damTexPos, damage, Color.black);
        AudioManager.Instance.PlayHittingSound();

        SetHealth();
        if (health <= 0)
        {
            GameManager.Instance.GiveCurrency(this.transform.position);
            Finish();
            transform.DOPunchScale(transform.localScale * Random.Range(1.1f, 1.5f), 0.1f, 2, 0.5f).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                transform.DOScale(0, 0.1f).SetEase(Ease.Flash)
                .OnComplete(() =>
                {

                    var part = Instantiate(GameManager.Instance.destructionParticles[Random.Range(0, GameManager.Instance.destructionParticles.Count - 1)]);
                    part.transform.position = this.transform.position;
                    part.Play();
                    Destroy(part.gameObject, 5f);
                    Destroy(this.gameObject);
                })
                ;
            });
        }
    }

    void Finish()
    {
        GameManager.Instance.OnEnemyDeath(this.gameObject);
    }

    public void SetMaxHealth(float givenHealth)
    {
        maxHealth = givenHealth;
    }

    public void SetStopDistance(float distance)
    {
        stopDistance = distance;
    }

    public void SetShootInterval(float interval)
    {
        shootInterval = interval;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}