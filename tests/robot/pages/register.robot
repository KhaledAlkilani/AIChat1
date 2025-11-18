*** Settings ***
Library    SeleniumLibrary

*** Variables ***
${LOC_GO_TO_REGISTER}        id=register-click
${LOC_REG_USERNAME}          id=register_username_field
${LOC_REG_PASSWORD}          id=register_password_field
${LOC_REG_SUBMIT}            id=register_submit_button
${LOC_REG_ERROR}             id=register_error_message

*** Keywords ***
Navigate To Registration
    Click Element    ${LOC_GO_TO_REGISTER}

Fill Registration Form
    [Arguments]    ${username}            ${password}
    Input Text     ${LOC_REG_USERNAME}    ${username}
    Input Text     ${LOC_REG_PASSWORD}    ${password}

Submit Registration
    Click Button    ${LOC_REG_SUBMIT}

Register As
    [Arguments]               ${username}        ${password}
    Fill Registration Form    ${username}        ${password}
    Submit Registration

Registration Should Succeed
    Location Should Contain    http://localhost:5173/chat