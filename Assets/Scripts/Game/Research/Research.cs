using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Research : MonoBehaviour, IPointerClickHandler
{
    #region Independent Properties
    [SerializeField]
    private bool _canBeResearched;
    public bool CanBeResearched { get => _canBeResearched; set => _canBeResearched = value; }

    [SerializeField]
    private bool _isResearched;
    public bool IsResearched { get => _isResearched; set => _isResearched = value; }

    [SerializeField]
    private bool _isResearching;
    public bool IsResearching { get => _isResearching; set => _isResearching = value; }

    [SerializeField]
    private List<Research> _siblings;
    public List<Research> Siblings { get => _siblings; }

    [SerializeField]
    private List<Research> _parents;
    public List<Research> Parents { get => _parents; }

    [SerializeField]
    private List<Research> _childs;
    public List<Research> Childs { get => _childs; }
    #endregion

    #region SO Properties
    [SerializeField] private ResearchSO _reference;
    public string Name { get => _reference.Name; }
    public string Description { get => _reference.Description; }
    public int Duration { get => _reference.Duration; }
    public float Cost { get => _reference.Cost; }
    public float DailyCost { get => _reference.DailyCost; }
    public bool IsMandatory { get => _reference.IsMandatory; }
    public List<Modifier> Modifiers { get => _reference.Modifiers; }
    public Sprite Icon { get => _reference.Icon; }
    #endregion

    #region UI Fields
    private Image panel;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject lines;
    #endregion

    #region UI Components
    private RectTransform rect;
    #endregion

    #region Animation Fields
    [Header("Animation")]
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private float colorTweenDuration;
    [SerializeField] private Ease colorTweenEase;
    private List<Tween> colorTweens;
    #endregion

    private void Awake() {
        panel = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        icon.sprite = _reference.Icon;
    }

    private void Start() {
        InitFamilies();
        UpdateUI();
    }

    private void InitFamilies() {
        foreach (Research rp in _parents) {
            if (!rp.Childs.Contains(this))
                rp.Childs.Add(this);
        }

        foreach (Research rp in _siblings) {
            if (!rp.Siblings.Contains(this))
                rp.Siblings.Add(this);
        }

        if (_childs.Count == 0) {
            _canBeResearched = true;
        } else
            _canBeResearched = false;
    }

    public void UpdateUI() {
        ResearchableUI();

        if (!_canBeResearched) {
            UnresearchableUI();
        } else if (_isResearching) {
            ResearchingUI();
        } else if (_isResearched) {
            ResearchedUI();
        }
    }

    #region State
    private void ResearchableUI() {
        if (colorTweens != null) {
            foreach (Tween t in colorTweens)
                t.Kill();
        }

        icon.color = Colors.BLUE_2;
        panel.color = Colors.WHITE;
        Color col = Colors.WHITE;
        ColorizeLines(new Color(col.r - 0.5f, col.g - 0.5f, col.r - 0.5f, 1));
    }

    public void UnresearchableUI() {
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0.25f);
        Color col = Colors.WHITE;
        panel.color = col;
        ColorizeLines(new Color(col.r - 0.5f, col.g - 0.5f, col.r - 0.5f, 1));
    }

    public void ResearchingUI() {
        colorTweens = new List<Tween>();
        panel.color = color1;
        icon.color = Colors.WHITE;
        colorTweens.Add(panel.DOColor(color2, colorTweenDuration).SetLoops(-1, LoopType.Yoyo));
        ColorizeLinesTween();
    }

    public void ResearchedUI() {
        Color col = Colors.BLUE;
        panel.color = col;
        ColorizeLines(col);
    }

    private void ColorizeLines(Color col) {
        foreach (Transform child in lines.transform) {
            Image img = child.gameObject.GetComponent<Image>();
            img.color = col;
        }
    }

    public void ColorizeLinesTween() {
        foreach (Transform child in lines.transform) {
            Image img = child.gameObject.GetComponent<Image>();
            img.color = color1;
            colorTweens.Add(img.DOColor(color2, colorTweenDuration).SetLoops(-1, LoopType.Yoyo));
        }
    }
    #endregion

    #region OnClick Behaviour
    public void OnPointerClick(PointerEventData eventData) {
        ResearchCanvas.Instance.UpdateFocusedResearch(this);
    }
    #endregion

    #region Debug
    private void OnDrawGizmos() {
        if (_reference == null || _parents == null)
            return;

        Gizmos.color = _reference.IsMandatory ? Color.red : Color.green;
        foreach (Research rp in _parents) {
            if (rp != null)
                Gizmos.DrawLine(transform.position, new Vector3(rp.transform.position.x, rp.transform.position.y - 20, rp.transform.position.z));
        }

        Gizmos.color = Color.magenta;
        foreach (Research rp in _childs) {
            if (rp != null)
                Gizmos.DrawLine(transform.position, new Vector3(rp.transform.position.x, rp.transform.position.y, rp.transform.position.z));
        }
    }
    #endregion

    #region Serialisation
    public void LoadFromSaveData(SaveData.ResearchData researchData) {
        _canBeResearched = researchData.canBeResearched;
        _isResearching = researchData.isResearching;
        _isResearched = researchData.isResearched;

        UpdateUI();
    }
    #endregion

    #region AI API
    public void SetResearchSO(ResearchSO reference) {
        _reference = reference;
    }
    #endregion
}
