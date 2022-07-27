using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIResearchManager : MonoBehaviour
{
    public static AIResearchManager Instance { get; private set; }

    public List<ResearchSO> ResearchSOList;

    private void Awake() {
        Instance = this;
    }
}
