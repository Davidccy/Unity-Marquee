using UnityEngine;
using UnityEngine.UI;

public class UISample : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btn = null;
    [SerializeField] private UIPADMarquee _marqueePADA = null;
    [SerializeField] private UIPADMarquee _marqueePADB = null;
    [SerializeField] private UISangokuMusou2Marquee _marqueeSangokuMusou2A = null;
    [SerializeField] private UISangokuMusou2Marquee _marqueeSangokuMusou2B = null;
    [SerializeField] private UISangokuMusou2Marquee _marqueeSangokuMusou2C = null;
    [SerializeField] private UISangokuMusou2Marquee _marqueeSangokuMusou2D = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }

    private void OnDestroy() {
        _btn.onClick.RemoveAllListeners();
    }
    #endregion

    #region Button Handlings
    private void ButtonOnClick() {
        _marqueePADA.Play();
        _marqueePADB.Play();

        _marqueeSangokuMusou2A.Play();
        _marqueeSangokuMusou2B.Play();
        _marqueeSangokuMusou2C.Play();
        _marqueeSangokuMusou2D.Play();
    }
    #endregion
}
