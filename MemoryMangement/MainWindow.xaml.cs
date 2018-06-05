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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemoryMangement
{
    public struct CommandPage { public int pageNum; }

    public partial class MainWindow : Window
    {
        public int LackedNum = 0;                                       //缺页数

        private int commandChosenButtonModel = 1;                       //1时生成，2时显示

        public int alogrithmesModel = 0;                                //1表示FIFO；2表示LRU

        public int[] command = new int[320];                            //320条指令序列

        public int currentCommand = -1;                                  //当前指令在指令中的次序

        public CommandPage[] commandPages = new CommandPage[32];        //内存块个数

        public int[] currentBlocks = new int[4];                 //内存中的当前四个块中存储的页面ID

        private Queue<CommandPage> blocksForFIFO = new Queue<CommandPage>();   //为FIFO算法维护的队列，记录pages进入block的前后顺序，先出头，后出尾
        
        private int[] envokedNumsForLRU = new int[4];                   //为LRU算法维护的数组，记录当前块中各page距上次被调用的距离
        
        //初始化函数
        //用于启动和复位
        private void Init()
        {
            command[0] = -1;

            //清空数组
            //currentBlocks.Clear();
            
            currentCommand = -1;

            for (int i = 1; i <= 40; i++)
            {
                Label label = FindName("BLabel"+ Convert.ToString(i) ) as Label;
                label.Content = '-';
            }

            blocksForFIFO.Clear();
            envokedNumsForLRU[0] = -1;

            CurrentCommandText.Text = "-";
            NextCommandText.Text = "1";
            LackedNumText.Text = "0";
        }

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 32; i++)
            {
                commandPages[i].pageNum = i + 1;
            }

            Init();

        }

        private void CommandChosenButton_Click(object sender, RoutedEventArgs e)
        {
            if (commandChosenButtonModel == 1)
            //通过弹窗选择随机生成或手动选择
            //返回beginCommand值
            {
                Random rd = new Random();
                BeginCommandChosen form = new BeginCommandChosen();
                form.ShowDialog();
                command[0] = form._beginCommand;

                int flag = 1;

                for (int i = 1; i < 319; i++)
                {
                    if (flag == 1 || flag == 3)
                    {
                        command[i] = command[i - 1] + 1;
                        flag++;
                    }
                    else if (flag == 2)
                    {
                        command[i] = command[i - 1] + rd.Next(1, 320 - command[i - 1]);
                        flag++;
                    }
                    else if (flag == 4)
                    {
                        command[i] = command[i - 1] - rd.Next(1, command[i - 1]);
                        flag = 1;
                    }
                }

                //当生成指令队列后
                //该按钮应变成显示已生成队列
                commandChosenButtonModel = 2;
                
                CommandChosenButton.Content = "显示指令序列";
                
            }
            if (commandChosenButtonModel == 2)
                ShowCommand();
        }

        //展示页面
        private void ShowCommand()
        {
            for (int i = 0; i < 319; i++)
                Console.WriteLine(command[i]);
        }

        private bool IfExist(int BlockNum)
        {
            for(int i=0;i<currentBlocks.Count();i++)
            {
                if (currentBlocks[i] == BlockNum)
                    return true;
            }
            return false;
        }

        private void ShowAddress()
        {
            //地址块变色
            //地址栏返回地址

            int commandPageID = currentCommand / 10 + 1;

            int commandPageNumber = currentCommand - commandPageID * 10;
            
        }

        private void FIFO(int targetBlock)
        {
            CommandPage abandonedPage = blocksForFIFO.Dequeue();
            blocksForFIFO.Enqueue(commandPages[targetBlock]);

            for (int i = 0; i < 4; i++)
            {
                if (currentBlocks[i] == abandonedPage.pageNum)
                {
                    currentBlocks[i] = abandonedPage.pageNum;

                    Label label = FindName("Block" + Convert.ToString(i + 1))as Label;
                    label.Content = "Page" + Convert.ToString(targetBlock);
                }
                for (int j = 1; j <= 10; j++)
                {
                    Label label = FindName("Blabel" + Convert.ToString((i * 10) + j)) as Label;
                    label.Content = targetBlock * 10 + j;
                }

                envokedNumsForLRU[i] = 0;

                for (int j = 0; j < 4; j++)
                {
                    if (j == i) continue;

                    envokedNumsForLRU[j]++;
                }
            }
        }

        private void LRU(int targetPage)
        {
            int minNum = 321;
            int minBlock = 0;

            for(int i=0;i<4;i++)
            {
                if(envokedNumsForLRU[i]<minNum)
                {
                    minNum = envokedNumsForLRU[i];
                    minBlock = i;
                }
            }

            currentBlocks[minNum] = commandPages[targetPage].pageNum;

            Label labelP = FindName("Block" + Convert.ToString(minBlock + 1)) as Label;

            for(int i=1;i<=10;i++)
            {
                Label labelC = FindName("BLabel" + Convert.ToString(targetPage)) as Label;
                labelC.Content = Convert.ToString(targetPage * 10 + i);
            }
            envokedNumsForLRU[minNum] = 0;

            for(int i=0;i<4;i++)
            {
                if (i == minNum) continue;
                envokedNumsForLRU[i]++;
            }

        }

        private void SingleExecute_Click(object sender, RoutedEventArgs e)
        {
            if(commandChosenButtonModel == 1)
            {
                MessageBox.Show("请先生成指令组");
                return;
            }
            if(alogrithmesModel == 0 )
            {
                MessageBox.Show("请先选择置换算法");
                return;
            }

            currentCommand++;

            int blockID = command[currentCommand] / 10 - 1;

            //说明块没有满
            if (currentBlocks[3] == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (currentBlocks[i] == 0)
                    {
                        currentBlocks[i] = blockID;
                        Label labelB = FindName("Block" + Convert.ToString(i + 1)) as Label;
                        labelB.Content = Convert.ToString(blockID);

                        for (int j=0;j<10;j++)
                        {
                            Label labelC = FindName("BLabel" + Convert.ToString(i + j + 1)) as Label;
                            labelC.Content = Convert.ToString(blockID * 10 + j + 1);
                        }
                        break;
                    }
                }
                return;
            }

            if (!IfExist(blockID)) 
            {
                //不在内存
                //调用内存块并凸显新内存块
                //返回地址
                //BLabel 变色
                //缺页数++
                LackedNum++;
                LackedNumText.Text = Convert.ToString(LackedNum);

                //这里要区分FIFO和LRU
                //FIFO
                if (alogrithmesModel == 1)
                    FIFO(blockID);


                //LRU
                if (alogrithmesModel == 2)
                    LRU(blockID);
            }

            //else
            //{
                //已在内存
                //返回地址
                //BLabel 变色
            //}
            
            ShowAddress();
        }

        private void AllExecute_Click(object sender, RoutedEventArgs e)
        {
            if (commandChosenButtonModel == 1)
            {
                MessageBox.Show("请先生成指令组");
                return;
            }
            if (alogrithmesModel == 0)
            {
                MessageBox.Show("请先选择置换算法");
                return;
            }

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            alogrithmesModel = 1;
        }

        private void LRU_Button_Checked(object sender, RoutedEventArgs e)
        {
            alogrithmesModel = 2;
        }
    }
}
