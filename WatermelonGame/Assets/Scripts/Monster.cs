using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    //몬스터
    public Text MonsterText;
    public int MonsterHp = 3;
    public int MonsterMaxHp = 3;

    public Image HpImage; //인스펙터 할당
    public Image RedHitUi; //공격받았을 때 빨개지는 UI

    public GameObject WinText; //지정

    //public GameObject MonsterFruit; //괴물의 공격

    //체력 감소
    public void HpDec()
    {
        MonsterHp--;
        MonsterText.text = MonsterHp + " / " + MonsterMaxHp;

        //체력바 서서히 감소
        StartCoroutine(DecHpImage());
        StartCoroutine(TurnRed());
    }

    //몬스터를 공격했을 때 화면이 빨갛게 잠깐 보이는 효과
    IEnumerator TurnRed()
    {
        float redFrame_1 = 1f / 80f; //한프레임마다 달라지는 red기

        //20 프레임동안 화면이 붉어졌다 돌아옴
        for (int frameCnt = 0; frameCnt < 20; frameCnt++)
        {
            //화면 빨개지는 효과
            Color col = new Color(RedHitUi.color.r, RedHitUi.color.g, RedHitUi.color.b, RedHitUi.color.a + redFrame_1);
            RedHitUi.color = col;
            yield return new WaitForSeconds(0.01f);
        }

        for (int frameCnt = 0; frameCnt < 20; frameCnt++)
        {
            //화면 투명해지는 효과
            Color col = new Color(RedHitUi.color.r, RedHitUi.color.g, RedHitUi.color.b, RedHitUi.color.a - redFrame_1);
            RedHitUi.color = col;
            yield return new WaitForSeconds(0.01f);
        }
        //투명하게 초기화
        Color col_transf = new Color(RedHitUi.color.r, RedHitUi.color.g, RedHitUi.color.b, 0f);
        RedHitUi.color = col_transf;

        yield return null;
    }

    IEnumerator DecHpImage()
    {
        float HpBar_1 = 1f / MonsterMaxHp; //hp바 1칸 길이

        //20 프레임동안 체력바가 HPBar_1 / 20만큼 감소
        for (int frameCnt = 0; frameCnt < 20; frameCnt++)
        {
            HpImage.fillAmount -= HpBar_1 / 20f;
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }


    private void Update()
    {
        if (MonsterHp <= 0) 
        {
            WinText.SetActive(true);
            Time.timeScale = 0f;
        }
    }

}
