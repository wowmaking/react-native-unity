import { NativeModule } from 'react-native';

export interface RNUnityNativeModule extends NativeModule {
  initialize: () => void;
  invokeCommand: (message: string) => void;
  invokeHandshake: () => void;
}

export type UnityMessageType = 'handshake' | 'event' | 'result';

export interface UnityMessageData {
  handshake: {};
  event: any;
  result: {
    id: number;
    resolved: boolean;
    result: any;
  };
}

export interface UnityMessageVariant<T extends UnityMessageType> {
  type: T;
  name: string;
  data: UnityMessageData[T];
}

export type UnityMessage = {
  [T in UnityMessageType]: UnityMessageVariant<T>;
}[UnityMessageType];
