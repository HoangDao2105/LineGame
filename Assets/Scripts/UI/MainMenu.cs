using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{
    New,
    Load,
    Save,
    Quit
}

public class MainMenu : MonoBehaviour
{
    public event Action<ButtonType> OnClick;
    
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button newGamebtn;
    [SerializeField] private Button loadGamebtn;
    [SerializeField] private Button saveGamebtn;
    [SerializeField] private Button quitGamebtn;

    public bool Active
    {
        get
        {
            return canvasGroup.alpha == 1f;
        }
        set
        {
            canvasGroup.alpha = value ? 1f : 0f;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
            
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        newGamebtn.onClick.AddListener(()=>NewGame());
        loadGamebtn.onClick.AddListener(()=>LoadGame());
        saveGamebtn.onClick.AddListener((() => SaveGame()));
        quitGamebtn.onClick.AddListener((() => QuitGame()));
    }

    private void Start()
    {
        Active = false;
    }


    
    void NewGame()
    {
        OnClick?.Invoke(ButtonType.New);
    }

    void LoadGame()
    {
        OnClick?.Invoke(ButtonType.Load);
    }

    void SaveGame()
    {
        OnClick?.Invoke(ButtonType.Save);
    }
    void QuitGame()
    {
        OnClick?.Invoke(ButtonType.Quit);
    }
    
    
}
