# WM - Warehouse Manager

Gerenciamento de armazéns, responsável por orquestrar entradas e saídas de mercadorias


> :warning: [Diretrizes de contribuição para este projeto](docs/CONTRIBUTING.md)


## Setup

> **Requisitos**
>
> * [Dotnet Core 6.0.x](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
> * [Docker](https://www.docker.com/products/docker-desktop/)
> * [RabbitMQ](https://www.rabbitmq.com)
> * [VSCode](https://code.visualstudio.com)

## Dependências
As dependências são resolvidas com *docker-compose.yml*
> * [UAA](https://github.com/CetroDevelopment/UAA)
> * [Elasticsearch, Logstash, Kibana (ELK)](https://elk-docker.readthedocs.io)
> * [APM monitoring](https://www.elastic.co/pt/observability/application-performance-monitoring)

## Ambiente de desenvolvimento

1. Clonar o repositório
   
   ```
   git clone git@github.com:CetroDevelopment/WM.git
   ```

2. Navegue até o diretório da aplicação
   
   ```
   cd {path-do-projeto}/WM
   ```

3. Compile a aplicação
   
   ```
   dotnet build
   ```

4. Execute o docker
   
Contrua e suba os containers utilizando o *docker-compose*, dessa forma irá subir uma imagem com RabbitMQ e a api do WM

``` 
docker-compose build
docker-compose -f docker-compose-dev.yml up -d
```

5. Execute com o Kubernetes

```
kubectl apply -f k8s/api.wm.yml
```

> Acesse o swagger através do endereço: http://kubernetes.docker.internal/swagger

Ou suba manualmente 1 instância do RabbitMQ e rode a api através do vsCode, contro do diretório .vscode, possui os arquivos launch.json e tasks.json. Através da opção `Executar e Depurar`, seleciona o compound `API` e execute.

## Testes

Na raiz da aplicação execute

```
dotnet test
```

## Arquitetura

A solução está baseada na [Arquitetura Hexagonal](https://engsoftmoderna.info/artigos/arquitetura-hexagonal.html)

![Arquitetura](/docs/img/Arquitetura.png)

## Esquema Estrutural

![Estrutura](/docs/img/Estrutura.png)

## Fluxo do Sistema

![Fluxo do sistema](/docs/img/Fluxo.png)

## ROADMAP

:construction: