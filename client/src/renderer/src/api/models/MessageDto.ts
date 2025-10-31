/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { MessageSender } from './MessageSender';
export type MessageDto = {
    id?: number;
    conversationId?: number;
    userId?: number;
    username?: string | null;
    sender?: MessageSender;
    content?: string | null;
    sentAt?: string;
};

