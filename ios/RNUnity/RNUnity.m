
#import "RNUnity.h"
#include <UnityFramework/UnityFramework.h>
#include <RNUProxy/RNUProxy.h>


UnityFramework* UnityFrameworkLoad()
{
  NSString* bundlePath = nil;
  bundlePath = [[NSBundle mainBundle] bundlePath];
  bundlePath = [bundlePath stringByAppendingString: @"/Frameworks/UnityFramework.framework"];
  
  NSBundle* bundle = [NSBundle bundleWithPath: bundlePath];
  if ([bundle isLoaded] == false) [bundle load];
  
  UnityFramework* ufw = [bundle.principalClass getInstance];
  if (![ufw appController])
  {
    // unity is not initialized
    [ufw setExecuteHeader: &_mh_execute_header];
  }
  return ufw;
}

@implementation RNUnity
{
    bool initialized;
    bool hasListeners;
}

static int argc;
+ (int) argc
{ @synchronized(self) { return argc; } }
+ (void) setArgc:(int)val
{ @synchronized(self) { argc = val; } }

static char** argv;
+ (char**) argv
{ @synchronized(self) { return argv; } }
+ (void) setArgv:(char**)val
{ @synchronized(self) { argv = val; } }

static UnityFramework* ufw;
+ (UnityFramework*) ufw
{ @synchronized(self) { return ufw; } }
+ (void) setUfw:(UnityFramework*)val
{ @synchronized(self) { ufw = val; } }

+ (UnityFramework*) launchWithOptions:(NSDictionary*)launchOptions
{
    @synchronized(self) {
         ufw = UnityFrameworkLoad();
        [ufw setDataBundleId: "com.unity3d.framework"];
        
        [ufw runEmbeddedWithArgc: argc argv: argv appLaunchOpts: launchOptions];
        
        return ufw;
    }
}

-(void)startObserving {
    hasListeners = YES;
}

-(void)stopObserving {
    hasListeners = NO;
}

RCT_EXPORT_METHOD(initialize)
{
    initialized = YES;
    [[RNUnity ufw] registerFrameworkListener: self];
    [NSClassFromString(@"RNUProxy") registerAPIforNativeCalls:self];
}

RCT_EXPORT_METHOD(invoke:(NSString *)entityName functionName:(NSString *)functionName message:(NSString *)message)
{
    if (initialized) {
        [[RNUnity ufw] sendMessageToGOWithName:[entityName UTF8String] functionName:[functionName UTF8String] message:[message UTF8String]];
    }
}

- (void)sendMessage:(NSString*)message
{
    if (hasListeners) {
        [self sendEventWithName:@"UnityMessage" body:message];
    }
}

- (NSArray<NSString *> *)supportedEvents
{
  return @[@"UnityMessage"];
}

- (dispatch_queue_t)methodQueue
{
    return dispatch_get_main_queue();
}

RCT_EXPORT_MODULE()

@end
  
