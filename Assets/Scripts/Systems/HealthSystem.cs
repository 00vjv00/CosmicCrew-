// ============================================================================
// HEALTH SYSTEM
// Cosmic Crew - Sistema de vida para jugador y enemigos
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de vida simple y event-driven
/// Se agrega a cualquier GameObject que deba tener HP (player, enemigos, etc)
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool debugMode = false;
    
    private float currentHealth;
    private bool isAlive = true;
    
    // ────────────────────────────────────────────────────────
    // EVENTOS - Públicos para que otros sistemas se suscriban
    // ────────────────────────────────────────────────────────
    
    /// <summary>Disparado cuando la salud cambia</summary>
    public System.Action<float, float> OnHealthChanged;  // (newHealth, maxHealth)
    
    /// <summary>Disparado cuando recibe daño</summary>
    public System.Action<float> OnDamageTaken;  // (damageAmount)
    
    /// <summary>Disparado cuando muere</summary>
    public System.Action OnDeath;
    
    /// <summary>Disparado cuando se cura</summary>
    public System.Action<float> OnHealed;  // (healAmount)
    
    // ────────────────────────────────────────────────────────
    // PROPIEDADES
    // ────────────────────────────────────────────────────────
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercent => currentHealth / maxHealth;
    public bool IsAlive => isAlive;
    
    // ════════════════════════════════════════════════════════════════
    // INICIALIZACIÓN
    // ════════════════════════════════════════════════════════════════
    
    private void Awake()
    {
        currentHealth = maxHealth;
        
        if (debugMode)
            Debug.Log($"[HEALTH] {gameObject.name} inicializado con {currentHealth}/{maxHealth} HP");
    }
    
    // ════════════════════════════════════════════════════════════════
    // ACCIONES: DAÑO Y CURACIÓN
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>Aplicar daño al objeto</summary>
    public void TakeDamage(float damageAmount)
    {
        if (!isAlive)
            return;
        
        if (damageAmount < 0)
            damageAmount = 0;
        
        currentHealth -= damageAmount;
        OnDamageTaken?.Invoke(damageAmount);
        
        if (debugMode)
            Debug.Log($"[HEALTH] {gameObject.name} recibió {damageAmount} daño. HP: {currentHealth}/{maxHealth}");
        
        // Broadcaster del cambio de salud
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Si muere
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    
    /// <summary>Curar al objeto</summary>
    public void Heal(float healAmount)
    {
        if (!isAlive || healAmount <= 0)
            return;
        
        float oldHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        
        float actualHeal = currentHealth - oldHealth;
        OnHealed?.Invoke(actualHeal);
        
        if (debugMode)
            Debug.Log($"[HEALTH] {gameObject.name} curado {actualHeal} HP. HP: {currentHealth}/{maxHealth}");
        
        // Broadcaster del cambio
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>Establecer salud a un valor específico</summary>
    public void SetHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
            Die();
    }
    
    /// <summary>Morir</summary>
    private void Die()
    {
        if (!isAlive)
            return;
        
        isAlive = false;
        OnDeath?.Invoke();
        
        if (debugMode)
            Debug.Log($"[HEALTH] {gameObject.name} ha muerto");
        
        // Aquí iría lógica de muerte (desaparecer, animación, etc)
        // Por ahora solo desactivamos
        gameObject.SetActive(false);
    }
    
    /// <summary>Resucitar (para debug/testing)</summary>
    public void Revive(float health = -1)
    {
        isAlive = true;
        currentHealth = health > 0 ? health : maxHealth;
        gameObject.SetActive(true);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (debugMode)
            Debug.Log($"[HEALTH] {gameObject.name} resucitado con {currentHealth} HP");
    }
    
    // ════════════════════════════════════════════════════════════════
    // DEBUG
    // ════════════════════════════════════════════════════════════════
    
    public override string ToString()
    {
        return $"{gameObject.name}: {currentHealth}/{maxHealth} HP ({HealthPercent * 100:F1}%) - {(isAlive ? "VIVO" : "MUERTO")}";
    }
}
