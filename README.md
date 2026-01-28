## Para rodar sem problemas:

docker compose up -d --build


## Aqui está o site com front, é o último link gerado.

docker compose logs tunnel-front | grep "trycloudflare.com"

## Aqui está o teste do back, só pegar o último link gerado e adicionar ao final dele "\swagger".

docker compose logs tunnel-api | grep "trycloudflare.com"



## 📸 Screenshots do Sistema

### Front-End
![FrontEnd](https://lh3.googleusercontent.com/d/1rH9sYiwEsMKxD7Pj30Y8jQI2Fxq4ytQX)

### Dashboard de Métricas
![Dashboard](https://lh3.googleusercontent.com/d/1n8k_9r0IfrYL-byiuBAQVeGLp6VD9ZjJ)

### Gerenciamento de Reservas (Swagger)
![Swagger](https://lh3.googleusercontent.com/d/1lLGVSPw9hNJwswhblMeaDQP26GRfuRXI)

### Monitoramento de Jobs (Hangfire)
![Hangfire](https://lh3.googleusercontent.com/d/1e8dyGZD9K6H3BTQQfVwYJa4feVT4z4TV)
