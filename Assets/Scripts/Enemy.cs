using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IEnemy
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public float stopDistance = 2f;
    [SerializeField] Transform shootingPosition;
    [SerializeField] public float shootInterval = 2f;
    [SerializeField] Image healthBar;
    GameObject myBullet;
    public float health;
    public float maxHealth = 100;
    Color myColor;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        SetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myBullet = GameManager.Instance.bullets[Random.Range(0, GameManager.Instance.bullets.Count - 1)];
        myColor = GameManager.Instance.colors[Random.Range(0, GameManager.Instance.colors.Count - 1)];
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

            transform.DOScale(Vector2.one, 0.5f);
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
        if (distanceToPlayer < stopDistance * 1.25f)
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
        var bullet = Instantiate(myBullet);
        bullet.transform.position = shootingPosition.position;
        bullet.GetComponent<Bullet>().Shoot(Player.Instance.transform.position - shootingPosition.position, myColor);
        transform.DOPunchScale(new Vector2(0.5f, 0.5f), shootInterval, 0, 0).OnComplete(() =>
        {
            isShooting = false;
        });

    }


    public void Hurt(float damage, Vector3 dir)
    {
        health -= damage;
        var d = (transform.position - dir).normalized;
        transform.position += d * 0.3f;
        WorldCanvas.Instance.ShowDamageText(this.transform.position, damage);

        SetHealth();
        if (health <= 0)
        {
            Finish();
            transform.DOPunchScale(transform.localScale * Random.Range(1.1f, 1.5f), 0.1f, 2, 0.5f).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                transform.DOScale(0, 0.1f).SetEase(Ease.Flash)
                .OnComplete(() =>
                {
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
}