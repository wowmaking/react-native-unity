using System;
using System.Runtime.InteropServices;

using Newtonsoft.Json.Linq;

#if UNITY_IOS
public class NativeAPI
{
    [DllImport("__Internal")]
    public static extern void sendMessage(string message);
}
#endif

namespace Wowmaking.RNU
{

    class Result
    {
        public string type;
        public string name;
        public object data;
    }

    class EventResult : Result
    {
        public EventResult()
        {
            this.type = "event";
        }
    }

    class CommandResult : Result
    {
        public int id;
        public bool resolved;

        public CommandResult()
        {
            this.type = "result";
        }
    }

    class HandShake : Result
    {
        public HandShake()
        {
            this.type = "handshake";
        }
    }

    public interface IRNCommandsDelegate
    {
        void HandleCommand(string message);
    }

    public interface IRNCommandsReceiver
    {
        void HandleCommand(RNCommand command);
    }

    public class RNCommand
    {

        public int id;
        public string name;
        public JObject data;
        public bool resolved;

        private bool completed = false;

        public virtual void Resolve(object resultData = null)
        {
            this.resolved = true;
            this.SendResult(resultData);
        }

        public virtual void Reject(object resultData = null)
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

        private static IRNCommandsReceiver commandsReceiver = null;

        public static void SetCommandsReceiver(IRNCommandsReceiver cReceiver)
        {
            RNBridge.commandsReceiver = cReceiver;
        }

        public static void SendCommandToReceiver(string message)
        {
            if (RNBridge.commandsReceiver == null)
            {
                return;
            }

            RNBridge.commandsReceiver.HandleCommand(RNBridge.CreateCommand(message));
        }


        public static RNCommand CreateCommand(string message)
        {
            var c = new RNCommand();
            var o = JObject.Parse(message);

            c.id = (int)o["id"];
            c.name = (string)o["name"];
            c.data = (JObject)o["data"];

            return c;
        }


        public static void HandShake()
        {
            var r = new HandShake();

            string message = JObject.FromObject(r).ToString();

            RNBridge.SendMessage(message);
        }

        public static void SendEvent(String name, object data = null)
        {
            EventResult r = new EventResult();
            r.name = name;
            r.data = data;

            string message = JObject.FromObject(r).ToString();

            RNBridge.SendMessage(message);
        }

        public static void SendResult(int id, String name, bool resolved, object data)
        {
            CommandResult r = new CommandResult();
            r.id = id;
            r.name = name;
            r.resolved = resolved;
            r.data = data;

            string message = JObject.FromObject(r).ToString();

            RNBridge.SendMessage(message);
        }

        public static void SendMessage(String message)
        {
#if UNITY_ANDROID
            try
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.wowmaking.rnunity.UnityReactActivity");
                AndroidJavaObject unityReactActivity = jc.GetStatic<AndroidJavaObject>("instance");
                unityReactActivity.Call("sendMessage", message);
            }
            catch (Exception e)
            {
                Debug.Log("Exception during sendMessage to UnityReactActivity");
                Debug.Log(e.Message);
            }
#elif UNITY_IOS
            NativeAPI.sendMessage(message);
#endif
        }

    }
}