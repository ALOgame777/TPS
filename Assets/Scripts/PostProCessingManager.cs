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
        // ����Ʈ ���μ��� ���� ������Ʈ�� �����´�.
        mainPost = GetComponent<Volume>();
    }

    public void GreyScaleOn()
    {
        ColorAdjustments colorAdjustment;
        // ����, ���������Ͽ��� ColorAdjustments �Ӽ��� �����Դٸ� ...
        if ( mainPost.profile.TryGet<ColorAdjustments>(out colorAdjustment))
        {
            //ColorAdjustments �Ӽ��� Ȱ��ȭ�Ѵ�.
            colorAdjustment.active = true;
            // Saturation ���� -100���� �����Ѵ�.
            colorAdjustment.saturation.value = -100.0f;
        }

    }
}
