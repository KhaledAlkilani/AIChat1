*** Settings ***
Documentation     Create a new chat and sending a message and receiving AI reply
Library           SeleniumLibrary
Resource          ../keywords/session.robot
Resource          ../pages/login.robot
Resource          ../pages/chat.robot
Test Teardown     Close Application

*** Variables ***
${MESSAGE_TEXT}   Hello from Robot

*** Test Cases ***
New Chat
    Open Application
    Login As    ${USERNAME}    ${PASSWORD}
    Create New Chat
    Send Message                       ${MESSAGE_TEXT}
    User message should be visible     ${MESSAGE_TEXT}