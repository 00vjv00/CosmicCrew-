using UnityEngine;

public class PlanoController : MonoBehaviour
{
    // Referencia al generador y mi índice
    public PrisonStationGenerator stationGenerator;
    public int miIndice;

    void OnCollisionEnter(Collision collision)
    {
        // Si el objeto que colisiona es el jugador
        if (collision.gameObject.CompareTag("Player") && stationGenerator != null)
        {
            stationGenerator.ActualizarPlanosActivos(miIndice);
        }
    }

    // Lógica específica de cada plano
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Activar()
    {
        gameObject.SetActive(true);
        if (meshRenderer != null)
            meshRenderer.enabled = true;
    }

    public void Desactivar()
    {
        if (meshRenderer != null)
            meshRenderer.enabled = false;
        gameObject.SetActive(false);
    }
}
