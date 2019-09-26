
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
    public static class Bridge
    {
        public static void sendEvent(String name, object data)
        {
            string message = JsonUtility.ToJson(new {
                  type = "event",
                  name,
                  data,
              });

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