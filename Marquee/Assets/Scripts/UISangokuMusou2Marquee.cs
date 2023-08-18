using System.Collections;
using TMPro;
using UnityEngine;

public class UISangokuMusou2Marquee : MonoBehaviour {
    private enum Step {
        FadeIn,
        HoldOn,
        FadeOut,
        Done,
    }

    #region Serialized Field
    [Header("UI")]
    [SerializeField] private UIGradientBackground _uiGraBG = null;
    [SerializeField] private TextMeshProUGUI _text = null;

    [Header("Options")]
    [SerializeField] private bool _isLoop = false;
    
    [SerializeField] private float _maxSlope;
    [SerializeField] private float _maxOffset;
    [SerializeField] private float _holdTime; // Seconds

    [SerializeField] private float _charProgressDelay = 3; // Progress delay than previous character
    [SerializeField] private float _progressPerFrame = 3;
    [SerializeField] private float _totalProgress = 60;
    #endregion

    #region Mono Behaviour Hooks
    private void OnDisable() {
        Stop();
    }
    #endregion

    #region APIs
    public void Play() {
        StopAllCoroutines();

        if (_text != null) {
            StartCoroutine(PlayTextAnimation());
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
    private IEnumerator PlayTextAnimation() {
        Step step = Step.FadeIn;
        float firstCharProgress = 0;
        bool stepFinished = false;

        TMP_TextInfo textInfo = _text.textInfo;
        string str = _text.text;
        str = str.TrimEnd();
        int finalValuableCharIndex = str.Length - 1; // Index of final character which is not empty or a space

        if (_uiGraBG != null) {
            _uiGraBG.Play();
            _uiGraBG.PlayFadeIn();
        }

        // Fade in => Hold on => Fade out => Done
        while (step != Step.Done) {
            // Reset every frame
            _text.ForceMeshUpdate();

            if (step == Step.HoldOn) {
                yield return new WaitForSeconds(_holdTime);

                stepFinished = true;
            }
            else {
                for (int charIndex = 0; charIndex < textInfo.characterCount; charIndex++) {
                    float charProgress = Mathf.Clamp(firstCharProgress - charIndex * _charProgressDelay, 0, _totalProgress);

                    // NOTE:
                    // If character is empty or a space
                    // there is no vertices info in "meshInfo.vertices"
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
                    if (!string.IsNullOrEmpty(charInfo.character.ToString()) && !string.IsNullOrWhiteSpace(charInfo.character.ToString())) {
                        float offset = 0;
                        float slope = 0;
                        float alpha = 0;

                        if (step == Step.FadeIn) {
                            offset = Mathf.Clamp((_totalProgress - charProgress) / _totalProgress, 0, 1) * _maxOffset;
                            slope = Mathf.Clamp((_totalProgress - charProgress) / _totalProgress, 0, 1) * _maxSlope;
                            alpha = Mathf.Clamp(charProgress / _totalProgress, 0, 1);
                        }
                        else {
                            offset = -Mathf.Clamp(charProgress / _totalProgress, 0, 1) * _maxOffset;
                            slope = -Mathf.Clamp(charProgress/ _totalProgress, 0, 1) * _maxSlope;
                            alpha = Mathf.Clamp((_totalProgress - charProgress) / _totalProgress, 0, 1);
                        }

                        // Vertex
                        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                        for (int j = 0; j < 4; j++) {
                            Vector3 oriVec = vertices[charInfo.vertexIndex + j];
                            if (j == 0 || j == 3) {
                                vertices[charInfo.vertexIndex + j] = oriVec + Vector3.right * offset;
                            }
                            else {
                                vertices[charInfo.vertexIndex + j] = oriVec + Vector3.right * (slope + offset);
                            }
                        }

                        // Color (Change alpha value only currently)
                        Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                        for (int j = 0; j < 4; j++) {
                            Color32 oriColor = colors[charInfo.vertexIndex + j];
                            Color newColor = new Color(oriColor.r, oriColor.g, oriColor.b, alpha);
                            colors[charInfo.vertexIndex + j] = newColor;
                        }
                    }

                    // Check is step finished
                    if (charIndex == finalValuableCharIndex && charProgress >= _totalProgress) {
                        stepFinished = true;
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                    TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    meshInfo.mesh.colors32 = meshInfo.colors32;
                    _text.UpdateGeometry(meshInfo.mesh, i);
                }

                firstCharProgress += _progressPerFrame;

                yield return new WaitForEndOfFrame();
            }

            // Check and change step
            if (stepFinished) {
                if (step == Step.FadeIn) {
                    step = Step.HoldOn;
                }
                else if (step == Step.HoldOn) {
                    step = Step.FadeOut;
                }
                else if (step == Step.FadeOut) {
                    if (_isLoop) {
                        step = Step.FadeIn;

                        yield return new WaitForSeconds(0.5f);
                    }
                    else {
                        step = Step.Done;
                    }
                }

                firstCharProgress = 0;
                stepFinished = false;
            }
        }

        if (_uiGraBG != null) {
            _uiGraBG.PlayFadeOut();
        }
    }
    #endregion
}
