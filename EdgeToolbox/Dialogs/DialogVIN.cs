using DarkUI.Forms;
using EdgeDeviceLibrary;
using EdgeDeviceLibrary.Communicator;
using EdgeToolbox.Classes;
using System;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DialogVIN : DarkDialog {
		private string _vinString = string.Empty;

		private VINFormat _formatRequirements;

		public DialogVIN() {
            InitializeComponent();

			this.btnOk.Click += btnOk_Click;
			this.btnCancel.Click += btnCancel_Click;
        }

        private void btnOk_Click(object sender, EventArgs e) {
			if (ValidateVIN(txtVIN.Text)) {
				if (!VIN_Checksum(txtVIN.Text)) {
					MessageBox.Show(this, "The VIN you have entered appears to be incorrect. Please check your VIN against what you have entered.", "Invalid VIN");
					return;
				}
				_vinString = txtVIN.Text;
				base.DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			base.DialogResult = DialogResult.Cancel;
			Close();
		}

		public static string ShowForm(VINFormat formatRequirements, Form owner) {
			DialogVIN vINPromptForm = new DialogVIN();
			vINPromptForm._formatRequirements = formatRequirements;
			if (vINPromptForm.ShowDialog(owner) == DialogResult.OK) {
				return vINPromptForm._vinString;
			}
			return null;
		}

		private bool VIN_Checksum(string vin) {
			byte[] array = new byte[26] {
				1, 2, 3, 4, 5, 6, 7, 8, 0, 1, 2, 3, 4,
				5, 0, 7, 0, 9, 2, 3, 4, 5, 6, 7, 8, 9
			};
			byte[] array2 = new byte[17] {
				8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2
			};
			bool result = false;
			if (vin.Length == 17) {
				uint num = 0u;
				for (int i = 0; i < 17; i++) {
					num += (uint)((char.IsDigit(vin[i]) ? (vin[i] - 48) : array[vin[i] - 65]) * array2[i]);
				}
				num %= 11u;
				num = ((num == 10) ? 88u : (num + 48));
				result = num == vin[8];
			}
			return result;
		}

		private bool ValidateVIN(string vin) {
			if (_formatRequirements == null) {
				return true;
			}
			VINFormat.VINDigit[] digits = _formatRequirements.Digits;
			foreach (VINFormat.VINDigit vINDigit in digits) {
				if (vINDigit.Position >= vin.Length || !vINDigit.ValidateDigit(vin[vINDigit.Position])) {
					DarkMessageBox.ShowInformation(this, "The VIN you have entered appears to be incorrect. Please check your VIN against what you have entered.", "Invalid VIN");
					return false;
				}
			}
			return true;
		}

	}
}
