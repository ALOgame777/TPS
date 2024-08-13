using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 특정 컴포넌트가 필요하다면 추가 RequireComponent(typeof())]
// Attribute로 필요 컴포넌트 자동 추가하기 []
[RequireComponent(typeof(Rigidbody))]
public class GrenadeExplosion : MonoBehaviour
{
    [Header("폭발 반경")]
    [Range(0.0f, 10.0f)]
    public float radius = 5.0f;
    public float damagePower = 50.0f;
    public Transform master;
    
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 만일, 바닥에 닿았다면....
    private void OnCollisionEnter(Collision collision)
    {
        // 현재 나의 위치를 기준으로 반경 radius 범위 안의 모든 게임 오브젝트를 찾는다.
        // Enemy layer = 6
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, 1 << 6);
        // 만일, 찾아진 것이 있다면
        if (cols.Length > 0)
        {
            // 찾은 오브젝트를 모두 파괴한다.
            for (int i = 0; i < cols.Length; i++)
            {
                //Destroy(cols[i].gameObject);

                Rigidbody rb = cols[i].gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(1000, transform.position, radius, 300);
                }
                // 만일, 찾은 오브젝트에 enemyFsm 컴포넌트가 있다면...
                EnemyFSM enemy = cols[i].transform.GetComponent<EnemyFSM>();
                if (enemy != null)
                {
                    // enemyFsm 컴포넌트에 있는 takeDamage 함수를 실행한다.
                    Vector3 hitDir = cols[i].transform.position - transform.position;
                    hitDir.Normalize();
                    enemy.TakeDamage(damagePower, hitDir, master);
                }
            }
        }
        Destroy(gameObject);
    }
}
