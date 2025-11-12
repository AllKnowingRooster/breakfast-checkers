using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum UserAction
{
    Hover,
    Click
}

public enum Team
{
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

    public bool isGameOver;
    public Team winner;

    [Header("Checkers Board Settings")]
    public GameObject borderCube;
    public GameObject cakeCube;
    public GameObject burgerCube;
    public int row;
    public int col;

    [Header("Checker Piece")]
    public GameObject cake;
    public GameObject burger;

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
        isGameOver = false;
        SceneManager.sceneLoaded += SceneLoadLogic;
    }

    private void SceneLoadLogic(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
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
        BoardGenerator.GenerateBoard(row, col);
        yield return null;
    }

    private IEnumerator Playing()
    {
        yield return null;
    }

    private IEnumerator EndGame()
    {
        yield return null;
    }
}
