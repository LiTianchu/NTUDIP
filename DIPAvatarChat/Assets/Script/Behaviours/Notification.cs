using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Notification : PageSingleton<Notification>
{
    [SerializeField]
    private float timeout = 2f;
    [SerializeField]
    private float fadeTime = 0.5f;

    private CanvasGroup _cg;
    private RectTransform _rect;

    private void Start()
    {
        if (_cg == null)
        {
            _cg = GetComponent<CanvasGroup>();
        }
        if (_rect == null)
        {
            _rect = GetComponent<RectTransform>();
        }
        StartCoroutine(NotifyRoutine());
    }


    IEnumerator NotifyRoutine()
    {
        UIManager.Instance.PanelFadeIn(_cg, fadeTime, UIManager.UIMoveDir.Stay, _rect.anchoredPosition);
        yield return new WaitForSeconds(timeout);
        UIManager.Instance.PanelFadeOut(_cg, fadeTime, UIManager.UIMoveDir.Stay, _rect.anchoredPosition);
        yield return new WaitForSeconds(fadeTime);
        Destroy(this.gameObject);
    }
}
