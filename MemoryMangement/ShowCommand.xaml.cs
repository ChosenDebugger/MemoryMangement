using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryMangement
{
    public partial class ShowCommand : Window
    {
        public ShowCommand(int[] command)
        {
            InitializeComponent();
            
            for(int i=0;i<320;i++)
            {
                CommandShowText.Text += "NO." + Convert.ToString(i) + ":    command " +
                    Convert.ToString(command[i]) + "\n";
            }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
