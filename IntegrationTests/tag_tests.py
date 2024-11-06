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
    def get_all_tags(self):
        # GIVEN
        for i in range(3):
            doc = create_rand_document()
            upload_document(doc.to_dict())

        # WHEN
        response = requests.get(url("Tags"))
        resObj = response.json()

        # THEN
        self.assertEqual(response.status_code, 200, f"Expected status code 200, got {response.status_code}")
        content_type = response.headers.get('Content-Type', '')
        self.assertTrue("application/json" in content_type, f"Expected JSON response, got {content_type}")
        self.assertGreater(len(resObj["data"]), 0, f"Expected more than 0 tags, got {len(resObj['data'])}")