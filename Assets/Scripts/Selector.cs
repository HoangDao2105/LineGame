using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour
{
    public event Action<Sphere, Cell> OnSelected; 
    [SerializeField] private Camera cam;
    [SerializeField] private EventSystem eventSystem;
    private Sphere selected;
    public bool Locked { get; set; }

    private T RayCast<T>() where T : Component
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.collider.name);
            return hit.transform.GetComponent<T>();   
        }
        return null;
    }
    private void Update()
    {
        if(Locked) return;
        if (Input.GetMouseButtonUp(0))
        {
            if (eventSystem.IsPointerOverGameObject()) return;
            var sphere = RayCast<Sphere>();
            if (sphere != null)
            {
                selected = sphere;
            }
            else
            {
                if (selected != null)
                {
                    var cell = RayCast<Cell>();
                    if (cell != null)
                        OnSelected?.Invoke(selected, cell);
                    selected = null;
                }
            }
        }
    }
}
