namespace SewingWorkshop.WinForms.UI
{
    public static class AppTheme
    {
        public static readonly Color Background = Color.White;
        public static readonly Color Surface = Color.FromArgb(247, 247, 247);
        public static readonly Color Border = Color.FromArgb(216, 216, 216);
        public static readonly Color BorderStrong = Color.FromArgb(170, 170, 170);
        public static readonly Color Text = Color.FromArgb(17, 17, 17);
        public static readonly Color TextMuted = Color.FromArgb(102, 102, 102);
        public static readonly Color TextFaint = Color.FromArgb(153, 153, 153);
        public static readonly Color Danger = Color.FromArgb(192, 57, 43);
        public static readonly Font BaseFont = new("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font HeaderFont = new("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font SmallFont = new("Segoe UI", 8F, FontStyle.Regular);
        public static readonly Font MonoFont = new("Consolas", 9F, FontStyle.Regular);

        public static Button Button(string text, bool primary = false)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = false,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                Font = BaseFont,
                Cursor = Cursors.Hand,
                BackColor = primary ? Text : Background,
                ForeColor = primary ? Color.White : Text,
                Padding = new Padding(10, 0, 10, 0)
            };
            button.FlatAppearance.BorderColor = primary ? Text : BorderStrong;
            button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(51, 51, 51) : Surface;
            return button;
        }

        public static TextBox TextBox(string placeholder = "")
        {
            return new TextBox
            {
                PlaceholderText = placeholder,
                BorderStyle = BorderStyle.FixedSingle,
                Font = BaseFont,
                Height = 30
            };
        }

        public static ComboBox ComboBox(params string[] values)
        {
            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = BaseFont,
                Height = 30
            };
            comboBox.Items.AddRange(values);
            if (values.Length > 0)
            {
                comboBox.SelectedIndex = 0;
            }

            return comboBox;
        }

        public static Label Label(string text, bool header = false)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = header ? HeaderFont : BaseFont,
                ForeColor = header ? Text : TextMuted
            };
        }

        public static DataGridView Grid()
        {
            var grid = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                BackgroundColor = Background,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                Dock = DockStyle.Fill,
                EnableHeadersVisualStyles = false,
                Font = BaseFont,
                GridColor = Border,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = Surface;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(BaseFont, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Background;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 235, 235);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.RowTemplate.Height = 34;

            return grid;
        }

        public static TableLayoutPanel FormGrid()
        {
            return new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
        }

        public static void AddField(TableLayoutPanel panel, string label, Control control)
        {
            var row = panel.RowCount++;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var labelControl = Label(label);
            labelControl.Margin = new Padding(0, 8, 12, 4);
            control.Margin = new Padding(0, 4, 0, 8);
            control.Width = 260;

            panel.Controls.Add(labelControl, 0, row);
            panel.Controls.Add(control, 1, row);
        }
    }
}
