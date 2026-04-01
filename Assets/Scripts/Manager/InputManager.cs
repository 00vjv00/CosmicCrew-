// ============================================================================
// INPUT MANAGER - GESTOR DE ENTRADA
// Archivo: Assets/Scripts/Manager/InputManager.cs
// Descripción: Lee input de forma DIRECTA y SIMPLE
// VERSIÓN: Ultra minimalista, sin Input System Actions
// ============================================================================

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// InputManager lee input de forma DIRECTA.
/// Sin complicaciones. Sin Input Actions. Solo Keyboard + Mouse.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    // ========================================================================
    // ESTADO DE INPUT
    // ========================================================================
    
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 touchPosition = Vector2.zero;
    private bool isTouching = false;
    
    // ========================================================================
    // EVENTOS
    // ========================================================================
    
    public event System.Action<Vector3> OnMovementInput;
    public event System.Action<Vector2> OnTouchBegan;
    public event System.Action<Vector2> OnTouchMoved;
    public event System.Action<Vector2> OnTouchEnded;
    public event System.Action OnPowerButtonPressed;
    public event System.Action OnPowerButtonReleased;
    public event System.Action OnInteractButtonPressed;
    
    // ========================================================================
    // INICIALIZACIÓN
    // ========================================================================
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[INPUT MANAGER] Duplicado. Destruyendo.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[INPUT MANAGER] Inicializado");
    }
    
    void Update()
    {
        // LEER INPUT DE FORMA DIRECTA
        ReadKeyboardInput();
        ReadMouseInput();
        ReadPowerInput();
        ReadInteractInput();
        
        // DEBUG: Mostrar estado de teclas cada frame
        if (Keyboard.current != null && (Keyboard.current.eKey.isPressed || Keyboard.current.eKey.wasPressedThisFrame))
        {
            Debug.LogWarning($"[INPUT DEBUG] E key state: isPressed={Keyboard.current.eKey.isPressed}, wasPressedThisFrame={Keyboard.current.eKey.wasPressedThisFrame}");
        }
    }
    
    // ========================================================================
    // LEER TECLADO
    // ========================================================================
    
    private void ReadKeyboardInput()
    {
        float horizontal = 0;
        float vertical = 0;
        
        // Leer teclas DIRECTAMENTE del Keyboard del Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                vertical += 1;
            if (Keyboard.current.sKey.isPressed)
                vertical -= 1;
            if (Keyboard.current.aKey.isPressed)
                horizontal -= 1;
            if (Keyboard.current.dKey.isPressed)
                horizontal += 1;
        }
        
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        
        // DEBUG: Log cuando hay input
        if (moveDirection.magnitude > 0.1f)
        {
            Debug.Log($"[INPUT] Keyboard input: {moveDirection}");
        }
        
        OnMovementInput?.Invoke(moveDirection);
    }
    
    // ========================================================================
    // LEER MOUSE
    // ========================================================================
    
    private void ReadMouseInput()
    {
        if (Mouse.current == null)
            return;
        
        // Click izquierdo
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isTouching = true;
            touchPosition = Mouse.current.position.ReadValue();
            Debug.Log($"[INPUT] Mouse DOWN: {touchPosition}");
            OnTouchBegan?.Invoke(touchPosition);
        }
        
        if (Mouse.current.leftButton.isPressed)
        {
            touchPosition = Mouse.current.position.ReadValue();
            OnTouchMoved?.Invoke(touchPosition);
        }
        
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isTouching = false;
            Debug.Log($"[INPUT] Mouse UP: {touchPosition}");
            OnTouchEnded?.Invoke(touchPosition);
        }
    }
    
    // ========================================================================
    // LEER PODER (ESPACIO)
    // ========================================================================
    
    private void ReadPowerInput()
    {
        if (Keyboard.current == null)
            return;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("[INPUT] Space pressed");
            OnPowerButtonPressed?.Invoke();
        }
        
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            Debug.Log("[INPUT] Space released");
            OnPowerButtonReleased?.Invoke();
        }
    }
    
    // ========================================================================
    // LEER INTERACCIÓN (E)
    // ========================================================================
    
    private void ReadInteractInput()
    {
        if (Keyboard.current == null)
        {
            Debug.LogWarning("[INPUT] Keyboard.current is NULL!");
            return;
        }
        
        // DEBUG: Siempre loguear si E está siendo presionado
        if (Keyboard.current.eKey.isPressed)
        {
            Debug.LogWarning($"[INPUT DEBUG] E is pressed (wasPressedThisFrame={Keyboard.current.eKey.wasPressedThisFrame})");
        }
        
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[INPUT] E pressed - Interact");
            OnInteractButtonPressed?.Invoke();
        }
    }
    
    // ========================================================================
    // QUERIES
    // ========================================================================
    
    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }
    
    public Vector2 GetTouchPosition()
    {
        return touchPosition;
    }
    
    public bool IsTouching()
    {
        return isTouching;
    }
    
    public bool HasMovementInput()
    {
        return moveDirection.magnitude > 0.1f;
    }
    
    // ========================================================================
    // DEBUG
    // ========================================================================
    
    public void DebugPrintInputState()
    {
        Debug.Log("[INPUT MANAGER STATE]");
        Debug.Log($"  Move Direction: {moveDirection}");
        Debug.Log($"  Is Touching: {isTouching}");
        Debug.Log($"  Touch Position: {touchPosition}");
        Debug.Log($"  Keyboard available: {Keyboard.current != null}");
        Debug.Log($"  Mouse available: {Mouse.current != null}");
    }
}
