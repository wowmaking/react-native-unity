using UnityEngine;


namespace Wowmaking.RNU
{

    public class RNCommadsDelegate : MonoBehaviour, IRNCommandsDelegate
    {

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