// ============================================================================
// ENEMY NAVIGATION SYSTEM - Sistema de navegación waypoints
// Archivo: Assets/Scripts/Enemies/EnemyNavigationSystem.cs
// Descripción: Maneja el movimiento del robot entre waypoints
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

namespace CosmicCrew.Enemies
{
    /// <summary>
    /// Sistema simple de navegación por waypoints
    /// El robot sigue una ruta lineal desde retaguardia → teletransportador → zona asignada
    /// </summary>
    public class EnemyNavigationSystem
    {
        private List<Vector3> currentRoute;
        private int currentWaypointIndex = 0;
        private const float WAYPOINT_REACH_DISTANCE = 1f;

        /// <summary>
        /// Inicializa la ruta del robot desde retaguardia hasta su zona asignada
        /// </summary>
        public void InitializeRoute(Enemy enemy)
        {
            currentRoute = new List<Vector3>();
            currentWaypointIndex = 0;

            // Aquí iría la lógica de route-building
            // Por ahora es un placeholder
            // TODO: Implementar sistema de waypoints o pathfinding
            
            // Agregamos al menos el destino final
            if (enemy.AssignedZone != null)
            {
                currentRoute.Add(enemy.AssignedZone.position);
            }
        }

        /// <summary>
        /// Actualiza el movimiento del robot hacia el siguiente waypoint
        /// </summary>
        public void UpdateMovement(Enemy enemy)
        {
            if (currentRoute == null || currentRoute.Count == 0)
                return;

            Vector3 targetWaypoint = currentRoute[currentWaypointIndex];
            Vector3 directionToWaypoint = (targetWaypoint - enemy.transform.position).normalized;

            // Mover hacia el waypoint
            enemy.transform.position += directionToWaypoint * enemy.MovementSpeed * Time.deltaTime;

            // Revisar si llegó al waypoint
            if (Vector3.Distance(enemy.transform.position, targetWaypoint) <= WAYPOINT_REACH_DISTANCE)
            {
                currentWaypointIndex++;
            }
        }

        /// <summary>
        /// Verifica si el robot llegó a su destino final
        /// </summary>
        public bool HasReachedDestination(Enemy enemy)
        {
            if (currentRoute == null || currentRoute.Count == 0)
                return false;

            return currentWaypointIndex >= currentRoute.Count;
        }

        /// <summary>
        /// Limpia la ruta actual
        /// </summary>
        public void ClearRoute()
        {
            currentRoute?.Clear();
            currentWaypointIndex = 0;
        }
    }
}
