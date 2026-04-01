// ============================================================================
// INSTRUCCIONES PARA CONECTAR LA ESCALERA
// Cosmic Crew - Setup de Escalera en la Escena
// ============================================================================

/*
PASOS PARA CONECTAR COOR MINI CON LA ESCALERA:

1. CREAR ZONA PISO 1 (Si no existe)
   └─ GameObject "Piso 1" o "Floor1"
      └─ Componente: DynamicZone
      └─ Collider: BoxCollider (Trigger)
      └─ Assets visuales (paredes, suelo)

2. CREAR ZONA PISO 2 (Si no existe)
   └─ GameO bject "Piso 2" o "Floor2"
      └─ Componente: DynamicZone
      └─ Collider: BoxCollider (Trigger)

3. CREAR SCALERAS COMO TRANSICIÓN
   └─ GameObject "Escalera"
      ├─ Collider: BoxCollider (Trigger) ← Esto es el trigger de escalera
      ├─ Componente: Stair.cs
      └─ Visual (mesh de escalera opcional)
      
   CONFIGURAR STAIR EN INSPECTOR:
   - Config Floor A → Arrastra Coor Mini
   - Config Floor B → Arrastra Piso 1
   - Position At Floor A → (5, 0, 10) ejemplo
   - Position At Floor B → (5, 3, 11) ejemplo con Y más alto
   - Stair Trigger → El BoxCollider del GameObject Escalera
   - Climb Duration → 2 segundos

4. CREAR PUERTA DE ACCESO (para conectar en el grafo)
   └─ GameObject "Door_CoórMini_Escalera"
      ├─ Collider: Trigger (visual de la puerta)
      ├─ Componente: Door.cs
      └─ Componente: DoorConfig
      
   CONFIGURAR DOOR EN INSPECTOR:
   - Door Name → "Escalera CoórMini"
   - Door Type → Sliding (rápida)
   - Zone A → Coor Mini
   - Zone B → Piso 1
   - Position A → (5, 0, 10) entrada en Coor Mini
   - Position B → (5, 3, 10) salida en Piso 1

LÓGICA DE FLUJO:
═════════════════════════════════════════════════════════════════

1. Player está en Coor Mini
2. Pasa por Door_CoórMini_Escalera → Genera conexión en grafo
3. Toca el trigger Escalera → Se dispara Stair.OnTriggerEnter()
4. Stair.cs:
   ├─ Detecta que va de Coor Mini (Piso 0) a Piso 1
   ├─ Activa CameraController.EnterStairMode() → Vista de perfil
   ├─ Mueve al player de posición A a B suavemente
   └─ Cambia zona: Coor Mini → Piso 1
5. Player sale del trigger Escalera
6. CameraController.ExitStairMode() → Vuelve a vista isométrica

PUNTOS CLAVE:
═════════════════════════════════════════════════════════════════

✓ La Door no se abre/cierra → solo conecta zonas para el grafo BFS
✓ La Stair maneja el movimiento suave del player
✓ La cámara automáticamente pasa a modo perfil
✓ El player puede subir y bajar la escalera múltiples veces
✓ Las zonas deben estar a diferente Y para que se vea el efecto

JERARQUÍA RECOMENDADA EN ESCENA:
═════════════════════════════════════════════════════════════════

Scene/
├─ CoórMini/                    ← Zona Piso 0
│  ├─ Geometry (suelo, paredes)
│  ├─ ZoneTrigger (BoxCollider trigger)
│  ├─ DynamicZone.cs
│  └─ Door_CoórMini_Escalera    ← Acceso a escalera
│     ├─ Collider (pequeño, area entrada)
│     └─ Door.cs + DoorConfig
│
├─ Escalera/                    ← Transición
│  ├─ Geometry (visuales escalera)
│  ├─ StairTrigger (BoxCollider trigger)
│  └─ Stair.cs
│
├─ Piso1/                       ← Zona Piso 1
│  ├─ Geometry (suelo, paredes)
│  ├─ ZoneTrigger (BoxCollider trigger)
│  ├─ DynamicZone.cs
│  └─ Door_Escalera_Piso1       ← Acceso desde escalera (A ← B)
│     ├─ Collider (pequeño, area salida)
│     └─ Door.cs + DoorConfig
│
└─ Piso2/                       ← Zona Piso 2
   └─ ...
*/
