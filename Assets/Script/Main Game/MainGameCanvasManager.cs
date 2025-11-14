using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameCanvasManager : MonoBehaviour
{
    public static MainGameCanvasManager instance { get; private set; }
    public UIState resultUIState;
    private UIState menuUIState;
    [SerializeField] private Image mainPanel;
    public Button checkBoardButton;
    public GameObject headerContainer;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private TextMeshProUGUI cakeScoreText;
    [SerializeField] private TextMeshProUGUI burgerScoreText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private Animator resultAnimator;
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button exitButton;
    private bool isCheckingBoard;
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
        isCheckingBoard = false;
        minAlpha = 0.0f;
        maxAlpha = 0.8f;
        checkBoardButton.onClick.RemoveAllListeners();
        checkBoardButton.onClick.AddListener(() => { CheckBoard(); });
        rematchButton.onClick.RemoveAllListeners();
        rematchButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });
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
            roundText.text = string.Format("Round Start in {0}", Mathf.Floor(time).ToString());
            time -= Time.deltaTime;
            yield return null;
        }
        roundText.gameObject.SetActive(false);
    }

    private void CheckBoard()
    {
        isCheckingBoard = !isCheckingBoard;
        ToggleBlackScreen();
        CanvasGroup cg = resultCanvas.GetComponent<CanvasGroup>();
        cg.interactable = isCheckingBoard ? false : true;
        cg.alpha = isCheckingBoard ? 0.0f : 1.0f;
        cg.blocksRaycasts = isCheckingBoard ? false : true;
    }

    public void ToggleBlackScreen()
    {
        Color color = mainPanel.color;
        if (isCheckingBoard)
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



}
