using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test.FlappyBird
{
    public class Pipe : MonoBehaviour
    {
        public float moveSpeed = 1.25f;

        public const float lifeTime = 10;
        private float timer;

        public Transform topPoint;
        public Transform botPoint;

        public Vector2 localTransformTopPoint;
        public Vector2 localTransformBotPoint;

        private void Start() {
            GameManager.instance.resetEvent += OnReset;
        }

        void Update() {
            localTransformTopPoint = topPoint.position;
            localTransformBotPoint = botPoint.position;

            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (timer >= lifeTime)
                OnReset();

            timer += Time.deltaTime;
        }

        void OnReset() {
            GameManager.instance.resetEvent -= OnReset;
            Destroy(this.gameObject);
        }
    }
}