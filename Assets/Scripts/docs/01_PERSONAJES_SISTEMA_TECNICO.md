# PRISIÓN ESPACIAL ROTATORIA - SISTEMA TÉCNICO DE PERSONAJES

## Índice
1. [Arquitectura Compartida](#arquitectura-compartida)
2. [Parámetros Visuales Sin UI](#parámetros-visuales-sin-ui)
3. [Personaje Base](#personaje-base)
4. [Personajes Específicos](#personajes-específicos)
5. [Sistema de Rotación](#sistema-de-rotación)
6. [Interacciones Entre Personajes](#interacciones-entre-personajes)

---

## Arquitectura Compartida

### Carpetas Compartidas (100%)

```
Assets/
├── Scripts/Core/
│   ├── PlayerCharacterController.cs (BASE ABSTRACTA)
│   ├── CharacterStats.cs (ScriptableObject)
│   ├── PowerAbilitySystem.cs (Sistema de grados 1-4)
│   ├── RotationStateManager.cs (Maneja estado de rotación)
│   └── InputManager.cs (Centralizado para los 5)
│
├── Animations/Shared/
│   ├── BaseAnimatorController.controller
│   ├── BlendTrees/
│   │   └── Walk_8Directions.blend
│   └── CommonAnimations/
│       ├── Idle.anim
│       ├── Walk_N/NE/E/SE/S/SW/W/NW.anim
│       ├── Dash.anim
│       └── Stun.anim
│
├── Input/
│   └── PlayerInputActions.inputactions (COMPARTIDA)
│
├── VFX/Shared/
│   ├── DustCloud.prefab
│   ├── Impact_Generic.prefab
│   └── Stun_Effect.prefab
│
└── Audio/Shared/
    ├── Footstep_Generic.mp3
    ├── Movement_Ambient.mp3
    └── Error_Beep.mp3
```

### Qué NO compartimos

- **Modelos 3D / Meshes:** Cada personaje es radicalmente diferente
- **Materiales:** Sus sistemas visuales son únicos
- **Animation Clips:** Aunque comparten estructura, las formas no lo permiten
- **VFX/SFX:** Cada uno tiene efectos específicos

---

## Parámetros Visuales Sin UI

### Principio: El Personaje Es Su Propia Interfaz

En lugar de barras de energía, indicadores numéricos o paneles flotantes:

#### FOLD - Anomalía por Opacidad y Distorsión
```
Estado Normal:
- Membranas translúcidas: opacidad 0.6-0.8
- Aristas en violeta frío: intensidad 1.0
- Sin distorsión del entorno

Grado 1-2 (Bajo costo):
- Opacidad baja a 0.5
- Destello violeta pulsante (5x/segundo)
- Distorsión del entorno: leve (desplazamiento 0.5u)

Grado 3+ (Alto costo):
- Opacidad cae a 0.3
- Destellos violeta extremos (10x/segundo)
- Distorsión severa del entorno visible como efecto de refracción
- Objetos cercanos se desplazan visiblemente en el espacio

En estado crítico:
- Opacidad casi 0 (transparencia pura)
- Aristas brillan en blanco cegador
- Distorsión máxima: el espacio alrededor se deforma visiblemente
```

**Lectura Visual para Jugador:** Cuanto más brillante y transparente, más cercano al límite está.

---

#### SWARM - Fragmentación Visible como Separación

```
Estado Normal (Integrado):
- Cuerpo central: color verde oliva saturado
- Cables verde neón: emisión máxima
- 8 ojos rojos brillando en sincronización
- Movimiento de extremidades: sincrónico

Grado 1-2 (3-4 unidades):
- Cuerpo central: opacidad 0.9
- Color del cuerpo: verde ligeramente desaturado
- Cables: emisión reducida a 60%
- Movimiento: las extremidades ralentizan
- Las unidades separadas: pequeñas, conectadas por cable de datos visible

Grado 3 (8 unidades):
- Cuerpo central: opacidad 0.7, color casi gris-verde
- Cables: apenas brillan (emisión 20%)
- Los 8 ojos pierden sincronización, parpadean irregularmente
- Las unidades se mueven de forma impredecible
- Cable de conexión se vuelve intermitente visualmente

Grado 4 (Dispersión Total):
- El cuerpo central desaparece visualmente
- Todas las unidades flotan independientemente
- Color verde nebuloso en lugar de verde sólido
- Movimiento completamente caótico
- Ocasionalmente "buscan" el centro de control (animación de confusión)
```

**Lectura Visual para Jugador:** Cuanto más los ojos se desincronícen y el cable se debilite, menos control hay.

---

#### MIST - Compresión Visual por Tamaño y Densidad

```
Estado Normal:
- Columna de niebla: altura 1.8m, anchura estándar
- Color: azul hielo en interior, blanco en corrientes internas
- Bordes: difusos, pulsación respiratoria regular
- Emisión eléctrica interna: destellos azul-blancos suaves

Grado 1-2 (Filtraje/Presión dirigida):
- Columna: se comprime ligeramente (altura 1.5m)
- Color: azul más intenso
- Bordes: más definidos, menos difusos
- Pulsación: ligeramente más rápida

Grado 3 (Compresión de sala):
- Columna: muy comprimida (altura 1.2m, muy densa)
- Color: azul casi blanco por densidad extrema
- Bordes: muy definidos, casi sólidos
- Pulsación: muy rápida, agitada
- Destellos eléctricos: constantes y violentos

Grado 4 (Vacío dirigido):
- Columna: mínima (altura 0.8m)
- Color: blanco puro, transparencia al máximo
- Bordes: casi inexistentes, solo una silueta etérea
- Pulsación: caótica, instable
- Destellos: descontrolados
```

**Lectura Visual para Jugador:** Más comprimido = más poder usado. En estado crítico, parece que se va a dispersar.

---

#### STONE - Fractura y Brillo Mineral por Daño

```
Estado Normal:
- Forma mineral: sólida, aristas definidas
- Color: gris antracita oscuro
- Vetas ocre: iluminación cálida normal
- Integridad visual: 100% — sin grietas visibles
- Fragmentos orbitales: órbita suave alrededor del cuerpo

Grado 1-2 (Presión controlada / Impacto):
- Vetas ocre: intensidad de iluminación aumenta 30%
- Fragmentos orbitales: empiezan a orbitar más rápido
- Superficie: sin cambios

Grado 3 (Carga estructural):
- Vetas ocre: 60% de intensidad (muy brillantes)
- Fragmentos orbitales: orbita agresiva, algunos parpadeando
- Superficie: primeras grietas visibles en forma de líneas negras finas
- Color general: ligeramente más gris por decoloración

Grado 4 (Desbalance de masa):
- Vetas ocre: blanco-naranja cegador
- Fragmentos orbitales: órbita caótica, algunos desprendiéndose
- Superficie: grietas profundas visibles, algunos fragmentos flotando
- Color: gris muy desaturado, casi carbón
- Movimiento: tiembla constantemente

En estado crítico (sin poder):
- Las grietas empiezan a brillar internamente (sin vetas)
- Fragmentos se desmoronan lentamente
- Toda la estructura está al borde de colapsar
```

**Lectura Visual para Jugador:** Cuantas más grietas, más dañado está. Las grietas nunca desaparecen en la misma sala.

---

#### LUMEN - Membrana Desgastada por Opacidad y Color

```
Estado Normal (Membrana íntegra):
- Esfera translúcida: opacidad 0.9
- Color membrana: crema translúcido
- Núcleo: ámbar cálido, pulsación 1 Hz
- Filamentos internos: suaves, azul-blanco tenue
- Aura exterior: débil, apenas visible

Grado 1 (Pulso de contacto - sin impacto visual):
- Sin cambios visibles en la membrana
- El filamento de contacto: blanco puro, visible
- Duración: 0.5 segundos

Grado 2 (Emisión dirigida):
- Membrana: opacidad cae a 0.75
- Color membrana: ligeramente naranjo por transparencia
- Núcleo: naranja más intenso
- Filamento emitido: blanco eléctrico, muy visible
- Aura: ahora claramente visible, naranja-blanco

Grado 3 (Pulso de área):
- Membrana: opacidad cae a 0.5
- Color membrana: naranja intenso
- Núcleo: rojo-naranja
- Filamentos internos: constantes, rojo-blanco
- Aura: muy visible, naranja radiante
- Destellos internos: frecuentes

Grado 4 (Sobrecarga):
- Membrana: opacidad 0.2 (casi invisible)
- Color membrana: blanco puro
- Núcleo: rojo-blanco cegador
- Filamentos: salen incontrolados en todas direcciones
- Aura: rojo-blanco radiante que cubre todo el radio de acción

En estado crítico (después de Grado 4):
- Membrana: intermitente transparencia
- Emite destellos aleatorios cada 1-2 segundos
- Filamentos: salen sin dirección durante 20 segundos
- Color nuclear: rojo-blanco parpadeante
- Cada emisión involuntaria: opacidad reduce más
```

**Lectura Visual para Jugador:** La membrana es una barra de salud visual. Cuanto más naranja-rojo y transparente, más cerca del colapso.

---

## Personaje Base

### CharacterStats - ScriptableObject

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Prisión/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("== IDENTIFICACIÓN ==")]
    public string characterName;
    public enum CharacterType { Fold, Swarm, Mist, Stone, Lumen }
    public CharacterType type;
    
    [Header("== STATS BASE ==")]
    public float maxHealth = 100f;
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float groundDrag = 8f;
    
    [Header("== PODER PRIMARIO ==")]
    public float powerCooldownGrade1 = 0.5f;
    public float powerCooldownGrade2 = 1.2f;
    public float powerCooldownGrade3 = 2.0f;
    public float powerCooldownGrade4 = 5.0f;
    
    [Header("== RECURSO ÚNICO (variable por personaje) ==")]
    // Para LUMEN: degradación de membrana
    // Para SWARM: número de unidades disponibles
    // Para MIST: energía de compresión
    // Para FOLD: estabilidad espacial
    // Para STONE: integridad estructural
    public float maxResourceValue = 100f;
    public float resourceRegenerationPerSector = 50f;
    
    [Header("== ANIMADOR ==")]
    public RuntimeAnimatorController animatorController;
    public Avatar avatar;
    
    [Header("== SONIDOS ==")]
    public AudioClip[] powerActivationSFX;
    public AudioClip[] footstepSFX;
}
```

---

### PlayerCharacterController - Base Abstracta

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerCharacterController : MonoBehaviour
{
    [Header("== REFERENCIAS ==")]
    protected Animator animator;
    protected Rigidbody rb;
    protected PlayerInputActions inputActions;
    protected Transform cameraTransform;
    protected CharacterStats stats;
    
    [SerializeField] protected CharacterStats characterStats;
    
    [Header("== ESTADO DE ROTACIÓN ==")]
    protected RotationState currentRotationState = RotationState.Normal;
    
    [Header("== GROUND CHECK ==")]
    [SerializeField] protected float groundCheckDistance = 0.5f;
    [SerializeField] protected LayerMask groundLayer;
    
    // ===== INPUT =====
    protected Vector2 moveInput;
    protected Vector2 lookInput;
    protected float grade1Input;
    protected float grade2Input;
    protected float grade3Input;
    protected float grade4Input;
    protected bool dashInput;
    
    // ===== STATE =====
    protected bool isGrounded;
    protected bool isPerformingAction;
    protected Vector3 moveDirection;
    protected float currentHealth;
    protected float currentResource; // Membrana, fragmentos, energía, etc
    
    // ===== 8 DIRECTIONS =====
    protected enum Direction8Way { N, NE, E, SE, S, SW, W, NW, None }
    protected Direction8Way currentDirection8Way = Direction8Way.None;

    protected virtual void Awake()
    {
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        
        if (characterStats != null)
            stats = characterStats;
        
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.drag = stats.groundDrag;
        
        currentHealth = stats.maxHealth;
        currentResource = stats.maxResourceValue;
    }

    protected virtual void OnEnable()
    {
        inputActions.Player.Enable();
        
        inputActions.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        
        inputActions.Player.CameraLook.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.CameraLook.canceled += ctx => lookInput = Vector2.zero;
        
        // Poderes sensibles a presión (0-1)
        inputActions.Player.OffensivePower.performed += ctx => grade1Input = ctx.ReadValue<float>();
        inputActions.Player.OffensivePower.canceled += ctx => grade1Input = 0f;
        
        inputActions.Player.DefensivePower.performed += ctx => grade2Input = ctx.ReadValue<float>();
        inputActions.Player.DefensivePower.canceled += ctx => grade2Input = 0f;
        
        inputActions.Player.PowerCombination.performed += ctx => grade3Input = ctx.ReadValue<float>();
        inputActions.Player.PowerCombination.canceled += ctx => grade3Input = 0f;
        
        inputActions.Player.Dash.performed += ctx => grade4Input = ctx.ReadValue<float>();
        inputActions.Player.Dash.canceled += ctx => grade4Input = 0f;
    }

    protected virtual void OnDisable()
    {
        inputActions.Player.Disable();
    }

    protected virtual void Update()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );

        CalculateMovementDirection();
        DetermineFacingDirection();
        UpdateAnimatorParameters();
    }

    protected virtual void FixedUpdate()
    {
        MoveCharacter();
    }

    // ===== MOVEMENT =====
    
    protected virtual void CalculateMovementDirection()
    {
        if (moveInput.magnitude < 0.1f)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 right = cameraTransform.right;
        Vector3 forward = cameraTransform.forward;
        
        forward.y = 0;
        forward.Normalize();
        right.y = 0;
        right.Normalize();

        moveDirection = (right * moveInput.x + forward * moveInput.y).normalized;
    }

    protected virtual void DetermineFacingDirection()
    {
        if (moveInput.magnitude < 0.1f)
        {
            currentDirection8Way = Direction8Way.None;
            return;
        }

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        currentDirection8Way = GetDirection8Way(angle);
        
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                stats.rotationSpeed * Time.deltaTime
            );
        }
    }

    protected Direction8Way GetDirection8Way(float angle)
    {
        if (angle >= 337.5f || angle < 22.5f) return Direction8Way.N;
        if (angle >= 22.5f && angle < 67.5f) return Direction8Way.NE;
        if (angle >= 67.5f && angle < 112.5f) return Direction8Way.E;
        if (angle >= 112.5f && angle < 157.5f) return Direction8Way.SE;
        if (angle >= 157.5f && angle < 202.5f) return Direction8Way.S;
        if (angle >= 202.5f && angle < 247.5f) return Direction8Way.SW;
        if (angle >= 247.5f && angle < 292.5f) return Direction8Way.W;
        if (angle >= 292.5f && angle < 337.5f) return Direction8Way.NW;
        return Direction8Way.None;
    }

    protected virtual void MoveCharacter()
    {
        if (isPerformingAction)
        {
            rb.velocity *= 0.8f;
            return;
        }

        Vector3 velocity = moveDirection * stats.speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    // ===== POWER SYSTEM - ABSTRACT =====
    
    /// <summary>
    /// Cada personaje implementa sus 4 grados de poder
    /// </summary>
    protected abstract void ExecuteGrade1Power();
    protected abstract void ExecuteGrade2Power();
    protected abstract void ExecuteGrade3Power();
    protected abstract void ExecuteGrade4Power();
    
    // ===== RESOURCE MANAGEMENT (Override si es necesario) =====
    
    protected virtual void ConsumeResource(float amount)
    {
        currentResource -= amount;
        currentResource = Mathf.Max(0, currentResource);
        UpdateResourceVisualization();
    }

    protected virtual void RestoreResource(float amount)
    {
        currentResource += amount;
        currentResource = Mathf.Min(stats.maxResourceValue, currentResource);
        UpdateResourceVisualization();
    }

    protected abstract void UpdateResourceVisualization();
    
    // ===== ANIMATOR =====
    
    protected virtual void UpdateAnimatorParameters()
    {
        float speed = moveDirection.magnitude * stats.speed;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetBool("IsMoving", moveInput.magnitude > 0.1f);
        animator.SetInteger("Direction", (int)currentDirection8Way);
    }

    // ===== GETTERS =====
    
    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => stats.maxHealth;
    public float GetResource() => currentResource;
    public float GetMaxResource() => stats.maxResourceValue;
    public bool IsPerformingAction() => isPerformingAction;
    public RotationState GetRotationState() => currentRotationState;
}

public enum RotationState
{
    Detenida,    // 0G - Rotación parada
    Baja,        // Baja velocidad de rotación
    Normal,      // Velocidad normal
    Alta         // Alta velocidad de rotación
}
```

---

## Personajes Específicos

### 1. FOLD - El Teleportador

```csharp
using UnityEngine;

public class FoldController : PlayerCharacterController
{
    [Header("== FOLD SPECIFIC ==")]
    [SerializeField] private float microSaltRange = 3f;
    [SerializeField] private float estandarSaltRange = 8f;
    [SerializeField] private float extendidoSaltRange = 15f;
    [SerializeField] private float colapsoRadius = 20f;
    
    // Visualización de anomalía
    private Renderer foldRenderer;
    private Material foldMaterial;
    private float anomalyIntensity;
    
    // Cooldowns
    private float grade1Cooldown;
    private float grade2Cooldown;
    private float grade3Cooldown;
    private float grade4Cooldown;

    protected override void Awake()
    {
        base.Awake();
        foldRenderer = GetComponent<Renderer>();
        foldMaterial = foldRenderer.material;
    }

    protected override void Update()
    {
        base.Update();
        
        // Actualizar cooldowns
        grade1Cooldown -= Time.deltaTime;
        grade2Cooldown -= Time.deltaTime;
        grade3Cooldown -= Time.deltaTime;
        grade4Cooldown -= Time.deltaTime;
        
        // Procesar inputs
        if (grade1Input > 0.1f && grade1Cooldown <= 0)
            ExecuteGrade1Power();
        
        if (grade2Input > 0.1f && grade2Cooldown <= 0)
            ExecuteGrade2Power();
        
        if (grade3Input > 0.1f && grade3Cooldown <= 0)
            ExecuteGrade3Power();
        
        if (grade4Input > 0.1f && grade4Cooldown <= 0)
            ExecuteGrade4Power();
        
        // Reducir anomalía con el tiempo
        anomalyIntensity -= Time.deltaTime * 2f;
        anomalyIntensity = Mathf.Max(0, anomalyIntensity);
    }

    protected override void ExecuteGrade1Power()
    {
        // Micro-salto: 3 unidades
        Vector3 teleportPos = transform.position + transform.forward * microSaltRange;
        TeleportTo(teleportPos, 0.3f); // Anomalía baja
        
        grade1Cooldown = stats.powerCooldownGrade1;
        ConsumeResource(10f);
    }

    protected override void ExecuteGrade2Power()
    {
        // Salto estándar: 8 unidades
        Vector3 teleportPos = transform.position + transform.forward * estandarSaltRange;
        TeleportTo(teleportPos, 0.6f); // Anomalía media
        
        grade2Cooldown = stats.powerCooldownGrade2;
        ConsumeResource(20f);
    }

    protected override void ExecuteGrade3Power()
    {
        // Salto extendido: 15 unidades
        Vector3 teleportPos = transform.position + transform.forward * extendidoSaltRange;
        TeleportTo(teleportPos, 1.0f); // Anomalía severa
        
        grade3Cooldown = stats.powerCooldownGrade3;
        ConsumeResource(40f);
    }

    protected override void ExecuteGrade4Power()
    {
        // Colapso espacial: geometría permanente
        CreateSpaceAnomaly(colapsoRadius);
        
        grade4Cooldown = stats.powerCooldownGrade4;
        ConsumeResource(100f);
    }

    private void TeleportTo(Vector3 targetPos, float anomalyStrength)
    {
        // Efecto de desaparición
        animator.SetTrigger("Teleport");
        isPerformingAction = true;
        
        // Destellos visuales
        StartCoroutine(TeleportCoroutine(targetPos, anomalyStrength));
    }

    private System.Collections.IEnumerator TeleportCoroutine(Vector3 targetPos, float anomalyStrength)
    {
        // Fade out
        float fadeTime = 0.2f;
        float elapsed = 0;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeTime);
            foldMaterial.SetFloat("_Opacity", alpha);
            yield return null;
        }
        
        // Teletransportar
        transform.position = targetPos;
        
        // Fade in
        elapsed = 0;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / fadeTime;
            foldMaterial.SetFloat("_Opacity", alpha);
            yield return null;
        }
        
        // Crear anomalía
        anomalyIntensity = anomalyStrength;
        CreateAnomalyEffect(targetPos, anomalyStrength);
        
        isPerformingAction = false;
    }

    private void CreateAnomalyEffect(Vector3 pos, float strength)
    {
        // Empujar objetos cercanos
        Collider[] nearby = Physics.OverlapSphere(pos, strength * 2f);
        foreach (Collider col in nearby)
        {
            if (col.CompareTag("Prop") || col.CompareTag("Robot"))
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 pushDir = (col.transform.position - pos).normalized;
                    rb.velocity += pushDir * strength * 5f;
                }
            }
        }
        
        // Instanciar FX de anomalía
        // TriggerSensor en el radio
    }

    private void CreateSpaceAnomaly(float radius)
    {
        // Grado 4: cambio permanente de geometría en la sala
        // Implementar según sistema de salas
        Debug.Log($"FOLD crea colapso espacial en radio {radius}");
    }

    protected override void UpdateResourceVisualization()
    {
        // Membrana translúcida se vuelve más visible con menos energía
        float resourceRatio = currentResource / stats.maxResourceValue;
        
        // Opacidad inversa: a menos recursos, más transparente
        float targetOpacity = Mathf.Lerp(0.3f, 0.8f, resourceRatio);
        
        // Intensidad de aristas (violeta)
        float edgeIntensity = Mathf.Lerp(0.5f, 1.5f, 1f - resourceRatio);
        
        foldMaterial.SetFloat("_Opacity", targetOpacity);
        foldMaterial.SetFloat("_EdgeIntensity", edgeIntensity);
        
        // Distorsión visual
        foldMaterial.SetFloat("_DistortionStrength", anomalyIntensity);
    }

    protected override void UpdateAnimatorParameters()
    {
        base.UpdateAnimatorParameters();
        animator.SetFloat("AnomalyIntensity", anomalyIntensity);
    }
}
```

---

### 2. SWARM - La Araña Robótica

```csharp
using UnityEngine;
using System.Collections.Generic;

public class SwarmController : PlayerCharacterController
{
    [Header("== SWARM SPECIFIC ==")]
    [SerializeField] private int maxFragments = 12;
    [SerializeField] private float fragmentSpeed = 8f;
    [SerializeField] private float fragmentRange = 20f;
    
    private int activeFragments = 0;
    private List<SwarmUnit> fragmentList = new List<SwarmUnit>();
    
    // Visualización
    private Color coreColor = new Color(0.4f, 0.6f, 0.2f); // Verde oliva
    private Color activeColor = new Color(0.5f, 1f, 0.2f); // Verde neón
    private Renderer coreRenderer;
    private Material coreMaterial;
    
    // Cooldowns
    private float grade1Cooldown;
    private float grade2Cooldown;
    private float grade3Cooldown;
    private float grade4Cooldown;

    protected override void Awake()
    {
        base.Awake();
        coreRenderer = GetComponent<Renderer>();
        coreMaterial = coreRenderer.material;
        activeFragments = 0;
    }

    protected override void Update()
    {
        base.Update();
        
        grade1Cooldown -= Time.deltaTime;
        grade2Cooldown -= Time.deltaTime;
        grade3Cooldown -= Time.deltaTime;
        grade4Cooldown -= Time.deltaTime;
        
        if (grade1Input > 0.1f && grade1Cooldown <= 0)
            ExecuteGrade1Power();
        
        if (grade2Input > 0.1f && grade2Cooldown <= 0)
            ExecuteGrade2Power();
        
        if (grade3Input > 0.1f && grade3Cooldown <= 0)
            ExecuteGrade3Power();
        
        if (grade4Input > 0.1f && grade4Cooldown <= 0)
            ExecuteGrade4Power();
        
        // Actualizar unidades
        foreach (SwarmUnit fragment in fragmentList)
        {
            if (fragment != null)
                fragment.Update();
        }
    }

    protected override void ExecuteGrade1Power()
    {
        // Extensión de cable: hackeo sin fragmentarse
        Debug.Log("SWARM: Extensión de cable");
        grade1Cooldown = stats.powerCooldownGrade1;
        
        // Buscar sistema cercano
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f))
        {
            if (hit.collider.CompareTag("System"))
            {
                animator.SetTrigger("CableExtend");
                ConsumeResource(5f);
            }
        }
    }

    protected override void ExecuteGrade2Power()
    {
        // Fragmentación parcial: 3-4 unidades
        if (activeFragments == 0)
        {
            FragmentSwarm(3);
            grade2Cooldown = stats.powerCooldownGrade2;
            ConsumeResource(20f);
        }
        else
        {
            ReagroupSwarm();
        }
    }

    protected override void ExecuteGrade3Power()
    {
        // Enjambre activo: 8 unidades
        if (activeFragments == 0)
        {
            FragmentSwarm(8);
            grade3Cooldown = stats.powerCooldownGrade3;
            ConsumeResource(50f);
        }
        else
        {
            ReagroupSwarm();
        }
    }

    protected override void ExecuteGrade4Power()
    {
        // Dispersión total: 12 unidades, pérdida de control
        if (activeFragments == 0)
        {
            FragmentSwarm(12);
            grade4Cooldown = stats.powerCooldownGrade4;
            ConsumeResource(100f);
        }
        else
        {
            ReagroupSwarm();
        }
    }

    private void FragmentSwarm(int count)
    {
        isPerformingAction = true;
        activeFragments = count;
        
        animator.SetInteger("FragmentCount", count);
        animator.SetTrigger("Fragment");
        
        // Crear unidades
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 2f;
            SwarmUnit unit = Instantiate(
                Resources.Load<SwarmUnit>("Prefabs/SwarmUnit"),
                spawnPos,
                Quaternion.identity
            );
            unit.Initialize(this, i);
            fragmentList.Add(unit);
        }
        
        Invoke(nameof(ResetAction), 0.5f);
    }

    private void ReagroupSwarm()
    {
        isPerformingAction = true;
        
        animator.SetTrigger("Regroup");
        
        // Reagrupar todas las unidades
        foreach (SwarmUnit fragment in fragmentList)
        {
            if (fragment != null)
                fragment.ReturnToCore(this);
        }
        
        activeFragments = 0;
        fragmentList.Clear();
        
        Invoke(nameof(ResetAction), 0.5f);
    }

    protected override void UpdateResourceVisualization()
    {
        float resourceRatio = currentResource / stats.maxResourceValue;
        
        // Color del núcleo cambia de verde vibrante a verde apagado
        Color targetColor = Color.Lerp(
            new Color(0.3f, 0.3f, 0.2f),
            activeColor,
            resourceRatio
        );
        
        coreMaterial.SetColor("_Color", targetColor);
        
        // Emisión de cables
        float cableEmission = Mathf.Lerp(0.2f, 1.0f, resourceRatio);
        coreMaterial.SetFloat("_CableEmission", cableEmission);
        
        // Sincronización de ojos
        float eyeSyncSpeed = Mathf.Lerp(0f, 2f, resourceRatio);
        animator.SetFloat("EyeSyncSpeed", eyeSyncSpeed);
    }

    private void ResetAction()
    {
        isPerformingAction = false;
    }
}

public class SwarmUnit : MonoBehaviour
{
    private SwarmController parentController;
    private int unitID;
    private Renderer unitRenderer;
    private Material unitMaterial;
    private bool returningToCore;
    
    public void Initialize(SwarmController controller, int id)
    {
        parentController = controller;
        unitID = id;
        unitRenderer = GetComponent<Renderer>();
        unitMaterial = unitRenderer.material;
    }

    public void Update()
    {
        if (returningToCore)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                parentController.transform.position,
                Time.deltaTime * 5f
            );
            
            if (Vector3.Distance(transform.position, parentController.transform.position) < 0.5f)
                Destroy(gameObject);
        }
        else
        {
            // Lógica de unidad libre: buscar sistemas, enemigos, residuos
            SearchForTargets();
        }
    }

    private void SearchForTargets()
    {
        // Implementar IA simple de unidad
        // Prioridades: detectar amenazas, consumir residuos, hackear sistemas
    }

    public void ReturnToCore(SwarmController controller)
    {
        returningToCore = true;
    }
}
```

---

### 3. MIST - El Gaseoso

```csharp
using UnityEngine;

public class MistController : PlayerCharacterController
{
    [Header("== MIST SPECIFIC ==")]
    [SerializeField] private float compressionDensity = 1f; // 0-1
    [SerializeField] private float ventDuctSpeed = 15f;
    [SerializeField] private float pressureRadius = 5f;
    
    private Renderer mistRenderer;
    private Material mistMaterial;
    private float currentPressure = 1f; // Presión actual en la sala
    
    // Cooldowns
    private float grade1Cooldown;
    private float grade2Cooldown;
    private float grade3Cooldown;
    private float grade4Cooldown;

    protected override void Awake()
    {
        base.Awake();
        mistRenderer = GetComponent<Renderer>();
        mistMaterial = mistRenderer.material;
    }

    protected override void Update()
    {
        base.Update();
        
        grade1Cooldown -= Time.deltaTime;
        grade2Cooldown -= Time.deltaTime;
        grade3Cooldown -= Time.deltaTime;
        grade4Cooldown -= Time.deltaTime;
        
        if (grade1Input > 0.1f && grade1Cooldown <= 0)
            ExecuteGrade1Power();
        
        if (grade2Input > 0.1f && grade2Cooldown <= 0)
            ExecuteGrade2Power();
        
        if (grade3Input > 0.1f && grade3Cooldown <= 0)
            ExecuteGrade3Power();
        
        if (grade4Input > 0.1f && grade4Cooldown <= 0)
            ExecuteGrade4Power();
    }

    protected override void ExecuteGrade1Power()
    {
        // Filtraje: pasar por conductos
        animator.SetTrigger("EnterVentDuct");
        // Implementar sistema de conductos
        grade1Cooldown = stats.powerCooldownGrade1;
        ConsumeResource(10f);
    }

    protected override void ExecuteGrade2Power()
    {
        // Presión dirigida: empujar objetos
        CreatePressureZone(pressureRadius, 0.6f);
        grade2Cooldown = stats.powerCooldownGrade2;
        ConsumeResource(20f);
    }

    protected override void ExecuteGrade3Power()
    {
        // Compresión de sala
        CompressRoom(1.0f);
        grade3Cooldown = stats.powerCooldownGrade3;
        ConsumeResource(60f);
    }

    protected override void ExecuteGrade4Power()
    {
        // Vacío dirigido
        CreateVacuum(pressureRadius * 2f);
        grade4Cooldown = stats.powerCooldownGrade4;
        ConsumeResource(100f);
    }

    private void CreatePressureZone(float radius, float intensity)
    {
        // Empujar objetos
        Collider[] nearby = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in nearby)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (col.transform.position - transform.position).normalized;
                rb.velocity += pushDir * intensity * 10f;
            }
        }
        
        animator.SetTrigger("PressureBlast");
    }

    private void CompressRoom(float intensity)
    {
        // Cambiar presión de la sala
        currentPressure = intensity;
        animator.SetTrigger("CompressRoom");
    }

    private void CreateVacuum(float radius)
    {
        // Descompresión
        animator.SetTrigger("CreateVacuum");
    }

    protected override void UpdateResourceVisualization()
    {
        float resourceRatio = currentResource / stats.maxResourceValue;
        
        // Tamaño y compresión de la columna
        float scaleY = Mathf.Lerp(0.8f, 1.8f, resourceRatio);
        transform.localScale = new Vector3(1f, scaleY, 1f);
        
        // Color: azul más intenso cuanto más comprimida
        float compression = 1f - resourceRatio;
        Color targetColor = Color.Lerp(
            new Color(0.5f, 0.8f, 1.0f),
            new Color(0.1f, 0.3f, 0.8f),
            compression
        );
        
        mistMaterial.SetColor("_Color", targetColor);
        
        // Pulsación más rápida cuanto más comprimida
        float pulsationSpeed = Mathf.Lerp(1f, 4f, compression);
        animator.SetFloat("PulsationSpeed", pulsationSpeed);
    }

    protected override void UpdateAnimatorParameters()
    {
        base.UpdateAnimatorParameters();
        animator.SetFloat("Pressure", currentPressure);
    }
}
```

---

### 4. STONE - El Mineral

```csharp
using UnityEngine;

public class StoneController : PlayerCharacterController
{
    [Header("== STONE SPECIFIC ==")]
    [SerializeField] private float structuralDamage = 0f; // 0-1
    [SerializeField] private float forceMagnitude = 50f;
    [SerializeField] private float destructionRadius = 5f;
    
    private Renderer stoneRenderer;
    private Material stoneMaterial;
    private float internalGlow = 0f; // Brillo de vetas (0-1)
    
    // Cooldowns
    private float grade1Cooldown;
    private float grade2Cooldown;
    private float grade3Cooldown;
    private float grade4Cooldown;

    protected override void Awake()
    {
        base.Awake();
        stoneRenderer = GetComponent<Renderer>();
        stoneMaterial = stoneRenderer.material;
    }

    protected override void Update()
    {
        base.Update();
        
        grade1Cooldown -= Time.deltaTime;
        grade2Cooldown -= Time.deltaTime;
        grade3Cooldown -= Time.deltaTime;
        grade4Cooldown -= Time.deltaTime;
        
        if (grade1Input > 0.1f && grade1Cooldown <= 0)
            ExecuteGrade1Power();
        
        if (grade2Input > 0.1f && grade2Cooldown <= 0)
            ExecuteGrade2Power();
        
        if (grade3Input > 0.1f && grade3Cooldown <= 0)
            ExecuteGrade3Power();
        
        if (grade4Input > 0.1f && grade4Cooldown <= 0)
            ExecuteGrade4Power();
    }

    protected override void ExecuteGrade1Power()
    {
        // Presión controlada: mover objetos
        ApplyForce(forceMagnitude * 0.5f);
        grade1Cooldown = stats.powerCooldownGrade1;
        ConsumeResource(15f);
    }

    protected override void ExecuteGrade2Power()
    {
        // Impacto dirigido: destruir muros débiles
        DestroyWeakStructure();
        structuralDamage += 0.2f;
        grade2Cooldown = stats.powerCooldownGrade2;
        ConsumeResource(30f);
    }

    protected override void ExecuteGrade3Power()
    {
        // Carga estructural: dañar soporte de sala
        DamageStructure(0.5f);
        structuralDamage += 0.5f;
        grade3Cooldown = stats.powerCooldownGrade3;
        ConsumeResource(70f);
    }

    protected override void ExecuteGrade4Power()
    {
        // Desbalance de masa: alterar rotación
        AlterRotation();
        structuralDamage = 1f; // Daño completo
        grade4Cooldown = stats.powerCooldownGrade4;
        ConsumeResource(100f);
    }

    private void ApplyForce(float magnitude)
    {
        rb.velocity += transform.forward * magnitude;
        animator.SetTrigger("Push");
        internalGlow = 0.3f;
    }

    private void DestroyWeakStructure()
    {
        animator.SetTrigger("Impact");
        internalGlow = 0.6f;
        
        // Encontrar paredes destruibles
        Collider[] nearby = Physics.OverlapSphere(transform.position + transform.forward * 2f, destructionRadius);
        foreach (Collider col in nearby)
        {
            if (col.CompareTag("DestructibleWall"))
            {
                col.GetComponent<DestructibleWall>()?.Destroy();
            }
        }
    }

    private void DamageStructure(float damage)
    {
        animator.SetTrigger("Charge");
        internalGlow = 1.0f;
        
        // Dañar sala actual
        // Implementar según sistema de salas
    }

    private void AlterRotation()
    {
        animator.SetTrigger("MassShift");
        internalGlow = 1.5f;
        
        // Cambiar estado de rotación global
        RotationStateManager.Instance?.AlterRotation();
    }

    protected override void UpdateResourceVisualization()
    {
        float resourceRatio = currentResource / stats.maxResourceValue;
        
        // Vetas ocre se iluminan con más daño
        float veinGlow = Mathf.Lerp(0.3f, 1.5f, 1f - resourceRatio);
        stoneMaterial.SetFloat("_VeinGlow", veinGlow);
        
        // Grietas visibles con daño estructural
        stoneMaterial.SetFloat("_CrackIntensity", structuralDamage);
        
        // Color más gris con más daño
        Color targetColor = Color.Lerp(
            new Color(0.3f, 0.3f, 0.3f),
            new Color(0.15f, 0.15f, 0.15f),
            structuralDamage
        );
        
        stoneMaterial.SetColor("_Color", targetColor);
    }

    protected override void UpdateAnimatorParameters()
    {
        base.UpdateAnimatorParameters();
        animator.SetFloat("StructuralDamage", structuralDamage);
        animator.SetFloat("InternalGlow", internalGlow);
        
        // Reducir brillo interno con el tiempo
        internalGlow -= Time.deltaTime * 0.5f;
        internalGlow = Mathf.Max(0, internalGlow);
    }
}
```

---

### 5. LUMEN - El Lumínico

```csharp
using UnityEngine;

public class LumenController : PlayerCharacterController
{
    [Header("== LUMEN SPECIFIC ==")]
    [SerializeField] private float membraneDegradation = 0f; // 0-1
    [SerializeField] private float uncontrolledEmissionRate = 0.5f;
    
    private Renderer lumenRenderer;
    private Material lumenMaterial;
    private bool inCriticalState = false;
    private float uncontrolledEmissionTimer;
    
    // Cooldowns
    private float grade1Cooldown;
    private float grade2Cooldown;
    private float grade3Cooldown;
    private float grade4Cooldown;

    protected override void Awake()
    {
        base.Awake();
        lumenRenderer = GetComponent<Renderer>();
        lumenMaterial = lumenRenderer.material;
    }

    protected override void Update()
    {
        base.Update();
        
        // Gestionar emisión incontrolada
        if (inCriticalState)
        {
            uncontrolledEmissionTimer -= Time.deltaTime;
            if (uncontrolledEmissionTimer <= 0)
            {
                PerformUncontrolledEmission();
                uncontrolledEmissionTimer = uncontrolledEmissionRate;
            }
        }
        
        grade1Cooldown -= Time.deltaTime;
        grade2Cooldown -= Time.deltaTime;
        grade3Cooldown -= Time.deltaTime;
        grade4Cooldown -= Time.deltaTime;
        
        if (grade1Input > 0.1f && grade1Cooldown <= 0)
            ExecuteGrade1Power();
        
        if (grade2Input > 0.1f && grade2Cooldown <= 0)
            ExecuteGrade2Power();
        
        if (grade3Input > 0.1f && grade3Cooldown <= 0)
            ExecuteGrade3Power();
        
        if (grade4Input > 0.1f && grade4Cooldown <= 0)
            ExecuteGrade4Power();
    }

    protected override void ExecuteGrade1Power()
    {
        // Pulso de contacto: hackeo sin emisión visible
        HackSystem();
        grade1Cooldown = stats.powerCooldownGrade1;
        ConsumeResource(5f);
    }

    protected override void ExecuteGrade2Power()
    {
        // Emisión dirigida: filamento visible
        EmitFilament(4f);
        grade2Cooldown = stats.powerCooldownGrade2;
        ConsumeResource(15f);
        membraneDegradation += 0.15f;
    }

    protected override void ExecuteGrade3Power()
    {
        // Pulso de área: desactivar sistemas
        AreaPulse(4f);
        grade3Cooldown = stats.powerCooldownGrade3;
        ConsumeResource(40f);
        membraneDegradation += 0.35f;
    }

    protected override void ExecuteGrade4Power()
    {
        // Sobrecarga: todos los sistemas en radio
        Overload(8f);
        grade4Cooldown = stats.powerCooldownGrade4;
        ConsumeResource(100f);
        membraneDegradation = 1f; // Membrana crítica
        inCriticalState = true;
        uncontrolledEmissionTimer = 2f;
    }

    private void HackSystem()
    {
        animator.SetTrigger("HackContact");
        
        // Buscar sistema para hackear
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f))
        {
            if (hit.collider.CompareTag("System"))
            {
                // Hackear sin emisión visible
            }
        }
    }

    private void EmitFilament(float range)
    {
        animator.SetTrigger("EmitFilament");
        
        // Filamento visible que alcanza objetivo
        // Cegar cámaras, desactivar robots
        Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward * range, 2f);
        foreach (Collider target in targets)
        {
            if (target.CompareTag("Camera") || target.CompareTag("Robot"))
            {
                target.GetComponent<ElectricalSystem>()?.Disable();
            }
        }
    }

    private void AreaPulse(float radius)
    {
        animator.SetTrigger("AreaPulse");
        
        // Pulso que afecta todos los sistemas en radio
        Collider[] systems = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider sys in systems)
        {
            if (sys.CompareTag("System") || sys.CompareTag("Robot"))
            {
                sys.GetComponent<ElectricalSystem>()?.Disable(8f);
            }
        }
    }

    private void Overload(float radius)
    {
        animator.SetTrigger("Overload");
        
        // Destruir todos los sistemas en radio permanentemente
        Collider[] systems = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider sys in systems)
        {
            if (sys.CompareTag("System"))
            {
                sys.GetComponent<ElectricalSystem>()?.Destroy();
            }
        }
    }

    private void PerformUncontrolledEmission()
    {
        // Emisión aleatoria que activa sistemas
        Vector3 randomDir = Random.onUnitSphere;
        
        Collider[] nearby = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider col in nearby)
        {
            if (col.CompareTag("System") || col.CompareTag("Robot"))
            {
                col.GetComponent<ElectricalSystem>()?.Toggle();
            }
        }
    }

    protected override void UpdateResourceVisualization()
    {
        float resourceRatio = currentResource / stats.maxResourceValue;
        
        // Membrana: opacidad reduce con uso
        float membraneOpacity = Mathf.Lerp(0.2f, 0.9f, resourceRatio);
        lumenMaterial.SetFloat("_Opacity", membraneOpacity);
        
        // Color cambio: ámbar -> naranja -> rojo
        Color targetColor;
        if (resourceRatio > 0.66f)
            targetColor = Color.Lerp(
                new Color(1.0f, 0.8f, 0.2f), // Ámbar
                new Color(1.0f, 0.5f, 0.2f), // Naranja
                1f - (resourceRatio - 0.66f) / 0.34f
            );
        else if (resourceRatio > 0.33f)
            targetColor = Color.Lerp(
                new Color(1.0f, 0.5f, 0.2f), // Naranja
                new Color(1.0f, 0.2f, 0.2f), // Rojo
                1f - (resourceRatio - 0.33f) / 0.33f
            );
        else
            targetColor = Color.Lerp(
                new Color(1.0f, 0.2f, 0.2f), // Rojo
                new Color(1.0f, 1.0f, 1.0f), // Blanco cegador
                1f - resourceRatio / 0.33f
            );
        
        lumenMaterial.SetColor("_Color", targetColor);
        
        // Aura electromagnética
        float auraIntensity = Mathf.Lerp(0.1f, 2.0f, 1f - resourceRatio);
        lumenMaterial.SetFloat("_AuraIntensity", auraIntensity);
        
        // Destello interno si está crítico
        if (inCriticalState)
        {
            float flicker = Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f;
            lumenMaterial.SetFloat("_FlickerIntensity", flicker);
        }
    }

    protected override void UpdateAnimatorParameters()
    {
        base.UpdateAnimatorParameters();
        animator.SetFloat("MembraneDegradation", membraneDegradation);
        animator.SetBool("IsCritical", inCriticalState);
    }
}
```

---

## Sistema de Rotación

### RotationStateManager

```csharp
using UnityEngine;

public class RotationStateManager : MonoBehaviour
{
    public static RotationStateManager Instance { get; private set; }
    
    [SerializeField] private AnimationCurve rotationCurve;
    
    private RotationState currentState = RotationState.Normal;
    private float rotationValue = 0f; // 0-1, visual representation
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public RotationState GetCurrentState() => currentState;

    public void SetRotationState(RotationState newState)
    {
        currentState = newState;
        rotationValue = (int)newState / 3f; // 0, 0.33, 0.66, 1
        
        // Aplicar cambios a todos los personajes
        ApplyRotationEffects();
    }

    public void AlterRotation(int delta = 1)
    {
        int currentIndex = (int)currentState;
        int newIndex = Mathf.Clamp(currentIndex + delta, 0, 3);
        SetRotationState((RotationState)newIndex);
    }

    private void ApplyRotationEffects()
    {
        // Cambiar gravedad, resistencia del aire, etc según estado
        Physics.gravity = Vector3.down * GetGravityForState(currentState);
    }

    private float GetGravityForState(RotationState state)
    {
        return state switch
        {
            RotationState.Detenida => 0f,      // 0G
            RotationState.Baja => 5f,          // Gravedad reducida
            RotationState.Normal => 9.81f,     // Gravedad normal
            RotationState.Alta => 15f,         // Gravedad aumentada
            _ => 9.81f
        };
    }
}
```

---

## Interacciones Entre Personajes

### Matriz de Consecuencias

```csharp
using UnityEngine;

public class ConsequenceMatrix : MonoBehaviour
{
    // Ejemplo: STONE altera rotación -> FOLD saltos impredecibles
    public void OnStoneAltersRotation()
    {
        // Notificar a todos los FOLD activos
        FindObjectOfType<FoldController>()?.SetAnomalyUnpredictable(true);
    }

    // SWARM consume cable -> LUMEN hackeo falla
    public void OnSwarmConsumesSystem(ElectricalSystem system)
    {
        system.SetDamaged(true);
    }

    // MIST despresuriza -> STONE carga estructural colapsa
    public void OnMistDepressurizes(Room room)
    {
        room.SetStructuralCompromise(true);
    }

    // LUMEN sobrecarga -> SWARM hackeo imposible
    public void OnLumenOverloads(Room room)
    {
        room.SetAllSystemsInactive(true);
    }

    // FOLD plega espacio -> MIST conductos distorsionados
    public void OnFoldCollapsesSpace(Room room)
    {
        room.SetVentDuctsDistorted(true);
    }
}
```

---

## Checklist de Implementación

✅ **Compartido 100%:**
- [ ] PlayerInputActions.inputactions
- [ ] PlayerCharacterController base
- [ ] CharacterStats.asset
- [ ] RotationStateManager
- [ ] Animator parámetros estándar (Speed, Direction, IsMoving)
- [ ] BlendTree 8 direcciones
- [ ] Shared animations (Idle, Walk, Dash)

✅ **Específico por Personaje:**
- [ ] FoldController + visualización transparencia
- [ ] SwarmController + visualización color cables
- [ ] MistController + visualización compresión
- [ ] StoneController + visualización grietas
- [ ] LumenController + visualización membrana
- [ ] Animator Controller específico (c/u con sus grados)
- [ ] Modelos 3D
- [ ] Materiales únicos
- [ ] VFX específicos
- [ ] SFX específicos

✅ **Sistema de Salas:**
- [ ] DestructibleWall
- [ ] ElectricalSystem
- [ ] Room system con estados
- [ ] Vent duct network (para MIST)

---

## Notas de Diseño

1. **Sin UI Flotante:** La información de poder y recursos está siempre en la visión del jugador como cambios de color, opacidad, brillo.

2. **Cada Grado es Diferente:** No es solo potencia aumentada, es mecánica diferente.

3. **Consecuencias Permanentes:** El entorno cambia y nunca vuelve. Esto define decisiones estratégicas.

4. **Rotación Afecta Mecánicas:** No es cosmético, cada estado requiere enfoque táctico diferente.

5. **Personajes Incompatibles:** No se pueden usar dos simultáneamente, pero sus consecuencias persisten.
