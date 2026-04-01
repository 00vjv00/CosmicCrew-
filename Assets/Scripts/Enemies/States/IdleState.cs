// ============================================================================
// IDLE STATE - Patrullando/Esperando
// Archivo: Assets/Scripts/Enemies/States/IdleState.cs
// Descripción: Estado de patrulla o espera en zona asignada
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.States
{
    /// <summary>
    /// Estado donde el robot está en su zona asignada, patrullando o esperando
    /// Busca al player y transiciona a PursuingState si lo detecta
    /// </summary>
    public class IdleState : IEnemyState
    {
        private float detectionCheckTimer = 0f;
        private const float DETECTION_CHECK_INTERVAL = 0.3f; // Revisar cada 0.3 segundos

        public void OnEnter(Enemy enemy)
        {
            enemy.SetVisibility(true);
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} en IdleState (patrullando zona)");
        }

        public void OnUpdate(Enemy enemy)
        {
            // Check de detección periódico
            detectionCheckTimer += Time.deltaTime;
            if (detectionCheckTimer >= DETECTION_CHECK_INTERVAL)
            {
                detectionCheckTimer = 0f;
                CheckForPlayer(enemy);
            }

            // Patrulla o animación idle
            // Aquí iría lógica de patrulla simple (animate o rotate)
        }

        public void OnExit(Enemy enemy)
        {
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} sale de IdleState");
        }

        /// <summary>
        /// Revisa si el player está detectado en rango
        /// </summary>
        private void CheckForPlayer(Enemy enemy)
        {
            if (enemy.PlayerTransform == null)
                return;

            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.PlayerTransform.position);
            
            // Radio de detección del enemigo
            const float DETECTION_RANGE = 35f; // Ajustar según necesidad
            
            if (distanceToPlayer <= DETECTION_RANGE)
            {
                // Cambiar a PursuingState
                enemy.ChangeState(new PursuingState());
            }
        }
    }
}
