import { FC } from 'react';
import {
  NativeModules,
  NativeEventEmitter,
  requireNativeComponent,
  ViewProps,
} from 'react-native';
import { EventTarget } from 'event-target-shim';

import { RNUnityNativeModule, UnityMessage } from './model';

const { RNUnity } = NativeModules as { RNUnity: RNUnityNativeModule };
const RNUnityEventEmitter = new NativeEventEmitter(RNUnity);

class UnityManager extends EventTarget {
  private handshake: UnityCommand | null = null;
  private handshakeResolved: boolean = false;
  private commandsMap: { [id: number]: UnityCommand<any> } = {};
  private commandsIdIterator: number = 0;

  constructor() {
    super();
  }

  init() {
    RNUnity.initialize();

    RNUnityEventEmitter.addListener(
      'UnityMessage',
      this.handleMessage.bind(this),
    );

    if (this.handshake === null) {
      this.handshake = new UnityCommand();

      this.handshake.promise.then(res => {
        this.handshakeResolved = true;
        return res;
      });

      this.checkHandshake();
    }

    return this.handshake.promise;
  }

  execCommand<R = undefined>(name: string, data?: object) {
    const id = ++this.commandsIdIterator;
    const command = new UnityCommand<R>(id, name, data);

    this.commandsMap[id] = command;
    this.invokeCommand(command.getMessage());

    return command.promise;
  }

  private checkHandshake() {
    if (!this.handshakeResolved) {
      this.invokeHandshake();
      setTimeout(this.checkHandshake.bind(this), 300);
    }
  }

  private invokeHandshake() {
    RNUnity.invokeHandshake();
  }

  private invokeCommand(message: string) {
    RNUnity.invokeCommand(message);
  }

  private handleMessage(message: string) {
    try {
      const messageData = JSON.parse(message) as UnityMessage;
      const { type, name, data } = messageData;

      switch (type) {
        case 'handshake': {
          this.handshake?.resolve();
          break;
        }
        case 'event': {
          this.dispatchEvent({ type: name, data });
          break;
        }
        case 'result': {
          const { id, resolved, result } = data;
          if (this.commandsMap[id]) {
            const command = this.commandsMap[id];

            if (resolved) {
              command.resolve(result);
            } else {
              command.reject(result);
            }

            delete this.commandsMap[id];
          }
          break;
        }
      }
    } catch (e) {
      console.warn(e instanceof Error ? e.message : e);
    }
  }
}

class UnityCommand<R = undefined> {
  private id: number | null = null;
  private name: string | null = null;
  private data: object | null = null;

  promise: Promise<R>;
  resolve: (result?: any) => void;
  reject: (result?: any) => void;

  constructor(id?: number, name?: string, data?: object) {
    this.id = id || null;
    this.name = name || null;
    this.data = data || null;
    this.promise = new Promise((res, rej) => {
      this.resolve = res;
      this.reject = rej;
    });
  }

  getMessage(): string {
    return JSON.stringify({
      id: this.id,
      name: this.name,
      data: this.data,
    });
  }
}

const Unity = new UnityManager();

const UnityResponderView = requireNativeComponent(
  'UnityResponderView',
) as unknown as FC<ViewProps>;

export { Unity, UnityResponderView };
