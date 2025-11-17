using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameCanvasManager : MonoBehaviour
{
    public static MainGameCanvasManager instance { get; private set; }

    [Header("Result")]
    public UIState resultUIState;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private Animator resultAnimator;
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button exitButton;

    [Header("Menu")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitMenuButton;
    [SerializeField] private GameObject settingsCanvas;

    [Header("Main")]
    public Button checkBoardButton;
    public GameObject headerContainer;
    [SerializeField] private Image mainPanel;
    [SerializeField] private TextMeshProUGUI cakeScoreText;
    [SerializeField] private TextMeshProUGUI burgerScoreText;
    [SerializeField] private Button menuButton;

    [Header("Settings")]
    public AudioUI masterUI;
    public AudioUI sfxUI;
    public AudioUI musicUI;
    [SerializeField] public Button returnButton;

    private bool hideBackground;
    private bool hideResult;
    private float minAlpha;
    private float maxAlpha;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;
        resultUIState = new UIState(resultAnimator, "Enter", "ResultMenuEnter", "Exit", "ResultMenuExit");
        hideBackground = false;
        hideResult = false;
        minAlpha = 0.0f;
        maxAlpha = 0.8f;
        checkBoardButton.onClick.RemoveAllListeners();
        checkBoardButton.onClick.AddListener(() => { CheckBoard(); });
        rematchButton.onClick.RemoveAllListeners();
        rematchButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });
        menuButton.onClick.RemoveAllListeners();
        menuButton.onClick.AddListener(() => { ToggleMenu(); });
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => { ToggleMenu(); });
        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(() => { ToggleSettings(); });
        exitMenuButton.onClick.RemoveAllListeners();
        exitMenuButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() => { ToggleSettings(); });
        AudioManager.instance.InitializeAudioUI(masterUI, "Master");
        AudioManager.instance.InitializeAudioUI(sfxUI, "SFX");
        AudioManager.instance.InitializeAudioUI(musicUI, "Music");
    }

    public void UpdateScoreText(Team team)
    {
        if (team == Team.Cake)
        {
            cakeScoreText.text = GameManager.instance.cakePieceCount.ToString();
        }
        else if (team == Team.Burger)
        {
            burgerScoreText.text = GameManager.instance.burgerPieceCount.ToString();
        }
    }

    public IEnumerator RoundStartCountDown(float time)
    {
        roundText.gameObject.SetActive(true);
        while (time > 0.0f)
        {
            roundText.text = string.Format("Round Start in {0}", Mathf.Ceil(time).ToString());
            time -= Time.deltaTime;
            yield return null;
        }
        roundText.gameObject.SetActive(false);
    }

    private void CheckBoard()
    {
        ToggleBlackScreen();
        CanvasGroup cg = resultCanvas.GetComponent<CanvasGroup>();
        hideResult = !hideResult;
        cg.interactable = hideResult ? false : true;
        cg.alpha = hideResult ? 0.0f : 1.0f;
        cg.blocksRaycasts = hideResult ? false : true;
    }

    public void ToggleBlackScreen()
    {
        hideBackground = !hideBackground;
        Color color = mainPanel.color;
        if (!hideBackground)
        {
            color.a = minAlpha;
        }
        else
        {
            color.a = maxAlpha;
        }
        mainPanel.color = color;
    }

    public void UpdateWinnerText()
    {
        winnerText.text = string.Format("{0} Is The Winner", GameManager.instance.winner == Team.Burger ? "Burger" : "Cake");
    }

    private void ToggleMenu()
    {
        ToggleBlackScreen();
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        headerContainer.SetActive(!headerContainer.activeSelf);
    }

    private void ToggleSettings()
    {
        settingsCanvas.SetActive(!settingsCanvas.activeSelf);
        CanvasGroup cg = settingsCanvas.GetComponent<CanvasGroup>();
        cg.interactable = cg.interactable ? false : true;
        cg.alpha = cg.alpha == 1.0f ? 0.0f : 1.0f;
        cg.blocksRaycasts = cg.blocksRaycasts ? false : true;
        cg = menuCanvas.GetComponent<CanvasGroup>();
        cg.interactable = cg.interactable ? false : true;
        cg.blocksRaycasts = cg.blocksRaycasts ? false : true;
    }

}
