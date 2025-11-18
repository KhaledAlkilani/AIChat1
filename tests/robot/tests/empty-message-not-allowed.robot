*** Settings ***
Documentation     Validation when sending an empty message
Library           SeleniumLibrary
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Resource          ../pages/login.robot
Resource          ../pages/chat.robot
Resource          ../pages/history.robot
Test Teardown     Close Application

*** Variables ***
${text}

*** Test Cases ***
Empty Message Not Allowed
    Open Application
    Login As    ${USERNAME}    ${PASSWORD}
    Create New Chat
    Send Empty Message    ${text}