using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None,
    Pistol,
    SniperGun
}


public class PlayerFire : MonoBehaviour
{
    public GameObject bulletFXObject;
    public GameObject grenadePrefab;
    public Vector3 direction;
    public float throwPower = 5;
    public Transform firePosition;
    public WeaponType myWeaponType = WeaponType.SniperGun;

    
    //수류탄 궤적 그리기용 변수
    public float simulationTime = 5.0f;
    public float interval = 0.1f;
    public float mass = 5;
    public float grenadeRange = 5.0f;
    public GameObject targetTexture;
    public Animator myAnim;

    public float attackPower = 20;
    
    List<Vector3> trajectory = new List<Vector3>();
    ParticleSystem bulletEffect;
    LineRenderer line;
    FollowCamera followCam;

    void Start()
    {
        // 커서를 게임 뷰 안에 가둔다.
        Cursor.lockState = CursorLockMode.Locked;
        bulletEffect = bulletFXObject.GetComponent<ParticleSystem>();
        line = firePosition.GetComponent<LineRenderer>();

        targetTexture.transform.localScale = new Vector3(grenadeRange, grenadeRange, 1);
        targetTexture.SetActive(false);
        followCam = Camera.main.GetComponent<FollowCamera>();
    }

    void Update()
    {
        FireType1();
        switch(myWeaponType)
        {
            case WeaponType.None:
                break;
            case WeaponType.Pistol:
                FireType2();
                break;
            case WeaponType.SniperGun:
                FireType3();
                break;
                
        }
        
    }


    void FireType1()
    {
        // 만일, 마우스 왼쪽 버튼을 눌렀다면, 나의 정면 방향으로 총알을 발사하고 싶다.
        // 1. 마우스 왼쪽 버튼 입력 체크

        if (Input.GetMouseButtonDown(0))
        {
            myAnim.SetTrigger("Fire");
            // 2. 방향, 레이 생성, 체크 거리   
            // 2-1. 레이를 만든다.
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); //나의 위치에서 정면방향으로 레이를 만듬
            // 2-2 레이가 충돌한 대상의 정보를 담기 위한 구조체(Struct)를 생성한다.
            RaycastHit hitInfo;

            // 2-3. 만들어진 레이를 지정된 방향과 거리만큼 발사한다.
            // 7번 레이어 빼고 전부다
            bool isHit = Physics.Raycast(ray, out hitInfo, 1000, ~(1<<7)); // out 키워드 
            // 2-4. 만일, 레이가 충돌을 했다면 레이가 닿은 위치에 총알 이펙트를 표시한다.
            if (isHit)
            {
                //print(hitInfo.transform.name);
                //GameObject go = Instantiate(bulletFXObject, hitInfo.point, Quaternion.identity);

                //만일 , 충돌한 대상이 EnemyFSM 컴포넌틀 가지고 있다면...
                EnemyFSM enemy = hitInfo.transform.GetComponent<EnemyFSM>();
                if (enemy != null)
                {
                    //EnemyFSM의 TakeDamage 함수를 실행한다.
                    enemy.TakeDamage(attackPower, ray.direction, transform);
                }
                // 그렇지 않다면...
                else
                {
                    // 반복적인 이펙트 사용 방법
                    bulletFXObject.transform.position = hitInfo.point;
                    //충돌 지점의 법선 방향으로 이펙트를 회전한다.
                    bulletFXObject.transform.forward = hitInfo.normal;
                    bulletEffect.Play();
                }
            }
        }
    }
    void FireType2()
    {
        // 만일, 마우스 우측 버튼을 누르고 있다면...
        if(Input.GetMouseButton(1))
        {
            // 수류탄이 날아가는 궤적을 그린다.
            Vector3 startPos = firePosition.position;
            //Vector3 dir = transform.TransformDirection(direction);
            //dir.Normalize();
            Vector3 dir = Camera.main.transform.forward + Camera.main.transform.up;
            dir.Normalize();    
            Vector3 gravity = Physics.gravity;
            int simulCount = (int)(simulationTime / interval);
            Vector3 hitNormal = Vector3.zero;
            trajectory.Clear();
            for(int i = 0; i< simulCount; i++)
            {
                //p = p0 + vt - 0.5 * g * t^2 *m^2;
                float currentTime = interval * i;
                Vector3 result = startPos + dir * throwPower * currentTime + 0.5f * gravity * currentTime * currentTime * Mathf.Pow(mass,2);

                // 계산된 result 위치와 직전 위치 사이에 충돌할 물체가 있는지 확인한다.
                // Raycast를 이용
                if (trajectory.Count > 0)
                {
                    Vector3 rayDir = result - trajectory[trajectory.Count - 1];
                    Ray ray = new Ray(trajectory[trajectory.Count - 1], rayDir.normalized);
                    RaycastHit hitInfo;

                    // 만일, 부딪친 대상이 있다면...
                    if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
                    {
                        // 그 지점을 리스트에 추가하고 반복문을 종료한다.
                        trajectory.Add(hitInfo.point);
                        hitNormal = hitInfo.normal * 0.01f;
                        break;
                    }
                    // 그렇지 않다면... 
                    else
                    {
                        // result 위치를 리스트에 추가한다.
                        trajectory.Add(result);

                    }
                }
                else
                {
                    trajectory.Add(result);
                }

            }
            // 라인 랜더러로 trajectory 예측선을 화면에 그린다.
            line.positionCount = trajectory.Count;
            line.SetPositions(trajectory.ToArray());
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.startColor = Color.white;
            line.endColor = Color.white;

            // 탄착 지점에 범위 텍스쳐 오브젝트를 배치하고 활성화 한다.

            targetTexture.transform.position = trajectory[trajectory.Count - 1] + hitNormal;
            targetTexture.transform.forward = hitNormal * -100;
            targetTexture.SetActive(true);

        }
        // 만일, 마우스의 우측 버튼을 눌렀다가 떼면...
        else if (Input.GetMouseButtonUp(1))
        {
            // 수류탄 프리팹을 생성하고, 물리적으로 발사한다.
            GameObject bomb = Instantiate(grenadePrefab, firePosition.position, firePosition.rotation);
            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = mass;
                Vector3 dir = Camera.main.transform.forward + Camera.main.transform.up;
                dir.Normalize();
                // 물리적으로 발사하기
                // 물리적 힘을 가할 떄 사용, 방향x 힘의크기, 질량(MASS)의 영향 O, 중력 가속도(GRAVITY) 영향 O, 공기 저항(DRAG) 영향 O
                // Mode(Force(질량의 영향을 받음), Acceleration(질량에 영향이 없음). , Impulse()
                // VelocityChange
                rb.AddForce(dir * throwPower, ForceMode.Impulse);
            }

            targetTexture.SetActive(false);
        }
    }


    void FireType3()
    {
        // 마우스 우측 버튼을 누르면...
        if (Input.GetMouseButtonDown(1))
        {
            // 카메라가 줌인(확대) 된다.
            // 스나이퍼 UI를 표시한다.
            followCam.ZoomIn(true);


        }
        // 마우스 우측 버튼을 떼면...
        else if(Input.GetMouseButtonUp(1))
        {
            // 카메라의 배율을 다시 원래대로 돌려놓는다.
            // 스나이퍼 UI를 투명하게 한다.
            followCam.ZoomIn(false);

        }

    }

    //Scene View에 기즈모를 그리는 이벤트 함수
    private void OnDrawGizmos()
    {
        // trajectory
        if (trajectory.Count < 1)
        {
            return;
        }
        // 라인의 색상은 녹색으로 설정한다.
        Gizmos.color = Color.green;
        //trajectory 리스트의 모든 번호를 연결하여 라인을 그린다.
        for (int i = 0; i < trajectory.Count-1; i++)
        {
            Gizmos.DrawLine(trajectory[i], trajectory[i + 1]);
        }
    }
}
