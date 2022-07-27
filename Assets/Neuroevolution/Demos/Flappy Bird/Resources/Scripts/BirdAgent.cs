using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using NEAT;

namespace Test.FlappyBird
{
    public class BirdAgent : Agent
    {
        public const float jumpForce = 425;
        private Rigidbody2D rb;

        [Header("Input Data")]
        public Pipe currentFocusedPipe;
        public float distanceToTop;
        public float distanceToBot;
        public float distanceToPipe;

        public override void Awake() {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            GameManager.instance.StartGame();
        }

        public override float CalculateFitness() {
            float value = GetFitness();
            value += Time.deltaTime;

            return value;
        }

        public override void OnResetAgent() {
            if (GameManager.instance.isImmortal)
                return;

            GameManager.instance.ResetGame();

            rb.velocity = Vector2.zero;

            transform.position = Vector2.zero;
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.A)) {
                Jump();
            }

                UIManagerNEAT.Instance.UpdateFitnessUI(GetFitness());
            

            if (GameManager.instance.gameStarted)
                UpdateFitness();

            if (currentFocusedPipe == null) {
                if (PipeSpawner.instance.pipes.Count > 0)
                    FocusPipe();
            }
        }

        private void FixedUpdate() {
            #region NEAT Simulation
            Think();
            #endregion
        }

        void Jump() {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
        }

        void FocusPipe() {
            currentFocusedPipe = PipeSpawner.instance.RemovePipe();
        }

        public override void ExecuteAction() {
            if (output[0] >= 0.5f)
                Jump();
        }

        public override void CalculateOutput() {
            base.CalculateOutput();

            if(output.Count != 2)
                Debug.LogError("Output Values have not the size of 2");
        }

        public override List<float> UpdateInput() {
            if (currentFocusedPipe == null)
                return new List<float> { 0, 0, 0 };

            List<float> inputs = new List<float>();

            distanceToPipe = Vector2.Distance(transform.position, currentFocusedPipe.transform.position);
            distanceToTop = Vector2.Distance(transform.position, currentFocusedPipe.topPoint.transform.position);
            distanceToBot = Vector2.Distance(transform.position, currentFocusedPipe.botPoint.transform.position);

            inputs.Add(distanceToPipe);
            inputs.Add(distanceToTop);
            inputs.Add(distanceToBot);

            float divisionFactor = 20;
            for (int i = 0; i < inputs.Count; i++) {
                inputs[i] = inputs[i] / divisionFactor;
            }

            return inputs;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.tag.Equals("Centre")) {
                FocusPipe();
            }

            if (collision.gameObject.tag.Equals("Pipe")) {
                Next();
            }
        }
    }
}