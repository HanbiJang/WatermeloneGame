using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class BtnSetting : MonoBehaviour
{
    string PopupName = "SettingPopup";
    UIPopup popup;

    public void BtnSettingOpen() {
        //get a clone of the UIPopup, with the given PopupName, from the UIPopup Database 
        popup = UIPopup.GetPopup(PopupName);

        //make sure that a popup clone was actually created
        if (popup == null)
            return;

        popup.Show(); //show popup
    }

    public void BtnSttingClose() {
        UIPopup.HidePopup(PopupName);
    }
}
