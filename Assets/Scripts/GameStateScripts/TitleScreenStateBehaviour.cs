/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameStateScripts
{
    using UnityEngine;
    using Tds.GameScripts;

    /// <summary>
    /// Expressed the behavior of the state when in the title screen. When the player presses the
    /// fire button, the game will start.
    /// 
    /// xxx move to scene
    /// </summary>
    public class TitleScreenStateBehaviour : MonoBehaviour, IGameState
    {
        private GameStateType _state = GameStateType.NotStarted;

        public GameObject inGameState;
        public GameObject titleScreenUI;

        public bool isFireButtonUp = false;

        public GameStateType State
        {
            get
            {
                return _state;
            }
        }

        public void StopGameState()
        {
            _state = GameStateType.Stopped;
            titleScreenUI.SetActive(false);
            inGameState.GetComponent<IGameState>().StartGameState();
            gameObject.SetActive(false);
        }

        public void StartGameState()
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            isFireButtonUp = false;
            _state = GameStateType.Started;
            titleScreenUI.SetActive(true);
        }

        void Start()
        {
            StartGameState();
        }

        void Update()
        {
            if (!isFireButtonUp)
            {
                isFireButtonUp = !Input.GetButton(InputNames.Fire1);
            }

            if (isFireButtonUp)
            {
                if (_state == GameStateType.Started && Input.GetButton(InputNames.Fire1))
                {
                    StopGameState();
                }
            }
        }
    }
}
