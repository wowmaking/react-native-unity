#import <Foundation/Foundation.h>

// NativeCallsProtocol defines protocol with methods you want to be called from managed
@protocol NativeCallsProtocol
@required
- (void) sendMessage:(NSString*)message;
@end

__attribute__ ((visibility("default")))
@interface RNUProxy : NSObject
// call it any time after UnityFrameworkLoad to set object implementing NativeCallsProtocol methods
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi;
+(void) sendMessage:(NSString*) message;
@end


