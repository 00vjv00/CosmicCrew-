// ============================================================================
// ATTACKING STATE - Atacando al player
// Archivo: Assets/Scripts/Enemies/States/AttackingState.cs
// Descripción: Estado de ataque activo
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.States
{
    /// <summary>
    /// Estado donde el robot está en combate activo
    /// Realiza ataques y puede volver a PursuingState si el player escapa
    /// </summary>
    public class AttackingState : IEnemyState
    {
        private float attackCooldown = 0f;
        private const float ATTACK_COOLDOWN_TIME = 1.5f; // Cada 1.5 segundos

        public void OnEnter(Enemy enemy)
        {
            attackCooldown = 0f;
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} en AttackingState (atacando)");
        }

        public void OnUpdate(Enemy enemy)
        {
            if (enemy.PlayerTransform == null)
            {
                enemy.ChangeState(new IdleState());
                return;
            }

            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.PlayerTransform.position);
            
            // Si se aleja demasiado, volver a perseguir
            const float ATTACK_RANGE = 8f;
            if (distanceToPlayer > ATTACK_RANGE)
            {
                enemy.ChangeState(new PursuingState());
                return;
            }

            // Atacar
            attackCooldown += Time.deltaTime;
            if (attackCooldown >= ATTACK_COOLDOWN_TIME)
            {
                attackCooldown = 0f;
                PerformAttack(enemy);
            }
        }

        public void OnExit(Enemy enemy)
        {
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} sale de AttackingState");
        }

        /// <summary>
        /// Realiza un ataque contra el player
        /// </summary>
        private void PerformAttack(Enemy enemy)
        {
            // Aquí iría la lógica de ataque específica
            // Por ahora es un placeholder
            Debug.Log($"[Enemy] {enemy.EnemyType} ataca a {enemy.PlayerTransform.gameObject.name}");
            
            // TODO: Instanciar proyectil, raycast, etc
        }
    }
}
