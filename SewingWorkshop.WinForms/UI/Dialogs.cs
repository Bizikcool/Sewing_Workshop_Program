using SewingWorkshop.WinForms.Api;
using SewingWorkshop.WinForms.Mapping;

namespace SewingWorkshop.WinForms.UI
{
    public sealed class AuthDialog : Form
    {
        private readonly TextBox _apiUrl = AppTheme.TextBox();
        private readonly TextBox _username = AppTheme.TextBox("admin");
        private readonly TextBox _password = AppTheme.TextBox("пароль");
        private readonly CheckBox _register = new() { Text = "Создать пользователя", AutoSize = true, Font = AppTheme.BaseFont };

        public AuthDialog(string baseUrl)
        {
            Text = "Подключение к API";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(440, 250);
            BackColor = AppTheme.Background;
            Font = AppTheme.BaseFont;

            _apiUrl.Text = baseUrl;
            _password.UseSystemPasswordChar = true;

            var title = AppTheme.Label("Авторизация", true);
            title.Location = new Point(24, 20);

            var grid = AppTheme.FormGrid();
            grid.Location = new Point(24, 58);
            AppTheme.AddField(grid, "API URL", _apiUrl);
            AppTheme.AddField(grid, "Логин", _username);
            AppTheme.AddField(grid, "Пароль", _password);

            _register.Location = new Point(176, 178);

            var login = AppTheme.Button("Войти", true);
            login.Location = new Point(244, 205);
            login.Width = 82;
            login.DialogResult = DialogResult.OK;

            var cancel = AppTheme.Button("Отмена");
            cancel.Location = new Point(334, 205);
            cancel.Width = 82;
            cancel.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { title, grid, _register, login, cancel });
            AcceptButton = login;
            CancelButton = cancel;
        }

        public string ApiUrl => _apiUrl.Text.Trim();
        public string Username => _username.Text.Trim();
        public string Password => _password.Text;
        public bool ShouldRegister => _register.Checked;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(ApiUrl) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show(this, "Заполните API URL, логин и пароль.", "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }

            base.OnFormClosing(e);
        }
    }

    public sealed class EmployeeDialog : Form
    {
        private readonly TextBox _fio = AppTheme.TextBox("ФИО сотрудника");
        private readonly TextBox _position = AppTheme.TextBox("Должность");
        private readonly NumericUpDown _rank = new() { Minimum = 0, Maximum = 20, Width = 260 };

        public EmployeeDialog(EmployeeRow? row = null)
        {
            Text = row is null ? "Добавить сотрудника" : "Редактировать сотрудника";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 230);
            BackColor = AppTheme.Background;
            Font = AppTheme.BaseFont;

            if (row is not null)
            {
                _fio.Text = row.Fio;
                _position.Text = row.Position;
                _rank.Value = row.Rank;
            }

            var title = AppTheme.Label(Text, true);
            title.Location = new Point(24, 20);

            var grid = AppTheme.FormGrid();
            grid.Location = new Point(24, 58);
            AppTheme.AddField(grid, "ФИО", _fio);
            AppTheme.AddField(grid, "Должность", _position);
            AppTheme.AddField(grid, "Разряд", _rank);

            AddActions(210, 178);
            Controls.AddRange(new Control[] { title, grid });
        }

        public EmployeeCreateRequest ToCreateRequest()
        {
            return new EmployeeCreateRequest
            {
                Fio = _fio.Text.Trim(),
                Position = _position.Text.Trim(),
                Rank = (int)_rank.Value
            };
        }

        public EmployeeUpdateRequest ToUpdateRequest()
        {
            return new EmployeeUpdateRequest
            {
                Fio = _fio.Text.Trim(),
                Position = _position.Text.Trim(),
                Rank = (int)_rank.Value
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && (string.IsNullOrWhiteSpace(_fio.Text) || string.IsNullOrWhiteSpace(_position.Text)))
            {
                MessageBox.Show(this, "ФИО и должность обязательны.", "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }

        private void AddActions(int x, int y)
        {
            var save = AppTheme.Button("Сохранить", true);
            save.Location = new Point(x, y);
            save.Width = 92;
            save.DialogResult = DialogResult.OK;

            var cancel = AppTheme.Button("Отмена");
            cancel.Location = new Point(x + 100, y);
            cancel.Width = 82;
            cancel.DialogResult = DialogResult.Cancel;

            Controls.Add(save);
            Controls.Add(cancel);
            AcceptButton = save;
            CancelButton = cancel;
        }
    }

    public sealed class ContragentDialog : Form
    {
        private readonly ComboBox _type = AppTheme.ComboBox("Поставщик", "Заказчик");
        private readonly TextBox _name = AppTheme.TextBox("Название");
        private readonly TextBox _contact = AppTheme.TextBox("Контактное лицо");
        private readonly TextBox _phone = AppTheme.TextBox("+373...");

        public ContragentDialog(ContragentRow? row = null)
        {
            Text = row is null ? "Добавить контрагента" : "Редактировать контрагента";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(440, 285);
            BackColor = AppTheme.Background;
            Font = AppTheme.BaseFont;

            if (row is not null)
            {
                _type.SelectedItem = row.Type;
                _name.Text = row.Name;
                _contact.Text = row.Contact;
                _phone.Text = row.Phone;
            }

            var title = AppTheme.Label(Text, true);
            title.Location = new Point(24, 20);

            var grid = AppTheme.FormGrid();
            grid.Location = new Point(24, 58);
            AppTheme.AddField(grid, "Тип", _type);
            AppTheme.AddField(grid, "Название", _name);
            AppTheme.AddField(grid, "Контакт", _contact);
            AppTheme.AddField(grid, "Телефон", _phone);

            AddActions(230, 232);
            Controls.AddRange(new Control[] { title, grid });
        }

        public ContragentCreateRequest ToCreateRequest()
        {
            return new ContragentCreateRequest
            {
                Type = ManualMapper.ToContragentApiValue(_type.Text),
                Name = _name.Text.Trim(),
                Contact = NullIfEmpty(_contact.Text),
                Phone = NullIfEmpty(_phone.Text)
            };
        }

        public ContragentUpdateRequest ToUpdateRequest()
        {
            return new ContragentUpdateRequest
            {
                Type = ManualMapper.ToContragentApiValue(_type.Text),
                Name = _name.Text.Trim(),
                Contact = NullIfEmpty(_contact.Text),
                Phone = NullIfEmpty(_phone.Text)
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && string.IsNullOrWhiteSpace(_name.Text))
            {
                MessageBox.Show(this, "Название контрагента обязательно.", "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }

        private static string? NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private void AddActions(int x, int y)
        {
            var save = AppTheme.Button("Сохранить", true);
            save.Location = new Point(x, y);
            save.Width = 92;
            save.DialogResult = DialogResult.OK;

            var cancel = AppTheme.Button("Отмена");
            cancel.Location = new Point(x + 100, y);
            cancel.Width = 82;
            cancel.DialogResult = DialogResult.Cancel;

            Controls.Add(save);
            Controls.Add(cancel);
            AcceptButton = save;
            CancelButton = cancel;
        }
    }

    public sealed class MaterialDialog : Form
    {
        private readonly ComboBox _type = AppTheme.ComboBox("Ткань", "Нитки");
        private readonly TextBox _name = AppTheme.TextBox("Название материала");
        private readonly TextBox _color = AppTheme.TextBox("Цвет");
        private readonly TextBox _article = AppTheme.TextBox("Артикул");
        private readonly TextBox _unit = AppTheme.TextBox("шт");
        private readonly NumericUpDown _qty = new() { Minimum = 0, Maximum = 1000000, DecimalPlaces = 3, Width = 260 };
        private readonly NumericUpDown _price = new() { Minimum = 0, Maximum = 1000000, DecimalPlaces = 2, Width = 260 };
        private readonly int _contragentId;

        public MaterialDialog(int contragentId, MaterialRow? row = null)
        {
            _contragentId = contragentId;
            Text = row is null ? "Добавить материал" : "Редактировать материал";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(455, 410);
            BackColor = AppTheme.Background;
            Font = AppTheme.BaseFont;

            _unit.Text = "шт";

            if (row is not null)
            {
                _type.SelectedItem = row.Type;
                _name.Text = row.Name;
                _color.Text = row.Color;
                _article.Text = row.Article;
                _unit.Text = row.Unit;
                _qty.Value = Clamp(row.Qty, _qty.Minimum, _qty.Maximum);
                _price.Value = Clamp(row.Price, _price.Minimum, _price.Maximum);
            }

            var title = AppTheme.Label(Text, true);
            title.Location = new Point(24, 20);

            var grid = AppTheme.FormGrid();
            grid.Location = new Point(24, 58);
            AppTheme.AddField(grid, "Тип", _type);
            AppTheme.AddField(grid, "Название", _name);
            AppTheme.AddField(grid, "Цвет", _color);
            AppTheme.AddField(grid, "Артикул", _article);
            AppTheme.AddField(grid, "Единица", _unit);
            AppTheme.AddField(grid, "Количество", _qty);
            AppTheme.AddField(grid, "Цена", _price);

            AddActions(245, 358);
            Controls.AddRange(new Control[] { title, grid });
        }

        public MaterialCreateRequest ToCreateRequest()
        {
            return new MaterialCreateRequest
            {
                ContragentId = _contragentId,
                Type = ManualMapper.ToMaterialApiValue(_type.Text),
                Name = _name.Text.Trim(),
                Color = NullIfEmpty(_color.Text),
                Article = NullIfEmpty(_article.Text),
                Unit = string.IsNullOrWhiteSpace(_unit.Text) ? "шт" : _unit.Text.Trim(),
                Qty = _qty.Value,
                Price = _price.Value
            };
        }

        public MaterialUpdateRequest ToUpdateRequest()
        {
            return new MaterialUpdateRequest
            {
                ContragentId = _contragentId,
                Type = ManualMapper.ToMaterialApiValue(_type.Text),
                Name = _name.Text.Trim(),
                Color = NullIfEmpty(_color.Text),
                Article = NullIfEmpty(_article.Text),
                Unit = string.IsNullOrWhiteSpace(_unit.Text) ? "шт" : _unit.Text.Trim(),
                Qty = _qty.Value,
                Price = _price.Value
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && string.IsNullOrWhiteSpace(_name.Text))
            {
                MessageBox.Show(this, "Название материала обязательно.", "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private static string? NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private void AddActions(int x, int y)
        {
            var save = AppTheme.Button("Сохранить", true);
            save.Location = new Point(x, y);
            save.Width = 92;
            save.DialogResult = DialogResult.OK;

            var cancel = AppTheme.Button("Отмена");
            cancel.Location = new Point(x + 100, y);
            cancel.Width = 82;
            cancel.DialogResult = DialogResult.Cancel;

            Controls.Add(save);
            Controls.Add(cancel);
            AcceptButton = save;
            CancelButton = cancel;
        }
    }

    public sealed class QuantityDialog : Form
    {
        private readonly NumericUpDown _qty = new() { Minimum = 0.001M, Maximum = 1000000, DecimalPlaces = 3, Width = 220 };

        public QuantityDialog(string title, string label)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(360, 150);
            BackColor = AppTheme.Background;
            Font = AppTheme.BaseFont;

            var titleLabel = AppTheme.Label(title, true);
            titleLabel.Location = new Point(24, 20);

            var labelControl = AppTheme.Label(label);
            labelControl.Location = new Point(24, 66);
            _qty.Location = new Point(116, 62);

            var save = AppTheme.Button("Применить", true);
            save.Location = new Point(164, 105);
            save.Width = 92;
            save.DialogResult = DialogResult.OK;

            var cancel = AppTheme.Button("Отмена");
            cancel.Location = new Point(264, 105);
            cancel.Width = 74;
            cancel.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { titleLabel, labelControl, _qty, save, cancel });
            AcceptButton = save;
            CancelButton = cancel;
        }

        public decimal Quantity => _qty.Value;
    }
}
