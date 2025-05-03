using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    TrailRenderer trailRenderer;
    Rigidbody2D rigidbody2D;

    bool canHitSelfType = false;


    public void Shoot(Vector2 dir, Color randomColor)
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        // Generate random color

        // Set color for both sprite and trail
        spriteRenderer.color = randomColor;
        trailRenderer.startColor = randomColor;
        trailRenderer.endColor = new Color(randomColor.r, randomColor.g, randomColor.b, 0f);

        // Shoot the bullet in the specified direction with given speed
        rigidbody2D.velocity = dir.normalized * GameManager.Instance.bulletShootSpeed;
        // Destroy(this, 50f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            if (player.isShieldOn)
            {
                canHitSelfType = true;
                var part = Instantiate(GameManager.Instance.playerCollidParticle);
                part.transform.position = this.transform.position;
                part.Play();
                Destroy(part.gameObject, 2f);
                // Get the current velocity direction
                // Reflect the direction
                Vector2 reflectedDirection = transform.position - Player.Instance.transform.position;
                rigidbody2D.velocity = reflectedDirection.normalized * GameManager.Instance.bulletShootSpeed;

            }
            else
            {
                var part = Instantiate(GameManager.Instance.bulletHitParticle);
                part.transform.position = this.transform.position;
                part.Play();
                Destroy(part.gameObject, 1f);
                player.Hurt(GameManager.Instance.currentWave.bulletDamage, this.transform.position);
                //Debug.LogError("hurting player");
                Destroy(this.gameObject);
            }

        }
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy) && canHitSelfType)
        {
            var part = Instantiate(GameManager.Instance.bulletHitParticle);
            part.transform.position = this.transform.position;
            part.Play();
            Destroy(part.gameObject, 1f);
            enemy.Hurt(GameManager.Instance.currentWave.bulletDamage*5f, this.transform.position);
            Destroy(this.gameObject);
        }

    }
}
