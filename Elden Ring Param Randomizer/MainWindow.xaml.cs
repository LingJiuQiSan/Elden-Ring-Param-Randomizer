using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Elden_Ring_Param_Randomizer.Resources;
using Elden_Ring_Param_Randomizer.Settings;
using Microsoft.Win32;
using SoulsFormats;

namespace Elden_Ring_Param_Randomizer;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
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
        EquipParamWeaponScaling.Content = Strings.EquipParamWeapon_Scaling_;
        EquipParamWeaponDamage.Content = Strings.EquipParamWeapon_AttackBase_;
    }

    private string RegulationPath { get; set; } = "";

    private float MaxWeaponWeight { get; set; } = 1000.0F;
    private bool HeavierWeaponSmallerProbability { get; set; } = true;

    private int[] WeaponRequirement { get; set; } = [99, 99, 99, 99, 99];

    private float[] WeaponCorrect { get; set; } = [200.0F, 200.0F, 200.0F, 200.0F, 200.0F];

    private int[] WeaponBaseDamage { get; set; } = [1000, 1000, 1000, 1000, 1000];

    private void UpdateConsole(string text)
    {
        Console.Text = text;
        Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = Strings.OpenFileDialogTitle,
            Filter = "Regulation File|regulation.bin|All Files|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            var directory = Path.GetDirectoryName(openFileDialog.FileName);
            if (directory != null && File.Exists(directory + "\\eldenring.exe"))
            {
                var result =
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
            && EquipParamWeaponRequirement.IsChecked == false
            && EquipParamWeaponScaling.IsChecked == false)
        {
            MessageBox.Show(Strings.NoParamSelected, Strings.Error);
            return;
        }

        var rngSeed = (int)Seed.Value!;
        if (rngSeed == -1)
        {
            var newSeed = new Random();
            Seed.Value = newSeed.Next(0, 2147483647);
            rngSeed = (int)Seed.Value;
        }

        Random rng = new(rngSeed);

        UpdateConsole(Strings.Decrypting_Regulation);

        var paramBnd = SFUtil.DecryptERRegulation(RegulationPath);

        if (TalkParamMsgId.IsChecked == true) paramBnd = RandTalk(rng, paramBnd);
        if (EquipParamWeaponWeight.IsChecked == true) paramBnd = RandWeaponWeight(rng, paramBnd);
        if (EquipParamWeaponRequirement.IsChecked == true) paramBnd = RandWeaponRequirement(rng, paramBnd);
        if (EquipParamWeaponScaling.IsChecked == true) paramBnd = RandWeaponCorrect(rng, paramBnd);

        SFUtil.EncryptERRegulation(RegulationPath, paramBnd);

        GC.Collect();

        UpdateConsole(Strings.Finished);
        SystemSounds.Exclamation.Play();
        MessageBox.Show(Strings.All_done, Strings.Randomization_Finished);
    }

    private BND4 RandWeaponCorrect(Random rng, BND4 paramBnd)
    {
        Dictionary<string, PARAM> paramList = new();

        UpdateConsole(Strings.Loading_ParamDefs);

        var paramDefs = new List<PARAMDEF>();
        var paramDef =
            PARAMDEF.XmlDeserialize($@"{Directory.GetCurrentDirectory()}\Paramdex\EquipParamWeapon.xml");
        paramDefs.Add(paramDef);

        UpdateConsole(Strings.Handling_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            var param = PARAM.Read(file.Bytes);

            if (param.ApplyParamdefCarefully(paramDefs)) paramList[name] = param;
        }

        UpdateConsole(Strings.Modifying_Params);

        var weaponParam = paramList["EquipParamWeapon"];

        foreach (var row in weaponParam.Rows)
        {
            if ((int)row["sortId"].Value == 9999999) continue;

            row["correctStrength"].Value = rng.Next(0, (int)(WeaponCorrect[0] * 10) + 1) / 10;
            row["correctAgility"].Value = rng.Next(0, (int)(WeaponCorrect[1] * 10) + 1) / 10;
            row["correctMagic"].Value = rng.Next(0, (int)(WeaponCorrect[2] * 10) + 1) / 10;
            row["correctFaith"].Value = rng.Next(0, (int)(WeaponCorrect[3] * 10) + 1) / 10;
            row["correctLuck"].Value = rng.Next(0, (int)(WeaponCorrect[4] * 10) + 1) / 10;
        }

        UpdateConsole(Strings.Exporting_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            if (paramList.ContainsKey(name))
                file.Bytes = paramList[name].Write();
        }

        return paramBnd;
    }

    private BND4 RandWeaponRequirement(Random rng, BND4 paramBnd)
    {
        Dictionary<string, PARAM> paramList = new();

        UpdateConsole(Strings.Loading_ParamDefs);

        var paramDefs = new List<PARAMDEF>();
        var paramDef =
            PARAMDEF.XmlDeserialize($@"{Directory.GetCurrentDirectory()}\Paramdex\EquipParamWeapon.xml");
        paramDefs.Add(paramDef);

        UpdateConsole(Strings.Handling_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            var param = PARAM.Read(file.Bytes);

            if (param.ApplyParamdefCarefully(paramDefs))
                paramList[name] = param;
        }

        UpdateConsole(Strings.Modifying_Params);

        var weaponParam = paramList["EquipParamWeapon"];

        foreach (var row in weaponParam.Rows)
        {
            if ((int)row["sortId"].Value == 9999999) continue;

            row["properStrength"].Value = rng.Next(0, WeaponRequirement[0] + 1);
            row["properAgility"].Value = rng.Next(0, WeaponRequirement[1] + 1);
            row["properMagic"].Value = rng.Next(0, WeaponRequirement[2] + 1);
            row["properFaith"].Value = rng.Next(0, WeaponRequirement[3] + 1);
            row["properLuck"].Value = rng.Next(0, WeaponRequirement[4] + 1);
        }

        UpdateConsole(Strings.Exporting_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            if (paramList.ContainsKey(name))
                file.Bytes = paramList[name].Write();
        }

        return paramBnd;
    }

    private BND4 RandWeaponWeight(Random rng, BND4 paramBnd)
    {
        Dictionary<string, PARAM> paramList = new();

        UpdateConsole(Strings.Loading_ParamDefs);

        var paramDefs = new List<PARAMDEF>();
        var paramDef =
            PARAMDEF.XmlDeserialize($@"{Directory.GetCurrentDirectory()}\Paramdex\EquipParamWeapon.xml");
        paramDefs.Add(paramDef);

        UpdateConsole(Strings.Handling_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            var param = PARAM.Read(file.Bytes);

            if (param.ApplyParamdefCarefully(paramDefs))
                paramList[name] = param;
        }

        UpdateConsole(Strings.Modifying_Params);

        var weaponParam = paramList["EquipParamWeapon"];

        foreach (var row in weaponParam.Rows)
        {
            if ((int)row["sortId"].Value == 9999999) continue;

            row["weight"].Value = HeavierWeaponSmallerProbability
                ? rng.Next(0, (int)(MaxWeaponWeight * 10) + 1) / 10.0
                : (int)Utils.GetExponentiallyDistributedRandom((int)(MaxWeaponWeight * 10)) / 10.0;
        }

        UpdateConsole(Strings.Exporting_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            if (paramList.ContainsKey(name))
                file.Bytes = paramList[name].Write();
        }

        return paramBnd;
    }

    private BND4 RandTalk(Random rng, BND4 paramBnd)
    {
        Dictionary<string, PARAM> paramList = new();

        UpdateConsole(Strings.Loading_ParamDefs);

        var paramDefs = new List<PARAMDEF>();
        var paramDef = PARAMDEF.XmlDeserialize($@"{Directory.GetCurrentDirectory()}\Paramdex\TalkParam.xml");
        paramDefs.Add(paramDef);

        UpdateConsole(Strings.Handling_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            var param = PARAM.Read(file.Bytes);

            if (param.ApplyParamdefCarefully(paramDefs)) paramList[name] = param;
        }

        UpdateConsole(Strings.Modifying_Params);

        var talkParam = paramList["TalkParam"];

        List<int> goodIds = new();
        foreach (var row in talkParam.Rows)
        {
            if (row.ID is 100 or 200) continue;

            goodIds.Add((int)row["msgId"].Value);
            goodIds.Add((int)row["msgId_female"].Value);

            row["msgId"].Value = -1;
            row["msgId_female"].Value = -1;
        }

        foreach (var row in talkParam.Rows)
        {
            if (row.ID is 100 or 200) continue;

            row["msgId"].Value = goodIds[rng.Next(0, goodIds.Count)];
            row["msgId_female"].Value = goodIds[rng.Next(0, goodIds.Count)];
        }

        UpdateConsole(Strings.Exporting_Params);

        foreach (var file in paramBnd.Files)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            if (paramList.ContainsKey(name)) file.Bytes = paramList[name].Write();
        }

        return paramBnd;
    }

    private void TalkParamMsgId_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        MessageBox.Show(Strings.No_Setting, Strings.Error);
    }

    private void EquipParamWeaponWeight_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var weaponWeight = new WeaponWeight(MaxWeaponWeight, HeavierWeaponSmallerProbability);
        if (weaponWeight.ShowDialog() == true)
        {
            MaxWeaponWeight = weaponWeight.MaxWeaponWeight;
            HeavierWeaponSmallerProbability = weaponWeight.HeavierWeaponSmallerProbability;
        }
    }

    private void EquipParamWeaponRequirement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var requirement = new WeaponRequirement(WeaponRequirement);
        if (requirement.ShowDialog() == true) WeaponRequirement = requirement.Requirement;
    }

    private void EquipParamWeaponScaling_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var correct = new WeaponCorrect(WeaponCorrect);
        if (correct.ShowDialog() == true) WeaponCorrect = correct.Scaling;
    }
}