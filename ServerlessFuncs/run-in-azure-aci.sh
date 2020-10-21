az container create \
    -n serverless-funcs-1 \
    -g serverless-funcs-docker \
    --image gbulicanu/serverlessfuncs1 \
    --ip-address public \
    --ports 80 \
    --dns-name-label gbulicanu-serverlessfuncsv1 \
    -e AzureWebJobsStorage=$CON_STR
