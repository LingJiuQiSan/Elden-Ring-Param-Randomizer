using System.Windows;
using Elden_Ring_Param_Randomizer.Resources;

namespace Elden_Ring_Param_Randomizer.Settings
{
    /// <summary>
    /// WeaponWeight.xaml 的交互逻辑
    /// </summary>
    public partial class WeaponWeight
    {
        public float UserInput { get; set; }

        public WeaponWeight()
        {
            InitializeComponent();
            Title = Strings.Weapon_Weight_Setting;
            Description.Text = Strings.Max_weight_0_1_1000_0_;
            Confirm.Content = Strings.Confirm;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            UserInput = (float)Range.Value;
            DialogResult = true;
            Close();
        }
    }
}
