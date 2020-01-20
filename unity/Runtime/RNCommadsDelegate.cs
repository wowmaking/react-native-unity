using UnityEngine;


namespace Wowmaking.RNU
{

    public class RNCommadsDelegate : MonoBehaviour, IRNCommandsDelegate
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void HandleCommand(string message)
        {
            RNBridge.SendCommandToReceiver(message);
        }

        public void HandShake()
        {
            RNBridge.HandShake();
        }

    }

}
