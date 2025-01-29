using UnityEngine;
using System.IO;

public class GrayScaleToPixel : MonoBehaviour
{
    public Texture2D texture; // PNG 파일을 Texture2D로 불러오기
    public SpriteRenderer targetRenderer; // 결과 텍스처를 표시할 오브젝트
    public int blockSize = 6; // n*n 블록 크기
    public string fileName = "Texture.png";
    public bool isReverse = true;//색 반전
    public bool isDrawOcean = true;//바다 유무
    public ColorByHeight[] colorByHeights;
    public Color OceanColor;
    private Texture2D outputTexture;

    [Space(10)]
    public bool isPerlinNoise = true;//펠른 노이즈 가져오기
    public int Perwidth = 256;
    public int Perheight = 256;
    public float scale = 1.0f;
    public int octaves = 3;
    public float persistance = 0.5f;
    public float lacunarity = 2;



    private float[] pixelValues;



    void Start()
    {
        Generate();

    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Generate();
        }

    }

    public void Generate()
    {
        if (texture == null)
        {
            Debug.LogError("텍스처를 지정해 주세요.");
            return;
        }

        int width;
        int height;

        if (isPerlinNoise)
        {
            pixelValues = HelperUtilities.Convert2DArrayTo1DArray(PerlinNoise.GenerateNoiseMap(Perwidth, Perheight, scale, octaves, persistance, lacunarity));

            width = Perwidth;
            height = Perheight;
        }
        else
        {
            width = texture.width / blockSize; // 블록 단위로 축소된 너비
            height = texture.height / blockSize; // 블록 단위로 축소된 높이

            // 블록 평균값을 저장할 float 배열 생성
            pixelValues = new float[width * height];

            for (int by = 0; by < height; by++)
            {
                for (int bx = 0; bx < width; bx++)
                {
                    // 블록 평균값 계산
                    float averageValue = GetBlockAverage(bx, by, blockSize);
                    pixelValues[by * width + bx] = averageValue;
                }
            }


        }

        // 일부 값 디버깅 출력
        outputTexture = GenerateTexture(pixelValues, width, height);

        Sprite newSprite = Sprite.Create(outputTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));


        // 결과를 오브젝트에 표시
        if (targetRenderer != null)
        {
            targetRenderer.sprite = newSprite;
        }

        //DebugPixels(width, height);
    }

    public void TexttoPng()
    {
        // 텍스처를 PNG로 변환
        byte[] pngData = outputTexture.EncodeToPNG();
        if (pngData == null)
        {
            Debug.LogError("PNG 변환에 실패했습니다.");
            return;
        }

        // 저장 경로 (Application.persistentDataPath 사용)
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        // 파일 저장
        File.WriteAllBytes(filePath, pngData);
        Debug.Log($"텍스처가 저장되었습니다: {filePath}");
    }

    private float GetBlockAverage(int blockX, int blockY, int blockSize)
    {
        float sum = 0;
        int pixelCount = 0;

        // 블록 내 픽셀 순회
        for (int y = 0; y < blockSize; y++)
        {
            for (int x = 0; x < blockSize; x++)
            {
                int px = blockX * blockSize + x;
                int py = blockY * blockSize + y;

                if (px < texture.width && py < texture.height) // 경계 검사
                {
                    Color pixelColor = texture.GetPixel(px, py);

                    if (isReverse)
                        sum += 1.0f - pixelColor.grayscale; // 흑 = 1, 백 = 0
                    else
                        sum = pixelColor.grayscale;
                    pixelCount++;
                }
            }
        }

        return pixelCount > 0 ? sum / pixelCount : 0; // 평균값 계산
    }

    private Texture2D GenerateTexture(float[] pixelValues, int width, int height)
    {
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = pixelValues[y * width + x];

                Color color = OceanColor;

                for (int i = 0; i < colorByHeights.Length; i++)
                {
                    if (value <= colorByHeights[i].maxValue && value != 0)
                    {
                        color = colorByHeights[i].color;
                        break;
                    }

                }

                if (color == OceanColor && !isDrawOcean)
                    newTexture.SetPixel(x, y, new Color(1, 0, 1, 1));
                else
                    newTexture.SetPixel(x, y, color);
            }
        }

        newTexture.filterMode = FilterMode.Point; // Point (No Filter)
        newTexture.wrapMode = TextureWrapMode.Clamp; // Clamp or Repeat

        // 텍스처 압축 해제
        newTexture.Apply(false, false); // Apply 호출로 변경 사항 적용
        return newTexture;
    }

    private void DebugPixels(int width, int height)
    {
        Debug.Log("디버깅용 출력:");

        Debug.Log($" {width} {height}, {pixelValues.Length}");

        for (int i = 0; i < width; i++)
        {
            Debug.Log($" {i} {height / 2}, {pixelValues[width * height / 2 + i]}");
        }
    }
}

[System.Serializable]
public class ColorByHeight
{
    [Range(0, 1)]
    public float maxValue;
    public Color color;
}