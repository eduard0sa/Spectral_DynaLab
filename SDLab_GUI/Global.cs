namespace SDLab_GUI
{
    public class Global
    {
        public enum enumVariableDataType
        {
            TYPE_INT,
            TYPE_FLOAT,
            TYPE_BOOL,
            TYPE_STRING,
            TYPE_SLIDER,
            TYPE_PICKER,
            TYPE_SWITCH,
            TYPE_DISTORTION_DSP_CLASS,
            TYPE_COMPRESSOR_DSP_CLASS,
            TYPE_CHORUS_DSP_CLASS,
            TYPE_REVERB_DSP_CLASS
        }

        public enum enumDSPType
        {
            DISTORTION,
            COMPRESSOR,
            REVERB,
            CHORUS,
            EQ,
            FILTER,
            GAIN
        }

        public struct structSliderData
        {
            public float minVal;
            public float maxVal;
            public float defVal;
            public int numDisplayDecPlaces;
        }

        public struct structPickerData
        {
            public int defValIndex;
            public List<string> items;
        }

        public struct structVariableDataTypeUnit
        {
            internal object dataUnit;
            internal enumVariableDataType dataType;
        }
    }
}