using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wowmaking.RNU;

public class RNCommadsDelegate : MonoBehaviour, IRNCommandsDelegate
{

    public void HandleCommand(string message)
    {
        RNBridge.SendCommandToReciever(message);
    }
}
