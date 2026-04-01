using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HatchPatternGenerator : MonoBehaviour
{
    [Header("Hatch Pattern Settings")]
    [Range(1, 50)] public int horizontalLineWidth = 2;
    [Range(1, 50)] public int horizontalSpacing = 24;
    
    [Range(1, 50)] public int verticalLineWidth = 2;
    [Range(1, 50)] public int verticalSpacing = 24;
    
    [Range(1, 50)] public int diagonalLineWidth = 2;
    [Range(1, 50)] public int diagonalSpacing = 24;
    
    public int textureSize = 512;
    public string outputPath = "Assets/Shaders/HatchPattern.png";
    
    [Header("Output")]
    public Material targetMaterial;
    
    public void GenerateHatchPattern()
    {
        // Crear textura
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
        Color[] pixels = new Color[textureSize * textureSize];
        
        // Inicializar con blanco
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        // LÍNEAS HORIZONTALES (Canal R = valor rojo)
        // Las líneas BLANCAS (1) se oscurecen; fondo NEGRO (0)
        for (int y = 0; y < textureSize; y++)
        {
            int posInGroup = y % horizontalSpacing;
            bool isLine = posInGroup < horizontalLineWidth;
            
            for (int x = 0; x < textureSize; x++)
            {
                int index = y * textureSize + x;
                Color c = pixels[index];
                c.r = isLine ? 1f : 0f;
                pixels[index] = c;
            }
        }
        
        // LÍNEAS VERTICALES (Canal G = valor verde)
        for (int x = 0; x < textureSize; x++)
        {
            int posInGroup = x % verticalSpacing;
            bool isLine = posInGroup < verticalLineWidth;
            
            for (int y = 0; y < textureSize; y++)
            {
                int index = y * textureSize + x;
                Color c = pixels[index];
                c.g = isLine ? 1f : 0f;
                pixels[index] = c;
            }
        }
        
        // LÍNEAS DIAGONALES (Canal B = valor azul)
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                int diagonal = (x + y) % diagonalSpacing;
                bool isLine = diagonal < diagonalLineWidth;
                
                int index = y * textureSize + x;
                Color c = pixels[index];
                c.b = isLine ? 1f : 0f;
                pixels[index] = c;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Guardar como PNG
        byte[] pngData = texture.EncodeToPNG();
        string fullPath = Path.Combine(Application.dataPath, outputPath.Replace("Assets/", ""));
        
        // Crear directorio si no existe
        string directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllBytes(fullPath, pngData);
        Debug.Log($"✅ HatchPattern generada: {fullPath}");
        
        // Limpiar
        DestroyImmediate(texture);
        
        // Refresco de assets
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        
        // Asignar al material si existe
        if (targetMaterial != null)
        {
            Texture2D loadedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(outputPath, typeof(Texture2D));
            if (loadedTexture != null)
            {
                targetMaterial.SetTexture("_HatchPattern", loadedTexture);
                Debug.Log("✅ Textura asignada al material!");
            }
        }
        #endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HatchPatternGenerator))]
public class HatchPatternGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🎨 Generate Hatch Pattern", GUILayout.Height(50)))
        {
            HatchPatternGenerator generator = (HatchPatternGenerator)target;
            generator.GenerateHatchPattern();
        }
    }
}
#endif
