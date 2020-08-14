#ifndef RNUProxy_h
#define RNUProxy_h

#if defined(__cplusplus)
#define RN_UNITY_EXTERN extern "C"
#else
#define RN_UNITY_EXTERN extern
#endif


typedef void (*proxy_receiver_handshake)();
typedef void (*proxy_receiver_command)(const char *);

RN_UNITY_EXTERN bool RNUProxyIsAvailable();
RN_UNITY_EXTERN void RNUProxySetReceiver(proxy_receiver_handshake, proxy_receiver_command);
RN_UNITY_EXTERN void RNUProxySendMessage(const char*);

#endif /* RNUProxy_h */
