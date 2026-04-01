// ============================================================================
// STORE STATE - En Retaguardia esperando despliegue
// Archivo: Assets/Scripts/Enemies/States/StoreState.cs
// Descripción: Estado de espera en Tropas/Ingeniería
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.States
{
    /// <summary>
    /// Estado donde el robot espera en la retaguardia (Tropas/Ingeniería)
    /// Inactivo, esperando a ser desplegado como reemplazo
    /// </summary>
    public class StoreState : IEnemyState
    {
        public void OnEnter(Enemy enemy)
        {
            // Desactivar visibilidad
            enemy.SetVisibility(false);
            
            // Desactivar colisionadores
            var colliders = enemy.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            // Log para debug
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} entra en StoreState (esperando en retaguardia)");
        }

        public void OnUpdate(Enemy enemy)
        {
            // No hay lógica activa en Store
            // El EnemyManager es responsable de cambiar de estado
        }

        public void OnExit(Enemy enemy)
        {
            // Reactivar colisionadores
            var colliders = enemy.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }

            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} sale de StoreState (iniciando despliegue)");
        }
    }
}
