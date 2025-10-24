# Quick Start Guide - Feature Flags Demo

Este guia rápido te ajudará a executar e demonstrar a API de Feature Flags em minutos.

## 🚀 Executar a API

```bash
cd src/Poliedro.Chapter.FeatureFlags.Api
dotnet run
```

A API estará disponível em: `http://localhost:5000`

## 📋 Roteiro de Demonstração (5 minutos)

### 1. Verificar Feature Flags (30 segundos)

```bash
curl http://localhost:5000/api/featureflags | jq
```

**Mostre:** Todas as flags estão desabilitadas por padrão.

### 2. Listar Produtos (30 segundos)

```bash
curl http://localhost:5000/api/products | jq
```

**Destaque:**
- 5 produtos visíveis (produto 6 está inativo)
- Notebook a R$ 4.500, Monitor a R$ 2.800

### 3. Criar Pedido Grande (1 minuto)

```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "João Silva",
    "items": [
      {"productId": 1, "quantity": 2},
      {"productId": 4, "quantity": 1}
    ]
  }' | jq
```

**Destaque:**
- Total: R$ 11.800
- Desconto: R$ 590 (5% fixo)
- Final: R$ 11.210

### 4. Habilitar Feature Flags (30 segundos)

Pare a API (Ctrl+C) e edite `appsettings.json`:

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

Inicie a API novamente: `dotnet run`

### 5. Mostrar Mudanças (2 minutos)

#### 5a. Produtos com Premium Pricing
```bash
curl http://localhost:5000/api/products | jq
```

**Destaque:**
- 6 produtos agora (incluindo inativo)
- Notebook: R$ 5.400 (+20%)
- Monitor: R$ 3.360 (+20%)

#### 5b. Pedido com Novo Desconto
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Maria Santos",
    "items": [
      {"productId": 1, "quantity": 2},
      {"productId": 4, "quantity": 1}
    ]
  }' | jq
```

**Destaque:**
- Total: R$ 14.160 (com premium pricing)
- Desconto: R$ 2.124 (15% progressivo!)
- Final: R$ 12.036

**Comparação:**
- Antes: R$ 11.210 final
- Depois: R$ 12.036 final
- **Ganho extra de R$ 826 com as novas estratégias!**

#### 5c. Busca Avançada
```bash
curl -X POST http://localhost:5000/api/products/search \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Eletrônicos",
    "minPrice": 5000
  }' | jq
```

**Destaque:** Busca por categoria e preço funcionando!

## 🎯 Pontos-Chave para Mencionar

1. **Sem Redeploy** - Mudamos o comportamento apenas editando configuração
2. **Segurança** - Mudanças controladas e reversíveis
3. **A/B Testing** - Pode testar diferentes estratégias com clientes reais
4. **Rollback Rápido** - Se algo der errado, basta desabilitar a flag
5. **Business Value** - Aumento de receita com novas estratégias

## 💡 Cenários de Discussão

- **Gradual Rollout**: Habilitar flags para 10%, depois 50%, depois 100% dos usuários
- **Canary Releases**: Testar com um grupo pequeno primeiro
- **Kill Switch**: Desabilitar rapidamente se houver problemas
- **Experimentos**: Comparar métricas entre estratégias

## 📊 Métricas de Demonstração

| Métrica | Sem Flags | Com Flags | Melhoria |
|---------|-----------|-----------|----------|
| Desconto em pedido grande | 5% (R$ 590) | 15% (R$ 2.124) | 259% mais |
| Preço Notebook | R$ 4.500 | R$ 5.400 | +20% margem |
| Produtos visíveis | 5 | 6 | +20% catálogo |
| Funcionalidades busca | 1 (nome) | 4 (nome, categoria, preço) | 4x mais |

## 🔄 Reset para Próxima Demo

1. Pare a API
2. Desabilite as flags em `appsettings.json`
3. Reinicie a API (o banco InMemory será recriado automaticamente)

## 📚 Mais Informações

- README principal: [README.md](README.md)
- Exemplos detalhados: [EXAMPLES.md](src/Poliedro.Chapter.FeatureFlags.Api/EXAMPLES.md)
- Documentação da API: [API README](src/Poliedro.Chapter.FeatureFlags.Api/README.md)

---

**Tempo total da demo:** ~5 minutos  
**Impacto:** Alto! Mostra valor de negócio claro com Feature Flags 🚀
