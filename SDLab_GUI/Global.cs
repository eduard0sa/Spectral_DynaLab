namespace SDLab_GUI
{
    /// <summary>
    /// Globalally accessed members class.
    /// </summary>
    public class Global
    {
        /// <summary>
        /// Represents a dynamic datatype. Used with structVariableDataTypeUnit
        /// </summary>
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

        /// <summary>
        /// Represents a DSP SFX type.
        /// </summary>
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

        public enum enumWaveShapeType
        {
            Sine,
            Square,
            Triangle
        }

        public enum enumBaseColor
        {
            RED,
            YELLOW,
            GREEN
        }

        /// <summary>
        /// Stores slider numeric data.
        /// </summary>
        public struct structSliderData
        {
            public float minVal;
            public float maxVal;
            public float defVal;
            public int numDisplayDecPlaces;
        }

        /// <summary>
        /// Stores picker numeric data.
        /// </summary>
        public struct structPickerData
        {
            public int defValIndex;
            public List<string> items;
        }

        /// <summary>
        /// Stores an arbitrary dataType as an object type, registering the original data type via enumVariableDataType.
        /// </summary>
        public struct structVariableDataTypeUnit
        {
            internal object dataUnit;
            internal enumVariableDataType dataType;
        }
    }
}