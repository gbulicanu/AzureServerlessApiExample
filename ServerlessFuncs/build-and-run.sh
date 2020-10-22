docker build -t serverlessfuncs:v1 . 

docker run serverlessfuncs:v1

CON_STR=$(az storage account show-connection-string -g gb-pl-msazdev-serverless-funcs-01 -n serverlessfuctionsdocker -o tsv)

docker run -e AzureWebJobsStorage=$CON_STR -p 8080:80 serverlessfuncs:v1

docker run -e WEB_HOST=https://storageaccountgbplm97dc.blob.core.windows.net/website -e AzureWebJobsStorage=$CON_STR -p 8080:80 gbulicanu/serverlessfuncs:v2

