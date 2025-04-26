# 🚀 Gestão de Pedidos - Backend (ASP.NET Core + PostgreSQL)

API RESTful para gerenciamento de pedidos, desenvolvida com ASP.NET Core, Entity Framework Core e PostgreSQL. Responsável por fornecer endpoints para criação, consulta, atualização e remoção de pedidos, além de integração futura com Azure Service Bus.

## 📋 Funcionalidades

- 📝 Cadastro de novos pedidos
- 📋 Listagem de pedidos
- 🔎 Consulta detalhada por ID
- ✏️ Atualização de pedidos
- ❌ Remoção de pedidos
- 💬 Integração opcional com Azure Service Bus (mensageria)
- 🛡️ Validação de dados e tratamento de erros

## 🛠️ Tecnologias Utilizadas

- **ASP.NET Core** - Framework para APIs modernas em C#
- **Entity Framework Core** - ORM para acesso ao banco de dados
- **Npgsql** - Provider PostgreSQL para .NET
- **Azure Service Bus** (opcional) - Mensageria
- **PostgreSQL** - Banco de dados relacional

## 📦 Estrutura do Projeto

```
backend-gestao-de-pedidos/
├── GestaoDePedidosApi/
│   ├── Controllers/
│   │   └── OrdersController.cs   # Endpoints da API de pedidos
│   ├── Models/
│   │   └── Order.cs             # Modelo de domínio Pedido
│   ├── Data/
│   │   └── AppDbContext.cs      # Contexto do Entity Framework
│   ├── Services/
│   │   └── ServiceBusService.cs # Serviço de mensageria (Azure)
│   └── Program.cs               # Configuração principal
└── ...
```

## 💻 Como Executar

### 🏗️ Execução Local Tradicional

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd backend-gestao-de-pedidos
   ```

2. **Configure as variáveis de ambiente**
   - Crie (ou edite) um arquivo `appsettings.Development.json` ou use variáveis de ambiente para definir:
     - String de conexão com PostgreSQL
     - (Opcional) Configuração do Azure Service Bus

   Exemplo de configuração:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=gestaopedidos;Username=postgres;Password=senha"
     },
     "AzureServiceBus": {
       "ConnectionString": "<sua-connection-string>"
     }
   }
   ```

3. **Aplique as migrations e crie o banco**
   ```bash
   dotnet ef database update
   ```

4. **Execute o projeto**
   ```bash
   dotnet run --project GestaoDePedidosApi
   ```

5. **Acesse a API**
   - Por padrão, estará disponível em [http://localhost:5158](http://localhost:5158)
   - Swagger UI: [http://localhost:5158/swagger](http://localhost:5158/swagger)

---

### 🐳 Execução com Docker e Docker Compose

1. **Pré-requisitos:**
   - Docker e Docker Compose instalados

2. **Arquivos necessários:**
   - `Dockerfile` (na raiz do backend)
   - `docker-compose.yml` (na raiz do backend)

3. **Build e execução dos containers:**
   ```bash
   docker-compose up --build
   ```
   Isso irá:
   - Subir o banco de dados PostgreSQL já configurado
   - Buildar a imagem do backend e rodar a API em http://localhost:5158

4. **Connection String já configurada:**
   - O backend irá se conectar automaticamente ao banco via:
     ```
     Host=db;Port=5432;Database=gestaopedidos;Username=postgres;Password=senha
     ```
   - Não é necessário alterar nada ao rodar pelo compose.

5. **Acompanhe os logs:**
   ```bash
   docker-compose logs -f
   ```

6. **Parar os containers:**
   ```bash
   docker-compose down
   ```

---

> Com Docker Compose, você garante que o backend e o banco sempre estarão configurados e rodando juntos, facilitando o desenvolvimento e o deploy.

## 🎯 Principais Características

### Endpoints RESTful
- `GET /api/orders` - Lista todos os pedidos
- `GET /api/orders/{id}` - Detalha um pedido
- `POST /api/orders` - Cria um novo pedido
- `PUT /api/orders/{id}` - Atualiza um pedido existente
- `DELETE /api/orders/{id}` - Remove um pedido

### Modelo de Pedido
```csharp
public class Order {
    public Guid Id { get; set; }
    public string Cliente { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; }
    public DateTime DataCriacao { get; set; }
}
```

### Integração com Azure Service Bus
- Envio de mensagens para fila configurável (opcional)
- Não impede criação de pedidos caso a mensageria falhe

### Validação e Erros
- Validação automática via DataAnnotations
- Retorno de erros detalhados para o frontend

---

Desenvolvido para demonstrar arquitetura limpa, integração com banco de dados, mensageria e boas práticas de APIs RESTful.
