*** Settings ***
Documentation     Unauthorized user cannot access protected chat view
Library           SeleniumLibrary
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Test Teardown     Close Application

*** Variables ***
${CHAT_URL}                ${BASE_URL}chat
${LOC_UNAUTH_MESSAGE}      id=unauthorized_message

*** Test Cases ***
Unauthorized Access
    Open Browser            ${CHAT_URL}    ${BROWSER}
    Set Selenium Timeout    ${SELENIUM_TIMEOUT}
    Maximize Browser Window
    Element Should Be Visible    ${LOC_UNAUTH_MESSAGE}
