using UnityEngine;
using System.Collections;

public class FlagButton : MonoBehaviour
{

    // Handles the external link on the flat button

    public void OpenURL()
    {
        Application.OpenURL("https://oregonstate.qualtrics.com/jfe/form/SV_3IfO3l2H7FbOOuW");
    }

}