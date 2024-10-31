#!/bin/bash

API_URL="$1" # Change this to the actual URL of the API

if [ -z "$API_URL" ]; then
  echo "Usage: $0 <api-url>"
  exit 1
fi

# Base64 encoded PDF content (sample short PDF file)
PDF_CONTENT=$(cat ./integration_test_mock.pdf | base64)

# Helper function to pretty-print JSON responses
pretty_print() {
  jq . <<< "$1"
}

# 1. Create a Tag
echo "Creating a Tag..."
create_tag_response=$(curl -s -X POST "$API_URL/api/Tags" -H "Content-Type: application/json" -d '{
  "label": "SampleTag",
  "color": "#FF5733",
  "value": "sample"
}')
echo "Tag Created Response:"
pretty_print "$create_tag_response"

tag_id=$(jq -r '.id' <<< "$create_tag_response")

# 2. Upload a Document
echo "Uploading a Document..."
upload_document_response=$(curl -s -X POST "$API_URL/api/Documents" -H "Content-Type: application/json" -d "{
  \"title\": \"Test Document\",
  \"content\": \"$PDF_CONTENT\",
  \"tags\": [{\"id\": \"$tag_id\", \"label\": \"SampleTag\", \"color\": \"#FF5733\", \"value\": \"sample\"}]
}")
echo "Document Upload Response:"
pretty_print "$upload_document_response"

document_id=$(jq -r '.id' <<< "$upload_document_response")

# 3. Retrieve the Document by ID
echo "Retrieving Document by ID..."
get_document_response=$(curl -s -X GET "$API_URL/api/Documents/$document_id")
echo "Get Document Response:"
pretty_print "$get_document_response"

# 4. Update the Document
echo "Updating the Document..."
update_document_response=$(curl -s -X PUT "$API_URL/api/Documents" -H "Content-Type: application/json" -d "{
  \"id\": \"$document_id\",
  \"title\": \"Updated Document\",
  \"tags\": [{\"id\": \"$tag_id\", \"label\": \"UpdatedTag\", \"color\": \"#FF5733\", \"value\": \"sample\"}]
}")
echo "Update Document Response:"
pretty_print "$update_document_response"

# 5. List All Documents
echo "Listing All Documents..."
list_documents_response=$(curl -s -X GET "$API_URL/api/Documents")
echo "List Documents Response:"
pretty_print "$list_documents_response"

# 6. Delete the Document
echo "Deleting the Document..."
delete_document_response=$(curl -s -X DELETE "$API_URL/api/Documents/$document_id")
echo "Delete Document Response:"
pretty_print "$delete_document_response"

# 7. Retrieve All Tags
echo "Retrieving All Tags..."
get_tags_response=$(curl -s -X GET "$API_URL/api/Tags")
echo "Get Tags Response:"
pretty_print "$get_tags_response"

# 8. Search Tags by Prefix
echo "Searching Tags by Prefix..."
search_tags_response=$(curl -s -X GET "$API_URL/search?tagPrefix=Sample")
echo "Search Tags Response:"
pretty_print "$search_tags_response"

echo "Integration tests completed."
