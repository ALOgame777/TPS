
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class PlayerMove : ActorBase
{
    enum PlayerMoveState
    {
        Normal,
        Jump
    }

    public PlayerStateBase mysuatus;
    public float rotSpeed = 200.0f;
    public float yVelocity = 2.0f;
    public float jumpPower = 4.0f;
    public int maxjumpcount = 1;
    public Image img_hitUI;
    public Animator playerAnim;

    // 회전 값을 미리 계산하기 위한 회전축(x, y) 변수
    float rotX;
    float rotY;
    float yPos;
    int currentJumpCount = 0;
    float[] idleAnims = new float[] { 0.0f, 0.3f, 0.6f, 1.0f};
    float currentTime = 0f;

    CharacterController cc;

    PlayerMoveState myMovestate = PlayerMoveState.Normal;
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

        // 2초에 한 번씩 idle 애니메이션을 변경하는 코루틴 함수를 실행한다.
        StartCoroutine(SelectIdleMotion(2.0f));
        
    }
    IEnumerator SelectIdleMotion(float changeTime)
    {
        while (true)
        {
            // 지정된 시간 마다 0 ~ 3 사이의 랜덤한 값을 뽑는다.
            int num = Random.Range(0, 4);
            // idleAnims 배열의 뽑은 인덱스의 해당하는 값을 애니메이터의 SelectedIdle 변수의 값으로 넣어준다.
            playerAnim.SetFloat("SelectedIdle", idleAnims[num]);
            yield return new WaitForSeconds(changeTime);
        }
    }

    void Update()
    {
        Move();
        Rotate();
        // 히트 ui 타이머
        //if (timerStart)
        //{
        //    currentTime -= Time.deltaTime;
        //    if (currentTime < 0)
        //    {
        //        timerStart = false;
        //        img_hitUI.gameObject.SetActive(false);
        //    }
        //}


    }
    #region Move 함수
    // "Horizontal" 과 "Vertical" 입력을 이용해서 수평면으로 이동하게 하고 싶다.
    // 1. 사용자의 입력을 받는다.
    // 2. 방향을 계산한다.
    // 3. 매 프레임마다 계산된 속도로 자신의 위치를 변경한다.
    #endregion
    void Move()
    {
        float h = 0;
        float v = 0;
        if (myMovestate == PlayerMoveState.Normal)
        {

            // 1. 수평 이동 계산
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            playerAnim.SetFloat("MoveHorizontal", h);
            playerAnim.SetFloat("MoveVertical", v);
        }
        // 나의 정면 방향에 따라서 이동하도록 변경한다.
        // 1-1. 로컬 방향 벡터를 이용해서 계산하는 방법
        // 나를 기준으로 한 forward 방향 , 나를 기준으로 한 right방향
        //Vector3 dir = transform.forward * v + transform.right * h;
        //dir.Normalize();

         // 1-2. 나의 회전 값에 따라서 월드 방향 벡터를 로컬 방향의 벡터로 변환하는 함수를 이용하는 방법
         Vector3 dir = new Vector3(h, 0, v); // 월드 방향 벡터
         playerAnim.SetFloat("DirLength", dir.magnitude);
         dir = transform.TransformDirection(dir); // 로컬 방향 -> 월드방향
         //transform.TransformPoint(dir); // 로컬 위치를 월드 위치로 환산
         dir.Normalize();
        
        // 2. 수직 이동 계산(중력)

        // 중력 적용
        yPos += gravityPower.y * yVelocity * Time.deltaTime;

        Ray ray = new Ray(transform.position, transform.up * -1);
        RaycastHit hitinfo;
        bool isHit = Physics.Raycast(ray, out hitinfo, (cc.height + cc.skinWidth + 0.4f) * 0.5f, ~(1 << 7));
        // 점프를 하지 않았는데 충돌이 없는 경우
        if (myMovestate != PlayerMoveState.Jump && !isHit)
        {
            // 낙하 애니메이션 신호를 준다.
            playerAnim.SetTrigger("IsFalling");
        }
        // 바닥에 닿아있을 때에는 yPos의 값을 0으로 초기화한다.
        //if(cc.collisionFlags == CollisionFlags.CollidedBelow)
        if(isHit)
        {
            // 점프 했다가 땅에 착지했을 때 1.25초 뒤에 이동할 수 있는 상태로 전환한다.
            if (myMovestate == PlayerMoveState.Jump)
            {
                playerAnim.SetBool("Jump", false);
                currentTime += Time.deltaTime;
                if(currentTime > 0.5f)
                {
                    currentTime = 0;
                    myMovestate = PlayerMoveState.Normal;
                }
            }
            yPos = 0;
            currentJumpCount = 0;
            
        }

        // 키보드의 스페이스바를 누르면 위쪽으로 점프를 하게 하고 싶다.
        if (Input.GetButtonDown("Jump") && currentJumpCount < maxjumpcount)
        {
            // 땅에서 처음 점프할 때 점프 상태로 전환한다.
            if(myMovestate == PlayerMoveState.Normal)
            {
                myMovestate = PlayerMoveState.Jump;
            }
            yPos = jumpPower;
            currentJumpCount++;
            playerAnim.SetBool("Jump", true);  
        }

        dir.y = yPos;

        //transform.position += dir * moveSpeed * Time.deltaTime;
        cc.Move(dir * mysuatus.speed * Time.deltaTime);
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

    // 데미지 받았을 때에 실행할 함수
    public override void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        base.TakeDamage(atkPower, hitDir, attacker);

        mysuatus.currentHP = Mathf.Clamp(mysuatus.currentHP - atkPower, 0 , mysuatus.maxHp);
        //print("내 체력 : " + mysuatus.currentHP);
        // img_hitUI 오브젝트를 활성화했다가 0.5초 뒤에 다시 비활성화한다.;
        StartCoroutine(TakeHit(0.5f));

    }

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{

    //}
    
    
    // 코루틴 함수
    IEnumerator TakeHit(float delayTime)
    {
        //float addValue = 0.05f;
        for (int i = 0; i < 100; i++)
        {
            Color colorVector = img_hitUI.color;
            float addValue = 0.05f;
            if (i > 49)
            {
                addValue *= -1;
            }
            colorVector.a += addValue;
            img_hitUI.color = colorVector;
            //yield return new WaitForSeconds(delayTime);
            yield return null;
        }
    }
}
