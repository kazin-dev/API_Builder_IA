# 🧠 API Builder IA  
Gere APIs completas a partir de descrições em linguagem natural — usando Inteligência Artificial.

---

## 🚀 Visão Geral
Este projeto transforma *briefs* (descrições textuais) em artefatos reais de API — como especificações **OpenAPI**, código base e documentação.  
Implementado em **.NET 8** com arquitetura **Clean Architecture**.

---

## 🏗️ Arquitetura

| Camada | Função | Depende de |
|---------|--------|------------|
| **ApiBuilder.Api** | Interface HTTP — endpoints, Swagger, DI, Serilog. | Application, Shared, Infrastructure |
| **ApiBuilder.Application** | Casos de uso e regras de negócio. | Domain, Shared |
| **ApiBuilder.Domain** | Entidades e lógica pura do domínio. | — |
| **ApiBuilder.Infrastructure** | Banco de dados, repositórios e serviços externos (ex: OpenAI). | Application, Domain, Shared |
| **ApiBuilder.Shared** | DTOs, validadores e contratos compartilhados. | — |

---

## 🧩 Tecnologias
- **.NET 8 LTS**
- **Entity Framework Core 8**
- **SQL Server (LocalDB)**
- **Serilog + Swagger**
- **Clean Architecture**
- **Scriban (templates IA)**

---

## ⚙️ Setup Rápido

```bash
# Clonar
git clone https://github.com/kazin-dev/API_Builder_IA.git
cd API_Builder_IA

# Restaurar e compilar
dotnet restore
dotnet build

# Rodar a API
dotnet run --project src/ApiBuilder.Api
