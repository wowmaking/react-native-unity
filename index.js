import {
  NativeModules,
  requireNativeComponent,
  NativeEventEmitter,
} from 'react-native';

import { EventTarget, } from 'event-target-shim';


const { RNUnity, } = NativeModules;
const RNUnityEventEmitter = new NativeEventEmitter(RNUnity);

const COMMANDS_DELEGATE_FN_NAME = 'HandleCommand';

let commandsIdIterator = 0;

class UnityManager extends EventTarget {

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

    this._invoke({
      entityName: this.delegateName,
      functionName: COMMANDS_DELEGATE_FN_NAME,
      message: command.getMessage(),
    });

    return command.promise;
  }

  /**
   * 
   * Invoke entity method
   * @private
   * @param {Object} options
   * @param {string} options.entityName Unity entity name
   * @param {string} options.functionName Unity entity script component function
   * @param {string=} options.message Unity entity script component function param
   */
  _invoke({ entityName, functionName, message = '', }) {
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

      switch (type) {

        case 'event':
          this.dispatchEvent({ type: name, data, });
          break;

        case 'result':
          const { id, resolved, } = messageData;
          if (this._commandsMap[id]) {
            const command = this._commandsMap[id];

            console.warn(resolved);
            if (resolved) {
              command.resolve(data);
            }
            else {
              command.reject(data);
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
const UnityView = requireNativeComponent('UnityResponderView');

export { Unity, UnityView, }
