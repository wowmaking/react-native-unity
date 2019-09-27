
package com.reactlibrary;

import androidx.annotation.Nullable;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.modules.core.DeviceEventManagerModule;

public class RNUnityModule extends ReactContextBaseJavaModule {

    private static RNUnityModule instance;

    private final ReactApplicationContext reactContext;

    public RNUnityModule(ReactApplicationContext reactContext) {
        super(reactContext);
        instance = this;
        this.reactContext = reactContext;
    }

    public static RNUnityModule getInstance() {
        return instance;
    }

    @Override
    public String getName() {
        return "RNUnity";
    }

    @ReactMethod
    public void initialize() {
    }

    @ReactMethod
    public void invoke(String entityName, String functionName, String message) {
        UnityReactActivity.UnitySendMessage(entityName, functionName, message);
    }

    public static void sendEvent(String eventName, @Nullable String params) {
        RNUnityModule.getInstance().reactContext
                .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                .emit(eventName, params);
    }
}