# Cosmic Crew

Cosmic Crew es un videojuego desarrollado con **Unity 6** del género **RPG topdown isométrico 3D** para **dispositivos móviles**. El objetivo del juego es manejar **5 caracteres** que deben escapar de una **prisión espacial** usando sus poderes especiales de forma separada y/o coordinada.

---

## SETUP TÉCNICO

**Versión:** Unity 6  
**Render Pipeline:** Built-in (Standard Pipeline) — simplicidad y rendimiento óptimo en móvil

### Paquetes requeridos:

**Core (generación y control):**
- ProBuilder (construcción de geometría proceduralizada, generación dinámica de planos)
- IA Navigation (NavMesh para pathfinding y movimiento de enemigos)
- Cinemachine (control avanzado de cámara con smoothing y colisiones)

**Gameplay:**
- Input System (control táctil y entrada flexible en móvil)
- Particle System (efectos de poderes, armas y explosiones) — integrado en Unity

**Audio:**
- AudioMixer (gestión de canales de audio) — integrado en Unity
- Audio 3D Spatial Blending (sonidos posicionados en espacio 3D)

**Optimización:**
- Burst Compiler (optimización de código matemático para rotaciones cilíndricas)
- Collections (estructuras de datos eficientes para gestión de planos activos)

**Opcional:**
- Timeline (cinemáticas y secuencias narrativas)

**Diseño visual:**
- Estilo comic Moebius (Jean Giraud)
- Tres shaders personalizados funcionales (requieren ajustes en prefabs, materiales y texturas)
- Modelos 3D de diferentes orígenes (requiere normalización de asset pipeline)

---

## PERSONAJES Y PODERES

El juego cuenta con **5 personajes únicos**, cada uno con un poder ofensivo y uno defensivo. Los poderes son **graduables** según el toque del botón y variables del carácter (energía, salud, etc.), lo que permite ataques normales, fuertes o superfuertes, con costes/riesgos asociados. Los poderes tienen reversibilidad: pueden ser tanto ofensivos como defensivos según la situación.

### 1. STONE (Piedra, Fuerza)
- **Poder ofensivo:** Puñetazo (normal, fuerte, superfuerte)
- **Poder defensivo:** Muralla defensiva
- **Mecánica especial:** Un puñetazo superfuerte puede desestabilizar estructuras
- **Flujo óptimo:** Salas de combate directo, áreas con estructuras destructibles
- **Control:** Botón de ataque táctil con presión variable

### 2. LUMEN (Energía, Invisibilidad)
- **Poder ofensivo:** Onda de energía
- **Poder defensivo:** Absorber energía (daña puertas, sistemas, enemigos robot)
- **Mecánica especial:** Casi invisible, emite ondas de energía
- **Flujo óptimo:** Stealth, salas con enemigos robot, sistemas electrónicos
- **Control:** Toque y desliz para direccionar ondas

### 3. FOLD (Geométrico, Origami)
- **Poder ofensivo:** Teletransporte (da saltos inmediatos; si atraviesa un enemigo robot en el camino, lo daña)
- **Poder defensivo:** Salto vertical
- **Mecánica especial:** Movimiento discontinuo por geometría
- **Flujo óptimo:** Espacios verticales, evasión de enemigos, atajos geométricos
- **Control:** Toque doble o hold para activar teletransporte

### 4. NUBE (Gaseoso, Densidad y Presión)
- **Poder ofensivo:** Onda de presión
- **Poder defensivo:** Convertirse en gas y atravesar enemigos y barreras
- **Mecánica especial:** Cambio de densidad y presión
- **Flujo óptimo:** Áreas cerradas, atravesar barreras, enfrentar hordas
- **Control:** Hold para transformación, desliz para onda de presión

### 5. ARAC (Araña Cibernética)
- **Poder ofensivo:** Lanza red que incapacita enemigos
- **Poder defensivo:** Se divide en nanobots que pueden consumir objetos
- **Mecánica especial:** Control de múltiples entidades pequeñas
- **Flujo óptimo:** Puzzles de construcción/consumo, combate múltiple
- **Control:** Toque para lanzar red, swipe para dispersar nanobots

### GAMEPLAY Y ELECCIÓN DE PODER

El jugador elige qué poder usar en función de:
- El carácter en juego
- Sus poderes disponibles
- La situación concreta (combate, stealth, puzzle, etc.)

Siempre hay una opción que es **más fácil/óptima** según estos criterios. Un carácter puede seguir el flujo de otro, pero será más difícil y en algún punto imposible sin el carácter adecuado.

---

## ARQUITECTURA Y DIMENSIONES

### Estructura base:
- **Tile base:** 6x4y6z (unidades de construcción)

### Dimensiones por plano:
- **Eje X (ancho):** 60 unidades (10 tiles)
- **Eje Z (profundidad):** 36 unidades (escalada suave de la curvatura cilíndrica)
- **Eje Y (altura):** 24 unidades totales (6 pisos de 4 unidades cada uno)

### Estructura de planos por sector:
- **20 planos por sector** (120 planos totales en la estación)
- **6 sectores:** AltaSeguridad, Habitat, Navegación, Hangar, Almacén, Ingeniería

### Rotación de planos:
- **Rotación entre sectores:** 60° (360° ÷ 6 sectores)
- **Rotación entre planos dentro de cada sector:** 3° incremental (60° ÷ 20 planos)
- Esto forma un **cilindro con curvatura suave** donde el player está en la cara interna

### Salas de referencia (subdivisiones dentro de planos):
- **Pequeña:** 12x12 (entrada, sala de combate simple)
- **Mediana:** 18x18 (hub, área de exploración)
- **Grande:** 24x24 (boss, puzzle complejo)

### Corredores:
- **Ancho:** 6 unidades (movimiento fluido, claridad visual)
- **Largo:** variable según conexión entre salas
- **Máximo:** 60 unidades en X o Z

### Pisos por plano (6 niveles de altura):
- **Piso0:** y=0 (nivel principal, más común)
- **Piso1:** y=4 (nivel elevado 1)
- **Piso2:** y=8 (nivel elevado 2)
- **PisoSótano:** y=-4 (nivel inferior para salas puntuales con puzzles desafiantes, acceso vigilado)
- **PisoTecho:** y=12 (nivel superior externo, formado por tejados y pasarelas que interconectan)
- **PisoRunner:** y=-8 (nivel de escape runner tipo sala de residuos con paredes que se cierran; el carácter debe encontrar la salida evitando residuos y ser aplastado)

---

## DISEÑO DE ESPACIOS

La prisión espacial es una estación de **estructura circular en constante rotación**, dividida en **sectores temáticos y arquitectónicos**.

### División de espacios:

Cada sector está dividido en **espacios:** salas y corredores que unen salas, más **caminos atajo** disponibles solo según los poderes del carácter.

### Contenido por espacio:

Cada espacio debe incluir:
- Un objetivo principal
- Objetivos secundarios
- Situaciones de:
  - a) Combate con enemigos
  - b) Stealth frente a enemigos
  - c) Puzzles de exploración
  - d) Puzzles de interacción
  - e) Puzzles laberinto

### Flujos de juego por personaje:

- Cada sector tiene **varios flujos de juego**, uno óptimo por cada uno de los 5 caracteres
- Los flujos atraviesan diferentes salas, corredores y atajos
- Pueden compartir espacios pero ser **combinaciones distintas**
- Tienen **sentido narrativo coherente**
- Cada carácter tiene una **ruta óptima** según su poder
- Un carácter puede seguir otro flujo, pero será más difícil y en algún punto imposible sin el carácter adecuado

---

## ROTACIÓN Y GRAVEDAD ARTIFICIAL

### Objetivos de la rotación:

La rotación constante de la estación tiene dos objetivos principales:
1. **Aumentar o reducir la dificultad** del juego en función de la rotación del escenario respecto a la cámara
2. **Simular fuerza centrífuga** que representa la gravedad artificial

### Dinámicas de rotación:

- La rotación puede **acelerarse, reducirse a 0 y/o invertirse** por interacción del jugador
- **A más rotación = mayor gravedad artificial**
- **Sin rotación = gravedad 0**
- **Con rotación inversa** = la inclinación del escenario se revierte progresivamente
- Estos cambios tienen **consecuencias en el gameplay y la narrativa**

### Cálculo de gravedad:
- Rotación actual afecta tanto a la dirección "down" del escenario como a la mecánica de movimiento del player
- Enemigos y objetos se ven afectados por la gravedad artificial
- Puzzles pueden requerir cambios de rotación para ser completables

---

## CÁMARA ADAPTATIVA

### Configuración isométrica:
- **Rotación de cámara:** 30° (eje X) y -45° (eje Y)
- **Posición base:** arriba, detrás y a la izquierda del jugador
- Se adapta dinámicamente en función del espacio a mostrar

### Comportamiento base:

- **Sigue al jugador** cuando se mueve
- **Aumenta o disminuye su distancia** para abarcar todo el espacio actual
- Muestra la **visión completa del espacio** (sala, corredor o atajo) para claridad táctica

### Adaptación a espacios:

- **Sala mínima:** 6x6 unidades
- **Corredores:** extensión máxima de 60 unidades (máximo X del plano)
- La cámara se **adapta dinámicamente** al tamaño del espacio, aumentando distancia FOV según sea necesario

### Ocultación de pisos:

- **Solo se muestra el piso** donde está el jugador (evita oclusión visual)
- Si el jugador está en **escalera/ascensor:** la cámara gira para mostrar perfil lateral de todos los pisos
- En **PisoTecho:** la cámara gira para mostrar altura y profundidad (vista exterior de pisos)

### Rotación de cámara vs. rotación de estación:

- La cámara **NO rota con el jugador** (salvo en zonas ocluidas por pisos elevados o pasillos estrechos)
- La cámara **NO rota inicialmente con la estación**, lo que causa inclinación visual progresiva del escenario
- Si el escenario se inclina demasiado (dificultando el juego), la cámara se convierte en **hija de la estación** y acompaña rotación + movimiento
- Si la rotación se invierte o el jugador se desplaza en **sentido de rotación**, la inclinación se reduce
- Hay un **punto específico en el juego** donde la cámara acompañará movimiento y rotación de la estación simultáneamente

### Visibilidad y exploración:

- El jugador solo ve **salas/corredores/atajos que ya conoce** (ha visitado)
- El **resto de espacios permanece inactivo y oculto**
- La carga dinámica sincroniza con el sistema de exploración

---

## SISTEMAS PRINCIPALES

### 1. STATION GENERATOR
- Genera proceduralmente la estructura cilíndrica de 120 planos
- Usa ProBuilder para crear geometría base (tiles 6x4y6z)
- Configura rotación de cada plano (3° incremental)
- Crea salas, corredores y puntos de conexión
- Genera NavMesh automáticamente para IA Navigation
- **Estado:** A implementar

### 2. STATION ROTATOR
- Maneja rotación constante de la estación
- Aplica rotación a planos activos
- Sincroniza con cámara y gameplay
- Simula gravedad artificial
- Permite aceleración/desaceleración/inversión de rotación por interacción del jugador
- **Estado:** A implementar

### 3. DYNAMIC ZONE / LEVEL STREAMING
- Carga/descarga salas dinámicamente según posición del player
- Solo renderiza espacios conocidos y activos
- Gestiona memoria en móvil
- Maneja transiciones entre planos
- Sincroniza con sistema de exploración (solo muestra visitados)
- **Estado:** A implementar

### 4. CAMERA CONTROLLER
- Sigue al player sin rotación por defecto
- Gira en circunstancias específicas (obstáculos, pasillos estrechos, transiciones de piso)
- NO rota por rotación de estación (hasta límite de inclinación)
- Mantiene claridad visual
- Integración con Cinemachine
- Maneja ocultación de pisos
- Adapta FOV dinámicamente al tamaño del espacio
- **Estado:** A implementar

### 5. PLAYER CONTROLLER
- Movimiento táctil en isométrico 3D
- Gestión de selección de poderes
- Detección de entrada (toque, hold, desliz)
- Interacción con el entorno
- Aplicación de gravedad artificial según rotación de estación
- **Estado:** A implementar

### 6. SAVE SYSTEM
- Persistencia de progreso del jugador
- Registra sectores/planos completados
- Guarda estado de personajes desbloqueados
- Almacena progresión narrativa
- Datos: posición última, sectores visitados, objetivos completados, personajes desbloqueados
- Formato: JSON o PlayerPrefs
- **Estado:** A implementar (fase prototipo: será simple)

### 7. AUDIO MANAGER
- Gestión de música de fondo por sector
- Sonidos ambientes 3D (rotación de estación, maquinaria)
- SFX de poderes (impacto, energía, teletransporte, etc.)
- SFX de enemigos y combate
- Control de volumen por canal (música, SFX, ambiente)
- AudioMixer para mezcla
- **Estado:** A implementar (fase posterior)

### 8. PARTICLE SYSTEM MANAGER
- Efectos de poderes:
  - Stone: impacto de puñetazo, desestabilización de estructuras
  - Lumen: ondas de energía, absorción de energía
  - Fold: teletransporte (flash), salto vertical
  - Nube: onda de presión, conversión a gas
  - Arac: lanzamiento de red, nanobots
- Explosiones e impactos de enemigos
- Daño visual y feedback
- Pool de partículas para optimización
- **Estado:** A implementar (fase posterior)

---

## PLATAFORMA Y RENDIMIENTO

- **Plataforma de destino:** dispositivos móviles de gama media
- **Rendimiento:** crítico para táctil fluido, no descargar memoria innecesaria
- **Optimización:**
  - Carga dinámica de zonas
  - Pooling de partículas
  - LOD (Level of Detail) en modelos 3D
  - NavMesh compartido entre planos activos
  - Oclusión de pisos no visibles

---

## PRÓXIMOS PASOS

1. ✅ Estructura técnica y dimensiones confirmadas
2. ✅ Personajes y poderes definidos
3. ✅ Arquitectura de niveles especificada
4. ⏳ **Implementar StationGenerator (prototipo)**
5. ⏳ **Implementar StationRotator**
6. ⏳ **Implementar CameraController**
7. ⏳ **Implementar PlayerController básico**
8. ⏳ **Crear primer sector prototipo**
9. ⏳ **Pruebas de carga dinámica**
10. ⏳ **Implementar SaveSystem**
11. ⏳ **Implementar AudioManager**
12. ⏳ **Implementar ParticleSystemManager**

---

**Última actualización:** Marzo 2026  
**Estado del proyecto:** Prototipo en fase de documentación y planificación técnica