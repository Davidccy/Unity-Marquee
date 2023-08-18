using System.Collections;
using TMPro;
using UnityEngine;

public class UISangokuMusou3Marquee : MonoBehaviour {
    private enum Step {
        FadeIn,
        HoldOn,
        FadeOut,
        Done,
    }

    #region Serialized Field
    [Header("UI")]
    [SerializeField] private UIGradientBackground _uiGraBG = null;
    [SerializeField] private RectTransform _rectBG = null;
    [SerializeField] private TextMeshProUGUI _text = null;

    [Header("Options")]
    [SerializeField] private bool _isLoop = false;
    [SerializeField] private float _durationEnter = 0.5f;
    [SerializeField] private float _waitEnter = 2.5f;
    [SerializeField] private float _durationLeave = 0.5f;
    [SerializeField] private float _waitLeave = 0.5f;
    #endregion

    #region Internal Fields
    private float _oriHeight = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _oriHeight = _rectBG.rect.height;
    }

    private void OnDisable() {
        Stop();
    }
    #endregion

    #region APIs
    public void Play() {
        StopAllCoroutines();

        if (_text != null) {
            StartCoroutine(PlayAnimation());
        }

        if (_uiGraBG != null) {
            _uiGraBG.Play();
        }
    }

    public void Stop() {
        StopAllCoroutines();
    }
    #endregion

    #region Internal Methods
    private IEnumerator PlayAnimation() {
        bool reverse = false;
        float progressTime = 0;
        float progress = 0;
        float wait = 0;

        while (_isLoop) {
            wait = 0;

            if (!reverse && progressTime >= _durationEnter) {
                progressTime = _durationEnter;
                wait = _waitEnter;
            }
            else if (reverse && progressTime >= _durationLeave) {
                progressTime = _durationLeave;
                wait = _waitLeave;
            }

            if (!reverse) {
                progress = _durationEnter > 0 ? progressTime / _durationEnter : 1;
            }
            else {
                progress = _durationLeave > 0 ? (1 - progressTime / _durationLeave) : 0;
            }
            _rectBG.sizeDelta = new Vector2(_rectBG.rect.width, _oriHeight * progress);

            if (progress >= 1) {
                reverse = true;
                progressTime = 0;
            }
            else if (progress <= 0) {
                reverse = false;
                progressTime = 0;
            }

            if (wait <= 0) {
                yield return new WaitForEndOfFrame();
            }
            else {
                yield return new WaitForSeconds(wait);
            }

            progressTime += Time.deltaTime;
        }
    }
    #endregion
}
