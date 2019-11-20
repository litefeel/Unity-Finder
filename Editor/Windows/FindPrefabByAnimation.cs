using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByAnimation : FindAssetWindowBase<AnimationClip, UnityEngine.Object>
    {

        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = m_Asset != null;
        }

        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            var has = UnityUtil.AnyOneComponentAndChildren<Animator>((animator) =>
            {
                if (!animator.runtimeAnimatorController) return false;
                foreach (var clip in animator.runtimeAnimatorController.animationClips)
                {
                    if (clip == m_Asset)
                        return true;
                }
                return false;
            }, prefab.transform);
            has |= UnityUtil.AnyOneComponentAndChildren<Animation>((animation) =>
            {
                foreach (var clip in AnimationUtility.GetAnimationClips(animation.gameObject))
                {
                    if (clip == m_Asset)
                        return true;
                }
                return false;
            }, prefab.transform);
            return has;
        }

    }
}

