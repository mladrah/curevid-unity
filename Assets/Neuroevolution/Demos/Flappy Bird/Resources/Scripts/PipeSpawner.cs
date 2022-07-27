using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test.FlappyBird
{
    public class PipeSpawner : MonoBehaviour
    {
        public static PipeSpawner instance;
        public GameObject pipePrefab;

        public float height = 2.5f;
        public float intervall;
        private float timer;

        public Queue<Pipe> pipes;

        private void Awake() {
            instance = this;

            pipes = new Queue<Pipe>();
            timer = intervall;
        }

        private void Update() {
            if (GameManager.instance.gameStarted) {

                if (timer >= intervall) {

                    SpawnPipe();

                    timer = 0;
                }

                timer += Time.deltaTime;
            }
        }

        public void ResetSpawner() {
            pipes.Clear();
            SpawnPipe();
            timer = 0;
        }

        public void SpawnPipe() {
            GameObject pipe = Instantiate(pipePrefab);
            pipe.transform.position = new Vector2(transform.position.x, Random.Range(-height, height));

            pipes.Enqueue(pipe.GetComponent<Pipe>());
        }

        public Pipe RemovePipe() {
            if (pipes.Count == 0)
                return null;

            return pipes.Dequeue();
        }
    }
}