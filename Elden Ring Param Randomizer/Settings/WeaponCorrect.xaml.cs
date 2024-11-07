using System.Windows;
using Elden_Ring_Param_Randomizer.Resources;

namespace Elden_Ring_Param_Randomizer.Settings;

public partial class WeaponCorrect
{
    
    public float[] Scaling = new float[5];
    
    public WeaponCorrect()
    {
        InitializeComponent();
        Title = Strings.Weapon_Scaling_Setting;
        Strength.Text = Strings.Strength;
        Dexterity.Text = Strings.Dexterity;
        Intelligence.Text = Strings.Intelligence;
        Faith.Text = Strings.Faith;
        Arcane.Text = Strings.Arcane;
        Confirm.Content = Strings.Confirm;
    }

    private void Confirm_OnClick(object sender, RoutedEventArgs e)
    {
        Scaling[0] = (float)StrengthRange.Value;
        Scaling[1] = (float)DexterityRange.Value;
        Scaling[2] = (float)IntelligenceRange.Value;
        Scaling[3] = (float)FaithRange.Value;
        Scaling[4] = (float)ArcaneRange.Value;
        DialogResult = true;
        Close();
    }
}