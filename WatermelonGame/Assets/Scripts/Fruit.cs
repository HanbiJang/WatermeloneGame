using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fruit : MonoBehaviour
{
    public GameObject FruitGameObject { get; set; } //과일 게임 오브젝트
    public int level { get; set; } //과일의 레벨
    public int FruitIndex { get; set; } //과일의 리스트 내 인덱스

    FruitManager fruitManager;
    Monster monsterManager;
    bool isMerge;
    bool isSounded = false;

    private void Start()
    {
        fruitManager = GameObject.Find("FruitManager").GetComponent<FruitManager>();
        monsterManager = GameObject.Find("MonsterManager").GetComponent<Monster>();
    }

    //과일이 처음 만들어졌을 때 초기화한다
    public void InitFruit(List<Fruit> fruits, GameObject fruitGameObject, int level)
    {
        this.FruitGameObject = fruitGameObject;
        this.FruitIndex = fruits.Count - 1;
        this.level = level;
        this.isMerge = false;
    }

    //과일의 충돌
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Fruit") {
            Fruit other = collision.gameObject.GetComponent<Fruit>(); //충돌한 오브젝트
            
            FruitSoundPlay();

            if (other.level == this.level && !isMerge && !other.isMerge)
            {
                //Debug.Log("충돌!");
                Transform Tother = other.FruitGameObject.transform;
                Rigidbody2D Rother = other.FruitGameObject.GetComponent<Rigidbody2D>();

                //왼쪽, 오른쪽이면 => 왼쪽에 있는 애가 살고 레벨업
                //위, 아래면 => 아래에 있는 애가 살고 레벨업
                if (this.transform.position.x < Tother.position.x) //내가 왼편
                {
                    isMerge = true;
                    other.isMerge = true;
                    //상대 끌어당기기 & level 업 or Hp 깎기           
                    StartCoroutine(SuckOther(Tother, Rother));

                }

                else if (this.transform.position.x == Tother.position.x && this.transform.position.y < Tother.position.y) //x좌표가 같고 내가 아래
                {
                    isMerge = true;
                    other.isMerge = true;
                    //상대 끌어당기기 & level 업 & Hp 깎기
                    StartCoroutine(SuckOther(Tother, Rother));

                }

            }
        }
    }

    IEnumerator SuckOther(Transform Tother, Rigidbody2D Rother) {
        Rother.simulated = false;
        this.GetComponent<Rigidbody2D>().simulated = false;
        int frameCnt = 0;

        //Debug.Log("닿은애 "+Tother.name +"," +"내이름"+ this.name );

        //20프레임동안 실행한다
        while (frameCnt < 20)
        {
            frameCnt++;
            Tother.position = Vector3.Lerp(Tother.position, this.transform.position, 0.1f);
            yield return new WaitForSeconds(0.025f); //1프레임에 while문 20번 돌리지 못하게 한다
        }
        //초기화하기
        Rother.simulated = true;
        Rother.velocity = Vector2.zero;
        Rother.angularVelocity = 0f;
        Tother.gameObject.SetActive(false);
        Destroy(Tother.gameObject);
        this.GetComponent<Rigidbody2D>().simulated = true; 
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;        //속도, 물리값 초기화
        this.GetComponent<Rigidbody2D>().angularVelocity = 0f;


        //파티클 이펙트 재생
        fruitManager.PlayEffect(gameObject.transform, fruitManager.MergeEffectGameObject);

        //사운드 재생
        fruitManager.EffectSoundPlay(EffectSound.Merge);

        //점수 갱신
        fruitManager.UpdateScore(this.level * 10);

        //나는 레벨업
        //Debug.Log("레벨업");
        if (this.level != FruitManager.maxLevel)
        {
            //레벨, 크기 증가
            gameObject.GetComponent<SpriteRenderer>().sprite = fruitManager.fruitSprite[(level+1) - 1];
            this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            this.level += 1;
            //최대 과일 레벨 증가
            fruitManager.fruitMaxLevel = Mathf.Max(this.level-1, fruitManager.fruitMaxLevel);
        }

        else //몬스터 타격
        {
            //사운드 재생
            Destroy(this.gameObject);
            fruitManager.EffectSoundPlay(EffectSound.Monster);

            //몬스터 체력 감소
            monsterManager.HpDec();
        }

        //초기화
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
