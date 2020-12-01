#import "RNUProxy.h"


@interface RNUProxy : NSObject

+ (Class)RNUnity;

+ (void)setReceiverHandshake:(proxy_receiver_handshake)receiverHandshake 
			 receiverCommand:(proxy_receiver_command)receiverCommand;
+ (void)sendMessage:(const char *)message;

@end


@implementation RNUProxy

+ (Class)RNUnity {
	return NSClassFromString(@"RNUnity");
}

+ (bool)isAvailable {
	return self.RNUnity;
}

+ (void)setReceiverHandshake:(proxy_receiver_handshake)receiverHandshake receiverCommand:(proxy_receiver_command)receiverCommand {
	[self.RNUnity setReceiverHandshake:receiverHandshake
                       receiverCommand:receiverCommand];

}

+ (void)sendMessage:(const char *)message {
    [self.RNUnity sendMessage:message];
}

@end

bool RNUProxyIsAvailable() {
	return [RNUProxy isAvailable];
}

void RNUProxySetReceiver(proxy_receiver_handshake handshake, proxy_receiver_command command) {
	[RNUProxy setReceiverHandshake:handshake receiverCommand:command];
}

void RNUProxySendMessage(const char *message) {
    [RNUProxy sendMessage:message];
}
