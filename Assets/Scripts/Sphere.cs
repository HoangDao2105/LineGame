using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sphere : MonoBehaviour
{
    public event Action<Sphere> OnMoveComplete;


    public void Move(List<Vector3> path)
    {
        transform.DOPath(path.ToArray(),  0.25f)
            .SetEase(Ease.Linear)
            .OnComplete(() => OnMoveComplete?.Invoke(this));
    }

    public void DestroySphere()
    {
        transform.DOScale(0.1f, 0.25f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    

}
