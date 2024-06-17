using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultProjectile : MonoBehaviour
{
    public float speed = 20f; // Velocidad del proyectil
    public float lifetime = 0.3f; // Tiempo de vida del proyectil en segundos

    private Rigidbody2D rb;
    private Vector2 shootDirection; // Dirección de disparo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtener la posición del cursor del ratón
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calcular la dirección desde el jugador hacia el cursor del ratón
        shootDirection = (mousePos - (Vector2)transform.position).normalized;

        // Aplicar velocidad constante al proyectil en la dirección calculada
        rb.velocity = shootDirection * speed;

        // Destruir el proyectil después de su tiempo de vida
        Destroy(gameObject, lifetime);

        // Calcular el ángulo de rotación en radianes
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x);

        // Convertir el ángulo a grados y rotar el sprite
        float angleDegrees = angle * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleDegrees - 90);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destruir el proyectil al colisionar con cualquier cosa
        Destroy(gameObject);
    }
}
