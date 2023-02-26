using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OutlineOnMouseOver : MonoBehaviour
{
    private Outline outline;

    private void Awake()
    {
        //find the outline script and add a new outlinehelper script to each renderable child with reference to the root outline script
        outline = GetComponent<Outline>();
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (renderer.gameObject == this.gameObject) continue; //skip the first item in the list (the gameobject with this script!)
            OutlineHelper temp =  renderer.gameObject.AddComponent<OutlineHelper>();
            temp.RootOutline = outline;
        }
    }
    private void Start()
    {
        outline.enabled = false;
    }

    private void OnMouseEnter()
    {
        outline.enabled = true;

        ZekiController.Instance.TestLED();
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
    }

    
}
