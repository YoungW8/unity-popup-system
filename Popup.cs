using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Popup : MonoBehaviour
{
    [SerializeField] protected Button _coreBtn;
    [SerializeField] protected Button _additionalBtn;
    [SerializeField] protected Button _closeBtn;
    [SerializeField] bool _goldCounter = true;
    [SerializeField] bool _fade = true;

    protected PopupController _popupController;
    public bool open { get; private set; }
    private Action _onClose;

    public bool GoldCouter => _goldCounter;
    public bool Fade => _fade;

    public void Init(PopupController popupController)
    {
        _popupController = popupController;

        if (_coreBtn)
        {
            _coreBtn.onClick.AddListener(CoreBtn);
        }

        if (_additionalBtn)
        {
            _additionalBtn.onClick.AddListener(AdditionalBtn);
        }

        if (_closeBtn)
        {
            _closeBtn.onClick.AddListener(CloseBtn);
        }
    }

    public void Show(Action actionOnClose)
    {
        if (ShowRequierment())
        {
            open = true;

            _onClose = actionOnClose;

            gameObject.SetActive(true);
            ShowEvent();
            ChangeValues();
            
        }
        else
        {
            _popupController.PopupClose();
        }


    }

    public Coroutine Close(bool hideFade)
    {
        open = false;

        if (gameObject.active)
        {
            return StartCoroutine(CloseCoroutine(hideFade));
        }           
        else
        {
            _onClose?.Invoke();
            _onClose = null;
            CloseEvent();
            return null;
        }

    }

    public IEnumerator CloseCoroutine(bool hideFade)
    {
        Coroutine closeAnimationCoroutine = null;

        // If you need to wait for the end close animation place animation coroutine in closeAnimationCoroutine, else do nothing

        yield return closeAnimationCoroutine;

        _onClose?.Invoke();
        _onClose = null;
        CloseEvent();

        _popupController.PopupClose();
        gameObject.SetActive(false);           
    }    
    
    public abstract bool ShowRequierment();
    protected abstract void ChangeValues();
    protected abstract void ShowEvent();
    protected abstract void CloseEvent();
    protected abstract void CoreBtn();
    protected abstract void AdditionalBtn();
    protected abstract void CloseBtn();
}

