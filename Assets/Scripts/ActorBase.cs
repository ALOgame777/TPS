using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // summary 표시가 안되면 vs 를 껐다 켜면 됨
    /// <summary>
    /// 데미지를 입힐 때 사용하는 함수
    /// </summary>
    /// <param name="atkPower">실제로 줄 데미지 값</param>
    /// <param name="hitDir">넉백을 시킬 방향 벡터</param>
    /// <param name="attacker">공격자의 트랜스폼 컵포넌트</param>
    public virtual void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {

    }
}
