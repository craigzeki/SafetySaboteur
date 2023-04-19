using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public enum CANVAS
    {
        MENU = 0,
        IN_PLAY,
        GAME_OVER,
        LEVEL_COMPLETE,
        NUM_OF_CANVASES
    }

    
    [SerializeField] private GameObject[] _canvasGameObjects = new GameObject[(int)CANVAS.NUM_OF_CANVASES];
    private iCanvasController[] _canvasControllers = new iCanvasController[(int)CANVAS.NUM_OF_CANVASES];

    private static CanvasManager instance;

    public static CanvasManager Instance
    {
        get
        {
            if(instance == null) instance = FindObjectOfType<CanvasManager>();
            return instance;
        }
    }

    private void Awake()
    {
        for(CANVAS i = CANVAS.MENU; i < CANVAS.NUM_OF_CANVASES; i++)
        {
            if (_canvasGameObjects[(int)i] != null) _canvasControllers[(int)i] = _canvasGameObjects[(int)i].GetComponent<iCanvasController>();
        }
    }

    public void LoadCanvas(CANVAS canvas, Action callback = null)
    {
        if (canvas >= CANVAS.NUM_OF_CANVASES) return;
        SetCanvasEnable(canvas, true);
        if (_canvasControllers[(int)canvas] != null) _canvasControllers[(int)canvas].DoLoad(callback);

    }


    public void UnLoadCanvas(CANVAS canvas, Action callback = null)
    {
        if (canvas >= CANVAS.NUM_OF_CANVASES) return;

        if (_canvasControllers[(int)canvas] != null) _canvasControllers[(int)canvas].DoUnload(callback);
    }

    public void SetCanvasEnable(CANVAS canvas, bool enable)
    {
        if (_canvasGameObjects[(int)canvas] != null) _canvasGameObjects[(int)canvas].SetActive(enable);
    }
}
