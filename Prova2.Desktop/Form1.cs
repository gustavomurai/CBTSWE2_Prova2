
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Prova2.Desktop;

public partial class Form1 : Form
{
    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("https://localhost:7222/")
    };

    private readonly Color _azulEscuro = Color.FromArgb(23, 37, 84);
    private readonly Color _azul = Color.FromArgb(37, 99, 235);
    private readonly Color _fundo = Color.FromArgb(243, 245, 249);
    private readonly Color _texto = Color.FromArgb(38, 50, 71);
    private readonly Color _cinza = Color.FromArgb(100, 116, 139);

    private readonly Panel _telaLogin = new();
    private readonly Panel _telaUsuarios = new();

    private TextBox _txtLoginNome = new();
    private TextBox _txtLoginSenha = new();
    private Label _lblLoginMensagem = new();

    private TextBox _txtId = new();
    private TextBox _txtNome = new();
    private TextBox _txtSenha = new();
    private CheckBox _chkStatus = new();
    private DataGridView _gridUsuarios = new();
    private Label _lblUsuarioLogado = new();

    private string? _token;
    private string _nomeUsuarioLogado = string.Empty;

    public Form1()
    {
        InitializeComponent();

        Text = "Prova2 - Gerenciamento de Usuários";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 680);
        Size = new Size(1180, 780);
        Font = new Font("Segoe UI", 9F);
        BackColor = _fundo;

        MontarTelaLogin();
        MontarTelaUsuarios();

        Controls.Add(_telaUsuarios);
        Controls.Add(_telaLogin);

        _telaLogin.BringToFront();

        FormClosed += (_, _) => _http.Dispose();
    }

    // =========================================================
    // TELA DE LOGIN
    // =========================================================

    private void MontarTelaLogin()
    {
        _telaLogin.Dock = DockStyle.Fill;
        _telaLogin.BackColor = _fundo;

        Panel cartao = new()
        {
            Width = 450,
            Height = 520,
            BackColor = Color.White,
            Padding = new Padding(30)
        };

        void CentralizarCartao()
        {
            cartao.Left = Math.Max(
                0,
                (_telaLogin.ClientSize.Width - cartao.Width) / 2);

            cartao.Top = Math.Max(
                0,
                (_telaLogin.ClientSize.Height - cartao.Height) / 2);
        }

        _telaLogin.Controls.Add(cartao);
        _telaLogin.Resize += (_, _) => CentralizarCartao();
        CentralizarCartao();

        Label lblTitulo = new()
        {
            Text = "Prova2",
            Font = new Font("Segoe UI", 25F, FontStyle.Bold),
            ForeColor = _azul,
            TextAlign = ContentAlignment.MiddleCenter,
            Left = 30,
            Top = 30,
            Width = 370,
            Height = 55
        };

        Label lblSubtitulo = new()
        {
            Text = "Gerenciamento de Usuários",
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = _texto,
            TextAlign = ContentAlignment.MiddleCenter,
            Left = 30,
            Top = 90,
            Width = 370,
            Height = 38
        };

        Label lblOrientacao = new()
        {
            Text = "Entre com uma conta ativa para acessar o painel.",
            Font = new Font("Segoe UI", 9F),
            ForeColor = _cinza,
            TextAlign = ContentAlignment.MiddleCenter,
            Left = 30,
            Top = 132,
            Width = 370,
            Height = 25
        };

        Label lblNome = new()
        {
            Text = "Nome de usuário",
            Left = 30,
            Top = 175,
            Width = 370,
            Height = 22,
            ForeColor = _texto,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        _txtLoginNome = new TextBox
        {
            Left = 30,
            Top = 200,
            Width = 370,
            Height = 34,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle
        };

        Label lblSenha = new()
        {
            Text = "Senha",
            Left = 30,
            Top = 250,
            Width = 370,
            Height = 22,
            ForeColor = _texto,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        _txtLoginSenha = new TextBox
        {
            Left = 30,
            Top = 275,
            Width = 370,
            Height = 34,
            Font = new Font("Segoe UI", 11F),
            BorderStyle = BorderStyle.FixedSingle,
            UseSystemPasswordChar = true
        };

        Button btnEntrar = CriarBotao(
            "Entrar",
            _azul,
            Color.White,
            370,
            42);

        btnEntrar.Left = 30;
        btnEntrar.Top = 335;
        btnEntrar.Click += async (_, _) => await LoginAsync();

        Button btnPrimeiroUsuario = CriarBotao(
            "Cadastrar primeiro usuário",
            Color.FromArgb(226, 232, 240),
            _texto,
            370,
            40);

        btnPrimeiroUsuario.Left = 30;
        btnPrimeiroUsuario.Top = 385;
        btnPrimeiroUsuario.Click += async (_, _) =>
            await CadastrarPrimeiroUsuarioAsync();

        _lblLoginMensagem = new Label
        {
            Left = 30,
            Top = 435,
            Width = 370,
            Height = 45,
            ForeColor = Color.FromArgb(185, 28, 28),
            Font = new Font("Segoe UI", 8.5F),
            TextAlign = ContentAlignment.MiddleCenter
        };

        cartao.Controls.Add(lblTitulo);
        cartao.Controls.Add(lblSubtitulo);
        cartao.Controls.Add(lblOrientacao);
        cartao.Controls.Add(lblNome);
        cartao.Controls.Add(_txtLoginNome);
        cartao.Controls.Add(lblSenha);
        cartao.Controls.Add(_txtLoginSenha);
        cartao.Controls.Add(btnEntrar);
        cartao.Controls.Add(btnPrimeiroUsuario);
        cartao.Controls.Add(_lblLoginMensagem);

        _txtLoginNome.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoginAsync();
            }
        };

        _txtLoginSenha.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoginAsync();
            }
        };
    }

    private async Task LoginAsync()
    {
        string nome = _txtLoginNome.Text.Trim();
        string senha = _txtLoginSenha.Text;

        if (string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(senha))
        {
            _lblLoginMensagem.Text =
                "Informe o nome de usuário e a senha.";

            return;
        }

        try
        {
            _lblLoginMensagem.Text = "Conectando à API...";

            using var resposta = await _http.PostAsJsonAsync(
                "api/usuarios/login",
                new
                {
                    nome,
                    senha
                });

            string conteudo =
                await resposta.Content.ReadAsStringAsync();

            if (!resposta.IsSuccessStatusCode)
            {
                _lblLoginMensagem.Text =
                    ExtrairMensagem(conteudo,
                        "Usuário ou senha inválidos.");

                return;
            }

            using JsonDocument documento =
                JsonDocument.Parse(conteudo);

            JsonElement raiz = documento.RootElement;

            if (!raiz.TryGetProperty("token", out JsonElement tokenJson))
            {
                _lblLoginMensagem.Text =
                    "A API não retornou o token JWT.";

                return;
            }

            _token = tokenJson.GetString();

            if (string.IsNullOrWhiteSpace(_token))
            {
                _lblLoginMensagem.Text =
                    "O token JWT retornado está vazio.";

                return;
            }

            _nomeUsuarioLogado = nome;

            if (raiz.TryGetProperty("usuario", out JsonElement usuarioJson) &&
                usuarioJson.TryGetProperty("nome", out JsonElement nomeJson))
            {
                _nomeUsuarioLogado = nomeJson.GetString() ?? nome;
            }

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            _lblLoginMensagem.Text = string.Empty;
            _txtLoginSenha.Clear();

            _lblUsuarioLogado.Text =
                $"Conectado como: {_nomeUsuarioLogado}";

            _telaUsuarios.BringToFront();

            await ListarAsync();
        }
        catch (HttpRequestException)
        {
            _lblLoginMensagem.Text =
                "Não foi possível conectar à API. Verifique se o projeto Prova2.Api está em execução.";
        }
        catch (Exception ex)
        {
            _lblLoginMensagem.Text =
                "Erro ao realizar login: " + ex.Message;
        }
    }

    // O primeiro cadastro só é aceito pela API quando ainda não existe usuário.
    private async Task CadastrarPrimeiroUsuarioAsync()
    {
        string nome = _txtLoginNome.Text.Trim();
        string senha = _txtLoginSenha.Text;

        if (nome.Length < 2)
        {
            _lblLoginMensagem.Text =
                "O nome deve ter pelo menos 2 caracteres.";

            return;
        }

        if (senha.Length < 8)
        {
            _lblLoginMensagem.Text =
                "A senha deve ter pelo menos 8 caracteres.";

            return;
        }

        try
        {
            using var resposta = await _http.PostAsJsonAsync(
                "api/usuarios",
                new
                {
                    nome,
                    senha,
                    status = true
                });

            string conteudo =
                await resposta.Content.ReadAsStringAsync();

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                _lblLoginMensagem.Text =
                    "Já existe usuário cadastrado. Entre com uma conta existente.";

                return;
            }

            if (!resposta.IsSuccessStatusCode)
            {
                _lblLoginMensagem.Text =
                    ExtrairMensagem(conteudo,
                        $"Não foi possível cadastrar o usuário. HTTP {(int)resposta.StatusCode}.");

                return;
            }

            MessageBox.Show(
                "Primeiro usuário cadastrado com sucesso. Agora faça login.",
                "Cadastro concluído",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            await LoginAsync();
        }
        catch (HttpRequestException)
        {
            _lblLoginMensagem.Text =
                "Não foi possível conectar à API. Verifique se o projeto Prova2.Api está em execução.";
        }
        catch (Exception ex)
        {
            _lblLoginMensagem.Text =
                "Erro ao cadastrar o primeiro usuário: " + ex.Message;
        }
    }

    // =========================================================
    // TELA DE GERENCIAMENTO
    // =========================================================

    private void MontarTelaUsuarios()
    {
        _telaUsuarios.Dock = DockStyle.Fill;
        _telaUsuarios.BackColor = _fundo;

        Panel cabecalho = new()
        {
            Dock = DockStyle.Top,
            Height = 76,
            BackColor = _azulEscuro
        };

        Label lblTitulo = new()
        {
            Text = "Gerenciamento de Usuários",
            Left = 25,
            Top = 20,
            Width = 430,
            Height = 35,
            Font = new Font("Segoe UI", 17F, FontStyle.Bold),
            ForeColor = Color.White
        };

        _lblUsuarioLogado = new Label
        {
            Text = "Conectado",
            AutoSize = false,
            Width = 300,
            Height = 28,
            Top = 25,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F)
        };

        Button btnSair = CriarBotao(
            "Sair",
            Color.FromArgb(220, 38, 38),
            Color.White,
            85,
            34);

        btnSair.Top = 21;
        btnSair.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSair.Click += (_, _) => Desconectar(string.Empty);

        cabecalho.Controls.Add(lblTitulo);
        cabecalho.Controls.Add(_lblUsuarioLogado);
        cabecalho.Controls.Add(btnSair);

        cabecalho.Resize += (_, _) =>
        {
            btnSair.Left = cabecalho.ClientSize.Width - btnSair.Width - 22;
            _lblUsuarioLogado.Left =
                btnSair.Left - _lblUsuarioLogado.Width - 18;
        };

        TableLayoutPanel principal = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 2,
            RowCount = 1,
            BackColor = _fundo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 350));

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        Panel cartaoFormulario = new()
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18),
            Margin = new Padding(8)
        };

        Panel cartaoTabela = new()
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(16),
            Margin = new Padding(8)
        };

        principal.Controls.Add(cartaoFormulario, 0, 0);
        principal.Controls.Add(cartaoTabela, 1, 0);

        MontarFormularioUsuario(cartaoFormulario);
        MontarTabelaUsuarios(cartaoTabela);

        TableLayoutPanel layoutPrincipal = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = _fundo
        };

        layoutPrincipal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        layoutPrincipal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 76));

        layoutPrincipal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        cabecalho.Dock = DockStyle.Fill;

        layoutPrincipal.Controls.Add(cabecalho, 0, 0);
        layoutPrincipal.Controls.Add(principal, 0, 1);

        _telaUsuarios.Controls.Add(layoutPrincipal);
    }

    private void MontarFormularioUsuario(Panel painel)
    {
        Label lblTitulo = new()
        {
            Text = "Dados do usuário",
            Left = 18,
            Top = 15,
            Width = 290,
            Height = 30,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = _azulEscuro
        };

        _txtId = AdicionarCampo(
            painel,
            "ID do usuário",
            58);

        _txtNome = AdicionarCampo(
            painel,
            "Nome",
            125);

        _txtSenha = AdicionarCampo(
            painel,
            "Senha (mínimo 8 caracteres)",
            192);

        _txtSenha.UseSystemPasswordChar = true;

        _chkStatus = new CheckBox
        {
            Text = "Usuário ativo",
            Left = 20,
            Top = 260,
            Width = 250,
            Height = 28,
            Checked = true,
            ForeColor = _texto,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        FlowLayoutPanel botoes = new()
        {
            Left = 18,
            Top = 305,
            Width = 300,
            Height = 155,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true
        };

        Button btnCadastrar = CriarBotao(
            "Cadastrar",
            _azul,
            Color.White,
            130,
            38);

        Button btnAtualizar = CriarBotao(
            "Atualizar",
            Color.FromArgb(15, 118, 110),
            Color.White,
            130,
            38);

        Button btnExcluir = CriarBotao(
            "Excluir",
            Color.FromArgb(220, 38, 38),
            Color.White,
            130,
            38);

        Button btnBuscar = CriarBotao(
            "Buscar ID",
            Color.FromArgb(226, 232, 240),
            _texto,
            130,
            38);

        Button btnLimpar = CriarBotao(
            "Limpar campos",
            Color.FromArgb(226, 232, 240),
            _texto,
            270,
            38);

        btnCadastrar.Click += async (_, _) =>
            await CadastrarAsync();

        btnAtualizar.Click += async (_, _) =>
            await AtualizarAsync();

        btnExcluir.Click += async (_, _) =>
            await ExcluirAsync();

        btnBuscar.Click += async (_, _) =>
            await BuscarAsync();

        btnLimpar.Click += (_, _) =>
            LimparCampos();

        botoes.Controls.Add(btnCadastrar);
        botoes.Controls.Add(btnAtualizar);
        botoes.Controls.Add(btnExcluir);
        botoes.Controls.Add(btnBuscar);
        botoes.Controls.Add(btnLimpar);

        Label lblObservacao = new()
        {
            Text = "Para manter a senha atual durante a edição, deixe o campo de senha vazio.",
            Left = 20,
            Top = 470,
            Width = 285,
            Height = 45,
            ForeColor = _cinza,
            Font = new Font("Segoe UI", 8F)
        };

        painel.Controls.Add(lblTitulo);
        painel.Controls.Add(_txtId);
        painel.Controls.Add(_txtNome);
        painel.Controls.Add(_txtSenha);
        painel.Controls.Add(_chkStatus);
        painel.Controls.Add(botoes);
        painel.Controls.Add(lblObservacao);
    }

    private TextBox AdicionarCampo(
        Panel painel,
        string titulo,
        int topo)
    {
        Label label = new()
        {
            Text = titulo,
            Left = 20,
            Top = topo,
            Width = 285,
            Height = 22,
            ForeColor = _texto,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        TextBox campo = new()
        {
            Left = 20,
            Top = topo + 25,
            Width = 285,
            Height = 32,
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        painel.Controls.Add(label);
        painel.Controls.Add(campo);

        return campo;
    }

    private void MontarTabelaUsuarios(Panel painel)
    {
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.White
        };

        layout.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        layout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 48));

        layout.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        Panel topo = new()
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        Label lblTitulo = new()
        {
            Text = "Usuários cadastrados",
            Left = 4,
            Top = 8,
            Width = 300,
            Height = 30,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = _azulEscuro
        };

        Button btnAtualizarLista = CriarBotao(
            "Atualizar lista",
            Color.FromArgb(226, 232, 240),
            _texto,
            125,
            32);

        btnAtualizarLista.Top = 5;
        btnAtualizarLista.Anchor =
            AnchorStyles.Top | AnchorStyles.Right;

        btnAtualizarLista.Click += async (_, _) =>
            await ListarAsync();

        topo.Controls.Add(lblTitulo);
        topo.Controls.Add(btnAtualizarLista);

        topo.Resize += (_, _) =>
        {
            btnAtualizarLista.Left =
                topo.ClientSize.Width - btnAtualizarLista.Width - 4;
        };

        _gridUsuarios = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            GridColor = Color.FromArgb(226, 232, 240),
            EnableHeadersVisualStyles = false,
            Font = new Font("Segoe UI", 9F)
        };

        _gridUsuarios.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(241, 245, 249);

        _gridUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = _texto;
        _gridUsuarios.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 9F, FontStyle.Bold);

        _gridUsuarios.ColumnHeadersHeight = 42;
        _gridUsuarios.RowTemplate.Height = 38;
        _gridUsuarios.DefaultCellStyle.SelectionBackColor =
            Color.FromArgb(219, 234, 254);

        _gridUsuarios.DefaultCellStyle.SelectionForeColor = _texto;

        _gridUsuarios.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "Id",
                HeaderText = "ID",
                FillWeight = 18
            });

        _gridUsuarios.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "Nome",
                DataPropertyName = "Nome",
                HeaderText = "Nome",
                FillWeight = 60
            });

        _gridUsuarios.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "Status",
                DataPropertyName = "Status",
                HeaderText = "Status",
                FillWeight = 22
            });

        _gridUsuarios.CellFormatting += (_, e) =>
        {
            if (_gridUsuarios.Columns[e.ColumnIndex].Name == "Status" &&
                e.Value is bool ativo)
            {
                e.Value = ativo ? "Ativo" : "Inativo";
                e.FormattingApplied = true;
            }
        };

        _gridUsuarios.SelectionChanged += (_, _) =>
        {
            if (_gridUsuarios.CurrentRow?.DataBoundItem
                is UsuarioDto usuario)
            {
                PreencherCampos(usuario);
            }
        };

        layout.Controls.Add(topo, 0, 0);
        layout.Controls.Add(_gridUsuarios, 0, 1);

        painel.Controls.Add(layout);
    }

    // =========================================================
    // OPERAÇÕES DE USUÁRIOS
    // =========================================================

    private async Task ListarAsync()
    {
        try
        {
            using var resposta =
                await _http.GetAsync("api/usuarios");

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                Desconectar(
                    "Sua sessão expirou ou o usuário foi desativado. Entre novamente.");

                return;
            }

            if (!resposta.IsSuccessStatusCode)
            {
                await ExibirErroApiAsync(
                    resposta,
                    "Não foi possível listar os usuários.");

                return;
            }

            var usuarios =
                await resposta.Content.ReadFromJsonAsync<List<UsuarioDto>>();

            _gridUsuarios.DataSource = null;
            _gridUsuarios.DataSource = usuarios ?? new List<UsuarioDto>();

            if (_gridUsuarios.Rows.Count > 0)
            {
                _gridUsuarios.ClearSelection();
                _gridUsuarios.Rows[0].Selected = true;
            }
            else
            {
                LimparCampos();
            }
        }
        catch (HttpRequestException)
        {
            MessageBox.Show(
                "Não foi possível conectar à API. Verifique se o projeto Prova2.Api está em execução.",
                "Erro de conexão",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Erro ao listar usuários: " + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async Task BuscarAsync()
    {
        if (!int.TryParse(_txtId.Text, out int id) || id <= 0)
        {
            MessageBox.Show(
                "Informe um ID válido.",
                "Atenção",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        try
        {
            using var resposta =
                await _http.GetAsync($"api/usuarios/{id}");

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                Desconectar(
                    "Sua sessão expirou ou o usuário foi desativado. Entre novamente.");

                return;
            }

            if (resposta.StatusCode == HttpStatusCode.NotFound)
            {
                MessageBox.Show(
                    "Não existe usuário com esse ID.",
                    "Usuário não encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (!resposta.IsSuccessStatusCode)
            {
                await ExibirErroApiAsync(
                    resposta,
                    "Não foi possível buscar o usuário.");

                return;
            }

            var usuario =
                await resposta.Content.ReadFromJsonAsync<UsuarioDto>();

            if (usuario != null)
                PreencherCampos(usuario);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Erro ao buscar usuário: " + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async Task CadastrarAsync()
    {
        string nome = _txtNome.Text.Trim();
        string senha = _txtSenha.Text;

        if (nome.Length < 2)
        {
            MessageBox.Show(
                "O nome deve ter pelo menos 2 caracteres.",
                "Validação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        if (senha.Length < 8)
        {
            MessageBox.Show(
                "A senha deve ter pelo menos 8 caracteres.",
                "Validação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        try
        {
            var dados = new
            {
                nome,
                senha,
                status = _chkStatus.Checked
            };

            using var resposta =
                await _http.PostAsJsonAsync("api/usuarios", dados);

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                Desconectar(
                    "Sua sessão expirou ou o usuário foi desativado. Entre novamente.");

                return;
            }

            if (!resposta.IsSuccessStatusCode)
            {
                await ExibirErroApiAsync(
                    resposta,
                    "Não foi possível cadastrar o usuário.");

                return;
            }

            MessageBox.Show(
                "Usuário cadastrado com sucesso.",
                "Cadastro concluído",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LimparCampos();
            await ListarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Erro ao cadastrar usuário: " + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async Task AtualizarAsync()
    {
        if (!int.TryParse(_txtId.Text, out int id) || id <= 0)
        {
            MessageBox.Show(
                "Selecione um usuário na tabela ou informe um ID válido.",
                "Validação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        string nome = _txtNome.Text.Trim();

        if (nome.Length < 2)
        {
            MessageBox.Show(
                "O nome deve ter pelo menos 2 caracteres.",
                "Validação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        if (!string.IsNullOrWhiteSpace(_txtSenha.Text) &&
            _txtSenha.Text.Length < 8)
        {
            MessageBox.Show(
                "A nova senha deve ter pelo menos 8 caracteres.",
                "Validação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        try
        {
            var dados = new
            {
                nome,
                novaSenha = string.IsNullOrWhiteSpace(_txtSenha.Text)
                    ? null
                    : _txtSenha.Text,
                status = _chkStatus.Checked
            };

            using var resposta = await _http.PutAsJsonAsync(
                $"api/usuarios/{id}",
                dados);

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                Desconectar(
                    "Sua sessão expirou ou o usuário foi desativado. Entre novamente.");

                return;
            }

            if (!resposta.IsSuccessStatusCode)
            {
                await ExibirErroApiAsync(
                    resposta,
                    "Não foi possível atualizar o usuário.");

                return;
            }

            MessageBox.Show(
                "Usuário atualizado com sucesso.",
                "Atualização concluída",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LimparCampos();
            await ListarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Erro ao atualizar usuário: " + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async Task ExcluirAsync()
    {
        if (!int.TryParse(_txtId.Text, out int id) || id <= 0)
        {
            MessageBox.Show(
                "Selecione um usuário na tabela ou informe um ID válido.",
                "Validação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        DialogResult confirmacao = MessageBox.Show(
            $"Deseja realmente excluir o usuário de ID {id}?",
            "Confirmar exclusão",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacao != DialogResult.Yes)
            return;

        try
        {
            using var resposta =
                await _http.DeleteAsync($"api/usuarios/{id}");

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            {
                Desconectar(
                    "Sua sessão expirou ou o usuário foi desativado. Entre novamente.");

                return;
            }

            if (!resposta.IsSuccessStatusCode)
            {
                await ExibirErroApiAsync(
                    resposta,
                    "Não foi possível excluir o usuário.");

                return;
            }

            MessageBox.Show(
                "Usuário excluído com sucesso.",
                "Exclusão concluída",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LimparCampos();
            await ListarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Erro ao excluir usuário: " + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // =========================================================
    // MÉTODOS AUXILIARES
    // =========================================================

    private async Task ExibirErroApiAsync(
        HttpResponseMessage resposta,
        string titulo)
    {
        string conteudo =
            await resposta.Content.ReadAsStringAsync();

        string mensagem = ExtrairMensagem(
            conteudo,
            "A API retornou uma resposta de erro.");

        MessageBox.Show(
            $"{titulo}\n\nHTTP {(int)resposta.StatusCode}\n{mensagem}",
            "Erro da API",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private static string ExtrairMensagem(
        string conteudo,
        string mensagemPadrao)
    {
        if (string.IsNullOrWhiteSpace(conteudo))
            return mensagemPadrao;

        try
        {
            using JsonDocument documento =
                JsonDocument.Parse(conteudo);

            JsonElement raiz = documento.RootElement;

            if (raiz.ValueKind == JsonValueKind.Object)
            {
                string[] propriedades =
                {
                    "mensagem",
                    "message",
                    "title",
                    "detail"
                };

                foreach (string propriedade in propriedades)
                {
                    if (raiz.TryGetProperty(
                        propriedade,
                        out JsonElement valor) &&
                        valor.ValueKind == JsonValueKind.String)
                    {
                        return valor.GetString() ?? mensagemPadrao;
                    }
                }
            }
        }
        catch
        {
            // Se a resposta não for JSON, o texto original será exibido.
        }

        return conteudo;
    }

    private void PreencherCampos(UsuarioDto usuario)
    {
        _txtId.Text = usuario.Id.ToString();
        _txtNome.Text = usuario.Nome;
        _txtSenha.Clear();
        _chkStatus.Checked = usuario.Status;
    }

    private void LimparCampos()
    {
        _txtId.Clear();
        _txtNome.Clear();
        _txtSenha.Clear();
        _chkStatus.Checked = true;

        if (_gridUsuarios.Rows.Count > 0)
            _gridUsuarios.ClearSelection();
    }

    private void Desconectar(string mensagem)
    {
        _token = null;
        _nomeUsuarioLogado = string.Empty;

        _http.DefaultRequestHeaders.Authorization = null;

        _txtLoginSenha.Clear();
        _lblLoginMensagem.Text = mensagem;

        _telaLogin.BringToFront();
    }

    private Button CriarBotao(
        string texto,
        Color fundo,
        Color corTexto,
        int largura,
        int altura)
    {
        Button botao = new()
        {
            Text = texto,
            Width = largura,
            Height = altura,
            BackColor = fundo,
            ForeColor = corTexto,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false,
            Margin = new Padding(4)
        };

        botao.FlatAppearance.BorderSize = 0;

        return botao;
    }
}

public class UsuarioDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public bool Status { get; set; }
}
