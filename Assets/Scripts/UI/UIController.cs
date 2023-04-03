using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private MainMenu mainmenu;  
    [SerializeField] private Point point;
    [SerializeField] private TextMeshProUGUI timertxt;

    public Point Points
    {
        get { return point; }
    }

    public MainMenu MainMenu
    {
        get { return mainmenu; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timertxt.text ="Time: " + (int)Time.time/60+":"+(int)Time.time%60;
    }
}
