using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance { get => _instance; }

    [NonSerialized] public bool mouseOverUI;
    [SerializeField] private List<DynamicCanvas> interfaces;

    private bool isKeyPressed;

    public bool inputDisable;

    private void Awake() {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        interfaces = new List<DynamicCanvas>();
        UILayer = LayerMask.NameToLayer("UI");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)  && TechnicalManager.Instance.debugScreenshot)
            ScreenCapture.CaptureScreenshot("C:/Users/rahmi/Desktop/" + UnityEngine.Random.Range(0, 10000) + ".png");
        if (Input.GetKeyDown(KeyCode.F5) && TechnicalManager.Instance.debugReset)
            ReloadScene();


        if (Input.GetKeyDown(KeyCode.Escape) && !isKeyPressed) {
            HideLastInterface();
            isKeyPressed = true;
        }else if (Input.GetKeyDown(KeyCode.F1) && !isKeyPressed && !inputDisable) {
            ShowResearchUI();
            isKeyPressed = true;
        } else if (Input.GetKeyDown(KeyCode.F2) && !isKeyPressed && !inputDisable) {
            ShowGlobalUI();
            isKeyPressed = true;
        } else {
            isKeyPressed = false;
        }

        mouseOverUI = IsPointerOverUIElement();
    }

    public void ReloadScene() {
        SceneManager.LoadScene("scene_game");
    }

    public static bool IsMouseOverUI() {
        return _instance.mouseOverUI;
    }

    public void AddInterface(DynamicCanvas panel) {
        if (!interfaces.Contains(panel))
            interfaces.Add(panel);
    }

    private void HideLastInterface() {
        if (interfaces.Count == 0) {
            if (MenuCanvas.Instance != null)
                ShowMenuUI();
        } else {
            interfaces[interfaces.Count - 1].HidePanel();
        }
    }

    public void RemoveInterfaceFromList(DynamicCanvas dynamicInterface) {
        for (int i = 0; i < interfaces.Count; i++) {
            if (interfaces[i] == dynamicInterface) {
                interfaces.RemoveAt(i);
                return;
            }
        }
    }

    public void ShowMenuUI() {
        if (MenuCanvas.Instance.menuPanel.activeInHierarchy) {
            MenuCanvas.Instance.HidePanel();
        } else {
            MenuCanvas.Instance.ShowPanel();
        }
    }

    public void ShowResearchUI() {
        if (ResearchCanvas.Instance.researchPanel.activeInHierarchy) {
            ResearchCanvas.Instance.HidePanel();
        } else {
            ResearchCanvas.Instance.ShowPanel();
        }
    }

    public void ShowGlobalUI() {
        if (GlobalCanvas.Instance.globalPanel.activeInHierarchy)
            GlobalCanvas.Instance.HidePanel();
        else
            GlobalCanvas.Instance.ShowPanel();
    }

    #region Animations
    public Tween MoveAnchorPosition(RectTransform UIElement, Vector2 endValue, float duration = 0.5f) {
        return DOTween.To(() => UIElement.anchoredPosition, x => UIElement.anchoredPosition = x, endValue, duration);
    }

    public Tween PunchScale(Transform transform, Vector2 punch, float duration = 0.2f) {
        return transform.DOPunchScale(punch, duration);
    }

    #endregion

    #region Mouse Over UI
    int UILayer;
    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults() {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    #endregion

}
