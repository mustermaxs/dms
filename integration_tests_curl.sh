#!/bin/bash

BASE_URL="$1" 

RED="\033[31m"
RESET="\033[0m"
GREEN="\033[32m"
BLUE="\033[34m"

if [ -z "$BASE_URL" ]; then
  echo "Usage: $0 <api-url>"
  exit 1
fi


print_test_name() {
  local title=$1
  local length=$((${#title} + 4))

  printf "${BLUE}    "
  for ((i=0; i<length; i++)); do
    printf "-"
  done
  printf "\n"

  echo -e "    | ${title} |${RESET}"

  printf "${BLUE}    "
  for ((i=0; i<length; i++)); do
    printf "-"
  done
  printf "${RESET}\n\n"
}

test_endpoint() {
    local method=$1
    local endpoint=$2
    local data=$3
    local expected_status=$4

    local tmp_response=$(mktemp)

    http_status=$(curl -s -X "$method" "$BASE_URL$endpoint" \
        -H "Content-Type: application/json" \
        -d "$data" \
        -o "$tmp_response" \
        -w "%{http_code}")

    response_body=$(cat "$tmp_response" | jq '.')
    rm "$tmp_response"

    if [ "$http_status" -eq "$expected_status" ]; then
        echo -e "${GREEN}Test $method $endpoint: Succeeded (Status: $http_status)${RESET}"
        echo "Response body:"
        echo "$response_body"
    else
        echo -e "${RED}Test $method $endpoint: Failed (Expected: $expected_status, Got: $http_status)${RESET}"
        echo "Response body:"
        echo "$response_body"
    fi
}

# Test POST /api/Documents
print_test_name "Testing POST /api/Documents"
test_endpoint "POST" "/api/Documents" '{
    "title": "SampleDocument.pdf",
    "content": "${PDF_CONTENT}",
    "tags": [
        {"label": "tag1", "color": "red", "value": "tag1"},
        {"label": "tag1", "color": "blue", "value": "tag1"}
    ]
}' 200
echo -e "\n"

# Test GET /api/Documents
print_test_name "Testing GET /api/Documents"
test_endpoint "GET" "/api/Documents" "" 200
echo -e "\n"

# Test PUT /api/Documents (replace YOUR_DOCUMENT_ID with actual ID)
print_test_name "Testing PUT /api/Documents"
test_endpoint "PUT" "/api/Documents/YOUR_DOCUMENT_ID" '{
    "id": "YOUR_DOCUMENT_ID",  
    "title": "Updated Document Title.pdf",
    "tags": [
        {"label": "updatedTag", "color": "green", "value": "updatedTag"}
    ]
}' 200
echo -e "\n"

# Test GET /api/Documents/{id} (replace YOUR_DOCUMENT_ID with actual ID)
print_test_name "Testing GET /api/Documents/YOUR_DOCUMENT_ID"
test_endpoint "GET" "/api/Documents/YOUR_DOCUMENT_ID" "" 200
echo -e "\n"

# Test DELETE /api/Documents/{id} (replace YOUR_DOCUMENT_ID with actual ID)
print_test_name "Testing DELETE /api/Documents/YOUR_DOCUMENT_ID"
test_endpoint "DELETE" "/api/Documents/YOUR_DOCUMENT_ID" "" 200
echo -e "\n"

# Test GET /api/Tags
print_test_name "Testing GET /api/Tags"
test_endpoint "GET" "/api/Tags" "" 200
echo -e "\n"

# # Test POST /api/Tags
# print_test_name "Testing POST /api/Tags"
# test_endpoint "POST" "/api/Tags" '{
#     "label": "New Tag",
#     "color": "yellow",
#     "value": "newTagValue"
# }' 200
# echo -e "\n"


# End of the script
echo "API testing completed."
