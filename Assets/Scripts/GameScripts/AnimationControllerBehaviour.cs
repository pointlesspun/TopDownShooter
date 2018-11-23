/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AnimationControllerBehaviour : MonoBehaviour
    {
        public AnimationClip moveLeft;
        public AnimationClip moveRight;

        public AnimationClip idleLeft;
        public AnimationClip idleRight;

        public void Start()
        {
            var animator = GetComponent<Animator>();
            var animationOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);

            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            foreach (var oldAnimation in animationOverride.animationClips)
            {
                switch(oldAnimation.name)
                {
                    case "RunRight":
                    {
                        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, moveRight));
                        break;
                    }

                    case "RunLeft":
                    {
                        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, moveLeft));
                        break;
                    }

                    case "StandLeft":
                    {
                        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, idleLeft));
                        break;
                    }

                    case "StandRight":
                    {
                        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, idleRight));
                        break;
                    }
                }
            }

            animationOverride.ApplyOverrides(anims);
            animator.runtimeAnimatorController = animationOverride;
        }
    }
}
