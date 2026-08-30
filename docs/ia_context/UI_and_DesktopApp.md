# Interface Desktop e Aplicação Client - UI_and_DesktopApp

## 🎯 Objetivo / Contexto

O projeto `IntegradorApi` é a camada de interface gráfica desktop desenvolvida em WinUI 3 (Windows App SDK). Ele fornece um painel administrativo para cadastro e gerenciamento das conexões de sincronização, acompanhamento em tempo real dos logs de execução, teste visual de conectividade e agendador em segundo plano integrado à bandeja do sistema (System Tray).

## 🧩 Arquivos e Componentes

- **`IntegradorApi.xaml` / `IntegradorApi.xaml.cs`**: Janela principal da aplicação.
- **`ViewModels/ConnectionStatusViewModel.cs`**: ViewModel que gerencia a exibição visual de cada conexão ativa (glifos de tecnologia, direção e cor do status).
- **`Services/SettingsService.cs`**: Serviço responsável por carregar e persistir as configurações locais do banco e status de sincronização no `appsettings.json`.
- **`AppConfig.cs`**: Classe estática de acesso centralizado às configurações do `IConfiguration`.
- **`LogEventBus`**: Evento estático que transmite mensagens formatadas pelo Serilog diretamente para a interface gráfica.

## 🖥️ Abas da Interface e Recursos de Tela

### 1. Aba Sincronização (`TabViewItem`: "Sincronização")
- **Painel de Status de Conexões (`StatusConectionsItemsControl`)**:
  - Exibe ícones dinâmicos para cada conexão ativa.
  - As cores dos ícones indicam o estado:
    - **Verde**: Conexão testada e operacional.
    - **Amarelo**: Teste de conexão em andamento.
    - **Vermelho**: Falha na conexão.
  - Permite clicar em um ícone individual para retestar o status daquela conexão (`TestConnectionStatus`).
- **Console de Log em Tempo Real (`LogTextBlock`)**:
  - `TextBlock` estilizado com fonte *Consolas* dentro de um `ScrollViewer`.
  - Exibe as saídas do Serilog capturadas via `LogEventBus.OnLogReceived`.
- **Painel de Ações Laterais**:
  - **Botão "Testar Conexões" (`ButtonClick_ConnectionsTests`)**: Executa o teste em paralelo de todas as conexões ativas cadastradas.
  - **Botão "Iniciar / Pausar Sincronizações" (`BtnToggleSync`)**: Altera o estado do agendador automático e inicia/para o temporizador em segundo plano.
  - **Botão "Limpar Log" (`ButtonClick_ClearLog`)**: Limpa o conteúdo de texto do console.
  - **Botão "Minimizar na Bandeja" (`ButtonClick_MinimizeToTray`)**: Oculta a janela principal e ativa o ícone na barra de tarefas.

### 2. Aba Conexões (`TabViewItem`: "Conexões")
- **Formulário de Cadastro de Fontes/Destinos**:
  - `SourceDataComboBox`: Seleção do papel (`ORIGIN` / `DESTINATION`).
  - `SourceTypeComboBox`: Tipo da conexão (`REST API`, `MySQL`, `PostgreSQL`).
  - `SourceDescriptionTextBox`: Nome identificador amigável.
  - `SourceAddressTextBox`: URL base ou Endereço do Servidor com Porta.
  - `SourceUserTextBox` / `SourcePasswordTextBox`: Credenciais de acesso.
  - `SourceOptionalTextBox`: Campo dinâmico (Nome do banco de dados para MySQL/PostgreSQL ou Endpoint base para REST API).
  - `SourceIntegrationComboBox`: Módulo de integração associado (`Manga`, `Novel`, `ComicInfo`, `DeckSubtitle`, `TextoIngles`, `TextoJapones`).
  - `SourceDeleteCheckBox`: Define se os registros devem ser apagados da origem após o envio.
  - `SourceEnabledCheckBox`: Define se a conexão está ativa para execução no orquestrador.
- **Tabela DataGrid (`SourceDataGrid`)**:
  - Lista todas as conexões cadastradas no banco local (`AppDbContext`).
  - Permite duplo clique em um registro (`DoubleTapped_SourceDataGrid`) para carregar os dados no formulário e alternar para o modo de atualização.
- **Botoes de Ação do Formulário**:
  - **"Adicionar / Atualizar"**: Salva ou atualiza a conexão no banco local via `DatabaseService`.
  - **"Excluir"**: Remove a conexão selecionada.
  - **"Testar"**: Executa um teste de conexão temporário com os dados preenchidos nos campos antes de salvar.

### 3. Integração com a Bandeja do Sistema (System Tray)
- Implementado através do pacote `H.NotifyIcon.WinUI` (`MyTaskbarIcon`).
- Exibe o menu de contexto (`TrayMenuFlyout`):
  - **"Restaurar Janela"**: Restaura a exibição da janela principal (`_appWindow.Show()`).
  - **"Sair da Aplicação"**: Encerra a aplicação (`Application.Current.Exit()`).

## ⏱️ Temporizador e Agendamento Automático

- **Delays e Intervalos Configurados**:
  - Delay Inicial (`_initialSyncDelay`): **10 minutos** após a inicialização com sincronização ativa.
  - Intervalo de Recorrência (`_recurringSyncInterval`): **3 horas** entre execuções automáticas.
- **Controle de Execução**:
  - O estado do botão e do timer é mantido através do `SettingsService`.
  - Execuções manuais resetam a contagem do temporizador para evitar execuções simultâneas.
