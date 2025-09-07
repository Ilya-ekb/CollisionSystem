using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CollisionSystem.Core;
using CollisionSystem.Conditions;

namespace CollisionSystem.Tests
{
    public sealed class RuntimeCollisionTests
    {
        private SimulationMode oldAutoSimulation3D;
        private SimulationMode2D oldMode2D;

        [SetUp]
        public void SetUp()
        {
            oldAutoSimulation3D = Physics.simulationMode;
            oldMode2D = Physics2D.simulationMode;
            Physics.simulationMode = SimulationMode.Script;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        [TearDown]
        public void TearDown()
        {
            Physics.simulationMode = oldAutoSimulation3D;
            Physics2D.simulationMode = oldMode2D;
            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID))
                Object.DestroyImmediate(go);
        }

        private static GameObject CreateBox3D(string name, Vector3 pos, bool kinematic = false)
        {
            var go = new GameObject(name)
            {
                transform =
                {
                    position = pos
                }
            };
            var rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = kinematic;
            go.AddComponent<BoxCollider>();
            go.AddComponent<CollisionParticipant>();
            go.AddComponent<CollisionReporter3D>();
            go.AddComponent<TestListener>();
            return go;
        }

        private static GameObject CreateBox2D(string name, Vector2 pos, bool kinematic = false)
        {
            var go = new GameObject(name)
            {
                transform =
                {
                    position = new Vector3(pos.x, pos.y, 0f)
                }
            };
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = kinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<CollisionParticipant>();
            go.AddComponent<CollisionReporter2D>();
            go.AddComponent<TestListener>();
            return go;
        }

        [UnityTest]
        public IEnumerator Deduplication3D_PairProcessedOnce()
        {
            new GameObject("CollisionManager").AddComponent<CollisionManager>();

            var a = CreateBox3D("A", new Vector3(0, 2, 0));
            var b = CreateBox3D("B", new Vector3(0, 0, 0), kinematic: true);

            for (int i = 0; i < 60; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                yield return null;
            }

            Assert.AreEqual(1, a.GetComponent<TestListener>().Count);
            Assert.AreEqual(1, b.GetComponent<TestListener>().Count);
        }

        [UnityTest]
        public IEnumerator Conditions_Block_ByLayer()
        {
            new GameObject("CollisionManager").AddComponent<CollisionManager>();

            var a = CreateBox3D("A", new Vector3(0, 2, 0));
            var b = CreateBox3D("B", new Vector3(0, 0, 0), kinematic: true);

            var cond = ScriptableObject.CreateInstance<LayerCondition>();
#if UNITY_EDITOR
            var so = new UnityEditor.SerializedObject(cond);
            so.FindProperty("allowedLayers").intValue = 0;
            so.ApplyModifiedPropertiesWithoutUndo();
#endif
            var paField = typeof(CollisionParticipant).GetField("conditions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var list = (IList)paField.GetValue(a.GetComponent<CollisionParticipant>());
            list.Add(cond);

            for (var i = 0; i < 40; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                yield return null;
            }

            Assert.AreEqual(0, a.GetComponent<TestListener>().Count);
            Assert.AreEqual(0, b.GetComponent<TestListener>().Count);
        }

        [UnityTest]
        public IEnumerator WorksWith2D()
        {
            new GameObject("CollisionManager").AddComponent<CollisionManager>();

            var a = CreateBox2D("A2D", new Vector2(0, 2));
            var b = CreateBox2D("B2D", new Vector2(0, 0), kinematic: true);

            for (var i = 0; i < 80; i++)
            {
                Physics2D.Simulate(Time.fixedDeltaTime);
                yield return null;
            }

            Assert.AreEqual(1, a.GetComponent<TestListener>().Count);
            Assert.AreEqual(1, b.GetComponent<TestListener>().Count);
        }
    }

    public sealed class TestListener : MonoBehaviour, ICollisionListener
    {
        public int Count { get; private set; }

        public void OnCollisionAccepted(in CollisionContext context)
        {
            Count++;
        }
    }
}