# Poliedro.Chapter.FeatureFlags

Chapter de apresentação para demonstrar Feature Flags com .NET 9

##  Sobre o Projeto

Esta é uma API de demonstração construída em .NET 9 que ilustra o uso efetivo de **Feature Flags** em aplicações reais. O projeto simula um sistema de e-commerce com produtos e pedidos, demonstrando como Feature Flags podem ser usadas para alterar regras de negócio sem necessidade de redeploy.

##  Tecnologias

- **.NET 9** - Framework principal
- **ASP.NET Core Web API** - API RESTful
- **Entity Framework Core** - ORM
- **InMemory Database** - Banco de dados em memória
- **Microsoft.FeatureManagement** - Gerenciamento de feature flags

##  Feature Flags Implementadas

O projeto demonstra 4 feature flags que controlam diferentes aspectos do sistema:

1. **NewDiscountCalculation** - Desconto progressivo vs desconto fixo
2. **PremiumPricing** - Precificação premium em eletrônicos
3. **ShowInactiveProducts** - Visibilidade de produtos inativos
4. **AdvancedProductFilters** - Filtros avançados de busca

##  Como Executar

```bash
cd src/Poliedro.Chapter.FeatureFlags.Api
dotnet run
```

A API estará disponível em `http://localhost:5000`

##  Documentação

Para documentação completa, incluindo exemplos de uso e cenários de teste, consulte:
- [README da API](src/Poliedro.Chapter.FeatureFlags.Api/README.md)

##  Testando as Feature Flags

### 1. Verificar status das feature flags
```bash
curl http://localhost:5000/api/featureflags | jq
```

### 2. Listar produtos
```bash
curl http://localhost:5000/api/products | jq
```

### 3. Criar pedido
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "João Silva",
    "items": [
      {"productId": 1, "quantity": 1},
      {"productId": 2, "quantity": 2}
    ]
  }' | jq
```

### 4. Buscar produtos (filtros avançados)
```bash
curl -X POST http://localhost:5000/api/products/search \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Eletrônicos",
    "minPrice": 3000,
    "maxPrice": 6000
  }' | jq
```

##  Configurando Feature Flags

As feature flags são configuradas no arquivo `src/Poliedro.Chapter.FeatureFlags.Api/appsettings.json`:

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

Altere os valores entre `true` e `false` para habilitar/desabilitar as funcionalidades.

##  Cenários de Demonstração

### Cenário 1: Desconto Progressivo
- **Flag Desabilitada**: Desconto fixo de 5% em todos os pedidos
- **Flag Habilitada**: Desconto progressivo (5%, 10%, 15% baseado no valor)

### Cenário 2: Precificação Premium
- **Flag Desabilitada**: Preço base dos produtos
- **Flag Habilitada**: +20% de margem em produtos eletrônicos

### Cenário 3: Produtos Inativos
- **Flag Desabilitada**: Mostra apenas 5 produtos ativos
- **Flag Habilitada**: Mostra todos os 6 produtos (incluindo inativo)

### Cenário 4: Filtros Avançados
- **Flag Desabilitada**: Busca simples por nome
- **Flag Habilitada**: Busca por nome, categoria e faixa de preço

##  Estrutura do Projeto

```
src/Poliedro.Chapter.FeatureFlags.Api/
├── Controllers/          # Endpoints da API
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   └── FeatureFlagsController.cs
├── Models/              # Modelos de dados
│   ├── Product.cs
│   ├── Order.cs
│   └── DTOs.cs
├── Data/                # Contexto do banco de dados
│   └── ApplicationDbContext.cs
├── Services/            # Lógica de negócio
│   ├── PricingService.cs
│   └── DiscountService.cs
├── Features/            # Definição de feature flags
│   └── AppFeatureFlags.cs
└── Program.cs           # Configuração da aplicação
```

##  Conceitos Demonstrados

- ✅ Feature Toggle Pattern
- ✅ Separação de Responsabilidades
- ✅ Injeção de Dependência
- ✅ API RESTful
- ✅ EF Core com In-Memory Database
- ✅ Clean Architecture
- ✅ Configuração Externa


---

**Desenvolvido para o Poliedro Chapter sobre Feature Flags** 🚀
