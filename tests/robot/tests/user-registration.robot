*** Settings ***
Documentation     New user registration
Library           SeleniumLibrary
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Resource          ../pages/register.robot
Test Teardown     Close Application

*** Test Cases ***
Open Application
    Open Application
    Navigate To Registration
    Register As                ${REG_USERNAME}    ${REG_PASSWORD}
