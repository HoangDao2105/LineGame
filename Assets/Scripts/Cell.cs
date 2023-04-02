using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private Color highlightedColor;

    private Renderer renderer;

    private Color originalColor;

    public void Highlight(bool highlighted)
    {
        renderer.material.color = highlighted ? highlightedColor : originalColor;
    }
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
