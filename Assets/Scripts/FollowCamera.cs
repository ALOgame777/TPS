using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowCamera : MonoBehaviour
{
    // 카메라의 추적 타입 열거형 상수
    public enum FollowType
    {
        SoftType,
        HardType,
        ShakingPositionType,
        ShakingRotationType,
    }

    public FollowType followType = FollowType.HardType;

    // 타겟으로 지정된 위치로 나를 이동시킨다.
    public Transform target;
    public float followSpeed = 3.0f;
    public Image zoomUI;
    
    
    Coroutine zoom;
    float rate;
    float currentFOV;
    float currentAlpha;

    CameraController camCon;
    void Start()
    {
        //player = GameObject.Find("Player").transform;
        camCon = FindObjectOfType<CameraController>();
    }

    void Update()
    {

        if (target != null)
        {
            // 1. 위치 설정
            // 카메라의 위치를 타겟 트랜스폼의 위치로 지정한다.
            if (followType == FollowType.HardType || followType == FollowType.ShakingRotationType)
            {
                transform.position = target.position;
            }
            else if (followType == FollowType.SoftType)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            }
            else if (followType == FollowType.ShakingPositionType)
            {

            }


            // 2. 회전 설정
            if (followType != FollowType.ShakingRotationType)
            {
                transform.rotation = target.rotation;
            }


            // 카메라의 정면 방향을 플레이어의 정면 방향으로 설정한다.
            //Vector3 dir = (player.position - transform.position).normalized;
            //transform.forward = player.forward;

            // 사용자의 마우스 상하 회전 값을 x축 회전으로 넣는다.
            //transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }


    public void ZoomIn(bool isIn)
    {
        if (zoom == null)
        {
            zoom = StartCoroutine(ZoomInCoruotine(isIn));
        }
        else
        {
            StopCoroutine(zoom);
            StartCoroutine(ZoomInCoruotine(isIn));
        }
    }

    IEnumerator ZoomInCoruotine(bool isIn)
    {
        float currentTime = 0;
        if (isIn)
        {
            rate = camCon.currentRate;
        }
        currentFOV = Camera.main.fieldOfView;
        currentAlpha = zoomUI.color.a;
        while (currentTime < 0.5f)
        {
            // 다른 모든 업데이트 함수가 끝날때까지 기다린다.
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
            // 시야각을 30도로 축소
            float startFOV = currentFOV;
            float endFOV = isIn ? 20: 60;
            Camera.main.fieldOfView = Mathf.Lerp(startFOV, endFOV, currentTime * 2);

            // 줌 UI 색상의 투명도를 1로 변경한다.
            float zoomRate = isIn ? currentTime : currentAlpha * 0.5f - currentTime;
            Color zoomAlpha = new Color(zoomUI.color.r, zoomUI.color.g, zoomUI.color.b, zoomRate * 2);
            zoomUI.color = zoomAlpha;

            //1인칭 상태로 바꾼다.
            camCon.currentRate = Mathf.Lerp(rate, 0, zoomRate * 2);
        }
        zoom = null;
    }

}
