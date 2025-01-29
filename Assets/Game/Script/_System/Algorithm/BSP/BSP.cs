using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize; //원하는 맵의 크기

    [Header("알고리즘")]
    [SerializeField] float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] private int maximumDepth; //트리의 높이, 높을 수록 방을 더 자세히 나누게 됨

    [Header("렌더러")]
    [SerializeField] private GameObject line; //lineRenderer를 사용해서 공간이 나눠진걸 시작적으로 보여주기 위함
    [SerializeField] private GameObject map; //lineRenderer를 사용해서 첫 맵의 사이즈를 보여주기 위함
    [SerializeField] private List<BspNode> nodes; //lineRenderer를 사용해서 첫 맵의 사이즈를 보여주기 위함
    void Start()
    { 
    BspNode root = new BspNode(new RectInt(0, 0, mapSize.x, mapSize.y)); //맵 사이즈의 첫 노드 생성
    DrawMap(0,0);
    nodes = new List<BspNode>();
    Divide(root, 0);
    }
    
    
    
    private void DrawMap(int x, int y) //x y는 화면의 중앙위치를 뜻함 (테두리 생성)
    {
        //기본적으로 mapSize/2라는 값을 계속해서 빼게 될건데, 화면의 중앙에서 화면의 크기의 반을 빼줘야 좌측 하단좌표를 구할 수 있기 때문이다.
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2); //좌측 하단
        lineRenderer.SetPosition(1, new Vector2(x + mapSize.x, y) - mapSize / 2); //우측 하단
        lineRenderer.SetPosition(2, new Vector2(x + mapSize.x, y + mapSize.y) - mapSize / 2);//우측 상단
        lineRenderer.SetPosition(3, new Vector2(x, y + mapSize.y) - mapSize / 2); //좌측 상단

    }
    void Divide(BspNode tree,int n)
    {
        if (n == maximumDepth) 
        {
            nodes.Add(tree);
            return; //최대 분할시 리턴
        }

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);//width,height 중 긴 값 반환
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));//나누는 비율에 따라 랜덤 반환


        if (tree.nodeRect.width >= tree.nodeRect.height) //가로 (좌,우 분할)
        {
            tree.leftNode = new BspNode(new RectInt(tree.nodeRect.x,tree.nodeRect.y,split,tree.nodeRect.height));//왼쪽 노드
            tree.rightNode= new BspNode(new RectInt(tree.nodeRect.x+split, tree.nodeRect.y, tree.nodeRect.width-split, tree.nodeRect.height));//우측 노드
            DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
        }
        else //세로
        {
            tree.leftNode = new BspNode(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width,split));
            tree.rightNode = new BspNode(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width , tree.nodeRect.height-split));
            DrawLine(new Vector2(tree.nodeRect.x , tree.nodeRect.y+ split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y  + split));
       
        }
        tree.leftNode.parNode = tree; //자식노드들의 부모노드를 나누기전 노드로 설정
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); //왼쪽, 오른쪽 자식 노드들도 나눠준다.
        Divide(tree.rightNode, n + 1);//왼쪽, 오른쪽 자식 노드들도 나눠준다.
    }
    private void DrawLine(Vector2 from, Vector2 to) //from->to로 이어지는 선을 그리게 될 것이다.
    {
        LineRenderer lineRenderer = Instantiate(line).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }
}
