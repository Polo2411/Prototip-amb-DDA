using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMonster : MonoBehaviour
{
    public float speed = 3f; // Velocidad del enemigo
    public int maxHealth = 80; // Vida máxima del enemigo
    public int damagePerProjectile = 20; // Daño recibido por proyectil
    public int damagePerChargedProjectile = 60; // Daño recibido por proyectil cargado
    private int currentHealth; // Vida actual del enemigo
    private Transform player; // Referencia al jugador
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private bool isVulnerable = false; // Estado de vulnerabilidad del enemigo

    void Start()
    {
        // Encuentra el objeto del jugador por su etiqueta
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Inicializa la vida actual
        currentHealth = maxHealth;

        // Obtiene el componente SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        SceneManagerScript.instance.RegisterEnemy();
    }

    void Update()
    {
        // Si hay un jugador al que seguir
        if (player != null)
        {
            // Dirección hacia el jugador
            Vector2 direction = (player.position - transform.position).normalized;

            // Movimiento hacia el jugador
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isVulnerable)
        {
            if (other.CompareTag("Projectile"))
            {
                // Reducir la vida del enemigo
                TakeDamage(damagePerProjectile);
            }
            else if (other.CompareTag("ChargedProjectile"))
            {
                // Reducir la vida del enemigo
                TakeDamage(damagePerChargedProjectile);
            }
        }
        else if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null && playerScript.IsDashing()) // Asegúrate de que el Player tenga un método IsDashing()
            {
                StartCoroutine(BecomeVulnerable());
            }
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Cambiar el color del sprite a rojo por 0.3 segundos
        StartCoroutine(FlashRed());

        // Verificar si la vida ha llegado a cero o menos
        if (currentHealth <= 0)
        {
            SceneManagerScript.instance.EnemyKilled();
            // Destruir el enemigo
            Destroy(gameObject);
            AudioManager.instance.PlaySound(AudioManager.instance.defeatSound, 0.3f, 0.4f);
        }
        else
        {
            AudioManager.instance.PlaySound(AudioManager.instance.hitSound, 0.2f, 0.4f);
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.3f);

        spriteRenderer.color = Color.cyan; // Restaurar el color de vulnerabilidad
    }

    private IEnumerator BecomeVulnerable()
    {
        isVulnerable = true;
        spriteRenderer.color = Color.cyan; // Cambiar el color del sprite a azul
        AudioManager.instance.PlaySound(AudioManager.instance.FireMonster, 0.3f, 0.2f);

        // Esperar 3 segundos mientras el enemigo es vulnerable
        yield return new WaitForSeconds(3f);

        isVulnerable = false;
        spriteRenderer.color = Color.white; // Restaurar el color original (asumiendo que era blanco)
    }
}
