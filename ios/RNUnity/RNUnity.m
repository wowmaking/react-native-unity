#import "RNUnity.h"

@interface RNUnity ()

@property (atomic, class) id<RNUnityFramework> ufw;

@property (nonatomic) BOOL hasListeners;

- (const char *)toSharpString:(NSString *)string;

@end


@implementation RNUnity

static char ** _RNUnity_argv;

+ (char **)argv {
    @synchronized (self) {
        return _RNUnity_argv;
    }
}

+ (void)setArgv:(char **)argv {
    @synchronized (self) {
        _RNUnity_argv = argv;
    }
}

static int _RNUnity_argc;

+ (int)argc {
    @synchronized (self) {
        return _RNUnity_argc;
    }
}

+ (void)setArgc:(int)argc {
    @synchronized (self) {
        _RNUnity_argc = argc;
    }
}

static id<RNUnityFramework> _RNUnity_ufw;

+ (id<RNUnityFramework>)ufw {
    @synchronized (self) {
        return _RNUnity_ufw;
    }
}

+ (void)setUfw:(id<RNUnityFramework>)ufw {
    @synchronized (self) {
        _RNUnity_ufw = ufw;
    }
}

+ (id<RNUnityFramework>) launchWithOptions:(NSDictionary*)launchOptions {
    if (!self.ufw) {
        NSString* bundlePath = nil;
        bundlePath = [[NSBundle mainBundle] bundlePath];
        bundlePath = [bundlePath stringByAppendingString: @"/Frameworks/UnityFramework.framework"];
        
        NSBundle* bundle = [NSBundle bundleWithPath: bundlePath];
        if ([bundle isLoaded] == false) [bundle load];
        
        id<RNUnityFramework> framework = [bundle.principalClass getInstance];
        if (![framework appController]) {
            // unity is not initialized
            [framework setExecuteHeader: &_mh_execute_header];
        }
        [framework setDataBundleId: [bundle.bundleIdentifier cStringUsingEncoding:NSUTF8StringEncoding]];
        [framework runEmbeddedWithArgc: self.argc argv: self.argv appLaunchOpts: launchOptions];
            
        self.ufw = framework;
    }
    return self.ufw;
}

static RNUnity *_RNUnity_sharedInstance;

RCT_EXPORT_METHOD(initialize) {
    _RNUnity_sharedInstance = self;
}

-(void)startObserving {
    self.hasListeners = YES;
}

-(void)stopObserving {
    self.hasListeners = NO;
}

- (NSArray<NSString *> *)supportedEvents {
    return @[@"UnityMessage"];
}

- (dispatch_queue_t)methodQueue {
    return dispatch_get_main_queue();
}

static unity_receive_handshake _RNUnity_receiver_handshake;
static unity_receive_command _RNUnity_receiver_command;

+ (void)setReceiverHandshake:(unity_receive_handshake)receiverHandshake
             receiverCommand:(unity_receive_command)receiverCommand {
    _RNUnity_receiver_handshake = receiverHandshake;
    _RNUnity_receiver_command = receiverCommand;
}

RCT_EXPORT_METHOD(invokeHandshake:(NSString *)entityName) {
    if (_RNUnity_sharedInstance) {
        if (_RNUnity_receiver_handshake) {
            _RNUnity_receiver_handshake([self toSharpString:entityName]);
        }
    }
}

RCT_EXPORT_METHOD(invokeCommand:(NSString *)entityName message:(NSString *)message) {
    if (_RNUnity_sharedInstance) {
        if (_RNUnity_receiver_command) {
            _RNUnity_receiver_command([self toSharpString:entityName], [self toSharpString:message]);
        }
    }
}

+ (void)sendMessage:(const char *)message {
    [_RNUnity_sharedInstance sendMessage:[NSString stringWithUTF8String:message]];
}

- (void)sendMessage:(NSString *)message {
    if (self.hasListeners) {
        [self sendEventWithName:@"UnityMessage" body:message];
    }
}

RCT_EXPORT_MODULE()

- (const char *)toSharpString:(NSString *)string {
    const char * result = NULL;
    const char * cString = [string cStringUsingEncoding:NSUTF8StringEncoding];
    size_t sizeString = strlen(cString);
    if (sizeString > 0) {
        char *copyString = (char *)malloc(sizeString + 1);
        strcpy(copyString, cString);
        result = copyString;
    }
    return result;
}

@end
