using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpMenuFadeAnimation : MonoBehaviour
{
    [SerializeField] float showTime;
    [SerializeField] float hideTime;
    [SerializeField] Vector3 direction;
    Vector3 startPosisition;
    Coroutine coroutine;
    private void Start()
    {
        startPosisition = transform.position;
    }

    public void Show()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        Vector3 targetPos = startPosisition + direction;
        float startTime = 0;
        Vector3 startPos = transform.position;

        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while(startTime < showTime)
        {
            startTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, startTime / showTime);
            yield return waitForFixedUpdate;
        }

        coroutine = null;
    }

    public void Hide()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(HideCoroutine());
    }

    IEnumerator HideCoroutine()
    {
        Vector3 targetPos = startPosisition;
        float startTime = 0;
        Vector3 startPos = transform.position;

        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (startTime < hideTime)
        {
            startTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, startTime / hideTime);
            yield return waitForFixedUpdate;
        }

        coroutine = null;
    }
}
