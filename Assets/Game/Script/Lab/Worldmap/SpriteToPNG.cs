using UnityEngine;
using System.IO;

public class SpriteToPNG : MonoBehaviour
{
    public Texture2D inputTexture; // 원본 텍스처
    public Material material; // 적용할 머티리얼

    public void SaveTextureWithMaterial(string path)
    {
        if (inputTexture == null || material == null)
        {
            Debug.LogError("Input Texture or Material is missing!");
            return;
        }

        // 1. RenderTexture 생성
        RenderTexture renderTexture = new RenderTexture(inputTexture.width, inputTexture.height, 0);

        // 2. Graphics.Blit으로 머티리얼 적용
        Graphics.Blit(inputTexture, renderTexture, material);

        // 3. RenderTexture를 Texture2D로 변환
        Texture2D outputTexture = new Texture2D(inputTexture.width, inputTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        outputTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        outputTexture.Apply();

        // 4. PNG로 저장
        byte[] bytes = outputTexture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        Debug.Log($"Saved PNG to {path}");

        // 5. 메모리 정리
        RenderTexture.active = null;
        renderTexture.Release();
        DestroyImmediate(renderTexture);
        DestroyImmediate(outputTexture);
    }

    private void Start()
    {
        // 예제: 저장 경로 지정
        string path = System.IO.Path.Combine(Application.dataPath, "ProcessedTexture.png");
        SaveTextureWithMaterial(path);
    }
}
