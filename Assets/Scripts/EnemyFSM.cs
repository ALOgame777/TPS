using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;




public class EnemyFSM : MonoBehaviour
{
    // ������ idle  -> ��Ʈ�� ���� patrol �Դٰ��� �ϴ� ��� �þ߰� ���� �ȿ� �÷��̾� ������ �Ǹ� �߰ݻ���(Trace)�� �ٲ�
    // �߰��� �ϰ� ���ݻ��� Attak ���� - ���ݴ�⸦ �Դٰ��� �� ���� �������ε� ���ݹ��� ������ ����� �߰ݻ��·�
    // ���� ����(Return) �߰����϶� �߰� ���� ������ ����� ���ͻ��� 
    // �ʱ� ��ġ�� ���ƿ����� idle�� ��ȯ
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
        // ���� ���� ���¿� ���� ������ �Լ��� �����Ѵ�.
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
        // ���õ� �������� �̵��Ѵ�
        Vector3 dir = patrolNext - transform.position;
        if (dir.magnitude > 0.1f)
        {
            cc.Move(dir.normalized * patrolSpeed * Time.deltaTime);
        }
        // �������� �����ϰ�, 2�� ~ 3�� ���̸�ŭ ����� ���� �ٸ� ������ ��÷�Ѵ�.
        else
        {
            //PatrolRadius�� �ݰ����� �ϴ� ���� ������ ������ �����Ѵ�.
            // 1.
            #region 1. Random Ŭ������ �ִ� inside �Լ��� �̿��ؼ� �����ϴ� ���
            Vector2 randomPoint = Random.insideUnitCircle * PatrolRadius;
            patrolNext = patrolCenter + new Vector3(randomPoint.x, 0, randomPoint.y);
            #endregion

            #region 2. ���� �⺻ ���� ���
            //float h = UnityEngine.Random.Range(-1.0f, 1.0f);
            //float v = UnityEngine.Random.Range(-1.0f, 1.0f);
            //patrolNext = patrolCenter + new Vector3(h, 0, v).normalized * PatrolRadius;
            #endregion
            #region 3. �ﰢ �Լ��� �̿��� ����
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
            // Ÿ���� ���� �̵��Ѵ�.
            cc.Move(dir.normalized * traceSpeed * Time.deltaTime);
        }
        else
        {
            // ���� ���� �̳��� ���� ���¸� Attak ���·� ��ȯ�Ѵ�.
            myState = Enemystate.Attack;

        }
    }

    private void Attack()
    {
        print("����");
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

    // �� �׸���
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
