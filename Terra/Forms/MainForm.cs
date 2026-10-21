using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Terra.Map;
using Terra.Map.Graph;
using Terra.Map.Render;

namespace Terra.Forms
{
    internal class MainForm : Form
    {
        private FormContext _context;

        // State
        public RenderData RenderData;
        public GraphData GraphData;
        public Graph Graph;

        // Form controls
        public MapControl MapControl { get; private set; }
        private MenuStrip _menuStrip;
        private ToolStripMenuItem _fileMenu;
        private ToolStripMenuItem _viewMenu;
        private ToolStripMenuItem _toolsMenu;


        public MainForm(FormContext context, string path)
        {
            this._context = context;

            this.Text = "Terra";
            this.ClientSize = new(1600, 900);

            using MapImporter mapImporter = new(path);
            MapData mapData = mapImporter.Import();

            this.RenderData = mapData.RenderData;
            this.GraphData = mapData.GraphData;
            this.Graph = new(mapData.GraphData);

            this.MapControl = new(this.RenderData, this.GraphData);

            this._menuStrip = new()
            {
                Dock = DockStyle.Top
            };

            // File
            this._fileMenu = new("File");
            this._fileMenu.DropDownItems.Add("Open map...", null, this.OpenMap);
            this._fileMenu.DropDownItems.Add(new ToolStripSeparator());
            this._fileMenu.DropDownItems.Add("Exit", null, (_, _) => this.Close());
            this._menuStrip.Items.Add(this._fileMenu);

            // View
            this._viewMenu = new("View");

            ToolStripMenuItem showRoads = new("Show roads")
            {
                CheckOnClick = true,
                Checked = true
            };
            showRoads.CheckedChanged += (sender, e) =>
                this.MapControl.ShowRoads = showRoads.Checked;
            this._viewMenu.DropDownItems.Add(showRoads);
            
            ToolStripMenuItem showBuildings = new("Show buildings")
            {
                CheckOnClick = true,
                Checked = true
            };
            showBuildings.CheckedChanged += (sender, e) =>
                this.MapControl.ShowBuildings = showBuildings.Checked;
            this._viewMenu.DropDownItems.Add(showBuildings);
            
            ToolStripMenuItem showPlaceNames = new("Show place names")
            {
                CheckOnClick = true,
                Checked = true
            };
            showPlaceNames.CheckedChanged += (sender, e) =>
                this.MapControl.ShowPlaceNames = showPlaceNames.Checked;
            this._viewMenu.DropDownItems.Add(showPlaceNames);
            
            this._menuStrip.Items.Add(this._viewMenu);

            // Tools
            this._toolsMenu = new("Tools");
            this._toolsMenu.DropDownItems.Add("Find place...", null, (_, _) => new FindPlaceForm(this).Show());
            this._menuStrip.Items.Add(this._toolsMenu);

            this.Controls.Add(this._menuStrip);
            this.Controls.Add(this.MapControl);
        }

        private void OpenMap(object? sender, EventArgs e)
        {

        }
    }
}
