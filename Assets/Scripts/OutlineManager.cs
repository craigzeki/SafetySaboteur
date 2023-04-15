using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    [SerializeField] private List<Outline> _outlines = new List<Outline>();

    private void OnEnable()
    {
        SetOutlineEnabled(true);
    }

    private void OnDisable()
    {
        SetOutlineEnabled(false);
    }

    private void Start()
    {
        SetOutlineEnabled(false);
    }

    private void SetOutlineEnabled(bool enabled)
    {
        foreach (Outline outline in _outlines)
        {
            outline.enabled = enabled;
        }
    }

}
