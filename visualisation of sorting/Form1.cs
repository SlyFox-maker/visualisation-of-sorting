using Medallion;
using System.Diagnostics;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime.Operations;
using static IronPython.Modules._ast;
using SharpDX.DXGI;

namespace visualisation_of_sorting
{
    public partial class Form1 : Form
    {

        private int sizeOfArray = 0;
        private int delay = 0;
        private String algorithm = "";

        private double[] arraySorting;
        private bool sorting = false;

        private String path = "./algorithms/";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //description for elements
            ToolTip algTP = new ToolTip();

            algTP.SetToolTip(comboBox1, "Choise algorithm");
            algTP.SetToolTip(numericUpDown2, "Size of array");
            algTP.SetToolTip(numericUpDown1, "Delay");

            pictureBox1.Image = Properties.Resources.preview as Bitmap;
            readFiles();
        }
        private void readFiles()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Update...");

            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string filename in files)
            {
                String name = Path.GetFileNameWithoutExtension(filename);
                if (Path.GetExtension(filename) == ".py")
                {
                    comboBox1.Items.Add(name);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sizeOfArray = Convert.ToInt32(numericUpDown2.Value) + 1;

            //Creating array
            generationRandomNumbers();

            //First painting
            paintArrays(0,0);

            button2.Enabled = true;
        }
        public void paintArrays(int firstElement, int secondElement)
        {
            Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bit);

            //initialization sizes
            double widthLines = Math.Floor((double)bit.Width / (double)sizeOfArray);
            Pen p;
            int startPointX = (int)widthLines;
            int startPointY = bit.Height;
            int unitHeight = bit.Height / sizeOfArray;

            //painting
            for (int i = 1; i < sizeOfArray; i++)
            {
                if (firstElement == i || secondElement==i)
                {
                    p = new Pen(Color.Red, (int)widthLines);
                }
                else
                {
                    p = new Pen(Color.White, (int)widthLines);
                }
                //new posision
                g.DrawLine(p, startPointX, startPointY, startPointX, (int)(startPointY - (unitHeight * arraySorting[i])));
                startPointX += (int)widthLines + 1;
                Debug.WriteLine(arraySorting[i]);
            }

            //putting to picturebox
            pictureBox1.Image = bit;
            g.Flush();
        }
        private void generationRandomNumbers() {
            Random r = new Random();
            arraySorting = new double[sizeOfArray];
            for (int i = 1; i < sizeOfArray; i++)
            {
                arraySorting[i] = i;
            }

            arraySorting.Shuffle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switchButtonStart();
            algorithm = (String)comboBox1.Text;

            //checking
            if (algorithm == ""||algorithm=="Update...")
            {
                MessageBox.Show("Algorithm was not choiced");
                return;
            }

            Thread workerThread = new Thread(new ThreadStart(execute));
            // Start secondary thread  
            workerThread.Start();
        }
        private void switchButtonStart()
        {
            if (!sorting)
            {
                button2.Text = "Stop";
                sorting = true;
            }
            else if (sorting)
            {
                if (button1.InvokeRequired)
                {
                    button2.Invoke(new MethodInvoker(delegate
                    {
                        button2.Text = "Start";
                    }));
                }
                else
                {
                    button2.Text = "Start";
                }
                sorting = false;
                return;
            }
        }
        private void execute()
        {
            //Execute py scripts
            String path = this.path + algorithm + ".py";
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            double[] arrayFilter = (double[])arraySorting.Clone();
            try
            {
                scope.SetVariable("arr", arrayFilter);
                engine.ExecuteFile(path, scope);  //start script

                var arrHistory = scope.GetVariable("arrHistory");
                var arrSort = scope.GetVariable("arr");

                for (int i = 0; i < arrHistory.Count; i++)
                {
                    if (!sorting)
                        return;
                    delay = (int)numericUpDown1.Value;
                    var parametres = arrHistory[i];


                    int firstElement = parametres[1];
                    int secondElement = parametres[2];

                    var arrVar = parametres[0];
                    double[] arr = new double[arrVar.Count];
                    for (int w = 0; w < arrVar.Count; w++)
                    {
                        double d = arrVar[w];
                        arr[w] = d;
                    }

                    arraySorting = arr;


                    paintArrays(firstElement, secondElement);


                    SoundUtilities.PlaySound("./check.wav", 0.1f);
                    Thread.Sleep(delay);
                }

                finalPaint();
            }
            catch (IOException e)
            {
                switchButtonStart();
                MessageBox.Show(e.Message);
            }
            catch(System.MissingMemberException e)
            {
                switchButtonStart();
                MessageBox.Show(e.Message);
            }
        }
        private void finalPaint()
        {
            paintArrays(0, 0);
        }


        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if((String)comboBox1.Text == "Update...")
            {
                readFiles();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sorting = false;
        }
    }
}