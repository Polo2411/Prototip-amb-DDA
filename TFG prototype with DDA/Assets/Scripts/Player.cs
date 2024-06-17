using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;
    public float chargedCooldown = 3f;
    public float chargeTime = 2f;
    public Transform playerSprite;
    public TrailRenderer dashTrail;
    public GameObject projectilePrefab;
    public GameObject chargedProjectilePrefab;
    public float fireRate = 0.5f;
    public int maxLives = 3;
    public TMP_Text playerHealthText;
    public Image dashCooldownImage;
    public Image chargedProjectileCooldownImage;
    //DDA
    public int deathCounter;
    public int hitsCounter;
    public float timeLevel;
    public int dashCounter;
    public int chargedCounter;


    private Rigidbody2D rb;
    private Camera mainCamera;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isCharging = false;
    private bool canShootCharged = true;
    private bool isInvincible = false;
    private Vector2 dashDirection;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float nextFireTime = 0f;
    private int currentLives;
    private float originalFireRate;
    private float originalDashCooldown;
    private float originalChargedCooldown;
    private float originalChargeTime;

    void Start()
    {
        // Play background music
        BackgroundMusic.instance.PlayBackgroundMusic();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        dashTrail.emitting = false;
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (SceneManager.GetActiveScene().name == "Level1")
        {
            currentLives = maxLives;
        }
        else
        {
            currentLives = PlayerPrefs.GetInt("PlayerLives", maxLives);
        }
        //DDA
        deathCounter = PlayerPrefs.GetInt("Deaths", 0);
        hitsCounter = PlayerPrefs.GetInt("Hits", 0);
        dashCounter = PlayerPrefs.GetInt("Dash", 0);
        chargedCounter = PlayerPrefs.GetInt("Charged", 0);

        originalFireRate = fireRate;
        originalDashCooldown = dashCooldown;
        originalChargedCooldown = chargedCooldown;
        originalChargeTime = chargeTime;
        UpdateHealthUI();
        UpdateCooldownUI();
    }

    void Update()
    {
        timeLevel += Time.deltaTime;
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;

        if (!isDashing && !isCharging)
        {
            rb.velocity = movement * moveSpeed;
        }

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        angle -= 90f;

        if (playerSprite != null)
        {
            playerSprite.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash(movement));
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootProjectile();
        }

        if (Input.GetButtonDown("Fire2") && canShootCharged)
        {
            StartCoroutine(ChargeAndShootProjectile());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Guardar la vida del jugador antes de cambiar de escena
            PlayerPrefs.SetInt("PlayerLives", currentLives);
            PlayerPrefs.Save();
            BackgroundMusic.instance.StopBackgroundMusic();
            SceneManagerScript.instance.ReturnToMenu();
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectile.GetComponent<DefaultProjectile>().speed;
        AudioManager.instance.PlaySound(AudioManager.instance.shootSound, 0.2f, 0.2f);
    }

    private IEnumerator Dash(Vector2 direction)
    {
        dashCounter++;
        isDashing = true;
        canDash = false;
        isInvincible = true;
        dashTrail.emitting = true;
        rb.velocity = direction * dashSpeed;
        spriteRenderer.color = new Color(0.3f, 0.5f, 1f);
        dashCooldownImage.color = Color.gray;
        AudioManager.instance.PlaySound(AudioManager.instance.dashSound, 0.2f, 0.3f);

        yield return new WaitForSeconds(dashDuration);

        dashTrail.emitting = false;
        rb.velocity = Vector2.zero;
        isDashing = false;
        spriteRenderer.color = originalColor;
        isInvincible = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        dashCooldownImage.color = Color.white;
    }

    private IEnumerator ChargeAndShootProjectile()
    {
        chargedCounter++;
        isCharging = true;
        canShootCharged = false;
        rb.velocity = Vector2.zero;
        spriteRenderer.color = Color.green;
        chargedProjectileCooldownImage.color = Color.gray;
        AudioManager.instance.PlaySound(AudioManager.instance.chargingSound, 0.2f, 2f);

        yield return new WaitForSeconds(chargeTime);

        GameObject chargedProjectile = Instantiate(chargedProjectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        chargedProjectile.GetComponent<Rigidbody2D>().velocity = direction * chargedProjectile.GetComponent<ChargedProjectile>().speed;
        spriteRenderer.color = originalColor;
        isCharging = false;
        AudioManager.instance.PlaySound(AudioManager.instance.chargedShootSound, 0.2f, 0.2f);

        yield return new WaitForSeconds(chargedCooldown);
        canShootCharged = true;
        chargedProjectileCooldownImage.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ghost") && !isInvincible)
        {
            StartCoroutine(TakeDamage());
        }
        else if (other.CompareTag("HealthPotion"))
        {
            currentLives = maxLives;
            PlayerPrefs.SetInt("PlayerLives", currentLives);
            PlayerPrefs.Save();
            UpdateHealthUI();
            Destroy(other.gameObject);
            AudioManager.instance.PlaySound(AudioManager.instance.ObjectTook, 0.2f, 0.2f);
        }
        else if (other.CompareTag("Attack speed potion"))
        {
            StartCoroutine(AttackSpeedBoost());
            Destroy(other.gameObject);
            AudioManager.instance.PlaySound(AudioManager.instance.ObjectTook, 0.2f, 0.2f);
            BackgroundMusic.instance.SpeedUpBackgroundMusic(1.2f); // Speed up background music
        }
        else if (other.CompareTag("CooldownBoost"))
        {
            StartCoroutine(CooldownBoost());
            Destroy(other.gameObject);
            AudioManager.instance.PlaySound(AudioManager.instance.ObjectTook, 0.2f, 0.2f);
            BackgroundMusic.instance.SpeedUpBackgroundMusic(1.2f); // Speed up background music
        }
    }

    private IEnumerator AttackSpeedBoost()
    {
        fireRate = 0.1f; // Reduce the fire rate to 0.1 seconds
        yield return new WaitForSeconds(5f); // Duration of the effect
        fireRate = originalFireRate; // Restore the original fire rate
        BackgroundMusic.instance.RestoreBackgroundMusicSpeed(); // Restore original background music speed
    }

    private IEnumerator CooldownBoost()
    {
        dashCooldown = 1f; // Reduce dash cooldown to 1 second
        chargedCooldown = 1f; // Reduce charged projectile cooldown to 1 second
        chargeTime = 1f;
        UpdateCooldownUI();

        yield return new WaitForSeconds(10f); // Duration of the effect

        dashCooldown = originalDashCooldown; // Restore original dash cooldown
        chargedCooldown = originalChargedCooldown; // Restore original charged projectile cooldown
        chargeTime = originalChargeTime;
        UpdateCooldownUI();
        BackgroundMusic.instance.RestoreBackgroundMusicSpeed(); // Restore original background music speed
    }

    private IEnumerator TakeDamage()
    {
        currentLives--;
        hitsCounter++;
        PlayerPrefs.SetInt("PlayerLives", currentLives);
        PlayerPrefs.Save();

        isInvincible = true;
        spriteRenderer.color = Color.red;
        UpdateHealthUI();

        if (currentLives <= 0)
        {
            deathCounter++;
            Debug.Log("Player Died");
            AudioManager.instance.PlaySound(AudioManager.instance.deathSound, 0.2f, 0.8f);
            timeLevel = 47f;
            SceneManagerScript.instance.ResetGame();
        }
        else
        {
            AudioManager.instance.PlaySound(AudioManager.instance.damageSound, 0.2f, 0.3f);
        }

        yield return new WaitForSeconds(2f);

        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    public void UpdateHealthUI()
    {
        playerHealthText.text = "Lives: " + currentLives;
    }

    private void UpdateCooldownUI()
    {
        dashCooldownImage.color = canDash ? Color.white : Color.gray;
        chargedProjectileCooldownImage.color = canShootCharged ? Color.white : Color.gray;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public void SetCurrentLives(int num)
    {
        currentLives = num;
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}
