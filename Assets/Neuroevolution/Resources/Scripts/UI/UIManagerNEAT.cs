using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace NEAT
{
    [System.Serializable]
    public struct HiddenLayer
    {
        public List<GameObject> hiddenNodes;
    }

    public class UIManagerNEAT : MonoBehaviour
    {
        public static UIManagerNEAT Instance { get; private set; }

        [Header("Time")]
        public Slider timeSlider;
        private bool _isTimeStopped;

        [Header("ANN Stats")]
        public TextMeshProUGUI currentFitness;
        public TextMeshProUGUI currentGenome;
        public TextMeshProUGUI currentSpecies;
        public TextMeshProUGUI speciesCount;
        public TextMeshProUGUI currentGeneration;
        public TextMeshProUGUI maxFitness;
        private float maxFit;

        [Header("ANN Serialisation")]
        public string directoryName;
        public Button saveBtn;
        public Button loadBtn;
        public TMP_InputField inputField;
        public string inputFieldPreLoadedText;
        public bool autoSaveBest;

        [Header("Misc.")]
        public Button resetBtn;
        public Button startBtn;

        [Header("ANN Model")]
        public GameObject annPanel;
        public GameObject layerPrefab;
        public GameObject nodePrefab;
        public Genome currentModel;
        public List<GameObject> inputNodes;
        public List<HiddenLayer> hiddenLayers;
        public List<GameObject> outputNodes;

        private void Awake() {
            Instance = this;

            timeSlider.value = 0;
            directoryName += "/";

            if (inputField != null)
                inputField.text = inputFieldPreLoadedText;

            maxFitness.text = 0.ToString();
            UpdateFitnessUI(0);
        }

        private void Start() {
            if (saveBtn != null)
                saveBtn.onClick.AddListener(() => NEAT.Instance.SaveModel(directoryName));
            if (loadBtn != null)
                loadBtn.onClick.AddListener(() => NEAT.Instance.LoadModel(inputField.text, directoryName));
            if (resetBtn != null)
                resetBtn.onClick.AddListener(() => {
                    GameManager.Instance.saveStateNumber = 0;
                    Application.LoadLevel(Application.loadedLevel);
                });
            if(startBtn != null) {
                startBtn.onClick.AddListener(() => TimeManager.Instance.AIFastSpeed());
            }
        }

        private void Update() {
            if (!_isTimeStopped)
                Time.timeScale = timeSlider.value;
        }

        public void StopTime() {
            _isTimeStopped = true;
        }

        public void UpdateFitnessUI(float value) {
            currentFitness.text = value + "";
        }

        public void UpdateMaxFitnessUI(Genome genome) {
            if (genome.Fitness >= maxFit) {
                maxFit = genome.Fitness;
                maxFitness.text = genome.Fitness.ToString();

                if (autoSaveBest && genome.Fitness > 0)
                    NEAT.Instance.AutoSaveModel(genome, directoryName);
            }
        }

        public void UpdateCurrentSpecies(int id) {
            if (id == -1)
                currentSpecies.text = "N/A";
            else
                currentSpecies.text = id.ToString();
        }

        public void UpdateSpeciesCount(int count) {
            speciesCount.text = count.ToString();
        }

        public void UpdateCurrentGeneration(int num) {
            num++;
            currentGeneration.text = num.ToString();
        }

        public void UpdateCurrentGenome(int populationNum) {
            populationNum++;
            currentGenome.text = populationNum.ToString();
        }

        public void BuildNN(Genome genome) {
            ClearNN();

            currentModel = genome;

            inputNodes = BuildLayer(Layer.Input, 0);

            int hiddenLayerCount = 0;
            HashSet<int> hashSet = new HashSet<int>();
            foreach (NodeGene nodeGene in genome.NodeGenes) {
                if (!hashSet.Contains(nodeGene.Order) && nodeGene.Layer.Equals(Layer.Hidden)) {
                    hashSet.Add(nodeGene.Order);
                    hiddenLayerCount++;
                }
            }

            for (int i = 0; i < hiddenLayerCount; i++) {
                HiddenLayer hiddenLayer = new HiddenLayer();
                hiddenLayer.hiddenNodes = BuildLayer(Layer.Hidden, (i + 1));
                hiddenLayers.Add(hiddenLayer);
            }

            outputNodes = BuildLayer(Layer.Output, hiddenLayerCount + 1);
        }

        private List<GameObject> BuildLayer(Layer layer, int order) {
            List<GameObject> nodeList = new List<GameObject>();

            GameObject layerGO = Instantiate(layerPrefab);
            layerGO.transform.SetParent(annPanel.transform, false);
            layerGO.name = layer.ToString() + " Layer " + order;

            if (layer.Equals(Layer.Hidden)) {
                layerGO.GetComponent<VerticalLayoutGroup>().spacing = 25;
            }

            foreach (NodeGene ng in currentModel.NodeGenes) {
                if (ng.Layer.Equals(layer) && ng.Order == order) {
                    GameObject nodeGO = Instantiate(nodePrefab);
                    nodeGO.transform.SetParent(layerGO.transform, false);
                    nodeGO.name = ng.Id.ToString();
                    if (ng.Id == 0)
                        nodeGO.GetComponent<Image>().color = Color.cyan;
                    nodeList.Add(nodeGO);

                    NodeGenomePrefab nodeGenomePrefab = nodeGO.GetComponent<NodeGenomePrefab>();
                    nodeGenomePrefab.idField.text = ng.Id.ToString();
                    nodeGenomePrefab.idField.gameObject.SetActive(true);
                }
            }

            return nodeList;
        }

        private void ClearNN() {
            foreach (Transform child in annPanel.transform) {
                Destroy(child.gameObject);
            }

            inputNodes.Clear();

            foreach (HiddenLayer hiddenLayer in hiddenLayers) {
                hiddenLayer.hiddenNodes.Clear();
            }

            hiddenLayers.Clear();

            outputNodes.Clear();
        }

        private void OnDrawGizmos() {
            if (currentModel != null) {
                if (ConfigNEAT.NETWORK_TYPE.Equals(NetworkType.Feedforwad))
                    DrawFFNN();
                else if (ConfigNEAT.NETWORK_TYPE.Equals(NetworkType.Recurrent))
                    DrawRNN();
            }
        }

        private void DrawFFNN() {
            foreach (ConnectionGene cg in currentModel.ConnectionGenes) {

                if (!cg.IsEnabled)
                    continue;

                GameObject inNodeGO = null;
                GameObject outNodeGO = null;

                foreach (GameObject go in inputNodes) {
                    if (cg.InNode.Id.ToString().Equals(go.name)) {
                        inNodeGO = go;
                    }

                    for (int i = 0; i < hiddenLayers.Count; i++) {
                        foreach (GameObject go2 in hiddenLayers[i].hiddenNodes) {
                            if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                                outNodeGO = go2;
                            }
                        }
                    }

                    foreach (GameObject go2 in outputNodes) {
                        if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                            outNodeGO = go2;
                        }
                    }
                }

                for (int i = 0; i < hiddenLayers.Count; i++) {
                    foreach (GameObject go in hiddenLayers[i].hiddenNodes) {
                        if (cg.InNode.Id.ToString().Equals(go.name)) {
                            inNodeGO = go;
                        }

                        for (int j = 0; j < hiddenLayers.Count; j++) {
                            foreach (GameObject go2 in hiddenLayers[j].hiddenNodes) {
                                if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                                    outNodeGO = go2;
                                }
                            }
                        }

                        foreach (GameObject go3 in outputNodes) {
                            if (cg.OutNode.Id.ToString().Equals(go3.name)) {
                                outNodeGO = go3;
                            }
                        }
                    }
                }

                if (inNodeGO == null || outNodeGO == null) {
                    currentModel.DebugNodeGenesLog();
                    currentModel.DebugConnectionsGenesLog();
                    Debug.LogError("Can not draw ANN; Cause of Error Connection: " + cg.InNode.Id + " -> " + cg.OutNode.Id + " Innov (" + cg.InnovNr + ") [InNodeGO: " + (inNodeGO == null ? "null" : "Exists") + "] " + " [OutNodeGo: " + (outNodeGO == null ? "null" : "Exists") + "]");
                }
                Gizmos.DrawLine(inNodeGO.transform.position, outNodeGO.transform.position);
            }
        }

        private void DrawRNN() {
            foreach (ConnectionGene cg in currentModel.ConnectionGenes) {

                if (!cg.IsEnabled)
                    continue;

                if (cg.IsRecurrent)
                    Gizmos.color = Color.magenta;
                else
                    Gizmos.color = Color.white;

                GameObject inNodeGO = null;
                GameObject outNodeGO = null;

                foreach (GameObject go in inputNodes) {
                    if (cg.InNode.Id.ToString().Equals(go.name)) {
                        inNodeGO = go;
                    }

                    foreach (GameObject go2 in inputNodes) {
                        if (cg.OutNode.Id.ToString().Equals(go2.name))
                            outNodeGO = go2;
                    }

                    for (int i = 0; i < hiddenLayers.Count; i++) {
                        foreach (GameObject go2 in hiddenLayers[i].hiddenNodes) {
                            if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                                outNodeGO = go2;
                            }
                        }
                    }

                    foreach (GameObject go2 in outputNodes) {
                        if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                            outNodeGO = go2;
                        }
                    }
                }

                for (int i = 0; i < hiddenLayers.Count; i++) {
                    foreach (GameObject go in hiddenLayers[i].hiddenNodes) {
                        if (cg.InNode.Id.ToString().Equals(go.name)) {
                            inNodeGO = go;
                        }

                        foreach (GameObject go2 in inputNodes) {
                            if (cg.OutNode.Id.ToString().Equals(go2.name))
                                outNodeGO = go2;
                        }

                        for (int j = 0; j < hiddenLayers.Count; j++) {
                            foreach (GameObject go2 in hiddenLayers[j].hiddenNodes) {
                                if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                                    outNodeGO = go2;
                                }
                            }
                        }

                        foreach (GameObject go2 in outputNodes) {
                            if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                                outNodeGO = go2;
                            }
                        }
                    }
                }

                foreach (GameObject go in outputNodes) {
                    if (cg.InNode.Id.ToString().Equals(go.name)) {
                        inNodeGO = go;
                    }

                    foreach (GameObject go2 in inputNodes) {
                        if (cg.OutNode.Id.ToString().Equals(go2.name))
                            outNodeGO = go2;
                    }

                    for (int i = 0; i < hiddenLayers.Count; i++) {
                        foreach (GameObject go2 in hiddenLayers[i].hiddenNodes) {
                            if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                                outNodeGO = go2;
                            }
                        }
                    }

                    foreach (GameObject go2 in outputNodes) {
                        if (cg.OutNode.Id.ToString().Equals(go2.name)) {
                            outNodeGO = go2;
                        }
                    }
                }

                if (inNodeGO == null || outNodeGO == null) {
                    currentModel.DebugNodeGenesLog();
                    currentModel.DebugConnectionsGenesLog();
                    Debug.LogError("Can not draw ANN; Cause of Error Connection: " + cg.InNode.Id + " -> " + cg.OutNode.Id + " Innov (" + cg.InnovNr + ") [InNodeGO: " + (inNodeGO == null ? "null" : "Exists") + "] " + " [OutNodeGo: " + (outNodeGO == null ? "null" : "Exists") + "]");
                }

                if (inNodeGO == outNodeGO)
                    Gizmos.DrawWireSphere(inNodeGO.transform.position, 0.1f);
                else {
                    if (cg.IsRecurrent)
                        Gizmos.DrawLine(new Vector2(inNodeGO.transform.position.x, inNodeGO.transform.position.y + 0.05f), new Vector2(outNodeGO.transform.position.x, outNodeGO.transform.position.y + 0.05f));
                    else
                        Gizmos.DrawLine(inNodeGO.transform.position, outNodeGO.transform.position);
                }
            }
        }
    }
}