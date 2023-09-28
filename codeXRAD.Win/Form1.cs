namespace codeXRAD.Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button3_Click(null,null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = CX.Parameter(textBox1.Text, textBox2.Text, "(What?)");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = CX.ParameterSet(textBox1.Text, textBox2.Text, textBox3.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Prova=\"Hello world!\"; Note=\"Nessuna rilevanza\"";
        }
    }
}