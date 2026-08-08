using System.ComponentModel;
using System.Net;
using SewingWorkshop.WinForms.Api;
using SewingWorkshop.WinForms.Mapping;
using SewingWorkshop.WinForms.UI;

namespace SewingWorkshop.WinForms
{
    public sealed class MainForm : Form
    {
        private const int PageSize = 20;

        private readonly ApiClient _api;
        private readonly Dictionary<string, Button> _navButtons = new();
        private readonly System.Windows.Forms.Timer _clock = new();

        private Panel _content = new();
        private Label _title = new();
        private Label _subtitle = new();
        private Label _status = new();
        private Label _time = new();

        private int _employeePage = 1;
        private int _contragentPage = 1;
        private int _warehousePage = 1;
        private int _fillPage = 1;

        private TextBox? _employeeSearch;
        private ComboBox? _employeePosition;
        private ComboBox? _employeeRank;
        private DataGridView? _employeeGrid;
        private Label? _employeePager;

        private TextBox? _contragentSearch;
        private ComboBox? _contragentType;
        private DataGridView? _contragentGrid;
        private Label? _contragentPager;

        private TextBox? _warehouseSearch;
        private ComboBox? _warehouseContragentType;
        private ComboBox? _warehouseMaterialType;
        private DataGridView? _warehouseGrid;
        private Label? _warehousePager;

        private ComboBox? _fillType;
        private DataGridView? _fillGrid;
        private Label? _fillPager;
        private Label? _statTotal;
        private Label? _statToday;
        private Label? _statEmployees;
        private Label? _statContragents;
        private TextBox? _loginApiUrl;
        private TextBox? _loginUsername;
        private TextBox? _loginPassword;
        private CheckBox? _loginRegister;
        private Label? _loginMessage;

        public MainForm(ApiClient api)
        {
            _api = api;
            InitializeFrame();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            await ShowSectionAsync(_api.IsAuthenticated ? "fills" : "login");
        }

        private void InitializeFrame()
        {
            Text = "Производственный учет";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1100, 680);
            Size = new Size(1280, 780);
            BackColor = AppTheme.Background;
            Font = AppTheme.BaseFont;

            var sidebar = BuildSidebar();
            var main = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Background };
            var topbar = BuildTopbar();
            _content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(24)
            };

            main.Controls.Add(_content);
            main.Controls.Add(topbar);
            Controls.Add(main);
            Controls.Add(sidebar);

            _clock.Interval = 1000;
            _clock.Tick += (_, _) => _time.Text = DateTime.Now.ToString("HH:mm:ss");
            _clock.Start();
        }

        private Panel BuildSidebar()
        {
            var sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 72,
                BackColor = AppTheme.Text,
                Padding = new Padding(10, 16, 10, 16)
            };

            var logo = new Label
            {
                Text = "▦",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 18F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 52,
                Height = 40,
                Location = new Point(10, 10)
            };
            sidebar.Controls.Add(logo);

            var y = 64;
            AddNav(sidebar, "fills", "Заполн.", y);
            y += 56;
            AddNav(sidebar, "employees", "Сотруд.", y);
            y += 56;
            AddNav(sidebar, "contragents", "Контраг.", y);
            y += 56;
            AddNav(sidebar, "warehouse", "Склад", y);

            var settings = CreateNavButton("Настр.");
            settings.Location = new Point(10, Height - 96);
            settings.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            settings.Click += async (_, _) => await ShowSectionAsync("settings");
            sidebar.Controls.Add(settings);
            _navButtons["settings"] = settings;

            return sidebar;
        }

        private void AddNav(Panel sidebar, string key, string text, int y)
        {
            var button = CreateNavButton(text);
            button.Location = new Point(10, y);
            button.Click += async (_, _) => await ShowSectionAsync(key);
            sidebar.Controls.Add(button);
            _navButtons[key] = button;
        }

        private static Button CreateNavButton(string text)
        {
            var button = new Button
            {
                Text = text,
                Width = 52,
                Height = 52,
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.Text,
                ForeColor = Color.FromArgb(175, 175, 175),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 45, 45);
            return button;
        }

        private Panel BuildTopbar()
        {
            var topbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = AppTheme.Background,
                Padding = new Padding(24, 0, 24, 0)
            };
            topbar.Paint += (_, e) =>
            {
                using var pen = new Pen(AppTheme.Border);
                e.Graphics.DrawLine(pen, 0, topbar.Height - 1, topbar.Width, topbar.Height - 1);
            };

            _title = new Label
            {
                AutoSize = true,
                Font = new Font(AppTheme.BaseFont, FontStyle.Bold),
                ForeColor = AppTheme.Text,
                Location = new Point(24, 16)
            };

            _subtitle = new Label
            {
                AutoSize = true,
                Font = AppTheme.SmallFont,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(136, 18)
            };

            _status = new Label
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = AppTheme.SmallFont,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(730, 18)
            };

            _time = new Label
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = AppTheme.MonoFont,
                ForeColor = AppTheme.TextFaint,
                Location = new Point(1030, 16)
            };

            topbar.Resize += (_, _) =>
            {
                _time.Location = new Point(topbar.Width - 110, 16);
                _status.Location = new Point(Math.Max(360, topbar.Width - 360), 18);
            };

            topbar.Controls.AddRange(new Control[] { _title, _subtitle, _status, _time });
            return topbar;
        }

        private async Task ShowSectionAsync(string key)
        {
            if (key != "settings" && key != "login" && !_api.IsAuthenticated)
            {
                key = "login";
            }

            foreach (var pair in _navButtons)
            {
                var active = pair.Key == key;
                pair.Value.BackColor = active ? Color.FromArgb(48, 48, 48) : AppTheme.Text;
                pair.Value.ForeColor = active ? Color.White : Color.FromArgb(175, 175, 175);
            }

            _content.Controls.Clear();

            switch (key)
            {
                case "employees":
                    _title.Text = "Сотрудники";
                    _subtitle.Text = "Список сотрудников предприятия";
                    BuildEmployeesSection();
                    await LoadEmployeesAsync();
                    break;
                case "contragents":
                    _title.Text = "Контрагенты";
                    _subtitle.Text = "Поставщики, заказчики и материалы";
                    BuildContragentsSection();
                    await LoadContragentsAsync();
                    break;
                case "warehouse":
                    _title.Text = "Склад";
                    _subtitle.Text = "Остатки материалов и операции";
                    BuildWarehouseSection();
                    await LoadWarehouseAsync();
                    break;
                case "login":
                    _title.Text = "Авторизация";
                    _subtitle.Text = "Вход в API и проверка PostgreSQL";
                    BuildLoginSection();
                    break;
                case "settings":
                    _title.Text = "Настройки";
                    _subtitle.Text = "API, авторизация и внешний вид";
                    BuildSettingsSection();
                    break;
                default:
                    _title.Text = "Заполнения";
                    _subtitle.Text = "История последних записей";
                    BuildFillsSection();
                    await LoadFillsAsync();
                    break;
            }

            RefreshStatus();
        }

        private void BuildFillsSection()
        {
            var root = ContentRoot();
            var stats = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 86,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            _statTotal = StatCard(stats, "0", "Всего записей");
            _statToday = StatCard(stats, "0", "Сегодня");
            _statEmployees = StatCard(stats, "0", "Сотрудников");
            _statContragents = StatCard(stats, "0", "Контрагентов");

            var toolbar = Toolbar();
            _fillType = AppTheme.ComboBox("Все типы", "Материал", "Сотрудник", "Контрагент", "Заказ", "Покупка");
            _fillType.Width = 170;
            _fillType.SelectedIndexChanged += async (_, _) =>
            {
                _fillPage = 1;
                await LoadFillsAsync();
            };
            toolbar.Controls.Add(_fillType);

            _fillGrid = AppTheme.Grid();
            AddTextColumn(_fillGrid, "Type", "Тип", 130);
            AddTextColumn(_fillGrid, "Description", "Описание", 320);
            AddTextColumn(_fillGrid, "Related", "Связано с", 220);
            AddTextColumn(_fillGrid, "CreatedAt", "Дата и время", 150);
            AddTextColumn(_fillGrid, "Status", "Статус", 120);

            var pager = Pager(out _fillPager, async () =>
            {
                if (_fillPage > 1)
                {
                    _fillPage--;
                    await LoadFillsAsync();
                }
            }, async () =>
            {
                _fillPage++;
                await LoadFillsAsync();
            });

            root.Controls.Add(_fillGrid);
            root.Controls.Add(pager);
            root.Controls.Add(toolbar);
            root.Controls.Add(stats);
            _content.Controls.Add(root);
        }

        private void BuildEmployeesSection()
        {
            var root = ContentRoot();
            var header = HeaderWithButton("Сотрудники", "Список сотрудников предприятия", "+ Добавить сотрудника", async () =>
            {
                using var dialog = new EmployeeDialog();
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    await ExecuteAsync(async () =>
                    {
                        await _api.CreateEmployeeAsync(dialog.ToCreateRequest());
                        _employeePage = 1;
                        await LoadEmployeesAsync();
                        await LoadFillStatsOnlyAsync();
                    });
                }
            });

            var toolbar = Toolbar();
            _employeeSearch = AppTheme.TextBox("Введите ФИО сотрудника");
            _employeeSearch.Width = 260;
            _employeeSearch.TextChanged += async (_, _) =>
            {
                _employeePage = 1;
                await LoadEmployeesAsync();
            };

            _employeePosition = AppTheme.ComboBox("Все должности", "Швея", "Закройщик", "Технолог", "Мастер", "Упаковщик");
            _employeePosition.Width = 170;
            _employeePosition.SelectedIndexChanged += async (_, _) =>
            {
                _employeePage = 1;
                await LoadEmployeesAsync();
            };

            _employeeRank = AppTheme.ComboBox("Все разряды", "0", "1", "2", "3", "4", "5", "6");
            _employeeRank.Width = 130;
            _employeeRank.SelectedIndexChanged += async (_, _) =>
            {
                _employeePage = 1;
                await LoadEmployeesAsync();
            };

            toolbar.Controls.AddRange(new Control[] { _employeeSearch, _employeePosition, _employeeRank });

            _employeeGrid = AppTheme.Grid();
            AddTextColumn(_employeeGrid, "Fio", "ФИО", 300);
            AddTextColumn(_employeeGrid, "Position", "Должность", 180);
            AddTextColumn(_employeeGrid, "Rank", "Разряд", 90);
            AddTextColumn(_employeeGrid, "CreatedAt", "Дата", 130);
            AddButtonColumn(_employeeGrid, "Edit", "Правка", "Изм.", 70);
            AddButtonColumn(_employeeGrid, "Delete", "Удалить", "Удалить", 80);
            _employeeGrid.CellContentClick += EmployeeGridCellContentClick;

            var pager = Pager(out _employeePager, async () =>
            {
                if (_employeePage > 1)
                {
                    _employeePage--;
                    await LoadEmployeesAsync();
                }
            }, async () =>
            {
                _employeePage++;
                await LoadEmployeesAsync();
            });

            root.Controls.Add(_employeeGrid);
            root.Controls.Add(pager);
            root.Controls.Add(toolbar);
            root.Controls.Add(header);
            _content.Controls.Add(root);
        }

        private void BuildContragentsSection()
        {
            var root = ContentRoot();
            var header = HeaderWithButton("Контрагенты", "Поставщики, заказчики и связанные материалы", "+ Добавить контрагента", async () =>
            {
                using var dialog = new ContragentDialog();
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    await ExecuteAsync(async () =>
                    {
                        await _api.CreateContragentAsync(dialog.ToCreateRequest());
                        _contragentPage = 1;
                        await LoadContragentsAsync();
                        await LoadFillStatsOnlyAsync();
                    });
                }
            });

            var toolbar = Toolbar();
            _contragentSearch = AppTheme.TextBox("Поиск по контрагентам");
            _contragentSearch.Width = 260;
            _contragentSearch.TextChanged += async (_, _) =>
            {
                _contragentPage = 1;
                await LoadContragentsAsync();
            };
            _contragentType = AppTheme.ComboBox("Все типы", "Поставщик", "Заказчик");
            _contragentType.Width = 160;
            _contragentType.SelectedIndexChanged += async (_, _) =>
            {
                _contragentPage = 1;
                await LoadContragentsAsync();
            };
            toolbar.Controls.AddRange(new Control[] { _contragentSearch, _contragentType });

            _contragentGrid = AppTheme.Grid();
            AddTextColumn(_contragentGrid, "Type", "Тип", 110);
            AddTextColumn(_contragentGrid, "Name", "Название", 260);
            AddTextColumn(_contragentGrid, "Contact", "Контакт", 180);
            AddTextColumn(_contragentGrid, "Phone", "Телефон", 140);
            AddTextColumn(_contragentGrid, "MaterialsCount", "Мат.", 70);
            AddTextColumn(_contragentGrid, "OrdersCount", "Заказ.", 80);
            AddTextColumn(_contragentGrid, "CreatedAt", "Дата", 110);
            AddButtonColumn(_contragentGrid, "Materials", "Материалы", "Материалы", 90);
            AddButtonColumn(_contragentGrid, "Edit", "Правка", "Изм.", 70);
            AddButtonColumn(_contragentGrid, "Delete", "Удалить", "Удалить", 80);
            _contragentGrid.CellContentClick += ContragentGridCellContentClick;
            _contragentGrid.CellDoubleClick += async (_, e) =>
            {
                if (e.RowIndex >= 0 && RowAt<ContragentRow>(_contragentGrid, e.RowIndex) is { } row)
                {
                    await OpenMaterialsAsync(row);
                }
            };

            var pager = Pager(out _contragentPager, async () =>
            {
                if (_contragentPage > 1)
                {
                    _contragentPage--;
                    await LoadContragentsAsync();
                }
            }, async () =>
            {
                _contragentPage++;
                await LoadContragentsAsync();
            });

            root.Controls.Add(_contragentGrid);
            root.Controls.Add(pager);
            root.Controls.Add(toolbar);
            root.Controls.Add(header);
            _content.Controls.Add(root);
        }

        private void BuildWarehouseSection()
        {
            var root = ContentRoot();
            var header = Header("Склад", "Остатки материалов, приход и списание");

            var toolbar = Toolbar();
            _warehouseSearch = AppTheme.TextBox("Поиск по складу");
            _warehouseSearch.Width = 260;
            _warehouseSearch.TextChanged += async (_, _) =>
            {
                _warehousePage = 1;
                await LoadWarehouseAsync();
            };
            _warehouseContragentType = AppTheme.ComboBox("Все контрагенты", "Поставщик", "Заказчик");
            _warehouseContragentType.Width = 170;
            _warehouseContragentType.SelectedIndexChanged += async (_, _) =>
            {
                _warehousePage = 1;
                await LoadWarehouseAsync();
            };
            _warehouseMaterialType = AppTheme.ComboBox("Все материалы", "Ткань", "Нитки");
            _warehouseMaterialType.Width = 160;
            _warehouseMaterialType.SelectedIndexChanged += async (_, _) =>
            {
                _warehousePage = 1;
                await LoadWarehouseAsync();
            };
            toolbar.Controls.AddRange(new Control[] { _warehouseSearch, _warehouseContragentType, _warehouseMaterialType });

            _warehouseGrid = AppTheme.Grid();
            AddTextColumn(_warehouseGrid, "ContragentName", "Контрагент", 220);
            AddTextColumn(_warehouseGrid, "MaterialType", "Тип", 110);
            AddTextColumn(_warehouseGrid, "MaterialName", "Материал", 260);
            AddTextColumn(_warehouseGrid, "DateAdded", "Дата", 110);
            AddTextColumn(_warehouseGrid, "Qty", "Кол-во", 100);
            AddTextColumn(_warehouseGrid, "Price", "Цена", 100);
            AddButtonColumn(_warehouseGrid, "Income", "Приход", "+", 60);
            AddButtonColumn(_warehouseGrid, "Deduct", "Списание", "-", 70);
            AddButtonColumn(_warehouseGrid, "Delete", "Удалить", "Удалить", 80);
            _warehouseGrid.CellContentClick += WarehouseGridCellContentClick;

            var pager = Pager(out _warehousePager, async () =>
            {
                if (_warehousePage > 1)
                {
                    _warehousePage--;
                    await LoadWarehouseAsync();
                }
            }, async () =>
            {
                _warehousePage++;
                await LoadWarehouseAsync();
            });

            root.Controls.Add(_warehouseGrid);
            root.Controls.Add(pager);
            root.Controls.Add(toolbar);
            root.Controls.Add(header);
            _content.Controls.Add(root);
        }

        private void BuildLoginSection()
        {
            var root = ContentRoot();
            var header = Header("Авторизация", "Вход в API и проверка подключения к PostgreSQL");
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 330,
                BackColor = AppTheme.Background,
                Padding = new Padding(0, 10, 0, 0)
            };

            _loginApiUrl = AppTheme.TextBox();
            _loginApiUrl.Text = _api.BaseUrl;
            _loginApiUrl.Location = new Point(170, 16);
            _loginApiUrl.Width = 340;

            _loginUsername = AppTheme.TextBox("Введите логин");
            _loginUsername.Location = new Point(170, 62);
            _loginUsername.Width = 260;

            _loginPassword = AppTheme.TextBox("Введите пароль");
            _loginPassword.Location = new Point(170, 108);
            _loginPassword.Width = 260;
            _loginPassword.UseSystemPasswordChar = true;

            _loginRegister = new CheckBox
            {
                Text = "Создать пользователя",
                AutoSize = true,
                Font = AppTheme.BaseFont,
                BackColor = AppTheme.Background,
                ForeColor = AppTheme.Text,
                Location = new Point(170, 152)
            };

            var loginButton = AppTheme.Button("Войти", true);
            loginButton.Location = new Point(170, 190);
            loginButton.Width = 120;
            loginButton.Click += async (_, _) => await SignInFromMainWindowAsync();

            var databaseButton = AppTheme.Button("Проверить PostgreSQL");
            databaseButton.Location = new Point(300, 190);
            databaseButton.Width = 170;
            databaseButton.Click += async (_, _) => await CheckDatabaseFromMainWindowAsync();

            var settingsButton = AppTheme.Button("Настройки");
            settingsButton.Location = new Point(480, 190);
            settingsButton.Width = 110;
            settingsButton.Click += async (_, _) => await ShowSectionAsync("settings");

            _loginMessage = AppTheme.Label("Сначала запустите API, затем войдите или создайте первого пользователя.");
            _loginMessage.Location = new Point(170, 238);
            _loginMessage.MaximumSize = new Size(640, 0);

            panel.Controls.AddRange(new Control[]
            {
                FieldLabel("API URL", 20),
                _loginApiUrl,
                FieldLabel("Логин", 66),
                _loginUsername,
                FieldLabel("Пароль", 112),
                _loginPassword,
                _loginRegister,
                loginButton,
                databaseButton,
                settingsButton,
                _loginMessage
            });

            root.Controls.Add(panel);
            root.Controls.Add(header);
            _content.Controls.Add(root);
        }

        private void BuildSettingsSection()
        {
            var root = ContentRoot();
            var header = Header("Настройки", "Подключение к API и параметры интерфейса");
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = AppTheme.Background,
                Padding = new Padding(0, 10, 0, 0)
            };

            var apiLabel = AppTheme.Label("API URL");
            apiLabel.Location = new Point(0, 20);
            var apiUrl = AppTheme.TextBox();
            apiUrl.Text = _api.BaseUrl;
            apiUrl.Location = new Point(160, 16);
            apiUrl.Width = 320;

            var saveUrl = AppTheme.Button("Сохранить");
            saveUrl.Location = new Point(500, 14);
            saveUrl.Width = 100;
            saveUrl.Click += (_, _) =>
            {
                _api.SetBaseUrl(apiUrl.Text);
                RefreshStatus();
                MessageBox.Show(this, "API URL сохранен.", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var authLabel = AppTheme.Label("Авторизация");
            authLabel.Location = new Point(0, 74);

            var login = AppTheme.Button(_api.IsAuthenticated ? "Сменить пользователя" : "Войти", true);
            login.Location = new Point(160, 68);
            login.Width = 160;
            login.Click += async (_, _) =>
            {
                _api.SetBaseUrl(apiUrl.Text);
                await ShowSectionAsync("login");
            };

            var logout = AppTheme.Button("Выйти");
            logout.Location = new Point(330, 68);
            logout.Width = 86;
            logout.Click += (_, _) =>
            {
                _api.Logout();
                RefreshStatus();
                MessageBox.Show(this, "Вы вышли из системы.", "Авторизация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var refresh = AppTheme.Button("Обновить данные");
            refresh.Location = new Point(160, 122);
            refresh.Width = 140;
            refresh.Click += async (_, _) => await ShowSectionAsync("fills");

            var note = AppTheme.Label("Интерфейс повторяет Wails/HTML: черная боковая панель, верхняя строка, карточки, таблицы и модальные формы.");
            note.Location = new Point(0, 188);
            note.MaximumSize = new Size(760, 0);

            panel.Controls.AddRange(new Control[] { apiLabel, apiUrl, saveUrl, authLabel, login, logout, refresh, note });
            root.Controls.Add(panel);
            root.Controls.Add(header);
            _content.Controls.Add(root);
        }

        private async Task LoadEmployeesAsync()
        {
            if (_employeeGrid is null)
            {
                return;
            }

            await ExecuteAsync(async () =>
            {
                var rank = int.TryParse(_employeeRank?.Text, out var parsedRank) ? parsedRank : (int?)null;
                var position = _employeePosition?.Text == "Все должности" ? null : _employeePosition?.Text;
                var response = await _api.GetEmployeesAsync(_employeePage, PageSize, _employeeSearch?.Text, position, rank);
                _employeeGrid.DataSource = new BindingList<EmployeeRow>(response.Items.Select(ManualMapper.ToRow).ToList());
                UpdatePager(_employeePager, response);
            });
        }

        private async Task LoadContragentsAsync()
        {
            if (_contragentGrid is null)
            {
                return;
            }

            await ExecuteAsync(async () =>
            {
                var type = _contragentType?.Text == "Все типы" ? null : ManualMapper.ToContragentApiValue(_contragentType?.Text ?? string.Empty);
                var response = await _api.GetContragentsAsync(_contragentPage, PageSize, _contragentSearch?.Text, type);
                _contragentGrid.DataSource = new BindingList<ContragentRow>(response.Items.Select(ManualMapper.ToRow).ToList());
                UpdatePager(_contragentPager, response);
            });
        }

        private async Task LoadWarehouseAsync()
        {
            if (_warehouseGrid is null)
            {
                return;
            }

            await ExecuteAsync(async () =>
            {
                var contragentType = _warehouseContragentType?.Text == "Все контрагенты"
                    ? null
                    : ManualMapper.ToContragentApiValue(_warehouseContragentType?.Text ?? string.Empty);
                var materialType = _warehouseMaterialType?.Text == "Все материалы"
                    ? null
                    : ManualMapper.ToMaterialApiValue(_warehouseMaterialType?.Text ?? string.Empty);

                var response = await _api.GetWarehouseAsync(_warehousePage, PageSize, _warehouseSearch?.Text, contragentType, materialType);
                _warehouseGrid.DataSource = new BindingList<WarehouseRow>(response.Items.Select(ManualMapper.ToRow).ToList());
                UpdatePager(_warehousePager, response);
            });
        }

        private async Task LoadFillsAsync()
        {
            if (_fillGrid is null)
            {
                return;
            }

            await ExecuteAsync(async () =>
            {
                var type = _fillType?.Text == "Все типы" ? null : _fillType?.Text;
                var response = await _api.GetFillsAsync(_fillPage, PageSize, type, null, null);
                _fillGrid.DataSource = new BindingList<FillRow>(response.Items.Select(ManualMapper.ToRow).ToList());
                UpdatePager(_fillPager, response);
                await LoadFillStatsOnlyAsync();
            });
        }

        private async Task LoadFillStatsOnlyAsync()
        {
            if (_statTotal is null || _statToday is null || _statEmployees is null || _statContragents is null)
            {
                return;
            }

            var fills = await _api.GetFillStatsAsync();
            var employees = await _api.GetEmployeesAsync(1, 1, null, null, null);
            var contragents = await _api.GetContragentsAsync(1, 1, null, null);

            _statTotal.Text = fills.TotalCount.ToString();
            _statToday.Text = fills.TodayCount.ToString();
            _statEmployees.Text = employees.Total.ToString();
            _statContragents.Text = contragents.Total.ToString();
        }

        private async void EmployeeGridCellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (_employeeGrid is null || e.RowIndex < 0 || RowAt<EmployeeRow>(_employeeGrid, e.RowIndex) is not { } row)
            {
                return;
            }

            var column = _employeeGrid.Columns[e.ColumnIndex].Name;
            if (column == "Edit")
            {
                using var dialog = new EmployeeDialog(row);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    await ExecuteAsync(async () =>
                    {
                        await _api.UpdateEmployeeAsync(row.Id, dialog.ToUpdateRequest());
                        await LoadEmployeesAsync();
                    });
                }
            }
            else if (column == "Delete" && ConfirmDelete("Удалить сотрудника?"))
            {
                await ExecuteAsync(async () =>
                {
                    await _api.DeleteEmployeeAsync(row.Id);
                    await LoadEmployeesAsync();
                    await LoadFillStatsOnlyAsync();
                });
            }
        }

        private async void ContragentGridCellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (_contragentGrid is null || e.RowIndex < 0 || RowAt<ContragentRow>(_contragentGrid, e.RowIndex) is not { } row)
            {
                return;
            }

            var column = _contragentGrid.Columns[e.ColumnIndex].Name;
            if (column == "Materials")
            {
                await OpenMaterialsAsync(row);
            }
            else if (column == "Edit")
            {
                using var dialog = new ContragentDialog(row);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    await ExecuteAsync(async () =>
                    {
                        await _api.UpdateContragentAsync(row.Id, dialog.ToUpdateRequest());
                        await LoadContragentsAsync();
                    });
                }
            }
            else if (column == "Delete" && ConfirmDelete("Удалить контрагента? Связанные материалы могут быть защищены правилами БД."))
            {
                await ExecuteAsync(async () =>
                {
                    await _api.DeleteContragentAsync(row.Id);
                    await LoadContragentsAsync();
                    await LoadFillStatsOnlyAsync();
                });
            }
        }

        private async void WarehouseGridCellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (_warehouseGrid is null || e.RowIndex < 0 || RowAt<WarehouseRow>(_warehouseGrid, e.RowIndex) is not { } row)
            {
                return;
            }

            var column = _warehouseGrid.Columns[e.ColumnIndex].Name;
            if (column == "Income")
            {
                using var dialog = new QuantityDialog("Приход товара", "Добавить");
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    await ExecuteAsync(async () =>
                    {
                        await _api.UpdateWarehouseQuantityAsync(row.Id, row.Qty + dialog.Quantity);
                        await _api.CreateFillAsync(new FillCreateRequest
                        {
                            Type = "Материал",
                            Description = $"Приход: {row.MaterialName}",
                            Related = $"+{dialog.Quantity} / {row.ContragentName}"
                        });
                        await LoadWarehouseAsync();
                    });
                }
            }
            else if (column == "Deduct")
            {
                using var dialog = new QuantityDialog("Списание товара", "Списать");
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    await ExecuteAsync(async () =>
                    {
                        await _api.DeductWarehouseQuantityAsync(row.Id, dialog.Quantity);
                        await _api.CreateFillAsync(new FillCreateRequest
                        {
                            Type = "Материал",
                            Description = $"Списание: {row.MaterialName}",
                            Related = $"-{dialog.Quantity} / {row.ContragentName}"
                        });
                        await LoadWarehouseAsync();
                    });
                }
            }
            else if (column == "Delete" && ConfirmDelete("Удалить позицию со склада?"))
            {
                await ExecuteAsync(async () =>
                {
                    await _api.DeleteWarehouseAsync(row.Id);
                    await LoadWarehouseAsync();
                });
            }
        }

        private async Task OpenMaterialsAsync(ContragentRow contragent)
        {
            var form = new Form
            {
                Text = $"Материалы: {contragent.Name}",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(920, 560),
                MinimumSize = new Size(760, 460),
                BackColor = AppTheme.Background,
                Font = AppTheme.BaseFont
            };

            var header = HeaderWithButton(contragent.Name, "Материалы контрагента", "+ Добавить материал", () => Task.CompletedTask);
            header.Dock = DockStyle.Top;

            var grid = AppTheme.Grid();
            AddTextColumn(grid, "Type", "Тип", 110);
            AddTextColumn(grid, "Name", "Название", 220);
            AddTextColumn(grid, "Color", "Цвет", 100);
            AddTextColumn(grid, "Article", "Артикул", 120);
            AddTextColumn(grid, "Unit", "Ед.", 70);
            AddTextColumn(grid, "Qty", "Кол-во", 100);
            AddTextColumn(grid, "Price", "Цена", 100);
            AddTextColumn(grid, "Sum", "Сумма", 110);
            AddButtonColumn(grid, "Edit", "Правка", "Изм.", 70);
            AddButtonColumn(grid, "Delete", "Удалить", "Удалить", 80);

            async Task LoadMaterials()
            {
                await ExecuteAsync(async () =>
                {
                    var response = await _api.GetMaterialsAsync(contragent.Id, 1, 500);
                    grid.DataSource = new BindingList<MaterialRow>(response.Items.Select(ManualMapper.ToRow).ToList());
                });
            }

            var addButton = header.Controls.OfType<Button>().FirstOrDefault();
            if (addButton is not null)
            {
                addButton.Click += async (_, _) =>
                {
                    using var dialog = new MaterialDialog(contragent.Id);
                    if (dialog.ShowDialog(form) == DialogResult.OK)
                    {
                        await ExecuteAsync(async () =>
                        {
                            await _api.CreateMaterialAsync(dialog.ToCreateRequest());
                            await LoadMaterials();
                            await LoadWarehouseAsync();
                            await LoadContragentsAsync();
                        });
                    }
                };
            }

            grid.CellContentClick += async (_, e) =>
            {
                if (e.RowIndex < 0 || RowAt<MaterialRow>(grid, e.RowIndex) is not { } row)
                {
                    return;
                }

                var column = grid.Columns[e.ColumnIndex].Name;
                if (column == "Edit")
                {
                    using var dialog = new MaterialDialog(contragent.Id, row);
                    if (dialog.ShowDialog(form) == DialogResult.OK)
                    {
                        await ExecuteAsync(async () =>
                        {
                            await _api.UpdateMaterialAsync(row.Id, dialog.ToUpdateRequest());
                            await LoadMaterials();
                            await LoadWarehouseAsync();
                        });
                    }
                }
                else if (column == "Delete" && ConfirmDelete("Удалить материал?"))
                {
                    await ExecuteAsync(async () =>
                    {
                        await _api.DeleteMaterialAsync(row.Id);
                        await LoadMaterials();
                        await LoadWarehouseAsync();
                        await LoadContragentsAsync();
                    });
                }
            };

            form.Controls.Add(grid);
            form.Controls.Add(header);
            await LoadMaterials();
            form.ShowDialog(this);
        }

        private async Task SignInFromMainWindowAsync()
        {
            if (_loginApiUrl is null || _loginUsername is null || _loginPassword is null || _loginRegister is null)
            {
                return;
            }

            var username = _loginUsername.Text.Trim();
            var password = _loginPassword.Text;

            if (string.IsNullOrWhiteSpace(_loginApiUrl.Text) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetLoginMessage("Заполните API URL, логин и пароль.", AppTheme.Danger);
                return;
            }

            try
            {
                UseWaitCursor = true;
                SetLoginMessage("Выполняется вход...", AppTheme.TextMuted);
                _api.SetBaseUrl(_loginApiUrl.Text);

                if (_loginRegister.Checked)
                {
                    await _api.RegisterAsync(new RegisterRequest
                    {
                        Username = username,
                        Password = password
                    });
                }
                else
                {
                    await _api.LoginAsync(new LoginRequest
                    {
                        Username = username,
                        Password = password
                    });
                }

                SetLoginMessage("Вход выполнен успешно.", Color.FromArgb(46, 125, 50));
                RefreshStatus();
                await ShowSectionAsync("fills");
            }
            catch (Exception ex)
            {
                SetLoginMessage(ex.Message, AppTheme.Danger);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async Task CheckDatabaseFromMainWindowAsync()
        {
            if (_loginApiUrl is null)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                SetLoginMessage("Проверяем подключение API к PostgreSQL...", AppTheme.TextMuted);
                _api.SetBaseUrl(_loginApiUrl.Text);

                var result = await _api.CheckDatabaseAsync();
                var color = result.CanConnect ? Color.FromArgb(46, 125, 50) : AppTheme.Danger;
                var status = result.CanConnect ? "PostgreSQL подключен" : "PostgreSQL не подключен";
                SetLoginMessage($"{status}. База: {result.Database}; источник: {result.DataSource}; {result.Message}", color);
                RefreshStatus();
            }
            catch (Exception ex)
            {
                SetLoginMessage($"Не удалось проверить БД: {ex.Message}", AppTheme.Danger);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void SetLoginMessage(string message, Color color)
        {
            if (_loginMessage is null)
            {
                return;
            }

            _loginMessage.Text = message;
            _loginMessage.ForeColor = color;
        }

        private async Task<bool> EnsureAuthenticatedAsync(bool forceDialog = false)
        {
            if (_api.IsAuthenticated && !forceDialog)
            {
                return true;
            }

            if (!_api.IsAuthenticated || forceDialog)
            {
                await ShowSectionAsync("login");
                RefreshStatus();
                return false;
            }

            using var dialog = new AuthDialog(_api.BaseUrl);
            while (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _api.SetBaseUrl(dialog.ApiUrl);
                    if (dialog.ShouldRegister)
                    {
                        await _api.RegisterAsync(new RegisterRequest
                        {
                            Username = dialog.Username,
                            Password = dialog.Password
                        });
                    }
                    else
                    {
                        await _api.LoginAsync(new LoginRequest
                        {
                            Username = dialog.Username,
                            Password = dialog.Password
                        });
                    }

                    RefreshStatus();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Авторизация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            RefreshStatus();
            return false;
        }

        private async Task ExecuteAsync(Func<Task> action)
        {
            try
            {
                UseWaitCursor = true;
                await action();
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _api.Logout();
                MessageBox.Show(this, ex.Message, "Требуется вход", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                await ShowSectionAsync("login");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                RefreshStatus();
            }
        }

        private static Panel ContentRoot()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background
            };
        }

        private static Panel Header(string title, string subtitle)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = AppTheme.Background
            };
            var titleLabel = AppTheme.Label(title, true);
            titleLabel.Location = new Point(0, 4);
            var subtitleLabel = AppTheme.Label(subtitle);
            subtitleLabel.Location = new Point(0, 30);
            panel.Controls.AddRange(new Control[] { titleLabel, subtitleLabel });
            return panel;
        }

        private static Label FieldLabel(string text, int y)
        {
            var label = AppTheme.Label(text);
            label.Location = new Point(0, y);
            label.Width = 150;
            return label;
        }

        private static Panel HeaderWithButton(string title, string subtitle, string buttonText, Func<Task> onClick)
        {
            var panel = Header(title, subtitle);
            var button = AppTheme.Button(buttonText, true);
            button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button.Location = new Point(720, 10);
            button.Width = 170;
            button.Click += async (_, _) => await onClick();
            panel.Resize += (_, _) => button.Location = new Point(panel.Width - button.Width, 8);
            panel.Controls.Add(button);
            return panel;
        }

        private static FlowLayoutPanel Toolbar()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 48,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = AppTheme.Background,
                Padding = new Padding(0, 4, 0, 8)
            };
        }

        private static Panel Pager(out Label label, Func<Task> prev, Func<Task> next)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                BackColor = AppTheme.Background
            };

            var prevButton = AppTheme.Button("Назад");
            prevButton.Width = 80;
            prevButton.Location = new Point(0, 8);
            prevButton.Click += async (_, _) => await prev();

            var nextButton = AppTheme.Button("Вперед");
            nextButton.Width = 80;
            nextButton.Location = new Point(90, 8);
            nextButton.Click += async (_, _) => await next();

            label = AppTheme.Label("Страница 1");
            label.Location = new Point(190, 16);

            panel.Controls.AddRange(new Control[] { prevButton, nextButton, label });
            return panel;
        }

        private static Label StatCard(FlowLayoutPanel parent, string value, string label)
        {
            var panel = new Panel
            {
                Width = 170,
                Height = 70,
                Margin = new Padding(0, 0, 12, 0),
                BackColor = AppTheme.Surface
            };

            var valueLabel = new Label
            {
                Text = value,
                AutoSize = true,
                Font = new Font("Consolas", 17F, FontStyle.Bold),
                ForeColor = AppTheme.Text,
                Location = new Point(14, 10)
            };
            var textLabel = AppTheme.Label(label);
            textLabel.Font = new Font(AppTheme.SmallFont, FontStyle.Bold);
            textLabel.Location = new Point(14, 42);
            panel.Controls.AddRange(new Control[] { valueLabel, textLabel });
            parent.Controls.Add(panel);
            return valueLabel;
        }

        private static void AddTextColumn(DataGridView grid, string property, string header, int width)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = property,
                DataPropertyName = property,
                HeaderText = header,
                Width = width
            });
        }

        private static void AddButtonColumn(DataGridView grid, string name, string header, string text, int width)
        {
            grid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = name,
                HeaderText = header,
                Text = text,
                UseColumnTextForButtonValue = true,
                Width = width,
                FlatStyle = FlatStyle.Flat
            });
        }

        private static T? RowAt<T>(DataGridView? grid, int rowIndex)
            where T : class
        {
            return grid?.Rows[rowIndex].DataBoundItem as T;
        }

        private static void UpdatePager<T>(Label? label, PagedResponse<T> response)
        {
            if (label is null)
            {
                return;
            }

            var totalPages = Math.Max(1, response.TotalPages);
            label.Text = $"Страница {response.Page} из {totalPages} · всего {response.Total}";
        }

        private static bool ConfirmDelete(string text)
        {
            return MessageBox.Show(text, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void RefreshStatus()
        {
            _status.Text = _api.IsAuthenticated
                ? $"API: {_api.BaseUrl} · {_api.Username}"
                : $"API: {_api.BaseUrl} · не выполнен вход";
        }
    }
}
