
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.AI;



public class EnemyFSM : ActorBase
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
        MeleeAttack = 4,
        FarAttack = 8,
        AttackDelay = 16,
        Return = 32,
        Damaged = 64,
        Dead = 128,
        Cinematic,
    }

    public Enemystate myState = Enemystate.Idle;
    public bool drawGizmos = true;
    [Header("�⺻ �Ӽ�")]
    public EnemyInitPreferences initPreferences;
    public EnemyStateBase myStatus;
    public Slider hpSlider;
    public Animator enemyAnim;
    public float attackType = 0;

    float currentTime = 0;
    float idleTime = 3.0f;
    Vector3 patrolCenter;
    CharacterController cc;
    Vector3 patrolNext;
    Vector3 hitDirection;
    float[] idleBlendValue = new float[] { 0, 0.5f, 1.0f };
    int idleNumber = 0;
    NavMeshAgent smith;

    [SerializeField] Transform target;

    void Start()
    {
        myState = Enemystate.Cinematic;
        patrolCenter = transform.position;
        patrolNext = patrolCenter;
        cc = GetComponent<CharacterController>();
        myStatus.Initialize(100, 9);
        myStatus.patrolSpeed = 5;
        hpSlider.value = myStatus.currentHP / myStatus.maxHp;
        smith = GetComponent<NavMeshAgent>();
        if(smith != null)
        {
            smith.speed = myStatus.patrolSpeed;
            smith.angularSpeed = 300f;
            smith.acceleration = 35.0f;
            smith.autoBraking = true;
            smith.autoTraverseOffMeshLink = false;
            smith.stoppingDistance = 1.0f;

        }
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
            case Enemystate.MeleeAttack:
                //Attack();
                break;
            case Enemystate.FarAttack:

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
            case Enemystate.Cinematic:

                break;

        }
    }
    //Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float percent)
    //{
    //    Vector3 mid1 = Vector3.Lerp(p0, p1, percent);
    //    Vector3 mid2 = Vector3.Lerp(p1, p2, percent);
    //    Vector3 center = Vector3.Lerp(mid1, mid2, percent);
    //    return center;
    //}

    //private void ParabolaType2(float height)
    //{
    //    OffMeshLinkData linkData = smith.currentOffMeshLinkData;
    //    Vector3 start = linkData.startPos + Vector3.up;
    //    Vector3 end = linkData.endPos + Vector3.up;
    //    Vector3 middle = (start + end) / 2;
    //    middle.y = height;

    //    if(currentTime < 1.0f)
    //    {
    //        currentTime += Time.deltaTime;
    //        transform.position = BezierCurve(start, middle, end, currentTime);
    //    }
    //    else
    //    {
    //        currentTime = 0;
    //        smith.CompleteOffMeshLink();
    //        smith.ResetPath();

    //    }
    //}

    //// � �̵� ��� 
    //void ParabolaType3()
    //{
    //    OffMeshLinkData linkData = smith.currentOffMeshLinkData;
    //    Vector3 start = linkData.startPos + Vector3.up;
    //    Vector3 end = linkData.endPos + Vector3.up;
    //    if (currentTime < 1.0f)
    //    {
    //        currentTime += Time.deltaTime;
    //        Vector3 result = Vector3.Lerp(start, end, currentTime);
    //        result.y += heightCurve.Evaluate(currentTime);
    //        transform.position = result;
    //    }
    //    else
    //    {
    //        currentTime = 0;
    //        smith.CompleteOffMeshLink();
    //    }
    //}

    private void Idle()
    {
        CheckSight(initPreferences.sightRange, initPreferences.sightDistance);
        currentTime += Time.deltaTime;
        if (currentTime > idleTime)
        {
            currentTime = 0;
            myState = Enemystate.Patrol;
            print("my State : idle -> Patrol");
            enemyAnim.SetBool("PatrolStart", true);

            // idle �ִϸ��̼��� 0, 1, 2 �� ��ȯ�ؼ� ���õǵ��� �����Ѵ�.
            idleNumber = (idleNumber + 1) % 3;
            float selectedIdle = idleBlendValue[idleNumber];
            enemyAnim.SetFloat("SeletedIdle", selectedIdle);

            // �׺�޽� ������Ʈ�� ��������  PatrolNext ������ �����Ѵ�.
            smith.SetDestination(patrolNext);
            
        }
    }

    private void Patrol()
    {
        CheckSight(initPreferences.sightRange, initPreferences.sightDistance);
        // ���õ� �������� �̵��Ѵ�
        Vector3 dir = patrolNext - transform.position;
        //dir.y = 0;
        if (dir.magnitude > 1.0f)
        {

            //cc.Move(dir.normalized * myStatus.patrolSpeed * Time.deltaTime);
            // �̵��Ϸ��� �������� �ٶ󺻴�.
            
            //transform.rotation = Quaternion.LookRotation(dir.normalized);
        }
        // �������� �����ϰ�, 2�� ~ 3�� ���̸�ŭ ����� ���� �ٸ� ������ ��÷�Ѵ�.
        else
        {
            //PatrolRadius�� �ݰ����� �ϴ� ���� ������ ������ �����Ѵ�.
            // 1.
            #region 1. Random Ŭ������ �ִ� inside �Լ��� �̿��ؼ� �����ϴ� ���
            Vector2 randomPoint = Random.insideUnitCircle * initPreferences.PatrolRadius;
            patrolNext = patrolCenter + new Vector3(randomPoint.x, 0, randomPoint.y);
            #endregion

            #region 2. ���� �⺻ ���� ���
            //float h = UnityEngine.Random.Range(-1.0f, 1.0f);
            //float v = UnityEngine.Random.Range(-1.0f, 1.0f);
            //float distance = Random.Range(0, patrolRadius);
            //newPos =  new Vector3(h, 0, v).normalized * distance;
            

            #endregion
            #region 3. �ﰢ �Լ��� �̿��� ����
            //float degree = Random.Range(-180.0f, 180.0f);
            //Vector3 newPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 0, Mathf.Sin(Mathf.Deg2Rad * degree));
            //float distance = Random.Range(0, patrolRadius);
            //patrolNext = patrolCenter + newPos * distance;
            #endregion
            myState = Enemystate.Idle;
            print("my State : Patrol -> Idle");
            idleTime = Random.Range(2.0f, 3.0f);
            enemyAnim.SetBool("PatrolStart", false);

            // ������ �׺�޽� ������Ʈ�� �����.
            smith.isStopped = true;
            smith.ResetPath();
        }
    }


    void CheckSight(float degree, float maxDistance)
    {
        // �þ� ���� �ȿ� ���� ����� �ִٸ� �� ����� Ÿ������ �����ϰ� �ʹ�.
        // �þ� ���� ����(�þ߰� : �¿� 30��, ����, �ִ� �þ� �Ÿ� : 15����)
        // ��� ������ ���� �±�(Player) 
        target = null;

        // 1. ���� �ȿ� ��ġ�� ������Ʈ �߿� Tag�� "Player"�� ������Ʈ�� ��� ã�´�.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 2. ã�� ������Ʈ �߿��� �Ÿ��� maxDistance �̳��� ������Ʈ�� ã�´�.
        for (int i = 0; i < players.Length; i++)
        {
            float distance = Vector3.Distance(players[i].transform.position, transform.position);
            if (distance <= maxDistance)
            {
                // 3. ã�� ������Ʈ�� �ٶ󺸴� ���Ϳ� ���� ���� ���͸� �����Ѵ�.
                Vector3 lookVector = players[i].transform.position - transform.position;
                lookVector.Normalize();

                float cosTheta = Vector3.Dot(transform.forward, lookVector);
                float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
                

                // 4-1. ����, ������ ��� ���� 0���� ũ��(������ ���ʿ� �ִ�.)...
                // 4-2. ����, ���հ��� ���� 30���� ������(���� �¿� 30�� �̳�)...

                if (cosTheta > 0 && theta < degree)
                {
                    target = players[i].transform;

                    // ���¸� trace ���·� ��ȯ�Ѵ�.
                    myState = Enemystate.Trace;
                    print("my State : idle/Patrol -> Trace");
                    enemyAnim.SetTrigger("Trace");
                    attackType = Random.Range(0, 1.0f);
                }

            }
        }
    }
    private void TraceTarget()
    {
        // ����, �ִ� �߰� �Ÿ��� ����ٸ�....
        if (Vector3.Distance(transform.position, patrolCenter) > initPreferences.maxTraceDistance)
        {
            // ���¸� Return ���·� ��ȯ�Ѵ�.
            myState = Enemystate.Return;
            print("my State : Trace -> Return");
            enemyAnim.SetBool("Returning", true);
            smith.speed = myStatus.speed;
            smith.SetDestination(patrolCenter);
            return;
        }
        // ���� ������
        float selectedRange = attackType > 0.5f ? initPreferences.attackRange : initPreferences.farAttackRange;
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        if (dir.magnitude > selectedRange)
        {
            // Ÿ���� ���� �̵��Ѵ�.
            //cc.Move(dir.normalized * myStatus.speed * Time.deltaTime);
            //transform.rotation = Quaternion.LookRotation(dir.normalized);
            smith.SetDestination(target.position);
            smith.speed = myStatus.speed;
        }
        else
        {
            // ���� ���� �̳��� ���� ���¸� Attak ���·� ��ȯ�Ѵ�.
            currentTime = 0;
            if (dir.magnitude > initPreferences.attackRange)
            {
                myState = Enemystate.FarAttack;
                print("my State : Trace -> FarAttack");
                enemyAnim.SetTrigger("FarAttack");
            }
            else
            {
                myState = Enemystate.MeleeAttack;
                print("my State : Trace -> MeleeAttack");
                enemyAnim.SetTrigger("Attack");
            }

            smith.isStopped = true;
            smith.ResetPath();

        }
    }


    public void Attack()
    {
        // ������ �Ѵ�.
        target.GetComponent<PlayerMove>().TakeDamage(20, Vector3.zero, transform);
        #region �ִϸ��̼� ���� ������ ���� ���� ��Ȳ�� ��������
        // ���� �ִϸ��̼��� ������...
        // ���� �������� �ִϸ��̼� ���� ������ �����´�.
        //AnimatorStateInfo stateInfo = enemyAnim.GetCurrentAnimatorStateInfo(0);
        //// ���� ������ ���¸� �ؽõ����ͷ� ��ȯ�Ѵ�.
        //int attackHash =  Animator.StringToHash("Base Layer.AttackMelee");
        //// ����, ���� ���� ���� ���°� ���� �����̶��...
        //if (stateInfo.fullPathHash == attackHash)
        //{
        //    //print("Length : " + stateInfo.length);
        //    //print("Progress : " + stateInfo.normalizedTime);
        //    if(stateInfo.normalizedTime > 1.0f)
        //    {
        //        // ���� ��� ���·� ��ȯ�Ѵ�.

        //        myState = Enemystate.AttackDelay;
        //        print("my State : Attack -> AttackDelay");
        //    }
        //}
        #endregion


    }

    private void AttackDelay()
    {
        // ����, Ÿ�ٰ� �Ÿ��� ���� ������ ������ ����ٸ�...
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > initPreferences.farAttackRange)
        {
            // �ٽ� �߰� ���·� ��ȯ�Ѵ�.
            myState = Enemystate.Trace;
            print("my State : AttackDelay -> Trace");
            enemyAnim.SetTrigger("Trace");
            currentTime = 0;
            return;
        }
        else if(dist > initPreferences.attackRange)
        {
            // �����ð� ����Ѵ�.
            currentTime += Time.deltaTime;
            // �����ð��� �����ٸ� ���¸� ���Ÿ� ���� ���·� ��ȯ�Ѵ�.
            if (currentTime > 1.5f)
            {
                currentTime = 0;
                myState = Enemystate.FarAttack;
                print("my State : AttackDelay -> FarAttack");
                enemyAnim.SetTrigger("FarAttack");
            }
        }
        else
        {
            // �����ð� ����Ѵ�.
            currentTime += Time.deltaTime;
            // �����ð��� �����ٸ� ���¸� �ٰŸ� ���� ���·� ��ȯ�Ѵ�.
            if (currentTime > 1.5f)
            {
                currentTime = 0;
                myState = Enemystate.MeleeAttack;
                print("my State : AttackDelay -> MeleeAttack");
                enemyAnim.SetTrigger("Attack");
            }
            
        }
    }

    private void ReturnHome()
    {
        // traceCenter ��ġ�� �ٽ� ���ư���.
        Vector3 dir = patrolCenter - transform.position;
        dir.y = 0;
        // �������� �����ߴٸ�...
        if (dir.magnitude < 1.1f)
        {
            // �׺���̼��� �����Ѵ�.
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = patrolCenter;
            // ���¸� idle ���·� ��ȯ�Ѵ�.
            myState = Enemystate.Idle;
            print("my State : Return -> Idle");
            enemyAnim.SetBool("Returning", false);
            smith.speed = myStatus.patrolSpeed;
        }
        // �׷��� �ʾҴٸ�
        //else
        //{
        //    cc.Move(dir.normalized * myStatus.speed* Time.deltaTime);
        //    transform.rotation = Quaternion.LookRotation(dir.normalized);
        //}

    }


    private void Damaged()
    {
        // ���� �ð� �ڷ� �������ٰ�(kcock-back) ���¸� Trace ���·� ��ȯ�Ѵ�.
        transform.position = Vector3.Lerp(transform.position, hitDirection, 0.05f);

        if (Vector3.Distance(transform.position, hitDirection) < 0.1f)
        {
            myState = Enemystate.Trace;
            print("my State : Damaged -> Trace");
            //enemyAnim.SetTrigger("Trace");
            enemyAnim.SetBool("Hit", false);
            smith.speed = myStatus.speed;
        }
    }

    // ������ ������ �������� �ο��ϴ� �Լ�
    public override void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        // �θ� ������ TakeDamage �Լ��� ���� �����Ѵ�.
        base.TakeDamage(atkPower, hitDir, attacker);

        if (myState == Enemystate.Dead || myState == Enemystate.Return || myState == Enemystate.Damaged)
        {
            return;
        }


        // 1. ���� ü�¿��� ����� ���ݷ¸�ŭ�� ���ҽ�Ų��.(min 0 ~ max 100)
        myStatus.currentHP = Mathf.Clamp(myStatus.currentHP - atkPower, 0, myStatus.maxHp);

        // 1-2. ü�� �����̴� UI�� ���� ü���� ǥ���Ѵ�.
        hpSlider.value = myStatus.currentHP / myStatus.maxHp;
        // 2. ����, �� ��� ���� ü���� 0���� ���϶��...
        if (myStatus.currentHP <= 0)
        {
            // 2-1. ���¸� ���� ���·� ��ȯ�Ѵ�.
            myState = Enemystate.Dead;
            print("my State : Any -> Dead");
            enemyAnim.SetTrigger("Die");
            currentTime = 0;
            // 2-2. �ݶ��̴� ������Ʈ�� ��Ȱ��ȭ ó���Ѵ�.
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
        }
        // 3. �׷��� �ʴٸ�.. 
        else
        {
            // 3-1. ������ ���·� ��ȯ�Ѵ�.
            myState = Enemystate.Damaged;
            print("my State : Any -> Damaged");
            enemyAnim.SetBool("Hit", true);

            //3-2. Ÿ�� �������� ���� �Ÿ���ŭ�� �˹� ��ġ�� �����Ѵ�.
            hitDir.y = 0;
            hitDirection = transform.position + hitDir * 2.5f;

            // ����, ���� hitDirection ���̿� ��ֹ��� �ִٸ�...
            Ray ray = new Ray(transform.position, hitDir);
            RaycastHit hitinfo;
            if(Physics.Raycast(ray, out hitinfo, 2.5f))
            {
                // hitDirection �� ��ġ�� ���̿� �ε��� ��ġ�κ��� �ڽ��� ������ ��ŭ �������� �����Ѵ�.
                hitDirection = hitinfo.point + hitDir * -1.1f * GetComponent<CapsuleCollider>().radius; 
            }
            smith.isStopped = true;
            smith.ResetPath();
            //3-3. �����ڸ� Ÿ������ �����Ѵ�.
            target = attacker;
        }

    }

    private void Dead()
    {
        // Ŭ���� ������ �Ͻÿ� �ʱ�ȭ�ϱ�
        

        // 4�� �ڿ� ���ŵȴ�.
        currentTime += Time.deltaTime;
        if (currentTime > 4.0f)
        {
            Destroy(gameObject);
        }
    }

    // �� �׸���
    private void OnDrawGizmos()
    {
        if(!drawGizmos)
        {
            return;
        }    
        Gizmos.color = new Color32(154, 14, 235, 255);

        #region ���׸���
        //List<Vector3> points = new List<Vector3>();
        //for (int i = 0; i < 360; i += 5)
        //{
        //    Vector3 point = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad)) * 5;
        //    points.Add(transform.position + point);
        //}

        //for (int i = 0; i < points.Count - 1; i++)
        //{
        //    Gizmos.DrawLine(points[i], points[i + 1]);
        //}
        #endregion

        // �þ߰� �׸���
        float rightDegree = 90 - initPreferences.sightRange;
        float leftDegree = 90 + initPreferences.sightRange;
        Vector3 rightPos = (transform.right * Mathf.Cos(rightDegree * Mathf.Deg2Rad) + transform.forward * Mathf.Sin(rightDegree * Mathf.Deg2Rad)) * initPreferences.sightDistance + transform.position;
        Vector3 leftPos = (transform.right *  Mathf.Cos(leftDegree * Mathf.Deg2Rad)+ transform.forward * Mathf.Sin(leftDegree * Mathf.Deg2Rad)) * initPreferences.sightDistance + transform.position;
        Gizmos.DrawLine(transform.position, rightPos);
        Gizmos.DrawLine(transform.position, leftPos);

        // �ִ� �߰� �Ÿ� �׸���
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolCenter, initPreferences.maxTraceDistance);
    }
}

//����ȭ Ŭ����
[System.Serializable]
public class EnemyInitPreferences
{
    public float PatrolRadius = 4.0f;
    public float attackRange = 1.5f;
    public float farAttackRange = 4.0f;
    [Range(0.0f, 90.0f)]
    public float sightRange = 30.0f;
    public float sightDistance = 15.0f;
    public float maxTraceDistance = 25.0f;

    //������ �Լ�
    public EnemyInitPreferences(float patrolRadius, float attackRange, float sightRange, float sightDistance, float maxTraceDistance)
    {
        this.PatrolRadius = patrolRadius;
        this.attackRange = attackRange;
        this.sightRange = Mathf.Clamp(sightRange, 0, 90.0f);
        this.sightDistance = sightDistance;
        this.maxTraceDistance = maxTraceDistance;
    }
}