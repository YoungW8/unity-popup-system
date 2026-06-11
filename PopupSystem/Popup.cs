using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Popup : MonoBehaviour
{
    [SerializeField] bool showVibration = true;
    [SerializeField] public bool hideMenuElements = true;
    ShowAnim showAnim;
    UIA_Bubble bubble;
    [SerializeField] protected Button coreBtn;
    [SerializeField] protected Button additionalBtn;
    [SerializeField] protected Button closeBtn;
    [SerializeField] bool goldCounter = true;
    [SerializeField] bool fade = true;
    protected PopupController popupController;
    [Space]
    [Header("Sounds")]
    [SerializeField] public string openSound = "Green_Button";
    [SerializeField] public string closeSound = "Green_Button";
    [SerializeField] public string additionalSound = "Green_Button";

    public bool open { get; private set; }

    public bool GoldCouter => goldCounter;
    public bool Fade => fade;

    protected void Start()
    {
        if (coreBtn)
        {
            coreBtn.onClick.AddListener(CoreBtn);
        }

        if (additionalBtn)
        {
            additionalBtn.onClick.AddListener(AdditionalBtn);
        }

        if (closeBtn)
        {
            closeBtn.onClick.AddListener(CloseBtn);
        }

    }

    public void Init(PopupController popupController)
    {
        this.popupController = popupController;
    }

    public void Show()
    {
        if (ShowRequierment())
        {
            if (showVibration)
            {
                //VibrationController.instance.Selection();
            }
            open = true;
            OpenSoundPlay();

            if (showAnim == null)
            {
                //showAnim = GetComponent<ShowAnim>();
            }

            //Coroutine showAnimCoroutine = showAnim.Show();
            gameObject.SetActive(true);
            ShowEvent();
            ChangeValues();
            


        }
        else
        {
            popupController.PopupClose();
        }


    }

    public Coroutine Close(bool hideFade)
    {
        open = false;
        
        if(gameObject.active)
            return StartCoroutine(closeCoroutine(hideFade));
        else
        {
            CloseEvent();
            return null;
        }

    }

    public IEnumerator closeCoroutine(bool hideFade)
    {

        if (showAnim == null)
        {
            showAnim = GetComponent<ShowAnim>();
        }

        if (bubble == null)
        {
            bubble = GetComponent<UIA_Bubble>();
        }

        if (bubble != null)
        {
            yield return bubble.StartAnimation();
        }

        CloseEvent();

        /*if (hideFade)
        {
            popupController.FadeActive(false);
        }*/

        CloseSoundPlay();
        popupController.PopupClose();
        gameObject.SetActive(false);               
        yield return null;
    }    

    public void OpenSoundPlay()
    {
        GameSounds.instance.Play(openSound);
        //VibrationController.instance.MediumImpact();
    }

    public void CloseSoundPlay()
    {
        GameSounds.instance.Play(closeSound);
        //VibrationController.instance.MediumImpact();
    }

    public void AdditionalSoundPlay()
    {
        GameSounds.instance.Play(additionalSound);
        //VibrationController.instance.MediumImpact();
    }

    

    public abstract bool ShowRequierment();
    protected abstract void ChangeValues();
    protected abstract void ShowEvent();
    protected abstract void CloseEvent();
    protected abstract void CoreBtn();
    protected abstract void AdditionalBtn();
    protected abstract void CloseBtn();
}

