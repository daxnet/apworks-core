# Initialize the Integration Test Environment
#
# Setting up Docker container for MongoDB
/bin/bash -c 'if [[ "$(docker ps -aqf "name=apworks_mongodb" 2> /dev/null)" != "" ]]; then \
    docker rm $(docker stop $(docker ps -aqf "name=apworks_mongodb" --format="{{.ID}}")); \
fi'
docker run -d -p 27017:27017 --name apworks_mongodb mongo

# Setting up Docker container for PostgreSQL
/bin/bash -c 'if [[ "$(docker ps -aqf "name=apworks_psql" 2> /dev/null)" != "" ]]; then \
    docker rm $(docker stop $(docker ps -aqf "name=apworks_psql" --format="{{.ID}}")); \
fi'
docker run -d -p 5432:5432 -e POSTGRESQL_USER=test -e POSTGRESQL_PASS=oe9jaacZLbR9pN -e POSTGRESQL_DB=test --name apworks_psql orchardup/postgresql

# Setting up Docker container for RabbitMQ
/bin/bash -c 'if [[ "$(docker ps -aqf "name=apworks_rabbitmq" 2> /dev/null)" != "" ]]; then \
    docker rm $(docker stop $(docker ps -aqf "name=apworks_rabbitmq" --format="{{.ID}}")); \
fi'
sudo docker run -d -p 5672:5672 -p 4369:4369 -p 5671:5671 -p 25672:25672 --name apworks_rabbitmq rabbitmq
