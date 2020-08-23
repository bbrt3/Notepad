using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void noweOknoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // nowa instancja aplikacji z lekko zmienioną pozycją
            var x = new Form1();
            x.StartPosition = FormStartPosition.CenterParent;
            x.Show();
        }

        private void zakonczToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void powiekszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            powiekszTekst();
        }

        private void powiekszTekst()
        {
            richTextBox1.Font = new Font("Microsoft Sans Serif", richTextBox1.Font.Size + 1);
        }

        private void pomniejszTekst()
        {
            if (richTextBox1.Font.Size > 1)
            {
                var font = new Font("Microsoft Sans Serif", richTextBox1.Font.Size - 1);
                richTextBox1.Font = font;
            }
            else
            {
                MessageBox.Show("Font is too small to see anything!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void ZapiszPlik()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog1.FileName.ToString());
                file.WriteLine(richTextBox1.Text);
                file.Close();
            }
        }

        private void OtworzPlik()
        {
            // otwieranie pliku z przechwyceniem treści do richTextBoxa z użyciem streamreadera
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileStream = openFileDialog1.OpenFile();
                // osobna proccedura dla rozszerzen .doc i .docx
                if (Path.GetExtension(openFileDialog1.FileName) == ".doc" || Path.GetExtension(openFileDialog1.FileName) == ".docx")
                {
                    object readOnly = false;
                    object visible = true;
                    object save = false;
                    object fileName = openFileDialog1.FileName;
                    object newTemplate = false;
                    object docType = 0;
                    object missing = Type.Missing;
                    Microsoft.Office.Interop.Word.Document document;
                    Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application()
                    {
                        Visible = false
                    };
                    document = application.Documents.Open(ref fileName, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref visible, ref missing, ref missing, ref missing, ref missing);
                    document.ActiveWindow.Selection.WholeStory();
                    document.ActiveWindow.Selection.Copy();
                    var clipboardCopy = Clipboard.GetText();
                    IDataObject dataObject = Clipboard.GetDataObject();
                    richTextBox1.Rtf = dataObject.GetData(DataFormats.Rtf).ToString();
                    application.Quit(ref missing, ref missing, ref missing);
                    Clipboard.SetText(clipboardCopy);
                }
                else if (Path.GetExtension(openFileDialog1.FileName) == ".bmp" || Path.GetExtension(openFileDialog1.FileName) == ".jpg" || Path.GetExtension(openFileDialog1.FileName) == ".png" || Path.GetExtension(openFileDialog1.FileName) == ".gif" || Path.GetExtension(openFileDialog1.FileName) == ".ico")
                {
                    var clipboardCopy = Clipboard.GetDataObject();
                    Image img = Image.FromFile(openFileDialog1.FileName);
                    Clipboard.SetImage(img);
                    richTextBox1.Text = "";
                    richTextBox1.Paste();
                    Clipboard.SetDataObject(clipboardCopy);
                }

                else
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        var fileContents = reader.ReadToEnd();
                        richTextBox1.Text = fileContents;
                    }
                }
            }
        }

        private void DrukujPlik()
        {
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void notatnikInformacjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var x = new AboutForm
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            x.Show();
        }

        private void pomniejszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pomniejszTekst();
        }


        // skróty klawiszowe
        // alternatywa to form_keypreview
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch(keyData)
            {
                case Keys.Control | Keys.Oemplus:
                    powiekszTekst();
                    break;

                case Keys.Control | Keys.OemMinus:
                    pomniejszTekst();
                    break;
                case Keys.Control | Keys.S:
                    ZapiszPlik();
                    break;
                case Keys.Control | Keys.O:
                    OtworzPlik();
                    break;
                case Keys.Control | Keys.P:
                    DrukujPlik();
                    break;
                default:
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void otworzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OtworzPlik();
        }

        // drukowanko ezpz
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox1.Text, new Font(richTextBox1.Font.FontFamily, richTextBox1.Font.Size, FontStyle.Regular), Brushes.Black, new PointF(100, 100));
        }

        private void drukujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrukujPlik();
        }

        private void przywrocDomyslneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Font = new Font("Microsoft Sans Serif", 8);
        }

        private void usunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text.Substring(0, richTextBox1.SelectionStart) + richTextBox1.Text.Substring(richTextBox1.SelectionStart + richTextBox1.SelectionLength, richTextBox1.Text.Length - (richTextBox1.SelectionStart + richTextBox1.SelectedText.Length));
        }

        private void wytnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text.Substring(0, richTextBox1.SelectionStart) + richTextBox1.Text.Substring(richTextBox1.SelectionStart + richTextBox1.SelectionLength, richTextBox1.Text.Length - (richTextBox1.SelectionStart + richTextBox1.SelectedText.Length));
        }

        private void kopiujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text.Substring(richTextBox1.SelectionStart, richTextBox1.SelectionLength));
        }

        private void wklejToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text =  richTextBox1.Text.Insert(richTextBox1.SelectionStart,Clipboard.GetText());
        }

        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZapiszPlik();
        }

    }
}
