using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopBtn : MonoBehaviour
{
    bool isClicked = false; //������
    public Sprite PlayImage; //�ν����� �Ҵ�
    public Sprite PauseImage; //�ν����� �Ҵ� 

    public void BtnPause() {
        if (!isClicked) { //��ư�� ������ ���߱� ��� ����
            Time.timeScale = 0f;
            isClicked = true;
            gameObject.GetComponent<Image>().sprite = PlayImage;
        }
        else { //��ư�� ������ ��� ��� ����
            Time.timeScale = 1f;
            isClicked = false;
            gameObject.GetComponent<Image>().sprite = PauseImage;
        }
    }
}
