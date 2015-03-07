using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Kodi_M3U_IPTV_Editor
{
    public partial class Editor : Form
    {
    
           public List<string> data = new List<string>();
            
        public string fileName = "", line;
        private int channelNum = 0;
        public StreamReader playlistFile;
        public SortableBindingList<Channel> channels = new SortableBindingList<Channel>();
    

        public Editor()
        {
            InitializeComponent();
           
        }
        public SortableBindingList<Channel> GetList()
        {
            return channels;
        }

        private void openPlaylist(object sender, EventArgs e)
        {
            alertSave();
            openFile.ShowDialog();
        }

        
        public void importM3U()
        {
           
         //  bool image = false;

          


            while ((line = playlistFile.ReadLine()) != null)
            {
                
                if (line.StartsWith("#EXTINF"))
                {
                    
                    //this one is for getting group-title data[0]
                    string between = textoperations.getBetween(line, "group-title=\"", "\"");
                    data.Add(between);
                     //this is the name of the channel... data[1]= rubbish data[2]=channel name 
                    data.AddRange(line.Substring(8).Split(','));
                    continue;
                }
                
                    
                else if (line.Contains("//"))
                {
                  //this is the url ...data[3]
                   data.Add(line);
                   
                }

                if (data.Count == 3)
                {
                    try
                    {
                        channels.Add(new Channel(channelNum, "Not Available", data[2].Trim(), data[3].Trim()));//, data[4].Trim(),data[6].Trim(), data[5].Trim()* 

                    }
                    catch (System.ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("A channel has been omitted due to its lack of url or its incorrect format");
                            continue;
                        }
                }
               

            if (data.Count == 4)
                {


                    channels.Add(new Channel(channelNum, data[0].Trim(), data[2].Trim(), data[3].Trim()));//, data[4].Trim(),data[6].Trim(), data[5].Trim()* 
                
                } 

               else if (data.Count == 4)
                {

                    channels.Add(new Channel(channelNum, data[0].Trim(), data[2].Trim(), data[3].Trim()));//, data[4].Trim(),data[6].Trim(), data[5].Trim()));
                  //  image = true;
                }
                
                data.Clear();
            }
            playlistFile.Close();

            if (channels.Count == 0)
            {
                MessageBox.Show("Selected file has incorrect structure! Please open an appropriate file.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                splitContainer1.Panel2Collapsed = false; enableEditing();
             
            }
        }

       

        private void enableEditing()
        {
            saveToolStripMenuItem.Enabled = true;
           // exportToolStripMenuItem.Enabled = true;
            toolStripSave.Enabled = true;

            toolStripNew.Enabled = true;
            toolStripRemove.Enabled = true;
           // this.channelsGrid.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxNumericInput(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void toolStripNew_Click(object sender, EventArgs e)
        {
            channels.Add(new Channel(channelNum, "","New Channel", "" ));
            channelsGrid.Rows[channels.Count - 1].Selected = true;
            channelsGrid.FirstDisplayedScrollingRowIndex = channels.Count - 1;
        }

        public void channelsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (channelsGrid.SelectedRows.Count == 0)
                return;

            int selectedRow = channelsGrid.SelectedRows[0].Index;
           /* if (selectedRow != 0)
                toolStripMoveUp.Enabled = true;
            else
                toolStripMoveUp.Enabled = false;
            if (channelsGrid.RowCount > 1 && selectedRow == channelsGrid.RowCount - 1)
                toolStripMoveDown.Enabled = false;
            else
                toolStripMoveDown.Enabled = true;
            */
            toolStripDuplicate.Enabled = true;
 
            channelName.Text = channels[selectedRow].Name;
            channelTags.Text = channels[selectedRow].Group;
        
            stream.Text = channels[selectedRow].IP;
            
           // channelEPG.Text = channels[selectedRow].EPG;
            //channelImage.Text = channels[selectedRow].Image;
            buttonSubmit.Enabled = true;
            string uri = stream.Text;
        }

        private void toolStripMoveUp_Click(object sender, EventArgs e)
        {
            if (channelsGrid.SelectedRows.Count == 0)
                return;

            int selectedRow = channelsGrid.SelectedRows[0].Index;
            Channel current = channels[selectedRow];
            Channel previous = channels[selectedRow - 1];
            channels.RemoveAt(selectedRow);
         
            channels.Insert(selectedRow-=1, current);
            channelsGrid.Rows[selectedRow].Selected = true;
        }

        private void toolStripMoveDown_Click(object sender, EventArgs e)
        {
            if (channelsGrid.SelectedRows.Count == 0)
                return;

            int selectedRow = channelsGrid.SelectedRows[0].Index;
            Channel current = channels[selectedRow];
            Channel next = channels[selectedRow + 1];
            channels.RemoveAt(selectedRow);
         
            channels.Insert(selectedRow+=1, current);
            channelsGrid.Rows[selectedRow].Selected = true;
        }
       
        private void toolStripDuplicate_Click(object sender, EventArgs e)
        {
            if (channelsGrid.SelectedRows.Count == 0)
                return;

            int selectedRow = channelsGrid.SelectedRows[0].Index;
            Channel current = channels[selectedRow++];
            channels.Insert(selectedRow, current);
            channelsGrid.Rows[selectedRow].Selected = true;
        }

        private void toolStripRemove_Click(object sender, EventArgs e)
        {
            if (channelsGrid.SelectedRows.Count == 0)
                return;

            int selectedRow = channelsGrid.SelectedRows[0].Index;

            if (channels.Count > 1 && selectedRow-1 > 0)
                channelsGrid.Rows[selectedRow - 1].Selected = true;
            else if (channels.Count > 1 && selectedRow+1 <= channels.Count-1)
                channelsGrid.Rows[selectedRow + 1].Selected = true;

            channels.RemoveAt(selectedRow);

            if (channels.Count == 0)
                buttonSubmit.Enabled = false;
        }


        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (channelsGrid.SelectedRows.Count == 0)
                return;

            //int id;
            if (/*channelID.Text.Trim().Length > 0 && int.TryParse(channelID.Text.Trim(), out id) && */ channelName.Text.Trim().Length > 0 && stream.Text.Trim().Length > 0)
            {
                int selectedRow = channelsGrid.SelectedRows[0].Index;
     
                channels[selectedRow].Name = channelName.Text;
                channels[selectedRow].Group = channelTags.Text;

                channels[selectedRow].IP = stream.Text;
                string uri = stream.Text;
              //  channels[selectedRow].EPG = channelEPG.Text;
                //channels[selectedRow].Image = channelImage.Text;
            }
            else
            {
                MessageBox.Show("A name and stream URL is required", "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void savePlaylist(object sender, EventArgs e)
        {
            saveFile.ShowDialog();
        }

        private void saveM3U()
        {
            StreamWriter file = new StreamWriter(saveFile.FileName, false, Encoding.UTF8);
            file.WriteLine("#EXTM3U");
         //   file.WriteLine("#EXTNAME:" + fileName);
            file.WriteLine();
            for (int i = 0; i < channels.Count; i++)
            {
                file.WriteLine("#EXTINF:" + "0" + " group-title=\"" + channels[i].Group + "\"" + "," + channels[i].Name);
                //file.WriteLine("#EXTTV:" + channels[i].Tag.Replace(",", "-") + ";" + channels[i].Language + ";" + channels[i].EPG + ((channels[i].Image.Trim().Length>0) ? ";"+channels[i].Image : ""));
                file.WriteLine(channels[i].IP );
                file.WriteLine();
            }
            file.Close();
        }

       

        private void openURL(object sender, EventArgs e)
        {
            alertSave();
            OpenURL url = new OpenURL();
            if (url.ShowDialog() != DialogResult.OK)
                return;

            fileName = Path.GetFileNameWithoutExtension(url.filePath);
            playlistFile = new StreamReader(url.filePath);
            channels.Clear();
            channelsGrid.AutoGenerateColumns = false;
            channelsGrid.DataSource = channels;
            if (playlistFile.ReadLine().StartsWith("#EXTM3U"))
                importM3U();
          
             
        }

        private void alertSave()
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            DialogResult dialogSave = MessageBox.Show("Do you want to save your current playlist?", "Save Playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogSave == DialogResult.Yes)
                saveFile.ShowDialog();
        }

       

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about1 about = new about1();
            about.Show();
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            alertSave();
        }

        private void openFile_FileOk(object sender, CancelEventArgs e)
        {
            fileName = Path.GetFileNameWithoutExtension(openFile.FileName);
            playlistFile = new StreamReader(openFile.FileName);
            channels.Clear();
            channelsGrid.DataSource = channels;
            switch (Path.GetExtension(openFile.FileName))
            {
                case ".m3u":
                    importM3U();
                    break;
               
            }
        }

        private void saveFile_FileOk(object sender, CancelEventArgs e)
        {
            switch (Path.GetExtension(saveFile.FileName))
            {
                case ".m3u":
                    saveM3U();
                    break;
               
            }
        }

        private void channelsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Editor_Load(object sender, EventArgs e)
        {

        }

        private void channelName_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void newListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            alertSave();
            enableEditing();
            channelsGrid.DataSource = channels;
            data.Clear();
            channels.Clear();
            data.Add("");
            data.Add("");
            data.Add("");
            data.Add("");
            channels.Add(new Channel(channelNum, data[0].Trim(), data[2].Trim(), data[3].Trim()));//, data[4].Trim(),data[6].Trim(), data[5].Trim()* 
 
            

            

            splitContainer1.Panel2Collapsed = false; enableEditing();
       //     channelsGrid.Rows[channels.Count - 1].Selected = true;
            //channelsGrid.FirstDisplayedScrollingRowIndex = channels.Count - 1;
            //channels.Add(new Channel(channelNum, data[0].Trim(), data[2].Trim(), data[3].Trim(), "Not-available"));//, data[4].Trim(),data[6].Trim(), data[5].Trim()* 
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            alertSave();
            enableEditing();
            channelsGrid.DataSource = channels;
            data.Clear();
            channels.Clear();
            data.Add("New Channel");
            data.Add("");
            data.Add("");
            data.Add("");
            channels.Add(new Channel(channelNum, data[0].Trim(), data[2].Trim(), data[3].Trim()));//, data[4].Trim(),data[6].Trim(), data[5].Trim()* 
 
            
        }

        private void axVLCPlugin21_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            axVLCPlugin21.playlist.items.clear();
            axVLCPlugin21.playlist.add(stream.Text, channelName.Text, "--brightness=2");
            axVLCPlugin21.playlist.play();
        }

        private void addAListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            alertSave();
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            fileName = Path.GetFileNameWithoutExtension(openFile.FileName);
            playlistFile = new StreamReader(openFile.FileName);
            channelsGrid.DataSource = channels;
            switch (Path.GetExtension(openFile.FileName))
            {
                case ".m3u":
                    importM3U();
                    break;

            }
        }

       
    }
}
