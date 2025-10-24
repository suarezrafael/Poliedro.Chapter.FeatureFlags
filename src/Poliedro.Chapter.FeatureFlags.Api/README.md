# Poliedro Chapter - Feature Flags Demo API

Este projeto é uma API de demonstração construída em .NET 9 para apresentar o uso de **Feature Flags** em aplicações reais.

##  Visão Geral

A aplicação simula um sistema de e-commerce simplificado com produtos e pedidos, demonstrando como Feature Flags podem ser usadas para:
- Alterar regras de negócio sem deploy
- Testar novas funcionalidades com segurança
- Implementar releases graduais
- A/B testing de estratégias de negócio

##  Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **InMemory Database** - Banco de dados em memória para demonstração
- **Microsoft.FeatureManagement** - Biblioteca de gerenciamento de feature flags

##  Feature Flags Implementadas

### 1. NewDiscountCalculation
**Propósito**: Controla a estratégia de cálculo de desconto

- **Desabilitada (Estratégia Antiga)**: Desconto fixo de 5% em todos os pedidos
- **Habilitada (Estratégia Nova)**: Desconto progressivo baseado no valor:
  - 5% para pedidos até R$ 1.000
  - 10% para pedidos até R$ 5.000
  - 15% para pedidos acima de R$ 5.000

**Impacto no Negócio**: Incentiva compras de maior valor com descontos crescentes

### 2. PremiumPricing
**Propósito**: Controla o sistema de precificação de produtos

- **Desabilitada (Estratégia Antiga)**: Usa o preço base do produto
- **Habilitada (Estratégia Nova)**: Aplica margem premium de 20% em produtos da categoria "Eletrônicos"

**Impacto no Negócio**: Aumenta a margem de lucro em produtos eletrônicos

### 3. ShowInactiveProducts
**Propósito**: Controla a visibilidade de produtos inativos

- **Desabilitada**: Mostra apenas produtos ativos no catálogo
- **Habilitada**: Mostra todos os produtos, incluindo inativos

**Impacto no Negócio**: Útil para administradores ou previews de produtos

### 4. AdvancedProductFilters
**Propósito**: Habilita filtros avançados de busca

- **Desabilitada**: Busca simples apenas por nome do produto
- **Habilitada**: Busca avançada por nome, categoria e faixa de preço

**Impacto no Negócio**: Melhora a experiência de busca do usuário

##  Modelo de Dados

### Product
```csharp
- Id (int)
- Name (string)
- Description (string)
- BasePrice (decimal)
- Category (string)
- StockQuantity (int)
- IsActive (bool)
- CreatedAt (DateTime)
```

### Order
```csharp
- Id (int)
- CustomerName (string)
- OrderDate (DateTime)
- TotalAmount (decimal)
- DiscountAmount (decimal)
- FinalAmount (decimal)
- Status (string)
- Items (List<OrderItem>)
```

##  Configuração

As feature flags são configuradas no arquivo `appsettings.json`:

```json
{
  "FeatureManagement": {
    "NewDiscountCalculation": false,
    "PremiumPricing": false,
    "ShowInactiveProducts": false,
    "AdvancedProductFilters": false
  }
}
```

Para habilitar/desabilitar uma feature flag, basta alterar o valor entre `true` e `false`.

##  Como Executar

1. Clone o repositório
2. Navegue até a pasta do projeto:
   ```bash
   cd src/Poliedro.Chapter.FeatureFlags.Api
   ```
3. Execute a aplicação:
   ```bash
   dotnet run
   ```
4. A API estará disponível em: `https://localhost:5001` ou `http://localhost:5000`

## Endpoints da API

### Feature Flags
- `GET /api/featureflags` - Lista todas as feature flags e seus status
- `GET /api/featureflags/{flagName}` - Verifica o status de uma flag específica

### Products
- `GET /api/products` - Lista todos os produtos
- `GET /api/products/{id}` - Obtém um produto específico
- `POST /api/products/search` - Busca produtos com filtros

**Exemplo de busca**:
```json
{
  "name": "Mouse",
  "category": "Periféricos",
  "minPrice": 100,
  "maxPrice": 1000
}
```

### Orders
- `GET /api/orders` - Lista todos os pedidos
- `GET /api/orders/{id}` - Obtém um pedido específico
- `POST /api/orders` - Cria um novo pedido

**Exemplo de criação de pedido**:
```json
{
  "customerName": "João Silva",
  "items": [
    {
      "productId": 1,
      "quantity": 1
    },
    {
      "productId": 2,
      "quantity": 2
    }
  ]
}
```

##  Cenários de Teste

### Cenário 1: Testando Desconto Progressivo

1. **Com a flag desabilitada** (`NewDiscountCalculation: false`):
   - Crie um pedido de R$ 500 → Desconto de 5% (R$ 25)
   - Crie um pedido de R$ 3000 → Desconto de 5% (R$ 150)
   - Crie um pedido de R$ 6000 → Desconto de 5% (R$ 300)

2. **Com a flag habilitada** (`NewDiscountCalculation: true`):
   - Crie um pedido de R$ 500 → Desconto de 5% (R$ 25)
   - Crie um pedido de R$ 3000 → Desconto de 10% (R$ 300)
   - Crie um pedido de R$ 6000 → Desconto de 15% (R$ 900)

### Cenário 2: Testando Precificação Premium

1. **Com a flag desabilitada** (`PremiumPricing: false`):
   - Produto 1 (Notebook - Eletrônicos) → R$ 4.500,00
   - Produto 2 (Mouse - Periféricos) → R$ 450,00

2. **Com a flag habilitada** (`PremiumPricing: true`):
   - Produto 1 (Notebook - Eletrônicos) → R$ 5.400,00 (+20%)
   - Produto 2 (Mouse - Periféricos) → R$ 450,00 (sem alteração)

### Cenário 3: Produtos Inativos

1. **Com a flag desabilitada** (`ShowInactiveProducts: false`):
   - Lista produtos → 5 produtos (somente ativos)

2. **Com a flag habilitada** (`ShowInactiveProducts: true`):
   - Lista produtos → 6 produtos (incluindo produto ID 6 - Headset inativo)

### Cenário 4: Filtros Avançados

1. **Com a flag desabilitada** (`AdvancedProductFilters: false`):
   - Busca só considera o campo `name`
   - Campos `category`, `minPrice`, `maxPrice` são ignorados

2. **Com a flag habilitada** (`AdvancedProductFilters: true`):
   - Busca considera todos os filtros disponíveis
   - Permite buscar por categoria, faixa de preço, etc.

##  Conceitos Demonstrados

1. **Separação de Responsabilidades**: Services para lógicas de negócio (Pricing, Discount)
2. **Injeção de Dependência**: Uso de interfaces e DI do ASP.NET Core
3. **Feature Toggle Pattern**: Alternância entre implementações antigas e novas
4. **Configuração Externa**: Feature flags em appsettings.json
5. **Clean Architecture**: Controllers, Services, Data, Models separados
6. **API RESTful**: Endpoints bem definidos e documentados

##  Boas Práticas Demonstradas

- ✅ Uso de DTOs para requisições e respostas
- ✅ Validação de entrada nos endpoints
- ✅ Logging de operações importantes
- ✅ Tratamento de erros apropriado
- ✅ Documentação inline com comentários XML
- ✅ Nomenclatura clara e descritiva
- ✅ Código limpo e organizado

##  Referências

- [Microsoft Feature Management](https://github.com/microsoft/FeatureManagement-Dotnet)
- [Feature Toggles (Martin Fowler)](https://martinfowler.com/articles/feature-toggles.html)
- [.NET 9 Documentation](https://docs.microsoft.com/dotnet/)

---

