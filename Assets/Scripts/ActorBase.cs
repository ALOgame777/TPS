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

    // summary ǥ�ð� �ȵǸ� vs �� ���� �Ѹ� ��
    /// <summary>
    /// �������� ���� �� ����ϴ� �Լ�
    /// </summary>
    /// <param name="atkPower">������ �� ������ ��</param>
    /// <param name="hitDir">�˹��� ��ų ���� ����</param>
    /// <param name="attacker">�������� Ʈ������ ������Ʈ</param>
    public virtual void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {

    }
}
