using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;





public class WorldmapUIManager : MonoBehaviour
{
    public CountryLoader countryLoader;
    public GameObject pinPrefab;
    public GameObject highlightObject;

    //추후에 하이라이트 전용 스크립트 따로 만들기

    public TextMeshProUGUI countryName;
    public TextMeshProUGUI countryPop;
    public Image countryFlag;

    //인덱스 확인 스프라이트
    public Sprite mapIndex;
    public Sprite mapClimateIndex;

    private Material material;

    private List<CountryPin> pinlist;

    private int index;
    private bool isHighLight;

    private void Start()
    {
        material = highlightObject.GetComponent<SpriteRenderer>().material;
        CreateCountryPin();
    }

    private void OnEnable()
    {
        TouchManager.Instance.OnTouch += Touch;
    }

    private void OnDisable()
    {
        TouchManager.Instance.OnTouch -= Touch;
    }

    public float highlightDuration = 0.1f; //하이라이트 될때까지 걸리는 시간
    private Vector2 beforeHighlightVec;
    private float beforeHighlightFo;



    public void CreateCountryPin()
    {
        pinlist = new List<CountryPin>();

        Country[] con = countryLoader.GetCountries();
        for (int i = 0; i < con.Length; i++)
        {
            Vector2 vec = con[i].center.ToVector2();

            GameObject prefab = Instantiate(pinPrefab, vec * 0.1f, quaternion.identity);

            CountryPin cityPin = prefab.GetComponent<CountryPin>();

            cityPin.text.text = con[i].name.ToString();
            cityPin.country = con[i];

            pinlist.Add(cityPin);
        }
    }

    /* public void CreateCountryPin()
    {
        pinlist = new List<CountryPin>();

        Country[] con = countryLoader.GetCountries();
        for (int i = 0; i < con.Length; i++)
        {
            if(con[i].countryIndex == 119)
            {
                foreach (var item in con[i].cities)
                {
                    Vector2 vec = item.coordinate.ToVector2();
                    GameObject prefab = Instantiate(pinPrefab, vec * 0.1f, quaternion.identity);
                    CountryPin cityPin = prefab.GetComponent<CountryPin>();
                    cityPin.text.text = con[i].name.ToString();
                    cityPin.country = con[i];

                    pinlist.Add(cityPin);
                }
            }
            

            

            
        }
    } */


    private void Touch()
    {
        if (isHighLight)
            EndHighlight();
        else
            Highlight();
    }
   

    #region HighLight

    private void Highlight()
    {
        Vector2 touchVec = TouchManager.Instance.lastPanPosition;

        index = GetColorFromTexture(mapIndex , GetVecFromRay(touchVec)).r;
        


        if (index != 0 && !countryLoader.IsExcludedCountry(index))
        {
            countryFlag.gameObject.SetActive(true);
            countryName.gameObject.SetActive(true);
            countryPop.gameObject.SetActive(true);



            highlightObject.SetActive(true);
            material.SetInt("_TargetIndex", index);

            for (int i = 0; i < pinlist.Count; i++)
            {
                if (index == pinlist[i].country.countryIndex)
                {
                    pinlist[i].gameObject.SetActive(true);
                    HighlightCam(pinlist[i].country);

                    countryFlag.sprite = pinlist[i].country.flag;
                    countryName.text = pinlist[i].country.name;
                    countryPop.text = $"pop : {pinlist[i].country.population}";

                }
                else
                    pinlist[i].gameObject.SetActive(false);
            }

            isHighLight = true;
        }


    }

    private void EndHighlight()
    {
        countryFlag.gameObject.SetActive(false);
        countryName.gameObject.SetActive(false);
        countryPop.gameObject.SetActive(false);
        highlightObject.SetActive(false);

        for (int i = 0; i < pinlist.Count; i++)
        {
            pinlist[i].gameObject.SetActive(true);
        }

        EndHighlightCam();

        isHighLight = false;
    }

    private void HighlightCam(Country country)
    {
        beforeHighlightVec = MapCameraController.Instance.camTransform.position;
        beforeHighlightFo = MapCameraController.Instance.mainCamera.fieldOfView;

        float a = Mathf.Max(Mathf.Abs(country.northernmost - country.southernmost), Mathf.Abs(country.westernmost - country.easternmost)) * 3;
        DOTween.To(() => MapCameraController.Instance.mainCamera.fieldOfView, x => MapCameraController.Instance.mainCamera.fieldOfView = x, Mathf.Clamp(10 * a, 6, 25), highlightDuration);
        MapCameraController.Instance.camTransform.DOMove(country.center.ToVector2() * 0.1f, highlightDuration).SetEase(Ease.OutQuad);

        //Debug.Log(MapCameraController.Instance.mainCamera.fieldOfView); 
    }

    private void EndHighlightCam()
    {
        DOTween.To(() => MapCameraController.Instance.mainCamera.fieldOfView, x =>
        MapCameraController.Instance.mainCamera.fieldOfView = x, beforeHighlightFo, highlightDuration);
        //MapCameraController.Instance.camTransform.DOMove(beforeHighlightVec , highlightDuration).SetEase(Ease.OutQuad);

        //Debug.Log(MapCameraController.Instance.mainCamera.fieldOfView); 
    }

    #region GetColorFromTexture(inGame)
    /* private Color32 GetColorFromTexture(string textureTag)
    {
        Vector2 touchVec = TouchManager.Instance.lastPanPosition;

        // 화면 좌표를 월드 좌표로 변환
        Ray ray = MapCameraController.Instance.mainCamera.ScreenPointToRay(touchVec);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Collider2D collider = hit.collider;

            // 태그가 "mapindex"인지 확인
            if (collider.CompareTag(textureTag))
            {
                SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Sprite sprite = spriteRenderer.sprite;
                    Texture2D texture = sprite.texture;

                    // 월드 좌표를 로컬 좌표로 변환 (카메라 변환 반영)
                    Vector2 localPoint = collider.transform.InverseTransformPoint(hit.point);
                    //Vector2 localPoint = hit.point;
                    //Debug.Log($"{hit.point} {localPoint}");

                    Debug.Log(hit.point + " " + localPoint);

                    // 스프라이트 UV 좌표 계산 (스케일 포함)
                    //Vector3 scale = collider.transform.lossyScale; // 글로벌 스케일 가져오기
                    Rect spriteRect = sprite.textureRect;
                    Vector2 pivot = sprite.pivot;
                    float ppu = sprite.pixelsPerUnit;

                    Vector2 texCoord = new Vector2(
                        (localPoint.x * ppu + pivot.x) / spriteRect.width,
                        (localPoint.y * ppu + pivot.y) / spriteRect.height
                    );



                    // 텍스처 픽셀 좌표 계산
                    int texX = Mathf.RoundToInt(spriteRect.x + texCoord.x * spriteRect.width);
                    int texY = Mathf.RoundToInt(spriteRect.y + texCoord.y * spriteRect.height);

                    // 텍스처 좌표가 유효한지 확인
                    if (texX >= 0 && texX < texture.width && texY >= 0 && texY < texture.height)
                    {
                        // 텍스처 색상 읽기
                        Color32 color = texture.GetPixel(texX, texY);
                        //Debug.Log($"Touched Color: {color}");

                        return color;
                    }
                    else
                    {
                        //Debug.Log("Touch is outside the sprite texture bounds.");
                    }
                }
            }
            else
            {
                //Debug.Log("Touched object does not have the 'mapindex' tag.");
            }
        }
        else
        {
            //Debug.Log("No object detected at touch position.");
        }
        
        Debug.Log($"cant get color from {textureTag}");
        return Color.blue;

    } */
    #endregion


    private Color32 GetColorFromTexture(Sprite sprite , Vector2 vec)
    {
        Texture2D texture = sprite.texture;

        float scale = texture.width / 3600f;
        Vector2 worldpoint = vec * scale;
        
        
        // 스프라이트 UV 좌표 계산 (스케일 포함)
        //Vector3 scale = collider.transform.lossyScale; // 글로벌 스케일 가져오기
        Rect spriteRect = sprite.textureRect;
        Vector2 pivot = sprite.pivot;
        float ppu = sprite.pixelsPerUnit;

        Vector2 texCoord = new Vector2(
            (worldpoint.x * ppu + pivot.x) / spriteRect.width,
            (worldpoint.y * ppu + pivot.y) / spriteRect.height
        );


        // 텍스처 픽셀 좌표 계산
        int texX = Mathf.RoundToInt(spriteRect.x + texCoord.x * spriteRect.width);
        int texY = Mathf.RoundToInt(spriteRect.y + texCoord.y * spriteRect.height);

        // 텍스처 좌표가 유효한지 확인
        if (texX >= 0 && texX < texture.width && texY >= 0 && texY < texture.height)
        {
            // 텍스처 색상 읽기
            Color32 color = texture.GetPixel(texX, texY);

            return color;
        }

        return Color.blue;
    }

    private Vector2 GetVecFromRay(Vector2 vec)
    {
        Ray ray = MapCameraController.Instance.mainCamera.ScreenPointToRay(vec);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        return hit.point;
    }
    
    #endregion 


    /// <summary>
    /// 도시 기후에 맞춰 스테이지 전투
    /// </summary>
    //Debug.Log(GetColorFromTexture(mapClimateIndex, GetVecFromRay(touchVec)) + " " + GetClimate(GetColorFromTexture(mapClimateIndex, GetVecFromRay(touchVec)).r));
    private Climate GetClimate(int index) => index switch
    {
        >= 1 and <= 3 => Climate.Tropical,
        >= 4 and <= 7 => Climate.Arid,
        >= 8 and <= 16 => Climate.Temperate,
        >= 17 and <= 28 => Climate.Cold,
        >= 29 and <= 30 => Climate.Polar,
        _ => Climate.Default
    };
    
    
}
