using UnityEngine;

public class LaserBarrido : MonoBehaviour
{
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float offsetX = 0f;  // Posición base en X
    [SerializeField] private float offsetZ = 0f;  // Posición base en Z
    [SerializeField] private float posMin = -9f;  // Mín movimiento en X
    [SerializeField] private float posMax = 9f;   // Máx movimiento en X
    [SerializeField] private Material laserMaterial;

    private float posActual;
    private float direccion = 1f;
    private LineRenderer lineRenderer;

    void Start()
    {
        posActual = posMin;
        lineRenderer = GetComponent<LineRenderer>();
        
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        
        if (laserMaterial != null)
        {
            lineRenderer.material = laserMaterial;
        }
        else
        {
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = Color.red;
            lineRenderer.material = mat;
        }
    }

    void Update()
    {
        posActual += velocidad * direccion * Time.deltaTime;

        if (posActual >= posMax)
        {
            posActual = posMax;
            direccion = -1f;
        }

        if (posActual <= posMin)
        {
            posActual = posMin;
            direccion = 1f;
        }

        // Posición del GameObject (punto de pivote)
        transform.position = new Vector3(offsetX + posActual, 1.5f, offsetZ);
        
        // Rayo que barre verticalmente (a lo largo de Z)
        // Empieza en Z = -9 y termina en Z = +9 (relativo a offsetZ)
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, new Vector3(offsetX + posActual, 2f, offsetZ - 9f));
            lineRenderer.SetPosition(1, new Vector3(offsetX + posActual, 2f, offsetZ + 9f));
        }
    }
    
    // ════════════════════════════════════════
    // API para configurar desde el Manager
    // ════════════════════════════════════════
    
    public void SetBasePosition(float baseX, float baseZ)
    {
        offsetX = baseX;
        offsetZ = baseZ;
    }
    
    public void SetSpeed(float newSpeed)
    {
        velocidad = newSpeed;
    }
    
    public void SetLaserColor(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}