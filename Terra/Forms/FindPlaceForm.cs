using Terra.Map.Render.Features;

namespace Terra.Forms
{
    internal class FindPlaceForm : Form
    {
        private MainForm _mainForm;
        private TextBox _searchBox;
        private Button _searchBtn;
        private ListBox _resultsBox;

        public FindPlaceForm(MainForm mainForm)
        {
            this._mainForm = mainForm;

            this.Text = "Terra: Find place...";
            this.Size = new(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;

            TableLayoutPanel layout = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            FlowLayoutPanel top = new()
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            this._searchBox = new()
            {
                PlaceholderText = "Search...",
                Width = 270
            };
            top.Controls.Add(this._searchBox);

            this._searchBtn = new()
            {
                Text = "Go",
                AutoSize = true
            };
            this._searchBtn.Click += _searchBtn_Click;
            top.Controls.Add(this._searchBtn);

            layout.Controls.Add(top, 0, 0);

            this._resultsBox = new()
            {
                Dock = DockStyle.Fill
            };
            foreach (var place in this._mainForm.RenderData.Places)
            {
                this._resultsBox.Items.Add(place.Name);
            }
            layout.Controls.Add(this._resultsBox, 0, 1);

            this.Controls.Add(layout);
        }

        private void _searchBtn_Click(object? sender, EventArgs e)
        {
            this._resultsBox.Items.Clear();

            string query = this._searchBox.Text;

            Dictionary<string, float> scores = new();

            foreach (var place in this._mainForm.RenderData.Places)
            {
                float lengthScorePerc = query.Length / place.Name.Length;

                int similarCharsCount = 0;
                for (int i = 0; i < Math.Min(query.Length, place.Name.Length); i++)
                {
                    if (place.Name[i] == query[i])
                        similarCharsCount++;
                }

                float charSimilarScorePerc = similarCharsCount / Math.Min(query.Length, place.Name.Length);

                float score = lengthScorePerc / 0.5f + charSimilarScorePerc / 0.5f;

                scores.Add(place.Name, score);
            }

            scores.OrderByDescending(x => x.Value);

            foreach (var (name, score) in scores)
            {
                this._resultsBox.Items.Add(name);
            }
        }
    }
}
