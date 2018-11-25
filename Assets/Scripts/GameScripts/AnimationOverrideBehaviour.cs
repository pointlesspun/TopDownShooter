/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Takes an animation controller then replaces the animations in the controller according
    /// to the specified animtion clip properties
    /// </summary>
    public class AnimationOverrideBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Name of the run left animation in the original animator
        /// </summary>
        public string moveLeftClipName = "RunLeft";

        /// <summary>
        /// Nameof the clip for moving left to override in the animator
        /// </summary>
        public AnimationClip moveLeft;

        /// <summary>
        /// Name of the 'move right' animation in the original animator
        /// </summary>
        public string moveRightClipName = "RunRight";

        /// <summary>
        /// Name of the clip for moving right to override in the animator
        /// </summary>
        public AnimationClip moveRight;

        /// <summary>
        /// Name of the 'idle facing left' animation in the original animator
        /// </summary>
        public string idleLeftClipName = "StandLeft";

        /// <summary>
        /// Name of the clip for idling facing left to override in the animator
        /// </summary>
        public AnimationClip idleLeft;

        /// <summary>
        /// Name of the 'idle facing right' animation in the original animator
        /// </summary>
        public string idleRightClipName = "StandRight";

        /// <summary>
        /// Name of the clip for idling facing right to override in the animator
        /// </summary>
        public AnimationClip idleRight;

        public void Start()
        {
            var animator = GetComponent<Animator>();
            var animationOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);

            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            foreach (var oldAnimation in animationOverride.animationClips)
            {
                if (moveLeft != null && oldAnimation.name == moveRightClipName) {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, moveRight));
                }
                else if (oldAnimation.name == moveLeftClipName) {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, moveLeft));
                }
                else if (oldAnimation.name == idleLeftClipName) {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, idleLeft));
                }
                else if (oldAnimation.name == idleRightClipName) {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldAnimation, idleRight));
                }
                else {
                    Debug.LogWarning("AnimationOverrideBehaviour: animation unknown name " + oldAnimation.name);
                }
            }

            animationOverride.ApplyOverrides(anims);
            animator.runtimeAnimatorController = animationOverride;
        }
    }
}
