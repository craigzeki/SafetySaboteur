using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHelper : MonoBehaviour
{
    public Outline RootOutline;

    private void OnMouseEnter()
    {
        if (RootOutline == null) return;
        RootOutline.enabled = true;
        ZekiController.Instance.TestLED();
    }

    private void OnMouseExit()
    {
        if (RootOutline == null) return;
        RootOutline.enabled = false;
    }
}
