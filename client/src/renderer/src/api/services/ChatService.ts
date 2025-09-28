/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
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
     * @returns MessageDto Created
     * @throws ApiError
     */
    public static startNewConversation(
        requestBody?: StartConversationRequest,
    ): CancelablePromise<MessageDto> {
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
}
