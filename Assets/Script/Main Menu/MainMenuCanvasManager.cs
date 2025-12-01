using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvasManager : MonoBehaviour
{

    public static MainMenuCanvasManager instance { get; private set; }
    private UIState currentState;

    [Header("Start Menu")]
    [SerializeField] private Animator startMenuAnimator;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    private UIState startMenuState;


    [Header("Settings Menu")]
    [SerializeField] private Animator settingsMenuAnimator;
    [SerializeField] private Button settingsReturn;
    public AudioUI masterUI;
    public AudioUI sfxUI;
    public AudioUI musicUI;
    private UIState settingsMenuState;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        startMenuState = new UIState(startMenuAnimator, "Enter", "StartMenuEnter", "Exit", "StartMenuExit");
        settingsMenuState = new UIState(settingsMenuAnimator, "Enter", "SettingsMenuEnter", "Exit", "SettingsMenuExit");
        singlePlayerButton.onClick.RemoveAllListeners();
        singlePlayerButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(() => { GoToSettingsMenu(); });
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => { StartCoroutine(Exit()); });
        settingsReturn.onClick.RemoveAllListeners();
        settingsReturn.onClick.AddListener(() => { GoToStartMenu(); });
        AudioManager.instance.InitializeAudioUI(masterUI, "Master");
        AudioManager.instance.InitializeAudioUI(sfxUI, "SFX");
        AudioManager.instance.InitializeAudioUI(musicUI, "Music");
    }

    private void Start()
    {
        GoToStartMenu();
    }


    private IEnumerator ChangeState(UIState state)
    {
        if (currentState != null)
        {
            yield return StartCoroutine(currentState.PlayExitAnimation());
        }
        currentState = state;
        yield return StartCoroutine(currentState.PlayEnterAnimation());
    }

    private IEnumerator Exit()
    {
        yield return CanvasAnimator.WaitForAnimation(startMenuAnimator, "StartMenuExit", "Exit");
        Application.Quit();
    }
    private void GoToStartMenu()
    {
        StartCoroutine(ChangeState(startMenuState));
    }

    private void GoToSettingsMenu()
    {
        StartCoroutine(ChangeState(settingsMenuState));
    }
}
