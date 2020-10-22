docker tag serverlessfuncs:v1 gbulicanu/serverlessfuncs:v1

docker push gbulicanu/serverlessfuncs:v1

# ...
# dotnet build -c Release

docker build -t gbulicanu/serverlessfuncs:v2 .
docker push gbulicanu/serverlessfuncs:v2