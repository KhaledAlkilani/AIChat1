*** Settings ***
Documentation     Browsing existing chat history
Library           SeleniumLibrary
Resource          ../resources/variables.robot
Resource          ../keywords/session.robot
Resource          ../pages/login.robot
Test Teardown     Close Application

*** Variables ***
${LOC_HISTORY_BUTTON}      id=browse_history_button
${LOC_HISTORY_LIST}        id=history_list

*** Test Cases ***
Browse History
    Open Application
    Login As    ${USERNAME}    ${PASSWORD}
    Click Button               ${LOC_HISTORY_BUTTON}
    Element Should Be Visible  ${LOC_HISTORY_LIST}
    # Optional: verify at least one item exists
    ${count}=    Get Element Count    xpath=//ul[@id='history_list']/li
    Should Be True    ${count} > 0
