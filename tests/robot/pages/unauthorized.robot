*** Settings ***
Library    SeleniumLibrary

*** Variables ***
${LOC_UNAUTHORIZED_MESSAGE}     id=unauthorized_message

*** Keywords ***
Unauthorized Message Should Be Visible
    Element Should Be Visible    ${LOC_UNAUTHORIZED_MESSAGE}
