#import <Foundation/Foundation.h>
#import "RNUProxy.h"


@implementation RNUProxy

id<NativeCallsProtocol> api = NULL;
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi
{
    api = aApi;
}

+(void) sendMessage:(NSString*) message
{
    [api sendMessage:message];
}

@end
