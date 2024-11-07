using System.Windows;
using Elden_Ring_Param_Randomizer.Resources;

namespace Elden_Ring_Param_Randomizer.Settings
{
    /// <summary>
    /// WeaponRequirement.xaml 的交互逻辑
    /// </summary>
    public partial class WeaponRequirement
    {

        public int[] Requirement = new int[5];

        public WeaponRequirement(int[] requirement)
        {
            InitializeComponent();
            Title = Strings.Weapon_Requirement_Setting;
            Strength.Text = Strings.Strength;
            Dexterity.Text = Strings.Dexterity;
            Intelligence.Text = Strings.Intelligence;
            Faith.Text = Strings.Faith;
            Arcane.Text = Strings.Arcane;
            Confirm.Content = Strings.Confirm;
            Requirement = requirement;
            StrengthRange.Value = requirement[0];
            DexterityRange.Value = requirement[1];
            IntelligenceRange.Value = requirement[2];
            FaithRange.Value = requirement[3];
            ArcaneRange.Value = requirement[4];
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            Requirement[0] = (int)StrengthRange.Value;
            Requirement[1] = (int)DexterityRange.Value;
            Requirement[2] = (int)IntelligenceRange.Value;
            Requirement[3] = (int)FaithRange.Value;
            Requirement[4] = (int)ArcaneRange.Value;
            DialogResult = true;
            Close();
        }
    }
}
