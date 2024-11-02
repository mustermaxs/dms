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
from utils import *

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

    def test_delete_all_documents(self):
        # GIVEN
        for i in range(5):
            doc = create_rand_document()
            upload_document(doc.to_dict())

        # WHEN
        response = requests.delete(url("Documents"))
        resObj = response.json()
        documents_in_db = requests.get(url("Documents")).json()

        minio_bucket = minio_client.bucket_exists(MINIO_BUCKET_NAME)
        if not minio_bucket:
            raise Exception(f"Bucket {MINIO_BUCKET_NAME} does not exist")
        
        minio_files = []
        minio_objects = minio_client.list_objects(MINIO_BUCKET_NAME, recursive=True)
        for obj in minio_objects:
            minio_files.append(obj.object_name)

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        self.assertEqual(len(documents_in_db["data"]), 0, f"Expected 0 documents, got {len(documents_in_db['data'])}")
        self.assertEqual(len(minio_files), 0, f"Expected 0 objects in Minio, got {len(minio_files)}")

