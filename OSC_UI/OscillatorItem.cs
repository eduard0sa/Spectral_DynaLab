using AUDIOPROCBRIDGE;

namespace OSC_UI
{
    internal class OscillatorItem : Panel
    {
        private Panel OscillatorEntityHeadingPanel = new Panel();
        private Label OscillatorEntityHeader = new Label();
        private OscillatorItemKnobGroupPanel OscillatorKnobGroupPanel = new OscillatorItemKnobGroupPanel();
        private Panel DistortionFunctionDisplay = new Panel();

        public OscillatorItem(JuceAudioProvider oscEngine, AudioEngineRef _audioEngineRef)
        {
            this.BackColor = Color.FromArgb(35, 38, 47);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Location = new Point(0, 0);
            this.Size = new Size(776, 88);
            this.TabIndex = 1;

            OscillatorEntityHeadingPanel.BackColor = Color.FromArgb(33, 34, 40);
            OscillatorEntityHeadingPanel.Location = new Point(7, 7);
            OscillatorEntityHeadingPanel.Size = new Size(188, 72);
            OscillatorEntityHeadingPanel.TabIndex = 1;

            DistortionFunctionDisplay.BackColor = Color.FromArgb(33, 34, 40);
            DistortionFunctionDisplay.Location = new Point(572, 7);
            DistortionFunctionDisplay.Size = new Size(188, 72);
            DistortionFunctionDisplay.TabIndex = 1;

            OscillatorEntityHeader.AutoSize = true;
            OscillatorEntityHeader.Font = new Font("Orbitron SemiBold", 14.2499981F, FontStyle.Bold, GraphicsUnit.Point, 0);
            OscillatorEntityHeader.ForeColor = Color.FromArgb(146, 18, 13);
            OscillatorEntityHeader.Location = new Point(12, 25);
            OscillatorEntityHeader.Size = new Size(154, 24);
            OscillatorEntityHeader.TabIndex = 0;
            OscillatorEntityHeader.Text = "OSCILLATOR";

            OscillatorKnobGroupPanel.AddKnobItem(1, 0, 100, 50, "Frequency");
            OscillatorKnobGroupPanel.AddKnobItem(2, 0, 100, 50, "Gain");
            OscillatorKnobGroupPanel.OnSliderValueChangedFunction = (float _newVal_) =>
            {
                _audioEngineRef._changeFrequency(oscEngine.Engine, _newVal_);
            };

            OscillatorEntityHeadingPanel.Controls.Add(OscillatorEntityHeader);

            this.Controls.Add(OscillatorEntityHeadingPanel);
            this.Controls.Add(OscillatorKnobGroupPanel);
            this.Controls.Add(DistortionFunctionDisplay);
        }
    }

    internal class OscillatorItemKnobGroupPanel : Panel
    {
        List<OscillatorItemKnobItem> KnobItems = new List<OscillatorItemKnobItem>();

        public delegate void OnSliderValueChanged(float newVal);
        public OnSliderValueChanged OnSliderValueChangedFunction;

        public OscillatorItemKnobGroupPanel()
        {
            this.BackColor = Color.FromArgb(33, 34, 40);
            this.Location = new Point(282, 7);
            this.Size = new Size(283, 72);
            this.TabIndex = 2;
        }

        public void AddKnobItem(int index, float minVaL, float maxVal, float initVal, string knobLabelText)
        {
            OscillatorItemKnobItem knobItem = new OscillatorItemKnobItem(knobLabelText, minVaL, maxVal, initVal, index);

            knobItem.OnSliderValueChangedFunction = (float newVal) =>
            {
                OnSliderValueChangedFunction(newVal);
            };

            KnobItems.Add(knobItem);
            this.Controls.Add(knobItem);
        }
    }

    internal class OscillatorItemKnobItem : Panel
    {
        private TextBox KnobTB = new TextBox();
        private _Slider progressBar = new _Slider(136, 10, 96, 4, 50);
        private Label KnobLabel = new Label();

        public delegate void OnSliderValueChanged(float newVal);
        public OnSliderValueChanged OnSliderValueChangedFunction;

        public OscillatorItemKnobItem(string labelText, float minSliderValue, float maxSliderValue, float initVal, int index) {
            this.Location = new Point(3, 9 * index + 26 * (index - 1));
            this.Size = new Size(271, 24);
            this.TabIndex = 2;

            KnobTB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            KnobTB.Location = new Point(238, 1);
            KnobTB.Size = new Size(33, 23);
            KnobTB.TabIndex = 1;
            KnobTB.BackColor = Color.FromArgb(26, 27, 32);
            KnobTB.ForeColor = Color.FromArgb(146, 18, 13);

            KnobLabel.AutoSize = true;
            KnobLabel.Font = new Font("Orbitron", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            KnobLabel.ForeColor = Color.FromArgb(146, 18, 13);
            KnobLabel.Location = new Point(11, 4);
            KnobLabel.Size = new Size(78, 15);
            KnobLabel.TabIndex = 2;
            KnobLabel.Text = labelText;

            /*progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            progressBar.BackColor = Color.FromArgb(26, 27, 32);
            progressBar.ForeColor = Color.FromArgb(56, 7, 5);
            progressBar.Location = new Point(96, 1);
            progressBar.Size = new Size(136, 23);
            progressBar.TabIndex = 0;
            progressBar.Minimum = (int)minSliderValue;
            progressBar.Maximum = (int)maxSliderValue;
            progressBar.Value = (int)initVal;*/
            progressBar.OnSliderValueChangedFunction += (float newVal) =>
            {
                KnobTB.Text = progressBar.sliderValue.ToString();
                OnSliderValueChangedFunction(newVal);
            };

            this.Controls.Add(KnobLabel);
            this.Controls.Add(progressBar);
            this.Controls.Add(KnobTB);
        }
    }

    internal class _Slider : Panel
    {
        private Panel baseSliderPanel = new Panel();
        private Panel SliderProgressPanel = new Panel();
        private Panel SliderThumbPanel = new Panel();

        private bool isDragging = false;
        public float sliderValue = 0.0f;

        public delegate void OnSliderValueChanged(float newVal);
        public OnSliderValueChanged OnSliderValueChangedFunction;

        public _Slider(int width, int height, int posX, int posY, float initVal)
        {
            this.Size = new Size(width, height + 4);
            this.Location = new Point(posX, posY);
            this.BackColor = Color.FromArgb(26, 27, 32);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.Cursor = Cursors.Hand;

            baseSliderPanel.Size = new Size(width, height);
            baseSliderPanel.Location = new Point(0, 2);
            baseSliderPanel.BackColor = Color.FromArgb(26, 27, 32);

            SliderProgressPanel.Size = new Size((int)((initVal * width) / 100), height);
            SliderProgressPanel.Location = new Point(0, 2);
            SliderProgressPanel.BackColor = Color.FromArgb(146, 18, 13);

            SliderThumbPanel.Size = new Size(5, height + 4);
            SliderThumbPanel.Location = new Point(SliderProgressPanel.Width - 2, 0);
            SliderThumbPanel.BackColor = Color.FromArgb(146, 18, 13);
            SliderThumbPanel.MouseDown += SliderThumbPanel_MouseDown;
            SliderThumbPanel.MouseMove += SliderThumbPanel_MousePress;
            SliderThumbPanel.MouseUp += SliderThumbPanel_MouseUp;
            SliderThumbPanel.AllowDrop = true;

            sliderValue = initVal;

            this.Controls.Add(SliderThumbPanel);
            this.Controls.Add(SliderProgressPanel);
            this.Controls.Add(baseSliderPanel);
        }

        private void SliderThumbPanel_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
        }

        private void SliderThumbPanel_MousePress(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int newX = e.X + SliderThumbPanel.Left;
                newX = Math.Clamp(Math.Abs(e.X), 0, this.Width - 2);
                Console.WriteLine(newX);
                SliderThumbPanel.Location = new Point(newX, SliderThumbPanel.Location.Y);
                SliderProgressPanel.Width = newX + (SliderThumbPanel.Width / 2);
            }
        }

        private void SliderThumbPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            sliderValue = (float)SliderProgressPanel.Width / this.Width * 100;
            OnSliderValueChangedFunction(sliderValue);
        }
    }
}