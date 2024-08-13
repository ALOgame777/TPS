using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponInfo
{
    public float damagePower;
    public int bulletCount;
    public float range;
    public WeaponType type;
}
public class Weapon : MonoBehaviour
{
    public WeaponInfo weaponInfo;

    

    void Update()
    {
        // y 축으로 회전을 한다.
        transform.Rotate(Vector3.up, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 만일 충돌한 대상이 PlayerFire 컴포넌트를 가지고 있다면...
        PlayerFire pFire = other.GetComponent<PlayerFire>();
        if (pFire != null)
        {
            // PlayerFire 컴포넌트에 weaponInfo 데이터를 넘긴다.
            pFire.ChangeWeapon(weaponInfo);
            // 자신은 사라진다.
            Destroy(gameObject);
        }
    }
}
