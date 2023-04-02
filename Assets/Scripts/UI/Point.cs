using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoretext;
    [SerializeField] private TextMeshProUGUI highScoretext;
    [SerializeField] private ParticleSystem newNighScoreEffect;
    private int points;

    public int Points
    {
        get { return points; }
        set
        {
            
            points = value;
            scoretext.text = "Score: " + points;
            transform.DOScale(Vector3.one * 1.5f, 0.25f)
                    .SetEase(Ease.Flash)
                    .OnComplete((() =>
                    {
                        transform.localScale = Vector3.one;
                    }));

            if (points > PlayerPrefs.GetInt("highscore"))
            {
                PlayerPrefs.SetInt("highscore", value);
                newNighScoreEffect.Play();
                highScoretext.text = "High Score: " + PlayerPrefs.GetInt("highscore");
            }
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {        
        //PlayerPrefs.DeleteKey("highscore");
        if (!PlayerPrefs.HasKey("highscore"))
        {
            PlayerPrefs.SetInt("highscore",0);
        }
        Points = 0;
        highScoretext.text = "High Score: " + PlayerPrefs.GetInt("highscore");
    }


}
