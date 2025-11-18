*** Settings ***
Library    SeleniumLibrary

*** Variables ***
${LOC_HISTORY_BUTTON}       id=browse_history_button
${LOC_HISTORY_LIST}         id=history_list
${LOC_HISTORY_ITEMS}        xpath=//ul[@id="history_list"]/li

*** Keywords ***
Open History
    Click Button    ${LOC_HISTORY_BUTTON}

History Should Be Visible
    Element Should Be Visible    ${LOC_HISTORY_LIST}

History Should Have Items
    ${count}=    Get Element Count    ${LOC_HISTORY_ITEMS}
    Should Be True    ${count} > 0
