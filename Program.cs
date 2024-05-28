using System;
using System.Diagnostics;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace MethodGrid
{


    public class MainForm : Form
    {
        public MainForm(TableOfValues tableOfValues, TableOfValues tableOfValues2, Stopwatch stopwatch)
        {
            InitializeComponent();

            var label = new Label();
            label.Dock = DockStyle.Fill;
            this.Controls.Add(label);

            this.Controls.Add(CreateplotByTableOfValues(tableOfValues, tableOfValues.Values.GetLength(1) - 1, "Кінцевий стан"));
            this.Controls.Add(CreateplotByTableOfValues(tableOfValues, tableOfValues2, 0, "Початковий стан"));
            this.Controls.Add(CreateplotByTableOfValues(tableOfValues2, tableOfValues2.Values.GetLength(1) - 1, "Кінцевий стан"));

            stopwatch.Stop();
            label.Text = $"Час роботи програми:\n{stopwatch.ElapsedMilliseconds} мілісекунд";
        }

        private PlotView CreateplotByTableOfValues(TableOfValues tableOfValues1, TableOfValues tableOfValues2, int rowForPlot, string name = null)
        {

            var plotView = new PlotView();
            plotView.Dock = DockStyle.Left;
            plotView.Width = Width / 3 - 20;


            var plotModel = CreatePlotModel(name is null ? tableOfValues1.Name : name);
            var series1 = CreateSeries(tableOfValues1, rowForPlot);
            var series2 = CreateSeries(tableOfValues2, rowForPlot);


            plotModel.Series.Add(series1);
            plotModel.Series.Add(series2);
            plotView.Model = plotModel;

            return plotView;
        }

        private PlotView CreateplotByTableOfValues(TableOfValues tableOfValues, int rowForPlot, string name = null)
        {

            var plotView = new PlotView();
            plotView.Dock = DockStyle.Left;
            plotView.Width = Width / 3 - 20;


            var plotModel = CreatePlotModel(name is null ? tableOfValues.Name : name) ;
            var series = CreateSeries(tableOfValues, rowForPlot);

            plotModel.Series.Add(series);
            plotView.Model = plotModel;

            return plotView;
        }

        private PlotModel CreatePlotModel(string title)
        {
            var plotModel = new PlotModel { Title = title };
            return plotModel;
        }

        private LineSeries CreateSeries(TableOfValues tableOfValues, int rowForPlot)
        {
            var series = new LineSeries();

            for (int i = 0; i < tableOfValues.Values.GetLength(0); i++)
                series.Points.Add(new DataPoint(tableOfValues.Grid.grid[i,0].X, tableOfValues.Values[i, rowForPlot]));

            return series;
        }

        [STAThread]
        static void Main()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Кроки сітки
            double h = 0.03;
            double tau = 0.0001;

            var grid = new Grid(h, tau);
            var variant3 = (new TableOfValues(grid));

            // Початкові умови
            MathFunction initialCond = new MathFunction ( x => 0.5 * Math.Cos(Math.PI * x));
            // Ліва гранична умова 
            MathFunction leftBoundaryCond = new MathFunction( k  => 0.5 * Math.Pow(Math.E, k)) ; 


            // Функція f
            MathFunction2 f = new MathFunction2((i, k) => i + k);

            // Константа a
            double a = (double)1 / 3;

            variant3.FillTable2(initialCond, leftBoundaryCond, a, f);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(variant3, variant3, stopwatch));
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1300, 400);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}
