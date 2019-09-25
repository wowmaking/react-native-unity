
import {
  NativeModules,
  requireNativeComponent,
  NativeEventEmitter,
} from 'react-native';


const { RNUnity, } = NativeModules;
const RNUnityEventEmitter = new NativeEventEmitter(RNUnity);

const _handlers = {};
let _subscriber;

const Unity = {
  /**
   * 
   * Initialize unity
   */
  init() {
    RNUnity.initialize();
    _subscriber = RNUnityEventEmitter.addListener('UnityMessage')
  },

  /**
   * 
   * Invoke entity method
   * @param {Object} options
   * @param {string} options.entityName Unity entity name
   * @param {string} options.functionName Unity entity script component function
   * @param {string=} options.message Unity entity script component function param
   */
  invoke({ entityName, functionName, message = '', }) {
    RNUnity.invoke(entityName, functionName, message);
  },

  /**
   * 
   * @private
   * @param {string} message
   */
  _handleMessage(message) {
    try {
      const messageData = JSON.parse(message);
      const { type, data, } = messageData;

      if (_handlers[type]) {
        _handlers[type].forEach((handler) => {
          if (typeof handler === 'function') {
            handler(data);
          }
        });
      }
    }
    catch (e) {
      console.warn(e.message);
    }
  },

  /**
   * 
   * Add unity messages listeners
   * @param {string} type Event type
   * @param {function} handler Event handler
   */
  addListener(type, handler) {
    if (!_handlers[type]) {
      _handlers[type] = [];
    }

    _handlers[type].push(handler);
  },

  /**
   * 
   * Remove unity messages listeners
   * @param {string} type Event type
   * @param {function} handler Event handler
   */
  removeListener(type, handler) {
    if (_handlers[type]) {
      _handlers[type] = _handlers[type].filter(h => h !== handler);
    }
  },

};

const UnityView = requireNativeComponent('UnityResponderView');

export { Unity, UnityView, }
