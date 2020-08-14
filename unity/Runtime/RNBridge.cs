using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

using Newtonsoft.Json.Linq;

namespace Wowmaking.RNU
{
    public class RNMessage
    {
        public readonly string type;
        public readonly string name;
        public readonly object data;

        public RNMessage(string type, string name, object data) 
        {
            this.type = type;
            this.name = name;
            this.data = data ?? new JObject();
        }

        public string ToJsonString()
        {
            var jobject = JObject.FromObject(this);
            return jobject.ToString();
        } 
    }

    public class RNMessageEvent : RNMessage
    {
        public RNMessageEvent(string name, object data) 
        : base("event", name, data) {}
    }

    public class RNMessageResult : RNMessage
    {
        public RNMessageResult(string name, object data)
        : base("result", name, data) {}
    }

    public class RNMessageHandshake : RNMessage
    {
        public RNMessageHandshake() 
        : base("handshake", string.Empty, null) {}
    }

    public interface IRNCommandsReceiver
    {
        void HandleCommand(RNCommand command);
    }

    public class RNCommand
    {
        public readonly int id;
        public readonly string name;
        public readonly JObject data;

        private bool resolved;
        private bool completed = false;

        public RNCommand(string message) 
        : this(JObject.Parse(message)) {}

        public RNCommand(JObject obj) 
        : this(obj.Value<int>(nameof(id)), 
            obj.Value<string>(nameof(name)), 
            obj.Value<JObject>(nameof(data))) {}
        
        public RNCommand(int id, string name, JObject data)
        {
            this.id = id;
            this.name = name;
            this.data = data;
        }
        
        public void Resolve(object resultData = null)
        {
            resolved = true;
            SendResult(resultData);
        }

        public void Reject(object resultData = null)
        {
            resolved = false;
            SendResult(resultData);
        }

        private void SendResult(object resultData = null)
        {
            if (!completed)
            {
                completed = true;
                RNCommandResult data = new RNCommandResult(id, resolved, resultData);
                RNMessageResult result = new RNMessageResult(name, data);
                RNBridge.SendResult(result);
            }
        }

        class RNCommandResult 
        {
            public readonly int id;
            public readonly bool resolved;
            public readonly object result;

            public RNCommandResult(int id, bool resolved, object resultData) 
            {
                this.id = id;
                this.resolved = resolved;
                this.result = resultData ?? new JObject();
            }
        }
    }
    
    public static class RNBridge
    {        
        // Public
        public static void RegisterCommandsReceiver(IRNCommandsReceiver cReceiver)
        {
            RegisterReceiver(cReceiver);
        }

        public static bool IsAvailable() 
        {
            return unityReact?.IsAvailable() ?? false;
        }

        public static void SendEvent(string name, object data = null) => SendMessage(new RNMessageEvent(name, data));

        public static void SendEvent(RNMessageEvent messageEvent) => SendMessage(messageEvent.ToJsonString());

        public static void SendResult(RNMessageResult messageResult) => SendMessage(messageResult.ToJsonString());

        public static void SendHandshake(RNMessageHandshake messageHandshake) => SendMessage(messageHandshake.ToJsonString());

        public static void SendMessage(RNMessage message) => SendMessage(message.ToJsonString());

        // Private
        private static System.Action<RNCommand> commandsReceivers;

        private static IUnityReact unityReact;
        
        private static System.Threading.Tasks.TaskScheduler unityScheduler;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            unityScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            if (!Application.isEditor) 
            {
                if (Debug.isDebugBuild) 
                {
                    Debug.Log($"{nameof(RNBridge)}: try initialize");
                }
                try
                {
                    if (Application.platform == RuntimePlatform.Android) 
                    {
                        unityReact = new AndroidUnityReact();
                    }
                    else
                    {
                        unityReact = new NativeUnityReact();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"{nameof(RNBridge)}: exception during try initialize <{e.Message}>");
                }
            }
        }

        private static void RegisterReceiver(IRNCommandsReceiver receiver)
        {
            bool exist = commandsReceivers != null;
            commandsReceivers += receiver.HandleCommand;            
            if (!exist) 
            {
                SentReceiver();
            }   
        }

        private static void SentReceiver()
        {
            if (Debug.isDebugBuild) 
            {
                Debug.Log($"{nameof(RNBridge)}: try set receiver");
            }            
            try
            {
                unityReact?.SetReceiver(ReceiveHandshake, ReceiveCommand);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{nameof(RNBridge)}: exception during try set receiver <{e.Message}>");
            }
        }

        private static void SendMessage(string message)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log($"{nameof(RNBridge)}: try send message <{message}>");
            }
            try 
            {
                unityReact?.SendMessage(message);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{nameof(RNBridge)}: exception during send message <{e.Message}>");
            }
        }
        
        [AOT.MonoPInvokeCallback(typeof(System.Action))]
        private static void ReceiveHandshake() 
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (commandsReceivers != null) 
                    {
                        if (Debug.isDebugBuild) 
                        {
                            Debug.Log($"{nameof(RNBridge)}: receive handshake");
                        }
                        RNBridge.SendHandshake(new RNMessageHandshake());
                    }
                }, 
                System.Threading.CancellationToken.None,
                System.Threading.Tasks.TaskCreationOptions.None, 
                unityScheduler);
        }

        [AOT.MonoPInvokeCallback(typeof(System.Action<string>))]
        private static void ReceiveCommand(string message) 
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (commandsReceivers != null)
                    {
                        if (Debug.isDebugBuild) 
                        {
                            Debug.Log($"{nameof(RNBridge)}: receive command <{message}>");
                        }            
                        commandsReceivers?.Invoke(new RNCommand(message));
                    }
                }, 
                System.Threading.CancellationToken.None,
                System.Threading.Tasks.TaskCreationOptions.None, 
                unityScheduler);
        }

        interface IUnityReact 
        {
            bool IsAvailable();
            void SetReceiver(System.Action handshake, System.Action<string> command);
            void SendMessage(string message);
        }

        class NativeUnityReact : IUnityReact 
        {
            bool IUnityReact.IsAvailable()
            {
                return RNUProxyIsAvailable();
            }

            void IUnityReact.SetReceiver(System.Action handshake, System.Action<string> command)
            {
                RNUProxySetReceiver(handshake, command);
            }

            void IUnityReact.SendMessage(string message) 
            {
                RNUProxySendMessage(message);
            }

            [DllImport("__Internal")]
            public static extern bool RNUProxyIsAvailable();
            [DllImport("__Internal")]
            public static extern void RNUProxySetReceiver(System.Action handshake, System.Action<string> command);
            [DllImport("__Internal")]
            public static extern void RNUProxySendMessage(string message);
        }

        class AndroidUnityReact : IUnityReact 
        {
            private readonly AndroidJavaObject unityReactActivity;

            public AndroidUnityReact()
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.wowmaking.rnunity.UnityReactActivity");
                unityReactActivity = jc.CallStatic<AndroidJavaObject>("getInstance");
            }

            bool IUnityReact.IsAvailable()
            {
                return unityReactActivity != null;
            }

            void IUnityReact.SetReceiver(System.Action handshake, System.Action<string> command) 
            {
                unityReactActivity.Call("setReceiver", new AndroidReceiver(handshake, command));
            }

            void IUnityReact.SendMessage(string message) 
            {
                unityReactActivity.Call("sendMessage", message);
            }

            private class AndroidReceiver : AndroidJavaProxy 
            {
                private readonly System.Action proxyHandshake;
                private readonly System.Action<string> proxyCommand;

                public AndroidReceiver(System.Action handshake, System.Action<string> command) 
                    : base("com.wowmaking.rnunity.UnityReactActivity$IUnityReceiver")
                {
                    proxyHandshake = handshake;
                    proxyCommand = command;
                }

                [UnityEngine.Scripting.Preserve]
                void receiveHandshake()
                {
                    proxyHandshake();
                }

                [UnityEngine.Scripting.Preserve]
                void receiveCommand(string arg) 
                {
                    proxyCommand(arg);
                }
            }
        }
    }
}