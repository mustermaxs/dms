#!/bin/bash

# MOCKS
## MOCK DOCUMENTDTO
DOCUMENTDTO=$(cat <<EOF
{
  "name": "Example Document",
  "type": "application/pdf",
  "content": "$PDF_CONTENT"
}
EOF
)

## MOCK TAGDTO
TAGDTO=$(cat <<EOF
{
  "label": "tag1",
  "color": "red",
  "value": "tag1"
}
EOF
)

## MOCK UPLOADDOCUMENTDTO
UPLOADDOCUMENTDTO=$(cat <<EOF
{
  "title": "SampleDocument.pdf",
  "content": "$PDF_CONTENT",
  "tags": [
    {
      "label": "tag1",
      "color": "red",
      "value": "tag1"
    },
    {
      "label": "tag2",
      "color": "blue",
      "value": "tag2"
    }
  ]
}
EOF
)

## MOCK UPDATEDOCUMENTDTO
UPDATEDOCUMENTDTO=$(cat <<EOF
{
  "title": "Updated Document Title.pdf",
  "tags": [
    {
      "label": "updatedTag",
      "color": "green",
      "value": "updatedTag"
    }
  ]
}
EOF
)

PDF_CONTENT=$(cat ./integration_test_mock.pdf | base64)