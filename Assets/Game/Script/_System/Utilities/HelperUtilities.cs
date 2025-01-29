using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유틸리티적인 함수들을 사용하기 쉽게 모아둔 클래스 입니다. 
/// </summary>
public static class HelperUtilities
{
    //ValueResetClamp
    //ValueReset
    #region b~c사이의 a값을 -> e~f

    //줌아웃에 사용한거
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
    #region 마우스위치 => 월드 좌표
    public static Camera mainCamera;
    public static Vector3 GetMouseWorldPosition()//마우스 위치 월드 좌표로 전환
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
    #region 이차원 <=> 일차원 배열 (float[] , int[])
    public static int[,] Convert1DArrayTo2DArray(int[] array1D, int rows, int cols) //1차원 배열을 2차원 배열로
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

    public static float[] Convert2DArrayTo1DArray(float[,] array2D)//2차원 배열을 1차원 배열로
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

    public static int[] Convert2DArrayTo1DArray(int[,] array2D)//2차원 배열을 1차원 배열로
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

    

    public static float[,] Convert1DArrayTo2DArray(float[] array1D, int rows, int cols) //1차원 배열을 2차원 배열로
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
    #region 각도 <=> 좌표
    public static Vector3 GetDirectionVectorFromAngle(float angle)//목표지점 발사 시 각도
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    public static float GetAngleFromVector(Vector3 vector)//포인트 좌표 각도 얻기.
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degress = radians * Mathf.Rad2Deg;

        return degress;
    }
    #endregion

    //GetIntegerCoordinatesInCircularRange - 일정 범위의 원형 정수 좌표를 가져옴
    //GetIntegerCoordinatesInAngleRange - 일정 범위의 방사형 정수 좌표를 가져옴
    //GetRandomPointInCircle - 원형 범위 내 무작위 좌표
    #region 원 좌표 얻기
    //일정 범위의 원형 정수 좌표를 가져옴
    public static List<Vector3> GetIntegerCoordinatesInCircularRange(Vector3 center, int minDistance, int maxDistance)
    {
        List<Vector3> vec = new List<Vector3>();
        for (int x = -maxDistance; x <= maxDistance; x++)
        {
            for (int y = -maxDistance; y <= maxDistance; y++)
            {
                // 좌표 (x, y)가 원형 범위 내에 있는지 확인
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

    //일정 범위의 방사형 정수 좌표를 가져옴
    public static List<Vector3> GetIntegerCoordinatesInAngleRange(Vector3 center, int minDistance, int maxDistance, float startAngle, float endAngle)
    {
        List<Vector3> vec = new List<Vector3>();
        for (float angle = startAngle; angle <= endAngle; angle += 1f)
        {
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            for (int distance = minDistance; distance <= maxDistance; distance++)
            {
                int x = Mathf.RoundToInt(center.x + distance * Mathf.Cos(radians));
                int y = Mathf.RoundToInt(center.y + distance * Mathf.Sin(radians));

                vec.Add(new Vector3(x,y));
            }
        }
        return vec;
    }

    //원형 범위 내 무작위 좌표
    public static Vector2 GetRandomPointInCircle(Vector2 targetVec,float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2); // 0에서 2π까지의 각도를 무작위로 선택
        float x = Mathf.Cos(angle) * radius; // x 좌표 계산
        float y = Mathf.Sin(angle) * radius; // y 좌표 계산

        // 원의 중심 위치에 좌표를 더하여 반환
        return new Vector2(targetVec.x + x, targetVec.y + y);
    }
    #endregion

    //DisplayArrayBySprite
    #region 배열을 Tex2d로 시각화
    public static Texture2D DisplayArrayBySprite(float[,]map)
    {
        int wid = map.GetLength(0);
        int hei = map.GetLength(1);

        Color[] pixelColors = new Color[wid* hei];

        // Sprite의 Texture2D 가져오기
        Texture2D texture = new Texture2D(wid, hei);
        texture.filterMode = FilterMode.Point;

        // 각 픽셀의 색상 정보 가져오기
        for (int x = 0; x < wid; x++)
        {
            for (int y = 0; y < hei; y++)
            {
                pixelColors[x + y * hei] = Color.Lerp(Color.black, Color.white, map[x,y]);
            }
        }
        // 변경된 색상 적용
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

        // Sprite의 Texture2D 가져오기
        Texture2D texture = new Texture2D(wid, hei);
        texture.filterMode = FilterMode.Point;

        // 각 픽셀의 색상 정보 가져오기
        for (int x = 0; x < wid; x++)
        {
            for (int y = 0; y < hei; y++)
            {
                pixelColors[x + y * hei] = Color.Lerp(Color.black, Color.white, map[x,y]);
            }
        }
        // 변경된 색상 적용
        texture.SetPixels(pixelColors);
        texture.Apply();

        return texture;

        //spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, wid, hei), new Vector2(0.5f, 0.5f));
    }
    #endregion
    
    //GradiantCircle
    #region 그라디언트 원을 만들어줌
        public static float[,] GradiantCircle(int diameter, float scale)
        {
            float[,] map = new float[diameter, diameter];
            float radius = diameter / 2.0f;
    
            for (int i = 0; i < diameter; i++)
            {
                for (int j = 0; j < diameter; j++)
                {
                    // 현재 좌표에서 중심 좌표까지의 거리 계산
                    float distance = Vector2.Distance(new Vector2(i, j), new Vector2(diameter/2, diameter/2));
    
                    // 거리가 반지름보다 작으면 1에서 0까지의 값을 설정
                    if (distance < radius)
                    {
                        // 거리에 따라 값을 조절 (예시로 간단한 선형 보간 사용)
                        float t = (1 - distance / radius) * scale;
                        map[i, j] = Mathf.Lerp(0.0f, 1.0f, t);
                    }
                }
            }
    
            return map;
        }
    #endregion
    
    //Gcd - 공약수
    //Lcm - 공배수
    #region 공약수 공배수
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
