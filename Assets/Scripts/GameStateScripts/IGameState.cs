/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameStateScripts
{
    public enum GameStateType
    {
        /// <summary>
        /// state is not started, may be used to differentiate between the first and Nth time the state is started
        /// </summary>
        NotStarted,

        /// <summary>
        /// State is currently running
        /// </summary>
        Started,

        /// <summary>
        /// State has been started but is now inactive
        /// </summary>
        Stopped
    }

    public interface IGameState
    {
        /// <summary>
        /// Current state type of the game state, should start with "NotStarted".
        /// </summary>
        GameStateType State { get; }

        /// <summary>
        /// Start the state, moving the 'State' property to started.
        /// </summary>
        void StartGameState();

        /// <summary>
        /// Stops the game state moving the 'State' property to stopped.
        /// </summary>
        void StopGameState();
    }
}
