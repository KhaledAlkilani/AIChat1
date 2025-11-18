*** Settings ***
Documentation     Renaming an existing conversation
Library           SeleniumLibrary
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Resource          ../pages/login.robot
Test Teardown     Close Application

*** Variables ***
${LOC_NEW_CHAT_BUTTON}      id=new_chat_button
${LOC_RENAME_BUTTON}        id=rename_chat_button
${LOC_RENAME_INPUT}         id=rename_chat_input
${LOC_RENAME_SAVE}          id=rename_chat_save
${LOC_ACTIVE_CHAT_TITLE}    id=active_chat_title
${NEW_CHAT_NAME}            Renamed conversation

*** Test Cases ***
Rename Conversation
    Open Application
    Login As    ${USERNAME}    ${PASSWORD}
    Click Button               ${LOC_NEW_CHAT_BUTTON}
    Click Button               ${LOC_RENAME_BUTTON}
    Input Text                 ${LOC_RENAME_INPUT}    ${NEW_CHAT_NAME}
    Click Button               ${LOC_RENAME_SAVE}
    Element Text Should Be     ${LOC_ACTIVE_CHAT_TITLE}    ${NEW_CHAT_NAME}
