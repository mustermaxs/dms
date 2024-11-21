#!/bin/env python3

import random
import unittest
import requests
import sys
import json
import httpx
import urllib.request
import base64
from minio import Minio
import time

MINIO_ACCESS_KEY = ""
MINIO_SECRET_KEY = ""
MINIO_ENDPOINT = ""
MINIO_BUCKET_NAME = ""
DOC_STATUS_CHECK_MAX_ATTEMPTS = 1

# write logger class. log to specific file
class Logger:
    def __init__(self, filename):
        self.filename = filename

    def write(self, message):
        with open(self.filename, "a") as f:
            f.write(message)

    def flush(self):
        pass

    def close(self):
        pass

logger = Logger("log.txt")


with open("../REST/DMS.Api/appsettings.json", "r") as f:
    config = json.load(f)

    MINIO_ACCESS_KEY = config["MinIO"]["AccessKey"]
    MINIO_SECRET_KEY = config["MinIO"]["SecretKey"]
    MINIO_ENDPOINT = config["MinIO"]["Endpoint"]
    MINIO_BUCKET_NAME = config["MinIO"]["BucketName"]

with open("config.json", "r") as file:
    DOC_STATUS_CHECK_MAX_ATTEMPTS = json.load(file)["document_processing_status_check_max_attempts"]

minio_client = Minio(endpoint="localhost:9000",
    access_key=MINIO_ACCESS_KEY,
    secret_key=MINIO_SECRET_KEY,
    secure=False,
)


def get_base_url():
    with open("config.json", "r") as file:
        config = json.load(file)

    API_BASE_URL = config["base_url"]

    if API_BASE_URL[-1] != "/":
        API_BASE_URL += "/"
    
    return API_BASE_URL

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
    content = pdf_to_base_64("mock_pdf.pdf")
    tags = [create_rand_tag()]
    return UploadDocumentDto(title, content, tags)

def url(path):
    return f"{get_base_url()}{path}"

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
    # value = "cauliflower"
    value = RandomWord().get_word()
    label = value
    # label = "cauliflower"
    color = "blue"
    return Tag(label, color, value)

def upload_document(document=None, waitUntilProcessed=True):
    if document is None:
        document = Mocks().get("UploadDocumentDto")
    document = json.dumps(document)
    response = requests.post(url("Documents"), data=document, headers={"Content-Type": "application/json"})
    
    if waitUntilProcessed:
        wait_for_document_to_be_processed(response.json()["content"]["id"])
    return response.json()["content"]

def delete_all_documents():
    response = requests.delete(url("Documents"))
    return response

def pdf_to_base_64(file_path):
    with open(file_path, "rb") as f:
        return base64.b64encode(f.read()).decode("utf-8")
    
def wait_for_document_to_be_processed(document_id):
    response = try_get_document_if_processed(document_id)
    return response

def wait_for_documents_to_be_processed(document_ids):
    for document_id in document_ids:
        wait_for_document_to_be_processed(document_id)
    
def try_get_document_if_processed(document_id):
    for _ in range(DOC_STATUS_CHECK_MAX_ATTEMPTS):
        response = requests.get(url(f"Documents/{document_id}"))
        res_json = response.json()
        
        if res_json["content"]["status"] == 2:
            return response
        time.sleep(2)
    raise Exception(f"Document {document_id} not processed after {DOC_STATUS_CHECK_MAX_ATTEMPTS} attempts")
