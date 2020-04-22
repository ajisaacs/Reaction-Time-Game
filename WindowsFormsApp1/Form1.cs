using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private BindingList<TestResults> results;

        public Form1()
        {
            InitializeComponent();

            results = new BindingList<TestResults>();
            results.ListChanged += Results_ListChanged;
            dataGridView1.DataSource = results;

            reactionTimeControl1.TestComplete += ReactionTimeControl1_TestComplete;
        }

        private void Results_ListChanged(object sender, ListChangedEventArgs e)
        {
            var best = results.Min(r => r.ReactionTimeMilliseconds);
            var worst = results.Max(r => r.ReactionTimeMilliseconds);
            var avgMs = results.Sum(r => r.ReactionTimeMilliseconds) / results.Count;
            label2.Text = $"Average: {avgMs} ms";
            label3.Text = $"Best: {best} ms";
            label4.Text = $"Worst: {worst} ms";
        }

        private void ReactionTimeControl1_TestComplete(object sender, TestResults e)
        {
            if (e.ReactionTimeMilliseconds == 0)
            {
                label1.Text = e.Message;
            }
            else
            {
                results.Add(e);
                label1.Text = $"Your reaction time was {e.ReactionTimeMilliseconds} ms";
            }
        }
    }
}
