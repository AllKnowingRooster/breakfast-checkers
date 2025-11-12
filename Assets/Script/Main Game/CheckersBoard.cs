using UnityEngine;
using UnityEngine.InputSystem;

public class CheckersBoard : MonoBehaviour
{
    private Camera cam;
    public GameObject[,] listCube;
    public CheckersPiece[,] listPiece;
    private CheckersPiece selectedPiece;
    private Vector3 prevSelectedPiecePos;
    private Vector2Int noTarget;
    private Vector2Int currentPointer;
    private Vector2Int lastPointer;
    private Plane draggingPlane;
    private void Awake()
    {
        cam = Camera.main;
        listCube = new GameObject[GameManager.instance.row, GameManager.instance.col];
        listPiece = new CheckersPiece[GameManager.instance.row, GameManager.instance.col];
        noTarget = Vector2Int.one * -1;
        lastPointer = noTarget;
        currentPointer = noTarget;
        draggingPlane = new Plane(Vector3.up, new Vector3(0, 1, 0));
    }

    private void Update()
    {
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
                    listCube[currentPointer.x, currentPointer.y].layer = LayerMask.NameToLayer("HoverLayer");
                    lastPointer = currentPointer;
                }
                else if (lastPointer != noTarget && lastPointer != currentPointer)
                {
                    listCube[lastPointer.x, lastPointer.y].layer = LayerMask.NameToLayer("CubeLayer");
                    listCube[currentPointer.x, currentPointer.y].layer = LayerMask.NameToLayer("HoverLayer");
                    lastPointer = currentPointer;
                }
                else if (selectedPiece == null && Mouse.current.leftButton.wasPressedThisFrame && listPiece[currentPointer.x, currentPointer.y] != null)
                {
                    selectedPiece = listPiece[currentPointer.x, currentPointer.y];
                    prevSelectedPiecePos = new Vector3(selectedPiece.x, 0.5f, selectedPiece.y);
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
                if (isValidMove())
                {
                    Vector3 newPiecePos = new Vector3(currentPointer.x, 0.5f, currentPointer.y);
                    selectedPiece.SetPosition(newPiecePos, false);
                }
                else
                {
                    ResetPosition();
                }

                selectedPiece = null;
                prevSelectedPiecePos = Vector3.zero;

            }
        }

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

    private bool isValidMove()
    {
        if (currentPointer == noTarget || listPiece[currentPointer.x, currentPointer.y] != null)
        {
            return false;
        }
        return true;
    }

    private void ResetPosition()
    {
        selectedPiece.SetPosition(prevSelectedPiecePos, false);
    }
}
