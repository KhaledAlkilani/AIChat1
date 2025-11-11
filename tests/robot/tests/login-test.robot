*** Settings ***
Documentation
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Resource          ../pages/login.robot
Test Teardown     Close Application

*** Test Cases ***
User Can Login
    Open Application
    Login As    ${USERNAME}    ${PASSWORD}
