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
        public int lackedNum = 0;                                       //缺页数

        private int commandChosenButtonModel = 1;                       //1时生成，2时显示

        public int alogrithmesModel = 0;                                //1表示FIFO；2表示LRU

        public int[] command = new int[320];                            //320条指令序列

        public int currentCommand = 0;                                  //当前指令在指令中的次序

        public int[] currentBlocks = new int[4];                        //内存中的当前四个块中存储的页面ID

        private Queue<int> blocksForFIFO = new Queue<int>();            //为FIFO算法维护的队列，记录pages进入block的前后顺序，先出头，后出尾
        
        private int[] envokedNumsForLRU = new int[4];                   //为LRU算法维护的数组，记录当前块中各page距上次被调用的距离

        public MainWindow()
        {
            InitializeComponent();

            Init();

        }

        //初始化函数
        //用于启动和复位
        private void Init()
        {
            //表示当前指令集无效
            command[0] = -1;

            //清空数组
            for (int i = 0; i < 4; i++)
            {
                currentBlocks[i] = -1;
            }

            currentCommand = 0;

            for (int i = 1; i <= 4; i++)
            {
                Label label = FindName("Block" + Convert.ToString(i)) as Label;
                label.Background = Brushes.LightGray;
                label.Content = "-";
            }

            for (int i = 1; i <= 40; i++)
            {
                Label label = FindName("BLabel" + Convert.ToString(i)) as Label;
                label.Content = '-';
                label.Background = Brushes.LightGray;
            }

            blocksForFIFO.Clear();
            for (int i = 0; i < 4; i++)
            {
                //-1表示i块目前无页面
                envokedNumsForLRU[i] = -1;
            }

            CurrentCommandText.Text = "-";
            CommandAddressText.Text = "-";
            NextCommandText.Text = "-";
            LackedNumText.Text = Convert.ToString(lackedNum);
        }

        private void UpdateMessage()
        {
            int currentCommandAddressBlock;
            int currentCommandAddressNo;

            int pageID = (command[currentCommand - 1] - 1) / 10 + 1;

            CurrentCommandText.Text = Convert.ToString(command[currentCommand - 1]);
            LackedNumText.Text = Convert.ToString(lackedNum);

            if (currentCommand == 320)
                NextCommandText.Text = "-";
            else
                NextCommandText.Text = Convert.ToString(command[currentCommand]);

            Label labelB, labelC;
            for (int i = 0; i < 4; i++)
            {
                if (currentBlocks[i] == -1) break;

                labelB = FindName("Block" + Convert.ToString(i + 1)) as Label;
                labelB.Content = "Page" + Convert.ToString(currentBlocks[i]);

                if (currentBlocks[i] == pageID)
                    labelB.Background = Brushes.Violet;
                else
                    labelB.Background = Brushes.White;

                for (int j = 0; j < 10; j++)
                {
                    labelC = FindName("BLabel" + Convert.ToString(i * 10 + j + 1)) as Label;
                    labelC.Content = Convert.ToString((currentBlocks[i] - 1) * 10 + j + 1);

                    if ((currentBlocks[i] - 1) * 10 + j + 1 == command[currentCommand - 1])
                    {
                        labelC.Background = Brushes.DarkViolet;
                        currentCommandAddressBlock = i + 1;
                        currentCommandAddressNo = j + 1;

                        CommandAddressText.Text = "Block" + Convert.ToString(currentCommandAddressBlock) +
                            "   NO." + Convert.ToString(currentCommandAddressNo);
                    }
                    else
                        labelC.Background = Brushes.LightGray;
                }
            }
        }

        private bool IfExist(int TargetPage)
        {
            for (int i = 0; i < 4; i++)
            {
                if (currentBlocks[i] == TargetPage)
                    return true;
            }
            return false;
        }

        private void FinalCommand()
        {
            LackedRate form = new LackedRate(lackedNum);
            form.ShowDialog();
        }

        private void FIFO(int targetPage)
        {
            int abandonedPage = blocksForFIFO.Dequeue();
            blocksForFIFO.Enqueue(targetPage);

            for (int i = 0; i < 4; i++)
            {
                if (currentBlocks[i] == abandonedPage)
                {
                    currentBlocks[i] = targetPage;
                    envokedNumsForLRU[i] = 0;
                }

                for (int j = 0; j < 4; j++)
                {
                    if (j == i) continue;
                    envokedNumsForLRU[j]++;
                }
            }
        }

        private void LRU(int targetPage)
        {
            int maxNum = 0;
            int maxBlock = -1;

            for (int i = 0; i < 4; i++)
            {
                if (envokedNumsForLRU[i] >= maxNum)
                {
                    maxNum = envokedNumsForLRU[i];
                    maxBlock = i;
                }
            }

            currentBlocks[maxBlock] = targetPage;
            envokedNumsForLRU[maxBlock] = 0;

            for (int i = 0; i < 4; i++)
            {
                if (i == maxNum) continue;
                envokedNumsForLRU[i]++;
            }

            blocksForFIFO.Dequeue();
            blocksForFIFO.Enqueue(targetPage);
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

                for (int i = 1; i < 320; i++)
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

                return;
            }
            //展示页面
            if (commandChosenButtonModel == 2)
            {
                ShowCommand form = new ShowCommand(command);
                form.ShowDialog();
            }
        }

        private void SingleExecute_Click(object sender, RoutedEventArgs e)
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

            if (currentCommand == 320)
            {
                FinalCommand();
                return;
            }
            currentCommand++;

            int pageID = (command[currentCommand - 1] - 1) / 10 + 1;

            if (IfExist(pageID))
            {
                UpdateMessage();

                if (currentCommand == 320)
                    FinalCommand();

                return;
            }

            else
            {
                //说明内存块还没有满
                //新调用页而不是置换
                if (currentBlocks[3] == -1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (currentBlocks[i] == -1)
                        {
                            currentBlocks[i] = pageID;
                            blocksForFIFO.Enqueue(pageID);
                            envokedNumsForLRU[i] = 0;
                            break;
                        }
                    }
                    UpdateMessage();
                }

                //说明内存已满并且没有所需内存
                //需要置换内存块
                //调用内存块并凸显新内存块
                //返回地址
                //BLabel 变色
                //缺页数++
                else
                {
                    lackedNum++;

                    //这里要区分FIFO和LRU
                    //FIFO
                    if (alogrithmesModel == 1)
                        FIFO(pageID);

                    //LRU
                    if (alogrithmesModel == 2)
                        LRU(pageID);

                    UpdateMessage();
                }
            }

            if (currentCommand == 320)
                FinalCommand();
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

            for (int i = currentCommand; i < 320; i++)
            {
                SingleExecute_Click(sender, e);
            }
        }
        
        private void FIFO_Button_Checked(object sender, RoutedEventArgs e)
        {
            alogrithmesModel = 1;
        }

        private void LRU_Button_Checked(object sender, RoutedEventArgs e)
        {
            alogrithmesModel = 2;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }
    }
}
