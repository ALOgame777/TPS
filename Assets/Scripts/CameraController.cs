using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 cameraOffset = new Vector3(0.3f, 0.3f, -4.0f);
    public float currentRate = 1;

    // 배열(Array)
    //Transform[] camPositions = new Transform[2];

    // 리스트(List)
    List<Transform> camList = new List<Transform>();

    float my = 0;

    void Start()
    {
        // 자식 게임오브젝트 중에서 두번째(Near)와 세번째(Far) 오브젝트를 찾아서 배열에 넣는다.
        //camPositions[0] = transform.GetChild(0);
        //camPositions[1] = transform.GetChild(1);

        camList.Add(transform.GetChild(0));
        camList.Add(transform.GetChild(1));

        camList[1].localPosition = camList[0].localPosition + cameraOffset;

        // 초기 카메라는 Near 카메라(1인칭)로 한다.
        ChangeCamTarget(1, false);
    }

    void Update()
    {
        // 만일, 숫자 키 1번을 누르면 NearPos를 타겟으로 지정하고,
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeCamTarget(0, false);
        }
        // 2번을 누르면 FarPos를 타겟으로 지정한다.
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeCamTarget(1, true);
        }


        // 마우스 휠에 따라 farPos의 위치를 변경한다.
        currentRate -= Input.GetAxis("Mouse ScrollWheel") * 0.5f;
        //currentRate += Time.deltaTime *0.5f;
        currentRate = Mathf.Clamp(currentRate, 0.0f, 1.0f);

        Vector3 startPos = camList[0].localPosition;
        Vector3 endPos = startPos + cameraOffset;

        // 만일, startPos에서 endPos까지 사이에 장애물이 있다면...
        Vector3 dir = endPos - startPos;
        dir = transform.TransformDirection(dir);

        // nearPos로부터 endPos 방향으로 Ray를 발사한다.
        Ray ray = new Ray(camList[0].position, dir.normalized);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, dir.magnitude, ~(1 << 7)))
        {
            //print(hitInfo.transform.name);
            // endPos의 위치를 장애물 앞쪽으로 10cm정도 앞으로 조정한다.
            Vector3 hitPosition = hitInfo.point + hitInfo.normal * 0.1f;
            endPos = hitPosition - transform.position;

            // 위에서 계산한 endPos (방향)벡터를 플레이어가 회전된 만큼 회전시킨다.
            // 카메라는 플레이어의 뒤쪽에 있으므로 Quaternion 방향을 역방향으로 계산한다.
            endPos = Quaternion.Inverse(transform.rotation) * endPos;
        }

        camList[1].localPosition = Vector3.Lerp(startPos, endPos, currentRate);

        // 마우스의 상하 드래그에 맞춰 farPos가 상하 회전을 하도록 한다.
        my += Input.GetAxis("Mouse Y") * 200 * Time.deltaTime;
        my = Mathf.Clamp(my, -30, 30);
        camList[1].localEulerAngles = new Vector3(-my, 0, 0);


    }

    void ChangeCamTarget(int targetNum, bool isDynamic)
    {
        // Far Pos 오브젝트의 위치를 변경한다.

        if (camList[1] != null)
        {
            if (targetNum == 0)
            {
                // farPos의 위치를 nearPos의 위치와 일치시킨다.
                camList[1].position = camList[0].position;
            }
            else if (targetNum == 1)
            {
                // farPos의 위치를 nearPos의 위치로부터 offset만큼 이동시킨다.

                camList[1].localPosition = camList[0].localPosition + cameraOffset;
            }

        }
    }


}
