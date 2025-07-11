using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceenWindow : Window
{
    protected override void OpenStart()
    {
        base.OpenStart();
        GameManager.Instance.IsCutsceenActive = true;
    }
    
    protected override void CloseStart()
    {
        base.CloseStart(); // Исправлено: было CloseEnd()
        GameManager.Instance.IsCutsceenActive = false;
    }
}