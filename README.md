# 📘 Sales API

API para gerenciamento de vendas, permitindo o cadastro, consulta, cancelamento total e parcial de pedidos e itens.

## 🚀 Tecnologias Utilizadas

- ASP.NET Core  
- Swagger UI  
- Entity Framework Core  
- Postgres

## 🗄️ Criando Banco de Dados

```
dotnet restore
dotnet build
dotnet ef database update

```

## 📦 Como Executar Localmente

```
git clone https://github.com/nascgabe/abi-gth-developer-evaluation.git
dotnet build
dotnet run
```

Acesse `https://localhost:7181/swagger` (Ou na porta que estiver configurado no launchSettings.json) para abrir a interface do Swagger.

---

## 🛒 PRODUTO

### ▶️ POST `/api/Product`

Cria um novo produto.

#### 🧾 Exemplo de Requisição

```json
{
  "title": "Lager Beer 350ml",
  "description": "Refreshing lager beer with a smooth taste, perfect for any occasion.",
  "category": "Beverages",
  "image": "https://example.com/images/ambev-lager-350ml.png",
  "price": 3.49,
  "stock": 120,
  "rating": {
    "rate": 4.5,
    "count": 87
  }
}
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "id": "8f55b258-6fc7-4049-adc5-2f9e7e9edf69",
    "name": "",
    "description": "Refreshing lager beer with a smooth taste, perfect for any occasion.",
    "price": 3.49,
    "stock": 0
  },
  "success": true,
  "message": "Product created successfully",
  "errors": []
}
```

---

### ▶️ POST `/api/Product/batch`

Cria novos produtos em lote.

#### 🧾 Exemplo de Requisição

```json
[
  {
    "title": "Lager Beer 350ml",
    "description": "Refreshing lager beer with a smooth taste, perfect for any occasion.",
    "category": "Beverages",
    "image": "https://example.com/images/ambev-lager-350ml.png",
    "price": 3.49,
    "stock": 120,
    "rating": {
      "rate": 4.5,
      "count": 87
    }
  },
  {
    "title": "IPA Beer 500ml",
    "description": "Strong and hoppy IPA for true beer lovers.",
    "category": "Beverages",
    "image": "https://example.com/images/ambev-ipa-500ml.png",
    "price": 5.99,
    "stock": 80,
    "rating": {
      "rate": 4.7,
      "count": 112
    }
  },
  {
    "title": "Pilsen Beer 269ml",
    "description": "Light and crisp Pilsen, perfect for summer days.",
    "category": "Beverages",
    "image": "https://example.com/images/ambev-pilsen-269ml.png",
    "price": 2.99,
    "stock": 150,
    "rating": {
      "rate": 4.3,
      "count": 65
    }
  }
]
```
#### ✅ Exemplo de Resposta

```json
{
  "data": [
    {
      "id": "4438239c-a7bc-4535-a8da-579ec100e690",
      "name": "",
      "description": "Refreshing lager beer with a smooth taste, perfect for any occasion.",
      "price": 3.49,
      "stock": 0
    },
    {
      "id": "ed38cf51-cd8e-42db-8318-e1ec8c2d55e9",
      "name": "",
      "description": "Strong and hoppy IPA for true beer lovers.",
      "price": 5.99,
      "stock": 0
    },
    {
      "id": "334537ee-29e8-4227-bba0-26bb03f59cd9",
      "name": "",
      "description": "Light and crisp Pilsen, perfect for summer days.",
      "price": 2.99,
      "stock": 0
    }
  ],
  "success": true,
  "message": "Batch products created successfully",
  "errors": []
}
```
---

### 🔍 GET `/api/Product/{id}`

Busca produto por ID.

#### 🧾 Exemplo de Requisição

```
Informar no Swagger Id do produto que deseja pesquisar.
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "data": {
      "id": "334537ee-29e8-4227-bba0-26bb03f59cd9",
      "title": "Pilsen Beer 269ml",
      "price": 2.99,
      "description": "Light and crisp Pilsen, perfect for summer days.",
      "category": "Beverages",
      "stock": 150,
      "image": "https://example.com/images/ambev-pilsen-269ml.png",
      "rating": {
        "rate": 4.3,
        "count": 65
      },
      "createdAt": "2025-04-08T22:33:34.463964Z",
      "updatedAt": null
    },
    "success": true,
    "message": "Product retrieved successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```
---

### ✏️ PATCH `/api/Product/{id}`

Atualiza informações do produto.

#### 🧾 Exemplo de Requisição

```json
{
  "title": "string"
}
Informar ID do produto que deseja alterar informação.
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "data": {
      "id": "334537ee-29e8-4227-bba0-26bb03f59cd9",
      "title": "Blonde Ale",
      "description": "Light and crisp Pilsen, perfect for summer days.",
      "category": "Beverages",
      "image": "https://example.com/images/ambev-pilsen-269ml.png",
      "price": 2.99,
      "stock": 150
    },
    "success": true,
    "message": "Product updated successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```
---

### ❌ DELETE `/api/Product/{id}`

Deleta produto.

#### 🧾 Exemplo de Requisição

```
Informar ID do produto que deseja deletar.
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "success": true,
    "message": "Product deleted successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```

---

## 💰 VENDAS

### ▶️ POST `/api/Sales`

Cria nova venda.

⚠️ **Atenção**: Para registrar o cliente na venda, o usuário deve estar autenticado.  
Veja a seção **AUTH** abaixo para saber como realizar a autenticação e obter o token de acesso.

#### 🧾 Exemplo de Requisição

```json
{
  "branch": "Store ABC",
  "items": [
    {
      "productId": "ed38cf51-cd8e-42db-8318-e1ec8c2d55e9",
      "quantity": 10
    }
  ]
}
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "id": "78a7876a-0024-411a-8e39-070e919d888b",
    "saleDate": "2025-04-08T22:52:56.9798528Z",
    "client": "Gabriel Nascimento",
    "branch": "Store ABC",
    "totalValue": 47.92,
    "items": [
      {
        "productId": "ed38cf51-cd8e-42db-8318-e1ec8c2d55e9",
        "productName": "IPA Beer 500ml",
        "quantity": 10,
        "unitPrice": 5.99,
        "discount": 11.98,
        "totalValue": 47.92
      }
    ]
  },
  "success": true,
  "message": "Sale created successfully",
  "errors": []
}
```
---

### 🔍 GET `/api/Sales`

Retorna todas as vendas registradas.

#### 🧾 Exemplo de Requisição

```
Como esse endpoint é um getAll é necessário somente enviar a requisição.
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "data": [
      {
        "id": "1a1b41d1-c010-4fa5-917f-9c597641ecb2",
        "saleNumber": "0004",
        "saleDate": "2025-04-08T01:11:44.146087Z",
        "client": "Gabriel Santos",
        "branch": "Branch C",
        "totalValue": 0,
        "items": []
      },
      {
        "id": "78a7876a-0024-411a-8e39-070e919d888b",
        "saleNumber": "0008",
        "saleDate": "2025-04-08T22:52:56.979852Z",
        "client": "Gabriel Nascimento",
        "branch": "Store ABC",
        "totalValue": 47.92,
        "items": [
          {
            "productId": "ed38cf51-cd8e-42db-8318-e1ec8c2d55e9",
            "productName": "IPA Beer 500ml",
            "quantity": 10,
            "unitPrice": 5.99,
            "discount": 11.98,
            "totalValue": 47.92
          }
        ]
      },
      {
        "id": "7ec7b25d-cc21-40b3-9a70-5238030b7614",
        "saleNumber": "0006",
        "saleDate": "2025-04-08T01:52:29.004393Z",
        "client": "Gabriel Santos",
        "branch": "Branch A",
        "totalValue": 0,
        "items": [
          {
            "productId": "50f7c05c-5d29-4b63-b530-33690c3151fb",
            "productName": "Soda",
            "quantity": 0,
            "unitPrice": 10,
            "discount": 0,
            "totalValue": 0
          }
        ]
      },
      {
        "id": "84b6d031-c571-4ee7-9335-2ebc41a7bd79",
        "saleNumber": "0003",
        "saleDate": "2025-04-08T01:11:35.712696Z",
        "client": "Gabriel Santos",
        "branch": "Branch B",
        "totalValue": 141,
        "items": [
          {
            "productId": "50f7c05c-5d29-4b63-b530-33690c3151fb",
            "productName": "Soda",
            "quantity": 12,
            "unitPrice": 10,
            "discount": 24,
            "totalValue": 96
          },
          {
            "productId": "f164caf1-9c1a-4589-88e1-13dbb30a9bb8",
            "productName": "Beer",
            "quantity": 5,
            "unitPrice": 10,
            "discount": 5,
            "totalValue": 45
          }
        ]
      },
      {
        "id": "a0f432e7-fbf9-4cf4-b842-d63189ae7eb0",
        "saleNumber": "0007",
        "saleDate": "2025-04-08T14:21:57.196528Z",
        "client": "Gabriel Santos",
        "branch": "Teste Bancada",
        "totalValue": 134.96,
        "items": [
          {
            "productId": "b471906a-4754-4daa-b62e-bfb29361d347",
            "productName": "Gaming Mouse",
            "quantity": 5,
            "unitPrice": 29.99,
            "discount": 15,
            "totalValue": 134.96
          }
        ]
      },
      {
        "id": "a6e56e31-0ba1-4a53-b38e-3563dc734857",
        "saleNumber": "0005",
        "saleDate": "2025-04-08T01:33:25.200601Z",
        "client": "Gabriel Santos",
        "branch": "Branch A",
        "totalValue": 45,
        "items": [
          {
            "productId": "50f7c05c-5d29-4b63-b530-33690c3151fb",
            "productName": "Soda",
            "quantity": 5,
            "unitPrice": 10,
            "discount": 5,
            "totalValue": 45
          }
        ]
      },
      {
        "id": "c1e9a684-6da2-48e6-bb01-f7c6aaf0dc85",
        "saleNumber": "0001",
        "saleDate": "2025-04-07T23:31:29.828959Z",
        "client": "Gabriel Santos",
        "branch": "Central Store",
        "totalValue": 96,
        "items": [
          {
            "productId": "50f7c05c-5d29-4b63-b530-33690c3151fb",
            "productName": "Soda",
            "quantity": 5,
            "unitPrice": 10,
            "discount": 5,
            "totalValue": 45
          },
          {
            "productId": "f164caf1-9c1a-4589-88e1-13dbb30a9bb8",
            "productName": "Beer",
            "quantity": 12,
            "unitPrice": 10,
            "discount": 24,
            "totalValue": 96
          }
        ]
      }
    ],
    "success": true,
    "message": "Sales retrieved successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```

---

### 🔍 GET `/api/Sales/{id}`

Consulta uma venda pelo `saleId`.

#### 🧾 Exemplo de Requisição

```
Informar ID da venda que deseja buscar.
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "data": {
      "id": "78a7876a-0024-411a-8e39-070e919d888b",
      "saleNumber": "0008",
      "saleDate": "2025-04-08T22:52:56.979852Z",
      "client": "Gabriel Nascimento",
      "branch": "Store ABC",
      "totalValue": 47.92,
      "items": [
        {
          "productId": "ed38cf51-cd8e-42db-8318-e1ec8c2d55e9",
          "productName": "IPA Beer 500ml",
          "quantity": 10,
          "unitPrice": 5.99,
          "discount": 11.98,
          "totalValue": 47.92
        }
      ]
    },
    "success": true,
    "message": "Sale retrieved successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```

---

### 🔁 PUT `/api/Sales/{saleId}/cancel`

Cancela o pedido e muda flag `IsCancelled` para `true`.

#### 🧾 Exemplo de Requisição

```
Informar ID da venda que deseja cancelar.
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "success": true,
    "message": "Sale cancelled successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```

---

### 🔁 PUT `/api/Sales/{saleId}/items/{itemId}/partial-cancellation`

Cancela item do pedido parcialmente. Muda a quantidade do item.

#### 🧾 Exemplo de Requisição

```
Informar ID da venda e do item que deseja fazer o cancelamento parcial.

{
  "quantity": 5
}
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "success": true,
    "message": "Item quantity updated successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```

---

### ❌ DELETE `/api/Sales/{saleId}/items/{itemId}`

Exclui item do pedido.

#### 🧾 Exemplo de Requisição

```
Informar ID da venda e do item que deseja excluir do pedido.
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "success": true,
    "message": "Item deleted successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```

---

### ❌ DELETE `/api/Sales/{saleId}`

Deleta pedido.

#### 🧾 Exemplo de Requisição

```
Informar ID da venda que deseja excluir.
```
#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "success": true,
    "message": "Sale deleted successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```
---

## 👤 USER

### ▶️ POST `/api/Users`

Cria novo usuário.

#### 🧾 Exemplo de Requisição

```json
{
  "username": "Test API",
  "password": "Test@2025",
  "phone": "24999999999",
  "email": "testeapi@test.com",
  "status": 1,
  "role": 3
}
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "id": "fa8d39b9-102f-4d9f-a6fb-2a9867cbfc25",
    "name": "",
    "email": "",
    "phone": "",
    "role": 0,
    "status": 0
  },
  "success": true,
  "message": "User created successfully",
  "errors": []
}
```
---

### 🔍 GET `/api/Users/{id}`

Busca usuário por ID.

#### 🧾 Exemplo de Requisição

```
Informar ID do usuário que queira buscar.
```
#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "data": {
      "id": "fa8d39b9-102f-4d9f-a6fb-2a9867cbfc25",
      "name": "",
      "email": "testeapi@test.com",
      "phone": "24999999999",
      "role": 3,
      "status": 1
    },
    "success": true,
    "message": "User retrieved successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```
---

### ❌ DELETE `/api/Users/{id}`

Deleta usuário.

#### 🧾 Exemplo de Requisição

```
Informar ID do usuário que queira deletar.
```
#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "success": true,
    "message": "User deleted successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```
---

## 🔐 AUTH

### ▶️ POST `/api/Auth`

API de autenticação do usuário.

#### 🧾 Exemplo de Requisição

```json
{
  "email": "gab.oliveiravr@gmail.com",
  "password": "Gabriel@2025"
}
```

#### ✅ Exemplo de Resposta

```json
{
  "data": {
    "data": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDk2YmRjMi1lM2I2LTQzNjgtYjg0Yi02NDJhYjU4MTBjZWMiLCJ1bmlxdWVfbmFtZSI6IkdhYnJpZWwgTmFzY2ltZW50byIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTc0NDE1Mzg2MywiZXhwIjoxNzQ0MTgyNjYzLCJpYXQiOjE3NDQxNTM4NjN9.vzEUKGeUPGynZ44VF5hSvefTUg7YnRT8xBr9BwCvlhM",
      "email": "gab.oliveiravr@gmail.com",
      "name": "Gabriel Nascimento",
      "role": "Admin"
    },
    "success": true,
    "message": "User authenticated successfully",
    "errors": []
  },
  "success": true,
  "message": "",
  "errors": []
}
```

Após criar seu usuário, você deverá se autenticar por essa API informando **email** e **senha**.  
A API irá gerar um **token JWT**, que você deve colar no botão **Authorize** do Swagger.  
Com o token ativo, você estará apto a realizar um pedido autenticado.
