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
    [SerializeField] private bool _isLoop = false;
    [SerializeField] private TextMeshProUGUI _text = null;
    [SerializeField] private float _maxSlope;
    [SerializeField] private float _maxOffset;
    [SerializeField] private float _holdTime; // Seconds
    [SerializeField] private UIGradientBackground _uiGraBG = null;
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
        float progressPerFrame = 0.05f;

        TMP_TextInfo textInfo = _text.textInfo;
        int charCount = textInfo.characterCount;

        // NOTE:
        // Progress of a character has 1 frame delay than previous character

        bool stepFinished = false;

        // Fade in => Hold on => Fade out => Done
        while (step != Step.Done) {
            // Reset every time
            _text.ForceMeshUpdate();

            if (step == Step.HoldOn) {
                yield return new WaitForSeconds(_holdTime);

                stepFinished = true;
            }
            else {
                for (int i = 0; i < textInfo.characterCount; i++) {
                    int charIndex = i;
                    float charProgress = firstCharProgress - charIndex * progressPerFrame * 1.5f;

                    float offset = 0;
                    float slope = 0;
                    float alpha = 0;
                    if (step == Step.FadeIn) {
                        offset = Mathf.Clamp(1 - charProgress, 0, 1) * _maxOffset;
                        slope = Mathf.Clamp(1 - charProgress, 0, 1) * _maxSlope;
                        alpha = Mathf.Clamp(charProgress, 0, 1);
                    }
                    else {
                        offset = -Mathf.Clamp(charProgress, 0, 1) * _maxOffset;
                        slope = -Mathf.Clamp(charProgress, 0, 1) * _maxSlope;
                        alpha = Mathf.Clamp(1 - charProgress, 0, 1);
                    }

                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    //if (!charInfo.isVisible) {
                    //    continue;
                    //}

                    if (charIndex == textInfo.characterCount - 1 && charProgress >= 1) {
                        stepFinished = true;
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

                for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                    TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    meshInfo.mesh.colors32 = meshInfo.colors32;
                    _text.UpdateGeometry(meshInfo.mesh, i);
                }

                firstCharProgress += progressPerFrame;

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

        _text.ForceMeshUpdate();
    }
    #endregion
}
