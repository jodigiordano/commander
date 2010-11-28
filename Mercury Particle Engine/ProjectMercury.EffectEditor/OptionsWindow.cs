/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.EffectEditor
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class OptionsWindow : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsWindow"/> class.
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();
        }

        public Action<Color> BackgroundColourPickedCallback { get; set; }

        public Action<String> BackgroundImagePickedCallback { get; set; }

        public Action<ImageOptions> BackgroundImageOptionsCallback { get; set; }

        public Action BackgroundImageClearedCallback { get; set; }

        private void uxChangeBackgroundColour_Click(object sender, EventArgs e)
        {
            if (this.uxColourPicker.ShowDialog() == DialogResult.OK)
            {
                this.uxBackgroundColour.BackColor = this.uxColourPicker.Color;

                if (this.BackgroundColourPickedCallback != null)
                    this.BackgroundColourPickedCallback(this.uxColourPicker.Color);
            }
        }

        private void uxBrowseBackgroundImage_Click(object sender, EventArgs e)
        {
            if (this.uxBackgroundImageDialog.ShowDialog() == DialogResult.OK)
            {
                if (this.BackgroundImagePickedCallback != null)
                    this.BackgroundImagePickedCallback(this.uxBackgroundImageDialog.FileName);
            }
        }

        private void uxClearBackgroundImage_Click(object sender, EventArgs e)
        {
            if (this.BackgroundImageClearedCallback != null)
                this.BackgroundImageClearedCallback();
        }

        private void rbStretch_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BackgroundImageOptionsCallback != null)
                this.BackgroundImageOptionsCallback(ImageOptions.Stretch);
        }

        private void rbCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BackgroundImageOptionsCallback != null)
                this.BackgroundImageOptionsCallback(ImageOptions.Center);
        }

        private void rbTile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BackgroundImageOptionsCallback != null)
                this.BackgroundImageOptionsCallback(ImageOptions.Tile);
        }
    }
}
