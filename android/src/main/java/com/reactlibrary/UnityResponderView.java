package com.reactlibrary;

import android.content.Context;
import android.view.MotionEvent;
import android.view.View;

public class UnityResponderView extends View {

    public UnityResponderView(Context context) {
        super(context);
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        return UnityReactActivity.instance.onTouchEvent(event);
    }
}