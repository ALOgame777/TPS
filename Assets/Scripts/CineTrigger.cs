using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CineTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 대상이 플레이어라면...
        if(other.transform.tag == "Player")
        {
            // 시네머신을 플레이 한다
            //CinemaManager.instance.StartCineMachine();

            // 비디오를 플레이한다.
            if (videoPlayer != null)
            {
                videoPlayer.Play();
                //videoPlayer.frame = 300;
                videoPlayer.time = 60;
            }
            gameObject.SetActive(false);
        }
    }
}
