using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    public float positionFrequency;
    public Vector3 positionAmplitude;
    public float positionDuration;

    public float rotationFrequency;
    public Vector3 rotationAmplitude;
    public float rotationDuration;

    bool shaking = false;
    Vector3 originPos;
    Quaternion originRot;
    FollowCamera followCamComp;
    float px = 0;
    float py = 0;


    void Start()
    {
        followCamComp = GetComponent<FollowCamera>();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P) && !shaking)
    //    {
    //        ShakePos();
    //    }

    //    if (Input.GetKeyDown(KeyCode.R) && !shaking)
    //    {
    //        ShakeRot();
    //    }

    //}

    public void ShakePos()
    {
        if (!shaking)
        {
            StartCoroutine(ShakePosition(positionDuration, positionFrequency, positionAmplitude));
        }
    }

    public void ShakeRot()
    {
        if (!shaking)
        {
            StartCoroutine(ShakeRotation(rotationDuration, rotationFrequency, rotationAmplitude));
        }
    }


    // 포지션 흔들기
    // 지정된 시간 동안 일정한 범위 안에서 일정한 시간 간격으로 위치를 변경한다.
    // 필요 요소: 전체 시간, 진동 횟수, 진폭
    IEnumerator ShakePosition(float duration, float frequency, Vector3 amplitude)
    {
        float interval = 1.0f / frequency;
        //originPos = transform.position;
        FollowCamera.FollowType currentType = followCamComp.followType;

        shaking = true;
        followCamComp.followType = FollowCamera.FollowType.ShakingPositionType;
        int testNum = 0;

        for (float i = 0; i < duration; i += interval)
        {
            // 랜덤 함수를 이용한 랜덤 방식
            Vector2 randomPos = Random.insideUnitCircle;
            randomPos.x *= amplitude.x;
            randomPos.y *= amplitude.y;

            // originPos를 기준을 랜덤한 위치 값을 계산해서 그 쪽으로 위치를 변경한다.
            transform.position = followCamComp.target.position + new Vector3(randomPos.x, randomPos.y, 0);

            yield return new WaitForSeconds(interval);
            testNum++;
            print("반복 횟수: " + testNum.ToString());
            print("누적 시간: " + i.ToString());
        }

        shaking = false;
        //transform.position = originPos;
        followCamComp.followType = currentType;
    }

    // 로테이션 흔들기
    // 지정된 시간 동안 일정한 범위 안에서 일정한 시간 간격으로 회전 값을 변경한다.
    // 필요 요소: 전체 시간, 진동 횟수, 진폭
    IEnumerator ShakeRotation(float duration, float frequency, Vector3 amplitude)
    {
        float interval = 1.0f / frequency;
        //originRot = transform.rotation;
        //Vector3 eulerOrigin = transform.eulerAngles;
        FollowCamera.FollowType currentType = followCamComp.followType;

        shaking = true;
        followCamComp.followType = FollowCamera.FollowType.ShakingRotationType;

        for (float i = 0; i < duration; i += interval)
        {
            //Vector2 randomPos = Random.insideUnitCircle;

            // Perline Noise를 이용한 랜덤 방식
            px += 0.1f;
            py += 0.1f;
            if (px >= 1.0f)
            {
                px = 0;

                if (py >= 1.0f)
                {
                    py = 0;
                }
            }

            Vector2 randomPos = new Vector2(Mathf.PerlinNoise(px, 0) - 0.5f, Mathf.PerlinNoise(0, py) - 0.5f);

            randomPos.x *= amplitude.x;
            randomPos.y *= amplitude.y;

            // originPos를 기준을 랜덤한 회전 값을 계산해서 그 쪽으로 회전 값을 변경한다.
            transform.eulerAngles = followCamComp.target.eulerAngles + new Vector3(randomPos.x, randomPos.y, 0);

            yield return new WaitForSeconds(interval);
        }

        shaking = false;
        //transform.rotation = originRot;
        followCamComp.followType = currentType;
    }
}
