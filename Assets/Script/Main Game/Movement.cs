using System.Collections.Generic;
using UnityEngine;

public static class Movement
{
    private static bool IsEmpty(ref CheckersPiece[,] listPiece, Vector2Int newPos)
    {
        if (listPiece[newPos.x, newPos.y] != null)
        {
            return false;
        }
        return true;
    }

    private static bool IsInBoard(Vector2Int newPos)
    {
        if (newPos.x < 0 || newPos.y < 0 || newPos.x >= GameManager.instance.row || newPos.y >= GameManager.instance.col)
        {
            return false;
        }
        return true;
    }

    private static bool IsCapturable(Vector2Int pattern, ref CheckersPiece[,] listPiece, Vector2Int newPos)
    {
        Vector2Int pos = newPos + pattern;
        if (IsInBoard(pos) && IsEmpty(ref listPiece, pos))
        {
            return true;
        }
        return false;
    }

    private static bool IsEnemy(Team pieceTeam, ref CheckersPiece[,] listPiece, Vector2Int newPos)
    {
        if (listPiece[newPos.x, newPos.y].team != pieceTeam)
        {
            return true;
        }
        return false;
    }

    public static (List<Vector2Int>, List<Vector2Int>) GenerateMoves(CheckersPiece piece, ref List<Vector2Int> movementPattern, ref CheckersPiece[,] listPiece)
    {
        List<Vector2Int> listMove = new List<Vector2Int>();
        List<Vector2Int> listAttack = new List<Vector2Int>();
        bool hasAttackablePiece = false;
        for (int i = 0; i < movementPattern.Count; i++)
        {
            Vector2Int newPos = new Vector2Int(piece.x + movementPattern[i].x, piece.y + movementPattern[i].y);
            if (!IsInBoard(newPos))
            {
                continue;
            }

            if (IsEmpty(ref listPiece, newPos))
            {
                if (hasAttackablePiece)
                {
                    continue;
                }
                listMove.Add(newPos);
            }
            else if (IsEnemy(piece.team, ref listPiece, newPos))
            {
                if (IsCapturable(movementPattern[i], ref listPiece, newPos))
                {
                    listAttack.Add(newPos + movementPattern[i]);
                    hasAttackablePiece = true;
                }
            }
        }

        if (hasAttackablePiece)
        {
            listMove.Clear();
        }

        return (listMove, listAttack);
    }

}
