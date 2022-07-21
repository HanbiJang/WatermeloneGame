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
    public GameObject FruitPrefab; //������ ������
    public GameObject FruitParent; //���� �θ�
    public GameObject newFruitGameObject { get; set; } //�� ����
    public GroundControl groundControl; //�� ��ũ��Ʈ ( �Ҹ� ��� ) 

    Vector2 ClickPoint; //Ŭ���� ��ǥ
    List<Fruit> fruits; //������ ����Ʈ
    bool isClicked; //Ŭ���Ǿ�����. true�� Ŭ�� ������ ���´�
    bool isReady; //true�� �ð� 2�� �������� Ȯ��
    float speed = 10f;
    Vector3 target; //��ǥ ����
    bool isThere = false; //��ǥ������ �����Ͽ��°�

    public const int minLevel = 1; //�ּ� ���� (����)
    public int fruitMaxLevel = 1; //������ ���� �ִ� ����
    public const int maxLevel = 7; //���� �ִ� ���� (����)

    //���� ��������Ʈ
    public List<Sprite> fruitSprite; //�ν����Ϳ��� �Ҵ�

    //��ƼŬ
    public GameObject EffectParent; //�ν����� �Ҵ�
    public GameObject MergeEffectGameObject; //���ӿ�����Ʈ, �ν����� �Ҵ�
    public GameObject ScoreEffectGameObject; //���ӿ�����Ʈ, �ν����� �Ҵ�

    //����
    public AudioClip[] audioClips; //�ν����� �Ҵ�
    public AudioSource[] SoundChannels; //�ν����� �Ҵ�
    int channelNum = 0;
    public AudioSource MonsterChannel;

    //����
    public int userScore = 0;
    public GameObject UIGameObject;
    public Text ScoreText;
    public Transform ScoreTextsPosition;



    void Start()
    {
        target = Vector3.zero;
        isReady = true;
        fruits = new List<Fruit>(); //���� ����Ʈ �����Ҵ�
        ClickPoint = Vector2.zero;

        //ȭ�鿡�� â������.
        CreateFruit();
        //������ �ٵ� ��� �����
        StopRigidSim();
    }


    void Update()
    {
        float step = speed * Time.deltaTime;

        //���콺�� ������ �� true�� �����
        SetIsClicked();

        //Ŭ���� �Ǹ�
        if (isClicked)
        {
            isClicked = false;

            target = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 0, 0);
            if (newFruitGameObject)
            {


                float rightBorder = 2.8f - newFruitGameObject.transform.localScale.x / 2;
                float leftBorder = -2.8f + newFruitGameObject.transform.localScale.x / 2;
                //���� �����ϱ�
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

                //��� ������ Ground ���¶��
                if (isReady)
                {
                    isReady = false;

                    //2�� ���� ���� �ʱ�ȭ : isGround, target
                    SetInitialCoroutine();
                }
            }
        }

        //������ �������� ��ġ �ٲٱ�. ������ٵ� Ȱ��ȭ
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

    //��ƼŬ ���
    public void PlayEffect(Transform trans, GameObject EffectGameObject)
    {
        //Debug.Log(trans.position);
        GameObject newEffect;
        newEffect = Instantiate(EffectGameObject, EffectParent.transform, false);
        newEffect.transform.position = new Vector3(trans.position.x, trans.position.y, trans.position.z);
        newEffect.GetComponent<ParticleSystem>().Play();//��� (�˾Ƽ� ������)
    }

    //���콺�� ������ �� true�� �����
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
    /// ȭ�鿡�� Ŭ��(��) x��ǥ�� �޾Ƽ� (�� x��ǥ, createPoint y��ǥ) ���� â������
    /// </summary>
    void CreateFruit()
    {
        Fruit newFruit;

        //����
        //int randomLevel = 7; //�׽�Ʈ��
        int randomLevel = (int)Random.Range(minLevel, fruitMaxLevel + 1);

        //���� n�� ���� ũ�Ⱑ 0.2 * n�� ����. 
        newFruitGameObject = Instantiate(FruitPrefab, FruitParent.transform, false) as GameObject;
        newFruitGameObject.transform.localScale = new Vector3(
            newFruitGameObject.transform.localScale.x + 0.2f * randomLevel,
            newFruitGameObject.transform.localScale.y + 0.2f * randomLevel,
            newFruitGameObject.transform.localScale.z + 0.2f * randomLevel
            );
        //��������Ʈ�� n-1��° ��������Ʈ�� �ٲ�����
        newFruitGameObject.GetComponent<SpriteRenderer>().sprite = fruitSprite[randomLevel - 1];


        //���� ����Ʈ�� �߰��Ѵ�
        newFruit = newFruitGameObject.GetComponent<Fruit>();
        newFruit.InitFruit(fruits, newFruitGameObject, randomLevel);
        fruits.Add(newFruit);

        //���� ���
        EffectSoundPlay(EffectSound.Merge);
    }

    //�ð��� 2f ������ ���� ������ �ʱ�ȭ �Ѵ�
    void SetInitialCoroutine()
    {
        StartCoroutine(Time2F());
    }

    /// <summary>
    /// 2�� �ð�
    /// </summary>
    /// <returns></returns>
    IEnumerator Time2F()
    {
        yield return new WaitForSeconds(2f);

        //�ʱ�ȭ
        isReady = true;
        target = Vector3.zero;
        isThere = false;
        CreateFruit();
        StopRigidSim();//������ �ٵ� ��� �����
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
            case EffectSound.Merge: //1�� 2�߿� �����ϰ� ���
                SoundChannels[channelNum].clip = audioClips[(int)Random.Range(1f, 2.1f)];
                SoundChannels[channelNum].Play();
                break;
            case EffectSound.Monster: //1�� 2�߿� �����ϰ� ���
                MonsterChannel.clip = audioClips[3];
                MonsterChannel.Play();
                break;
        }

        channelNum = (channelNum + 1) % SoundChannels.Length;

    }

    //���� ����
    public void UpdateScore(int addedScore)
    {
        userScore += addedScore;
        //ScoreText.text = userScore.ToString();
        ScoreText.text = string.Format("{0:00000}", userScore);
        PlayEffect(ScoreTextsPosition, ScoreEffectGameObject);
        UIGameObject.GetComponent<DOTweenAnimation>().DORestart();
    }
}
