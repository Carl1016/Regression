using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Regression
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static double AverageX = 0, AverageY = 0;
        static double Numerator = 0;
        static double Denominator = 0;
        static double RCB;
        static double RCA;
        static double Residual_SS = 0;
        static double Regression_SS = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            String filePath = "D:/hw1_data1.csv";
            DataTable tb = new DataTable();
            read_csv(filePath, ref tb);
            double[] X = new double[100];
            double[] Log_Y = new double[100];
            double[] Y = new double[100];
            double[,] dataset = new double[100, 2];
            Convert2Array(ref X, ref Y, ref Log_Y, tb);
            draw_origin_point( X,  Y);
            Build_train_dataset( X,  Log_Y, dataset);
            LinearRegression(dataset, X);

        }
        public void LinearRegression(double[,] point, double[] X)
        {
            if (point.GetLength(0) < 2)
            {
                Console.WriteLine("點的數量小於2，無法進行線性回歸");
                return;
            }
            for (int i = 0; i < point.GetLength(0); i++)
            {
                AverageX += point[i, 0];
                AverageY += point[i, 1];
            }
            AverageX /= point.GetLength(0);
            AverageY /= point.GetLength(0);
            Console.WriteLine("平均X: {0}\n平均Y: {1}", AverageX, AverageY);
            for (int i = 0; i < point.GetLength(0); i++)
            {
                Numerator += (point[i, 0] - AverageX) * (point[i, 1] - AverageY);
                Denominator += (point[i, 0] - AverageX) * (point[i, 0] - AverageX);
            }
            RCB = Numerator / Denominator;
            RCA = AverageY - RCB * AverageX;
            draw_spline( X);
            for (int i = 0; i < point.GetLength(0); i++)
            {
                Residual_SS += (point[i, 1] - RCA - RCB * point[i, 0]) * (point[i, 1] - RCA - RCB * point[i, 0]);
                Regression_SS += (RCA + RCB * point[i, 0] - AverageY) * (RCA + RCB * point[i, 0] - AverageY);
            }
        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }
        public void read_csv(String filePath, ref DataTable dt)
        {
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            string strLine = "";
            string[] aryLine;
            int columnCount = 0;
            bool IsFirst = true;
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];

                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
        }
        public void Convert2Array(ref double[] X, ref double[] Y,ref double[] Log_Y,DataTable tb)
        {
            for (int i = 0; i <= 99; i++)
            {
                X[i] = Convert.ToDouble(tb.Rows[i][0]);
                Log_Y[i] = Convert.ToDouble(tb.Rows[i][2]);
                Y[i] = Convert.ToDouble(tb.Rows[i][1]);
            }
        }
        public void Build_train_dataset( double[] X,  double[] Log_Y, double[,] dataset)
        {
            for (int i = 0; i <= 99; i++)
            {
                dataset[i, 0] = X[i];
                dataset[i, 1] = Log_Y[i];
            }
        }
        public void draw_origin_point( double[] X,  double[] Y)
        {
            this.chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            for (int i = 0; i <= 99; i++)
            {
                chart1.Series[0].Points.AddXY(X[i], Y[i]);
            }
        }
        public void draw_spline(double[]X)
        {
            double e = 2.71828182846;
            double y = 0;
            for (int i=0;i<=99;i++)
            {
                y = Math.Pow(e, RCA) * Math.Pow(e,RCB*X[i]);
                this.chart1.Series[1].Points.AddXY(X[i], y);
            }
            label1.Text = "轉換後的Beta值為: "+ Convert.ToString(RCB) + "\n轉換後的Alpha值為: "
                +Convert.ToString(Math.Pow(e, RCA));
        }
    }
}

