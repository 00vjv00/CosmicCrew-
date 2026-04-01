// ============================================================================
// GAME MANAGER - ORQUESTADOR PRINCIPAL
// Archivo: Assets/Scripts/Manager/GameManager.cs
// Descripción: Gestor central del juego. Singleton que controla todo.
// ============================================================================

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GameManager es el corazón del juego.
/// 
/// RESPONSABILIDADES:
/// - Inicializar todos los sistemas
/// - Gestionar el estado global del juego
/// - Proporcionar acceso a otros managers
/// - Manejar transiciones entre estados
/// - Pausar/Reanudar el juego
/// 
/// PATRÓN: Singleton - Solo existe UNA instancia
/// </summary>
public class GameManager : MonoBehaviour
{
    // ========================================================================
    // SINGLETON
    // ========================================================================
    
    /// <summary>
    /// Instancia única de GameManager
    /// Acceso global: GameManager.Instance
    /// </summary>
    public static GameManager Instance { get; private set; }
    
    // ========================================================================
    // REFERENCIAS A OTROS MANAGERS
    // ========================================================================
    
    private InputManager inputManager;
    private PlayerController playerController;
    
    // ========================================================================
    // ESTADO DEL JUEGO
    // ========================================================================
    
    /// <summary>
    /// Estado actual del juego
    /// </summary>
    private GameState currentGameState = GameState.LOADING;
    
    /// <summary>
    /// Si el juego está pausado
    /// </summary>
    private bool isPaused = false;
    
    // ========================================================================
    // EVENTOS
    // ========================================================================
    
    /// <summary>
    /// Se dispara cuando cambia el estado del juego
    /// </summary>
    public event System.Action<GameState> OnGameStateChanged;
    
    /// <summary>
    /// Se dispara cuando se pausa/reanuda el juego
    /// </summary>
    public event System.Action<bool> OnGamePausedChanged;
    
    // ========================================================================
    // INICIALIZACIÓN
    // ========================================================================
    
    void Awake()
    {
        // SINGLETON PATTERN
        // Si ya existe una instancia, destruir esta y mantener la anterior
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[GAME MANAGER] Ya existe una instancia de GameManager. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }
        
        // Establecer como instancia única
        Instance = this;
        
        // No destruir entre escenas
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[GAME MANAGER] Inicializado correctamente como Singleton");
    }
    
    void Start()
    {
        // Inicializar todos los sistemas
        InitializeGame();
    }
    
    void Update()
    {
        // Detectar tecla de pausa
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }
    
    // ========================================================================
    // INICIALIZACIÓN DEL JUEGO
    // ========================================================================
    
    /// <summary>
    /// Inicializar todos los sistemas del juego
    /// </summary>
    private void InitializeGame()
    {
        Debug.Log("[GAME MANAGER] Inicializando sistemas...");
        
        // 1. Obtener referencia al InputManager
        inputManager = Object.FindAnyObjectByType<InputManager>();
        if (inputManager == null)
        {
            Debug.LogWarning("[GAME MANAGER] InputManager no encontrado en la escena");
        }
        else
        {
            Debug.Log("[GAME MANAGER] InputManager encontrado");
        }
        
        // 2. Obtener referencia al PlayerController
        playerController = Object.FindAnyObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("[GAME MANAGER] PlayerController no encontrado en la escena");
        }
        else
        {
            Debug.Log("[GAME MANAGER] PlayerController encontrado");
        }
        
        // 3. GENERAR LA ESTACIÓN
        PrisonStationGenerator stationGenerator = Object.FindAnyObjectByType<PrisonStationGenerator>();
        if (stationGenerator == null)
        {
            Debug.LogError("[GAME MANAGER] PrisonStationGenerator no encontrado en la escena");
        }
        else
        {
            Debug.Log("[GAME MANAGER] Iniciando generación de estación...");
            stationGenerator.GenerarEstacion();
        }
        
        // 4. Cambiar a estado PLAYING
        SetGameState(GameState.PLAYING);
        
        Debug.Log("[GAME MANAGER] Inicialización completada");
    }
    
    // ========================================================================
    // GESTIÓN DEL ESTADO DEL JUEGO
    // ========================================================================
    
    /// <summary>
    /// Cambiar el estado del juego
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (currentGameState == newState)
            return;
        
        GameState oldState = currentGameState;
        currentGameState = newState;
        
        Debug.Log($"[GAME MANAGER] Estado cambió: {oldState} → {newState}");
        
        // Disparar evento
        OnGameStateChanged?.Invoke(newState);
        
        // Ejecutar lógica según estado
        HandleStateChange(newState);
    }
    
    /// <summary>
    /// Obtener estado actual del juego
    /// </summary>
    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }
    
    /// <summary>
    /// Ejecutar lógica según el nuevo estado
    /// </summary>
    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.MENU:
                OnEnterMenu();
                break;
            
            case GameState.LOADING:
                OnEnterLoading();
                break;
            
            case GameState.PLAYING:
                OnEnterPlaying();
                break;
            
            case GameState.PAUSED:
                OnEnterPaused();
                break;
            
            case GameState.GAME_OVER:
                OnEnterGameOver();
                break;
            
            case GameState.WIN:
                OnEnterWin();
                break;
            
            case GameState.CUTSCENE:
                OnEnterCutscene();
                break;
        }
    }
    
    // ========================================================================
    // HANDLERS DE ESTADO
    // ========================================================================
    
    private void OnEnterMenu()
    {
        Time.timeScale = 1f;  // Asegurar que el tiempo corre
        Debug.Log("[GAME] Entrando a MENÚ");
    }
    
    private void OnEnterLoading()
    {
        Time.timeScale = 1f;
        Debug.Log("[GAME] CARGANDO...");
    }
    
    private void OnEnterPlaying()
    {
        Time.timeScale = 1f;  // Asegurar que el tiempo corre
        isPaused = false;
        Debug.Log("[GAME] JUGANDO");
    }
    
    private void OnEnterPaused()
    {
        Time.timeScale = 0f;  // Detener tiempo
        isPaused = true;
        Debug.Log("[GAME] PAUSADO");
    }
    
    private void OnEnterGameOver()
    {
        Time.timeScale = 0f;
        Debug.Log("[GAME] FIN DEL JUEGO - DERROTA");
    }
    
    private void OnEnterWin()
    {
        Time.timeScale = 0f;
        Debug.Log("[GAME] VICTORIA");
    }
    
    private void OnEnterCutscene()
    {
        Time.timeScale = 1f;
        Debug.Log("[GAME] CINEMÁTICA");
    }
    
    // ========================================================================
    // PAUSA / REANUDACIÓN
    // ========================================================================
    
    /// <summary>
    /// Pausar o reanudar el juego
    /// </summary>
    public void TogglePause()
    {
        if (currentGameState == GameState.PLAYING)
        {
            if (isPaused)
            {
                ResumGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    /// <summary>
    /// Pausar el juego
    /// </summary>
    public void PauseGame()
    {
        if (isPaused)
            return;
        
        isPaused = true;
        Time.timeScale = 0f;  // Detener tiempo
        OnGamePausedChanged?.Invoke(true);
        Debug.Log("[GAME MANAGER] Juego pausado");
    }
    
    /// <summary>
    /// Reanudar el juego
    /// </summary>
    public void ResumGame()
    {
        if (!isPaused)
            return;
        
        isPaused = false;
        Time.timeScale = 1f;  // Reanudar tiempo
        OnGamePausedChanged?.Invoke(false);
        Debug.Log("[GAME MANAGER] Juego reanudado");
    }
    
    /// <summary>
    /// Comprobar si el juego está pausado
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }
    
    // ========================================================================
    // ACCESO A MANAGERS
    // ========================================================================
    
    /// <summary>
    /// Obtener InputManager
    /// </summary>
    public InputManager GetInputManager()
    {
        return inputManager;
    }
    
    /// <summary>
    /// Obtener PlayerController
    /// </summary>
    public PlayerController GetPlayerController()
    {
        return playerController;
    }
    
    // ========================================================================
    // UTILIDADES
    // ========================================================================
    
    /// <summary>
    /// Reiniciar el nivel actual
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;  // Asegurar que el tiempo funciona
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    /// <summary>
    /// Ir a menú principal
    /// </summary>
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Salir del juego
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[GAME MANAGER] Saliendo del juego...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // ========================================================================
    // DEBUG
    // ========================================================================
    
    /// <summary>
    /// Imprimir estado actual
    /// </summary>
    public void DebugPrintState()
    {
        Debug.Log($"[GAME MANAGER STATE]");
        Debug.Log($"  State: {currentGameState}");
        Debug.Log($"  Paused: {isPaused}");
        Debug.Log($"  Time.timeScale: {Time.timeScale}");
        Debug.Log($"  Has InputManager: {inputManager != null}");
        Debug.Log($"  Has PlayerController: {playerController != null}");
    }
}
