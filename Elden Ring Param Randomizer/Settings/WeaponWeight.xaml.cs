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
        public bool HeavierWeaponSmallerProbability { get; private set; }

        public WeaponWeight(float maxWeaponWeight, bool heavierWeaponSmallerProbability)
        {
            InitializeComponent();
            Title = Strings.Weapon_Weight_Setting;
            Description.Text = Strings.Max_weight_0_1_1000_0_;
            Confirm.Content = Strings.Confirm;
            HeavierWeaponSmallerProbabilityCheckBox.Content =
                Strings.The_heavier_the_weapon__the_smaller_the_chance_of_a_random_hit_;
            Range.Value = maxWeaponWeight;
            MaxWeaponWeight = maxWeaponWeight;
            HeavierWeaponSmallerProbability = heavierWeaponSmallerProbability;
            HeavierWeaponSmallerProbabilityCheckBox.IsChecked = heavierWeaponSmallerProbability;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            MaxWeaponWeight = (float)Range.Value;
            HeavierWeaponSmallerProbability = HeavierWeaponSmallerProbabilityCheckBox.IsChecked ?? false;
            DialogResult = true;
            Close();
        }
    }
}
