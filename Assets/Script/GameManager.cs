using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum UserAction
{
    Hover,
    Click,
    Move
}

public enum Team
{
    None,
    Cake,
    Burger
}

public enum PieceType
{
    Men,
    King
}

public class GameManager : MonoBehaviour, ISubject
{
    public static GameManager instance { get; private set; }
    private List<IObserver> listObserver;


    [Header("Checkers Board Settings")]
    public GameObject borderCube;
    public GameObject cakeCube;
    public GameObject burgerCube;
    public int row;
    public int col;

    [Header("Checker Piece")]
    public GameObject cake;
    public GameObject burger;

    [Header("Gameplay")]
    public bool isGameOver;
    public Team winner;
    [SerializeField] public Team whoTurn;
    [SerializeField] public int cakePieceCount;
    [SerializeField] public int burgerPieceCount;
    private float roundCountDown;
    public bool isRoundStart;


    public void AddObserver(IObserver observer)
    {
        listObserver.Add(observer);
    }

    public void NotifyObserver(UserAction action)
    {
        for (int i = 0; i < listObserver.Count; i++)
        {
            listObserver[i].OnNotify(action);
        }
    }

    public void RemoveObserver(IObserver observer)
    {
        listObserver.Remove(observer);
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);
        listObserver = new List<IObserver>();
        SceneManager.sceneLoaded += SceneLoadLogic;

    }

    private void SceneLoadLogic(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            ResetGameData();
            StartCoroutine(GameLoop());
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoadLogic;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(StartGame());
        yield return StartCoroutine(Playing());
        yield return StartCoroutine(EndGame());
    }

    private IEnumerator StartGame()
    {
        whoTurn = Team.Cake;
        winner = Team.None;
        BoardGenerator.GenerateBoard(row, col);
        MainGameCanvasManager.instance.UpdateScoreText(Team.Cake);
        MainGameCanvasManager.instance.UpdateScoreText(Team.Burger);
        yield return MainGameCanvasManager.instance.RoundStartCountDown(roundCountDown);
        isRoundStart = true;
    }

    private IEnumerator Playing()
    {
        while (!isGameOver)
        {
            yield return null;
        }
    }

    private IEnumerator EndGame()
    {
        isRoundStart = false;
        MainGameCanvasManager.instance.ToggleBlackScreen();
        MainGameCanvasManager.instance.checkBoardButton.gameObject.SetActive(true);
        MainGameCanvasManager.instance.headerContainer.SetActive(false);
        MainGameCanvasManager.instance.UpdateWinnerText();
        yield return MainGameCanvasManager.instance.resultUIState.PlayEnterAnimation();
    }

    public void SubtractPieceCount(CheckersPiece piece)
    {
        if (piece.team == Team.Cake)
        {
            cakePieceCount--;
        }
        else if (piece.team == Team.Burger)
        {
            burgerPieceCount--;
        }

    }

    public void IncreasePieceCount(CheckersPiece piece)
    {
        if (piece.team == Team.Cake)
        {
            cakePieceCount++;
        }
        else if (piece.team == Team.Burger)
        {
            burgerPieceCount++;
        }
    }

    private void ResetGameData()
    {
        isGameOver = false;
        cakePieceCount = 0;
        burgerPieceCount = 0;
        roundCountDown = 3.0f;
        isRoundStart = false;
    }
}
