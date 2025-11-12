using System.Collections.Generic;
using UnityEngine;

public class CheckersPiece : MonoBehaviour
{
    public Team team;
    public int x;
    public int y;
    public PieceType pieceType;
    public Vector3 desiredPos;
    private float speed = 4.0f;
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

    public virtual (List<Vector2Int>, List<Vector2Int>) GetPossibleMoves()
    {
        return (null, null);
    }

}
