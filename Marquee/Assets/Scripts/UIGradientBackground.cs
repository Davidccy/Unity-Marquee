using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIGradientBackground : MonoBehaviour {
    // NOTE:
    // Loop type = ping pong

    #region Serialized Fields
    [SerializeField] private bool _playWhenEnable = false;
    [SerializeField] private Image _imageBG = null;
    [SerializeField] private float _loopTime = 0;
    [SerializeField] private AnimationCurve _aniCurve = default;
    [SerializeField] private Color _colorFrom = Color.white;
    [SerializeField] private Color _colorTo = Color.white;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        if (_playWhenEnable) {
            Play();
        }
    }

    private void OnDisable() {
        Stop();
    }
    #endregion

    #region APIs
    public void Play() {
        StopAllCoroutines();
        StartCoroutine(PlayAnimation());
    }

    public void Stop() {
        StopAllCoroutines();
    }
    #endregion

    #region Internal Methods
    private IEnumerator PlayAnimation() {
        float passedTime = 0;
        bool reverse = false;
        float loopTime = 0;
        float progress = 0;
        float curveValue = 0;
        while (true) {
            loopTime = Math.Max(0.05f, _loopTime);

            if (!reverse && passedTime >= loopTime) {
                reverse = true;
                passedTime = loopTime;
            }
            else if (reverse && passedTime <= 0) {
                reverse = false;
                passedTime = 0;
            }

            progress = passedTime / loopTime;
            curveValue = _aniCurve.Evaluate(progress);

            Color c = Color.Lerp(_colorFrom, _colorTo, curveValue);
            _imageBG.color = c;

            yield return new WaitForEndOfFrame();

            passedTime += reverse ? -Time.deltaTime : Time.deltaTime;
        }
    }
    #endregion
}
