using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    //����
    public Text MonsterText;
    public int MonsterHp = 3;
    public int MonsterMaxHp = 3;

    public Image HpImage; //�ν����� �Ҵ�
    public Image RedHitUi; //���ݹ޾��� �� �������� UI

    public GameObject WinText; //����

    //public GameObject MonsterFruit; //������ ����

    //ü�� ����
    public void HpDec()
    {
        MonsterHp--;
        MonsterText.text = MonsterHp + " / " + MonsterMaxHp;

        //ü�¹� ������ ����
        StartCoroutine(DecHpImage());
        StartCoroutine(TurnRed());
    }

    //���͸� �������� �� ȭ���� ������ ��� ���̴� ȿ��
    IEnumerator TurnRed()
    {
        float redFrame_1 = 1f / 80f; //�������Ӹ��� �޶����� red��

        //20 �����ӵ��� ȭ���� �Ӿ����� ���ƿ�
        for (int frameCnt = 0; frameCnt < 20; frameCnt++)
        {
            //ȭ�� �������� ȿ��
            Color col = new Color(RedHitUi.color.r, RedHitUi.color.g, RedHitUi.color.b, RedHitUi.color.a + redFrame_1);
            RedHitUi.color = col;
            yield return new WaitForSeconds(0.01f);
        }

        for (int frameCnt = 0; frameCnt < 20; frameCnt++)
        {
            //ȭ�� ���������� ȿ��
            Color col = new Color(RedHitUi.color.r, RedHitUi.color.g, RedHitUi.color.b, RedHitUi.color.a - redFrame_1);
            RedHitUi.color = col;
            yield return new WaitForSeconds(0.01f);
        }
        //�����ϰ� �ʱ�ȭ
        Color col_transf = new Color(RedHitUi.color.r, RedHitUi.color.g, RedHitUi.color.b, 0f);
        RedHitUi.color = col_transf;

        yield return null;
    }

    IEnumerator DecHpImage()
    {
        float HpBar_1 = 1f / MonsterMaxHp; //hp�� 1ĭ ����

        //20 �����ӵ��� ü�¹ٰ� HPBar_1 / 20��ŭ ����
        for (int frameCnt = 0; frameCnt < 20; frameCnt++)
        {
            HpImage.fillAmount -= HpBar_1 / 20f;
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

    //2�п� �ѹ��� ������ ��ҿ� �� �������� ���� ��ȯ
    private void Update()
    {
        if (MonsterHp <= 0) 
        {
            WinText.SetActive(true);
            Time.timeScale = 0f;
        }
    }

}
