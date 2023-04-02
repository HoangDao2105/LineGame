using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [Header("Difficult")] [SerializeField] private int ballSpawn = 3;
    [Space(20)]
    [SerializeField] private DebugGrid grid;
    [SerializeField] private Selector selector;
    [SerializeField] private UIController ui;
    
    private StepData stepData;
    
    // Start is called before the first frame update
    void Start()
    {
        if (ballSpawn <= 0) ballSpawn = 3;
        selector.OnSelected += Selector_OnSelected;
        ui.MainMenu.OnClick += MainMenu_OnClick;
        newGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ui.MainMenu.Active = !ui.MainMenu.Active;
        }
    }

    private void MainMenu_OnClick(ButtonType btnType)
    {
        switch (btnType)
        {
            case ButtonType.New:
                newGame();
                break;
            case ButtonType.Quit:
                Application.Quit();
                break;
        }
    }

    void newGame()
    {
        ui.Points.Points = 0;
        ui.MainMenu.Active = false;
        grid.Clear();
        grid.StartGenerate(ballSpawn);
    }

    private void Selector_OnSelected(Sphere sphere, Cell cell)
    {
        var spherePos = sphere.transform.position;
        var cellPos = cell.transform.position;
        if (spherePos == cellPos ) return;
        var path = grid.GetPath(spherePos, cellPos);
        if(path==null) return;
        if (path.Count > 0)
        {
            stepData = new StepData(sphere, cell, path);
            StartStep(); 
        }
        
    }

    private void StartStep()
    {
        selector.Locked = true;
        grid.Move(stepData.Sphere.transform.position,stepData.Cell.transform.position);
        stepData.Cell.Highlight(true);
        stepData.Sphere.Move(stepData.Path);
        stepData.Sphere.OnMoveComplete += Sphere_OnMoveComplete;
        
    }

    private void Sphere_OnMoveComplete(Sphere obj)
    {
        CompleteStep();
    }

    private void CompleteStep()
    {
        selector.Locked = false;
        stepData.Sphere.OnMoveComplete -= Sphere_OnMoveComplete;
        stepData.Cell.Highlight(false);
        ui.Points.Points += grid.DestroyLines(stepData.Cell.transform.position);
        //
        grid.GenerateBall(ballSpawn);
        if (grid.GetEmptyCoords().Count < 5)
        {
            //Lose
            ui.MainMenu.Active = true;
            
        }
    }
    


}


public class StepData
{
    public Sphere Sphere
    {
        get;
        private set;
    }

    public Cell Cell
    {
        get;
        private set;
    }

    public List<Vector3> Path
    {
        get;
        private set;
    }

    public StepData(Sphere sphere, Cell cell, List<Vector3> path)
    {
        Sphere = sphere;
        Cell = cell;
        Path = path;
    }
}