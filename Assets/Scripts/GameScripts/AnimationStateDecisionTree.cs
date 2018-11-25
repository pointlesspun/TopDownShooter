/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Class which provides methods to decide what animations to play given a character's state.
    /// </summary>
    public static class AnimationStateDecisionTree 
    {
        /// <summary>
        /// Currently we only have 1 type of character which has an aim and a moving direction based on
        /// which the correct animation state is decided. While this can be done in editor via the animator's state
        /// machine, it quickly becomes messy and hard to maintain. Doing it in code is much easier.
        ///
        /// XXX consider creating the animator state and motions in code to maintain consistency of where elements are defined ?
        /// 
        /// </summary>
        /// <param name="characterPosition">Current position of the character</param>
        /// <param name="targetPostion">Position of the target</param>
        /// <param name="velocity">2D Velocity of the character</param>
        /// <param name="idleVelocity">Velocity below which (-rv, rv) the character is considered idle </param>
        /// <returns></returns>
        public static int GetAnimationState(Vector3 characterPosition, Vector3 targetPostion, Vector3 velocity, float idleVelocity)
        {
            var velocityMagnitude = velocity.sqrMagnitude;

            // is the character idle ?
            if (velocityMagnitude < idleVelocity * idleVelocity)
            {
                // it is idle, decide what direction to face
                if (targetPostion.x > characterPosition.x)
                {
                    return AnimationState.IdleFacingRight;
                }
                else
                {
                    return AnimationState.IdleFacingLeft;
                }
            }
            else
            {
                // is the character moving left ?
                if ( velocity.x < -idleVelocity)
                {
                    // it is moving left, decide the motion
                    if (targetPostion.x > characterPosition.x)
                    {
                        return AnimationState.MovingBackwardFacingRight;
                    }
                    else
                    {
                        return AnimationState.MovingForwardFacingLeft;
                    }
                }
                // is the character moving right ?
                else if (velocity.x > idleVelocity)
                {
                    // it is moving right, decide the motion
                    if (targetPostion.x > characterPosition.x)
                    {
                        return AnimationState.MovingForwardFacingRight;
                    }
                    else
                    {
                        return AnimationState.MovingBackwardFacingLeft;
                    }

                }
                // else the character may be moving up or down
                else
                {
                    if (targetPostion.x > characterPosition.x)
                    {
                        return AnimationState.MovingForwardFacingRight;
                    }
                    else
                    {
                        return AnimationState.MovingForwardFacingLeft;
                    }
                }
            }
        }
    }
}
