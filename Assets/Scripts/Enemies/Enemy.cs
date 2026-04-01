// ============================================================================
// ENEMY BASE CLASS
// Archivo: Assets/Scripts/Enemies/Enemy.cs
// Descripción: Clase base para todos los enemigos del juego
// ============================================================================

using UnityEngine;
using CosmicCrew.Enemies.States;

namespace CosmicCrew.Enemies
{
    /// <summary>
    /// Clase base para todos los tipos de robot enemigo
    /// Maneja state machine, movimiento, detección y destrucción
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        // ====================================================================
        // INSPECTABLES
        // ====================================================================
        
        [SerializeField] private string enemyType = "RobotGuard";
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private int health = 50;
        [SerializeField] private int maxHealth = 50;

        // ====================================================================
        // COMPONENTES INTERNOS
        // ====================================================================
        
        private IEnemyState currentState;
        private Renderer enemyRenderer;
        private Collider[] colliders;
        private Transform playerTransform;
        private Transform assignedZone;
        private EnemyNavigationSystem navigationSystem;

        // ====================================================================
        // PROPIEDADES PÚBLICAS
        // ====================================================================

        public string EnemyType => enemyType;
        public float MovementSpeed => movementSpeed;
        public int Health => health;
        public int MaxHealth => maxHealth;
        public Transform PlayerTransform => playerTransform;
        public Transform AssignedZone => assignedZone;
        public EnemyNavigationSystem NavigationSystem => navigationSystem;

        // ====================================================================
        // EVENTOS
        // ====================================================================

        public delegate void EnemyEventHandler(Enemy enemy);
        public event EnemyEventHandler OnEnemyDestroyed;
        public event EnemyEventHandler OnStateChanged;

        // ====================================================================
        // LIFECYCLE
        // ====================================================================

        private void Awake()
        {
            // Cache de componentes
            enemyRenderer = GetComponent<Renderer>();
            colliders = GetComponents<Collider>();
            navigationSystem = new EnemyNavigationSystem();

            // Buscar player
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        private void Start()
        {
            // Iniciar en StoreState
            ChangeState(new StoreState());
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.OnUpdate(this);
            }
        }

        // ====================================================================
        // STATE MACHINE
        // ====================================================================

        /// <summary>
        /// Cambia el estado actual del enemigo
        /// </summary>
        public void ChangeState(IEnemyState newState)
        {
            // Salir del estado anterior
            currentState?.OnExit(this);

            // Entrar al nuevo estado
            currentState = newState;
            currentState.OnEnter(this);

            // Notificar evento
            OnStateChanged?.Invoke(this);
        }

        // ====================================================================
        // VISIBILIDAD
        // ====================================================================

        /// <summary>
        /// Controla la visibilidad del robot
        /// </summary>
        public void SetVisibility(bool visible)
        {
            if (enemyRenderer != null)
            {
                enemyRenderer.enabled = visible;
            }

            // Los colliders ya se manejan en los estados
        }

        // ====================================================================
        // MOVIMIENTO & POSICIONAMIENTO
        // ====================================================================

        /// <summary>
        /// Asigna la zona donde este robot debe estar
        /// </summary>
        public void SetAssignedZone(Transform zone)
        {
            assignedZone = zone;
        }

        /// <summary>
        /// Asigna el tipo de enemigo dinámicamente
        /// </summary>
        public void SetEnemyType(string type)
        {
            enemyType = type;
        }

        // ====================================================================
        // HEALTH & DAMAGE
        // ====================================================================

        /// <summary>
        /// Recibe daño y verifica si muere
        /// </summary>
        public void TakeDamage(int damage)
        {
            health -= damage;
            Debug.Log($"[Enemy] {enemyType} {gameObject.name} recibe {damage} daño. Health: {health}/{maxHealth}");

            if (health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Restaura salud
        /// </summary>
        public void Heal(int amount)
        {
            health = Mathf.Min(health + amount, maxHealth);
        }

        /// <summary>
        /// Destruye este robot y notifica al manager
        /// </summary>
        public void Die()
        {
            Debug.Log($"[Enemy] {enemyType} {gameObject.name} MUERE");
            
            ChangeState(new DeadState());
            OnEnemyDestroyed?.Invoke(this);
        }

        /// <summary>
        /// Restablece el robot a su estado inicial
        /// Usado cuando se recicla desde la tienda
        /// </summary>
        public void ResetEnemy()
        {
            health = maxHealth;
            ChangeState(new StoreState());
            gameObject.SetActive(true);
        }

        // ====================================================================
        // DEBUG
        // ====================================================================

        private void OnGUI()
        {
            // Mostrar info en pantalla para debug
            if (GUI.Button(new Rect(10, 10, 200, 30), $"{enemyType}: {currentState?.GetType().Name}"))
            {
                // Placeholder para debug interactivo
            }
        }
    }
}
