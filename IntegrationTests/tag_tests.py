#!/bin/env python3

import random
import unittest
import requests
import sys
import json
import httpx
import urllib.request
import base64
from utils import *

class TagsTests(unittest.TestCase):
    # def test_all_tags_uploaded_with_documents_are_persisted(self):
    #     # GIVEN
    #     for i in range(3):
    #         doc = create_rand_document()
    #         upload_document(doc.to_dict(), True)

    #     # WHEN
    #     response = requests.get(url("Tags"))
    #     resObj = response.json()
    #     print(resObj)

    #     # THEN
    #     self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
    #     content_type = response.headers.get('Content-Type', '')
    #     self.assertTrue("application/json" in content_type, f"Expected JSON response, got {content_type}")
    #     self.assertGreater(len(resObj["data"]), 0, f"Expected more than 0 tags, got {len(resObj['data'])}")

    
    def test_uploaded_document_has_tags(self):
        # GIVEN
        documentToUpload = create_rand_document().to_dict()
        tagLabelsExpected = [label for tag in documentToUpload["tags"] for label in [tag["label"]]]
        document = upload_document(documentToUpload)
        document_id = document["id"]

        # WHEN
        response = try_get_document_if_processed(document_id)
        resObj = response.json()
        tagLabelsActual = [tag["label"] for tag in resObj["content"]["tags"]]

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        for expectedLabel in tagLabelsExpected:
            self.assertTrue(expectedLabel in tagLabelsActual, f"Expected tag {expectedLabel}, got {resObj['content']['tags']}")