using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EffectSound
{
    Drop, Merge, Monster
}
public class FruitManager : MonoBehaviour
{
    public GameObject FruitPrefab; //과일의 프리펩
    public GameObject FruitParent; //과일 부모
    public GameObject newFruitGameObject { get; set; } //새 과일
    public GroundControl groundControl; //땅 스크립트 ( 소리 기능 ) 

    Vector2 ClickPoint; //클릭된 좌표
    List<Fruit> fruits; //과일의 리스트
    bool isClicked; //클릭되었는지. true면 클릭 동작을 막는다
    bool isReady; //true면 시간 2초 지났음을 확인
    float speed = 10f;
    Vector3 target; //목표 지점
    bool isThere = false; //목표지점에 도달하였는가

    public const int minLevel = 1; //최소 레벨 (고정)
    public int fruitMaxLevel = 1; //씬상의 과일 최대 레벨
    public const int maxLevel = 7; //실제 최대 레벨 (고정)

    //과일 스프라이트
    public List<Sprite> fruitSprite; //인스펙터에서 할당

    //파티클
    public GameObject EffectParent; //인스펙터 할당
    public GameObject MergeEffectGameObject; //게임오브젝트, 인스펙터 할당
    public GameObject ScoreEffectGameObject; //게임오브젝트, 인스펙터 할당

    //사운드
    public AudioClip[] audioClips; //인스펙터 할당
    public AudioSource[] SoundChannels; //인스펙터 할당
    int channelNum = 0;
    public AudioSource MonsterChannel;

    //점수
    public int userScore = 0;
    public GameObject UIGameObject;
    public Text ScoreText;
    public Transform ScoreTextsPosition;



    void Start()
    {
        target = Vector3.zero;
        isReady = true;
        fruits = new List<Fruit>(); //과일 리스트 동적할당
        ClickPoint = Vector2.zero;

        //화면에서 창조를함.
        CreateFruit();
        //리지드 바디를 잠시 멈춘다
        StopRigidSim();
    }


    void Update()
    {
        float step = speed * Time.deltaTime;

        //마우스가 눌렸을 시 true로 만든다
        SetIsClicked();

        //클릭이 되면
        if (isClicked)
        {
            isClicked = false;

            target = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 0, 0);
            if (newFruitGameObject)
            {


                float rightBorder = 2.8f - newFruitGameObject.transform.localScale.x / 2;
                float leftBorder = -2.8f + newFruitGameObject.transform.localScale.x / 2;
                //범위 제한하기
                if (newFruitGameObject != null)
                {
                    if (target.x > rightBorder)
                    {
                        target.x = rightBorder;
                    }
                    else if (target.x < leftBorder)
                    {
                        target.x = leftBorder;
                    }
                }

                //모든 과일이 Ground 상태라면
                if (isReady)
                {
                    isReady = false;

                    //2초 게임 조건 초기화 : isGround, target
                    SetInitialCoroutine();
                }
            }
        }

        //과일의 떨어지는 위치 바꾸기. 리지드바디 활성화
        if (target != Vector3.zero && !isThere)
        {
            PlayRigidSim();

            if (newFruitGameObject.transform.localPosition == target)
            {
                isThere = true;
            }
            newFruitGameObject.transform.localPosition = Vector3.MoveTowards(newFruitGameObject.transform.localPosition, target, step);
        }

    }

    //파티클 재생
    public void PlayEffect(Transform trans, GameObject EffectGameObject)
    {
        //Debug.Log(trans.position);
        GameObject newEffect;
        newEffect = Instantiate(EffectGameObject, EffectParent.transform, false);
        newEffect.transform.position = new Vector3(trans.position.x, trans.position.y, trans.position.z);
        newEffect.GetComponent<ParticleSystem>().Play();//재생 (알아서 삭제됨)
    }

    //마우스가 눌렸을 시 true로 만든다
    private void SetIsClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > 1.8f &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x < 2.8f &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y < 5.3f &&
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 3.9f
                )
            {

            }
            else isClicked = true;
        }
    }

    /// <summary>
    /// 화면에서 클릭(업) x좌표를 받아서 (그 x좌표, createPoint y좌표) 에서 창조를함
    /// </summary>
    void CreateFruit()
    {
        Fruit newFruit;

        //생성
        //int randomLevel = 7; //테스트용
        int randomLevel = (int)Random.Range(minLevel, fruitMaxLevel + 1);

        //레벨 n에 따라 크기가 0.2 * n씩 증가. 
        newFruitGameObject = Instantiate(FruitPrefab, FruitParent.transform, false) as GameObject;
        newFruitGameObject.transform.localScale = new Vector3(
            newFruitGameObject.transform.localScale.x + 0.2f * randomLevel,
            newFruitGameObject.transform.localScale.y + 0.2f * randomLevel,
            newFruitGameObject.transform.localScale.z + 0.2f * randomLevel
            );
        //스프라이트도 n-1번째 스프라이트로 바뀌어야함
        newFruitGameObject.GetComponent<SpriteRenderer>().sprite = fruitSprite[randomLevel - 1];


        //과일 리스트에 추가한다
        newFruit = newFruitGameObject.GetComponent<Fruit>();
        newFruit.InitFruit(fruits, newFruitGameObject, randomLevel);
        fruits.Add(newFruit);

        //사운드 재생
        EffectSoundPlay(EffectSound.Merge);
    }

    //시간이 2f 지나면 게임 조건을 초기화 한다
    void SetInitialCoroutine()
    {
        StartCoroutine(Time2F());
    }

    /// <summary>
    /// 2초 시간
    /// </summary>
    /// <returns></returns>
    IEnumerator Time2F()
    {
        yield return new WaitForSeconds(2f);

        //초기화
        isReady = true;
        target = Vector3.zero;
        isThere = false;
        CreateFruit();
        StopRigidSim();//리지드 바디를 잠시 멈춘다
        groundControl.isPlayed = false;
    }

    void StopRigidSim()
    {
        newFruitGameObject.GetComponent<Rigidbody2D>().simulated = false;
    }

    void PlayRigidSim()
    {
        newFruitGameObject.GetComponent<Rigidbody2D>().simulated = true;
    }

    public void EffectSoundPlay(EffectSound effectSound)
    {

        switch (effectSound)
        {
            case EffectSound.Drop:
                SoundChannels[channelNum].clip = audioClips[0];
                SoundChannels[channelNum].Play();
                break;
            case EffectSound.Merge: //1과 2중에 랜덤하게 재생
                SoundChannels[channelNum].clip = audioClips[(int)Random.Range(1f, 2.1f)];
                SoundChannels[channelNum].Play();
                break;
            case EffectSound.Monster: //1과 2중에 랜덤하게 재생
                MonsterChannel.clip = audioClips[3];
                MonsterChannel.Play();
                break;
        }

        channelNum = (channelNum + 1) % SoundChannels.Length;

    }

    //점수 계산기
    public void UpdateScore(int addedScore)
    {
        userScore += addedScore;
        //ScoreText.text = userScore.ToString();
        ScoreText.text = string.Format("{0:00000}", userScore);
        PlayEffect(ScoreTextsPosition, ScoreEffectGameObject);
        UIGameObject.GetComponent<DOTweenAnimation>().DORestart();
    }
}
