# @wowmaking/react-native-unity

> :warning: **Warning:** This is **v1.0** documentation. Please use [this link](https://github.com/wowmaking/react-native-unity/tree/v0.5.0) to read stable version 0.5.0 docs

## Package installation

```bash
npm install @wowmaking/react-native-unity
```

## Unity project setup

1. Move your Unity project to `unity` folder at RN project root

2. Add [Newtonsoft Json Unity Package](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.1) to Unity project

3. Add the following line at your `[rn_project_root]/unity/Packages/manifest.json`:
   ```json
   {
     ...
     "com.wowmaking.react-native-unity": "file:../../node_modules/@wowmaking/react-native-unity/unity"
   }
   ```
4. To receive commands from JavaScript create a new game object or use existing one. Commands receiver object should implement `IRNCommandsReceiver` interface:

   ```csharp
   using Wowmaking.RNU;

   public class RNCommandsReceiver : MonoBehaviour, IRNCommandsReceiver
   {
      ...
   }
   ```

5. Set your object as commands receiver to `RNBridge` on `Awake`:

   ```csharp
   using Wowmaking.RNU;

   public class RNCommandsReceiver : MonoBehaviour, IRNCommandsReceiver
   {
      private void Awake()
      {
         RNBridge.RegisterCommandsReceiver(this);
      }
   }
   ```

6. Implement `IRNCommandsReceiver` interface by adding `HandleCommand` method:

   ```csharp
   using Wowmaking.RNU;

   public class RNCommandsReceiver : MonoBehaviour, IRNCommandsReceiver
   {
      private void Awake()
      {
         RNBridge.RegisterCommandsReceiver(this);
      }

      public void HandleCommand(RNCommand command)
      {
         switch (command.name)
         {
            command.Resolve(new {});
            // command.Reject(new {});
         }
      }
   }
   ```

   > :warning: **Important:** Call `Resolve` or `Reject` method of received `RNCommand` instance to remove it from JavaScript thread

## iOS project setup

1. Run `pod install`

2. Export Unity app for iOS using Unity Editor menu options:

   `ReactNative → Export iOS (Device)` - exports to `[rn_project_root]/unity/builds/ios_device`

   `ReactNative → Export iOS (Simulator)` - exports to `[rn_project_root]/unity/builds/ios_simulator`

3. Rename the folder with required sdk (`ios_device` or `ios_simulator`) to `ios`

4. Add `Unity-iPhone.xcodeproj` to your RN project workspace in Xcode and add UnityFramework.framework (see **step 3** and **step 4**) of [Integrating Unity as a library into standard iOS app](https://github.com/Unity-Technologies/uaal-example/blob/master/docs/ios.md) guide

5. Add the following lines to your project `main.m` file (located at same folder with `AppDelegate`)

   ```objectivec
   #import <UIKit/UIKit.h>
   #import <RNUnity/RNUnity.h> // ← Add this line

   #import "AppDelegate.h"

   int main(int argc, char * argv[]) {
      @autoreleasepool {
         [RNUnity setArgc:argc]; // ← Add this line
         [RNUnity setArgv:argv]; // ← Add this line
         return UIApplicationMain(argc, argv, nil, NSStringFromClass([AppDelegate class]));
      }
   }
   ```

6. Add the following lines to `AppDelegate.m` file

   ```objectivec
   #import "AppDelegate.h"

   #import <React/RCTBridge.h>
   #import <React/RCTBundleURLProvider.h>
   #import <React/RCTRootView.h>
   #import <RNUnity/RNUnity.h> // ← Add this line

   @implementation AppDelegate

   - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
   {
      [RNUnity launchWithOptions:launchOptions]; // ← Add this line before React root view creation
      RCTBridge *bridge = [[RCTBridge alloc] initWithDelegate:self launchOptions:launchOptions];
      ...
   }

   // ↓ Add these lines
   - (void)applicationWillResignActive:(UIApplication *)application { [[[RNUnity ufw] appController] applicationWillResignActive: application]; }
   - (void)applicationDidEnterBackground:(UIApplication *)application { [[[RNUnity ufw] appController] applicationDidEnterBackground: application]; }
   - (void)applicationWillEnterForeground:(UIApplication *)application { [[[RNUnity ufw] appController] applicationWillEnterForeground: application]; }
   - (void)applicationDidBecomeActive:(UIApplication *)application { [[[RNUnity ufw] appController] applicationDidBecomeActive: application]; }
   - (void)applicationWillTerminate:(UIApplication *)application { [[[RNUnity ufw] appController] applicationWillTerminate: application]; }
   // ↑ Add these lines

   @end
   ```

7. Make React root view background color transparent in `AppDelegate.m` file:

   ```objectivec
   rootView.backgroundColor = [[UIColor alloc] initWithRed:1.0f green:1.0f blue:1.0f alpha:0];
   ```

   For RN v0.71+ this line is located in `[rn_project_root]/node_modules/react-native/Libraries/AppDelegate/RCTAppDelegate.mm`, so use [patch-package](https://www.npmjs.com/package/patch-package) to modify it

## Android project setup

1. Export Unity app for Android using Unity Editor menu option:

   `ReactNative → Export Android` - exports to `[rn_project_root]/unity/builds/android`

2. Configure your project to use only `armeabi-v7a` and `arm64-v8a` architectures in `[rn_project_root]/android/gradle.properties`:

   ```gradle
   reactNativeArchitectures=armeabi-v7a,arm64-v8a
   ```

   For older RN versions add ndk section to `[rn_project_root]/android/app/build.gradle`:

   ```gradle
   defaultConfig {
      ...
      ndk {
         abiFilters "armeabi-v7a", "arm64-v8a"
      }
   }
   ```

3. Append the following lines to `[rn_project_root]/android/settings.gradle`:

   ```gradle
   include ':unityLibrary'
   project(':unityLibrary').projectDir=new File('..\\unity\\builds\\android\\unityLibrary')
   ```

4. Insert the following lines inside the dependencies block in `[rn_project_root]/android/app/build.gradle`:

   ```gradle
   implementation project(':unityLibrary')
   implementation files("${project(':unityLibrary').projectDir}/libs/unity-classes.jar")
   ```

5. Change parent activity in `MainActivity.java` from `ReactActivity` to `UnityReactActivity`

   ```java
   import com.wowmaking.rnunity.UnityReactActivity;

   public class MainActivity extends UnityReactActivity {
      ...
   }
   ```

If you have any troubles while configuring Android project, please refer to [Integrating Unity as a library into standard Android app](https://github.com/Unity-Technologies/uaal-example/blob/master/docs/android.md) guide

## JavaScript API

```javascript
import { Unity, UnityResponderView } from '@wowmaking/react-native-unity';

Unity.init();

const App = () => {
  const handlePress = () => {
    Unity.execCommand('command_name', {
      /* any specific command data */
    });
  };

  return (
    <View>
      {/* UnityResponderView provides all touch events to Unity */}
      <UnityResponderView />
      <Touchable onPress={handlePress}>Press me!</Touchable>
    </View>
  );
};
```

##### **`Unity`** - main module object

###### Methods:

1. `init` - initialize `react-native-unity` lib

   Usage:

   ```javascript
   Unity.init();
   ```

2. `execCommand` - send command to Unity

   Params:

   - `name` (`string`) - Unity command name
   - `data` (`Object`, optional) - Unity command data

   Return `Promise`

   Usage:

   ```javascript
   Unity.execCommand('command_name', { a: 1, b: 'b' });
   ```

3. `addEventListener` - add listener of Unity events

   Params: - `type` (`string`) - type of Unity event - `listener` (`function`) - function, that's calling on Unity event receiving

   Usage:

   ```javascript
   Unity.addEventListener('event_type', e => {
     console.warn(e);
   });
   ```

4. `removeEventListener` - remove Unity event listener
   Params: - `type` (`string`) - type of Unity event - `listener` (`function`) - specific listener to remove
   Usage:
   ```javascript
   Unity.addEventListener('event_type', { listener });
   ```

##### **`UnityResponderView`** - React-component that provides all touch events to Unity

## Unity API

##### **Package namespace is `Wowmaking.RNU`**

##### **`interface IRNCommandsReceiver`** - interface to receive commands from JaveScript

###### Methods:

1. `void HandleCommand(RNCommand command)` - method, that calls from JavaScript

   Params:

   - `command` (`RNCommand`) - command object, received from JavaScript

##### **`RNCommand`** - class of reciving JavaScript commands

###### Properties

1. `name` (`string`) - name of received command
2. `data` (`object`) - data of received command

###### Methods

1. `Resolve` - invoke on successful command execution

   Params:

   - `data` (`object`, optional) - object, that will receive JavaScript

   Usage:

   ```c
   command.Resolve(new { text = "test", });
   ```

2. `Reject` - invoke on unsuccessful command execution

   Params:

   - `data` (`object`, optional) - object, that will receive JavaScript

   Usage:

   ```c
   command.Reject(new { text = "test", });
   ```

##### **`static RNBridge`**

###### Methods

1. `RegisterCommandsReceiver` - add commands reveiver to bridge

   Params:

   - `cReceiver` (`IRNCommandsReceiver`) - game object, that implements IRNCommandsReceiver interface

   Usage:

   ```c
   private void Awake()
   {
       RNBridge.RegisterCommandsReceiver(this);
   }
   ```

2. `SendEvent` - send event to JavaScript

   Params:

   - `name` (`string`) - event name, that receive JavaScript
   - `data` (`object`) - data object, that receive JavaScript listeners

## CLI

You can add some useful commands to `package.json` file of your RN project:

```json
{
  "var": {
    "unity": "/Applications/Unity/Hub/Editor/2020.3.44f1/Unity.app/Contents/MacOS/Unity -batchmode -quit -logFile - -projectPath ./unity -buildTarget"
  },
  "scripts": {
    "unity-export-android": "${npm_package_var_unity} Android -executeMethod Wowmaking.RNU.Editor.RNBuild.PerformAndroidBuild",
    "unity-export-device-ios": "${npm_package_var_unity} iOS -executeMethod Wowmaking.RNU.Editor.RNBuild.PerformIOSBuildForDevice",
    "unity-export-simulator-ios": "${npm_package_var_unity} iOS -executeMethod Wowmaking.RNU.Editor.RNBuild.PerformIOSBuildForSimulator",
    "unity-select-device-ios": "cd ./unity/Builds && rm -f ./ios && ln -s ./ios_device ./ios",
    "unity-select-simulator-ios": "cd ./unity/Builds && rm -f ./ios && ln -s ./ios_simulator ./ios"
  }
}
```

Replace `2020.3.44f1` with your Unity version
