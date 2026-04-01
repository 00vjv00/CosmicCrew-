// ============================================================================
// CRATE DOOR - Una puerta individual de un crate
// Cada puerta rota sobre su pivote local cuando se abre
// ============================================================================

using UnityEngine;

/// <summary>
/// Eje de rotación para la puerta
/// </summary>
public enum RotationAxisType
{
    X,
    Y,
    Z
}

/// <summary>
/// Representa una puerta de un crate que rota sobre su pivote local.
/// Una instancia por puerta (izquierda, derecha).
/// </summary>
public class CrateDoor : MonoBehaviour
{
    [SerializeField] private RotationAxisType rotationAxis = RotationAxisType.Z;  // Eje de rotación
    [SerializeField] private float openAngle = 90f;                  // Ángulo de apertura (grados)
    [SerializeField] private float openSpeed = 90f;                  // Velocidad de rotación (grados/segundo)
    [SerializeField] private bool startOpen = false;                 // ¿Comienza abierta?
    [SerializeField] private Vector3 pivotOffset = Vector3.zero;     // Posición de la bisagra relativa al GameObject (LOCAL)
    [SerializeField] private bool invertDirection = false;           // Invertir dirección de rotación
    
    // ────────────────────────────────────────────────────────────
    // ESTADO INTERNO
    // ────────────────────────────────────────────────────────────
    
    private Quaternion closedRotation;   // Rotación inicial (cerrada)
    private Quaternion openedRotation;   // Rotación final (abierta)
    private Quaternion targetRotation;   // Hacia dónde rotar
    private Vector3 closedPosition;      // Posición inicial
    private bool isOpen = false;
    private bool isRotating = false;
    
    // ────────────────────────────────────────────────────────────
    // PROPIEDADES
    // ────────────────────────────────────────────────────────────
    
    public bool IsOpen => isOpen;
    public bool IsRotating => isRotating;
    
    // ────────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────────
    
    private void Awake()
    {
        // Guardar rotación cerrada (inicial)
        closedRotation = transform.localRotation;
        closedPosition = transform.localPosition;
        
        // Convertir enum a Vector3
        Vector3 axis = GetAxisVector(rotationAxis);
        
        // Aplicar inversión si está activada
        float finalAngle = invertDirection ? openAngle : -openAngle;
        
        // Calcular rotación abierta: rotar openAngle grados alrededor del eje local
        openedRotation = closedRotation * Quaternion.AngleAxis(finalAngle, axis);
        
        targetRotation = startOpen ? openedRotation : closedRotation;
        isOpen = startOpen;
        
        Debug.Log($"[CRATE DOOR] {gameObject.name} initialized (open angle: {finalAngle}°, axis: {rotationAxis}, invert: {invertDirection}, pivot offset: {pivotOffset})", gameObject);
    }
    
    /// <summary>
    /// Convertir enum a Vector3
    /// </summary>
    private Vector3 GetAxisVector(RotationAxisType axis)
    {
        return axis switch
        {
            RotationAxisType.X => Vector3.right,
            RotationAxisType.Y => Vector3.up,
            RotationAxisType.Z => Vector3.forward,
            _ => Vector3.forward
        };
    }
    
    // ────────────────────────────────────────────────────────────
    // ACTUALIZACIÓN
    // ────────────────────────────────────────────────────────────
    
    private void Update()
    {
        if (!isRotating) return;
        
        // Rotar suavemente hacia la rotación objetivo
        float rotationStep = openSpeed * Time.deltaTime;
        Quaternion newRotation = Quaternion.RotateTowards(
            transform.localRotation,
            targetRotation,
            rotationStep
        );
        
        // Si hay un pivotOffset, hacer que la puerta orbite alrededor del pivote
        if (pivotOffset != Vector3.zero)
        {
            // Diferencia de rotación desde el estado cerrado
            Quaternion rotationDelta = newRotation * Quaternion.Inverse(closedRotation);
            
            // Vector desde pivote a la posición inicial
            Vector3 pivotToInitialPos = closedPosition - pivotOffset;
            
            // Rotar ese vector
            Vector3 pivotToNewPos = rotationDelta * pivotToInitialPos;
            
            // Nueva posición = pivote + vector rotado
            transform.localPosition = pivotOffset + pivotToNewPos;
        }
        
        // Aplicar rotación
        transform.localRotation = newRotation;
        
        // Verificar si se alcanzó la rotación
        float angleDifference = Quaternion.Angle(transform.localRotation, targetRotation);
        if (angleDifference < 0.1f)
        {
            transform.localRotation = targetRotation;
            isRotating = false;
            
            Debug.Log($"[CRATE DOOR] {gameObject.name} rotation completed", gameObject);
        }
    }
    
    // ────────────────────────────────────────────────────────────
    // MÉTODOS PÚBLICOS
    // ────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Abrir la puerta (rotar hasta openAngle grados)
    /// </summary>
    public void Open()
    {
        if (isOpen && !isRotating) return;  // Ya está abierta y no está rotando
        
        targetRotation = openedRotation;
        isOpen = true;
        isRotating = true;
        
        Debug.Log($"[CRATE DOOR] Opening {gameObject.name}", gameObject);
    }
    
    /// <summary>
    /// Cerrar la puerta (rotar hacia posición inicial)
    /// </summary>
    public void Close()
    {
        if (!isOpen && !isRotating) return;  // Ya está cerrada y no está rotando
        
        targetRotation = closedRotation;
        isOpen = false;
        isRotating = true;
        
        Debug.Log($"[CRATE DOOR] Closing {gameObject.name}", gameObject);
    }
    
    /// <summary>
    /// Abrir o cerrar alternativamente
    /// </summary>
    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }
}
