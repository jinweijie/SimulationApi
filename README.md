# Introduction

This application is a set of handy APIs to simulate the various response scenarios like high CPU consumption, high memory consumption, high disk usage, application crash... etc.

You may use this application to test your microservices deployment environment, reverse proxy, monitoring infrastructure by simulating some abnormal behaviors from a specific container or multiple containers.

# Usage

## Run with Docker in 5 seconds
1. Ensure you have Docker installed. If not you may use [this repo](https://github.com/jinweijie/install-docker-and-compose) to install Docker with just one command.
2. Spin up a container `sudo docker run -d --rm -p 5001:80 jinweijiedocker/simulation-api`.
3. You have the APIs available on port 5001 on your local host.
4. Test the API with `curl http://localhost:5001/delay`.


## Run with Docker Compose in 10 seconds
1. Ensure you have Docker and Docker Compose installed. If not you may use [this repo](https://github.com/jinweijie/install-docker-and-compose) to install Docker and Docker Compose with just one command.
2. Clone this repository `git clone https://github.com/jinweijie/SimulationApi.git` and `cd SimulationApi`.
3. Since there is the [docker-compose.yml](https://github.com/jinweijie/SimulationApi/blob/master/docker-compose.yml) file in this repo, you can just run `sudo docker compose up -d` (If you're using the old `docker-compose`, then you need to run `sudo docker-compose up -d` ) and then you will have 3 containers running on port `5001`, `5002`, `5003`.

# Available APIs

You need to add your host and port prefix. For example, if you are running this application on `http://localhost:5001`, to use the delay endpoint with parameter 5, the full url will be `http://localhost:5001/delay/5`.

* `/` - Display system information.
* `/delay/{ms:int?}` - Simulate a slow response with specific milliseconds, default is 3000.
* `/delay/{msMin:int}/{msMax:int}` - Simulate a slow response with random milliseconds between `msMin` and `msMax`.
* `/cpu/{seconds:int?}/{percentage:int?}` - Simulate high CPU utilization. For example, to simulate 100% CPU utilization for 5 seconds, you can either access `http://localhost:5001/5/100` with browser or use command line `curl http://localhost:5001/5/100`.
* `/memory/{seconds:int?}/{sizeInM:int?}` - Simulate high memory usage.
* `/disk/{seconds:int?}/{sizeInM:int?}` - Simulate high disk usage. This endpoint will create a dummy file based on the size of `sizeInM` and delete after `seconds`.
* `/{statusCode:int}` - Return the status code. For example, `http://localhost:5001/401` will return status code 401.
* `/exception/{probability:int}` - Simulate random exception based on the `probability` provided.
* `/exception` - Simulate the server side exception every time.
* `/crash/{probability:int?}` - Simulate application crash based on the `probability` provided.
* `/crash` - Simulate application crash every time.
* `/exit` - Simulate application normal exit (exit code 0). 

## Donate

If you would like to support my development, feel free to buy me a coffee, it makes a big difference! Thanks.

<a href="https://www.buymeacoffee.com/jinweijie" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/white_img.png" alt="Buy Me A Coffee"></a>
