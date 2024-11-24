import sys

if sys.platform.startswith('linux'):
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
from tag_tests import TagsTests
from document_tests import DocumentTests
from utils import *


if __name__ == "__main__":
    print("Running tests...")
    unittest.main()
    delete_all_documents()
