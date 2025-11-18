*** Settings ***
Documentation     Logging out from the application
Library           SeleniumLibrary
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Resource          ../pages/login.robot
Resource          ../pages/chat.robot
Test Teardown     Close Application

*** Test Cases ***
Logout
    Open Application
    Login As    ${USERNAME}    ${PASSWORD}
    Click Logout
