# Document Management System (DMS)
## Todos (Readme)
- [ ] TODO add config for `frontend` for running app in Docker/locally
- [ ] add instructions on how to setup and run application
- [ ] add Troubleshooting guide for common bugs/errors
- [ ] add architecture description
### Testing
- [ ] FileType returns correct file type
- [ ] 
## Run in Docker
**Change the following parameters in**

`REST/DMS.Api/appsettings.json`:
```
  "MinIO": {
    "Endpoint": "minio:9000",
    "Port": 9000,
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketName": "dmsbucket",
    "FilePath": "/home/dms/uploads/"
  },
  "RabbitMq": {
    "HostName": "rabbitmq",
    "UserName": "dmsadmin",
    "Password":"dmsadmin",
    "Endpoint": "rabbitmq",
    "Port": 5672
  },
  "ElasticSearch": {
    "Uri": "http://elastic_search:9200"
  },
```

`OcrWorker/OcrWorker/settings.json`
```
{
  "RabbitMq": {
    "HostName": "rabbitmq",
    "UserName": "dmsadmin",
    "Password":"dmsadmin",
    "Endpoint": "rabbitmq",
    "Port": 5672,
    "Queue": "ocr-process"
  },
  "MinIO": {
    "Endpoint": "minio:9000",
    "Port": 9000,
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketName": "dmsbucket",
    "FilePath": "/home/dms/uploads/"
  },
  "ElasticSearch": {
    "Uri": "http://elastic_search:9200"
  },
}
```

#todo `frontend/src/config.json`


## Run locally
**Change the following parameters in**

`REST/DMS.Api/appsettings.json`:
```
  "MinIO": {
    "Endpoint": "localhost:9000",
    "Port": 9000,
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketName": "dmsbucket",
    "FilePath": "/home/dms/uploads/"
  },
  "RabbitMq": {
    "HostName": "localhost:5672",
    "UserName": "dmsadmin",
    "Password":"dmsadmin",
    "Endpoint": "localhost",
    "Port": 5672
  },
  "ElasticSearch": {
    "Uri": "http://localhost:9200" // ? always runs in docker
  },
```

`OcrWorker/OcrWorker/settings.json`:

```
  "MinIO": {
    "Endpoint": "localhost:9000",
    "Port": 9000,
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketName": "dmsbucket",
    "FilePath": "/home/dms/uploads/"
  },
  "RabbitMq": {
    "HostName": "localhost:5672",
    "UserName": "dmsadmin",
    "Password":"dmsadmin",
    "Endpoint": "localhost",
    "Port": 5672
  },
  "ElasticSearch": {
    "Uri": "http://localhost:9200" // ? always runs in docker
  },
```

#todo `frontend/src/config.json`

## MinIO
Connect to MinIO from CLI
`mc alias set <alias> <minio_server_url> <access_key> <secret_key>`

List objects in bucket
`mc ls <alias>/<bucket_name>`

## ElasticSearch
List all indicies via API call
`curl '<elastic-search-url>/_cat/indices?v'`