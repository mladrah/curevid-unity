using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationManager : MonoBehaviour
{
    #region UI Components
    private static ConfirmationManager instance;
    [Header("Content Box")]
    public Confirmation confirmation;
    #endregion

    private void Awake(){
        instance = this;
    }

    private void Start() {
        confirmation.gameObject.SetActive(false);
    }

    #region DynamicContentBox Behaviour
    public static void Show(string header, string content, string modifier = "") {
        instance.confirmation.rectTransform.localPosition = new Vector3(0, 0, 0);
        instance.confirmation.SetText(header, content, modifier);
        instance.confirmation.gameObject.SetActive(true);
    }

    public static void Hide() {
        instance.confirmation.gameObject.SetActive(false);
        instance.confirmation.Reset();
    }

    public static Confirmation.Result GetResult() {
        return instance.confirmation.result;
    }
    #endregion
}
