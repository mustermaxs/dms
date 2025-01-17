function getEmptyGuid() {
    return '00000000-0000-0000-0000-000000000000';
}

function generateTagDto(id, label, value) {
    return {
        id,
        label,
        value,
        color: "red"
    };
}

function generateCreateDocumentDto(title, base64Content, tags) {
    return {
        title: title,
        content: base64Content,
        tags: tags,
        fileType: "pdf"
    };
}

function createRequestObj(method, endpoint, body = undefined) {
    const requestObj = {
        method: method,
        url: `${pm.globals.get("baseUrl")}${endpoint}`,
        header: {
            'Content-Type': 'application/json'
        }
    };

    if (body) {
        requestObj.body = {
            mode: 'raw',
            raw: JSON.stringify(body)
        };
    }

    return requestObj;
}

function sendRequest(requestObj, callback = (err, res) => {}) {
    pm.sendRequest(requestObj, function (err, response) {
        if (err) {
            console.error('Request failed:', err);
            callback(err, response);
        } else {
            try {
                callback(null, response);
            } catch (parseError) {
                console.error('Error parsing response:', parseError);
                callback(parseError, null);
            }
        }
    });
}

function createNewDocument(createDocumentDto, callback = (err, res) => {}) {
    const requestObj = createRequestObj('POST', '/api/Documents', createDocumentDto);
    sendRequest(requestObj, callback);
}

function deleteDocument(documentId, callback = (err, res) => {}) {
    const requestObj = createRequestObj('DELETE', `/api/Documents/${documentId}`);
    sendRequest(requestObj, callback);
}

function getDocument(documentId, callback = (err, res) => {}) {
    const requestObj = createRequestObj("GET", `/api/Documents/${documentId}`);
    sendRequest(requestObj, callback);
}

function getContent(documentId, callback = (err, res) => {}) {
    const requestObj = createRequestObj("GET", `/api/Documents/${documentId}/content`);
    sendRequest(requestObj, callback);
}

function updateDocument(documentId, title, tags, callback = (err, res) => {}) {
    let updateDocumentDto = {
        title: title,
        tags: tags,
        id: documentId
    };
    const requestObj = createRequestObj('PUT', `/api/Documents/${documentId}`, updateDocumentDto);

    sendRequest(requestObj, callback);
}

function document() {
    return {
        create: createNewDocument,
        delete: deleteDocument,
        get: getDocument,
        getContent: getContent,
        updateDocument: updateDocument
    };
}

function collectionVariables() {
    let _this = {};

    _this.getDocIds = () => {
        let docIdsString = pm.collectionVariables.get("createdDocumentIds") || "[]";
        return JSON.parse(docIdsString);
    };

    _this.addDocId = (documentId) => {
        let documentIds = _this.getDocIds();
        documentIds.push(documentId);
        pm.collectionVariables.set("createdDocumentIds", JSON.stringify(documentIds));
    };

    _this.removeDocId = (documentId) => {
        let documentIds = _this.getDocIds();
        let indexOfDocId = documentIds.indexOf(documentId);

        if (indexOfDocId == -1)
            throw "Document id not found in collection variables";

        documentIds.splice(indexOfDocId, 1);
        pm.collectionVariables.unset("createdDocumentIds");
        pm.collectionVariables.set("createdDocumentIds", JSON.stringify(documentIds));
    };

    return _this;
}

function documentContentTests(cleanUpCallback) {
    const _cleanUpCallback = cleanUpCallback;
    
    return (response) => {
        pm.test("Status code is 200", function () {
            pm.response.to.have.status(200);
        });
    
        pm.test("Document content retrieval successful", function () {
        
            const responseBody = response.json();
            console.log(responseBody)
            
            pm.expect(responseBody.success).to.be.true;
            pm.expect(responseBody).to.have.property('content');
            pm.expect(responseBody.content).to.have.property('title');
            pm.expect(responseBody.content).to.have.property('content');
    
            pm.expect(responseBody.content.title).to.equal("MockPDF");
            pm.expect(responseBody.content.content).to.include("Azure");
            pm.expect(responseBody.content.content).to.include("Pipelines");
        });
        _cleanUpCallback(response.json().content.id);
    }
}


function runTestsOnDocumentProcessed(testRunner, onDocumentProcessed) {
    console.info("[POST-RESPONSE] waiting for document to be processed");
    
    document().get(pm.response.json().content.id, (err, res) => {
        if (res.code == 200 && res.json().content.status == 2)
        {
            let documentId = res.json().content.id;
            onDocumentProcessed(documentId);
        }
        else
        {
            setTimeout(() => {
                runTestsOnDocumentProcessed(testRunner, onDocumentProcessed);
            }, 2500);
        }
    });    
}

const collectionVars = collectionVariables();

module.exports = { generateCreateDocumentDto, generateTagDto, getEmptyGuid, document, collectionVars, documentContentTests, runTestsOnDocumentProcessed };

// ------------------------------------------------------------------------------------------------------------

// GET /api/Documents
// PRE-REQUEST SCRIPT

const {generateCreateDocumentDto, generateTagDto, document, getEmptyGuid, collectionVars } = pm.require('@winter-sunset-971800/document')


let createDocumentDto = generateCreateDocumentDto("MockPDF", pm.globals.get("mockPdf"), [generateTagDto(getEmptyGuid(), "mocktag", "mocktag")]);
let createdDocument = document().create(createDocumentDto, (err, res) => {
    collectionVars.addDocId(res.json().content.id);
    console.log("[PRE-SCRIPT] Created document with id: " + res.json().content.id);
});


// ------------------------------------------------------------------------------------------------------------

// GET /api/Documents
// POST-RESPONSE SCRIPT

const {generateCreateDocumentDto, generateTagDto, document, getEmptyGuid } = pm.require('@winter-sunset-971800/document')


pm.test("All documents are fetched", () => {
    pm.test("Status code is 200", function () {
        pm.response.to.have.status(200);
    });
})

let documentId = pm.collectionVariables.get("createdDocumentId");

document().delete(documentId, (err, res) => {
    if (res.code == 200)
    {
        collectionVars.removeDocId(documentId);
    }
    else
    {
        console.error("[POST-SCRIPT] Failed to delete document by id: " + documentId);
    }
});

// GET /api/Documents/{documentId}/content
// POST-RESPONSE SCRIPT

const {generateCreateDocumentDto, generateTagDto, document, getEmptyGuid } = pm.require('@winter-sunset-971800/document')




function deleteDocument(documentId) {
    document().delete(documentId, (err, res) => {
        if (res.code == 200)
        {
            collectionVars.removeDocId(documentId);
        }
        else
        {
            console.error("[POST-SCRIPT] Failed to delete document by id: " + documentId);
        }
    });
}



let testRunner = documentContentTests((documentId) => {deleteDocument(documentId);});

runTestsOnDocumentProcessed(testRunner, (documentId) => {
    document().getContent(documentId, (err, res) => {
        testRunner(res);
    })
});

// GET /api/Documents/{id}
// PRE-RESPONSE SCRIPT
const {generateCreateDocumentDto, generateTagDto, document, getEmptyGuid } = pm.require('@winter-sunset-971800/document')


let createDocumentDto = generateCreateDocumentDto("MockPDF", pm.globals.get("mockPdf"), [generateTagDto(getEmptyGuid(), "mocktag", "mocktag")]);
let createdDocument = document().create(createDocumentDto, (err, res) => {
    collectionVars.addDocId(res.json().content.id);
    console.log("[PRE-SCRIPT] Created document with id: " + res.json().content.id);
});

// POST-RESPONSE SCRIPT
const {generateCreateDocumentDto, generateTagDto, document, getEmptyGuid, documentContentTests, runTestsOnDocumentProcessed } = pm.require('@winter-sunset-971800/document')


pm.test("Single document fetched", () => {
    pm.test("Status code is 200", function () {
        pm.response.to.have.status(200);
    });
})

function deleteDocument(documentId) {
    document().delete(documentId, (err, res) => {
        if (res.code == 200)
        {
            collectionVars.removeDocId(documentId);
        }
        else
        {
            console.error("[POST-SCRIPT] Failed to delete document by id: " + documentId);
        }
    });
}


let testRunner = documentContentTests((documentId) => {deleteDocument(documentId);});

runTestsOnDocumentProcessed(testRunner, (documentId) => {
    document().getContent(documentId, (err, res) => {
        testRunner(res);
    })
});

// GET /api/Search?query=SearchingForNonExistingDocument
// PRE-REQUEST SCRIPT

const {generateCreateDocumentDto, generateTagDto, document, onDocumentProcessed, runTestsOnDocumentProcessed } = pm.require('@winter-sunset-971800/document')
let createDocumentDto = generateCreateDocumentDto("MockPDF", pm.globals.get("mockPdf"), [generateTagDto(getEmptyGuid(), "mocktag", "mocktag")]);
let createdDocument = document().create(createDocumentDto, (err, res) => {
    collectionVars.addDocId(res.json().content.id);
    console.log("[PRE-SCRIPT] Created document with id: " + res.json().content.id);
});

// POST-REQUEST SCRIPT
const {generateCreateDocumentDto, generateTagDto, document, getEmptyGuid, documentContentTests, runTestsOnDocumentProcessed } = pm.require('@winter-sunset-971800/document')

pm.test("No search results", () => {
    pm.test("Status code is 200", function () {
        let responseBody = pm.response.json();
        pm.response.to.have.status(200);
        pm.expect(responseBody.content.length).equals(0, "Expected 0 search results.");
        
    });
});

