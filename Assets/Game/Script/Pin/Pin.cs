using UnityEngine;
using TMPro;


public class Pin : MonoBehaviour
{
    public TextMeshProUGUI text;
    public SpriteRenderer sprite;
    public float minTextScale = 0.05f;
    public float maxTextScale = 0.25f;

    private void Update() { 
        textScale();
    }

    private void textScale()
    {
        text.fontSize = HelperUtilities.ValueResetClamp
        (MapCameraController.Instance.mainCamera.fieldOfView ,MapCameraController.Instance.minZoom , MapCameraController.Instance.maxZoom , minTextScale , maxTextScale);
    }
}
