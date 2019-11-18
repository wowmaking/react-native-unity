
#import <React/RCTBridgeModule.h>
#import <React/RCTEventEmitter.h>
#include <UnityFramework/UnityFramework.h>


@interface RNUnity : RCTEventEmitter <RCTBridgeModule>

+ (int) argc;
+ (void) setArgc:(int)val;

+ (char**) argv;
+ (void) setArgv:(char**)val;

+ (UnityFramework*) ufw;
+ (void) setUfw:(UnityFramework*)val;

+ (UnityFramework*) launchWithOptions:(NSDictionary*)launchOptions;

@end
  
