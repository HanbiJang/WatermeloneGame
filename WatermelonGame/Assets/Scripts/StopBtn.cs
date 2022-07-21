using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopBtn : MonoBehaviour
{
    bool isClicked = false; //눌리면
    public Sprite PlayImage; //인스펙터 할당
    public Sprite PauseImage; //인스펙터 할당 

    public void BtnPause() {
        if (!isClicked) { //버튼이 눌리면 멈추기 기능 동작
            Time.timeScale = 0f;
            isClicked = true;
            gameObject.GetComponent<Image>().sprite = PlayImage;
        }
        else { //버튼이 눌리면 재생 기능 동작
            Time.timeScale = 1f;
            isClicked = false;
            gameObject.GetComponent<Image>().sprite = PauseImage;
        }
    }
}
