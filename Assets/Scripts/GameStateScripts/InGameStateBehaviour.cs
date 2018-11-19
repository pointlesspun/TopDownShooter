/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameStateScripts
{
    using Tds.GameScripts;
    using UnityEngine;

    /// <summary>
    /// Expressed the behavior of the state when in game. Will track the state of the player.
    /// When the player is dead, will end the game state and go to the the title screen.
    /// 
    /// xxx move to scene
    /// </summary>
    public class InGameStateBehaviour : MonoBehaviour, IGameState
    {
        public static GameObject _inGameStateObject;

        private GameStateType _state = GameStateType.NotStarted;

        public GameObject titleScreenGameState;
        public GameObject levelPrefab;

        public GameStateType State
        {
            get
            {
                return _state;
            }
        }

        private GameObject _level;
        private GameObject _player;

        public void StartGameState()
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            _state = GameStateType.Started;
            _level = Instantiate<GameObject>(levelPrefab);
            _level.transform.parent = transform;
            _inGameStateObject = _level;
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);
        }

        public void StopGameState()
        {
            _state = GameStateType.Stopped;
            Destroy(_level);
            titleScreenGameState.GetComponent<IGameState>().StartGameState();
            gameObject.SetActive(false);
        }
        
        public void Update()
        {
            if (_state == GameStateType.Started)
            {
                if (_player == null || !_player.activeInHierarchy)
                {
                    StopGameState();
                }
            }
        }
    }
}
