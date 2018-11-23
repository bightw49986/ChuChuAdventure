using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem;
using System;


namespace FSM
{
    public class Goblin : NpcFSM
{
        Material[] mat;

        void Start()
        {
            Renderer[] ren = GetComponentsInChildren<Renderer>();
            mat = new Material[ren.Length - 1];
            for (int i = 0; i < ren.Length -1 ; i++)
            {
                mat[i] = ren[i].material;
            }

            for (int i = 0; i < m_BattleData.DefendBoxes.Count - 1; i++)
            {
                m_BattleData.EneableDefendBox(i);
            }
        }


        protected override void OnCharacterHit()
        {
            //暫無特效
        }

        protected override void OnCharacterFreezed(DefendBox damagedPart)
        {
            ResetTriggers();
            PerformGlobalTransition(Npc.Freezed);
            m_AIData.Destination = AISystem.AIData.DestinationState.None;
        }

        protected override void OnCharacterKOed()
        {
            ResetTriggers();
            PerformGlobalTransition(Npc.Freezed);
            m_AIData.Destination = AISystem.AIData.DestinationState.None;
        }

        protected override void OnCharacterDied()
        {
            ResetTriggers();
            PerformGlobalTransition(Npc.Died);
            m_AIData.Destination = AISystem.AIData.DestinationState.None;
            CloseRigidBody();
        }

        public void CloseRigidBody()
        {
            Rigidbody rig = GetComponent<Rigidbody>();
            rig.useGravity = false;
            rig.constraints = RigidbodyConstraints.FreezeAll;
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                c.enabled = false;
            }
        }

        protected override void CheckGlobalConditions()
        {
        }

        public void Jump_On()
        {
            Rigidbody rig = GetComponent<Rigidbody>();
            if (rig)
            {
                rig.useGravity = false;
                rig.isKinematic = true;
            }
        }

        public void Jump_Off()
        {
            Rigidbody rig = GetComponent<Rigidbody>();
            if (rig)
            {
                rig.useGravity = true;
                rig.isKinematic = false;
            }
        }

        public void Atk_On(int index)
        {
            m_BattleData.EnableAttackBox(index);
        }

        public void Atk_Off(int index)
        {
            m_BattleData.DisableAttackBox(index);
        }

        public void Spawn_Dissolve(float fTime)
        {
            foreach (var m in mat)
            {
                StartCoroutine(DissolveIn(m, fTime));
            }
        }

        public void Die_Dissolve(float fTime)
        {
            foreach (var m in mat)
            {
                StartCoroutine(DissolveOut(m,fTime));
            }
        }

        IEnumerator DissolveIn(Material m, float fTime)
        {
            float f = fTime;
            m.shader = Shader.Find("DissolverShader/DissolveShader");
            while(f > 0)
            {
                f -= Time.deltaTime;
                m.SetFloat("_DissolveAmount", f / fTime);
                yield return null;
            }
        }

        IEnumerator DissolveOut(Material m ,  float fTime)
        {

            float f = 0f;
            m.shader = Shader.Find("DissolverShader/DissolveShader");
            while (f < fTime)
            {
                f += Time.deltaTime;
                m.SetFloat("_DissolveAmount", f / fTime);
                yield return null;
            }
        }
    }

}
