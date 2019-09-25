package com.company.product;
import android.os.Bundle;
import android.widget.FrameLayout;

import com.unity3d.player.UnityPlayerActivity;

public abstract class OverrideUnityActivity extends UnityPlayerActivity
{ 
  public static OverrideUnityActivity instance = null;

  protected void UnitySendMessage(String gameObj, String method, String arg) { mUnityPlayer.UnitySendMessage(gameObj,method,arg); }

  protected FrameLayout getUnityFrameLayout() { return mUnityPlayer; }

  abstract protected void showMainActivity(String setToColor);
  
  @Override
  protected void onCreate(Bundle savedInstanceState)
  {
      super.onCreate(savedInstanceState);
      instance = this;
  }

  @Override
  protected void onDestroy() {
      super.onDestroy();
      instance = null;
  }
}
