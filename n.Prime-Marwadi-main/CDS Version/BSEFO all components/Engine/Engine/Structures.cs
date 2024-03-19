namespace Engine
{
    public class Greeks
    {
        public double IV { get; set; } = 30;
        public double IVHigher { get; set; } = 45;
        public double IVLower { get; set; } = 15;
        public double Delta { get; set; } = 0;
        public double Theta { get; set; } = 0;
        public double Gamma { get; set; } = 0;
        public double Vega { get; set; } = 0;
        public bool IsReceived { get; set; } = false;
    }
}
