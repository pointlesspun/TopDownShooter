/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using UnityEngine;
    
    public class InputStateBehaviour : MonoBehaviour
    {
        public GameObject _inputStateListener;

        private bool _isFireButtonDown;

        public void Start()
        {
            _isFireButtonDown = false;
        }

        public void Update()
        {

            if (_isFireButtonDown)
            {
                if (!Input.GetButton(InputNames.Fire1))
                {
                    if (_inputStateListener == null)
                    {
                        gameObject.SendMessage(MessageNames.OnFireButton);
                    }
                    else
                    {
                        _inputStateListener.SendMessage(MessageNames.OnFireButton);
                    }
                }
            }

            _isFireButtonDown = Input.GetButton(InputNames.Fire1);
        }
    }
}
