using UnityEngine;

//노드 저장
public class GridNodes
{
    //노드 범위
    private int width;
    private int height;

    //2차원 배열로 노드 저장
    private Node[,] gridNode;

    //Init
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        //노드 생성
        gridNode = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    //노드 가져오기
    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.Log("요구한 노드가 범위를 초과하였습니다.");
            return null;
        }
    }
}
