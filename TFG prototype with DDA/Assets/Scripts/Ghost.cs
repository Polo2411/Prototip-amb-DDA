using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 3f; // Velocidad del enemigo
    public int maxHealth = 60; // Vida máxima del enemigo
    public int damagePerProjectile = 20; // Daño recibido por proyectil
    public int damagePerChargedProjectile = 60; // Daño recibido por proyectil cargado
    private int currentHealth; // Vida actual del enemigo
    private Transform player; // Referencia al jugador
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer

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
            AudioManager.instance.PlaySound(AudioManager.instance.defeatSound, 0.3f, 1f);
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

        spriteRenderer.color = Color.white; // Restaurar el color original (asumiendo que era blanco)
    }
}
