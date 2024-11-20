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

upload_documents = []

class DocumentTests(unittest.TestCase):

    @classmethod
    def setUp(cls) -> None:
        delete_all_documents()
        upload_documents = []
    
    def test_get_all_documents(self):
        # GIVEN
        for i in range(3):
            doc = create_rand_document()
            uploaded_doc = upload_document(doc.to_dict())
            document_id = uploaded_doc["id"]
            wait_for_document_to_be_processed(document_id)


        # WHEN            
        response = requests.get(url("Documents"))
        resObj = response.json()

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        content_type = response.headers.get('Content-Type', '')
        self.assertTrue("application/json" in content_type, f"Expected JSON response, got {content_type}")
        self.assertGreater(len(resObj["content"]), 0, f"Expected more than 0 documents, got {len(resObj['content'])}")

    
    def test_get_document_by_id(self):
        # GIVEN
        document = upload_document(create_rand_document().to_dict())
        document_id = document["id"]
        wait_for_document_to_be_processed(document_id)

        # WHEN
        response = requests.get(url(f"Documents/{document_id}"))
        resObj = response.json()

        # THEN
        self.assertEqual(resObj["content"]["id"], document_id, f"Expected document ID {document_id}, got {resObj['content']['id']}")
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        self.assertEqual(resObj["content"]["title"], document["title"], f"Expected title {document['title']}, got {resObj['content']['title']}")
        
    def test_delete_document_by_id(self):
        # GIVEN
        document = upload_document(create_rand_document().to_dict())
        document_id = document["id"]
        wait_for_document_to_be_processed(document_id)


        # WHEN
        response = requests.delete(url(f"Documents/{document_id}"))
        resObj = response.json()

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")

    def test_update_document_by_id(self):
        # GIVEN
        rand_doc = create_rand_document()
        document = upload_document(rand_doc.to_dict())
        document_id = document["id"]
        wait_for_document_to_be_processed(document_id)


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
        self.assertEqual(resObj["content"]["id"], document_id, f"Expected document ID {document_id}, got {resObj['content']['id']}")
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        self.assertEqual(resObj["content"]["title"], updated_document["title"], f"Expected title {updated_document['title']}, got {resObj['content']['title']}")
        self.assertEqual(resObj["content"]["tags"][0]["label"], updated_document["tags"][0]["label"], f"Expected label {updated_document['tags'][0]['label']}, got {resObj['content']['tags'][0]['label']}")
        self.assertEqual(resObj["content"]["tags"][0]["value"], updated_document["tags"][0]["value"], f"Expected value {updated_document['tags'][0]['value']}, got {resObj['content']['tags'][0]['value']}")

    def test_delete_all_documents(self):
        # GIVEN
        for i in range(5):
            rand_doc = create_rand_document()
            document = upload_document(rand_doc.to_dict())
            document_id = document["id"]
            wait_for_document_to_be_processed(document_id)


        # WHEN
        response = requests.delete(url("Documents"))
        resObj = response.json()
        documents_in_db = requests.get(url("Documents")).json()["content"]

        minio_bucket = minio_client.bucket_exists(MINIO_BUCKET_NAME)
        if not minio_bucket:
            raise Exception(f"Bucket {MINIO_BUCKET_NAME} does not exist")
        
        minio_files = []
        minio_objects = minio_client.list_objects(MINIO_BUCKET_NAME, recursive=True)
        for obj in minio_objects:
            minio_files.append(obj.object_name)

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        self.assertEqual(len(documents_in_db), 0, f"Expected 0 documents, got {len(documents_in_db)}")
        self.assertEqual(len(minio_files), 0, f"Expected 0 objects in Minio, got {len(minio_files)}")

    def test_uploaded_document_has_content(self):
        # GIVEN
        document = upload_document(create_rand_document().to_dict())
        document_id = document["id"]
        wait_for_document_to_be_processed(document_id)

        # WHEN
        response = try_get_document_if_processed(document_id)
        resObj = response.json()
        
        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        self.assertTrue((resObj["content"] != "") and (resObj["content"] != None))