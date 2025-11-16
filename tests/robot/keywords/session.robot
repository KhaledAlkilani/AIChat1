*** Settings ***
Library    SeleniumLibrary
Resource   ../resources/variables.robot

*** Keywords ***
Open Application
    Open Browser            ${BASE_URL}    headlessfirefox
    Set Selenium Timeout    ${SELENIUM_TIMEOUT}
    # Maximize Browser Window

Close Application
    Close All Browsers