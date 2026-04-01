# Templates de Código - Primeros Sistemas para AS_Cell_0

Esta guía contiene templates y pseudocódigo para los sistemas críticos de AS_Cell_0.

---

## 1. PlayerController.cs - Movimiento Básico

**Ubicación**: `Assets/Scripts/Player/PlayerController.cs`

```csharp
// ============================================================================
// PlayerController.cs - Controlador de movimiento y lógica del jugador
// Ubicación: Assets/Scripts/Player/
// Responsabilidades: Movimiento basado en input, rotación, animación
// ============================================================================

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ========================================================================
    // REFERENCIAS
    // ========================================================================
    
    [SerializeField] private InputManager inputManager;
    private Rigidbody rb;
    private Animator animator; // si lo tienes
    
    // ========================================================================
    // PARÁMETROS
    // ========================================================================
    
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    
    // ========================================================================
    // ESTADO
    // ========================================================================
    
    private Vector3 moveDirection = Vector3.zero;
    
    // ========================================================================
    // EVENTS
    // ========================================================================
    
    public event System.Action<Vector3> OnPlayerMoved;
    
    // ========================================================================
    // LIFECYCLE HOOKS
    // ========================================================================
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // puede ser nulo
        
        if (rb == null)
            Debug.LogError("[PLAYER] No hay Rigidbody en el jugador");
    }
    
    void Start()
    {
        if (inputManager == null)
            inputManager = InputManager.Instance;
            
        if (inputManager != null)
            inputManager.OnMovementInput += HandleMovementInput;
    }
    
    void Update()
    {
        if (moveDirection != Vector3.zero)
            RotateTowardsMoveDirection();
        
        // Opcional: actualizar animación
        if (animator != null)
            animator.SetFloat("Speed", moveDirection.magnitude);
    }
    
    void FixedUpdate()
    {
        ApplyMovement();
    }
    
    void OnDestroy()
    {
        if (inputManager != null)
            inputManager.OnMovementInput -= HandleMovementInput;
    }
    
    // ========================================================================
    // PUBLIC METHODS
    // ========================================================================
    
    public Vector3 GetCurrentDirection() => moveDirection;
    
    public void StopMovement()
    {
        moveDirection = Vector3.zero;
        if (rb != null) rb.velocity = Vector3.zero;
    }
    
    // ========================================================================
    // PRIVATE METHODS
    // ========================================================================
    
    private void HandleMovementInput(Vector3 direction)
    {
        moveDirection = direction.normalized;
        OnPlayerMoved?.Invoke(moveDirection);
    }
    
    private void ApplyMovement()
    {
        if (rb == null) return;
        
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y; // Mantener gravedad
        rb.velocity = velocity;
    }
    
    private void RotateTowardsMoveDirection()
    {
        // Rotar el personaje hacia donde se mueve
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
```

---

## 2. InputManager.cs - Lectura de Entrada

**Ubicación**: `Assets/Scripts/Manager/InputManager.cs` (template simplificado)

```csharp
// ============================================================================
// InputManager.cs - Gestión centralizada de entrada del jugador
// ============================================================================

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // ========================================================================
    // SINGLETON
    // ========================================================================
    
    public static InputManager Instance { get; private set; }
    
    // ========================================================================
    // EVENTOS
    // ========================================================================
    
    public event System.Action<Vector3> OnMovementInput;
    public event System.Action OnAttackButtonPressed;
    public event System.Action OnPowerButtonPressed;
    public event System.Action OnInteractButtonPressed;
    
    // ========================================================================
    // LIFECYCLE HOOKS
    // ========================================================================
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        HandleMovementInput();
        HandleAttackInput();
        HandlePowerInput();
        HandleInteractInput();
    }
    
    // ========================================================================
    // PRIVATE METHODS
    // ========================================================================
    
    private void HandleMovementInput()
    {
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Keyboard.current.wKey.isPressed)
            vertical += 1;
        if (Keyboard.current.sKey.isPressed)
            vertical -= 1;
        if (Keyboard.current.dKey.isPressed)
            horizontal += 1;
        if (Keyboard.current.aKey.isPressed)
            horizontal -= 1;
        
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        OnMovementInput?.Invoke(moveDirection);
    }
    
    private void HandleAttackInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame || 
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnAttackButtonPressed?.Invoke();
        }
    }
    
    private void HandlePowerInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnPowerButtonPressed?.Invoke();
        }
    }
    
    private void HandleInteractInput()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            OnInteractButtonPressed?.Invoke();
        }
    }
}
```

---

## 3. Enemy.cs - Base de Sistema de Enemigos

**Ubicación**: `Assets/Scripts/Enemies/Enemy.cs`

```csharp
// ============================================================================
// Enemy.cs - Clase base para enemigos
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour
{
    // ========================================================================
    // REFERENCIAS
    // ========================================================================
    
    protected Rigidbody rb;
    protected Transform playerTransform;
    
    // ========================================================================
    // PARÁMETROS (herencia)
    // ========================================================================
    
    [SerializeField] protected float maxHealth = 20f;
    [SerializeField] protected float detectionRange = 40f;
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float moveSpeed = 3f;
    
    // ========================================================================
    // ESTADO
    // ========================================================================
    
    protected float currentHealth;
    protected IEnemyState currentState;
    protected float lastAttackTime;
    
    // ========================================================================
    // EVENTOS
    // ========================================================================
    
    public event System.Action<Enemy> OnEnemyDied;
    
    // ========================================================================
    // LIFECYCLE HOOKS
    // ========================================================================
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        
        if (rb == null)
            Debug.LogError($"[ENEMY] No Rigidbody en {gameObject.name}");
    }
    
    protected virtual void Start()
    {
        // Encontrar al jugador
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
            playerTransform = playerGO.transform;
        
        // Estado inicial
        EnterState(new IdleState());
    }
    
    protected virtual void Update()
    {
        if (currentState != null)
            currentState.OnUpdate(this);
    }
    
    // ========================================================================
    // PUBLIC METHODS
    // ========================================================================
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
            Die();
    }
    
    public void EnterState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.OnExit(this);
        
        currentState = newState;
        currentState.OnEnter(this);
    }
    
    public bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }
    
    public void PerformAttack()
    {
        // Realizar ataque (sobreescribir en subclases)
        lastAttackTime = Time.time;
    }
    
    public bool IsPlayerInRange()
    {
        if (playerTransform == null) return false;
        
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= detectionRange;
    }
    
    public float GetDistanceToPlayer()
    {
        if (playerTransform == null) return float.MaxValue;
        return Vector3.Distance(transform.position, playerTransform.position);
    }
    
    public Transform GetPlayerTransform() => playerTransform;
    
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    
    // ========================================================================
    // PRIVATE METHODS
    // ========================================================================
    
    protected virtual void Die()
    {
        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }
}

// ============================================================================
// INTERFACE PARA ESTADOS
// ============================================================================

public interface IEnemyState
{
    void OnEnter(Enemy enemy);
    void OnUpdate(Enemy enemy);
    void OnExit(Enemy enemy);
}

// ============================================================================
// ESTADO IDLE
// ============================================================================

public class IdleState : IEnemyState
{
    private float checkTimer = 0f;
    private const float CHECK_INTERVAL = 0.3f;
    
    public void OnEnter(Enemy enemy)
    {
        Debug.Log($"[ENEMY] {enemy.gameObject.name} entrando en Idle");
    }
    
    public void OnUpdate(Enemy enemy)
    {
        checkTimer += Time.deltaTime;
        
        if (checkTimer >= CHECK_INTERVAL)
        {
            checkTimer = 0f;
            
            if (enemy.IsPlayerInRange())
            {
                enemy.EnterState(new PursuingState());
            }
        }
    }
    
    public void OnExit(Enemy enemy)
    {
        Debug.Log($"[ENEMY] {enemy.gameObject.name} saliendo de Idle");
    }
}

// ============================================================================
// ESTADO PERSIGUIENDO
// ============================================================================

public class PursuingState : IEnemyState
{
    public void OnEnter(Enemy enemy)
    {
        Debug.Log($"[ENEMY] {enemy.gameObject.name} persiguiendo al jugador");
    }
    
    public void OnUpdate(Enemy enemy)
    {
        Transform player = enemy.GetPlayerTransform();
        
        if (player == null)
        {
            enemy.EnterState(new IdleState());
            return;
        }
        
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        
        // Si está lo suficientemente cerca, ataque
        if (distanceToPlayer <= enemy.attackRange)
        {
            enemy.EnterState(new AttackingState());
            return;
        }
        
        // Si el jugador se fue muy lejos, volver a idle
        if (!enemy.IsPlayerInRange())
        {
            enemy.EnterState(new IdleState());
            return;
        }
        
        // Moverse hacia el jugador
        Vector3 directionToPlayer = (player.position - enemy.transform.position).normalized;
        enemy.GetComponent<Rigidbody>().velocity = directionToPlayer * 3f; // moveSpeed
    }
    
    public void OnExit(Enemy enemy)
    {
        enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}

// ============================================================================
// ESTADO ATACANDO
// ============================================================================

public class AttackingState : IEnemyState
{
    public void OnEnter(Enemy enemy)
    {
        Debug.Log($"[ENEMY] {enemy.gameObject.name} atacando");
    }
    
    public void OnUpdate(Enemy enemy)
    {
        Transform player = enemy.GetPlayerTransform();
        
        if (player == null || !enemy.IsPlayerInRange())
        {
            enemy.EnterState(new IdleState());
            return;
        }
        
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        
        // Si se fue muy lejos, perseguir
        if (distanceToPlayer > enemy.attackRange * 1.5f)
        {
            enemy.EnterState(new PursuingState());
            return;
        }
        
        // Intentar atacar si está listo
        if (enemy.CanAttack())
        {
            enemy.PerformAttack();
        }
    }
    
    public void OnExit(Enemy enemy)
    {
    }
}
```

---

## 4. LowRobot.cs - Enemigo Específico

**Ubicación**: `Assets/Scripts/Enemies/LowRobot.cs`

```csharp
// ============================================================================
// LowRobot.cs - Robot de seguridad nivel bajo
// ============================================================================

using UnityEngine;

public class LowRobot : Enemy
{
    // ========================================================================
    // DEPENDENCIAS
    // ========================================================================
    
    private PlayerCombat playerCombat;
    
    // ========================================================================
    // SOBRESCRIBIR STARTUP
    // ========================================================================
    
    protected override void Start()
    {
        base.Start();
        
        // Ajustes específicos para Low Robot
        maxHealth = 20f;
        detectionRange = 40f;
        attackRange = 5f;
        attackCooldown = 0.8f;
        moveSpeed = 3f;
        
        currentHealth = maxHealth;
        
        Debug.Log($"[LOW ROBOT] {gameObject.name} iniciado - Salud: {currentHealth}");
    }
    
    // ========================================================================
    // ATAQUE ESPECÍFICO
    // ========================================================================
    
    public override void PerformAttack()
    {
        base.PerformAttack();
        
        // Buscar al jugador e infligir daño
        if (playerTransform == null) return;
        
        float distanceToPlayer = Vector3.Distance(
            transform.position, 
            playerTransform.position
        );
        
        if (distanceToPlayer <= attackRange)
        {
            // Obtener componente PlayerCombat y usar TakeDamage
            PlayerCombat playerCombat = playerTransform.GetComponent<PlayerCombat>();
            
            if (playerCombat != null)
                playerCombat.TakeDamage(5f); // daño del Low Robot
            
            Debug.Log("[LOW ROBOT] ¡Ataque conectado!");
        }
    }
}
```

---

## 5. PlayerCombat.cs - Sistema de Combate

**Ubicación**: `Assets/Scripts/Player/PlayerCombat.cs`

```csharp
// ============================================================================
// PlayerCombat.cs - Sistema de combate del jugador
// ============================================================================

using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // ========================================================================
    // REFERENCIAS
    // ========================================================================
    
    private InputManager inputManager;
    private Collider attackCollider;
    
    // ========================================================================
    // PARÁMETROS
    // ========================================================================
    
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 0.5f;
    
    // ========================================================================
    // ESTADO
    // ========================================================================
    
    private float lastAttackTime = -1f;
    private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;
    
    // ========================================================================
    // EVENTOS
    // ========================================================================
    
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnAttacked;
    
    // ========================================================================
    // LIFECYCLE HOOKS
    // ========================================================================
    
    void Start()
    {
        inputManager = InputManager.Instance;
        currentHealth = maxHealth;
        
        if (inputManager != null)
            inputManager.OnAttackButtonPressed += HandleAttackInput;
    }
    
    void OnDestroy()
    {
        if (inputManager != null)
            inputManager.OnAttackButtonPressed -= HandleAttackInput;
    }
    
    // ========================================================================
    // PUBLIC METHODS
    // ========================================================================
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"[PLAYER] Recibió {damage} de daño. Salud: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
            Die();
    }
    
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    
    // ========================================================================
    // PRIVATE METHODS
    // ========================================================================
    
    private void HandleAttackInput()
    {
        if (!CanAttack())
            return;
        
        PerformAttack();
    }
    
    private void PerformAttack()
    {
        lastAttackTime = Time.time;
        OnAttacked?.Invoke();
        
        Debug.Log("[PLAYER] ¡Realizando ataque!");
        
        // Detectar enemigos en rango
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            attackRange
        );
        
        foreach (Collider col in hitColliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);
                Debug.Log($"[PLAYER] Golpeó a {enemy.gameObject.name}");
            }
        }
    }
    
    private bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }
    
    private void Die()
    {
        Debug.Log("[PLAYER] ¡GAME OVER!");
        Time.timeScale = 0f; // Pausar juego
    }
}
```

---

## 6. DoorController.cs - Control de Puertas

**Ubicación**: `Assets/Scripts/Station/DoorController.cs`

```csharp
// ============================================================================
// DoorController.cs - Control de puertas y accesos
// ============================================================================

using UnityEngine;

public class DoorController : MonoBehaviour
{
    // ========================================================================
    // PARÁMETROS
    // ========================================================================
    
    [SerializeField] private string requiredCode = "AS_p1"; // código necesario
    [SerializeField] private float openHeight = 3f; // altura abierta
    [SerializeField] private float openSpeed = 2f; // velocidad de apertura
    
    // ========================================================================
    // ESTADO
    // ========================================================================
    
    private bool isOpen = false;
    private Vector3 startPosition;
    private Vector3 openPosition;
    
    // ========================================================================
    // LIFECYCLE HOOKS
    // ========================================================================
    
    void Awake()
    {
        startPosition = transform.position;
        openPosition = startPosition + Vector3.up * openHeight;
    }
    
    // ========================================================================
    // PUBLIC METHODS
    // ========================================================================
    
    public void TryOpen(string code)
    {
        if (code == requiredCode && !isOpen)
        {
            OpenDoor();
        }
        else if (code != requiredCode)
        {
            Debug.LogWarning($"[DOOR] Código incorrecto: {code} != {requiredCode}");
        }
    }
    
    public void OpenDoor()
    {
        if (isOpen) return;
        
        isOpen = true;
        Debug.Log($"[DOOR] Abriendo puerta {gameObject.name}");
        StartCoroutine(AnimateOpening());
    }
    
    // ========================================================================
    // PRIVATE METHODS
    // ========================================================================
    
    private System.Collections.IEnumerator AnimateOpening()
    {
        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                openPosition,
                openSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        transform.position = openPosition;
    }
}
```

---

## 7. Configuración en la Escena

### Jerarquía mínima de AS_Cell_0:

```
AS_Cell_0 (Scene Root)
├── GroundPlane (Mesh)
├── Player
│   ├── (Sphere mesh - visual)
│   ├── Rigidbody (Dynamic, Constraints: Freeze Y rotation)
│   ├── Collider (Sphere)
│   ├── PlayerController.cs
│   └── PlayerCombat.cs
├── Enemy_LowRobot_01
│   ├── (Cube mesh - visual)
│   ├── Rigidbody (Dynamic, Constraints: Freeze Y rotation)
│   ├── Collider (Box)
│   └── LowRobot.cs
├── Doors
│   ├── Door_Exit
│   │   ├── (Cube mesh - visual)
│   │   └── DoorController.cs (requiredCode: "AS_p1")
│   └── Door_Secret
│       ├── (Cube mesh - visual)
│       └── DoorController.cs (requiredCode: "poder_defensivo")
├── Items
│   ├── CodePickup_AS_p1
│   │   └── CodePickup.cs (código: "AS_p1")
│   └── CodePickup_AS_p2
│       └── CodePickup.cs (código: "AS_p2")
├── Managers
│   ├── InputManager (prefab singleton)
│   └── GameManager (prefab singleton)
├── Camera
│   └── CameraController (LookAt player, isometric angle)
└── UI Canvas
    ├── HealthBar (Text)
    └── Objectives (Text)
```

---

## 8. Index de Códigos Necesarios

Crear un enum en `GlobalDefinitions.cs`:

```csharp
public enum AccessCode
{
    AS_p1,      // Abre puerta salida AS_Cell_0
    AS_p2,      // Código bonus
    AS_Asc,     // Acceso ascensor
    // ... más códigos según el documento flujo_manual
}
```

---

## 📝 PRÓXIMO PASO

1. **Crear los scripts** en sus ubicaciones respectivas
2. **Armar la escena** en Unity con la jerarquía propuesta
3. **Asignar referencias** en los inspectors
4. **Test**: Ejecutar y verificar cada funcionalidad
5. **Commit**: Guardar en Git con mensaje `"Add: PlayerController, InputManager, Enemy base systems"`

---

**Última actualización**: Abril 2026
