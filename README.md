# 🏆 Meus Hábitos — Sistema de Controle de Hábitos

> Sistema simples e bonito para acompanhar seus hábitos diários.
> Feito com C# + .NET 8, SQLite e Bootstrap 5.

---

## 🖥️ Tecnologias

| Tecnologia | Uso |
|---|---|
| ASP.NET Core Web API | Backend / API REST |
| ASP.NET Core MVC | Frontend |
| Entity Framework Core | ORM / acesso ao banco |
| SQLite | Banco de dados leve |
| Bootstrap 5 (CDN) | Estilização responsiva |

---

## ⚡ Como rodar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)

### Instalação
```bash
# Clone o repositório
git clone https://github.com/SEU-USUARIO/habitos-dotnet.git
cd habitos-dotnet

# Entre na API e instale os pacotes
cd HabitosAPI
dotnet restore

# Crie o banco de dados
dotnet ef migrations add CriacaoInicial
dotnet ef database update

# Rode a API (Terminal 1)
dotnet run
```
```bash
# Em outro terminal, rode o frontend
cd HabitosFront
dotnet restore
dotnet run
```

Acesse: **http://localhost:5100** 🎉

---

## 📁 Estrutura
```
habitos/
├── HabitosAPI/      ← API REST (backend)
│   ├── Controllers/ ← Rotas HTTP
│   ├── Data/        ← DbContext (banco)
│   └── Models/      ← Entidades
└── HabitosFront/    ← Frontend MVC
    ├── Controllers/ ← Lógica das páginas
    ├── Models/      ← ViewModels
    └── Views/       ← HTML (Razor)
```

---

## ✅ Funcionalidades

- ✅ Criar, editar e excluir hábitos
- ✅ Marcar como concluído com um clique
- ✅ Filtrar por categoria e status
- ✅ Barra de progresso diário
- ✅ Layout responsivo e bonito
- ✅ Mensagens de sucesso e erro

---

## 📚 Feito para aprender

Este projeto foi criado com foco didático para aprender:
- API REST com ASP.NET Core
- Entity Framework Core + SQLite
- Padrão MVC
- Consumo de API no frontend
- CRUD completo

---

*Feito com ❤️ e C#*