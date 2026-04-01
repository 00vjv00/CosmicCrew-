// ============================================================================
// ENEMY STATE INTERFACE
// Archivo: Assets/Scripts/Enemies/States/IEnemyState.cs
// Descripción: Interfaz para todos los estados de enemigos
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.States
{
    /// <summary>
    /// Interfaz base para todos los estados de enemigos
    /// Cada estado implementa su propia lógica de entrada, actualización y salida
    /// </summary>
    public interface IEnemyState
    {
        /// <summary>
        /// Se llama cuando el estado se activa
        /// </summary>
        void OnEnter(Enemy enemy);

        /// <summary>
        /// Se llama cada frame mientras el estado está activo
        /// </summary>
        void OnUpdate(Enemy enemy);

        /// <summary>
        /// Se llama cuando el estado se desactiva
        /// </summary>
        void OnExit(Enemy enemy);
    }
}
