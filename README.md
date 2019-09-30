
# @wowmaking/react-native-unity

## Getting started

`$ npm install @wowmaking/react-native-unity --save`

### Installation

1. Install package via `npm`
2. Move your Unity project to `unity` folder at project root

#### iOS

1. Run `pod install`
2. Build Unity app to `[project_root]/unity/builds/ios`
3. Add `Unity-iPhone.xcodeproj` to your workspace: `Menu` -> `File` -> `Add Files to [workspace_name]...` -> `[project_root]/unity/builds/ios/Unity-iPhone.xcodeproj`
4. Add `UnityFramework.framework` to `Embedded Binaries`: 
    - select `your_app` target in workspace
    - in `General` / `Embedded Binaries` press `+`
    - select `Unity-iPhone/Products/UnityFramework.framework`
    - remove `UnityFramework.framework` from `Linked Frameworks and Libraries` ( select it and press `-` )
    - in `Build Phases` move `Embedded Binaries` before `Compile Sources` ( drag and drop )
    ![Example](https://forum.unity.com/attachments/image1-png.427024/)
5. Expose `NativeCallProxy.h`. Native application implements NativeCallsProtocol defined in following file.
    - find and select `Unity-iPhone/Libraries/com.wowmaking.react-native-unity/Plugins/iOS/NativeCallProxy.h`
    - enable `UnityFramework` in `Target Membership` and set `Public` header visibility (small dropdown on right side to `UnityFramework`)
![Example](https://forum.unity.com/attachments/image7-png.427027/)
6. Make `Data` folder to be part of the `UnityFramework`. By default `Data` folder is part of Unity-iPhone target, we change that to make everything encapsulated in one single framework file. Change `Target Membership` for `Data` folder to `UnityFramework`.
![Example](https://forum.unity.com/attachments/image4-png.427030/)
7. Add following lines to your project `main.m` file (located at same folder with `AppDelegate`)
```objectivec
#import <UIKit/UIKit.h>
+++ #import <RNUnity/RNUnity.h>

#import "AppDelegate.h"

int main(int argc, char * argv[]) {
  @autoreleasepool {
    +++ [RNUnity setArgc:argc];
    +++ [RNUnity setArgv:argv];
    return UIApplicationMain(argc, argv, nil, NSStringFromClass([AppDelegate class]));
  }
}
```
8. Add following lines to your project `AppDelegate.m` file
```objectivec
#import "AppDelegate.h"

#import <React/RCTBridge.h>
#import <React/RCTBundleURLProvider.h>
#import <React/RCTRootView.h>
+++ #import <RNUnity/RNUnity.h>

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
   +++ [RNUnity launchWithOptions:launchOptions]; // IMPORTANT to add this before react view creation
  RCTBridge *bridge = [[RCTBridge alloc] initWithDelegate:self launchOptions:launchOptions];
  ...
}

+++ - (void)applicationWillResignActive:(UIApplication *)application { [[[RNUnity ufw] appController] applicationWillResignActive: application]; }
+++ - (void)applicationDidEnterBackground:(UIApplication *)application { [[[RNUnity ufw] appController] applicationDidEnterBackground: application]; }
+++ - (void)applicationWillEnterForeground:(UIApplication *)application { [[[RNUnity ufw] appController] applicationWillEnterForeground: application]; }
+++ - (void)applicationDidBecomeActive:(UIApplication *)application { [[[RNUnity ufw] appController] applicationDidBecomeActive: application]; }
+++ - (void)applicationWillTerminate:(UIApplication *)application { [[[RNUnity ufw] appController] applicationWillTerminate: application]; }

@end
```

#### Android

1. Add ndk support into `android/app/build.gradle`
    ```gradle
    defaultConfig {
        ...
        ndk {
            abiFilters "armeabi", "armeabi-v7a", "x86", "mips"
        }
    }
    ```
2. Append the following lines to `android/settings.gradle`:
  	```gradle
  	include ':unityLibrary'
    project(':unityLibrary').projectDir=new File('..\\unity\\builds\\android\\unityLibrary')
  	```
3. Insert the following lines inside the dependencies block in `android/app/build.gradle`:
  	```gradle
      implementation project(':unityLibrary')
      implementation files("${project(':unityLibrary').projectDir}/libs/unity-classes.jar")
  	```
4. Change parent activity in `MainActivity.java` from `ReactActivity` to `UnityReactActivity`
    ```java
    public class MainActivity extends UnityReactActivity {
        ...
    }
    ```
    
#### Unity
1. Add following line at your `unity/Packages/manifest.json`
    ```json
    {
        ...
        "com.wowmaking.react-native-unity": "file:../../node_modules/@wowmaking/react-native-unity/unity"
    }
    ```
2. Right-click at you `Hierarchy` menu, then click `Create Empty`, rename new game object (example: `UICommandsDelegate`). **IMPORTANT! Remember the name of new game object, that needed to initialize JavaScript lib.**
3. Add script `RNCommadsDelegate.cs` to new game object (step 2) (location: `Packages/react-native-unity/Runtime/Scripts`)
4. To receive commands from JavaScript, you must create another game object, or use existing. Commands receiver object must implements `IRNCommandsReceiver` interface
    ```c
    using Wowmaking.RNU;
    
    public class NewGameObject : MonoBehaviour, IRNCommandsReceiver
    {
    ...
    }
    ```
5. Set your object as commands receiver to `RNBridge` on `Awake`
    ```c
    using Wowmaking.RNU;
    
    public class NewGameObject : MonoBehaviour, IRNCommandsReceiver
    {
        private void Awake()
        {
            RNBridge.SetCommandsReceiver(this);
        }
    }
    ```
6. Implement `IRNCommandsReceiver` interface by adding `HandleCommand` method
    ```c
    using Wowmaking.RNU;
    
    public class NewGameObject : MonoBehaviour, IRNCommandsReceiver
    {
        private void Awake()
        {
            RNBridge.SetCommandsReceiver(this);
        }
        
        public void HandleCommand(RNCommand command)
        {
            switch (command.name)
            {
                // command.Resolve(new {}) || command.Reject(new {})
            }
        }
    }
    ``` 
    **IMPORTANT! Call `Resolve` or `Reject` method of received `RNCommand` instance to remove it from JavaScript thread**

## Usage
```javascript
import { Unity, UnityResponderView } from '@wowmaking/react-native-unity';

// Don't forget to initialize with name of GameObject, that you create at `Unity`->`Step 2`
Unity.init('UICommandsDelegate');

const App = () => {
  return (
    <View>
      <!-- UnityResponderView provide all touch events to Unity -->
      <UnityResponderView />
      <Touchable onPress={()=>Unity.execCommand('command_name', { /* any specific command data */ })}>Press ME!</Touchable>
    </View>
  );
};
```

## JavaScript API

##### **`Unity`** - main module object
###### Methods:
1. `init` - initialize `react-native-unity` lib
    Params: 
    - `delegateName` (`string`) - name of Unity GameObject, that was created at `Unity`->`Step 2`
    
    Usage:
    ```javascript
    Unity.init('UICommandsDelegate');
    ```
2. `execCommand` - send command to Unity
    Params: 
    - `name` (`string`) - Unity command name
    - `data` (`Object`, optional) - Unity command data
    
    Return `Promise`
    Usage:
    ```javascript
    Unity.execCommand('command_name', { a: 1, b: 'b', })
    ```
3. `addEventListener` - add listener of Unity events
    Params:
        - `type` (`string`) - type of Unity event
        - `listener` (`function`) - function, that's calling on Unity event receiving
        
    Usage:
    ```javascript
    Unity.addEventListener('event_type', (e) => { console.warn(e); });
    ```
4. `removeEventListener` - remove Unity event listener
   Params:
        - `type` (`string`) - type of Unity event
        - `listener` (`function`) - specific listener to remove
        
    Usage:
    ```javascript
    Unity.addEventListener('event_type', listener });
    ```
    
##### **`UnityResponderView`** - React-component, that provide all touch events to Unity

## Unity API
##### **Package namespace is `Wowmaking.RNU`**
#
##### **`interface IRNCommandsReceiver`** - interface to receive commands from JaveScript
###### Methods:
1. `void HandleCommand(RNCommand command)` - method, that calls from JavaScript
    Params:
    - `command` (`RNCommand`) - command object, received from JavaScript
    
##### **`RNCommand`** - class of reciving JavaScript commands
###### Properties
1. `name` (`string`) - name of received command
2. `data` ('object') - data of received command
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
1. `SetCommandsReceiver` - set commands reveiver to bridge
    Params:
    - `cReceiver` (`IRNCommandsReceiver`) - game object, that implements IRNCommandsReceiver interface
        
    Usage:
    ```c
    private void Awake()
    {
        RNBridge.SetCommandsReceiver(this);
    }
    ```
2. `SendEvent` - send event to JavaScript
    Params:
    - `name` (`string`) - event name, that receive JavaScript
    - `data` (`object`) - data object, that receive JavaScript listeners