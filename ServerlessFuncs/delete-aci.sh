az container logs \
    -n serverless-funcs-1 \
    -g serverless-funcs-docker

az container delete \
    -n serverless-funcs-1 \
    -g serverless-funcs-docker