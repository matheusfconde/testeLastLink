docker build -t lastlink-api:v1 .

docker run -d -p 8080:80 --name lastlink-container lastlink-api:v1

docker logs lastlink-container