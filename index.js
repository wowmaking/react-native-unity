
import {
  NativeModules,
  requireNativeComponent,
  NativeEventEmitter,
} from 'react-native';

import { EventTarget, } from 'event-target-shim';


const { RNUnity, } = NativeModules;
const RNUnityEventEmitter = new NativeEventEmitter(RNUnity);

const _handlers = {};
let _subscriber;

class UnityManager extends EventTarget {

  _subscriber = null;

  /**
   * 
   * Initialize unity
   */
  init() {
    RNUnity.initialize();
    this._subscriber = RNUnityEventEmitter.addListener('UnityMessage', this._handleMessage.bind(this));
  }

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
  }

  /**
   * 
   * @private
   * @param {string} message
   */
  _handleMessage(message) {
    try {
      const messageData = JSON.parse(message);
      const { type, name, data, } = messageData;

      if (type === 'event') {
        this.dispatchEvent({ type: name, data, });
      }
    }
    catch (e) {
      console.warn(e.message);
    }
  }

}

const Unity = new UnityManager();
const UnityView = requireNativeComponent('UnityResponderView');

export { Unity, UnityView, }
