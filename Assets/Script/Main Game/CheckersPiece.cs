using System.Collections.Generic;
using UnityEngine;

public class CheckersPiece : MonoBehaviour
{


    public Team team;
    public int x;
    public int y;
    public PieceType pieceType;
    public Vector3 desiredPos;
    public bool canMove = false;
    private float speed = 4.0f;
    public List<Vector2Int> listMove = new List<Vector2Int>();
    public List<Vector2Int> listAttack = new List<Vector2Int>();
    private void Update()
    {
        if (transform.position == desiredPos)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, desiredPos, speed * Time.deltaTime);
        Vector3 distance = transform.position - desiredPos;
        if (distance.sqrMagnitude < 0.001f)
        {
            transform.position = desiredPos;
        }
    }

    public void SetPosition(Vector3 pos, bool force)
    {
        if (force)
        {
            transform.position = pos;
        }
        desiredPos = pos;
    }

    public virtual void GetPossibleMoves(ref CheckersPiece[,] listPiece)
    {

    }

    public void ToggleIndicator()
    {
        Transform indicator = transform.Find("Indicator");
        indicator.gameObject.SetActive(!indicator.gameObject.activeSelf);
        canMove = !canMove;
    }

}
