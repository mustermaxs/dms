#!/bin/env python3

import random
import unittest
import requests
import sys
import json
import httpx
import urllib.request
import base64



with open("config.json", "r") as file:
    config = json.load(file)

API_BASE_URL = config["base_url"]

if API_BASE_URL[-1] != "/":
    API_BASE_URL += "/"

class Mocks:
    _instance = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(Mocks, cls).__new__(cls)
            cls._instance.load_mocks()
        return cls._instance

    def load_mocks(self):
        try:
            with open("mocks.json", "r") as file:
                self.mocks = json.load(file)
        except FileNotFoundError:
            print("mocks.json not found.")
            sys.exit(1)
        except json.JSONDecodeError:
            print("Error reading JSON from mocks.json.")
            sys.exit(1)

    def get(self, key):
        return json.dumps(self.mocks[key])

class RandomWord:
    _instance = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(RandomWord, cls).__new__(cls)
        return cls._instance
    
    def __init__(self):
        # Create an instance of Mocks to access the random words
        self.mocks = Mocks()
        self.words = json.loads(self.mocks.get("RandomWords"))

    def get_word(self):
        if not self.words:
            return None  # Return None if no words are available
        return self.words[random.randint(0, len(self.words) - 1)]
    

def generate_random_content(max_words):
    num_words = random.randint(1, max_words)
    words = [RandomWord().get_word() for _ in range(num_words)]
    return " ".join(words)

def string_to_base64(string):
    return base64.b64encode(string.encode("utf-8")).decode("utf-8")

class UploadDocumentDto:
    def __init__(self, title, content, tags):
        self.title = title
        self.content = content
        self.tags = tags
    
    def to_json(self):
        return json.dumps(self, default=lambda o: o.__dict__)
    
    def to_dict(self):
        return {
            "title": self.title,
            "content": self.content,
            "tags": [tag.to_dict() for tag in self.tags]
        }
    
    def from_json(self, json):
        return json.loads(json)
    
def create_rand_document():
    title = RandomWord().get_word() + ".pdf"
    content = string_to_base64(generate_random_content(100))
    tags = [create_rand_tag()]
    return UploadDocumentDto(title, content, tags)

def url(path):
    return f"{API_BASE_URL}{path}"

class Tag:
    def __init__(self, label, color, value):
        self.label = label
        self.color = color
        self.value = value

    def to_dict(self):
        return {
            "label": self.label,
            "color": self.color,
            "value": self.value
        }
    
    def to_json(self):
        return json.dumps(self, default=lambda o: o.__dict__)
    
    def from_json(self, json):
        return json.loads(json)
    
def create_rand_tag():
    value = "cauliflower"
    # value = RandomWord().get_word()
    label = "cauliflower"
    color = "blue"
    return Tag(label, color, value)

def upload_document(document=None):
    if document is None:
        document = Mocks().get("UploadDocumentDto")
    document = json.dumps(document)
    response = requests.post(url("Documents"), data=document, headers={"Content-Type": "application/json"})
    return response.json()

def delete_all_documents():
    response = requests.delete(url("Documents"))
    return response

class DocumentTests(unittest.TestCase):
    @classmethod
    def setUp(cls) -> None:
        delete_all_documents()
    
    def test_get_all_documents(self):
        # GIVEN
        for i in range(3):
            doc = create_rand_document()
            upload_document(doc.to_dict())

        # WHEN            
        response = requests.get(url("Documents"))
        resObj = response.json()

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        content_type = response.headers.get('Content-Type', '')
        self.assertTrue("application/json" in content_type, f"Expected JSON response, got {content_type}")
        self.assertGreater(len(resObj["data"]), 0, f"Expected more than 0 documents, got {len(resObj['data'])}")

    
    def test_get_document_by_id(self):
        # GIVEN
        document = upload_document(create_rand_document().to_dict())
        document_id = document["data"]["id"]

        # WHEN
        response = requests.get(url(f"Documents/{document_id}"))
        resObj = response.json()

        # THEN
        self.assertEqual(resObj["data"]["id"], document_id, f"Expected document ID {document_id}, got {resObj['data']['id']}")
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        self.assertEqual(resObj["data"]["title"], document["data"]["title"], f"Expected title {document['data']['title']}, got {resObj['data']['title']}")
        
    def test_delete_document_by_id(self):
        # GIVEN
        document = upload_document(create_rand_document().to_dict())
        document_id = document["data"]["id"]

        # WHEN
        response = requests.delete(url(f"Documents/{document_id}"))
        resObj = response.json()

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")

    def test_update_document_by_id(self):
        # GIVEN
        rand_doc = create_rand_document()
        document = upload_document(rand_doc.to_dict())
        document_id = document["data"]["id"]

        # WHEN
        updated_document = rand_doc.to_dict()
        del updated_document["content"]
        updated_document["id"] = document_id
        updated_document["title"] = "updated_" + updated_document["title"]
        updated_document["tags"][0]["label"] = "updated_" + updated_document["tags"][0]["label"]
        updated_document["tags"][0]["value"] = "updated_" + updated_document["tags"][0]["value"]
        response = requests.put(url(f"Documents/{document_id}"), data=json.dumps(updated_document), headers={"Content-Type": "application/json"})
        resObj = response.json()

        # THEN
        self.assertEqual(resObj["data"]["id"], document_id, f"Expected document ID {document_id}, got {resObj['data']['id']}")
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        self.assertEqual(resObj["data"]["title"], updated_document["title"], f"Expected title {updated_document['title']}, got {resObj['data']['title']}")
        self.assertEqual(resObj["data"]["tags"][0]["label"], updated_document["tags"][0]["label"], f"Expected label {updated_document['tags'][0]['label']}, got {resObj['data']['tags'][0]['label']}")
        self.assertEqual(resObj["data"]["tags"][0]["value"], updated_document["tags"][0]["value"], f"Expected value {updated_document['tags'][0]['value']}, got {resObj['data']['tags'][0]['value']}")

if __name__ == "__main__":
    unittest.main()
