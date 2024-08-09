using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class CinemaManager : MonoBehaviour
{
    public static CinemaManager instance;
    public PlayableDirector director;

    private PlayerMove player;
    private List<EnemyFSM> enemies = new List<EnemyFSM>();
    bool isStartCinema = false;
    GameObject mainCam;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }    
    }
    void Start()
    {
        player = FindAnyObjectByType<PlayerMove>();
        enemies = FindObjectsOfType<EnemyFSM>().ToList();
        mainCam = Camera.main.gameObject;
    }

    void Update()
    {
        // ����, �ó׸ӽ��� ���۵Ǿ��ٸ�...
        if(isStartCinema)
        {
            // ����, ���� ���� �ð��� ��ü ���� �ð��� �����ߴٸ�...
            if (director.time >= director.duration)
            {
                // �ó׸ӽ��� �����ϰ� �ʹ�.
                director.Stop();
                // �÷��̾�� ���ʹ̵��� ���¸� �븻, idle ���·� ��ȯ�Ѵ�.
                player.myMoveState = PlayerMove.PlayerMoveState.Normal;
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].myState = EnemyFSM.Enemystate.Idle;
                }
                isStartCinema = false;
            }
        }
    }

    public void StartCineMachine()
    {
        // �ó׸ӽ��� ������ ���� PlayableDirector ������Ʈ�� �˸���
        director.Play();
        isStartCinema = true;
        // �÷��̾�� ���ʹ̵��� ���¸� ��� ���׸� ���·� ��ȯ�Ѵ�.
        player.myMoveState = PlayerMove.PlayerMoveState.Cinematic;
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].myState = EnemyFSM.Enemystate.Cinematic;
        }
    }
}
