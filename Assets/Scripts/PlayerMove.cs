using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : ActorBase
{
    enum PlayerMoveState
    {
        Normal,
        Jump,
    }

    public PlayerStateBase myStatus;
    public float rotSpeed = 200.0f;
    public float yVelocity = 2;
    public float jumpPower = 4;
    public int maxJumpCount = 1;
    public Image img_hitUI;
    public Animator playerAnim;

    float rotX;
    float rotY;
    float yPos;
    int currentJumpCount = 0;
    float currentTime = 0;
    bool timerStart = false;
    float[] idleAnims = new float[4] { 0.0f, 0.3f, 0.6f, 1.0f };

    CharacterController cc;
    PlayerMoveState myMoveState = PlayerMoveState.Normal;

    Vector3 gravityPower;

    void Start()
    {
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;

        cc = GetComponent<CharacterController>();

        gravityPower = Physics.gravity;

        StartCoroutine(SelectIdleMotion(2.0f));
    }

    IEnumerator SelectIdleMotion(float changeTime)
    {
        while (true)
        {
            int num = Random.Range(0, 4);
            playerAnim.SetFloat("SelectedIdle", idleAnims[num]);

            yield return new WaitForSeconds(changeTime);
        }
    }

    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        float h = 0;
        float v = 0;

        if (myMoveState == PlayerMoveState.Normal)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            playerAnim.SetFloat("MoveHorizontal", h);
            playerAnim.SetFloat("MoveVertical", v);
        }

        Vector3 dir = new Vector3(h, 0, v);
        playerAnim.SetFloat("DirLength", dir.magnitude);
        dir = transform.TransformDirection(dir);
        dir.Normalize();

        yPos += gravityPower.y * yVelocity * Time.deltaTime;

        Ray ray = new Ray(transform.position, transform.up * -1);
        RaycastHit hitInfo;
        bool isHit = Physics.Raycast(ray, out hitInfo, (cc.height + cc.skinWidth + 0.4f) * 0.5f, ~(1 << 7));

        if (myMoveState != PlayerMoveState.Jump && !isHit)
        {
            playerAnim.SetTrigger("IsFalling");
        }

        if (isHit)
        {
            if (myMoveState == PlayerMoveState.Jump)
            {
                playerAnim.SetBool("Jump", false);
                currentTime += Time.deltaTime;
                if (currentTime > 0.3f)
                {
                    currentTime = 0;
                    myMoveState = PlayerMoveState.Normal;
                }
            }

            yPos = 0;
            currentJumpCount = 0;
        }

        if (Input.GetButtonDown("Jump") && currentJumpCount < maxJumpCount)
        {
            if (myMoveState == PlayerMoveState.Normal)
            {
                myMoveState = PlayerMoveState.Jump;
            }

            yPos = jumpPower;
            currentJumpCount++;
            playerAnim.SetBool("Jump", true);
        }

        dir.y = yPos;

        cc.Move(dir * myStatus.speed * Time.deltaTime);
    }

    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");

        rotY += mouseX * rotSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, rotY, 0);
    }

    public override void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        base.TakeDamage(atkPower, hitDir, attacker);

        myStatus.currentHP = Mathf.Clamp(myStatus.currentHP - atkPower, 0, myStatus.maxHp);

        Camera.main.GetComponent<ShakeObject>().ShakeRot();

        StartCoroutine(DeActivateHitUI(0.5f));
    }

    IEnumerator DeActivateHitUI(float delayTime)
    {
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
            yield return null;
        }
    }
}
