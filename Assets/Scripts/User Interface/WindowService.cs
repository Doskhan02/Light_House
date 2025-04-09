using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowService : MonoBehaviour
{
    [SerializeField] private Window[] windows;
    private Dictionary<Type, Window> windowsDictionary;

    public void Initialize()
    {
        windowsDictionary = new Dictionary<Type, Window>();
        foreach (Window window in windows)
        {
            windowsDictionary.Add(window.GetType(), window);
            window.Hide(true);
            window.Initialize();
        }

        ShowWindow<MainMenuWindow>(true);
    }

    public T GetWindow<T>() where T : Window
    {
        return windowsDictionary[typeof(T)] as T;
    }

    public void ShowWindow<T>(bool isImmediately) where T : Window
    {
        var window = windowsDictionary[typeof(T)] as T;
        if (window == null)
        {
            Debug.LogError("Window not found");
            return;
        }
        window.Show(isImmediately);
    }
    public void HideWindow<T>(bool isImmediately) where T : Window
    {
        var window = windowsDictionary[typeof(T)] as T;
        if (window == null)
        {
            Debug.LogError("Window not found");
            return;
        }
        window.Hide(isImmediately);
    }
    public void HideAllWindows(bool isImmediately)
    {
        foreach (var window in windows)
        {
            window.Hide(isImmediately);
        }
    }
}
