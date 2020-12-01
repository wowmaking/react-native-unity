#include "RNUProxy.h"

#include <stdexcept>

bool RNUProxyIsAvailable() {
	return false;
}

void RNUProxySetReceiver(proxy_receiver_handshake handshake, proxy_receiver_command command) {
	throw std::runtime_error("RNUProxy not support this platform");
}

void RNUProxySendMessage(const char *message) {
	throw std::runtime_error("RNUProxy not support this platform");
}