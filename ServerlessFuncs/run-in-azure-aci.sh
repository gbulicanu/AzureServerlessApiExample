az container create \
    -n serverless-funcs-1 \
    -g serverless-funcs-docker \
    --image gbulicanu/serverlessfuncs:v1 \
    --ip-address public \
    --ports 80 443 \
    --dns-name-label gbulicanu-serverlessfuncsv1 \
    -e AzureWebJobsStorage=$CON_STR


az container create \
    -n serverless-funcs-1 \
    -g serverless-funcs-docker \
    --image gbulicanu/serverlessfuncs:v2 \
    --ip-address public \
    --ports 80 443 \
    --dns-name-label gbulicanu-serverlessfuncsv1 \
    -e AzureWebJobsStorage=$CON_STR WEB_HOST=https://storageaccountgbplm97dc.blob.core.windows.net/website


az container logs \
    -n serverless-funcs-1 \
    -g serverless-funcs-docker