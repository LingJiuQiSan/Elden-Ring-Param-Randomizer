using System.Windows;
using Elden_Ring_Param_Randomizer.Resources;

namespace Elden_Ring_Param_Randomizer.Settings
{
    /// <summary>
    /// WeaponWeight.xaml 的交互逻辑
    /// </summary>
    public partial class WeaponWeight
    {
        public float MaxWeaponWeight { get; private set; }

        public WeaponWeight(float maxWeaponWeight)
        {
            InitializeComponent();
            Title = Strings.Weapon_Weight_Setting;
            Description.Text = Strings.Max_weight_0_1_1000_0_;
            Confirm.Content = Strings.Confirm;
            Range.Value = maxWeaponWeight;
            MaxWeaponWeight = maxWeaponWeight;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            MaxWeaponWeight = (float)Range.Value;
            DialogResult = true;
            Close();
        }
    }
}
