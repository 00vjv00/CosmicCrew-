# CosmicCrew 🚀

Un juego de acción 3D isométrico ambientado en una estación espacial carcelaria donde controlas un equipo de 5 personajes únicos con poderes especiales para escapar de la prisión generada proceduralmente.

## 📖 Descripción del Proyecto

**CosmicCrew** es un juego de acción 3D desarrollado en **Unity** que combina:
- **Mecánicas de acción**: Combate, stealth y uso de poderes
- **Diseño de niveles**: Estación espacial carcelaria con 248 robots enemigos
- **Sistema de personajes**: 5 héroes (Stone, Lumen, Fold, Nube, Arac) con poderes únicos
- **Mecánica de fusión**: Los personajes pueden combinarse para resolver retos imposibles solos
- **Generación procedural**: Zonas dinámicas que se cargan/descargan según la posición del jugador

## 🎮 Características Principales

### Personajes
- **Stone**: Fuerza bruta física
- **Lumen**: Energía, invisibilidad y absorción
- **Fold**: Teletransporte y geometría
- **Nube**: Densidad y presión gaseosa
- **Arac**: Red de nanobots y tecnología

### Sistemas
- **System Zonas Dinámicas**: Carga/descarga inteligente de áreas
- **Pool de Enemigos**: 248 robots con IA de patrulla y persecución
- **State Machines**: Para enemigos, jugador y estados globales del juego
- **Sistema de Cámara Isométrica**: Follow camera con zoom dinámico (Cinemachine)
- **Input System**: Soporte para teclado, mouse y touch

## 🛠️ Stack Tecnológico

- **Engine**: Unity (LTS Latest)
- **Lenguaje**: C#
- **Librerías**: 
  - Cinemachine (cámara)
  - Input System (entrada)
  - UI Toolkit (interfaz)

## 📁 Estructura del Proyecto

```
Assets/
├── Scripts/
│   ├── Core/              # Definiciones globales y singleton base
│   ├── Manager/           # GameManager, InputManager, CameraController
│   ├── Player/            # Controlador de jugador
│   ├── Enemies/           # Sistema de enemigos y IA
│   ├── ZoneSystem/        # Gestión de zonas dinámicas
│   ├── Station/           # Estructura de la estación
│   ├── Prefabs/           # Assets prefabricados
│   └── docs/              # Documentación y notas de diseño
├── Prefabs/               # Objetos prefabricados
├── Scenes/                # Escenas Unity
└── Shaders/               # Shaders personalizados
```

## 🚀 Primeros Pasos

### Requisitos
- Unity 2022 LTS o superior
- Visual Studio Code o Visual Studio (recomendado)

### Clonar y Configurar
```bash
# Clonar el repositorio
git clone https://github.com/TuUsuario/CosmicCrew.git
cd CosmicCrew

# Abrir en Unity Hub
# Seleccionar Unity 2022 LTS o superior
```

## 🎯 Roadmap de Desarrollo

### Fase 1: Prototipo Base ✅ (En Progreso)
- [x] Estructura de proyecto y carpetas
- [ ] Primera sala (AS_Cell_0)
- [ ] Enemy Low básico
- [ ] Sistema de input y movement
- [ ] Tutoriales iniciales

### Fase 2: Sistemas Principales
- [ ] Sistema de zonas dinámicas (streaming)
- [ ] Pool de enemigos con IA completa
- [ ] Sistema de poderes y fusión
- [ ] Interfaz de usuario

### Fase 3: Contenido
- [ ] Todas las salas del Capítulo I
- [ ] Enemigos variados (3+ tipos)
- [ ] Cutscenes y narrativa
- [ ] Música y sonido

## 📖 Documentación

Consulta las instrucciones de desarrollo en `Assets/Scripts/docs/`:
- `cosmic-crew.instructions.md` - Arquitectura general y patrones
- `cosmic-crew-enemies.instructions.md` - Sistema de enemigos
- `cosmic-crew-zones.instructions.md` - Sistema de zonas
- `cosmic-crew-csharp.instructions.md` - Convenciones C#
- `flujo_manual.txt` - Mapa completo del flujo del jugador

## 🤝 Contribuir

Este es un proyecto en desarrollo. Para contribuir:

1. Crea una rama desde `main`: `git checkout -b feature/tu-feature`
2. Haz commits descriptivos: `git commit -m "Add: descripción clara"`
3. Verifica en el editor que no hay errores
4. Push a tu rama: `git push origin feature/tu-feature`
5. Abre un Pull Request

### Convenciones de Código
Sigue las guías en `cosmic-crew-csharp.instructions.md`:
- C# - PascalCase para clases y métodos públicos, camelCase para privados
- Estructura de archivo con secciones claramente comentadas
- XML doc comments en públicos
- Event-driven architecture para desacoplamiento

## 📝 Licencia

[MIT License](LICENSE) - Libre para uso educativo y comercial

## 👥 Equipo

Proyecto desarrollado con pasión para crear una experiencia única de action-RPG en el espacio.

---

**Estado**: En desarrollo activo 🔨 | **Última actualización**: Abril 2026
