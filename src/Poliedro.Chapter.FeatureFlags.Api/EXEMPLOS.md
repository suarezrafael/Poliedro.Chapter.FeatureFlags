# Exemplos de Requisições - Feature Flags API

Este documento contém exemplos práticos de requisições para testar a API e demonstrar as feature flags.

##  1. Consultar Status das Feature Flags

### Listar todas as feature flags
```bash
curl http://localhost:5000/api/featureflags | jq
```

**Resposta esperada:**
```json
{
  "NewDiscountCalculation": {
    "enabled": false,
    "description": "Novo cálculo de desconto progressivo...",
    "oldBehavior": "Desconto fixo de 5%",
    "newBehavior": "Desconto progressivo: 5% até R$1000, 10% até R$5000, 15% acima"
  },
  ...
}
```

### Verificar uma flag específica
```bash
curl http://localhost:5000/api/featureflags/NewDiscountCalculation | jq
```

##  2. Produtos

### Listar todos os produtos
```bash
curl http://localhost:5000/api/products | jq
```

**Com ShowInactiveProducts = false:** 5 produtos  
**Com ShowInactiveProducts = true:** 6 produtos (inclui produto inativo)

### Obter produto específico
```bash
curl http://localhost:5000/api/products/1 | jq
```

### Buscar produtos com filtros avançados
```bash
# Buscar produtos da categoria Eletrônicos
curl -X POST http://localhost:5000/api/products/search \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Eletrônicos"
  }' | jq

# Buscar produtos por faixa de preço
curl -X POST http://localhost:5000/api/products/search \
  -H "Content-Type: application/json" \
  -d '{
    "minPrice": 1000,
    "maxPrice": 5000
  }' | jq

# Busca completa com todos os filtros
curl -X POST http://localhost:5000/api/products/search \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Logitech",
    "category": "Periféricos",
    "minPrice": 400,
    "maxPrice": 600
  }' | jq
```

**Nota:** Filtros de categoria e preço só funcionam quando `AdvancedProductFilters = true`

##  3. Pedidos

### Criar pedido pequeno (< R$ 1.000)
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "João Silva",
    "items": [
      {
        "productId": 2,
        "quantity": 2
      }
    ]
  }' | jq
```

**Valor total:** R$ 900 (2x R$ 450)  
**Com NewDiscountCalculation = false:** 5% desconto (R$ 45)  
**Com NewDiscountCalculation = true:** 5% desconto (R$ 45)  

### Criar pedido médio (R$ 1.000 - R$ 5.000)
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Maria Santos",
    "items": [
      {
        "productId": 4,
        "quantity": 1
      }
    ]
  }' | jq
```

**Valor total:** R$ 2.800 (sem premium) ou R$ 3.360 (com premium)  
**Com NewDiscountCalculation = false:** 5% desconto  
**Com NewDiscountCalculation = true:** 10% desconto  

### Criar pedido grande (> R$ 5.000)
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Pedro Costa",
    "items": [
      {
        "productId": 1,
        "quantity": 2
      },
      {
        "productId": 4,
        "quantity": 1
      }
    ]
  }' | jq
```

**Valor total:** R$ 11.800 (sem premium) ou R$ 14.160 (com premium)  
**Com NewDiscountCalculation = false:** 5% desconto (R$ 590 ou R$ 708)  
**Com NewDiscountCalculation = true:** 15% desconto (R$ 1.770 ou R$ 2.124)  

### Listar todos os pedidos
```bash
curl http://localhost:5000/api/orders | jq
```

### Obter pedido específico
```bash
curl http://localhost:5000/api/orders/1 | jq
```

##  4. Comparações de Cenários

### Cenário A: Pedido com Flags Desabilitadas

**Configuração:** Todas as flags = false

**Produtos:**
- Notebook: R$ 4.500
- Monitor: R$ 2.800
- Total: 5 produtos visíveis

**Pedido (1 Notebook + 1 Monitor):**
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Cliente A",
    "items": [
      {"productId": 1, "quantity": 1},
      {"productId": 4, "quantity": 1}
    ]
  }' | jq
```

**Resultado:**
- Total: R$ 7.300
- Desconto: R$ 365 (5%)
- Final: R$ 6.935

### Cenário B: Pedido com Flags Habilitadas

**Configuração:** Todas as flags = true

**Produtos:**
- Notebook: R$ 5.400 (+20%)
- Monitor: R$ 3.360 (+20%)
- Total: 6 produtos visíveis

**Pedido (1 Notebook + 1 Monitor):**
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Cliente B",
    "items": [
      {"productId": 1, "quantity": 1},
      {"productId": 4, "quantity": 1}
    ]
  }' | jq
```

**Resultado:**
- Total: R$ 8.760
- Desconto: R$ 876 (10% - desconto progressivo)
- Final: R$ 7.884

**Diferença:** R$ 949 a mais com as novas estratégias!

##  5. Teste de Filtros Avançados

### Com AdvancedProductFilters = false
```bash
# Esta busca só considera o nome
curl -X POST http://localhost:5000/api/products/search \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mouse",
    "category": "Eletrônicos",
    "minPrice": 1000
  }' | jq
```

**Resultado:** Retorna o Mouse (ignora categoria e preço)

### Com AdvancedProductFilters = true
```bash
# Esta busca considera todos os campos
curl -X POST http://localhost:5000/api/products/search \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mouse",
    "category": "Eletrônicos",
    "minPrice": 1000
  }' | jq
```

**Resultado:** Retorna vazio (Mouse não é Eletrônico e não custa mais que R$ 1.000)


##  Alternando Feature Flags

Para alternar as feature flags, edite o arquivo `appsettings.json`:

```json
{
  "FeatureManagement": {
    "NewDiscountCalculation": true,
    "PremiumPricing": true,
    "ShowInactiveProducts": true,
    "AdvancedProductFilters": true
  }
}
```

Reinicie a aplicação para aplicar as mudanças.

---

**Nota:** Todos os exemplos assumem que a API está rodando em `http://localhost:5000`
