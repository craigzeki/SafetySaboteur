using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvasController : MonoBehaviour, iCanvasController
{
    [SerializeField] private RectTransform _menuPanelRectTransform;
    [SerializeField] private RectTransform _infoPanelRectTransform;
    [SerializeField] private RectTransform _buttonsRectTransform;
    [SerializeField] private Button _infoButton;

    private bool _loaded = false;
    private Vector3 _menuPanelEndPos;
    private Vector3 _menuPanelStartPos = new Vector3(0f,-1050f,0f);
    private Color  _infoPanelEndColor;
    private Color  _infoPanelStartColor;
    private float _infoPanelStartAlpha = 0f;
    private Vector3 _buttonsEndPos = new Vector3(-588f, -15f, 0f);
    private Vector3 _buttonsStartPos;

    private Coroutine _infoSelectedAnniCoroutine;
    private Coroutine _infoDeSelectedAnniCoroutine;

    private bool _infoButtonSelected = false;
    
    private void Awake()
    {
        if (_menuPanelRectTransform != null)
        {
            _menuPanelEndPos = _menuPanelRectTransform.localPosition;
            _menuPanelRectTransform.localPosition = _menuPanelStartPos;
        }
        
        if(_infoPanelRectTransform != null)
        {
            if(_infoPanelRectTransform.gameObject.TryGetComponent<Image>(out Image img))
            {
                _infoPanelEndColor = img.color;
                _infoPanelStartColor = img.color;
                _infoPanelStartColor.a = _infoPanelStartAlpha;
                img.color = _infoPanelStartColor;
            }
            _infoPanelRectTransform.gameObject.SetActive(false);
        }

        if(_buttonsRectTransform != null)
        {
            _buttonsStartPos = _buttonsRectTransform.localPosition;
        }

    }

    public void InfoButton()
    {
        if (_infoButton == null) return;
        if(_infoButtonSelected)
        {
            _infoButton.interactable = false;
            _infoButtonSelected = false;
            //deselect
            DoInfoSelected(false, () => { _infoButton.interactable = true; _infoPanelRectTransform.gameObject.SetActive(false); });
        }
        else
        {
            _infoButton.interactable = false;
            _infoButtonSelected = true;
            _infoPanelRectTransform.gameObject.SetActive(true);
            //select
            DoInfoSelected(true, () => { _infoButton.interactable = true; });
        }
    }

    private void DoInfoSelected(bool selected, Action callback)
    {
        if(selected)
        {
            if (_infoSelectedAnniCoroutine == null) _infoSelectedAnniCoroutine = StartCoroutine(InfoSelectedAnni(callback));
        }
        else
        {
            if (_infoDeSelectedAnniCoroutine == null) _infoDeSelectedAnniCoroutine = StartCoroutine(InfoDeSelectedAnni(callback));
        }
    }

    IEnumerator InfoSelectedAnni(Action callback)
    {
        if (!LeanTween.isTweening(_buttonsRectTransform)) _buttonsRectTransform.LeanMoveLocalX(_buttonsEndPos.x, 0.5f).setEaseOutBack();
        while(LeanTween.isTweening(_buttonsRectTransform))
        {
            yield return new WaitForEndOfFrame();
        }
        if (!LeanTween.isTweening(_infoPanelRectTransform)) _infoPanelRectTransform.LeanAlpha(_infoPanelEndColor.a, 0.5f).setEaseOutQuart().setOnComplete(callback);
        _infoSelectedAnniCoroutine = null;
    }

    IEnumerator InfoDeSelectedAnni(Action callback)
    {
        if (!LeanTween.isTweening(_infoPanelRectTransform)) _infoPanelRectTransform.LeanAlpha(_infoPanelStartColor.a, 0.5f).setEaseOutQuart();
        
        while (LeanTween.isTweening(_infoPanelRectTransform))
        {
            yield return new WaitForEndOfFrame();
        }
        if (!LeanTween.isTweening(_buttonsRectTransform)) _buttonsRectTransform.LeanMoveLocalX(_buttonsStartPos.x, 0.5f).setEaseOutBack().setOnComplete(callback);
        _infoDeSelectedAnniCoroutine = null;
    }

    public void DoLoad(Action callback = null)
    {
        if(_menuPanelRectTransform == null) return;
        if (_loaded) return;
        _loaded = true;
        if (!LeanTween.isTweening(_menuPanelRectTransform)) _menuPanelRectTransform.LeanMoveLocalY(_menuPanelEndPos.y, 1f).setEaseInOutBack().setOnComplete(callback);
        
    }

    public void DoUnload(Action callback = null)
    {
        if (_menuPanelRectTransform == null) return;
        if (!_loaded) return;
        _loaded = false;
        if (!LeanTween.isTweening(_menuPanelRectTransform)) _menuPanelRectTransform.LeanMoveLocalY(_menuPanelStartPos.y, 1f).setEaseInOutBack().setOnComplete(callback);
    }
}
