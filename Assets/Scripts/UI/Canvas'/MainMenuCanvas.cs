using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuCanvas : MonoBehaviour
{
    public Button continueBtn;
    private MenuButton contineMenuBtn;

    [Header("Image Animation")]
    public RectTransform menuImage;
    public Ease ease;
    public float duration = 0.5f;

    private void Awake() {
        contineMenuBtn = continueBtn.GetComponent<MenuButton>();
    }

    private void Start() {
        Vector2 imageInitPos = menuImage.anchoredPosition;

        Sequence seq = DOTween.Sequence();

        seq.
            Append(UIManager.Instance.MoveAnchorPosition(menuImage, new Vector2(imageInitPos.x, imageInitPos.y + 25), duration).SetEase(ease)).
            Append(UIManager.Instance.MoveAnchorPosition(menuImage, new Vector2(imageInitPos.x, imageInitPos.y), duration).SetEase(ease)).

        SetLoops(-1);
    }

    public void Update() {
        CheckContinueState();
    }

    private void CheckContinueState() {
        if (SerialisationCanvas.Instance.saveFileList != null) {
            if (SerialisationCanvas.Instance.saveFileList.Count > 0)
                continueBtn.interactable = true;
            else
                continueBtn.interactable = false;

            contineMenuBtn.OnInteractableChange();
        }
    }
}
