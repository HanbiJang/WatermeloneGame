using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnStart : MonoBehaviour
{
    public GameObject progressCanvas; //�ν����� �Ҵ�
    public void SetActiveProgressCanvas() {
        progressCanvas.SetActive(true);
    }

}
