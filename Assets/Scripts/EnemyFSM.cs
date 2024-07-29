using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;




public class EnemyFSM : MonoBehaviour
{
    // 대기상태 idle  -> 패트롤 상태 patrol 왔다갔다 하는 기능 시야각 범위 안에 플레이어 감지가 되면 추격상태(Trace)에 바꿈
    // 추격을 하고 공격상태 Attak 공격 - 공격대기를 왔다갔다 함 만약 공격중인데 공격범위 밖으로 벗어나면 추격상태로
    // 복귀 상태(Return) 추격중일때 추격 범위 밖으로 벗어나면 복귀상태 
    // 초기 위치로 돌아왔을때 idle로 전환
    public enum Enemystate
    {
        Idle = 0,
        Patrol = 1,
        Trace = 2,
        Attack = 4,
        AttackDelay = 8,
        Return = 16,
        Damaged = 32,
        Dead = 64
    }

    public Enemystate myState = Enemystate.Idle;
    public float PatrolRadius = 4.0f;
    public float patrolSpeed = 5.0f;
    public float traceSpeed = 9.0f;
    public float attackRange = 2.0f;

    float currentTime = 0;
    float idleTime = 3.0f;
    Vector3 patrolCenter;
    CharacterController cc;
    Vector3 patrolNext;
    
    [SerializeField]Transform target;

    void Start()
    {
        patrolCenter = transform.position;
        patrolNext = patrolCenter;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 나의 현재 상태에 따라 각각의 함수를 실행한다.
        switch (myState)
        {
            case Enemystate.Idle:
                Idle();
                break;
            case Enemystate.Patrol:
                Patrol();
                break;
            case Enemystate.Trace:
                TraceTarget();
                break;
            case Enemystate.Attack:
                Attack();
                break;
            case Enemystate.AttackDelay:
                AttackDelay();
                break;
            case Enemystate.Return:
                ReturnHome();
                break;
            case Enemystate.Damaged:
                Damaged();
                break;
            case Enemystate.Dead:
                Dead();
                break;

        }
    }

    private void Idle()
    {
        currentTime += Time.deltaTime;
        if (currentTime > idleTime)
        {
            currentTime = 0;
            myState = Enemystate.Patrol;
            print("my State : idle -> Patrol");
        }
    }

    private void Patrol()
    {
        // 선택된 지점으로 이동한다
        Vector3 dir = patrolNext - transform.position;
        if (dir.magnitude > 0.1f)
        {
            cc.Move(dir.normalized * patrolSpeed * Time.deltaTime);
        }
        // 목적지에 도달하고, 2초 ~ 3초 사이만큼 대기한 다음 다른 지점을 추첨한다.
        else
        {
            //PatrolRadius를 반경으로 하는 원의 임의의 지점을 선택한다.
            // 1.
            #region 1. Random 클래스에 있는 inside 함수를 이용해서 연산하는 방식
            Vector2 randomPoint = Random.insideUnitCircle * PatrolRadius;
            patrolNext = patrolCenter + new Vector3(randomPoint.x, 0, randomPoint.y);
            #endregion

            #region 2. 벡터 기본 연산 방식
            //float h = UnityEngine.Random.Range(-1.0f, 1.0f);
            //float v = UnityEngine.Random.Range(-1.0f, 1.0f);
            //patrolNext = patrolCenter + new Vector3(h, 0, v).normalized * PatrolRadius;
            #endregion
            #region 3. 삼각 함수를 이용한 계산식
            //float degree = Random.Range(-180.0f, 180.0f);
            //Vector3 newPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 0, Mathf.Sin(Mathf.Deg2Rad * degree));
            //float distance = Random.Range(0, patrolRadius);
            //patrolNext = patrolCenter + newPos * distance;
            #endregion
            myState = Enemystate.Idle;
            idleTime = Random.Range(2.0f, 3.0f);
        }
    }
    private void TraceTarget()
    {
        Vector3 dir = target.position - transform.position;
        if (dir.magnitude > attackRange)
        {
            // 타겟을 향해 이동한다.
            cc.Move(dir.normalized * traceSpeed * Time.deltaTime);
        }
        else
        {
            // 공격 범위 이내로 들어가면 상태를 Attak 상태로 전환한다.
            myState = Enemystate.Attack;

        }
    }

    private void Attack()
    {
        print("공격");
    }

    private void AttackDelay()
    {
    }

    private void ReturnHome()
    {
    }

    private void Damaged()
    {
    }

    private void Dead()
    {
        
    }

    // 원 그리기
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color32(154, 14, 235, 255);

    //    List<Vector3> points = new List<Vector3>();
    //    for(int i = 0; i < 360; i += 5)
    //    {
    //        Vector3 point = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0,  Mathf.Sin(i * Mathf.Deg2Rad)) * 5;
    //        points.Add(transform.position + point);
    //    }

    //    for(int i = 0; i < points.Count -1; i++)
    //    {
    //        Gizmos.DrawLine(points[i], points[i + 1]);
    //    }
    //}
}
