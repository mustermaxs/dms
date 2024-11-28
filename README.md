# Document Management System (DMS)
## Todos (Readme)
- [] TODO add config for `frontend` for running app in Docker/locally
- [] add instructions on how to setup and run application
- [] add Troubleshooting guide for common bugs/errors
- [] add architecture description
### Testing
- [] FileType returns correct file type
- [] 
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
  }
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
```

#todo `frontend/src/config.json`

## MinIO
Connect to MinIO from CLI
´mc alias set <alias> <minio_server_url> <access_key> <secret_key>´

List objects in bucket
´mc ls <alias>/<bucket_name>´
