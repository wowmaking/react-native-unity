package com.wowmaking.rnunity;

import com.facebook.react.uimanager.SimpleViewManager;
import com.facebook.react.uimanager.ThemedReactContext;

import javax.annotation.Nonnull;

public class RNUnityManager extends SimpleViewManager<UnityResponderView> {
    public static final String REACT_CLASS = "UnityResponderView";

    @Nonnull
    @Override
    public String getName() {
        return REACT_CLASS;
    }

    @Nonnull
    @Override
    protected UnityResponderView createViewInstance(@Nonnull ThemedReactContext reactContext) {
        return new UnityResponderView(reactContext);
    }
}
