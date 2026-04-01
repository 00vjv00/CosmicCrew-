using UnityEngine;

/// <summary>
/// RotationManager - Singleton que gestiona el estado de rotación de la estación
/// Compatible con Unity 2023+
/// SIN dependencias de StarterAssets
/// 
/// INSTRUCCIONES:
/// 1. Crea un GameObject vacío llamado "RotationManager"
/// 2. Añade este script
/// 3. Llama desde código: RotationManager.SetRotationState(RotationManager.RotationState.HIGH);
/// </summary>
public class RotationManager : MonoBehaviour
{
    public enum RotationState { HIGH, LOW, STOPPED }

    [SerializeField] private RotationState currentState = RotationState.HIGH;
    [SerializeField] private float transitionDuration = 2f;

    private static RotationManager instance;

    // Eventos para que otros sistemas se suscriban
    public static event System.Action<RotationState> OnRotationChanged;
    public static event System.Action<float> OnRotationTransition;

    private float transitionTimer = 0f;
    private RotationState targetState;
    private bool isTransitioning = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[RotationManager] Inicializado. Estado: " + currentState);
    }

    void Update()
    {
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float progress = transitionTimer / transitionDuration;

            OnRotationTransition?.Invoke(progress);

            if (progress >= 1f)
            {
                currentState = targetState;
                isTransitioning = false;
                OnRotationChanged?.Invoke(currentState);
                Debug.Log($"[RotationManager] Transición completada: {currentState}");
            }
        }
    }

    public static void SetRotationState(RotationState newState)
    {
        if (instance == null)
        {
            Debug.LogError("[RotationManager] No hay instancia en la escena!");
            return;
        }

        if (newState == instance.currentState && !instance.isTransitioning)
            return;

        instance.targetState = newState;
        instance.transitionTimer = 0f;
        instance.isTransitioning = true;

        Debug.Log($"[RotationManager] Transición hacia: {newState}");
    }

    public static RotationState GetCurrentState()
    {
        if (instance == null) return RotationState.HIGH;
        return instance.currentState;
    }

    public static bool IsTransitioning()
    {
        if (instance == null) return false;
        return instance.isTransitioning;
    }
}