namespace Terra.Forms
{
    internal class WelcomeForm : Form
    {
        private FormContext _context;

        private ListView _mapsList;
        private Button _openMapButton;
        private Button _deleteMapButton;
        private Button _downloadMapButton;

        public WelcomeForm(FormContext context)
        {
            this._context = context;

            this.Text = "Terra: Welcome";
            this.ClientSize = new(1200, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Helps to centre the content horizontally
            TableLayoutPanel root = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            // Helps to structure the content in the centre
            TableLayoutPanel content = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty
            };
            content.RowStyles.Add(new(SizeType.Percent, 20));
            content.RowStyles.Add(new(SizeType.Percent, 60));
            content.RowStyles.Add(new(SizeType.Percent, 20));

            // This is all added to the middle column (the one taking up 60% of vertical space)
            content.Controls.Add(this.BuildButtons(), 0, 2);
            content.Controls.Add(this.BuildMapList(), 0, 1);
            content.Controls.Add(new Label()
            {
                Text = "Terra",
                Font = new Font(SystemFonts.DefaultFont.FontFamily, 24),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            }, 0, 0);

            root.Controls.Add(content, 1, 0);
            this.Controls.Add(root);
        }

        private ListView BuildMapList()
        {
            this._mapsList = new()
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true
            };
            this._mapsList.MouseDoubleClick += _mapsList_MouseDoubleClick;
            this._mapsList.Columns.Add("Name");
            this._mapsList.Columns.Add("Size");
            this._mapsList.Columns.Add("Date added");

            this.UpdateMapList();

            return this._mapsList;
        }

        private void UpdateMapList()
        {
            this._mapsList.Items.Clear();

            DirectoryInfo dirInfo = new(Program.DataPath);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                this._mapsList.Items.Add(new ListViewItem(
                [
                    file.Name,
                    $"{file.Length / 1024 / 1024} MiB",
                    $"{file.CreationTime}"
                ]));
            }
        }

        private FlowLayoutPanel BuildButtons()
        {
            FlowLayoutPanel buttonsPanel = new()
            {
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = true,
                Dock = DockStyle.Right
            };

            this._openMapButton = new()
            {
                Text = "Open",
                AutoSize = true
            };
            this._openMapButton.Click += _openMapButton_Click;
            this._deleteMapButton = new()
            {
                Text = "Delete",
                AutoSize = true
            };
            this._deleteMapButton.Click += _deleteMapButton_Click;
            this._downloadMapButton = new()
            {
                Text = "Download",
                AutoSize = true
            };
            this._downloadMapButton.Click += _downloadMapButton_Click;

            buttonsPanel.Controls.Add(this._openMapButton);
            buttonsPanel.Controls.Add(this._deleteMapButton);
            buttonsPanel.Controls.Add(this._downloadMapButton);

            return buttonsPanel;
        }

        private void _openMapButton_Click(object? sender, EventArgs e)
        {
            this._context.SwitchForm(this, new MainForm(this._context, ""));
        }

        private void _deleteMapButton_Click(object? sender, EventArgs e)
        {
            // TODO: Clean this up, the formatting is messy
            if (MessageBox.Show(
                "Are you sure you want to delete this map?",
                "Delete map",
                MessageBoxButtons.YesNo) 
                == DialogResult.Yes)
            {
                File.Delete(
                    $"{Program.DataPath}{this._mapsList.SelectedItems[0].SubItems[0].Text}");
                this.UpdateMapList();
            }
        }

        private void _downloadMapButton_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _mapsList_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            this._context.SwitchForm(this, new MainForm(
                this._context, 
                $"{Program.DataPath}{this._mapsList.SelectedItems[0].SubItems[0].Text}"));
        }

    }
}
