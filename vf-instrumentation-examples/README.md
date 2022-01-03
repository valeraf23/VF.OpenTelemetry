# snp-instrumentation-examples
This project is example how you can integrate with snp-instrumentation-sdk.

#### Demo

+ Turn on `VPN`
+ Go to `Snp folder` where is the `docker-compose.yml`
+ Run `docker-compose up --build`
+ Open [kibana](http://localhost:5601)
  + create ***__apm__*** index. Open `Stack Management --> Index Management`
  + add [**logging**](../../docs/logs) index
+ Open [swagger](http://localhost/swagger/index.html)
You can execute next cases:
![](./attachments/Swagger.png)
+ Open [kibana APM](http://localhost:5601/app/apm/services?rangeFrom=now-15m&rangeTo=now)

for ***data/parallel*** case should be rendered next chart

![](./attachments/data_parallel.png)

## Service Map
![](./attachments/map.png)

## Documentation for services

* [Master](./Src/Logging.Service.Master/README.md)
* [Slave1](./Src/Logging.Service.Slave1/README.md)
* [Slave2](./Src/Logging.Service.Slave2/README.md)
* [KafkaConsoleApp](./Src/Logging.KafkaConsoleApp/README.md)