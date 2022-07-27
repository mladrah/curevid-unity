using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;
using UnityEngine.UI;
using TMPro;

namespace Test.XOR
{
    public class XORAgent : Agent
    {
        int counts = 1;

        int attempts;
        int correctAttempt;

        public bool randomEva;
        public bool fixedEva;
        public TextMeshProUGUI processField;
        List<string> choices;

        public GameObject configRandom;
        public GameObject configFixed;

        public override void Awake() {
            base.Awake();

            configRandom.SetActive(false);
            configFixed.SetActive(false);

            if (randomEva)
                configRandom.SetActive(true);

            if (fixedEva)
                configFixed.SetActive(true);
        }

        private void FixedUpdate() {
            if (randomEva)
                RandomEvaluation();
            if (fixedEva)
                FixedEvaluation();
        }

        public void FixedEvaluation() {
            List<float> inputs = new List<float>();

            switch (attempts) {
                case 0:
                    inputs.Add(0);
                    inputs.Add(1);
                    break;
                case 1:
                    inputs.Add(1);
                    inputs.Add(0);
                    break;
                case 2:
                    inputs.Add(0);
                    inputs.Add(0);
                    break;
                case 3:
                    inputs.Add(1);
                    inputs.Add(1);
                    break;
            }

            List<float> outputValues = brainNEAT.Evaluate(inputs);


            if (outputValues.Count != 1)
                Debug.LogError("Output Values have not the size of 2");


            switch (attempts) {
                case 0:
                    fitness += outputValues[0];
                    break;
                case 1:
                    fitness += outputValues[0];
                    break;
                case 2:
                    fitness += (1 - outputValues[0]);
                    break;
                case 3:
                    fitness += (1 - outputValues[0]);
                    break;
            }

            switch (attempts) {
                case 0:
                    if (outputValues[0] >= 0.5f)
                        correctAttempt++;
                    break;
                case 1:
                    if (outputValues[0] >= 0.5f)
                        correctAttempt++;
                    break;
                case 2:
                    if (outputValues[0] < 0.5f)
                        correctAttempt++;
                    break;
                case 3:
                    if (outputValues[0] < 0.5f)
                        correctAttempt++;
                    break;
            }

            attempts++;

            processField.text = inputs[0] + " XOR " + inputs[1] + " = " + outputValues[0] + " -> Correct (" + correctAttempt + ")";
            choices.Add(inputs[0] + " XOR " + inputs[1] + " = " + outputValues[0] + " -> Correct (" + correctAttempt + ")\n");

            if (attempts >= 4) {
                Debug.Log("Fitness: <b>" + GetFitness() + "</b>\n\n" + string.Join("", choices));
                if (correctAttempt >= 4) {
                    Debug.LogError("Found");
                }
                FindObjectOfType<NEAT.NEAT>().Lost(GetFitness());
            }
        }

        public void RandomEvaluation() {
            List<float> inputs = new List<float>();

            inputs.Add(UnityEngine.Random.Range(0, 2));
            inputs.Add(UnityEngine.Random.Range(0, 2));

            List<float> outputValues = brainNEAT.Evaluate(inputs);

            if (outputValues.Count != 1)
                Debug.LogError("Output Values have not the size of 2");

            processField.text = inputs[0] + " XOR " + inputs[1] + " = " + outputValues[0] + " -> Total (" + counts + ")";
            //Debug.Log(inputs[0] + " XOR " + inputs[1] + " = " + outputValues[0] + " -> Total (" + counts + ")");

            counts++;

            if (inputs[0] == 0 && inputs[1] == 0 && outputValues[0] < 0.5f) {
                UpdateFitness();
            } else if (inputs[0] == 0 && inputs[1] == 1 && outputValues[0] >= 0.5f) {
                UpdateFitness();
            } else if (inputs[0] == 1 && inputs[1] == 0 && outputValues[0] >= 0.5f) {
                UpdateFitness();
            } else if (inputs[0] == 1 && inputs[1] == 1 && outputValues[0] < 0.5f) {
                UpdateFitness();
            } else {
                FindObjectOfType<NEAT.NEAT>().Lost(GetFitness());
            }

            //if (counts == 100) {
            //    brainNEAT.DebugConnectionsGenesLog();
            //    brainNEAT.DebugNodeGenesLog();
            //}
        }

        public override float CalculateFitness() {
            float value = GetFitness();
            if (randomEva)
                value++;

            return value;
        }

        public override void OnResetAgent() {
            attempts = 0;
            correctAttempt = 0;
            if (randomEva)
                fitness = 1;
            counts = 0;
            choices = new List<string>();
        }

        public override void ExecuteAction() {
            throw new System.NotImplementedException();
        }

        public override List<float> UpdateInput() {
            throw new System.NotImplementedException();
        }
    }
}