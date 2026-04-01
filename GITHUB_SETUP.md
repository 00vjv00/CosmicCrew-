# INSTRUCCIONES: Crear Repositorio de CosmicCrew en GitHub

## ✅ Archivos ya Creados

Los siguientes archivos ya están listos en la carpeta raíz del proyecto:
- `README.md` - Documentación del proyecto
- `.gitignore` - Configuración para ignorar archivos de Unity

## 📋 PASOS PARA CREAR EL REPOSITORIO

### Paso 1: Crear Repositorio en GitHub (Web)

1. Ve a **github.com** y logueate en tu cuenta
2. Click en **"+"** (arriba a la derecha) → **"New repository"**
3. Rellena los detalles:
   ```
   Repository name: CosmicCrew
   Description: 3D isometric action game set in a prison space station
   Visibility: Public (o Private si prefieres)
   Initialize with:
     ☐ Add a README file (ya lo tenemos)
     ☐ Add .gitignore (ya lo tenemos)
     ☐ Choose a license (recomendado: MIT)
   ```
4. Click **"Create repository"**

### Paso 2: Inicializar Git Localmente

Abre PowerShell en la raíz del proyecto (`c:\Users\User\CosmicCrew`) y ejecuta:

```powershell
# Navegar a la carpeta
cd c:\Users\User\CosmicCrew

# Inicializar repositorio
git init

# Agregar todos los archivos (excepto los ignorados)
git add .

# Commit inicial
git commit -m "Initial commit: Project structure and documentation"

# Ver el status
git status
```

**Resultado esperado**: Deberías ver decenas de archivos listados (no miles, porque .gitignore excluye Library/, Temp/, etc.)

### Paso 3: Conectar Repositorio Local con GitHub

Después de crear el repositorio en GitHub, verás instrucciones. Copia la URL (HTTPS o SSH) y ejecuta:

```powershell
# Agregar el repositorio remoto (reemplaza con tu URL)
git remote add origin https://github.com/TuUsuario/CosmicCrew.git

# (O si usas SSH)
git remote add origin git@github.com:TuUsuario/CosmicCrew.git

# Verificar la conexión
git remote -v
```

### Paso 4: Push Inicial

```powershell
# Cambiar rama a 'main' (GitHub usa main por defecto ahora)
git branch -M main

# Hacer push del código
git push -u origin main
```

**Resultado esperado**: El código sube a GitHub. Deberías ver mensajes como:
```
Enumerating objects: 156, done.
Counting objects: 100% (156/156), done.
...
To github.com:TuUsuario/CosmicCrew.git
 * [new branch]      main -> main
Branch 'main' set up to track remote branch 'main' from 'origin'.
```

---

## 🔄 FLUJO DE TRABAJO FUTURO

Después de esta configuración inicial, para cada cambio:

```powershell
# 1. Ver estado
git status

# 2. Agregar cambios
git add .

# 3. Hacer commit
git commit -m "Add: descripción clara de los cambios"

# 4. Push
git push origin main
```

---

## 📌 OPCIONAL: Agregar Licencia MIT

Si deseas agregar una licencia oficial de MIT a tu repositorio:

1. En GitHub, ve a tu repositorio
2. Click en **"Add file"** → **"Create new file"**
3. Nombre: `LICENSE`
4. GitHub te mostrará opciones de licencia
5. Selecciona **"MIT License"**
6. GitHub completará el contenido automáticamente
7. Commit el archivo

O crea el archivo localmente:

```powershell
# Crear archivo LICENSE en el directorio raíz
"MIT License

Copyright (c) 2026 [Tu Nombre]

Permission is hereby granted, free of charge, to any person obtaining a copy..." | Out-File LICENSE -Encoding UTF8
```

---

## 🛠️ RAMAS DE DESARROLLO (Recomendado)

Después del commit inicial, puedes crear ramas para organizar el trabajo:

```powershell
# Ver ramas
git branch -a

# Crear rama para la primera feature
git checkout -b feature/as-cell-0-implementation

# Trabajar en esa rama, luego:
git push origin feature/as-cell-0-implementation

# En GitHub, abrir Pull Request (PR) para fusionar a main
```

---

## ⚠️ TIPS IMPORTANTES

1. **No hacer commit de cambios grandes de Library/** - si accidentalmente agregaste, limpia con:
   ```powershell
   git reset HEAD Library/
   ```

2. **Ver qué archivos van a subir**:
   ```powershell
   git status
   ```
   Debe mostrar solo Scripts, Assets (sin Library), Packages, ProjectSettings, Logs, etc.

3. **Tamaño del repositorio**: GitHub tiene límite de 100MB por archivo. Si builds (.exe) pesan demasiado, agrégalos al .gitignore

4. **Autenticación SSH vs HTTPS**:
   - HTTPS: más fácil, pero pide credenciales cada vez (o usa git credential manager)
   - SSH: más seguro, requiere configuración inicial

---

## ✨ RESULTADO FINAL

Tu repositorio GitHub tendrá:
```
CosmicCrew/
├── README.md
├── .gitignore
├── Assets/
│   ├── Scripts/
│   │   ├── Manager/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── ZoneSystem/
│   │   ├── docs/
│   │   │   ├── flujo_manual.txt
│   │   │   └── ANALISIS_AS_CELL_0.md
│   │   └── ... (otros)
│   ├── Prefabs/
│   ├── Scenes/
│   └── ... (otros)
├── Packages/
├── ProjectSettings/
└── ... (otros archivos de configuración)
```

Se **excluyen**:
- Library/
- Temp/
- Logs/
- UserSettings/
- .vs/ (visual studio)

---

## 📞 PRÓXIMOS PASOS

1. ✅ Crear repositorio en GitHub
2. ✅ Inicializar git local
3. ✅ Push inicial
4. **Crear rama `feature/as-cell-0`** para empezar a implementar
5. **Trabajar en sprints**: cada funcionalidad en su rama, PR a main

---

**Guía creada**: Abril 2026 | **Estado**: Listo para ejecutar
