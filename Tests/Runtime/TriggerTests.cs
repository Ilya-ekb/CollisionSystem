using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CollisionSystem.Core;

namespace CollisionSystem.Tests {
    public sealed class TriggerTests {
        private bool _oldAuto;
#if ENABLE_2D_PHYSICS
        private SimulationMode2D _old2D;
#endif

        [SetUp]
        public void SetUp() {
            _oldAuto = Physics.autoSimulation;
            Physics.autoSimulation = false;
#if ENABLE_2D_PHYSICS
            _old2D = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;
#endif
        }

        [TearDown]
        public void TearDown() {
            Physics.autoSimulation = _oldAuto;
#if ENABLE_2D_PHYSICS
            Physics2D.simulationMode = _old2D;
#endif
            foreach (var go in Object.FindObjectsOfType<GameObject>())
                Object.DestroyImmediate(go);
        }

        // ---------- 3D TRIGGER ----------
        [UnityTest]
        public IEnumerator Trigger3D_OnEnter_RegistersOnce() {
            new GameObject("CollisionManager").AddComponent<CollisionManager>();

            // Trigger volume (kinematic RB + isTrigger collider)
            var trigger = new GameObject("Trigger3D");
            trigger.transform.position = new Vector3(0,0.5f,0);
            var trb = trigger.AddComponent<Rigidbody>(); trb.isKinematic = true;
            var tcol = trigger.AddComponent<BoxCollider>(); 
            tcol.isTrigger = true; 
            tcol.size = new Vector3(2,1,2);
            trigger.AddComponent<CollisionParticipant>();
            trigger.AddComponent<CollisionReporter3D>();
            trigger.AddComponent<TestListener>();

            // Dynamic body falling through trigger
            var body = new GameObject("Body3D");
            body.transform.position = new Vector3(0,3f,0);
            var rb = body.AddComponent<Rigidbody>(); rb.useGravity = true;
            body.AddComponent<SphereCollider>();
            body.AddComponent<CollisionParticipant>();
            body.AddComponent<CollisionReporter3D>();
            body.AddComponent<TestListener>();

            // Simulate
            for(int i=0;i<120;i++){ 
                Physics.Simulate(Time.fixedDeltaTime); 
                yield return null; 
            }

            Assert.AreEqual(1, trigger.GetComponent<TestListener>().Count);
            Assert.AreEqual(1, body.GetComponent<TestListener>().Count);
        }

#if ENABLE_2D_PHYSICS
        // ---------- 2D TRIGGER ----------
        [UnityTest]
        public IEnumerator Trigger2D_OnEnter_RegistersOnce() {
            new GameObject("CollisionManager").AddComponent<CollisionManager>();

            // Trigger area 2D
            var trigger = new GameObject("Trigger2D");
            trigger.transform.position = new Vector3(0,0.5f,0);
            var trb = trigger.AddComponent<Rigidbody2D>(); 
            trb.bodyType = RigidbodyType2D.Kinematic;
            var tcol = trigger.AddComponent<BoxCollider2D>(); 
            tcol.isTrigger = true; 
            tcol.size = new Vector2(2,1);
            trigger.AddComponent<CollisionParticipant>();
            trigger.AddComponent<CollisionReporter2D>();
            trigger.AddComponent<TestListener>();

            // Falling circle 2D
            var body = new GameObject("Body2D");
            body.transform.position = new Vector3(0,3f,0);
            var rb = body.AddComponent<Rigidbody2D>(); 
            rb.bodyType = RigidbodyType2D.Dynamic;
            body.AddComponent<CircleCollider2D>();
            body.AddComponent<CollisionParticipant>();
            body.AddComponent<CollisionReporter2D>();
            body.AddComponent<TestListener>();

            for(int i=0;i<160;i++){ 
                Physics2D.Simulate(Time.fixedDeltaTime); 
                yield return null; 
            }

            Assert.AreEqual(1, trigger.GetComponent<TestListener>().Count);
            Assert.AreEqual(1, body.GetComponent<TestListener>().Count);
        }
#endif

        // Simple listener
        private sealed class TestListener: MonoBehaviour, ICollisionListener {
            public int Count{get; private set;}
            public void OnCollisionAccepted(in CollisionContext ctx){ Count++; }
        }
    }
}