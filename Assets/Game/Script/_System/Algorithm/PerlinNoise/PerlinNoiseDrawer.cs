using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseDrawer : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 1.0f;
    public int octaves = 3;
    public float persistance = 0.5f;
    public float lacunarity = 2;

    [Range(0,1)]
    public float heightValue;

    public SpriteRenderer spriteRenderer; // SpriteRenderer를 가진 GameObject에 연결

    float[,] noiseMap;

    private void Start() {
        SetPixel(16,16,HelperUtilities.GradiantCircle(16,1.2f));
    }
    private void Update() {
        
        if(Input.GetMouseButtonDown(0))
        {
            noiseMap = PerlinNoise.GenerateNoiseMap(width, height, scale, octaves, persistance, lacunarity);
            SetPixel(width,width,noiseMap);
        }
        
    }

    void SetPixel(int wid , int hei , float[,]map)
    {
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

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, wid, hei), new Vector2(0.5f, 0.5f));

    }


    

}
