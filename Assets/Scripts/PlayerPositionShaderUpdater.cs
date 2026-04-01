using UnityEngine;

/// <summary>
/// Actualiza la posición del jugador como uniform global en los shaders.
/// Permite que los shaders de fade dinámico detecten proximidad al jugador.
/// </summary>
public class PlayerPositionShaderUpdater : MonoBehaviour
{
    private Transform playerTransform;
    
    void Start()
    {
        // Busca el player por tag o por nombre
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            playerObj = GameObject.Find("Player");
        }
        
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("PlayerPositionShaderUpdater: No se encontró el jugador. Asigna el tag 'Player' al objeto del jugador.");
        }
    }
    
    void Update()
    {
        if (playerTransform != null)
        {
            // Envía la posición del jugador a TODOS los shaders como uniform global
            Shader.SetGlobalVector("_PlayerPos", playerTransform.position);
        }
    }
}
