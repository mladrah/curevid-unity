using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using NEAT;

namespace Test.FlappyBird
{
    public class TestCode : MonoBehaviour
    {
        Genome a;
        Genome b;
        Genome c;
        Genome xor;
        public Agent agent;
        List<(float, float)> inp;

        int index = 0;

        void Start() {
            BuildGenomeC();
            UIManagerNEAT.Instance.BuildNN(c);
            c.DebugNodeGenesLog();
            c.DebugConnectionsGenesLog();
            Debug.Log("-----");
            //BuildXOR();
            //inp = new List<(float, float)> { (0, 0), (1, 0), (0, 1), (1, 1) };
            //agent.ResetAgent(xor);
        }

        private void FixedUpdate() {

        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                xor.Evaluate(new List<float> { inp[index].Item1, inp[index].Item2 });
                xor.DebugNodeGenesLog();
                xor.DebugConnectionsGenesLog();
                index = Random.Range(0, 3);
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                c.MutateConnection();
                UIManagerNEAT.Instance.BuildNN(c);
                c.DebugNodeGenesLog();
                c.DebugConnectionsGenesLog();
                Debug.Log("-----");
            }

            if (Input.GetKeyDown(KeyCode.M)) {
                c.MutateNode();
                UIManagerNEAT.Instance.BuildNN(c);
                c.DebugNodeGenesLog();
                c.DebugConnectionsGenesLog();
                Debug.Log("-----");
            }

        }


        void BuildXOR() {
            xor = new Genome();

            NodeGene b = new NodeGene(0, Layer.Input, 0, 1);
            NodeGene a = new NodeGene(1, Layer.Input, 0);
            NodeGene c = new NodeGene(2, Layer.Input, 0);
            NodeGene o = new NodeGene(3, Layer.Output, 2);
            NodeGene h = new NodeGene(4, Layer.Hidden, 1);

            xor.AddNodeGene(b);
            xor.AddNodeGene(a);
            xor.AddNodeGene(c);
            xor.AddNodeGene(h);
            xor.AddNodeGene(o);

            ConnectionGene cg0 = new ConnectionGene(a, o, 12.46f, 0);
            ConnectionGene cg1 = new ConnectionGene(a, h, -3.14f, 1);
            ConnectionGene cg2 = new ConnectionGene(c, h, 10.99f, 2);
            ConnectionGene cg3 = new ConnectionGene(b, o, -7.47f, 3);
            ConnectionGene cg4 = new ConnectionGene(b, h, -3.67f, 4);
            ConnectionGene cg5 = new ConnectionGene(h, o, 11.78f, 5);
            ConnectionGene cg6 = new ConnectionGene(h, b, 8.01f, 6);

            xor.AddConnectionGene(cg0);
            xor.AddConnectionGene(cg1);
            xor.AddConnectionGene(cg2);
            xor.AddConnectionGene(cg3);
            xor.AddConnectionGene(cg4);
            xor.AddConnectionGene(cg5);
            xor.AddConnectionGene(cg6);

            xor.DebugNodeGenesLog();
            xor.DebugConnectionsGenesLog();
            UIManagerNEAT.Instance.BuildNN(xor);
            Debug.Log("-----");
        }


        void BuildGenomeC() {
            c = new Genome();

            NodeGene cNg1 = new NodeGene(0, Layer.Input, 0);
            NodeGene cNg2 = new NodeGene(1, Layer.Input, 0);
            NodeGene cNg6 = new NodeGene(2, Layer.Output, 1);
            NodeGene cNg3 = new NodeGene(3, Layer.Hidden);
            NodeGene cNg4 = new NodeGene(4, Layer.Hidden);
            NodeGene cNg5 = new NodeGene(5, Layer.Hidden);

            c.AddNodeGene(cNg1);
            c.AddNodeGene(cNg2);
            c.AddNodeGene(cNg6);

            c.AddNodeGene(cNg3);
            c.AddNodeGene(cNg4);
            c.AddNodeGene(cNg5);

            ConnectionGene cCg1 = new ConnectionGene(cNg1, cNg3, Random.Range(-1f, 1f), 0);
            ConnectionGene cCg2 = new ConnectionGene(cNg1, cNg4, Random.Range(-1f, 1f), 1);
            ConnectionGene cCg3 = new ConnectionGene(cNg2, cNg4, Random.Range(-1f, 1f), 2);
            ConnectionGene cCg4 = new ConnectionGene(cNg3, cNg5, Random.Range(-1f, 1f), 3);
            ConnectionGene cCg5 = new ConnectionGene(cNg5, cNg6, Random.Range(-1f, 1f), 5);
            ConnectionGene cCg6 = new ConnectionGene(cNg4, cNg6, Random.Range(-1f, 1f), 6);

            c.AddConnectionGene(cCg1);
            c.AddConnectionGene(cCg2);
            c.AddConnectionGene(cCg3);
            c.AddConnectionGene(cCg4);
            c.AddConnectionGene(cCg5);
            c.AddConnectionGene(cCg6);
        }

        void BuildGenomeA() {
            a = new Genome();

            NodeGene aNg1 = new NodeGene(1, Layer.Input, 0);
            NodeGene aNg2 = new NodeGene(2, Layer.Input, 0);
            NodeGene aNg3 = new NodeGene(3, Layer.Input, 0);
            NodeGene aNg4 = new NodeGene(4, Layer.Output, 2);
            NodeGene aNg5 = new NodeGene(5, Layer.Hidden, 1);


            a.AddNodeGene(aNg1);
            a.AddNodeGene(aNg2);
            a.AddNodeGene(aNg3);
            a.AddNodeGene(aNg4);
            a.AddNodeGene(aNg5);


            ConnectionGene aCg1 = new ConnectionGene(aNg1, aNg4, Random.Range(-1f, 1f), 0);
            ConnectionGene aCg2 = new ConnectionGene(aNg2, aNg4, Random.Range(-1f, 1f), 1, false);
            ConnectionGene aCg3 = new ConnectionGene(aNg3, aNg4, Random.Range(-1f, 1f), 2);
            ConnectionGene aCg4 = new ConnectionGene(aNg2, aNg5, Random.Range(-1f, 1f), 3);
            ConnectionGene aCg5 = new ConnectionGene(aNg5, aNg4, Random.Range(-1f, 1f), 4);
            ConnectionGene aCg8 = new ConnectionGene(aNg1, aNg5, Random.Range(-1f, 1f), 7);

            a.AddConnectionGene(aCg1);
            a.AddConnectionGene(aCg2);
            a.AddConnectionGene(aCg3);
            a.AddConnectionGene(aCg4);
            a.AddConnectionGene(aCg5);
            a.AddConnectionGene(aCg8);
        }

        void BuildGenomeB() {
            b = new Genome();

            NodeGene bNg1 = new NodeGene(1, Layer.Input, 0);
            NodeGene bNg2 = new NodeGene(2, Layer.Input, 0);
            NodeGene bNg3 = new NodeGene(3, Layer.Input, 0);
            NodeGene bNg4 = new NodeGene(4, Layer.Output, 3);
            NodeGene bNg5 = new NodeGene(5, Layer.Hidden, 1);
            NodeGene bNg6 = new NodeGene(6, Layer.Hidden, 2);

            b.AddNodeGene(bNg1);
            b.AddNodeGene(bNg2);
            b.AddNodeGene(bNg3);
            b.AddNodeGene(bNg4);
            b.AddNodeGene(bNg5);
            b.AddNodeGene(bNg6);

            ConnectionGene bCg1 = new ConnectionGene(bNg1, bNg4, Random.Range(-1f, 1f), 0);
            ConnectionGene bCg2 = new ConnectionGene(bNg2, bNg4, Random.Range(-1f, 1f), 1, false);
            ConnectionGene bCg3 = new ConnectionGene(bNg3, bNg4, Random.Range(-1f, 1f), 2);
            ConnectionGene bCg4 = new ConnectionGene(bNg2, bNg5, Random.Range(-1f, 1f), 3);
            ConnectionGene bCg5 = new ConnectionGene(bNg5, bNg4, Random.Range(-1f, 1f), 4, false);
            ConnectionGene bCg6 = new ConnectionGene(bNg5, bNg6, Random.Range(-1f, 1f), 5);
            ConnectionGene bCg7 = new ConnectionGene(bNg6, bNg4, Random.Range(-1f, 1f), 6);
            ConnectionGene bCg9 = new ConnectionGene(bNg3, bNg5, Random.Range(-1f, 1f), 8);
            ConnectionGene bCg10 = new ConnectionGene(bNg1, bNg6, Random.Range(-1f, 1f), 9);

            b.AddConnectionGene(bCg1);
            b.AddConnectionGene(bCg2);
            b.AddConnectionGene(bCg3);
            b.AddConnectionGene(bCg4);
            b.AddConnectionGene(bCg5);
            b.AddConnectionGene(bCg6);
            b.AddConnectionGene(bCg7);
            b.AddConnectionGene(bCg9);
            b.AddConnectionGene(bCg10);
        }

        void GenomeDrawTest() {
            Genome testGenome = new Genome();
            NodeGene in1 = new NodeGene(0, Layer.Input, 0);
            NodeGene in2 = new NodeGene(1, Layer.Input, 0);

            NodeGene h = new NodeGene(2, Layer.Hidden, 1);
            NodeGene h2 = new NodeGene(5, Layer.Hidden, 2);
            NodeGene h3 = new NodeGene(6, Layer.Hidden, 2);
            NodeGene h4 = new NodeGene(7, Layer.Hidden, 3);

            NodeGene out1 = new NodeGene(3, Layer.Output, 4);
            NodeGene out2 = new NodeGene(4, Layer.Output, 4);

            testGenome.AddNodeGene(in1);
            testGenome.AddNodeGene(in2);
            testGenome.AddNodeGene(h);
            testGenome.AddNodeGene(h2);
            testGenome.AddNodeGene(h3);
            testGenome.AddNodeGene(h4);
            testGenome.AddNodeGene(out1);
            testGenome.AddNodeGene(out2);

            ConnectionGene cg1 = new ConnectionGene(in1, out1, Random.Range(-1f, 1f));
            ConnectionGene cg2 = new ConnectionGene(in2, h, Random.Range(-1f, 1f));
            ConnectionGene cg3 = new ConnectionGene(h, out2, Random.Range(-1f, 1f));
            ConnectionGene cg4 = new ConnectionGene(in2, out2, Random.Range(-1f, 1f));
            cg4.IsEnabled = false;
            ConnectionGene cg5 = new ConnectionGene(h, h2, Random.Range(-1f, 1f));
            ConnectionGene cg6 = new ConnectionGene(h2, out1, Random.Range(-1f, 1f));
            ConnectionGene cg7 = new ConnectionGene(in1, h3, Random.Range(-1f, 1f));
            ConnectionGene cg8 = new ConnectionGene(h3, out1, Random.Range(-1f, 1f));
            ConnectionGene cg9 = new ConnectionGene(h3, h4, Random.Range(-1f, 1f));
            ConnectionGene cg10 = new ConnectionGene(h4, out2, Random.Range(-1f, 1f));

            testGenome.ConnectionGenes.Add(cg1);
            testGenome.ConnectionGenes.Add(cg2);
            testGenome.ConnectionGenes.Add(cg3);
            testGenome.ConnectionGenes.Add(cg4);
            testGenome.ConnectionGenes.Add(cg5);
            testGenome.ConnectionGenes.Add(cg6);
            testGenome.ConnectionGenes.Add(cg7);
            testGenome.ConnectionGenes.Add(cg8);
            testGenome.ConnectionGenes.Add(cg9);
            testGenome.ConnectionGenes.Add(cg10);

            testGenome.Evaluate(new List<float> { 0.25f, 0.5f, 0.75f });
            UIManagerNEAT.Instance.BuildNN(testGenome);
        }

        void GenomeTest() {
            Genome a = new Genome();

            NodeGene aNg1 = new NodeGene(1, Layer.Input, 0);
            NodeGene aNg2 = new NodeGene(2, Layer.Input, 0);
            NodeGene aNg3 = new NodeGene(3, Layer.Input, 0);
            NodeGene aNg4 = new NodeGene(4, Layer.Output, 2);
            NodeGene aNg5 = new NodeGene(5, Layer.Hidden, 1);


            a.AddNodeGene(aNg1);
            a.AddNodeGene(aNg2);
            a.AddNodeGene(aNg3);
            a.AddNodeGene(aNg4);
            a.AddNodeGene(aNg5);


            ConnectionGene aCg1 = new ConnectionGene(aNg1, aNg4, Random.Range(-1f, 1f), 0);
            ConnectionGene aCg2 = new ConnectionGene(aNg2, aNg4, Random.Range(-1f, 1f), 1, false);
            ConnectionGene aCg3 = new ConnectionGene(aNg3, aNg4, Random.Range(-1f, 1f), 2);
            ConnectionGene aCg4 = new ConnectionGene(aNg2, aNg5, Random.Range(-1f, 1f), 3);
            ConnectionGene aCg5 = new ConnectionGene(aNg5, aNg4, Random.Range(-1f, 1f), 4);
            ConnectionGene aCg8 = new ConnectionGene(aNg1, aNg5, Random.Range(-1f, 1f), 7);



            ///<summary>
            /// --
            /// </summary>

            Genome b = new Genome();

            NodeGene bNg1 = new NodeGene(1, Layer.Input, 0);
            NodeGene bNg2 = new NodeGene(2, Layer.Input, 0);
            NodeGene bNg3 = new NodeGene(3, Layer.Input, 0);
            NodeGene bNg4 = new NodeGene(4, Layer.Output, 3);
            NodeGene bNg5 = new NodeGene(5, Layer.Hidden, 1);
            NodeGene bNg6 = new NodeGene(6, Layer.Hidden, 2);

            b.AddNodeGene(bNg1);
            b.AddNodeGene(bNg2);
            b.AddNodeGene(bNg3);
            b.AddNodeGene(bNg4);
            b.AddNodeGene(bNg5);
            b.AddNodeGene(bNg6);

            ConnectionGene bCg1 = new ConnectionGene(bNg1, bNg4, Random.Range(-1f, 1f), 0);
            ConnectionGene bCg2 = new ConnectionGene(bNg2, bNg4, Random.Range(-1f, 1f), 1, false);
            ConnectionGene bCg3 = new ConnectionGene(bNg3, bNg4, Random.Range(-1f, 1f), 2);
            ConnectionGene bCg4 = new ConnectionGene(bNg2, bNg5, Random.Range(-1f, 1f), 3);
            ConnectionGene bCg5 = new ConnectionGene(bNg5, bNg4, Random.Range(-1f, 1f), 4, false);
            ConnectionGene bCg6 = new ConnectionGene(bNg5, bNg6, Random.Range(-1f, 1f), 5);
            ConnectionGene bCg7 = new ConnectionGene(bNg6, bNg4, Random.Range(-1f, 1f), 6);
            ConnectionGene bCg9 = new ConnectionGene(bNg3, bNg5, Random.Range(-1f, 1f), 8);
            ConnectionGene bCg10 = new ConnectionGene(bNg1, bNg6, Random.Range(-1f, 1f), 9);

            ///<summary>
            ///
            /// </summary>

            a.AddConnectionGene(aCg1);
            a.AddConnectionGene(aCg2);
            a.AddConnectionGene(aCg3);
            a.AddConnectionGene(aCg4);
            a.AddConnectionGene(aCg5);

            b.AddConnectionGene(bCg1);
            b.AddConnectionGene(bCg2);
            b.AddConnectionGene(bCg3);
            b.AddConnectionGene(bCg4);
            b.AddConnectionGene(bCg5);
            b.AddConnectionGene(bCg6);
            b.AddConnectionGene(bCg7);

            a.AddConnectionGene(aCg8);

            b.AddConnectionGene(bCg9);
            b.AddConnectionGene(bCg10);

            Debug.Log("----------");

            a.Fitness = Random.Range(0f, 1f);
            b.Fitness = Random.Range(0f, 1f);

            Debug.Log("A");
            a.DebugConnectionsGenesLog();
            Debug.Log("B");
            b.DebugConnectionsGenesLog();
            NEAT.NEAT.Instance.DebugGenomeComparisonLog(a, b);

            Genome c = NEAT.NEAT.Instance.Crossover(a, b);
            Debug.Log("C");
            c.DebugConnectionsGenesLog();
            c.DebugNodeGenesLog();
            UIManagerNEAT.Instance.BuildNN(c);

        }
    }
}