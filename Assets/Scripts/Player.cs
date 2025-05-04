using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour, IPlayer
{
    private static Player instance;
    public static Player Instance => instance;

    public bool IsShieldOn => isShieldOn;

    public float moveSpeed = 5f; // প্লেয়ারের গতি
    public Image healthBar;
    private Rigidbody2D rb;
    private Vector2 movement;

    private Tween scaleTween; // স্কেল টুইন ট্র্যাক করার জন্য
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;
    public float teleportDistance = 1.5f; // টেলিপোর্ট কতদূর যাবে
    public float teleportHideTime = 0.15f; // কতক্ষণ অদৃশ্য থাকবে
    public bool isShieldOn = false;
    public float health;
    float maxHealth = 500;
    public float maxShieldTime = 5f;
    public ParticleSystem shieldParticle;
    public Image shieldBarImage;
    public TextMeshProUGUI shieldBarSeconds;
    float elapsedShieldTime;


    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartBreathing(); // শুরুতেই ব্রিদিং
        health = maxHealth;
        SetHealth();
        elapsedShieldTime = maxShieldTime;
        SetShieldTimer();
    }

    void SetHealth()
    {
        healthBar.fillAmount = health / maxHealth;
    }

    void SetShieldTimer()
    {

        elapsedShieldTime = Mathf.Min(elapsedShieldTime, maxShieldTime); // সর্বাধিক সময়ের চেয়ে বেশি হলে সেটিকে সর্বাধিক সময়ের সমান করুন
        shieldBarSeconds.text = $"{elapsedShieldTime:F2} / {maxShieldTime:F2}"; // দুই দশমিক স্থানে রূপান্তর

    }

    // Update is called once per frame
    void Update()
    {
        // ইনপুট নেওয়া
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized; // ডায়াগোনাল মুভমেন্টে গতি ঠিক রাখতে

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.L))
        {
            if (elapsedShieldTime > 0)
            {
                isShieldOn = true;
                elapsedShieldTime -= Time.deltaTime;
                shieldBarImage.fillAmount = elapsedShieldTime / maxShieldTime;
                SetShieldTimer();
                if (!shieldParticle.isPlaying)
                {
                    shieldParticle.Play();
                }
            }
            else
            {
                isShieldOn = false;
                if (shieldParticle.isPlaying)
                {
                    shieldParticle.Stop();
                }
            }
        }
        else
        {
            isShieldOn = false;
            if (elapsedShieldTime < maxShieldTime)
            {
                if (shieldParticle.isPlaying)
                {
                    shieldParticle.Stop();
                }
                elapsedShieldTime += Time.deltaTime * 0.5f; // শিল্ড রিচার্জ হবে আস্তে আস্তে
                shieldBarImage.fillAmount = elapsedShieldTime / maxShieldTime;
                SetShieldTimer();
            }
        }

        // মুভিং চেক
        bool currentlyMoving = movement.sqrMagnitude > 0.01f;
        if (currentlyMoving && !isMoving)
        {
            isMoving = true;
            StartDirectionalSquish();
        }
        else if (!currentlyMoving && isMoving)
        {
            isMoving = false;
            StartBreathing();
        }
        // মুভিং অবস্থায় ডিরেকশন চেঞ্জ করলে অ্যানিমেশনও চেঞ্জ হবে
        else if (currentlyMoving && isMoving)
        {
            StartDirectionalSquish();
        }

        // স্পেস প্রেসে টেলিপোর্ট
        if (Input.GetKeyDown(KeyCode.Space) && movement.sqrMagnitude > 0.01f)
        {
            TeleportInDirection();
        }
    }

    void FixedUpdate()
    {
        // Rigidbody2D দিয়ে মুভমেন্ট
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        shieldParticle.gameObject.transform.position = transform.position;
    }

    void StartDirectionalSquish()
    {
        if (scaleTween != null) scaleTween.Kill();
        // ডায়াগোনাল মুভমেন্ট
        if (Mathf.Abs(movement.x) > 0.01f && Mathf.Abs(movement.y) > 0.01f)
        {
            // ডায়াগোনাল: X ও Y দুটোই একটু squish/stretch
            float xSquish = movement.x > 0 ? 1.1f : 0.9f;
            float ySquish = movement.y > 0 ? 1.1f : 0.9f;
            scaleTween = transform.DOScale(new Vector3(xSquish, ySquish, 1f), 0.1f)
                .OnComplete(() =>
                    scaleTween = transform.DOScale(Vector3.one, 0.15f)
                );
        }
        // Horizontal dominant
        else if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            // বামে-ডানে: X বড়, Y ছোট
            scaleTween = transform.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.1f)
                .OnComplete(() =>
                    scaleTween = transform.DOScale(Vector3.one, 0.15f)
                );
        }
        // Vertical dominant
        else if (Mathf.Abs(movement.y) > 0f)
        {
            // উপরে-নিচে: X ছোট, Y বড়
            scaleTween = transform.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.1f)
                .OnComplete(() =>
                    scaleTween = transform.DOScale(Vector3.one, 0.15f)
                );
        }
    }

    void StartBreathing()
    {
        if (scaleTween != null) scaleTween.Kill();
        // Breathing effect: scale oscillate
        scaleTween = transform.DOScale(new Vector3(1.05f, 0.95f, 1f), 0.7f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void TeleportInDirection()
    {
        var teliParticle = Instantiate(GameManager.Instance.telePortParticle);
        teliParticle.transform.position = this.transform.position;
        teliParticle.Play();
        Destroy(teliParticle.gameObject, 10f);
        if (scaleTween != null) scaleTween.Kill();
        // টেলিপোর্টের আগে squash/stretch + fade out
        Sequence teleportSeq = DOTween.Sequence();
        teleportSeq.Append(transform.DOScale(new Vector3(1.3f, 0.7f, 1f), 0.08f)); // squash
        teleportSeq.Append(transform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.07f)); // vanish
        teleportSeq.Join(spriteRenderer.DOFade(0f, 0.07f)); // fade out
        teleportSeq.AppendCallback(() =>
        {
            // টার্গেট পজিশনে টেলিপোর্ট
            rb.position = rb.position + movement * teleportDistance;
        });
        teleportSeq.AppendInterval(teleportHideTime * 0.5f); // অল্প সময় অদৃশ্য
        teleportSeq.Append(spriteRenderer.DOFade(1f, 0.09f)); // fade in
        teleportSeq.Join(transform.DOScale(new Vector3(1.2f, 1.3f, 1f), 0.09f)); // bounce overshoot
        teleportSeq.Append(transform.DOScale(Vector3.one, 0.12f).SetEase(Ease.OutBack)); // bounce back
        teleportSeq.OnStart(() => { if (spriteRenderer != null) spriteRenderer.enabled = true; });
    }
    public void Hurt(float damage, Vector3 dir)
    {
        health -= damage;
        var d = (transform.position - dir).normalized;
        transform.position += d * 0.3f;
        SetHealth();
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            var part = Instantiate(GameManager.Instance.playerCollidParticle);
            part.transform.position = this.transform.position;
            part.Play();
            Destroy(part.gameObject, 2f);
            var d = (transform.position - enemy.transform.position).normalized;
            transform.position += d * 0.8f;
            ShakeEffect();
            health -= 10f;
            SetHealth();
        }
    }

    void ShakeEffect()
    {
        // Scale shake effect using DOTween
        transform.DOShakeScale(0.2f, 0.2f, 10, 90, true)
            .OnComplete(() => transform.localScale = Vector3.one);
    }
}
