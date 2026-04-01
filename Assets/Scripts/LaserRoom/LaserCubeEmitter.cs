// ============================================================================
// LASER CUBE EMITTER - Cubo que emite rayos láser
// Archivo: Assets/Scripts/LaserRoom/LaserCubeEmitter.cs
// Descripción: Cubo prototipo que emite rayos láser en una dirección.
//              Los rayos rebotan en prismas y se visualizan con LineRenderer.
// ============================================================================

using UnityEngine;

/// <summary>
/// Emite rayos láser desde un cubo.
/// Los rayos rebotan en objetos con tag "Prisma".
/// </summary>
public class LaserCubeEmitter : MonoBehaviour
{
    // ────────────────────────────────────────────────────────
    // CONFIGURACIÓN
    // ────────────────────────────────────────────────────────
    
    [Header("Emisor")]
    [SerializeField] private Vector3 emitDirection = Vector3.right;
    [SerializeField] private float raycastLength = 60f;
    
    [Header("Visual")]
    [SerializeField] private Color laserColor = Color.red;
    [SerializeField] private float lineWidth = 0.4f;
    [SerializeField] private Material laserMaterial;
    
    // ────────────────────────────────────────────────────────
    // REFERENCIAS
    // ────────────────────────────────────────────────────────
    
    private LineRenderer lineRenderer;
    private MeshRenderer cubeRenderer;
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Awake()
    {
        // Crear cubo visible (0.5 × 0.5 × 0.5)
        if (GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            
            cubeRenderer = GetComponent<MeshRenderer>();
            Material cubeMat = new Material(Shader.Find("Standard"));
            cubeMat.color = new Color(0.2f, 0.2f, 0.2f);
            cubeRenderer.material = cubeMat;
        }
        
        // Escala del cubo
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // LineRenderer para el rayo
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
        
        if (laserMaterial != null)
        {
            lineRenderer.material = laserMaterial;
        }
        else
        {
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = laserColor;
            lineRenderer.material = mat;
        }
        
        Debug.Log($"[LASER EMITTER] Inicializado en {transform.position}, dirección: {emitDirection}");
    }
    
    // ────────────────────────────────────────────────────────
    // UPDATE
    // ────────────────────────────────────────────────────────
    
    private void Update()
    {
        DrawLaser();
    }
    
    // ────────────────────────────────────────────────────────
    // LÓGICA DE RAYOS
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Dibuja el rayo láser con reflexiones en prismas
    /// </summary>
    private void DrawLaser()
    {
        Vector3 rayStart = transform.position;
        Vector3 rayDir = emitDirection.normalized;
        
        // Iniciar con 2 posiciones (inicio y fin)
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, rayStart);
        
        // Raycast simple (sin reflexión de momento)
        RaycastHit hit;
        Vector3 rayEnd = rayStart + rayDir * raycastLength;
        
        if (Physics.Raycast(rayStart, rayDir, out hit, raycastLength))
        {
            rayEnd = hit.point;
            
            // Si ha golpeado un prisma, dibujar reflexión
            if (hit.collider.CompareTag("Prisma"))
            {
                DrawReflection(hit);
                return;
            }
        }
        
        lineRenderer.SetPosition(1, rayEnd);
    }
    
    /// <summary>
    /// Dibuja el rayo reflejado en un prisma
    /// </summary>
    private void DrawReflection(RaycastHit initialHit)
    {
        // Línea hasta el prisma
        lineRenderer.SetPosition(1, initialHit.point);
        
        // Obtener ángulo de rotación del prisma
        PrismColumn prism = initialHit.collider.GetComponent<PrismColumn>();
        if (prism == null)
        {
            return;
        }
        
        float prismAngle = prism.GetCurrentAngle();
        
        // Calcular reflexiones según el ángulo del prisma
        // 0° → reflexión derecha
        // 45° → reflexión diagonal
        // 90° → reflexión hacia arriba
        Vector3 reflectedDir = GetReflectedDirection(initialHit.normal, prismAngle);
        
        // Segunda línea (reflejada)
        Vector3 reflectionStart = initialHit.point;
        Vector3 reflectionEnd = reflectionStart + reflectedDir * raycastLength;
        
        RaycastHit secondHit;
        if (Physics.Raycast(reflectionStart, reflectedDir, out secondHit, raycastLength))
        {
            reflectionEnd = secondHit.point;
        }
        
        // Expandir LineRenderer para mostrar ambas líneas
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(1, initialHit.point);
        lineRenderer.SetPosition(2, reflectionEnd);
    }
    
    /// <summary>
    /// Calcula la dirección reflejada basada en el ángulo del prisma
    /// </summary>
    private Vector3 GetReflectedDirection(Vector3 normal, float prismAngle)
    {
        // Conversión simple: ángulo del prisma → dirección reflejada
        // 0° = reflejado a la derecha (X+)
        // 45° = diagonal (X+ Z+)
        // 90° = reflejado hacia arriba conceptualmente pero en 2D (Z+)
        
        float angleRad = prismAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)).normalized;
    }
    
    // ────────────────────────────────────────────────────────
    // SETTERS
    // ────────────────────────────────────────────────────────
    
    public void SetEmitDirection(Vector3 newDirection)
    {
        emitDirection = newDirection.normalized;
    }
    
    public void SetLaserColor(Color newColor)
    {
        laserColor = newColor;
        if (lineRenderer != null)
        {
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;
        }
    }
}
