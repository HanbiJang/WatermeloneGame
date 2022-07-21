using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fruit : MonoBehaviour
{
    public GameObject FruitGameObject { get; set; } //���� ���� ������Ʈ
    public int level { get; set; } //������ ����
    public int FruitIndex { get; set; } //������ ����Ʈ �� �ε���

    FruitManager fruitManager;
    Monster monsterManager;
    bool isMerge;
    bool isSounded = false;

    private void Start()
    {
        fruitManager = GameObject.Find("FruitManager").GetComponent<FruitManager>();
        monsterManager = GameObject.Find("MonsterManager").GetComponent<Monster>();
    }

    //������ ó�� ��������� �� �ʱ�ȭ�Ѵ�
    public void InitFruit(List<Fruit> fruits, GameObject fruitGameObject, int level)
    {
        this.FruitGameObject = fruitGameObject;
        this.FruitIndex = fruits.Count - 1;
        this.level = level;
        this.isMerge = false;
    }

    //������ �浹
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Fruit") {
            Fruit other = collision.gameObject.GetComponent<Fruit>(); //�浹�� ������Ʈ
            
            FruitSoundPlay();

            if (other.level == this.level && !isMerge && !other.isMerge)
            {
                //Debug.Log("�浹!");
                Transform Tother = other.FruitGameObject.transform;
                Rigidbody2D Rother = other.FruitGameObject.GetComponent<Rigidbody2D>();

                //����, �������̸� => ���ʿ� �ִ� �ְ� ��� ������
                //��, �Ʒ��� => �Ʒ��� �ִ� �ְ� ��� ������
                if (this.transform.position.x < Tother.position.x) //���� ����
                {
                    isMerge = true;
                    other.isMerge = true;
                    //��� ������� & level �� or Hp ���           
                    StartCoroutine(SuckOther(Tother, Rother));

                }

                else if (this.transform.position.x == Tother.position.x && this.transform.position.y < Tother.position.y) //x��ǥ�� ���� ���� �Ʒ�
                {
                    isMerge = true;
                    other.isMerge = true;
                    //��� ������� & level �� & Hp ���
                    StartCoroutine(SuckOther(Tother, Rother));

                }

            }
        }
    }

    IEnumerator SuckOther(Transform Tother, Rigidbody2D Rother) {
        Rother.simulated = false;
        this.GetComponent<Rigidbody2D>().simulated = false;
        int frameCnt = 0;

        //Debug.Log("������ "+Tother.name +"," +"���̸�"+ this.name );

        //20�����ӵ��� �����Ѵ�
        while (frameCnt < 20)
        {
            frameCnt++;
            Tother.position = Vector3.Lerp(Tother.position, this.transform.position, 0.1f);
            yield return new WaitForSeconds(0.025f); //1�����ӿ� while�� 20�� ������ ���ϰ� �Ѵ�
        }
        //�ʱ�ȭ�ϱ�
        Rother.simulated = true;
        Rother.velocity = Vector2.zero;
        Rother.angularVelocity = 0f;
        Tother.gameObject.SetActive(false);
        Destroy(Tother.gameObject);
        this.GetComponent<Rigidbody2D>().simulated = true; 
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;        //�ӵ�, ������ �ʱ�ȭ
        this.GetComponent<Rigidbody2D>().angularVelocity = 0f;


        //��ƼŬ ����Ʈ ���
        fruitManager.PlayEffect(gameObject.transform, fruitManager.MergeEffectGameObject);

        //���� ���
        fruitManager.EffectSoundPlay(EffectSound.Merge);

        //���� ����
        fruitManager.UpdateScore(this.level * 10);

        //���� ������
        //Debug.Log("������");
        if (this.level != FruitManager.maxLevel)
        {
            //����, ũ�� ����
            gameObject.GetComponent<SpriteRenderer>().sprite = fruitManager.fruitSprite[(level+1) - 1];
            this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            this.level += 1;
            //�ִ� ���� ���� ����
            fruitManager.fruitMaxLevel = Mathf.Max(this.level-1, fruitManager.fruitMaxLevel);
        }

        else //���� Ÿ��
        {
            //���� ���
            Destroy(this.gameObject);
            fruitManager.EffectSoundPlay(EffectSound.Monster);

            //���� ü�� ����
            monsterManager.HpDec();
        }

        //�ʱ�ȭ
        isMerge = false;
        Camera.main.GetComponent<DOTweenAnimation>().DORestart();
    }


    void FruitSoundPlay() {

        if (!isSounded)
        {
            isSounded = true;
            StartCoroutine(FruitSoundRoutine());
        }
        
    }

    IEnumerator FruitSoundRoutine()
    {
        fruitManager.EffectSoundPlay(EffectSound.Drop);

        yield return new WaitForSeconds(0.5f);
        isSounded = false;
    }

}
