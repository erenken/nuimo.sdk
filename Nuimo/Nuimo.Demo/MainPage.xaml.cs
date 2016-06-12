using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Nuimo.Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		Nuimo.SDK.Nuimo _nuimo = new Nuimo.SDK.Nuimo();

		List<CheckBox> _checkBoxes = new List<CheckBox>(81);

        public MainPage()
        {
            this.InitializeComponent();

			for (int row = 0; row < 9; row ++)
			{
				for (int col = 0; col < 9; col ++)
				{
					CheckBox checkBox = new CheckBox();
					Grid.SetRow(checkBox, row);
					Grid.SetColumn(checkBox, col);

					uiLEDGrid.Children.Add(checkBox);

					
					_checkBoxes.Add(checkBox);
				}
			}
        }

		private async void uiConnectButton_Click(object sender, RoutedEventArgs e)
		{
			await _nuimo.Connect();

			uiConnectTextBox.Text = _nuimo.Connected ? "Connected" : "Disconnected";
			uiSetLEDMatrixButton.IsEnabled = true;
		}

		private async void uiSetLEDMatrixButton_Click(object sender, RoutedEventArgs e)
		{
			StringBuilder matrix = new StringBuilder();
			foreach(var checkbox in _checkBoxes)
			{
				if ((bool)checkbox.IsChecked)
					matrix.Append("*");
				else
					matrix.Append(" ");
			}

			await _nuimo.SetLEDMatrix(matrix.ToString());
		}
	}
}
