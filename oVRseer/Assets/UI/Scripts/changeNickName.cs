using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeNickName : MonoBehaviour
{
    public Text nickText;
    public void ChangeNickName(string newNickname)
    {
        nickText.text = newNickname;
    }
}
