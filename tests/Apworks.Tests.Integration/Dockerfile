FROM microsoft/dotnet
RUN apt-get update && apt-get upgrade
RUN apt-get install git
RUN mkdir /app
WORKDIR /app
RUN git clone https://github.com/daxnet/apworks-core
WORKDIR /app/apworks-core/tests/Apworks.Tests
VOLUME ["/app/apworks-core/tests/Apworks.Tests/TestResults"]
CMD ["dotnet", "test", "-l", ""trx""]
