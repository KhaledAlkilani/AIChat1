*** Settings ***
Documentation     Deleting a chat
Library           SeleniumLibrary
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Resource          ../pages/login.robot
Resource          ../pages/chat.robot
Test Teardown     Close Application

*** Variables ***
${MESSAGE_TEXT}   Hello from Robot

*** Test Cases ***
Delete Chat
    Open Application
    Login As    ${USERNAME}    ${PASSWORD}
    Create New Chat
    Send Message    ${MESSAGE_TEXT}
    Delete Active Chat
    Click Logout
