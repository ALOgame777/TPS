using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Transform player;

    void Start()
    {
        //player = GameObject.Find("Player").transform;

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
}
