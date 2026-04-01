using UnityEngine;

public class RayoLaser : MonoBehaviour
{
    [SerializeField] private Vector3 direccion = Vector3.right;
    [SerializeField] private float velocidad = 20f;
    [SerializeField] private float longitud = 50f;
    [SerializeField] private Color colorLaser = Color.red;
    [SerializeField] private float danio = 10f;
    
    private LineRenderer lineRenderer;
    private float posicion = 0f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.startColor = colorLaser;
        lineRenderer.endColor = colorLaser;
        
        Material laserMat = new Material(Shader.Find("Sprites/Default"));
        laserMat.color = colorLaser;
        lineRenderer.material = laserMat;
    }

    void Update()
    {
        posicion += velocidad * Time.deltaTime;
        if (posicion > longitud)
            posicion = 0f;

        Vector3 puntoInicio = transform.position;
        Vector3 puntoFinal = puntoInicio + direccion * longitud;

        // Raycast principal
        RaycastHit hit;
        if (Physics.Raycast(puntoInicio, direccion.normalized, out hit, longitud))
        {
            puntoFinal = hit.point;
            
            // Si toca columna prisma, dividir rayo
            if (hit.collider.CompareTag("ColumnaPrismaTag"))
            {
                Debug.Log($"[LÁSER] Toca prisma: {hit.collider.gameObject.name}");
                DibujarRayoDividido(puntoInicio, hit.point, hit.normal);
                return;  // No dibujar rayo normal
            }
        }

        // Dibujar rayo normal
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, puntoInicio);
        lineRenderer.SetPosition(1, puntoFinal);
    }

    void DibujarRayoDividido(Vector3 puntoInicio, Vector3 puntoPrisma, Vector3 normalPrisma)
    {
        // Rayos divididos a 45 y -45 grados
        Vector3 dir45 = Quaternion.AngleAxis(45f, Vector3.up) * direccion;
        Vector3 dirMinus45 = Quaternion.AngleAxis(-45f, Vector3.up) * direccion;

        // Rayo original hasta prisma
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, puntoInicio);
        lineRenderer.SetPosition(1, puntoPrisma);

        // Rayo 1: +45 grados
        RaycastHit hit1;
        Vector3 fin1 = puntoPrisma + dir45 * longitud;
        if (Physics.Raycast(puntoPrisma, dir45.normalized, out hit1, longitud))
        {
            fin1 = hit1.point;
        }
        
        // Dibujar segundo rayo (expandir LineRenderer)
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(2, puntoPrisma);
        lineRenderer.SetPosition(3, fin1);

        // Rayo 2: -45 grados (crear LineRenderer adicional)
        RaycastHit hit2;
        Vector3 fin2 = puntoPrisma + dirMinus45 * longitud;
        if (Physics.Raycast(puntoPrisma, dirMinus45.normalized, out hit2, longitud))
        {
            fin2 = hit2.point;
        }

        // Crear LineRenderer adicional para segundo rayo dividido
        LineRenderer lr2 = GetOrCreateLineRenderer("RayoDividido2");
        lr2.positionCount = 2;
        lr2.SetPosition(0, puntoPrisma);
        lr2.SetPosition(1, fin2);
    }

    LineRenderer GetOrCreateLineRenderer(string nombre)
    {
        LineRenderer lr = transform.Find(nombre)?.GetComponent<LineRenderer>();
        
        if (lr == null)
        {
            GameObject obj = new GameObject(nombre);
            obj.transform.SetParent(transform);
            lr = obj.AddComponent<LineRenderer>();
            lr.startWidth = 0.2f;
            lr.endWidth = 0.2f;
            lr.startColor = colorLaser;
            lr.endColor = colorLaser;
            
            Material laserMat = new Material(Shader.Find("Sprites/Default"));
            laserMat.color = colorLaser;
            lr.material = laserMat;
        }

        return lr;
    }
}