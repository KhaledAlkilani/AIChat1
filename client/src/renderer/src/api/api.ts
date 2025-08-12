import * as AuthServiceExports from "./services/AuthService";
import * as ChatServiceExports from "./services/ChatService";


export const api = {
  ...AuthServiceExports,
  ...ChatServiceExports
};


export * from "./models/LoginRequest";
export * from "./models/MessageDto";
export * from "./models/RegisterRequest";
export * from "./models/SendMessageRequest";
