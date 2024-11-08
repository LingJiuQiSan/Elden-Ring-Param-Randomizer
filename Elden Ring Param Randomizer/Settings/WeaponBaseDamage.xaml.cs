using System.Windows;
using Elden_Ring_Param_Randomizer.Resources;

namespace Elden_Ring_Param_Randomizer.Settings;

public partial class WeaponBaseDamage
{
    public int[] AttackBase;

    public WeaponBaseDamage(int[] attackBase)
    {
        InitializeComponent();
        Title = Strings.Weapon_Base_Attack_Setting;
        AttackBasePhysics.Text = Strings.Physics;
        AttackBaseMagic.Text = Strings.Magic;
        AttackBaseFire.Text = Strings.Fire;
        AttackBaseLightning.Text = Strings.Lightning;
        AttackBaseHoly.Text = Strings.Holy;
        AttackBase = attackBase;
        PhysicsRange.Value = attackBase[0];
        MagicRange.Value = attackBase[1];
        FireRange.Value = attackBase[2];
        LightningRange.Value = attackBase[3];
        HolyRange.Value = attackBase[4];
    }

    private void Confirm_OnClick(object sender, RoutedEventArgs e)
    {
        AttackBase[0] = (int)PhysicsRange.Value;
        AttackBase[1] = (int)MagicRange.Value;
        AttackBase[2] = (int)FireRange.Value;
        AttackBase[3] = (int)LightningRange.Value;
        AttackBase[4] = (int)HolyRange.Value;
        DialogResult = true;
        Close();
    }
}