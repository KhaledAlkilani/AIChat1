*** Settings ***
Library    SeleniumLibrary

*** Variables ***
${LOC_NEW_CHAT}                id=new_chat_button
${LOC_MESSAGE_INPUT}           id=message_input
${LOC_SEND_MESSAGE}            id=send_message_button
${LOC_USER_MESSAGE}            id=user-message
${LOC_AI_MESSAGE}              id=ai-message
${LOC_ACTIVE_CHAT_TITLE}       id=active_chat_title

${LOC_RENAME_BUTTON}           id=rename_chat_button
${LOC_RENAME_INPUT}            id=rename_chat_input
${LOC_RENAME_SAVE}             id=rename_chat_save

${LOC_DELETE_BUTTON}           id=delete_chat_button
${LOC_CONFIRM_DELETE}          id=confirm_delete_button

${LOC_USER_MENU}               id=user_menu_button
${LOC_LOGOUT_BUTTON}           id=logout_button

${LOC_CHAT_LIST}               id=chat_list
${LOC_EMPTY_ERROR}             id=empty_message_error

*** Keywords ***

Create New Chat
    Wait Until Element Is Visible    ${LOC_NEW_CHAT}          10s
    Click Button                ${LOC_NEW_CHAT}
    # Element Should Be Visible   ${LOC_ACTIVE_CHAT_TITLE}      10s

Send Message
    [Arguments]    ${text}
    Wait Until Element Is Visible    ${LOC_MESSAGE_INPUT}    5s
    Input Text     ${LOC_MESSAGE_INPUT}     ${text}
    Wait Until Element Is Enabled    ${LOC_SEND_MESSAGE}     15s
    Click Button   ${LOC_SEND_MESSAGE}

Send Empty Message
    [Arguments]    ${text}
    Wait Until Element Is Visible    ${LOC_MESSAGE_INPUT}    5s
    Input Text     ${LOC_MESSAGE_INPUT}     ${text}
    Element Should Be Visible    ${LOC_SEND_MESSAGE}

User message should be visible
    [Arguments]    ${expected}
    Wait Until Page Contains         ${expected}    20s

AI Reply Should Be Visible
    Wait Until Element Is Visible    ${LOC_AI_MESSAGE}      20s

Rename Chat
    [Arguments]    ${new_name}
    Click Button    ${LOC_RENAME_BUTTON}
    Input Text      ${LOC_RENAME_INPUT}    ${new_name}
    Click Button    ${LOC_RENAME_SAVE}

Chat Title Should Be
    [Arguments]    ${expected}
    Element Text Should Be    ${LOC_ACTIVE_CHAT_TITLE}    ${expected}

Delete Active Chat
    Wait Until Element Is Visible    ${LOC_DELETE_BUTTON}    10s
    Click Button    ${LOC_DELETE_BUTTON}
    Handle Alert    action=ACCEPT

Click Logout
    Wait Until Element Is Visible    ${LOC_LOGOUT_BUTTON}    10s
    Click Element                    ${LOC_LOGOUT_BUTTON}

Empty Message Error Should Appear
    Element Should Be Visible    ${LOC_EMPTY_ERROR}
