#import <Foundation/Foundation.h>

#import <UIKit/UIKit.h>

#include <mach-o/ldsyms.h>

#import <React/RCTBridgeModule.h>
#import <React/RCTEventEmitter.h>


@protocol RNUnityAppController <UIApplicationDelegate>

- (UIWindow *)window;

@end


@protocol RNUnityFramework <NSObject>

+ (id<RNUnityFramework>)getInstance;
- (id<RNUnityAppController>)appController;

- (void)setExecuteHeader:(const typeof(_mh_execute_header)*)header;
- (void)setDataBundleId:(const char*)bundleId;

- (void)runEmbeddedWithArgc:(int)argc argv:(char*[])argv appLaunchOpts:(NSDictionary*)appLaunchOpts;

@end


typedef void (*unity_receive_handshake)(const char *);
typedef void (*unity_receive_command)(const char *, const char *);


@interface RNUnity : RCTEventEmitter <RCTBridgeModule>

@property (atomic, class) int argc;
@property (atomic, class) char** argv;

+ (id<RNUnityFramework>)launchWithOptions:(NSDictionary*)launchOptions;

+ (id<RNUnityFramework>)ufw;

+ (void)setReceiverHandshake:(unity_receive_handshake)receiverHandshake
			 receiverCommand:(unity_receive_command)receiverCommand;
+ (void)sendMessage:(const char *)message;

@end
  
