
# @wowmaking/react-native-unity

## Getting started

`$ npm install @wowmaking/react-native-unity --save`

### Installation

1. Install package via `npm`
2. Move your Unity project to `unity` folder at project root

#### iOS specific steps

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
```
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
```
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

1. Open up `android/app/src/main/java/[...]/MainActivity.java`
  - Add `import com.reactlibrary.RNUnityPackage;` to the imports at the top of the file
  - Add `new RNUnityPackage()` to the list returned by the `getPackages()` method
2. Append the following lines to `android/settings.gradle`:
  	```
  	include ':@wowmaking/react-native-unity'
  	project(':@wowmaking/react-native-unity').projectDir = new File(rootProject.projectDir, 	'../node_modules/@wowmaking/react-native-unity/android')
  	```
3. Insert the following lines inside the dependencies block in `android/app/build.gradle`:
  	```
      compile project(':@wowmaking/react-native-unity')
  	```

## Usage
```javascript
import RNUnity from '@wowmaking/react-native-unity';

// TODO: What to do with the module?
RNUnity;
```
  