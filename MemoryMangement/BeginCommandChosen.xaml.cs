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
    /// <summary>
    /// BeginCommandChosen.xaml 的交互逻辑
    /// </summary>
    public partial class BeginCommandChosen : Window
    {
        public int _beginCommand = 999;
        public int createModel = 0;     //1表示随机生成 //2表示手动输入

        public BeginCommandChosen()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            createModel = 1; 
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            createModel = 2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //bug
            //没有选择应弹窗后跳回选择界面
            //而不是关闭
            if (createModel == 0)
                MessageBox.Show("请先选择生成方式");

            else if (createModel == 1)
            {
                Random rd = new Random();
                _beginCommand = rd.Next(1, 320);
            }

            else if (createModel == 2) 
            {
                _beginCommand = Convert.ToInt32(SpecifiedBeginCommand.Text);
            }

            Close();
        }
    }
}
