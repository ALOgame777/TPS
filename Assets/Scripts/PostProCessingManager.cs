using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProCessingManager : MonoBehaviour
{
    public static PostProCessingManager Instance;
    Volume mainPost;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // 포스트 프로세싱 볼륨 컴포넌트를 가져온다.
        mainPost = GetComponent<Volume>();
    }

    public void GreyScaleOn()
    {
        ColorAdjustments colorAdjustment;
        // 만일, 프로필파일에서 ColorAdjustments 속성을 가져왔다면 ...
        if ( mainPost.profile.TryGet<ColorAdjustments>(out colorAdjustment))
        {
            //ColorAdjustments 속성을 활성화한다.
            colorAdjustment.active = true;
            // Saturation 값을 -100으로 변경한다.
            colorAdjustment.saturation.value = -100.0f;
        }

    }
}
