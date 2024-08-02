using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionEvents : MonoBehaviour
{

    EnemyFSM enemyState;

    private void Start()
    {
        //부모 오브젝트에서 EnemyFSM 컴포넌트를 가져온다.
        enemyState = transform.parent.GetComponent<EnemyFSM>();
    }
    // 나의 상태를 AttackDelay 상태로 변환한다.
    public void ChangeToAttackDelay()
    {
        enemyState.myState = EnemyFSM.Enemystate.AttackDelay;
        print("My State : Attack -> AttackDelay");
        enemyState.attackType = Random.Range(0, 1.0f);
    }


    // 타격의 순간 데미지를 부여한다.
    public void AttackHit()
    {
        enemyState.Attack();
    }
}
