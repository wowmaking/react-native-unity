
# @wowmaking/react-native-unity

## Getting started

`$ npm install @wowmaking/react-native-unity --save`

### Mostly automatic installation

`$ react-native link @wowmaking/react-native-unity`

### Manual installation


#### iOS

1. In XCode, in the project navigator, right click `Libraries` ➜ `Add Files to [your project's name]`
2. Go to `node_modules` ➜ `@wowmaking/react-native-unity` and add `RNUnity.xcodeproj`
3. In XCode, in the project navigator, select your project. Add `libRNUnity.a` to your project's `Build Phases` ➜ `Link Binary With Libraries`
4. Run your project (`Cmd+R`)<

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

#### Windows
[Read it! :D](https://github.com/ReactWindows/react-native)

1. In Visual Studio add the `RNUnity.sln` in `node_modules/@wowmaking/react-native-unity/windows/RNUnity.sln` folder to their solution, reference from their app.
2. Open up your `MainPage.cs` app
  - Add `using Unity.RNUnity;` to the usings at the top of the file
  - Add `new RNUnityPackage()` to the `List<IReactPackage>` returned by the `Packages` method


## Usage
```javascript
import RNUnity from '@wowmaking/react-native-unity';

// TODO: What to do with the module?
RNUnity;
```
  