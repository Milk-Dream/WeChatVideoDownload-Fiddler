using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fiddler视频号助手
{
    public partial class MyControl : UserControl
    {
        private int file_index = 1;
        public MyControl()
        {
            InitializeComponent();
        }

        public  void logg(string msg)
        {
            DateTime now = DateTime.Now;
            text_log.AppendText($"[{now.ToString("yyyy-MM-dd HH:mm:ss")}]"+msg+"\r\n");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if(!Path.IsPathRooted(text_save.Text))
            {
                logg(text_save+"是一个非法路径，请重新填写后再下载");
                return;
            }
            if(System.IO.Directory.Exists(text_save.Text))
            {
                System.IO.Directory.CreateDirectory(text_save.Text);
            }
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    item.Checked = false;
                    string url = item.SubItems[2].Text;
                    string fileName = item.SubItems[1].Text+".mp4"; // 下载保存的文件名
                    string fullPath = System.IO.Path.Combine(text_save.Text, fileName);
                    while(System.IO.File.Exists(fullPath))
                    {
                        fileName = item.SubItems[1].Text + "_" + file_index + ".mp4";
                        fullPath = System.IO.Path.Combine(text_save.Text, fileName);
                        file_index = file_index + 1;
                    }

                    // 创建WebClient实例
                    WebClient webClient = new WebClient();

                    // 注册事件处理程序
                    webClient.DownloadProgressChanged += (s, args) =>
                    {
                        UpdateListViewItemProgress(item, args.ProgressPercentage, args.BytesReceived, args.TotalBytesToReceive);
                    };
                    webClient.DownloadFileCompleted += (s, args) =>
                    {
                        if (args.Error != null)
                        {
                            logg($"{fileName}下载文件时发生错误：" + args.Error.Message);
                        }
                        else if (args.Cancelled)
                        {
                            logg($"{fileName}下载已取消");
                        }
                        else
                        {
                            logg($"{fileName}下载完成");
                        }

                        webClient.Dispose();
                    };

                    try
                    {
                        // 开始下载
                        webClient.DownloadFileAsync(new Uri(url), fullPath);
                    }
                    catch (Exception ex)
                    {
                        logg("下载文件时发生错误：" + ex.Message);
                        webClient.Dispose();
                    }
                }
            }
        }

        private void UpdateListViewItemProgress(ListViewItem item, int progressPercentage, long bytesReceived, long totalBytesToReceive)
        {
            // 更新进度信息
            item.SubItems[3].Text = $"{progressPercentage}%";
            
            //item.SubItems[4].Text = $"{bytesReceived / 1024}KB / {totalBytesToReceive / 1024}KB";
        }

        private void btn_openSave_Click(object sender, EventArgs e)
        {
            //打开保存目录
            System.Diagnostics.Process.Start("explorer.exe", text_save.Text);
        }

        private void btn_selectSave_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    text_save.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            // 遍历 ListView 的项
            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = listView1.Items[i];

                // 检查项中复选框的选中状态
                if (item.Checked)
                {
                    // 删除选中的项
                    listView1.Items.Remove(item);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listView1.Clear();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedRow = listView1.SelectedItems[0];
                string subItemText = selectedRow.SubItems[2].Text; // 假设要复制第二个子项的内容

                // 复制子项内容到剪贴板
                Clipboard.SetText(subItemText);
            }
        }

        // 自定义输入框对话框
        private string ShowInputDialog(string title, string prompt,string val)
        {
            Form inputForm = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label promptLabel = new Label() { Left = 20, Top = 20, Text = prompt };
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 20, Top = 50, Width = 250,Text=val};
            System.Windows.Forms.Button okButton = new System.Windows.Forms.Button() { Text = "确定", Left = 150, Width = 100, Top = 90 };
            System.Windows.Forms.Button cancelButton = new System.Windows.Forms.Button() { Text = "取消", Left = 20, Width = 100, Top = 90 };
            
            okButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.OK; };
            cancelButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.Cancel; };

            inputForm.Controls.Add(promptLabel);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(okButton);
            inputForm.Controls.Add(cancelButton);

            return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // 创建输入框并显示对话框
                string newContent = ShowInputDialog("请输入新标题:", "请输入新的标题", listView1.SelectedItems[0].SubItems[1].Text);

                if (!string.IsNullOrEmpty(newContent))
                {
                    // 修改ListView中选中行的指定子项内容
                    if (listView1.SelectedItems.Count > 0)
                    {
                        ListViewItem selectedRow = listView1.SelectedItems[0];
                        selectedRow.SubItems[1].Text = newContent; // 假设要修改第二个子项的内容
                    }
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                text_title.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                text_title.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void text_title_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].SubItems[1].Text = text_title.Text;
            }
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本软件由小白老师s开发,目前主攻前端","小白老师s提示你");
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本软件永久免费,思路由群里管理年少轻狂提供","小白老师s提示你");
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            listView1.Clear();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            // 遍历ListView的所有项
            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = listView1.Items[i];

                // 检查项是否被勾选
                if (item.Checked)
                {
                    // 如果项被勾选，从ListView中删除
                    listView1.Items.Remove(item);
                }
            }
        }

        private void ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            // 遍历ListView的所有项
            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = listView1.Items[i];

                // 检查项是否被勾选
                if (item.Checked)
                {
                    item.Checked = false;
                } else
                {
                    item.Checked = true;
                }
            }
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            // 遍历ListView的所有项
            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = listView1.Items[i];

                item.Checked = false;
            }
        }

        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            // 遍历ListView的所有项
            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = listView1.Items[i];

                item.Checked = true;
            }
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            //打开保存目录
            System.Diagnostics.Process.Start("explorer.exe", text_save.Text);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].Remove();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("已经复制1080p的github地址,请前往下载", "小白老师s提示你");
        }
    }
}
