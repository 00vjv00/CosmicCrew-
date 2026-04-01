# 🎨 RESUMEN MINIMALISTA: ESCALA DE GRISES PARA ALBEDO

## ✅ EXACTO. TRES VALORES NADA MÁS.

```
TEXTURAS ALBEDO = SOLO ESCALA DE GRISES

#FFFFFF (Blanco)    = Color claro (detalles casi invisibles)
#AAAAAA (Gris 67%)  = Color medio (detalles visibles)
#808080 (Gris 50%)  = Color oscuro (detalles prominentes)
#404040 (Gris 25%)  = Negro (detalles muy oscuros, rayado visible)

REGLA:
  Más blanco = más luminoso + menos detalles
  Más gris = más detalles + más oscuro
  Más negro = rayado muy visible + muy dramático
```

---

## 🎯 APLICACIÓN EN TU NAVE

### Paredes Claras (#4A7BA7 azul)
```
Texture: #AAAAAA (gris claro)
Inspector _Color: #4A7BA7 (azul principal)
Resultado: Azul luminoso + detalles sutiles ✅
```

### Paredes/Suelos Normales
```
Texture: #808080 (gris neutral)
Inspector _Color: #4A7BA7 (azul principal)
Resultado: Azul normal + detalles visibles ✅
```

### Sombras Profundas (#5A7F8C)
```
Texture: #808080 (gris neutral)
Inspector _Color: #5A7F8C (azul oscuro)
Resultado: Azul oscuro + rayado VISIBLE ✅
```

### Tuberías Turquesa (#7FD8D8)
```
Texture: #CCCCCC (gris muy claro)
Inspector _Color: #7FD8D8 (turquesa)
Resultado: Turquesa vibrant + detalles sutiles ✅
```

---

## 🛠️ WORKFLOW EN GIMP (SUPER SIMPLE)

```
1. Abre texture original
2. Colors > Desaturate (a escala gris)
3. Colors > Levels > Output: [X] - 255
   
   Donde [X] es:
   ├─ 255  = Blanco puro (#FFFFFF)
   ├─ 204  = Gris claro (#CCCCCC)
   ├─ 170  = Gris medio (#AAAAAA)
   ├─ 128  = Gris neutral (#808080)
   └─ 64   = Gris oscuro (#404040)

4. Export como _moebius.png
5. En Unity: asigna color en inspector
6. Done
```

---

## 📋 TABLA RÁPIDA

```
QUIERO EN UNITY          TEXTURE          LEVELS OUTPUT
────────────────────────────────────────────────────────
Color claro + detalles sutiles    #FFFFFF    255-255
Color medio + detalles visibles   #AAAAAA    170-255
Color oscuro + detalles visibles  #808080    128-255
Color muy oscuro + rayado visible #404040     64-255
────────────────────────────────────────────────────────

MÁS SIMPLE:
  Usa SIEMPRE #808080 (128-255)
  Funciona con TODO
  Balance perfecto
```

---

## ✨ VENTAJAS

```
✅ Super simple: Solo 4 valores gris
✅ Predecible: Sabes exactamente el resultado
✅ Flexible: Combina con cualquier color inspector
✅ Rápido: 30 segundos por texture en GIMP
✅ Profesional: Resultado muy Moebius
✅ Consistente: Todas las texturas iguales
```

---

## 🎯 DEFAULT RECOMENDADO

```
Para TODO (paredes, suelos, detalles):
  Texture: #808080 (gris neutral 50%)
  Levels output: 128-255
  
DESPUÉS en Unity aplicas color según tipo:
  Pared claro: #4A7BA7
  Pared oscuro: #5A7F8C
  Tuberías: #7FD8D8
  Etc.

RESULTADO: Consistente, profesional, Moebius ✅
```

---

**REGLA DE ORO:**

```
Texture = ESCALA GRIS (qué tan oscuro)
Color Inspector = QUÉ COLOR (del que color es)

Gris #808080 + Inspector #4A7BA7 = Azul normal ✅
```

---

**¡Así de simple! Tres tonos de gris. Done.** 🎨✨
