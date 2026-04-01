# ⚡ QUICK START SHEET - CosmicCrew

**Última actualización**: Abril 2026

---

## 🎯 TAREA 1: GitHub (5 minutos)

### Ejecutar en PowerShell (c:\Users\User\CosmicCrew)
```powershell
git init
git add .
git commit -m "Initial commit: Project structure and documentation"
```

### En GitHub.com
1. Crear nuevo repositorio: `CosmicCrew`
2. Copiar la URL (HTTPS)

### De vuelta en PowerShell
```powershell
git remote add origin https://github.com/[TU_USUARIO]/CosmicCrew.git
git branch -M main
git push -u origin main
```

✅ **Listo**: Tu código en GitHub

---

## 🎮 TAREA 2: Implementar AS_Cell_0 (5 Sprints)

### Material de Referencia
- 📖 `ANALISIS_AS_CELL_0.md` - Análisis completo (LEE ESTO PRIMERO)
- 💾 `TEMPLATES_CODIGOS.md` - Código listo para copiar/pegar
- 📋 `flujo_manual.txt` - Mapa del juego

### Sprint 1: Movimiento (30 min)
```
1. Abre Assets/Scripts/Player/PlayerController.cs
2. Copia el template de TEMPLATES_CODIGOS.md
3. Abre Assets/Scripts/Manager/InputManager.cs
4. Copia el template de InputManager
5. En Unity, crea escena AS_Cell_0
6. Agrega Player GameObject
7. Asigna componentes y referencias
8. Test: W/A/S/D mueve el personaje ✓
```

**Archivo esperado:**
```
Assets/Scripts/Player/PlayerController.cs
Assets/Scripts/Manager/InputManager.cs
```

### Sprint 2: Enemigos (45 min)
```
1. Copia plantilla Enemy.cs → Assets/Scripts/Enemies/
2. Copia LowRobot.cs → Assets/Scripts/Enemies/
3. Crea states (IdleState, PursuingState, AttackingState)
4. Copia PlayerCombat.cs → Assets/Scripts/Player/
5. En escena: agrega LowRobot_01 GameObject
6. Test: Enemigo patrulla, persigue, ataca ✓
```

**Archivo esperado:**
```
Assets/Scripts/Enemies/Enemy.cs
Assets/Scripts/Enemies/LowRobot.cs
Assets/Scripts/Player/PlayerCombat.cs
```

### Sprint 3: Puertas y Códigos (20 min)
```
1. Copia DoorController.cs → Assets/Scripts/Station/
2. Crea CodePickup.cs (tag items con código)
3. En escena: agrega puertas y códigos
4. Configura requiredCode = "AS_p1"
5. Test: Derrotar robot → obtiene AS_p1 → puerta se abre ✓
```

### Sprint 4: Poderes (30 min)
```
1. Crea PowerSystem.cs → Assets/Scripts/Core/
2. Crea OffensivePower.cs base
3. Integra con PlayerCombat
4. Test: Activar poder aumenta daño (10 → 15) ✓
```

### Sprint 5: Pulido (30 min)
```
1. Crea UI: Health bar, Objectives, Hints
2. Cutscene inicial (opcional: terremoto)
3. Sonidos y efectos visuales
4. Test: Checklist en ANALISIS_AS_CELL_0.md ✓
```

---

## ✅ ENEMIES: Low Robot Stats

```
Clase: LowRobot
Salud: 20 HP
Daño: 5 por golpe
Detección: 40m
Ataque: 5m de rango
Estados: Idle → Pursuing → Attacking → Dead
Especial: Ninguno
```

---

## 📁 ESTRUCTURA FINAL AS_CELL_0

```
Escena: AS_Cell_0
├── Player
│   ├── PlayerController.cs ⭐
│   └── PlayerCombat.cs ⭐
├── LowRobot_01
│   ├── Enemy.cs
│   └── LowRobot.cs ⭐
├── Doors
│   └── Door_Exit (DoorController.cs, requiere: AS_p1)
├── Items
│   ├── Código_AS_p1 (CodePickup.cs)
│   └── Código_AS_p2 (CodePickup.cs - optional)
├── InputManager ⭐ (Singleton)
└── Camera (CameraController)
```

⭐ = Ya existe template en TEMPLATES_CODIGOS.md

---

## 🔧 Git Workflow Después de Cada Sprint

```powershell
# Después de completar sprint:
git add .
git commit -m "Add: Sprint [N] - [descripción]"
git push origin main

# Ejemplo:
git commit -m "Add: Sprint 1 - PlayerController and InputManager"
```

---

## 📊 PROGRESO ESPERADO

| Sprint | Objetivo | Arquivos | Tiempo |
|--------|----------|----------|--------|
| 1 | Movimiento + Input | PlayerController, InputManager | 30 min |
| 2 | Enemigos + Combate | Enemy, LowRobot, States, PlayerCombat | 45 min |
| 3 | Puertas + Códigos | DoorController, CodePickup | 20 min |
| 4 | Poderes básicos | PowerSystem, OffensivePower | 30 min |
| 5 | UI + Pulido | Canvas, Cutscenes, Efectos | 30 min |
| **TOTAL** | **AS_Cell_0 Completa** | **7 scripts** | **2.5 hrs** |

---

## ⚠️ ERRORES COMUNES A EVITAR

❌ **NO hacer**:
```csharp
// Acceder a Singleton sin verificar
Vector3 dir = InputManager.Instance.GetMovement(); // ¿Si es null?

// Olvidar desuscribirse de eventos
void Start() { GameManager.OnStateChanged += Handler; } // Memory leak

// Hardcodear valores
if (distance > 40f) { } // ¿De dónde viene 40?
```

✅ **HACER**:
```csharp
// Verificar antes de usar
InputManager inp = InputManager.Instance;
if (inp == null) return;

// Desuscribirse siempre
void OnDestroy() { GameManager.OnStateChanged -= Handler; }

// Usar constantes
const float PURSUIT_RANGE = 40f;
if (distance > PURSUIT_RANGE) { }
```

---

## 🎨 COSAS OPCIONALES (Pero Recomendadas)

- [ ] Animaciones para el jugador (movement, attack)
- [ ] Sonido de pasos y ataque
- [ ] Efecto visual al golpear
- [ ] Cutscene de cinemática inicial (terremoto)
- [ ] Camino secreto con poder defensivo
- [ ] PowerUp en la sala (recupera salud)
- [ ] Iluminación isométrica

---

## 📞 SI TIENES DUDAS

1. Lee **ANALISIS_AS_CELL_0.md** - responde el 80% de preguntas
2. Lee **cosmic-crew.instructions.md** - patrones generales
3. Mira **TEMPLATES_CODIGOS.md** - código listo
4. Test en editor - juega y prueba

---

## 🎯 OBJETIVO FINAL

```
✓ Repositorio en GitHub con .gitignore y README
✓ Escena AS_Cell_0 completamente jugable
✓ Jugador controla personaje con WASD
✓ 1 enemigo Low Robot patrulla y persigue
✓ Combate: jugador ataca, enemigo se defiende
✓ Al derrotar: obtiene código AS_p1
✓ Código abre puerta hacia AS_AccC_0
✓ [Opcional] Camino secreto con poder defensivo
```

---

**¡A crear!** 🚀
