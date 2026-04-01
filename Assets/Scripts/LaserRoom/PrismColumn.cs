// ============================================================================
// PRISM COLUMN - Columna rotable que refleja rayos
// Archivo: Assets/Scripts/LaserRoom/PrismColumn.cs
// Descripción: Columna que refleja rayos láser. Puede rotar entre 0°, 45°, 90°.
// ============================================================================

using UnityEngine;

/// <summary>
/// Columna prismática que refleja rayos láser.
/// Puede cambiar su ángulo de reflexión (0°, 45°, 90°) para crear
/// diferentes patrones de reflejos.
/// </summary>
public class PrismColumn : MonoBehaviour
{
    // ────────────────────────────────────────────────────────
    // ESTADO
    // ────────────────────────────────────────────────────────
    
    private float currentAngle = 0f;  // 0°, 45°, 90°
    private float targetAngle = 0f;
    
    // ────────────────────────────────────────────────────────
    // CONFIGURACIÓN
    // ────────────────────────────────────────────────────────
    
    [Header("Dimensiones")]
    [SerializeField] private float prismRadius = 0.5f;
    [SerializeField] private float prismHeight = 3f;
    
    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 180f;  // grados/segundo
    [SerializeField] private bool rotateSmooth = true;
    
    [Header("Visual")]
    [SerializeField] private Color colorIdle = Color.white;
    [SerializeField] private Color colorActive = Color.cyan;
    [SerializeField] private Color colorWarning = Color.yellow;
    
    // ────────────────────────────────────────────────────────
    // REFERENCIAS
    // ────────────────────────────────────────────────────────
    
    private MeshRenderer meshRenderer;
    private Material prismMaterial;
    private bool isRotating = false;
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Awake()
    {
        // Crear cilindro (prototipo de columna)
        if (GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
        }
        
        // Material de la columna
        meshRenderer = GetComponent<MeshRenderer>();
        prismMaterial = new Material(Shader.Find("Standard"));
        prismMaterial.color = colorIdle;
        meshRenderer.material = prismMaterial;
        
        // Escala (cilindro: radio × altura)
        transform.localScale = new Vector3(
            prismRadius * 2f,
            prismHeight * 0.5f,
            prismRadius * 2f
        );
        
        // Agregar collider
        if (GetComponent<CapsuleCollider>() == null)
        {
            CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
            col.radius = prismRadius;
            col.height = prismHeight;
        }
        
        // Tag para que los rayos lo reconozcan
        gameObject.tag = "Prisma";
        
        currentAngle = Random.Range(0, 3) * 45f;  // Random entre 0°, 45°, 90°
        targetAngle = currentAngle;
        
        Debug.Log($"[PRISMA] Columna creada en {transform.position}, ángulo inicial: {currentAngle}°");
    }
    
    // ────────────────────────────────────────────────────────
    // UPDATE
    // ────────────────────────────────────────────────────────
    
    private void Update()
    {
        if (rotateSmooth && !Mathf.Approximately(currentAngle, targetAngle))
        {
            RotateSmooth();
        }
    }
    
    // ────────────────────────────────────────────────────────
    // ROTACIÓN
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Rota suavemente hacia el ángulo objetivo
    /// </summary>
    private void RotateSmooth()
    {
        float step = rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, step);
        
        // Aplicar rotación visual
        transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
        
        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            isRotating = false;
            SetColor(colorIdle);
        }
    }
    
    /// <summary>
    /// Cambia el ángulo objetivo a uno de los 3 permitidos (0°, 45°, 90°)
    /// </summary>
    public void SetAngle(float newAngle)
    {
        // Normalizar a uno de: 0°, 45°, 90°
        newAngle = Mathf.Round(newAngle / 45f) * 45f;
        newAngle = Mathf.Clamp(newAngle, 0f, 90f);
        
        if (!Mathf.Approximately(targetAngle, newAngle))
        {
            targetAngle = newAngle;
            isRotating = true;
            SetColor(colorActive);
            Debug.Log($"[PRISMA] Rotando a {newAngle}°");
        }
    }
    
    /// <summary>
    /// Rota a todos los ángulos cíclicamente
    /// </summary>
    public void RotateCycle()
    {
        float[] angles = { 0f, 45f, 90f };
        int currentIndex = (int)(currentAngle / 45f);
        int nextIndex = (currentIndex + 1) % 3;
        
        SetAngle(angles[nextIndex]);
    }
    
    // ────────────────────────────────────────────────────────
    // VISUAL
    // ────────────────────────────────────────────────────────
    
    private void SetColor(Color color)
    {
        if (prismMaterial != null)
        {
            prismMaterial.color = color;
        }
    }
    
    public void SetColorActive()
    {
        SetColor(colorActive);
    }
    
    public void SetColorWarning()
    {
        SetColor(colorWarning);
    }
    
    public void SetColorIdle()
    {
        SetColor(colorIdle);
    }
    
    // ────────────────────────────────────────────────────────
    // GETTERS
    // ────────────────────────────────────────────────────────
    
    public float GetCurrentAngle()
    {
        return currentAngle;
    }
    
    public float GetTargetAngle()
    {
        return targetAngle;
    }
    
    public bool IsRotating()
    {
        return isRotating;
    }
}
