# Sistema de Autenticação Bluube - Exemplo em C#

Aplicativos de exemplo em **C#** que demonstram a integração com o BluubeAuth: um projeto de **console** (menu no terminal) e outro **Windows Forms** (interface com Guna — login, registro e dados do usuário). O arquivo `BluubeAuth.cs` é o núcleo reutilizável do SDK em cada pasta.

## 📋 Sobre o Projeto

Este repositório traz duas amostras prontas:

- **Console** (`Console/`): menu interativo — login com usuário e senha, registro com chave de licença + usuário + senha, saída. Após sucesso, exibe `UserData`.
- **WinForms** (`Form/`): telas com Guna — Login e Registro; ao iniciar, chama `Initialize` e abre o fluxo; a tela **Home** mostra os dados do usuário após autenticação.

O SDK usa **`namespace Bluube.Auth`** e a classe **`BluubeAuth`**, como na documentação, evitando confundir o nome da classe com o namespace.

## 💓 O heartbeat: o que é, por que existe e o que acontece se não estiver certo

Depois de um **`Initialize()`** bem-sucedido, o SDK **inicia automaticamente** um pulso periódico (**heartbeat**) que chama o endpoint **`/heartbeat`** no servidor, enviando sessão, IP, versão e HWID quando aplicável.

**Por que isso importa**

- O servidor sabe que o cliente **continua vivo** e pode aplicar **políticas** (versão do app, bloqueios, HWID, VPN, etc.).
- Sessões **fantasma** ou **revogadas** são detectadas: a API pode responder com falha; em caso de mensagem **`Invalid session`** (inglês, conforme o contrato da API), o exemplo encerra o processo de forma controlada.
- Sem heartbeat bem implementado (ou se você **remover** o pulso e **não** substituir por nada), o servidor **não recebe** esses sinais: expiração por tempo, limpeza de sessão e regras de segurança **deixam de funcionar como o painel espera**. Na prática: o usuário pode parecer “logado” localmente enquanto o servidor já invalidou a sessão, ou o contrário — comportamento inconsistente e suporte mais difícil.

**O que “implementado direito” significa neste exemplo**

- Não chamar heartbeat **só depois** do login se a sua API já exige sessão válida na tela inicial — por isso o exemplo inicia o heartbeat **logo após** o `Initialize()`, inclusive **antes** do usuário logar.
- Manter o intervalo coerente com o servidor (aqui, por volta de **30 segundos** no código de exemplo).
- Em **depuração**: se você **pausar todo o processo** no depurador (“Break All”), timers e threads param — o heartbeat **não dispara** até continuar a execução. Isso é esperado; não indica bug em produção.

## 🛠️ Tecnologias Utilizadas

### Console (`Console/`)

- **.NET** (`net8.0`)
- **Newtonsoft.Json** — serialização JSON
- **Sodium.Core** — verificação Ed25519
- **HttpClient** com cookies (`UseCookies = true`)

### WinForms (`Form/`)

- **.NET** (`net8.0-windows`)
- **Guna.UI2.WinForms** — interface
- **Newtonsoft.Json** e **Sodium.Core**
- **HttpClient** com cookies

## 📦 Dependências

- **Console:** `Newtonsoft.Json`, `Sodium.Core` — arquivo `Console/Bluube-CSHARP-Example.csproj`
- **Form:** `Guna.UI2.WinForms`, `Newtonsoft.Json`, `Sodium.Core` — arquivo `Form/Bluube-CSHARP-Form.csproj`

## 🚀 Como Configurar

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) — este exemplo usa **`net8.0`**; costuma funcionar também com **.NET 6** ou **7** alterando o `TargetFramework` no `.csproj` e testando; **.NET Framework** legado exige adaptar projeto e referências
- Conta na Bluube com AppID, OwnerID e Version configurados
- Para o **Form**: ambiente Windows com **Windows Forms**

### Instalação

1. Clone ou baixe este repositório.
2. Abra a solution que for usar:
   - `Console/Bluube-CSHARP-Example.sln`
   - `Form/Bluube-CSHARP-Form.sln`
3. Configure suas credenciais BluubeAuth (use placeholders em repositórios públicos):

**Console** — `Console/Program.cs`:

```csharp
// APP_ID, OWNER_ID, VERSION
public static readonly BluubeAuth App = new("APP_ID", "OWNER_ID", "1.0");
```

**Form** — `Form/Program.cs`:

```csharp
using Bluube.Auth;
public static readonly BluubeAuth App = new("APP_ID", "OWNER_ID", "1.0");
```

4. Compile e execute (na pasta `Bluube-CSHARP-Example`):

```bash
dotnet build Console/Bluube-CSHARP-Example.sln
dotnet build Form/Bluube-CSHARP-Form.sln
```

```bash
dotnet run --project Console/Bluube-CSHARP-Example.csproj
dotnet run --project Form/Bluube-CSHARP-Form.csproj
```

## 📁 Estrutura do Projeto

```
Bluube-CSHARP-Example/
├── README.md                          # Documentação
├── Console/
│   ├── Bluube-CSHARP-Example.sln
│   ├── Bluube-CSHARP-Example.csproj
│   ├── Program.cs                     # Menu e UserData
│   ├── BluubeAuth.cs                  # SDK (Initialize, Login, Register, heartbeat)
│   └── Properties/
│       └── AssemblyInfo.cs
└── Form/
    ├── Bluube-CSHARP-Form.sln
    ├── Bluube-CSHARP-Form.csproj
    ├── Program.cs                     # Initialize e Login
    ├── BluubeAuth.cs                  # Mesmo SDK
    ├── Login.cs / Login.Designer.cs
    ├── Login.resx
    ├── Home.cs / Home.Designer.cs
    └── Home.resx
```

## 🔑 Funcionalidades

### BluubeAuth.cs

A classe `BluubeAuth` concentra a comunicação segura com a API:

- **`Initialize()`**: registra o aplicativo no servidor, obtém a sessão e valida assinaturas; em seguida **inicia o heartbeat** automático (não é necessário iniciar de novo após login/registro neste exemplo).
- **`Login`** / **`Register`**: login e registro após o `Initialize()`.
- **`UserData`** (`JObject`) e **`LastMessage`**: feedback para a interface ou para o terminal.
- **Heartbeat**: ver a seção **O heartbeat** no início deste README; respostas com **`Invalid session`** encerram o processo com código 0, alinhado à API.

### Program.cs (Console)

Menu numerado, cabeçalho, limpeza de tela, pausa após cada ação; após login ou registro bem-sucedido, exibe username, IP, HWID e datas.

### Form — interface

Painéis **Login** e **Registro**; botões desabilitados durante operações assíncronas; **Home** com os dados do usuário (username, IP, HWID, datas).

### Características em comum

- **Mensagens** de sucesso ou erro via `LastMessage`.
- **Dados do usuário** após login ou registro bem-sucedido em `UserData`.

## 🔒 Segurança

- **Assinatura Ed25519 em respostas**: o SDK verifica `X-Bluube-Signature` e `X-Bluube-Timestamp`; respostas forjadas sem a chave privada do servidor são rejeitadas.
- **Pinning de chave pública**: a chave esperada está fixa no código do exemplo.
- **Anti-replay** por janela de tempo no timestamp assinado.
- **HWID no Windows** (SID do usuário, com fallback para `MachineGuid` quando aplicável).
- **Heartbeat**: mantém a sessão alinhada às regras do servidor; ver seção dedicada acima.

## ⚙️ Configuração da API

A API BluubeAuth está configurada para usar o endpoint:

```
https://api.bluube.com
```

Certifique-se de que o endpoint está acessível e que AppID, OwnerID e Version estão corretos no painel Bluube.

## 🐛 Tratamento de Erros e Segurança

Violações de integridade (assinatura inválida, resposta adulterada, etc.) disparam encerramento do processo (`Terminate` / `Environment.Exit`), em vez de seguir com estado inconsistente.

## ⚠️ Avisos

- **Nunca compartilhe** AppID e OwnerID publicamente.
- **Use variáveis de ambiente** ou cofres de segredos em produção.
- **Mantenha dependências atualizadas** (NuGet).
- **Ofuscação / integridade** do binário são responsabilidade do seu pipeline de release, se precisar.

## 📞 Suporte

Para questões sobre a Bluube, consulte a documentação oficial ou entre em contato com o suporte pelo discord.

---

**Nota:** Este é um projeto de exemplo educacional. Adapte credenciais, interface, mensagens e tratamento de erros ao seu caso de uso.
