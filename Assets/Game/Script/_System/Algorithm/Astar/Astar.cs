using System.Collections.Generic;
using UnityEngine;


//정적 클래스로 생성
public static class Astar
{

    #region 생성
    //startgridposition 에서 endgridposition 까지 방의 경로를 만들고 반환된 스택에 이동 단계를 추가합니다. 

    //경로가 발견되지 않으면 null을 반환
    public static Stack<Vector3> BuildPath(AstarSettings aSetting, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        //구하는 값의 포지션을 방의 포지션에 맞게 조절

        //endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // openNode = 평가할 항목 / closeNode = 평가된 항목
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        //노드를 저장할 그리드 노드 생성
        //노드는 방의 크기에 맞게 생성
        GridNodes gridNodes = new GridNodes(aSetting.width + 1 , aSetting.height + 1);


        //start , end position 을 바탕으로 노드 생성
        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        //길찾기 시작
        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, aSetting);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, aSetting);
        }

        return null;
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet, AstarSettings aSettings)
    {
        openNodeList.Add(startNode);

        //노드가 타겟 노드면 리턴 아니면 코스트 비교하여 넘기기
        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            if (currentNode == targetNode)
            {
                return currentNode;
            }

            closedNodeHashSet.Add(currentNode);

            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, aSettings);
        }

        return null;
    }

    private static Stack<Vector3> CreatePathStack(Node targetNode, AstarSettings astarSettings)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        //Vector3 cellMidPoint = new Vector3(1,1,0) * 0.5f; //타일을 중간에 위치하게 설정 하는건데 지금은 x
        //cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            Vector3 worldPosition =  new Vector3Int(nextNode.gridPosition.x ,nextNode.gridPosition.y, 0);

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    //노드들의 코스트를 비교하여 평가
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet, AstarSettings aSettings)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes
                    , closedNodeHashSet, aSettings);

                if (validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    int movementPenaltyForGridSpace = aSettings.aStarMovementPenalty
                        [validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }


    //거리구하기
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes,
        HashSet<Node> closedNodeHashSet, AstarSettings aSettings)
    {

        //초과값
        if (neighbourNodeXPosition >= aSettings.width ||
            neighbourNodeXPosition < 0 || neighbourNodeYPosition >= aSettings.height || neighbourNodeYPosition < 0)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        int movementPenaltyForGridSpace = aSettings.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        

        if (movementPenaltyForGridSpace == 0 ||closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
    #endregion
    
    #region 유틸

    public static int[,] GetDefaultAstarPenalty(int width , int height)
    {
        int[,]aStarMovementPenalty = new int[width +1 , height+1];

        for (int x = 0; x < (width + 1); x++)
        {
            for (int y = 0; y < (height + 1); y++)
            {
                aStarMovementPenalty[x, y] = 1;
            }
        }

        return aStarMovementPenalty;
    }
    #endregion
}

public class AstarSettings
{
    public int[,] aStarMovementPenalty;//이 2D 배열을 사용하여 Astar 길 찾기에 사용할 타일맵의 이동 패널티를 저장합니다.
    public int width;
    public int height;

    public AstarSettings(int _width  = 100 , int _height = 100 , int[,] aSMP = null)
    {
        if(aSMP == null)
        aSMP = Astar.GetDefaultAstarPenalty(_width , _height);

        width = _width;
        height = _height;
        aStarMovementPenalty = aSMP;
    }

}