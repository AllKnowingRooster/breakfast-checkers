using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckersBoard : MonoBehaviour
{
    private Camera cam;
    public GameObject[,] listCube;
    public CheckersPiece[,] listPiece;
    private CheckersPiece selectedPiece;
    private Vector2Int prevSelectedPiecePos;
    private Vector2Int noTarget;
    private Vector2Int currentPointer;
    private Vector2Int lastPointer;
    private Plane draggingPlane;
    private List<CheckersPiece> burgerDeadzone;
    private List<CheckersPiece> cakeDeadzone;
    private float deadStackoffset;
    private List<CheckersPiece> togglePieces;
    private bool moveAgain = false;
    public Transform cinemachineCameras;
    private void Awake()
    {
        cam = Camera.main;
        listCube = new GameObject[GameManager.instance.row, GameManager.instance.col];
        listPiece = new CheckersPiece[GameManager.instance.row, GameManager.instance.col];
        noTarget = Vector2Int.one * -1;
        lastPointer = noTarget;
        currentPointer = noTarget;
        draggingPlane = new Plane(Vector3.up, new Vector3(0, 1, 0));
        burgerDeadzone = new List<CheckersPiece>();
        cakeDeadzone = new List<CheckersPiece>();
        deadStackoffset = 0.7f;
        togglePieces = new List<CheckersPiece>();
    }
    private void Update()
    {
        if (GameManager.instance.isGameOver || !GameManager.instance.isRoundStart)
        {
            return;
        }
        RaycastHit hit;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.GetMask("Default")))
        {
            currentPointer = FindCube(hit.collider);
            if (currentPointer != noTarget)
            {
                if (lastPointer == noTarget)
                {
                    listCube[currentPointer.x, currentPointer.y].layer = ChangeLayer(currentPointer);
                    lastPointer = currentPointer;
                }
                else if (lastPointer != noTarget && lastPointer != currentPointer)
                {
                    listCube[lastPointer.x, lastPointer.y].layer = ChangeLayer(lastPointer);
                    listCube[currentPointer.x, currentPointer.y].layer = ChangeLayer(currentPointer);
                    lastPointer = currentPointer;
                }
                else if (selectedPiece == null && Mouse.current.leftButton.wasPressedThisFrame && listPiece[currentPointer.x, currentPointer.y] != null && GameManager.instance.whoTurn == listPiece[currentPointer.x, currentPointer.y].team && listPiece[currentPointer.x, currentPointer.y].canMove)
                {
                    selectedPiece = listPiece[currentPointer.x, currentPointer.y];
                    prevSelectedPiecePos = new Vector2Int(selectedPiece.x, selectedPiece.y);
                    ShowHighlight();
                }
            }
            else if (currentPointer == noTarget)
            {
                if (lastPointer != noTarget)
                {
                    listCube[lastPointer.x, lastPointer.y].layer = LayerMask.NameToLayer("CubeLayer");
                    lastPointer = noTarget;
                }
            }
        }

        if (selectedPiece != null)
        {
            float point;
            if (draggingPlane.Raycast(ray, out point))
            {
                Vector3 hitPointPlane = ray.GetPoint(point);
                selectedPiece.SetPosition(hitPointPlane, false);
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                MovePiece(currentPointer);
            }
        }

    }

    private void PutPiece()
    {
        selectedPiece = null;
        prevSelectedPiecePos = noTarget;
    }

    private void ChangeTurn()
    {
        PutPiece();
        ToggleAvailablePiece();
        togglePieces.Clear();
        GameManager.instance.whoTurn = GameManager.instance.whoTurn == Team.Burger ? Team.Cake : Team.Burger;
        SwapCamera();
        GetAllTogglablePiece();
    }
    private void ResetPosition()
    {
        Vector3 piecePos = new Vector3(prevSelectedPiecePos.x, 0.5f, prevSelectedPiecePos.y);
        selectedPiece.SetPosition(piecePos, false);
        HideHighlight();
        PutPiece();
    }

    private Vector2Int FindCube(Collider hitCollider)
    {
        for (int i = 0; i < GameManager.instance.row; i++)
        {
            for (int j = 0; j < GameManager.instance.col; j++)
            {
                if (listCube[i, j] == hitCollider.gameObject)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return noTarget;
    }

    private void MovePiece(Vector2Int pos)
    {
        bool isAttacking = false;
        if (!IsValidMove(pos))
        {
            ResetPosition();
            return;
        }

        if (isPieceAttacking(pos))
        {
            CheckersPiece enemyPiece = FindEnemy(pos);
            KillPiece(enemyPiece, ref (enemyPiece.team == Team.Cake ? ref cakeDeadzone : ref burgerDeadzone));
            isAttacking = true;
        }
        AfterMove(pos);
        if (selectedPiece.pieceType == PieceType.Men)
        {
            selectedPiece.GetComponent<ManPiece>().Promote(ref listPiece, ref selectedPiece);
        }

        HideHighlight();
        if (!isAttacking)
        {
            moveAgain = false;
        }
        else
        {
            if (CanMoveAgain())
            {
                ToggleAvailablePiece();
                togglePieces.Clear();
                togglePieces.Add(selectedPiece);
                selectedPiece.ToggleIndicator();
                selectedPiece = null;
                moveAgain = true;
            }
            else
            {
                moveAgain = false;
            }
        }

        if (!moveAgain)
        {
            CheckGameover();
            if (!GameManager.instance.isGameOver)
            {
                ChangeTurn();
            }
        }
    }

    private void CheckGameover()
    {
        if (GameManager.instance.cakePieceCount == 0 || GameManager.instance.burgerPieceCount == 0)
        {
            GameManager.instance.isGameOver = true;
            GameManager.instance.winner = GameManager.instance.whoTurn == Team.Cake ? Team.Cake : Team.Burger;
        }
    }

    private bool CanMoveAgain()
    {
        selectedPiece.GetPossibleMoves(ref listPiece);

        if (selectedPiece.listAttack.Count > 0)
        {
            return true;
        }
        return false;
    }
    private bool isPieceAttacking(Vector2Int pos)
    {
        if (selectedPiece.listMove.Contains(pos))
        {
            return false;
        }
        return true;
    }
    private CheckersPiece FindEnemy(Vector2Int pos)
    {
        Vector2Int whichPattern = new Vector2Int(pos.x - selectedPiece.x, pos.y - selectedPiece.y) / 2;
        Vector2Int enemyPos = new Vector2Int(selectedPiece.x + whichPattern.x, selectedPiece.y + whichPattern.y);
        return listPiece[enemyPos.x, enemyPos.y];
    }
    private void AfterMove(Vector2Int pos)
    {
        Vector3 vector3Pos = new Vector3(pos.x, 0.5f, pos.y);
        selectedPiece.SetPosition(vector3Pos, false);
        listPiece[prevSelectedPiecePos.x, prevSelectedPiecePos.y] = null;
        listPiece[pos.x, pos.y] = selectedPiece;
        selectedPiece.x = pos.x;
        selectedPiece.y = pos.y;
    }
    private void KillPiece(CheckersPiece piece, ref List<CheckersPiece> deadzone)
    {
        Vector2Int deadPos = new Vector2Int(piece.team == Team.Cake ? -1 : 8, 1 + (deadzone.Count / 2));
        Vector3 vector3DeadPos = new Vector3(deadPos.x, (deadStackoffset * ((deadzone.Count % 2) + 1)), deadPos.y);
        deadzone.Add(piece);
        piece.SetPosition(vector3DeadPos, false);
        listPiece[piece.x, piece.y] = null;
        piece.x = deadPos.x;
        piece.y = deadPos.y;
        GameManager.instance.SubtractPieceCount(piece);
        MainGameCanvasManager.instance.UpdateScoreText(piece.team);
    }

    private void ShowHighlight()
    {
        for (int i = 0; i < selectedPiece.listMove.Count; i++)
        {
            listCube[selectedPiece.listMove[i].x, selectedPiece.listMove[i].y].layer = ChangeLayer(selectedPiece.listMove[i]);
        }

        for (int i = 0; i < selectedPiece.listAttack.Count; i++)
        {
            listCube[selectedPiece.listAttack[i].x, selectedPiece.listAttack[i].y].layer = ChangeLayer(selectedPiece.listAttack[i]);
        }
    }

    private void HideHighlight()
    {
        for (int i = 0; i < selectedPiece.listMove.Count; i++)
        {
            listCube[selectedPiece.listMove[i].x, selectedPiece.listMove[i].y].layer = LayerMask.NameToLayer("CubeLayer");
        }

        for (int i = 0; i < selectedPiece.listAttack.Count; i++)
        {
            listCube[selectedPiece.listAttack[i].x, selectedPiece.listAttack[i].y].layer = LayerMask.NameToLayer("CubeLayer");
        }
    }


    private LayerMask ChangeLayer(Vector2Int pos)
    {
        if (pos == currentPointer)
        {
            return LayerMask.NameToLayer("HoverLayer");
        }
        else if (selectedPiece != null && selectedPiece.listMove.Count > 0 && selectedPiece.listMove.Contains(pos))
        {
            return LayerMask.NameToLayer("MoveLayer");
        }
        else if (selectedPiece != null && selectedPiece.listAttack.Count > 0 && selectedPiece.listAttack.Contains(pos))
        {
            return LayerMask.NameToLayer("AttackLayer");
        }
        return LayerMask.NameToLayer("CubeLayer");
    }

    private bool IsValidMove(Vector2Int pos)
    {
        if (currentPointer == noTarget || listPiece[pos.x, pos.y] != null || (!selectedPiece.listAttack.Contains(pos) && !selectedPiece.listMove.Contains(pos)))
        {
            return false;
        }
        return true;
    }

    public void GetAllTogglablePiece()
    {
        bool hasPieceCanAttack = false;
        for (int i = 0; i < GameManager.instance.row; i++)
        {
            for (int j = 0; j < GameManager.instance.col; j++)
            {
                if (listPiece[i, j] != null && listPiece[i, j].team == GameManager.instance.whoTurn)
                {
                    listPiece[i, j].GetPossibleMoves(ref listPiece);
                    if (listPiece[i, j].listMove.Count == 0 && listPiece[i, j].listAttack.Count == 0)
                    {
                        continue;
                    }
                    if (listPiece[i, j].listAttack.Count > 0)
                    {
                        if (!hasPieceCanAttack)
                        {
                            togglePieces.Clear();
                            hasPieceCanAttack = true;
                        }
                        togglePieces.Add(listPiece[i, j]);
                    }
                    else if (listPiece[i, j].listMove.Count > 0 && !hasPieceCanAttack)
                    {
                        togglePieces.Add(listPiece[i, j]);
                    }
                }
            }
        }
        if (togglePieces.Count == 0)
        {
            GameManager.instance.isGameOver = true;
            GameManager.instance.winner = GameManager.instance.whoTurn == Team.Cake ? Team.Burger : Team.Cake;
        }
        ToggleAvailablePiece();
    }

    private void ToggleAvailablePiece()
    {
        for (int i = 0; i < togglePieces.Count; i++)
        {
            togglePieces[i].ToggleIndicator();
        }
    }

    public void SwapCamera()
    {
        DisableCameras();
        if (GameManager.instance.whoTurn == Team.Cake)
        {
            cinemachineCameras.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            cinemachineCameras.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void DisableCameras()
    {
        Debug.Log(cinemachineCameras);
        for (int i = 0; i < cinemachineCameras.childCount; i++)
        {
            cinemachineCameras.GetChild(i).gameObject.SetActive(false);
        }
    }

}
