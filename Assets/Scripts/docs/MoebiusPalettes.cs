// PALETAS DE COLORES MOEBIUS PARA UNITY
// Copia y pega en tu script

using UnityEngine;
using System.Collections.Generic;

public class MoebiusPalettes
{
    public static class MoebiusClassico
    {
        public static Color NegroM = new Color(0.04f, 0.04f, 0.04f); // #0a0a0a
        public static Color AzulMarino = new Color(0.10f, 0.23f, 0.32f); // #1a3a52
        public static Color AzulCielo = new Color(0.29f, 0.48f, 0.65f); // #4a7ba7
        public static Color VerdeOscuro = new Color(0.18f, 0.35f, 0.24f); // #2d5a3d
        public static Color VerdeBosque = new Color(0.35f, 0.54f, 0.44f); // #5a8a6f
        public static Color Tierra = new Color(0.55f, 0.44f, 0.28f); // #8b6f47
        public static Color Ocre = new Color(0.79f, 0.66f, 0.38f); // #c9a961
        public static Color GrisMedio = new Color(0.48f, 0.48f, 0.48f); // #7a7a7a
        public static Color BlancoMarfil = new Color(0.96f, 0.94f, 0.91f); // #f5f1e8
        
        public static Color[] All => new Color[] {
            NegroM, AzulMarino, AzulCielo, VerdeOscuro, VerdeBosque,
            Tierra, Ocre, GrisMedio, BlancoMarfil
        };
    }
    
    public static class TheIncal
    {
        public static Color NegroAbsoluto = new Color(0.00f, 0.00f, 0.00f); // #000000
        public static Color PurpuraProfundo = new Color(0.24f, 0.10f, 0.31f); // #3d1a4f
        public static Color MagentaVibrante = new Color(0.77f, 0.12f, 0.23f); // #c41e3a
        public static Color RojoOxido = new Color(0.55f, 0.23f, 0.23f); // #8b3a3a
        public static Color Dorado = new Color(0.85f, 0.65f, 0.13f); // #daa520
        public static Color NaranjaQuemado = new Color(0.80f, 0.33f, 0.00f); // #cc5500
        public static Color AmarilloPalido = new Color(1.00f, 0.98f, 0.80f); // #fffacd
        public static Color CianOscuro = new Color(0.00f, 0.30f, 0.30f); // #004d4d
        public static Color BlancoCrema = new Color(1.00f, 0.99f, 0.94f); // #fffef0
        
        public static Color[] All => new Color[] {
            NegroAbsoluto, PurpuraProfundo, MagentaVibrante, RojoOxido, Dorado,
            NaranjaQuemado, AmarilloPalido, CianOscuro, BlancoCrema
        };
    }
    
    public static class Arzach
    {
        public static Color GrisAcero = new Color(0.21f, 0.27f, 0.31f); // #36454f
        public static Color AzulPastel = new Color(0.44f, 0.70f, 0.85f); // #6fb3d9
        public static Color TurquesaSuave = new Color(0.50f, 0.85f, 0.85f); // #7fd8d8
        public static Color Beige = new Color(0.76f, 0.70f, 0.50f); // #c2b280
        public static Color MarronClaro = new Color(0.63f, 0.51f, 0.43f); // #a0826d
        public static Color VerdeMenta = new Color(0.60f, 1.00f, 0.60f); // #98ff98
        public static Color Lavanda = new Color(0.90f, 0.90f, 0.98f); // #e6e6fa
        public static Color GrisSuave = new Color(0.83f, 0.83f, 0.83f); // #d3d3d3
        public static Color BlancoPuro = new Color(1.00f, 1.00f, 1.00f); // #ffffff
        
        public static Color[] All => new Color[] {
            GrisAcero, AzulPastel, TurquesaSuave, Beige, MarronClaro,
            VerdeMenta, Lavanda, GrisSuave, BlancoPuro
        };
    }
    
    public static class Blueberry
    {
        public static Color AzulProfundo = new Color(0.00f, 0.00f, 0.55f); // #00008b
        public static Color AzulMedianoche = new Color(0.10f, 0.10f, 0.44f); // #191970
        public static Color NaranjaMaradera = new Color(0.63f, 0.33f, 0.18f); // #a0522d
        public static Color SienaQuemada = new Color(0.55f, 0.27f, 0.08f); // #8b4513
        public static Color RojoTostado = new Color(0.74f, 0.25f, 0.25f); // #bc3f3f
        public static Color OroEnvejecido = new Color(0.72f, 0.53f, 0.04f); // #b8860b
        public static Color GrisPizarra = new Color(0.44f, 0.50f, 0.56f); // #708090
        public static Color GrisClaro = new Color(0.88f, 0.88f, 0.88f); // #e0e0e0
        public static Color Hueso = new Color(0.96f, 0.96f, 0.86f); // #f5f5dc
        
        public static Color[] All => new Color[] {
            AzulProfundo, AzulMedianoche, NaranjaMaradera, SienaQuemada, RojoTostado,
            OroEnvejecido, GrisPizarra, GrisClaro, Hueso
        };
    }
    
    public static class WorldOfEdena
    {
        public static Color Indigo = new Color(0.29f, 0.00f, 0.51f); // #4b0082
        public static Color Violeta = new Color(0.55f, 0.00f, 0.55f); // #8b008b
        public static Color VerdeEsmeralda = new Color(0.31f, 0.78f, 0.47f); // #50c878
        public static Color VerdeJade = new Color(0.00f, 0.66f, 0.42f); // #00a86b
        public static Color Cian = new Color(0.00f, 1.00f, 1.00f); // #00ffff
        public static Color AzulCieloClaro = new Color(0.53f, 0.81f, 0.92f); // #87ceeb
        public static Color RosaPastel = new Color(1.00f, 0.75f, 0.80f); // #ffc0cb
        public static Color GrisPerla = new Color(0.90f, 0.90f, 0.90f); // #e5e5e5
        public static Color BlancoFantasma = new Color(0.97f, 0.97f, 1.00f); // #f8f8ff
        
        public static Color[] All => new Color[] {
            Indigo, Violeta, VerdeEsmeralda, VerdeJade, Cian,
            AzulCieloClaro, RosaPastel, GrisPerla, BlancoFantasma
        };
    }
    
    public static class SableInspired
    {
        public static Color ArenaOscura = new Color(0.55f, 0.45f, 0.33f); // #8b7355
        public static Color ArenaCálida = new Color(0.83f, 0.64f, 0.45f); // #d4a373
        public static Color NaranjaSuave = new Color(1.00f, 0.72f, 0.30f); // #ffb84d
        public static Color OcreRojo = new Color(0.80f, 0.42f, 0.36f); // #cc6b5b
        public static Color RosaPolvo = new Color(0.85f, 0.65f, 0.65f); // #d8a5a5
        public static Color AzulDesierto = new Color(0.37f, 0.55f, 0.66f); // #5f8ca8
        public static Color VerdeOasis = new Color(0.50f, 0.65f, 0.42f); // #7fa66a
        public static Color GrisArena = new Color(0.77f, 0.71f, 0.63f); // #c4b5a0
        public static Color Crema = new Color(1.00f, 0.99f, 0.82f); // #fffdd0
        
        public static Color[] All => new Color[] {
            ArenaOscura, ArenaCálida, NaranjaSuave, OcreRojo, RosaPolvo,
            AzulDesierto, VerdeOasis, GrisArena, Crema
        };
    }
}

// EJEMPLO DE USO:
/*
public class MyMoebiusScene : MonoBehaviour
{
    void Start()
    {
        // Usar color individual
        GetComponent<Renderer>().material.color = MoebiusPalettes.MoebiusClassico.AzulCielo;
        
        // O iterar sobre paleta completa
        Color[] colors = MoebiusPalettes.TheIncal.All;
        foreach (Color color in colors)
        {
            Debug.Log("Color: " + color);
        }
    }
}
*/
