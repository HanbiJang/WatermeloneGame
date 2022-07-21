using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnStart : MonoBehaviour
{
    public GameObject progressCanvas; //인스펙터 할당
    public void SetActiveProgressCanvas() {
        progressCanvas.SetActive(true);
    }

}
