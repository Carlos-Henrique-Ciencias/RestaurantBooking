# 🍽️ Restaurant Booking System - Desafio Técnico

Este projeto é uma solução robusta para gerenciamento de reservas de restaurantes, focada em alta performance, escalabilidade e manutenibilidade.

🎥 **[ASSISTA AQUI O VÍDEO DE APRESENTAÇÃO TÉCNICA](https://youtu.be/BSnxxpPxKlU)**

## 🛠️ Tecnologias Utilizadas

* **.NET 8** (C#)
* **PostgreSQL** (Persistência de dados)
* **Redis** (Cache Híbrido L1/L2)
* **RabbitMQ** (Mensageria e processamento assíncrono)
* **Hangfire** (Agendamento de tarefas recorrentes)
* **xUnit & FluentAssertions** (Testes Unitários)
* **Docker & Docker Compose** (Containerização)

## 🏗️ Arquitetura e Decisões de Projeto

O sistema foi desenvolvido seguindo os princípios da **Clean Architecture**, dividido em camadas de Domínio, Aplicação, Infraestrutura e API.

* **CQRS Manual:** Implementado sem bibliotecas externas para manter o código explícito e facilitar a navegação.
* **Cache Híbrido:** O Dashboard utiliza uma estratégia de cache em memória e Redis para garantir respostas instantâneas e reduzir a carga no banco de dados.
* **Resiliência:** O processamento de No-Show é feito via Job recorrente que publica eventos no RabbitMQ, garantindo que o sistema seja assíncrono e resiliente.

## ⚖️ Regras de Negócio Implementadas (RO-01 a RO-05)

Para garantir a integridade do domínio e o fluxo correto do sistema, as seguintes regras foram blindadas na camada de **Domain**:

### 📅 Cadastro de Reservas (RO-01)
* **Status Inicial:** Toda reserva é criada obrigatoriamente com o status PENDING.
* **Antecedência Mínima:** A data da reserva deve ser de, no mínimo, 2 horas no futuro em relação ao horário de criação.
* **Capacidade de Convidados:** O campo NumberOfGuests deve estar entre 1 e 20 pessoas.
* **Identificação Única:** O sistema gera automaticamente um Código Único (GUID) para cada reserva.
* **Validações de Contato:** E-mail e Telefone (10-11 dígitos) são validados rigorosamente via FluentValidation.

### 🔄 Confirmação e Check-in (RO-03 e RO-04)
* **Fluxo de Estados:** Uma reserva só pode ser confirmada se estiver PENDING.
* **Check-in:** O registro de chegada do cliente só é permitido para reservas que já foram previamente CONFIRMED.
* **Idempotência:** O processamento de confirmações via Webhook valida o transactionId para evitar duplicidade.

### 🤖 Processamento de No-Show (RO-05)
* **Monitoramento:** O Hangfire executa um Job recorrente configurado de hora em hora (Cron 0 * * * *).
* **Escalabilidade:** Para grandes volumes (acima de 1000), o sistema publica os IDs em lotes no RabbitMQ para processamento assíncrono.

## 🚀 Como Executar o Projeto

**Pré-requisitos:** Docker e Docker Compose instalados.

1. Na raiz do projeto, abra o terminal e execute o comando:
docker compose up -d

2. O sistema estará disponível em:
* **Swagger (API):** http://localhost:5130/swagger
* **Hangfire Dashboard:** http://localhost:5130/hangfire
* **RabbitMQ Management:** http://localhost:15672 (Usuário: booking / Senha: booking123)

## 🧪 Testes Unitários

Para validar as regras de negócio mencionadas, execute no terminal:
dotnet test

O projeto conta com os 12 testes obrigatórios cobrindo criação, validação de data, limites de convidados e transições de status.
## 📸 Screenshots do Sistema

### Dashboard de Métricas
![Dashboard](https://lh3.googleusercontent.com/d/1n8k_9r0IfrYL-byiuBAQVeGLp6VD9ZjJ)

### Gerenciamento de Reservas (Swagger)
![Swagger](https://lh3.googleusercontent.com/d/1lLGVSPw9hNJwswhblMeaDQP26GRfuRXI)

### Monitoramento de Jobs (Hangfire)
![Hangfire](https://lh3.googleusercontent.com/d/1e8dyGZD9K6H3BTQQfVwYJa4feVT4z4TV)
