using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : SingletonMonobehaviour<MapCameraController>
{
    public Camera mainCamera; // 메인 카메라
    public Transform camTransform;
    public float swipeSpeed = 0.5f; // 스와이프 속도
    public float zoomSpeed = 3f; // 확대/축소 속도
    public float minZoom = 0.2f; // 최소 줌
    public float maxZoom = 20f; // 최대 줌

    

    private void Update()
    {
        HandleMouseSwipe();
        HandleMouseZoom();
    }

    private void HandleMouseSwipe()
    {
        float swipeSpeed = 0.5f * mainCamera.orthographicSize;


        // 드래그 중일 때
        if (TouchManager.Instance.isPanning)
        {
            Vector3 difference = TouchManager.Instance.lastPanPosition - Input.mousePosition;

            // 카메라 위치를 변경
            camTransform.Translate(difference.x * swipeSpeed * Time.deltaTime, difference.y * swipeSpeed * Time.deltaTime, 0);

            // 드래그 시작점 갱신
            TouchManager.Instance.lastPanPosition = Input.mousePosition;
            
        }
    }

    private void HandleMouseZoom()
    {
        float zoomSpeed = 1f * mainCamera.orthographicSize;

        // 마우스 휠로 확대/축소

        if (TouchManager.Instance.scroll != 0f)
        {
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - TouchManager.Instance.scroll * zoomSpeed, minZoom, maxZoom);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - TouchManager.Instance.scroll * zoomSpeed * 15f, minZoom, maxZoom); // 감도 올리기
            }
        }
    }
}
