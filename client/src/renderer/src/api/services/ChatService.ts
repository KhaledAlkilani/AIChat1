/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ConversationDto } from '../models/ConversationDto';
import type { MessageDto } from '../models/MessageDto';
import type { SendMessageRequest } from '../models/SendMessageRequest';
import type { StartConversationRequest } from '../models/StartConversationRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class ChatService {
    /**
     * @param requestBody
     * @returns MessageDto Created
     * @throws ApiError
     */
    public static sendMessage(
        requestBody?: SendMessageRequest,
    ): CancelablePromise<MessageDto> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/chat/send',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param requestBody
     * @returns ConversationDto OK
     * @throws ApiError
     */
    public static startNewConversation(
        requestBody?: StartConversationRequest,
    ): CancelablePromise<ConversationDto> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/chat/start',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns MessageDto OK
     * @throws ApiError
     */
    public static getMessageById(
        id: number,
    ): CancelablePromise<MessageDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/chat/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param userId
     * @returns ConversationDto OK
     * @throws ApiError
     */
    public static getConversations(
        userId?: number,
    ): CancelablePromise<Array<ConversationDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/chat/conversations',
            query: {
                'userId': userId,
            },
        });
    }
    /**
     * @param conversationId
     * @returns MessageDto OK
     * @throws ApiError
     */
    public static getConversationMessages(
        conversationId: number,
    ): CancelablePromise<Array<MessageDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/chat/conversations/{conversationId}/messages',
            path: {
                'conversationId': conversationId,
            },
        });
    }
    /**
     * @param id
     * @returns void
     * @throws ApiError
     */
    public static deleteConversation(
        id: number,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/chat/conversations/{id}',
            path: {
                'id': id,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
}
