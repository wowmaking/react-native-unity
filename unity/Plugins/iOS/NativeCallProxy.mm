#import <Foundation/Foundation.h>
#import "RNUProxy/RNUProxy.h"

extern "C" {
    void sendMessage(const char* message) { return [RNUProxy sendMessage:[NSString stringWithUTF8String:message]]; }
}
