using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ƿ��Ƽ���� �Լ����� ����ϱ� ���� ��Ƶ� Ŭ���� �Դϴ�. 
/// </summary>
public static class HelperUtilities
{
    //ValueResetClamp
    //ValueReset
    #region b~c������ a���� -> e~f

    //�ܾƿ��� ����Ѱ�
    public static float ValueResetClamp(float a, float b, float c, float e, float f)
	{
		float t = Mathf.Clamp01((a - b) / (c - b));

		// 2. Apply Ease-In-Out transformation
		t = t * t * (3f - 2f * t);

		// 3. Map to the target range
		return Mathf.Lerp(e, f, t);
	}

    public static float ValueReset(float a, float b, float c, float e, float f)
	{
		float t =(a - b) / (c - b);

		// 2. Apply Ease-In-Out transformation
		t = t * t * (3f - 2f * t);

		// 3. Map to the target range
		return Mathf.Lerp(e, f, t);
	}
    #endregion

    

    //GetMouseWorldPosition
    #region ���콺��ġ => ���� ��ǥ
    public static Camera mainCamera;
    public static Vector3 GetMouseWorldPosition()//���콺 ��ġ ���� ��ǥ�� ��ȯ
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;

    }
    #endregion
    
    //Convert1DArrayTo2DArray
    //Convert2DArrayTo1DArray
    #region ������ <=> ������ �迭 (float[] , int[])
    public static int[,] Convert1DArrayTo2DArray(int[] array1D, int rows, int cols) //1���� �迭�� 2���� �迭��
    {
        if (array1D.Length != rows * cols)
        {
            Debug.LogError("The size of the 1D array does not match the specified dimensions for the 2D array.");
            return null;
        }

        int[,] array2D = new int[rows, cols];
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array2D[i, j] = array1D[index];
                index++;
            }
        }

        return array2D;
    }

    public static float[] Convert2DArrayTo1DArray(float[,] array2D)//2���� �迭�� 1���� �迭��
    {
        int rows = array2D.GetLength(0);
        int cols = array2D.GetLength(1);

        float[] array1D = new float[rows * cols];
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array1D[index] = array2D[i, j];
                index++;
            }
        }

        return array1D;
    }

    public static int[] Convert2DArrayTo1DArray(int[,] array2D)//2���� �迭�� 1���� �迭��
    {
        int rows = array2D.GetLength(0);
        int cols = array2D.GetLength(1);

        int[] array1D = new int[rows * cols];
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array1D[index] = array2D[i, j];
                index++;
            }
        }

        return array1D;
    }

    

    public static float[,] Convert1DArrayTo2DArray(float[] array1D, int rows, int cols) //1���� �迭�� 2���� �迭��
    {
        if (array1D.Length != rows * cols)
        {
            Debug.LogError("The size of the 1D array does not match the specified dimensions for the 2D array.");
            return null;
        }

        float[,] array2D = new float[rows, cols];
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array2D[i, j] = array1D[index];
                index++;
            }
        }

        return array2D;
    }

    #endregion
    
    //GetDirectionVectorFromAngle
    //GetAngleFromVector
    #region ���� <=> ��ǥ
    public static Vector3 GetDirectionVectorFromAngle(float angle)//��ǥ���� �߻� �� ����
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    public static float GetAngleFromVector(Vector3 vector)//����Ʈ ��ǥ ���� ���.
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degress = radians * Mathf.Rad2Deg;

        return degress;
    }
    #endregion

    //GetIntegerCoordinatesInCircularRange - ���� ������ ���� ���� ��ǥ�� ������
    //GetIntegerCoordinatesInAngleRange - ���� ������ ����� ���� ��ǥ�� ������
    //GetRandomPointInCircle - ���� ���� �� ������ ��ǥ
    #region �� ��ǥ ���
    //���� ������ ���� ���� ��ǥ�� ������
    public static List<Vector3> GetIntegerCoordinatesInCircularRange(Vector3 center, int minDistance, int maxDistance)
    {
        List<Vector3> vec = new List<Vector3>();
        for (int x = -maxDistance; x <= maxDistance; x++)
        {
            for (int y = -maxDistance; y <= maxDistance; y++)
            {
                // ��ǥ (x, y)�� ���� ���� ���� �ִ��� Ȯ��
                int distanceSquared = x * x + y * y;
                if (distanceSquared >= minDistance * minDistance && distanceSquared <= maxDistance * maxDistance)
                {
                    Vector3 position = new Vector3(center.x + x, center.y + y);

                    vec.Add(position);
                }
            }
        }
        return vec;
    }

    //���� ������ ����� ���� ��ǥ�� ������
    public static List<Vector3> GetIntegerCoordinatesInAngleRange(Vector3 center, int minDistance, int maxDistance, float startAngle, float endAngle)
    {
        List<Vector3> vec = new List<Vector3>();
        for (float angle = startAngle; angle <= endAngle; angle += 1f)
        {
            float radians = angle * Mathf.Deg2Rad; // ������ �������� ��ȯ
            for (int distance = minDistance; distance <= maxDistance; distance++)
            {
                int x = Mathf.RoundToInt(center.x + distance * Mathf.Cos(radians));
                int y = Mathf.RoundToInt(center.y + distance * Mathf.Sin(radians));

                vec.Add(new Vector3(x,y));
            }
        }
        return vec;
    }

    //���� ���� �� ������ ��ǥ
    public static Vector2 GetRandomPointInCircle(Vector2 targetVec,float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2); // 0���� 2������� ������ �������� ����
        float x = Mathf.Cos(angle) * radius; // x ��ǥ ���
        float y = Mathf.Sin(angle) * radius; // y ��ǥ ���

        // ���� �߽� ��ġ�� ��ǥ�� ���Ͽ� ��ȯ
        return new Vector2(targetVec.x + x, targetVec.y + y);
    }
    #endregion

    //DisplayArrayBySprite
    #region �迭�� Tex2d�� �ð�ȭ
    public static Texture2D DisplayArrayBySprite(float[,]map)
    {
        int wid = map.GetLength(0);
        int hei = map.GetLength(1);

        Color[] pixelColors = new Color[wid* hei];

        // Sprite�� Texture2D ��������
        Texture2D texture = new Texture2D(wid, hei);
        texture.filterMode = FilterMode.Point;

        // �� �ȼ��� ���� ���� ��������
        for (int x = 0; x < wid; x++)
        {
            for (int y = 0; y < hei; y++)
            {
                pixelColors[x + y * hei] = Color.Lerp(Color.black, Color.white, map[x,y]);
            }
        }
        // ����� ���� ����
        texture.SetPixels(pixelColors);
        texture.Apply();

        return texture;

        //spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, wid, hei), new Vector2(0.5f, 0.5f));
    }

    public static Texture2D DisplayArrayBySprite(int[,]map)
    {
        int wid = map.GetLength(0);
        int hei = map.GetLength(1);

        Color[] pixelColors = new Color[wid* hei];

        // Sprite�� Texture2D ��������
        Texture2D texture = new Texture2D(wid, hei);
        texture.filterMode = FilterMode.Point;

        // �� �ȼ��� ���� ���� ��������
        for (int x = 0; x < wid; x++)
        {
            for (int y = 0; y < hei; y++)
            {
                pixelColors[x + y * hei] = Color.Lerp(Color.black, Color.white, map[x,y]);
            }
        }
        // ����� ���� ����
        texture.SetPixels(pixelColors);
        texture.Apply();

        return texture;

        //spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, wid, hei), new Vector2(0.5f, 0.5f));
    }
    #endregion
    
    //GradiantCircle
    #region �׶���Ʈ ���� �������
        public static float[,] GradiantCircle(int diameter, float scale)
        {
            float[,] map = new float[diameter, diameter];
            float radius = diameter / 2.0f;
    
            for (int i = 0; i < diameter; i++)
            {
                for (int j = 0; j < diameter; j++)
                {
                    // ���� ��ǥ���� �߽� ��ǥ������ �Ÿ� ���
                    float distance = Vector2.Distance(new Vector2(i, j), new Vector2(diameter/2, diameter/2));
    
                    // �Ÿ��� ���������� ������ 1���� 0������ ���� ����
                    if (distance < radius)
                    {
                        // �Ÿ��� ���� ���� ���� (���÷� ������ ���� ���� ���)
                        float t = (1 - distance / radius) * scale;
                        map[i, j] = Mathf.Lerp(0.0f, 1.0f, t);
                    }
                }
            }
    
            return map;
        }
    #endregion
    
    //Gcd - �����
    //Lcm - �����
    #region ����� �����
        public static int Gcd(int n, int m)
        {
            if(m==0) return n;
            else return Gcd(m, n%m);
        }
    
        public static int Lcm(int n, int m)
        {
            return (n * m) / Gcd(n , m);
        }
    #endregion
    
}
