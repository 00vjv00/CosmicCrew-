// ============================================================================
// ELEVATOR - Ascensores multi-piso
// Cosmic Crew - Sistema de Zonas Event-Driven
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ascensor que conecta múltiples pisos
/// Pre-carga el piso destino cuando se llama
/// </summary>
public class Elevator : MonoBehaviour
{
    [System.Serializable]
    public class FloorStop
    {
        public int floorNumber;
        public DynamicZone zone;
        public Vector3 elevatorExitPos;
    }
    
    public string elevatorName = "Elevator";
    public List<FloorStop> floors = new();
    
    private int currentFloor = 0;
    private bool isMoving = false;
    
    private ZoneManager zoneManager;
    
    private void Awake()
    {
        zoneManager = Object.FindAnyObjectByType<ZoneManager>();
    }
    
    /// <summary>
    /// Llamar elevador a un piso
    /// </summary>
    public void CallFloor(int floorNumber)
    {
        if (isMoving)
        {
            Debug.Log($"[ELEVATOR] {elevatorName} is already moving", gameObject);
            return;
        }
        
        FloorStop targetFloor = floors.Find(f => f.floorNumber == floorNumber);
        if (targetFloor == null)
        {
            Debug.LogWarning($"[ELEVATOR] Floor {floorNumber} not found", gameObject);
            return;
        }
        
        isMoving = true;
        
        // PRE-CARGAR: Avisar al ZoneManager que pre-cargue el piso destino
        if (zoneManager != null)
        {
            zoneManager.OnElevatorCalled(this, floorNumber);
        }
        
        // Animación de movimiento
        StartCoroutine(MoveToFloor(targetFloor));
    }
    
    private System.Collections.IEnumerator MoveToFloor(FloorStop stop)
    {
        // Simulación: tomar 2 segundos en llegar
        yield return new WaitForSeconds(2f);
        
        currentFloor = stop.floorNumber;
        isMoving = false;
        
        Debug.Log($"[ELEVATOR] {elevatorName} arrived at floor {currentFloor}");
    }
    
    /// <summary>
    /// Obtener zonas en el piso especificado
    /// </summary>
    public List<DynamicZone> GetZonesOnFloor(int floorNumber)
    {
        var result = new List<DynamicZone>();
        foreach (var floor in floors)
        {
            if (floor.floorNumber == floorNumber)
                result.Add(floor.zone);
        }
        return result;
    }
}
