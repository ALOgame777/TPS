using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed = 7.0f;
    public float rotSpeed = 200.0f;
    public float yVelocity = 2.0f;
    public float jumpPower = 4.0f;
    public int maxjumpcount = 1;
    

    // 회전 값을 미리 계산하기 위한 회전축(x, y) 변수
    float rotX;
    float rotY;
    float yPos;
    int currentJumpCount = 0;

    CharacterController cc;

    // 중력을 적용하고 싶다.
    // 바닥에 충돌이 있을 때까지는 아래로 계속 내려가게 하고 싶다.
    // 방향 : 아래, 크기 : 중력
    Vector3 gravityPower;

    void Start()
    {
        // 최초의 회전 상태대로 시작을 하고 싶다.(전역 변수 초기화)
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;
        // 캐릭터 컨트롤러 컴포넌트를 변수에 담아놓는다.
        cc = GetComponent<CharacterController>();

        // 중력 값을 초기화한다.
        gravityPower = Physics.gravity;
       
        
    }

    void Update()
    {
        Move();
        Rotate();


    }
    #region Move 함수
    // "Horizontal" 과 "Vertical" 입력을 이용해서 수평면으로 이동하게 하고 싶다.
    // 1. 사용자의 입력을 받는다.
    // 2. 방향을 계산한다.
    // 3. 매 프레임마다 계산된 속도로 자신의 위치를 변경한다.
    #endregion
    void Move()
    {
        // 1. 수평 이동
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        // 나의 정면 방향에 따라서 이동하도록 변경한다.
        // 1-1. 로컬 방향 벡터를 이용해서 계산하는 방법
        // 나를 기준으로 한 forward 방향 , 나를 기준으로 한 right방향
        //Vector3 dir = transform.forward * v + transform.right * h;
        //dir.Normalize();

        // 1-2. 나의 회전 값에 따라서 월드 방향 벡터를 로컬 방향의 벡터로 변환하는 함수를 이용하는 방법
        Vector3 dir = new Vector3(h, 0, v); // 월드 방향 벡터
        dir = transform.TransformDirection(dir); // 로컬 방향 -> 월드방향
        //transform.TransformPoint(dir); // 로컬 위치를 월드 위치로 환산
        dir.Normalize();

        // 2. 수직 이동 계산(중력)

        // 중력 적용
        yPos += gravityPower.y * yVelocity * Time.deltaTime;

        // 바닥에 닿아있을 때에는 yPos의 값을 0으로 초기화한다.
        if(cc.collisionFlags == CollisionFlags.CollidedBelow)
        {
            yPos = 0;
            currentJumpCount = 0;
        }

        // 키보드의 스페이스바를 누르면 위쪽으로 점프를 하게 하고 싶다.
        if (Input.GetButtonDown("Jump") && currentJumpCount < maxjumpcount)
        {
            yPos = jumpPower;
            currentJumpCount++;
        }

        dir.y = yPos;

        //transform.position += dir * moveSpeed * Time.deltaTime;
        cc.Move(dir * moveSpeed * Time.deltaTime);
        //cc.SimpleMove(dir * moveSpeed);
    }
    #region  마우스 이동으로 회전값을 변경
    // 사용자의 마우스 드래그 방향에 따라서 사용자의 상하좌우 회전이 되게 하고 싶다.
    // 1. 사용자의 마우스 드래그 입력을 받는다.Mouse X [마우스 x 축의 위치 델타], Mouse Y = Delta(변화량) 값이다.
    // 2. 회전속력, 회전 방향이 필요하다.
    // 회전의 Lock이 x축에 있음 강제로 Lock 통과해서 생기는 문제가 발생 > 반대로 회전을 함
    // 엔진 내부에서 카메라 문제 랜더링 계산이 반대되는 상황을 막기 위해
    // 3. 매 프레임마다 계산된 속도로 자신의 회전값을 변경한다.
    #endregion
    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 각 축 별로 회전 값을 미리 계산한다.(R = r0 +vt)
        rotX += mouseY * rotSpeed * Time.deltaTime;
        rotY += mouseX * rotSpeed * Time.deltaTime;

        // 상하 회전은 -60도 ~ +60도 까지로 제한한다.
        if (rotX > 60.0f)
        {
            rotX = 60.0f;
        }
        else if (rotX < -60.0f)
        {
            rotX = -60.0f;
        }
        // 계산된 회전 값을 나의 트랜스폼 회전 값으로 적용한다.
        transform.eulerAngles = new Vector3(0, rotY, 0);
        Camera.main.transform.GetComponent<FollowCamera>().rotX = rotX;
    }

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
        
    //}


}
