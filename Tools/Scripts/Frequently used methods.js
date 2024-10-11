
var _formContext = null;

// Set the form context (should be called in the beginning)
var setFormContext = function (executionContext) {
    _formContext = executionContext.getFormContext();
};

// Get the form context
var getFormContext = function () {
    if (_formContext === null) {
        console.log('Form: FormContext is NOT set!');
        return null;
    }
    return _formContext;
};

// Set the value of a field and optionally fire the onChange event
var setValue = function (field, value, fireOnChange = true) {
    var formContext = getFormContext();
    if (!formContext) return;

    var att = formContext.getAttribute(field);
    if (att === null) {
        console.log(`SetValue: "${field}" is not available`);
    } else {
        att.setValue(value);
        att.setSubmitMode("always");
        if (fireOnChange) {
            att.fireOnChange();
        }
    }
};

// Set the value of a lookup field
var setLookupValue = function (field, id, name, logicalname, fireOnChange = true) {
    if (id) {
        id = `{${id.toUpperCase()}}`;
        var value = [{
            id: id,
            name: name,
            entityType: logicalname
        }];
        setValue(field, value, fireOnChange);
    } else {
        setValue(field, null, fireOnChange);
    }
};

// Get the value of a field
var getValue = function (field) {
    var formContext = getFormContext();
    if (!formContext) return;

    var att = formContext.getAttribute(field);
    if (att === null) {
        console.log(`GetValue: "${field}" is not available`);
        return null;
    } else {
        return att.getValue();
    }
};

// Enable or disable a field
var setDisabled = function (field, disabled) {
    var formContext = getFormContext();
    if (!formContext) return;

    var ctrl = formContext.getControl(field);
    if (ctrl === null) {
        console.log(`SetDisabled: "${field}" is not available`);
    } else {
        ctrl.setDisabled(disabled);
    }
};

// Add an onChange event handler to a field
var addOnChange = function (field, func) {
    var formContext = getFormContext();
    if (!formContext) return;

    var att = formContext.getAttribute(field);
    if (att !== null) {
        att.addOnChange(func);
    }
};

// Add an event to grid refresh
var addEventToGridRefresh = function (gridName, functionToCall) {
    var grid = document.getElementById(gridName);
    if (grid === null) {
        setTimeout(function () {
            addEventToGridRefresh(gridName, functionToCall);
        }, 1000);
    } else {
        grid.control.add_onRefresh(functionToCall);
    }
};

// Set the visibility of a field
var setFieldVisible = function (field, visible) {
    var formContext = getFormContext();
    if (!formContext) return;

    var ctrl = formContext.getControl(field);
    if (ctrl === null) {
        console.log(`SetFieldVisible: "${field}" is not available`);
    } else {
        ctrl.setVisible(visible);
    }
};

// Set the visibility of a section
var setSectionVisible = function (tabname, sectionname, visible) {
    var formContext = getFormContext();
    if (!formContext) return;

    var tab = formContext.ui.tabs.get(tabname);
    if (tab) {
        var section = tab.sections.get(sectionname);
        if (section) {
            section.setVisible(visible);
        } else {
            console.log(`SetSectionVisible: SECTION "${sectionname}" is not available`);
        }
    } else {
        console.log(`SetSectionVisible: TAB "${tabname}" is not available`);
    }
};

// Set the visibility of a tab
var setTabVisible = function (tabname, visible) {
    var formContext = getFormContext();
    if (!formContext) return;

    var tab = formContext.ui.tabs.get(tabname);
    if (tab) {
        tab.setVisible(visible);
    } else {
        console.log(`SetTabVisible: TAB "${tabname}" is not available`);
    }
};

// Set a field as required or not
var setRequired = function (field, required) {
    var formContext = getFormContext();
    if (!formContext) return;

    var att = formContext.getAttribute(field);
    if (att !== null) {
        att.setRequiredLevel(required ? "required" : "none");
    }
};

// Show the document tab
var showDocumentTab = function (mainTabName) {
    var formContext = getFormContext();
    if (!formContext) return;

    var navItem = formContext.ui.navigation.items.get("navSPDocuments");
    if (navItem) {
        navItem.setFocus();
        var mainTab = formContext.ui.tabs.get(mainTabName);
        if (mainTab) {
            mainTab.setFocus();
        } else {
            console.log(`ShowDocumentTab: TAB "${mainTabName}" is not available`);
        }
    } else {
        console.log('ShowDocumentTab: TAB "navSPDocuments" is not available on the form');
    }
};

// Export functions
return {
    SetFormContext: setFormContext,
    GetFormContext: getFormContext,
    SetValue: setValue,
    SetLookupValue: setLookupValue,
    GetValue: getValue,
    SetDisabled: setDisabled,
    AddOnChange: addOnChange,
    AddEventToGridRefresh: addEventToGridRefresh,
    SetFieldVisible: setFieldVisible,
    SetSectionVisible: setSectionVisible,
    SetTabVisible: setTabVisible,
    SetRequired: setRequired,
    ShowDocumentTab: showDocumentTab,
};
