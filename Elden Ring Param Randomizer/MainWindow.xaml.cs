using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Elden_Ring_Param_Randomizer.Resources;
using Elden_Ring_Param_Randomizer.Settings;
using Microsoft.Win32;
using SoulsFormats;

namespace Elden_Ring_Param_Randomizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private string RegulationPath { get; set; } = "";

        public float MaxWeaponWeight { get; set; } = 1000.0F;

        public int[] WeaponRequirement { get; set; } = [99, 99, 99, 99, 99];

        public MainWindow()
        {
            InitializeComponent();
            Title = $"{Strings.Elden_Ring_Param_Randomizer} {Strings.Version}";
            UpdateConsole(Strings.Waiting_for_regulation);
            Browse.Content = Strings.Browse;
            Randomize.Content = Strings.Randomize;
            Description.Text = Strings.Right_Click_CheckBox_to_Setting;
            TalkParamMsgId.Content = Strings.Talk_Param_Message_;
            EquipParamWeaponWeight.Content = Strings.EquipParamWeapon_Weight_;
            EquipParamWeaponRequirement.Content = Strings.EquipParamWeapon_Requirement_;
        }

        public void UpdateConsole(string text)
        {
            Console.Text = text;
            Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
        }

        private void Browse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = Strings.OpenFileDialogTitle,
                Filter = "Regulation File|regulation.bin|All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string? directory = Path.GetDirectoryName(openFileDialog.FileName);
                if (directory != null && File.Exists(directory + "\\eldenring.exe"))
                {
                    MessageBoxResult result =
                        MessageBox.Show(Strings.SameFolder, Strings.Error, MessageBoxButton.OKCancel);
                    if (result != MessageBoxResult.OK)
                    {
                        openFileDialog.FileName = "";
                        return;
                    }
                }

                RegulationPath = openFileDialog.FileName;
                UpdateConsole(Strings.File_Selected);
            }
        }

        private void Randomize_OnClick(object sender, RoutedEventArgs e)
        {
            if (RegulationPath == "")
            {
                MessageBox.Show(Strings.NoFileSelected, Strings.Error);
                return;
            }

            if (TalkParamMsgId.IsChecked == false
                && EquipParamWeaponWeight.IsChecked == false
                && EquipParamWeaponRequirement.IsChecked == false)
            {
                MessageBox.Show(Strings.NoParamSelected, Strings.Error);
                return;
            }

            int rngSeed = (int)Seed.Value;
            if (rngSeed == -1)
            {
                Random newSeed = new Random();
                Seed.Value = newSeed.Next(0, 2147483647);
                rngSeed = (int)Seed.Value;
            }

            Random rng = new(rngSeed);

            UpdateConsole(Strings.Decrypting_Regulation);

            BND4 paramBnd = SFUtil.DecryptERRegulation(RegulationPath);

            if (TalkParamMsgId.IsChecked == true) paramBnd = randTalk(rng, paramBnd);
            if (EquipParamWeaponWeight.IsChecked == true) paramBnd = randWeaponWeight(rng, paramBnd);
            if (EquipParamWeaponRequirement.IsChecked == true) paramBnd = randWeaponRequirement(rng, paramBnd);

            SFUtil.EncryptERRegulation(RegulationPath, paramBnd);

            GC.Collect();

            UpdateConsole(Strings.Finished);
            System.Media.SystemSounds.Exclamation.Play();
            MessageBox.Show(Strings.All_done, Strings.Randomization_Finished);
        }

        private BND4 randWeaponRequirement(Random rng, BND4 paramBnd)
        {
            Dictionary<string, PARAM> paramList = new();

            UpdateConsole(Strings.Loading_ParamDefs);

            List<PARAMDEF> paramdefs = new List<PARAMDEF>();
            PARAMDEF paramdef = PARAMDEF.XmlDeserialize($@"{Directory.GetCurrentDirectory()}\Paramdex\EquipParamWeapon.xml");
            paramdefs.Add(paramdef);

            UpdateConsole(Strings.Handling_Params);

            foreach (BinderFile file in paramBnd.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                var param = PARAM.Read(file.Bytes);

                if (param.ApplyParamdefCarefully(paramdefs))
                    paramList[name] = param;
            }

            UpdateConsole(Strings.Modifying_Params);

            PARAM weaponParam = paramList["EquipParamWeapon"];

            for (int i = 0; i < weaponParam.Rows.Count; i++)
            {
                PARAM.Row row = weaponParam.Rows[i];

                if ((int)row["sortId"].Value == 9999999)
                {
                    continue;
                }

                row["properStrength"].Value = rng.Next(0, WeaponRequirement[0] + 1);
                row["properAgility"].Value = rng.Next(0, WeaponRequirement[1] + 1);
                row["properMagic"].Value = rng.Next(0, WeaponRequirement[2] + 1);
                row["properFaith"].Value = rng.Next(0, WeaponRequirement[3] + 1);
                row["properLuck"].Value = rng.Next(0, WeaponRequirement[4] + 1);
            }

            UpdateConsole(Strings.Exporting_Params);

            foreach (BinderFile file in paramBnd.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                if (paramList.ContainsKey(name))
                    file.Bytes = paramList[name].Write();
            }

            return paramBnd;
        }

        private BND4 randWeaponWeight(Random rng, BND4 paramBnd)
        {
            Dictionary<string, PARAM> paramList = new();

            UpdateConsole(Strings.Loading_ParamDefs);

            List<PARAMDEF> paramDefs = new List<PARAMDEF>();
            PARAMDEF paramDef = PARAMDEF.XmlDeserialize($@"{Directory.GetCurrentDirectory()}\Paramdex\EquipParamWeapon.xml");
            paramDefs.Add(paramDef);

            UpdateConsole(Strings.Handling_Params);

            foreach (BinderFile file in paramBnd.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                var param = PARAM.Read(file.Bytes);

                if (param.ApplyParamdefCarefully(paramDefs))
                    paramList[name] = param;
            }

            UpdateConsole(Strings.Modifying_Params);

            PARAM weaponParam = paramList["EquipParamWeapon"];

            for (int i = 0; i < weaponParam.Rows.Count; i++)
            {
                PARAM.Row row = weaponParam.Rows[i];

                if ((int)row["sortId"].Value == 9999999)
                {
                    continue;
                }

                row["weight"].Value = rng.Next(0, (int)(MaxWeaponWeight * 10) + 1) / 10.0;
            }

            UpdateConsole(Strings.Exporting_Params);

            foreach (BinderFile file in paramBnd.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                if (paramList.ContainsKey(name))
                    file.Bytes = paramList[name].Write();
            }

            return paramBnd;
        }

        private BND4 randTalk(Random rng, BND4 paramBnd)
        {
            Dictionary<string, PARAM> paramList = new();

            UpdateConsole(Strings.Loading_ParamDefs);

            List<PARAMDEF> paramDefs = new List<PARAMDEF>();
            PARAMDEF paramDef = PARAMDEF.XmlDeserialize($@"{Directory.GetCurrentDirectory()}\Paramdex\TalkParam.xml");
            paramDefs.Add(paramDef);

            UpdateConsole(Strings.Handling_Params);

            foreach (BinderFile file in paramBnd.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                PARAM param = PARAM.Read(file.Bytes);

                if (param.ApplyParamdefCarefully(paramDefs))
                {
                    paramList[name] = param;
                }
            }

            UpdateConsole(Strings.Modifying_Params);

            PARAM talkParam = paramList["TalkParam"];

            List<int> goodIds = new();
            for (int i = 0; i < talkParam.Rows.Count; i++)
            {
                PARAM.Row row = talkParam.Rows[i];

                if (row.ID is 100 or 200)
                {
                    continue;
                }

                goodIds.Add((int)row["msgId"].Value);
                goodIds.Add((int)row["msgId_female"].Value);

                row["msgId"].Value = -1;
                row["msgId_female"].Value = -1;
            }

            for (int i = 0; i < talkParam.Rows.Count; i++)
            {
                PARAM.Row row = talkParam.Rows[i];

                if (row.ID is 100 or 200)
                {
                    continue;
                }

                row["msgId"].Value = goodIds[rng.Next(0, goodIds.Count)];
                row["msgId_female"].Value = goodIds[rng.Next(0, goodIds.Count)];
            }

            UpdateConsole(Strings.Exporting_Params);

            foreach (BinderFile file in paramBnd.Files)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                if (paramList.ContainsKey(name))
                {
                    file.Bytes = paramList[name].Write();
                }
            }

            return paramBnd;
        }

        private void TalkParamMsgId_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(Strings.No_Setting, Strings.Error);
        }

        private void EquipParamWeaponWeight_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WeaponWeight weaponWeight = new WeaponWeight();
            if (weaponWeight.ShowDialog() == true)
            {
                MaxWeaponWeight = weaponWeight.UserInput;
            }
        }

        private void EquipParamWeaponRequirement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WeaponRequirement requirement = new WeaponRequirement();
            if (requirement.ShowDialog() == true)
            {
                WeaponRequirement = requirement.Requirement;
            }
        }
    }
}