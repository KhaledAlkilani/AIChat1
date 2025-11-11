*** Settings ***
Library    SeleniumLibrary

*** Variables ***
${LOC_USERNAME}    id=username_field
${LOC_PASSWORD}    id=password_field
${LOC_SUBMIT}      id=submit_button

*** Keywords ***
Fill Credentials
    [Arguments]    ${username}        ${password}
    Input Text     ${LOC_USERNAME}    ${username}
    Input Text     ${LOC_PASSWORD}    ${password}

Submit Login
    Click Button   ${LOC_SUBMIT}

Login As
    [Arguments]         ${username}    ${password}
    Fill Credentials    ${username}    ${password}
    Submit Login
