using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPADMarquee : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rtRoot = null;
    [SerializeField] private RectTransform _rtText = null;

    [SerializeField] private float _speedEnter = 10;
    [SerializeField] private float _speedLeave = 2;
    [SerializeField] private float _delay = 2f; // Seconds
    [SerializeField] private float _bufferSpace = 20;
    #endregion

    #region APIs
    public void Play() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rtRoot);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rtText);

        float widthRoot = _rtRoot.rect.size.x;
        float widthText = _rtText.rect.size.x;

        StopAllCoroutines();
        if (widthText <= widthRoot) {
            // Show fixed position
            _rtText.anchoredPosition = Vector2.zero;
        }
        else {
            // Show animation
            StartCoroutine(ShowAnimation());
        }
    }
    #endregion

    #region Internal Methods
    private IEnumerator ShowAnimation() {
        while (true) {
            // Step 1
            _rtText.anchoredPosition = new Vector2(_rtRoot.rect.size.x, 0);

            // Step 2
            while (_rtText.anchoredPosition.x > 0) {
                _rtText.anchoredPosition = new Vector2(_rtText.anchoredPosition.x - _speedEnter, 0);
                yield return new WaitForEndOfFrame();
            }

            // Step 3
            _rtText.anchoredPosition = Vector2.zero;
            yield return new WaitForSeconds(_delay);

            // Step 4
            while (_rtText.anchoredPosition.x > -_rtText.rect.size.x - _bufferSpace) {
                _rtText.anchoredPosition = new Vector2(_rtText.anchoredPosition.x - _speedLeave, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    #endregion
}
