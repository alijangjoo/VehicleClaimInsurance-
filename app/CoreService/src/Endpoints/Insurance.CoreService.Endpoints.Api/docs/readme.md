# docker-compose up -d
# docker-compose down -v
# docker-compose logs -f setup


# Restart APM Server
docker-compose restart apm-server

# Check the config to ensure secret_token is now set
docker exec apm-server cat /usr/share/apm-server/apm-server.yml | findstr secret_token


# Stop all containers
docker-compose down -v

# Remove any partially created containers
docker rm -f apm-server elasticsearch kibana setup 2>$null

# Prune the system (clean up any lingering containers)
docker system prune -f

# Now try starting again
docker-compose up -d