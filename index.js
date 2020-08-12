import {
  NativeModules,
  requireNativeComponent,
  NativeEventEmitter,
} from 'react-native';

import { EventTarget, } from 'event-target-shim';


const { RNUnity, } = NativeModules;
const RNUnityEventEmitter = new NativeEventEmitter(RNUnity);

let commandsIdIterator = 0;

class UnityManager extends EventTarget {

  /**
   * @private
   * @type {UnityCommand}
   */
  _handshake = null;

  /**
   * @private
   * @type {function}
   */
  _subscriber = null;

  /** 
   * @private
   * @type {Object<number, UnityCommand>} 
   */
  _commandsMap = {};

  /**
   * Unity commands delegate name
   * @type {string}
   */
  delegateName = null;

  /**
   * 
   * Initialize unity
   * @param {string} delegateName name of GameObj, that implements IRNCommandsDelegate interface at unity
   */
  init(delegateName = '') {
    this.delegateName = delegateName;
    RNUnity.initialize();
    this._subscriber = RNUnityEventEmitter.addListener('UnityMessage', this._handleMessage.bind(this));

    if (!this._handshake) {
      this._handshake = new UnityCommand();

      this._handshake.promise
        .then((res) => {
          this._handshake.resolved = true;
          return res;
        });

      this.handshake();
    }

    return this._handshake.promise;
  }

  handshake = () => {
    if (!this._handshake.resolved) {
      this._invokeHandshake({
        entityName: this.delegateName,
      });

      setTimeout(this.handshake, 300);
    }
  }

  /**
   * 
   * Call unity command
   * @param {string} name command name
   * @param {Object=} data command data
   * @returns {Promise}
   */
  execCommand(name, data) {
    let id = ++commandsIdIterator;

    const command = new UnityCommand(id, name, data);
    this._commandsMap[id] = command;

    this._invokeCommand({
      entityName: this.delegateName,
      message: command.getMessage(),
    });

    return command.promise;
  }

  /**
   * 
   * invoke handshake method
   * @private
   * @param {Object} options
   * @param {string} options.entityName Unity entity name
   * @param {string=} options.message Unity entity script component function param
   */
  _invokeHandshake({ entityName, }) {
    RNUnity.invokeHandshake(entityName);
  }

  /**
   * 
   * invoke entity method
   * @private
   * @param {Object} options
   * @param {string} options.entityName Unity entity name
   * @param {string=} options.message Unity entity script component function param
   */
  _invokeCommand({ entityName, message = '', }) {
    RNUnity.invokeCommand(entityName, message);
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

      switch (type) {

        case 'handshake':
          this._handshake.resolve();
          break;

        case 'event':
          this.dispatchEvent({ type: name, data, });
          break;

        case 'result':
          const { id, resolved, result } = data;
          if (this._commandsMap[id]) {
            const command = this._commandsMap[id];

            if (resolved) {
              command.resolve(result);
            }
            else {
              command.reject(result);
            }

            delete this._commandsMap[id];
          }
          break;
      }

    }
    catch (e) {
      console.warn(e.message);
    }
  }

}

class UnityCommand {

  /** @type {number} */
  id = null;

  /** @type {string} */
  name = null;

  /** @type {Object} */
  data = null;

  /** @type {Promise} */
  promise = null;

  /** @type {function} */
  resolve = null;

  /** @type {function} */
  reject = null;

  /**
   * 
   * @param {number} id 
   * @param {string} name 
   * @param {Object=} data 
   */
  constructor(id, name, data) {
    this.id = id;
    this.name = name;
    this.data = data;
    this.promise = new Promise((res, rej) => {
      this.resolve = res;
      this.reject = rej;
    });
  }

  /**
   * 
   * @returns {string}
   */
  getMessage() {
    return JSON.stringify({
      id: this.id,
      name: this.name,
      data: this.data,
    });
  }

}

const Unity = new UnityManager();
const UnityResponderView = requireNativeComponent('UnityResponderView');

export { Unity, UnityResponderView, }
