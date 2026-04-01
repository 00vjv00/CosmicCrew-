# Análisis de flujo_manual.txt y Plan de Implementación

## 📋 RESUMEN EJECUTIVO

El documento `flujo_manual.txt` define el flujo del Capítulo I: "Escape del Sector Alta Seguridad". El primer punto de entrada es **AS_Cell_0** (Celda 0), donde el jugador comienza después de una cinemática de terremoto provocado por un asteroide.

---

## 🎬 PRIMER ACTO: AS_Cell_0

### Descripción Actual (del documento)
```
AS_Cell_0 — Celda 0
Enemies: 1 low
|-- CutScene: Liberación por golpe de asteroide y fallo técnico
|-- Tutorial: Movimiento básico
|-- Tutorial: Combate + Poder ofensivo -> Tutorial adquisición código --> AS_p1
|-- [AS_p1] Abrir la puerta
|-- Encontrar código --> AS_p2
|-- <Poder defensivo> Camino oculto -> AS_Secreto_A
```

### Propuestas de Cambios

#### 1. **Estructura de la Sala**
La sala debe tener:
- **Zona de Inicio**: Donde despierta el jugador (fuera de los escombros del asteroide)
- **Zona de Combate**: Donde está el Enemy Low acorralado
- **Zona de Puerta**: Mecanismo de puerta que requiere código AS_p1
- **Zona Secreta**: Accesible solo con poder defensivo
- **Zona de Salida**: AS_AccC_0

#### 2. **Enemy Low - Especificación Inicial**
```
Nombre: Low Robot / Soldier
Tipo: Enemigo básico de patrulla
Salud: 20 puntos
Daño: 5 por golpe
Rango de Patrulla: 40m (configurable)
Rango de Persecución: 40m
Rango de Ataque: 5m
Estados: Idle, Pursuing, Attacking, Dead
Comportamiento: 
  - Inicia en patrulla aleatoria
  - Al detectar al jugador: persigue
  - En combate: ataca cuerpo a cuerpo
  - Sin línea de vista: abandona tras 5 segundos
```

#### 3. **Progresión de Tutoriales**
Propuesta de orden recomendado:
1. **Movimiento** (inmediato al despertar)
   - Tutorial passivo: moverse con W/A/S/D
   - Objetivo: llegar a cierta zona
2. **Detección** (al acercarse al Enemy Low)
   - UI: "Detectado enemigo"
3. **Combate** (encuentro con el Low Robot)
   - Tutorial: botón de ataque
   - Sin poder aún (puño básico)
4. **Poder Ofensivo** (tras primera victoria)
   - Obtiene código AS_p1
   - Tutorial: cómo activar poder ofensivo
5. **Abre la Puerta** (uso del código)
   - Tutorial: interactuar con dispositivos
6. **Obtiene Poder Defensivo** (opcional, en la sala)
   - Desbloquea camino secreto

#### 4. **Cambios Sugeridos en el Flujo**

**Cambio 1: Separar Tutorial de Combate y Poder Ofensivo**

❌ Actual:
```
|-- Tutorial: Combate + Poder ofensivo -> Tutorial adquisición código --> AS_p1
```

✅ Propuesto:
```
|-- Tutorial: Combate básico
|-- Combate: Derrota Enemy Low (sin poder)
|-- Tutorial: Poder ofensivo (ahora que ganó)
|-- Obtiene código --> AS_p1
|-- [AS_p1] Abrir puerta
|-- Tutorial: Poder defensivo (opcional)
|-- <Poder defensivo> Acceso a camino oculto -> AS_Secreto_A
```

**Cambio 2: Agregar un "Momento de Respiro"**

Después de la primera batalla, el jugador debería tener un momento para respirar y explorar antes de que la complejidad aumente. Propuesta:

```
|-- CutScene post-victoria: alarms apagadas, la sala está quieta
|-- Exploración libre de 20-30 segundos
|-- Tutorial: revisar la sala, buscar ítems
|-- Encontrar energía / suministros básicos
|-- Tutorial: Sistema de inventario / PowerUps
|-- [Optional] Camino secreto hacia AS_Secreto_A
```

---

## 🏗️ SISTEMAS NECESARIOS PARA AS_Cell_0

### Sistemas Críticos (Fase 1)

#### 1. **Sistema de Entrada (InputManager)**
- [x] Ya existe según las instrucciones
- **Necesarios**: ReadKey(W/A/S/D), DetectTouchInput
- **Eventos**: `OnMovementInput`, `OnAttackButtonPressed`

#### 2. **Sistema de Movimiento (PlayerController)**
- [ ] **Crear**: controlador de movimiento del player
- Usa InputManager para obtener dirección
- Mueve el player usando Rigidbody.velocity o transform.Translate
- **Parámetros**: moveSpeed (5-7 m/s)

#### 3. **Sistema de Combate Base**
- [ ] **Crear**: PlayerCombat.cs
- Detecta colisión con enemigos en rango 5m
- Ataque sin poder: daño base 10
- Animación de ataque
- **Parámetros**: attackCooldown (0.5s), attackRange (5m)

#### 4. **Sistema de Enemigos - Enemy Low**
- [ ] **Crear**: Enemy.cs (base)
- [ ] **Crear**: LowRobot.cs (subclase específica)
- [ ] **Crear**: estados (IdleState, PursuingState, AttackingState, DeadState)
- **Comportamiento**: patrulla → persecución → ataque → muerte
- **Health**: 20 HP, toma 10 de daño por ataque del player

#### 5. **Sistema de Zonas (DynamicZone)**
- [x] Aparentemente existe
- **Necesario**: AS_Cell_0 como zona individual
- Trigger para definir límite de la sala
- Manejo de spawn/despawn de enemigos

#### 6. **Sistema de Códigos / Puertas**
- [ ] **Crear**: DoorController.cs
- [ ] **Crear**: CodeSystem.cs (para AS_p1, AS_p2, etc.)
- Puerta bloqueada inicialmente
- Al obtener AS_p1 → puerta se abre
- Animación de apertura

#### 7. **Sistema de Cutscenes**
- [ ] **Crear**: CutScenePlayer.cs (simple)
- Para cinemática inicial de asteroide
- Para cinemática post-combate
- **Necesario**: Transición a juego en vivo

#### 8. **Sistema de Poder Ofensivo (Base)**
- [ ] **Crear**: PowerSystem.cs
- [ ] **Crear**: offensive power template
- Tutorial de activación
- Incremento de daño (base 10 → 15+ con poder)

#### 9. **UI Mínima**
- [ ] Barra de salud del jugador
- [ ] Tutorial/hint text (presionar X para atacar)
- [ ] Objetivo actual (ej: "Derrota el robot")
- [ ] Código adquirido

#### 10. **Cámara Isométrica**
- [ ] Configurar CameraController para seguir al player
- Ángulo: 45° isométrico
- Zoom: ajustable según tamaño de sala
- No debe atravesar paredes

### Sistemas Secundarios (Fase 2)

- [ ] Poder defensivo y camino secreto
- [ ] Sistema de inventario (PowerUps)
- [ ] Sistema de daño y knockback
- [ ] Efectos visuales de combate
- [ ] Sonido y música

---

## 🎯 ORDEN RECOMENDADO DE IMPLEMENTACIÓN

### Sprint 1: Movimiento y Entrada
1. **PlayerController.cs** - Movimiento básico con InputManager
2. **CameraController** - Configurar para seguir al player
3. **Test**: Jugador puede moverse por AS_Cell_0

### Sprint 2: Combate básico
1. **PlayerCombat.cs** - Sistema de ataque sin poder
2. **Enemy.cs** base - Structure con health
3. **LowRobot enemigo específico**
4. **Enemy states** (Idle, Pursuing, Attacking, Dead)
5. **Test**: Jugador puede derrotar al robot

### Sprint 3: Puertas y Códigos
1. **CodeSystem.cs** - Gestión de códigos
2. **DoorController.cs** - Puertas que se abren con código
3. **Test**: Código AS_p1 abre la puerta

### Sprint 4: Poderes básicos
1. **PowerSystem.cs** - Framework genérico
2. **Offensive Power** - Daño aumentado
3. **Tutorial UI** - Hints para usar poder
4. **Test**: Poder aumenta daño a 15

### Sprint 5: Pulido y Cutscenes
1. **CutScenePlayer.cs** - Secuencia inicial
2. **UI completa** - HUD, objetivos
3. **Efectos visuales** - Explosión, combate
4. **Test**: Sala completa jugable

---

## 📊 ESTRUCTURA MÍNIMA DE AS_Cell_0

```
AS_Cell_0 (Escena/Prefab)
├── Terreno/Escenario
│   ├── Piso (plane/mesh)
│   ├── Puertas
│   │   ├── Puerta-Entrada (dinámica)
│   │   ├── Puerta-Salida [AS_p1] (requiere código)
│   │   └── Puerta-Secreta [Poder_Defensivo]
│   ├── Escombros asteroide (decorativo)
│   └── ZoneTrigger (define límites)
├── Player
│   ├── GameObject con Rigidbody
│   ├── PlayerController.cs
│   ├── PlayerCombat.cs
│   ├── Health.cs
│   └── Mesh/Animator (visual)
├── Enemies
│   ├── LowRobot_01
│   │   ├── Enemy.cs
│   │   ├── LowRobot.cs
│   │   ├── Health.cs
│   │   ├── Mesh/Animator
│   │   └── Collider (ataque)
│   └── [EnemyManager referencia]
├── Interactables
│   ├── Puerta_Salida
│   │   └── DoorController.cs [requiere AS_p1]
│   ├── CodePickup_AS_p1
│   │   └── obtiene al derrotar robot
│   └── Puerta_Secreta
│       └── DoorController.cs [requiere poder defensivo]
├── Cámara
│   └── CameraController (sigue al player, isométrico)
├── Lighting (lighting isométrico)
└── UI Canvas
    ├── HealthBar (player)
    ├── Objectives text
    ├── Hint text
    └── Code indicator
```

---

## ✅ CHECKLIST DE VALIDACIÓN

Antes de dar AS_Cell_0 por "completada", verificar:

- [ ] Música ambiental de la celda
- [ ] El jugador aparece en la posición correcta tras cutscene
- [ ] Puedo moverme sin obstáculos
- [ ] El enemigo me detecta cuando me acerco
- [ ] Puedo atacar al enemigo
- [ ] El enemigo ataca de vuelta
- [ ] Tras derrotar al enemigo, obtengo código AS_p1
- [ ] Puedo recoger el código
- [ ] Con el código, puedo abrir la puerta de salida
- [ ] La puerta se abre con animación
- [ ] Puedo salir a AS_AccC_0
- [ ] [Opcional] Puedo acceder a camino secreto con poder defensivo

---

## 🔗 REFERENCIAS A INSTRUCCIONES DEL PROYECTO

Para implementar estos sistemas, consultar:

1. **cosmic-crew.instructions.md**:
   - Patrones Singleton para managers
   - Event-driven architecture
   - Naming conventions

2. **cosmic-crew-enemies.instructions.md**:
   - Ciclo de vida del enemigo
   - State machine pattern
   - Navigation system

3. **cosmic-crew-zones.instructions.md**:
   - Configuración de zonas
   - Triggers de zona
   - Despawn de enemigos

4. **cosmic-crew-csharp.instructions.md**:
   - Estructura de archivo C#
   - Documentación XML
   - Logging standards

---

## 🎨 NOTAS VISUALES / NARRATIVAS

Para hacer AS_Cell_0 más interesante:

1. **Ambiente**:
   - Escombros de asteroide visible a través de apertura en pared/techo
   - Alarmas de baja potencia (parpadeantes)
   - Sistema eléctrico dañado (chispas ocasionales)

2. **Narrativa**:
   - Cutscene de 10-15 segundos: terremoto, fallo del sistema
   - Diálogo opcional de C2 en celda adyacente ("¿Qué pasó?")
   - Hologramas dañados mostrando lo que pasó

3. **Progresión Visual**:
   - Al derrotar robot: luces se estabilizan
   - Al recoger código: interfaz destella
   - Al abrir puerta: luz verde encima de puerta

---

## 📝 PRÓXIMOS PASOS

1. **Implementar esta estructura en sprints**
2. **Crear prefabs para enemigos reutilizables**
3. **Documentar APIs de cada sistema en el código**
4. **Testing manual en editor después de cada sprint**
5. **Guardar y versionar en Git después de cada milestone**

---

**Última actualización**: Abril 2026 | **Estado**: Listo para implementación
