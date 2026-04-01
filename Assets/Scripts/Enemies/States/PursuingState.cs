// ============================================================================
// PURSUING STATE - Persiguiendo al player
// Archivo: Assets/Scripts/Enemies/States/PursuingState.cs
// Descripción: Estado activo de persecución
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.States
{
    /// <summary>
    /// Estado donde el robot persigue activamente al player
    /// Puede transicionar a AttackingState si está en rango de ataque
    /// O volver a IdleState si pierde de vista al player
    /// </summary>
    public class PursuingState : IEnemyState
    {
        private float lostTargetTimer = 0f;
        private const float LOST_TARGET_TIMEOUT = 5f; // 5 segundos sin ver al player = volver a Idle

        public void OnEnter(Enemy enemy)
        {
            enemy.SetVisibility(true);
            lostTargetTimer = 0f;
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} en PursuingState (persiguiendo)");
        }

        public void OnUpdate(Enemy enemy)
        {
            if (enemy.PlayerTransform == null)
            {
                enemy.ChangeState(new IdleState());
                return;
            }

            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.PlayerTransform.position);
            
            // Si está en rango de ataque
            const float ATTACK_RANGE = 5f;
            if (distanceToPlayer <= ATTACK_RANGE)
            {
                enemy.ChangeState(new AttackingState());
                return;
            }

            // Si el player salió del rango de persecución
            const float PURSUIT_RANGE = 40f;
            if (distanceToPlayer > PURSUIT_RANGE)
            {
                lostTargetTimer += Time.deltaTime;
                if (lostTargetTimer >= LOST_TARGET_TIMEOUT)
                {
                    enemy.ChangeState(new IdleState());
                    return;
                }
            }
            else
            {
                lostTargetTimer = 0f; // Reset timer si sigue viéndolo
            }

            // Moverse hacia el player
            Vector3 directionToPlayer = (enemy.PlayerTransform.position - enemy.transform.position).normalized;
            enemy.transform.position += directionToPlayer * enemy.MovementSpeed * Time.deltaTime;
        }

        public void OnExit(Enemy enemy)
        {
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} sale de PursuingState");
        }
    }
}
