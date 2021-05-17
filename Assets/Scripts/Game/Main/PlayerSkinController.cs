using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Main
{
    public class PlayerSkinController : MonoBehaviour
    {
        [SerializeField] private List<Animator> skinAnimators;

        private Animator parentAnimator;
        private int SkinId;

        private void Awake()
        {
            for (var index = 0; index < skinAnimators.Count; index++)
            {
                var skinAnimator = skinAnimators[index];
                
                skinAnimator.gameObject.SetActive(false);
            }
        }

        public void Init(Animator animator)
        {
            var lastSkin = GamePlayerPrefs.SkinIndex;
            Debug.Log("Skin # " + lastSkin);
            parentAnimator = animator;
            
            SetSkin(lastSkin);
        }

        private void SetSkin(int id)
        {
            if (id < 0 || id >= skinAnimators.Count)
            {
                return;
            }

            var animator = skinAnimators[id];
            
            animator.gameObject.SetActive(true);

            parentAnimator.Play("Base Layer.Idle", 0, 0);
            animator.Play("Base Layer.Idle", 0, 0);
        }

        public void SetFloat(string key, float value)
        {
            for (var i = 0; i < skinAnimators.Count; i++)
            {
                var skin = skinAnimators[i];
                
                skin.SetFloat(key, value);
            }
        }

        public void SetTrigger(string key)
        {
            for (var i = 0; i < skinAnimators.Count; i++)
            {
                var skin = skinAnimators[i];
                
                skin.SetTrigger(key);
            }
        }

        [ContextMenu("BANANA")]
        public void SetBanana()
        {
            SetSkin(0);
        }
    }
}