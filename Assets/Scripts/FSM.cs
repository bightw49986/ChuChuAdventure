using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class WiwiFSM : MonoBehaviour
    {
        protected Transform playerTransform;
        protected Vector3 destPos;
        protected GameObject[] pointList;

        protected float shootRate;
        protected float elapsedTime;

        protected virtual void Initialize() { }
        protected virtual void FSMUpdate() { }
        protected virtual void FSMFixedUpdate() { }


        // Use this for initialization
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            FSMUpdate();
        }
        void FixedUpdate()
        {
            FSMFixedUpdate();
        }
    }




