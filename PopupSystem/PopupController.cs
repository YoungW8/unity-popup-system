using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject fade;
    [SerializeField] GameObject orangeBack;
    [SerializeField] GameObject yellowBack;
    [SerializeField] GameObject blueBack;
    [SerializeField] GameObject goldCounter;
    //[SerializeField] TMP_Text goldCount;
    [SerializeField] UpMenuFadeAnimation upMenuFadeAnimation;
    [SerializeField] private string _popupPrefabsPath;
    [SerializeField] private Transform _popupLayout;
    List<PopupShowParams> popupsQueue = new List<PopupShowParams>();

    Action callback;

    private Dictionary<string, Popup> _createdPopups = new Dictionary<string, Popup>();

    public static PopupController Instance { get; private set; }

    public void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCamera(Camera camera)
    {
        _canvas.worldCamera = camera;
    }

    public void ShowPopup(string name, bool fade, bool goldCounter, string backName)
    {
        Popup popup = GetPopup(name);

        if (!popup)
        {
            CallbackInvoke();
            return;
        }
        if (popup)
        {
            this.goldCounter.SetActive(goldCounter);
            FadeActive(fade);
            BackActive(backName, true);

            foreach (KeyValuePair<string, Popup> otherCreatedPopup in _createdPopups)
            {
                if (otherCreatedPopup.Value.open & otherCreatedPopup.Value != popup)
                {
                    otherCreatedPopup.Value.Close(false);
                }
            }
            EventBus.OnUIPause?.Invoke();
            popup.Show();

        }

    }

    public Popup GetPopup(string name)
    {
        Popup popup;

        if (_createdPopups.ContainsKey(name)) popup = _createdPopups[name];
        else popup = CreatePopup(name);

        return popup;
    }

    private Popup CreatePopup(string name)
    {
        Popup popupPrefab = Resources.Load<Popup>(_popupPrefabsPath + name);
        if (!popupPrefab) return null;
        
        Popup newPopup = Instantiate(popupPrefab, _popupLayout.transform);
        newPopup.gameObject.name = name;
        newPopup.Init(this);
        _createdPopups.Add(name, newPopup);
        newPopup.gameObject.SetActive(false);

        return newPopup;
    }

    public void ShowPopup(string name, bool fade, bool goldCounter, Action callback, string backName)
    {
        this.callback = callback;
        ShowPopup(name, fade, goldCounter, "");
    }

    public Popup FindPopupByName(string name)
    {
        foreach (KeyValuePair<string, Popup> popup in _createdPopups)
        {
            if (popup.Value.name == name)
            {
                return popup.Value;
            }
        }
        Debug.LogError($"Cant find Popups - {name}");
        return null;
    }

    public void GoldCounterActive(bool active)
    {
        if (goldCounter == null) return;
        goldCounter.SetActive(active);
        //PersistentRoot.Instance.CoinsUpdate();
    }

    public void FadeActive(bool active)
    {
        fade.SetActive(active);
    }

    public void BackActive(string backName, bool back)
    {
        if (back) 
        {
            orangeBack.SetActive(false);
            yellowBack.SetActive(false);
            blueBack.SetActive(false);
            if (backName == "orange")
                orangeBack.SetActive(true);
            if (backName == "yellow")
                yellowBack.SetActive(true);
            if (backName == "blue")
                 blueBack.SetActive(true);
        }
        else
        {
            orangeBack.SetActive(false);
            yellowBack.SetActive(false);
            blueBack.SetActive(false);
        }
    }    

    public void AddPopupInQueue(PopupShowParams @params)
    {
        popupsQueue.Add(@params);
    }


    public void ShowQueuePopups()
    {
                goldCounter.SetActive(false);
        if (popupsQueue.Count > 0)
        {
            Popup popup = GetPopup(popupsQueue[0].name);            
            ShowPopup(popup.name, popupsQueue[0].fade, popupsQueue[0].goldCounter, popupsQueue[0].backName);
            popupsQueue.Remove(popupsQueue[0]);
        }
    }

    public void PopupClose()
    {
        bool allPopupCloses = true;
        foreach(KeyValuePair<string, Popup> popup in _createdPopups)
        {
            if (popup.Value.open)
            {
                allPopupCloses = false;
            }
        }

        if (allPopupCloses)
        {
            EventBus.OnUIResume?.Invoke();
            FadeActive(false);
            BackActive("", false);
            GoldCounterActive(false);
            if (upMenuFadeAnimation)
                upMenuFadeAnimation.Hide();
            if(popupsQueue.Count > 0)
            {
                ShowQueuePopups();
            }
            else
            {
                CallbackInvoke();
            }
            
        }
    }

    public void CallbackInvoke()
    {
        if (callback == null) return;
        Action callbackCopy = callback;
        callback = null;
        callbackCopy?.Invoke();
    }

    private void Update()
    {/*
        if (goldCount & goldCount.gameObject.active)
        {
            //goldCount.text = PlayerProfile.instance.coins.goldCount.ToString();
        }*/
    }


}

public class PopupShowParams
{
    public string name;
    public bool fade;
    public bool goldCounter;
    public string backName;
}
