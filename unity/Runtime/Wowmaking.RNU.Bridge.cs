
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class NativeAPI
{
    [DllImport("__Internal")]
    public static extern void sendMessage(string message);
}

namespace Wowmaking.RNU
{

    public interface IRNCommandsDelegate
    {
        void HandleCommand(string message);
    }

    public interface IRNCommandsReciever
    {
        void HandleCommand(RNCommand command);
    }

    public class RNCommand
    {

        public int id;
        public string name;
        public object data;
        public bool resolved;

        private bool completed = false;

        public void Resolve(object resultData = null)
        {
            this.resolved = true;
            this.SendResult(resultData);
        }

        public void Reject(object resultData = null)
        {
            this.resolved = false;
            this.SendResult(resultData);
        }

        private void SendResult(object resultData = null)
        {
            if (!this.completed)
            {
                this.completed = true;
                if (resultData == null)
                {
                    resultData = new { };
                }

                RNBridge.SendResult(id, name, resolved, resultData);
            }
        }
    }

    public static class RNBridge
    {

        private static IRNCommandsReciever commandsReciever = null;

        public static void SetCommandsReciever(IRNCommandsReciever cReciever)
        {
            RNBridge.commandsReciever = cReciever;
        }

        public static void SendCommandToReciever(string message)
        {
            if (RNBridge.commandsReciever == null)
            {
                return;
            }

            RNBridge.commandsReciever.HandleCommand(RNBridge.CreateCommand(message));
        }


        public static RNCommand CreateCommand(string message) {
            return JsonUtility.FromJson<RNCommand>(message);
        }


        public static void SendEvent(String name, object data)
        {
            string message = JsonUtility.ToJson(new {
                  type = "event",
                  name,
                  data,
              });

            RNBridge.SendMessage(message);
        }

        public static void SendResult(int id, String name, bool resolved, object data)
        {
            string message = JsonUtility.ToJson(new
            {
                type = "result",
                id,
                name,
                resolved,
                data,
            });

            RNBridge.SendMessage(message);
        }

        public static void SendMessage(String message)
        {
#if UNITY_ANDROID
            try
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.company.product.OverrideUnityActivity");
                AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
                overrideActivity.Call("showMainActivity", lastStringColor);
            } catch(Exception e)
            {
                appendToText("Exception during showHostMainWindow");
                appendToText(e.Message);
            }
#elif UNITY_IOS
            NativeAPI.sendMessage(message);
#endif
        }

    }
}