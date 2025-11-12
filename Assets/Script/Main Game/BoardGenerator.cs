using UnityEngine;

public static class BoardGenerator
{

    private static GameObject CreateObject(string name)
    {
        GameObject obj = new GameObject();
        obj.name = name;
        return obj;
    }
    private static void GenerateCube(int row, int col, ref GameObject[,] listTile, Transform parent)
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                GameObject boardCube;
                if (i % 2 == j % 2)
                {
                    boardCube = GameObject.Instantiate(GameManager.instance.cakeCube, new Vector3(i, 0, j), Quaternion.identity, parent);
                    boardCube.name = "Cake Cube";
                }
                else
                {
                    boardCube = GameObject.Instantiate(GameManager.instance.burgerCube, new Vector3(i, 0, j), Quaternion.identity, parent);
                    boardCube.name = "Burger Cube";
                }
                listTile[i, j] = boardCube;
            }
        }
    }
    private static void GenerateBorder(int maxBoard, string name, Transform parent)
    {
        for (int i = 0; i < maxBoard; i++)
        {
            GameObject bottomBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(-1, 0, i), Quaternion.identity, parent);
            GameObject rightBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(i, 0, -1), Quaternion.identity, parent);
            GameObject leftBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(8, 0, i), Quaternion.identity, parent);
            GameObject topBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(i, 0, 8), Quaternion.identity, parent);
            bottomBorder.name = name;
            rightBorder.name = name;
            leftBorder.name = name;
            topBorder.name = name;
        }

        GameObject bottomRightBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(-1, 0, -1), Quaternion.identity, parent);
        GameObject bottomLeftBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(8, 0, -1), Quaternion.identity, parent);
        GameObject topRightBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(-1, 0, 8), Quaternion.identity, parent);
        GameObject topLeftBorder = GameObject.Instantiate(GameManager.instance.borderCube, new Vector3(8, 0, 8), Quaternion.identity, parent);
    }

    private static void GenerateAllPiece(ref CheckersPiece[,] listPiece, Transform parent)
    {
        GenerateSinglePiece(Team.Cake, 0, 1, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 1, 0, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 1, 2, parent, ref listPiece);

        GenerateSinglePiece(Team.Cake, 2, 1, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 3, 0, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 3, 2, parent, ref listPiece);

        GenerateSinglePiece(Team.Cake, 4, 1, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 5, 0, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 5, 2, parent, ref listPiece);

        GenerateSinglePiece(Team.Cake, 6, 1, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 7, 0, parent, ref listPiece);
        GenerateSinglePiece(Team.Cake, 7, 2, parent, ref listPiece);


        GenerateSinglePiece(Team.Burger, 7, 6, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 6, 7, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 6, 5, parent, ref listPiece);

        GenerateSinglePiece(Team.Burger, 5, 6, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 4, 7, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 4, 5, parent, ref listPiece);

        GenerateSinglePiece(Team.Burger, 3, 6, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 2, 7, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 2, 5, parent, ref listPiece);

        GenerateSinglePiece(Team.Burger, 1, 6, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 0, 7, parent, ref listPiece);
        GenerateSinglePiece(Team.Burger, 0, 5, parent, ref listPiece);
    }

    private static void GenerateSinglePiece(Team team, int x, int y, Transform parent, ref CheckersPiece[,] listPiece)
    {
        Vector3 positionOnBoard = new Vector3(x, 0.5f, y);
        GameObject piecePrefab = team == Team.Cake ? GameManager.instance.cake : GameManager.instance.burger;
        Quaternion rotation = team == Team.Cake ? Quaternion.identity : Quaternion.identity;
        CheckersPiece piece = GameObject.Instantiate(piecePrefab, positionOnBoard, rotation, parent).AddComponent<CheckersPiece>();
        piece.transform.name = team == Team.Cake ? "Cake Piece" : "Burger Piece";
        //piece.gameObject.layer = LayerMask.NameToLayer("Default");
        piece.x = x;
        piece.y = y;
        piece.team = team;
        piece.pieceType = PieceType.Men;
        piece.SetPosition(positionOnBoard, true);
        listPiece[x, y] = piece;
    }

    public static void GenerateBoard(int row, int col)
    {
        CheckersBoard checkersBoard = CreateObject("Checkers Board").AddComponent<CheckersBoard>();
        GameObject checkerPiece = CreateObject("Checkers Piece");
        GameObject checkerCube = CreateObject("Checkers Cube");
        GameObject checkerBorder = CreateObject("Checkers Border");
        checkerPiece.transform.parent = checkersBoard.transform;
        checkerCube.transform.parent = checkersBoard.transform;
        checkerBorder.transform.parent = checkersBoard.transform;
        GenerateCube(row, col, ref checkersBoard.listCube, checkerCube.transform);
        GenerateBorder(row, "Border Cube", checkerBorder.transform);
        GenerateAllPiece(ref checkersBoard.listPiece, checkerPiece.transform);
    }
}
