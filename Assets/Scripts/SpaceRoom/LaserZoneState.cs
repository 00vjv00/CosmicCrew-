// LaserZoneState.cs
// Tipos compartidos por el sistema de zonas láser.
// Incluir en el proyecto, no necesita MonoBehaviour.

public enum LaserState
{
    Lethal,   // Rojo  — mata (daño por segundo)
    Slow,     // Naranja — frena al jugador
}

/// <summary>
/// Identifica a qué zona pertenece un LaserPoint.
/// El Manager asigna zonas en el momento de generar la grid.
/// </summary>
public enum LaserZoneID
{
    ZoneA,  // Cuadrante NW
    ZoneB,  // Cuadrante NE
    ZoneC,  // Cuadrante SW
    ZoneD,  // Cuadrante SE
}
