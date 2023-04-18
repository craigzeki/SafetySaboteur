using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaknessRow : MonoBehaviour
{
    [SerializeField] private Image _padlock;
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Sprite _unlockedSprite;
    [SerializeField] private Color _lockedColor;
    [SerializeField] private Color _unlockedColor;

    public void SetPadlockState(bool locked)
    {
        if(locked)
        {
            _padlock.sprite = _lockedSprite;
            _padlock.color = _lockedColor;
        }
        else
        {
            _padlock.sprite = _unlockedSprite;
            _padlock.color = _unlockedColor;
        }
    }

}
