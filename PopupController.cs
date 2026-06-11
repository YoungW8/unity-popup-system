using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _fade;
    [SerializeField] private GameObject _goldCounter;
    [SerializeField] private string _popupPrefabsPath;
    [SerializeField] private Transform _popupLayout;
    List<PopupShowParams> _popupsQueue = new List<PopupShowParams>();

    private Dictionary<string, Popup> _createdPopups = new Dictionary<string, Popup>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    //if canvas space - camera, set camera on change scene
    public void SetCamera(Camera camera)
    {
        _canvas.worldCamera = camera;
    }

    public void ShowPopup(string name, Action onPopupClose = null)
    {
        Popup popup = GetPopup(name);

        if (!popup) return;

        _goldCounter.SetActive(popup.GoldCounter);
        FadeActive(popup.Fade);

        foreach (KeyValuePair<string, Popup> otherCreatedPopup in _createdPopups)
        {
            if (otherCreatedPopup.Value.open & otherCreatedPopup.Value != popup)
            {
                otherCreatedPopup.Value.Close();
            }
        }
        popup.Show(onPopupClose);

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

    public void FadeActive(bool active)
    {
        _fade.SetActive(active);
    }   

    public void AddPopupInQueue(PopupShowParams parameters)
    {
        _popupsQueue.Add(parameters);
    }


    public void ShowQueuePopups()
    {
        _goldCounter.SetActive(false);
        while (_popupsQueue.Count > 0)
        {
            PopupShowParams next = _popupsQueue[0];
            _popupsQueue.RemoveAt(0);

            if (GetPopup(next.name))
            {
                ShowPopup(next.name, next.onCloseAction);
                return;
            }

            Debug.LogError($"Popup prefab not found, skipping - {next.name}");
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
            FadeActive(false);
            _goldCounter.SetActive(false);
            if (_popupsQueue.Count > 0)
            {
                ShowQueuePopups();
            }
            
        }
    }


}

public class PopupShowParams
{
    public string name;
    public Action onCloseAction;
}
