using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test.FlappyBird
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public bool isImmortal;
        public bool gameStarted = false;

        public delegate void gameManagerDelegate();
        public event gameManagerDelegate resetEvent;

        private void Awake() {
            instance = this;
        }

        public void StartGame() {
            gameStarted = true;
        }

        public void ResetGame() {
            PipeSpawner.instance.ResetSpawner();

            resetEvent?.Invoke();
        }
    }
}
