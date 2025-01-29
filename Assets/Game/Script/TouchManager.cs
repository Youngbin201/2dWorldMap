using System;
using UnityEngine;

public class TouchManager : SingletonMonobehaviour<TouchManager>
{
    

    private float swipeThreshold = 10f; // 스와이프 거리 임계값 (픽셀 단위)
    public Vector3 lastPanPosition; // 마지막 터치 위치
    public Vector3 panPosition; // 마지막 터치 위치
    public int panFingerId; // 팬 조작에 사용된 손가락 ID
    public bool isPanning = false; // 팬 여부 체크
    private bool isTouching = false; // 터치 여부 체크
    public float scroll;

    public event Action OnTouch;

    public void CallTouchEvent()
    {
        OnTouch?.Invoke();
    }

    private void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput(); // PC 입력 처리
        #endif

        #if UNITY_IOS || UNITY_ANDROID
        HandleTouchInput(); // 모바일 입력 처리
        #endif
    }

    void HandleMouseInput()
    {
        // 마우스 휠 줌
        scroll = Input.GetAxis("Mouse ScrollWheel");

        // 마우스 드래그로 스와이프
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
            isPanning = true;
            isTouching = true;
        }
        else if (Input.GetMouseButton(0) && isPanning)
        {
            panPosition = Input.mousePosition;
            if (Vector3.Distance(lastPanPosition, Input.mousePosition) > swipeThreshold)
            {
                isTouching = false; // 스와이프로 인식되면 터치 해제
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isTouching)
            {
                CallTouchEvent();
            }
            isPanning = false;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1) // 스와이프 또는 터치
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && touch.fingerId == panFingerId)
            {
                panPosition = touch.position;

                if (Vector2.Distance(lastPanPosition, touch.position) > swipeThreshold)
                {
                    isTouching = false; // 스와이프로 인식되면 터치 해제
                }
            }
            else if (touch.phase == TouchPhase.Ended && isTouching)
            {
                CallTouchEvent();
            }
        }
        else if (Input.touchCount == 2) // 핀치 줌
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 이전 프레임의 터치 간 거리
            float prevDistance = (touch1.position - touch1.deltaPosition - touch2.position + touch2.deltaPosition).magnitude;

            // 현재 터치 간 거리
            float currentDistance = (touch1.position - touch2.position).magnitude;

            // 거리 차이를 기반으로 줌 인/아웃
            scroll = (currentDistance - prevDistance) * Time.deltaTime;

        }
    }

    
}
