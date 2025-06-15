using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceenWindow : Window
{
    protected override void OpenStart()
    {
        GameManager.Instance.WindowService.HideAllWindows(true);
        base.OpenStart();
        GameManager.Instance.IsCutsceenActive = true;
        
    }
    protected override void CloseEnd()
    {
        base.CloseEnd();
        GameManager.Instance.IsCutsceenActive = false;
    }
}
