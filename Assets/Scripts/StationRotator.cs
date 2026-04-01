using UnityEngine;

public class StationRotator : MonoBehaviour
{
    [Header("Rotación angular (grados/seg)")]
    public float velocidadRotacion = 0.01f;
    [Header("¿Rotar?")]
    public bool rotar = true;
    [Header("¿Reversa?")]
    public bool reversa = false;

    void Update()
    {
        if (rotar)
        {
            float sentido = reversa ? -1f : 1f;
            transform.Rotate(Vector3.right, velocidadRotacion * sentido * Time.deltaTime, Space.World);
        }
    }

    // Métodos para controlar la rotación desde otros scripts
    public void PausarRotacion() => rotar = false;
    public void ReanudarRotacion() => rotar = true;
    public void InvertirSentido() => reversa = !reversa;
    public void SetVelocidad(float nuevaVelocidad) => velocidadRotacion = nuevaVelocidad;
}
