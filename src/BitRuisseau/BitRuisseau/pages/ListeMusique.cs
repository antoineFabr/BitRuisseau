namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Load_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = fbd.SelectedPath;

                    string[] extensions = { ".mp3", ".wav" };

                    // Liste des fichiers correspondant
                    var files = Directory.GetFiles(selectedPath, "*.*", SearchOption.AllDirectories)
                                         .Where(file => extensions.Contains(Path.GetExtension(file).ToLower())).ToList();

                    // Vide la liste avant de la remplir
                    ListeSong.Items.Clear();

                   
                    files.ForEach(x => ListeSong.Items.Add(Path.GetFileName(x)));
                    
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}